using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main()
    {
        // Define the port to listen on
        int port = 8080;
        // Create a TCP Listener to listen for incoming connections
        TcpListener listener = new TcpListener(IPAddress.Any, port);

        Console.WriteLine($"Starting server on port {port}...");
        listener.Start();

        // Listen indefinitely for incoming connections
        while (true)
        {
            Console.WriteLine("Waiting for a connection...");

            // Accept an incoming client connection
            using (TcpClient client = listener.AcceptTcpClient())
            {
                Console.WriteLine("Client connected.");

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
                string htmlResponse = string.Format("<html><body><h1>Hello from MultiThreaded Web Server!</h1>" +
                    "<h2>{0}</h2>"+
                    "</body></html>", DateTime.Now);
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
        }
    }
}
