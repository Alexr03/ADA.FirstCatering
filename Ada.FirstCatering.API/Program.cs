using System.Text.Json.Serialization;
using Ada.FirstCatering.API;
using Ada.FirstCatering.API.Filters;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var configuration = builder.Configuration;
builder.Services.AddControllers(options => { options.Filters.Add<BaseResponseFilter>(); })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FirstCateringContext>(options =>
{
    options.EnableDetailedErrors();
    options.EnableSensitiveDataLogging();

    var connectionString = configuration.GetConnectionString("FirstCatering");
    if (connectionString.StartsWith("Server="))
    {
        options.UseMySql(connectionString,
            MariaDbServerVersion.LatestSupportedServerVersion,
            y =>
            {
                y.EnableRetryOnFailure(5);
                y.CommandTimeout(30);
                y.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
    }
    else
    {
        options.UseSqlite(connectionString, optionsBuilder => { optionsBuilder.CommandTimeout(30); });
    }
}, ServiceLifetime.Transient);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.InjectStylesheet("/swagger-ui/SwaggerDark.css"); });
}

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var serviceScope = app.Services.CreateScope())
{
    var firstCateringContext = serviceScope.ServiceProvider.GetRequiredService<FirstCateringContext>();
    firstCateringContext.Database.EnsureCreated();
    firstCateringContext.Database.Migrate();
}

app.Run();