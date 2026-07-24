using ClinicManagement.Application;
using ClinicManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Framework Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2. Add Swashbuckle Swagger
builder.Services.AddSwaggerGen();

// 3. Register Clean Architecture Layers
// builder.Services.AddApplication();
// builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddInfrastructure(builder.Configuration);
var app = builder.Build();

// 4. Configure Request Pipeline
if (app.Environment.IsDevelopment())
{
    // Serves OpenAPI spec and Swagger UI at http://localhost:<port>/swagger
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinic Management API v1");
    });
}

app.UseHttpsRedirection();

// Required if you add Authentication/Authorization down the line
app.UseAuthentication();
app.UseAuthorization();

// Route requests to Controller classes in the API/Contracts layer
app.MapControllers();

app.Run();