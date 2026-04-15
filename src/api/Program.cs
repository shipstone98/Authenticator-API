using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder();
builder.Services.AddControllers();
WebApplication app = builder.Build();
app.MapControllers();
await app.RunAsync();
return 0;
