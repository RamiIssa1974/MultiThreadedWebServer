using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Build the Host
var host = Host.CreateDefaultBuilder(args)
               .ConfigureServices((context, services) =>
               {
                   // Register ILogger as a Singleton
                   services.AddSingleton<ILogger, Logger>();
                   // Register HttpHandler and HttpServer
                   services.AddSingleton<HttpHandler>();
                   services.AddSingleton<HttpServer>(provider => new HttpServer(8080));
               })
               .Build();

// Resolve and start the server
var server = host.Services.GetRequiredService<HttpServer>();
server.Start();
