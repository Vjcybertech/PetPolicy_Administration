using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using PolicyAdmin.API.Data;
using PolicyAdmin.API.Middleware;
using PolicyAdmin.API.Repository;
using PolicyAdmin.API.Services;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=policyadmin.db";

builder.Services.AddDbContext<PolicyDbContext>(options =>
    options.UseSqlite(connectionString));

// Register Application Services
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<IQuoteService, QuoteService>();
builder.Services.AddScoped<IExcelService, ExcelService>();

//Repository
builder.Services.AddScoped<IpolicyRepository,PolicyRepository>();
builder.Services.AddScoped<IExcelRepository,ExcelRepository>();
builder.Services.AddScoped<IQuoteRepository,QuoteRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Policy Administration Web API",
        Version = "v1",
        Description = "ASP.NET Core Web API for Pet Insurance Policy Administration"
    });
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PolicyDbContext>();
    dbContext.Database.EnsureCreated();
    PolicyDbSeeder.Seed(dbContext);
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Policy Admin API");
    c.RoutePrefix = string.Empty;
    c.DefaultModelsExpandDepth(-1);
});

app.UseAuthorization();
app.MapControllers();
app.Run();