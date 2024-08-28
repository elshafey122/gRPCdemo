using grpcPrpjectTest.Context;
using grpcPrpjectTest.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace grpcPrpjectTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAuthentication("BasicAuth").AddScheme<AuthenticationSchemeOptions,BasicAuthenticationHandler>("BasicAuth", opt => { });

            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer("Server=DESKTOP-OBAQKT7;Database=gRPCService;Trusted_Connection=True;TrustServerCertificate=true;"));

            // Add services to the container. 
            builder.Services.AddGrpc().AddJsonTranscoding();  //used to convert grpc services into restful api endpoints 

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<GreeterService>(); //  add service of grbs to handle it
            app.MapGrpcService<TodoService>().RequireCors();    // to allow the cors i make to call 

            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled=true});   // to make anyclientweb like react and angular to call grpc services

            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");


            app.Run();
        }
    }
}