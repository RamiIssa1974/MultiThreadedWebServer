using System;
using System.Net.Sockets;
using System.Text;

public static class HttpHandler
{
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

            // Prepare the HTTP response
            string htmlResponse = "<html><body><h1>Hello from MultiThreaded Web Server!</h1></body></html>";
            string response =
                "HTTP/1.1 200 OK\r\n" +
                "Content-Type: text/html\r\n" +
                $"Content-Length: {Encoding.UTF8.GetByteCount(htmlResponse)}\r\n" +
                "Connection: close\r\n" +
                "\r\n" +
                htmlResponse;

            // Convert the response to bytes and send it
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            stream.Write(responseBytes, 0, responseBytes.Length);

            Console.WriteLine("Response sent. Closing connection.");
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
}
