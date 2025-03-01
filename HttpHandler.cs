using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

public class HttpHandler
{
    // Define the root directory for static files
    private readonly string rootDirectory;
    // Dependency Injection for ILogger
    private readonly ILogger logger;

    // Constructor to receive ILogger instance
    public HttpHandler(ILogger logger)
    {
        this.logger = logger;
        rootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    }

    // Method to handle the client connection
    public void HandleClient(object clientObj)
    {
        // Cast the object back to TcpClient
        TcpClient client = (TcpClient)clientObj;

        try
        {
            // Get the network stream for reading and writing
            NetworkStream stream = client.GetStream();

            // Read the incoming request
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            // Log the request
            logger.Info($"Request Received:\n{request}");

            // Extract the requested URL
            string[] requestLines = request.Split("\r\n");
            string[] requestLine = requestLines[0].Split(" ");
            string url = requestLine[1];

            // Log the requested URL with timestamp
            logger.Info($"Requested URL: {url}");

            // Default to index.html if no specific file is requested
            if (url == "/")
            {
                url = "/index.html";
            }

            // Construct the file path
            string filePath = Path.Combine(rootDirectory, url.TrimStart('/'));

            // Check if the file exists
            if (File.Exists(filePath))
            {
                // Read the file contents
                byte[] fileBytes = File.ReadAllBytes(filePath);

                // Determine the Content-Type
                string contentType = GetContentType(filePath);

                // Prepare the HTTP response
                string responseHeader =
                    "HTTP/1.1 200 OK\r\n" +
                    $"Content-Type: {contentType}\r\n" +
                    $"Content-Length: {fileBytes.Length}\r\n" +
                    "Connection: close\r\n" +
                    "\r\n";

                // Send the response header
                byte[] headerBytes = Encoding.UTF8.GetBytes(responseHeader);
                stream.Write(headerBytes, 0, headerBytes.Length);

                // Send the file contents
                stream.Write(fileBytes, 0, fileBytes.Length);

                logger.Info($"Served: {url}");
            }
            else
            {
                // If the file is not found, send a 404 response
                string response =
                    "HTTP/1.1 404 Not Found\r\n" +
                    "Content-Type: text/html\r\n" +
                    "Connection: close\r\n" +
                    "\r\n" +
                    "<html><body><h1>404 - Not Found</h1></body></html>";

                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);

                // Log 404 errors
                logger.Warning($"404 Not Found: {url}");
            }
        }
        catch (Exception ex)
        {
            // Log exceptions
            logger.Error($"Error: {ex.Message}");
        }
        finally
        {
            // Close the connection
            client.Close();
        }
    }

    // Determine the Content-Type based on the file extension
    private string GetContentType(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLower();
        return extension switch
        {
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            _ => "application/octet-stream"
        };
    }
}
