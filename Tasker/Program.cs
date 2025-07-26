using Serilog;
using Tasker.Extensions;
using Tasker.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAppDbContext(builder.Configuration)
    .AddAppServices()
    .AddAppAuthentication(builder.Configuration)
    .AddAppSwagger()
    .AddAppValidation();

/*
Serilog.Debugging.SelfLog.Enable(msg =>
{
    File.AppendAllText("serilog-selflog.txt", msg);
});
*/

Log.Logger = SerilogConfigurator.Configure(builder.Configuration).CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddMemoryCache();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ErrorLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception during app run");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
