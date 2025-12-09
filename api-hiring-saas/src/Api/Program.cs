using Api;
using Api.Middlewares;
using Application;
using Application.Common.Helpers;
using Infrastructure.Authentication;
using Infrastructure.Common.Extensions;
using Infrastructure.Identity.Configuration;
using Infrastructure.Localization;
using Infrastructure.Logging;
using Infrastructure.Payments;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddHttpContextAuthProvider();
builder.Services.AddConfigurationBoundOptions<WorkerAuthConfig>(WorkerAuthConfig.SectionName);
builder.Services.AddConfigurationBoundOptions<StripeConfig>(StripeConfig.SectionName);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, typeof(Program).Assembly);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddLocalization(builder.Configuration);
builder.Services.AddWebApiServices();

builder.Host.AddLogging();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
ServiceResolver.Initialize(app.Services);

app.UseCors("AllowAll");
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
    app.UseSwaggerUI();
//}

// Initialise and seed database
using (var scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
    await initialiser.InitialiseAsync();
}
app.UseMiddleware<LocalizationMiddleware>();
app.UseMediaDocumentStaticFiles(builder.Environment);

app.UseStaticFiles();

app.UseAuthorization();


app.MapControllers();
app.UseDocumentStaticFiles(builder.Environment);

app.Run();
