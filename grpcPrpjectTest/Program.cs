using grpcPrpjectTest.Context;
using grpcPrpjectTest.Services;
using Microsoft.EntityFrameworkCore;

namespace grpcPrpjectTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer("Server=DESKTOP-OBAQKT7;Database=gRPCService;Trusted_Connection=True;TrustServerCertificate=true;"));

            // Add services to the container. 
            builder.Services.AddGrpc().AddJsonTranscoding();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<GreeterService>();
            app.MapGrpcService<TodoService>();

            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}