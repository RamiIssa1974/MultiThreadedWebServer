using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

public static class HttpHandler
{
    // Define the root directory for static files
    private static readonly string rootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

    // Method to handle the client connection
    public static void HandleClient(object clientObj)
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

            // Display the request in the console
            Console.WriteLine("Request:");
            Console.WriteLine(request);

            // Extract the requested URL
            string[] requestLines = request.Split("\r\n");
            string[] requestLine = requestLines[0].Split(" ");
            string url = requestLine[1];

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

                Console.WriteLine($"Served: {url}");
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

                Console.WriteLine($"404 Not Found: {url}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            // Close the connection
            client.Close();
        }
    }

    // Determine the Content-Type based on the file extension
    private static string GetContentType(string filePath)
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
