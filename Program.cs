using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", (string? message) => message ?? "Hello World!");
app.MapGet("/hash", (string? message) => Convert.ToBase64String(SHA384.HashData(Encoding.UTF8.GetBytes(message ?? "Hello World!"))));
app.MapGet("/env", () => new EnvironmentInfo());

app.Run();
