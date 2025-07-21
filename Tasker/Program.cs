using Tasker.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAppDbContext(builder.Configuration)
    .AddAppServices()
    .AddAppAuthentication(builder.Configuration)
    .AddAppSwagger()
    .AddAppValidation();

builder.Services.AddMemoryCache();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

public partial class Program { }
