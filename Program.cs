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
        HttpServer server = new HttpServer(port);
        server.Start();
    }
}
