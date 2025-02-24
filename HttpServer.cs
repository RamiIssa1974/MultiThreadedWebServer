 using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class HttpServer
{
    private readonly int port;
    private readonly TcpListener listener;

    public HttpServer(int port)
    {
        this.port = port;
        listener = new TcpListener(IPAddress.Any, port);
    }

    // Start the server
    public void Start()
    {
        Console.WriteLine($"Starting multi-threaded server on port {port}...");
        listener.Start();

        // Listen indefinitely for incoming connections
        while (true)
        {
            Console.WriteLine("Waiting for a connection...");

            // Accept an incoming client connection
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected.");

            // Use ThreadPool to handle the client connection
            ThreadPool.QueueUserWorkItem(HttpHandler.HandleClient, client);
        }
    }
}
