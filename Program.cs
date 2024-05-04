using Angular_Project_Generator.Models.Model;
using Angular_Project_Generator.Services;
using Angular_Project_Generator.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

//Dependency Concepts:
builder.Services.AddScoped<IGenerateProjectService, GenerateProjectService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapPost("/download-app", async (AppConfiguration appConfiguration, IGenerateProjectService generateProjectService) =>
{
    var response = await generateProjectService.DownloadAngularProject(appConfiguration);
    return response;
});

app.MapPost("/generate-app", async (AppConfiguration appConfiguration, IGenerateProjectService generateProjectService) =>
{
    var res = await generateProjectService.GenerateAngularProject(appConfiguration);
    return res;
}).WithName("generate-app")
.WithOpenApi();

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
