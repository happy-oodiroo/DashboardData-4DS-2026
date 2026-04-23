using DashboardData.Components;
using DashboardData.Data;
using DashboardData.Models;
using DashboardData.Services;
using Microsoft.EntityFrameworkCore;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//===== Configure Entity Framework Core with SQLite ======

// Get the connection string from appsettings.json and configure the DbContext to use SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// This registers the AppDbContext with the dependency injection system, so it can be injected into components and other services that need to access the database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
//================================================================================


// Register the SensorService as a scoped service, so it can be injected into components and other services
builder.Services.AddScoped<ISensorService, SensorService>();
builder.Services.AddRadzenComponents();

var app = builder.Build();

//===== Generate test data if the database is empty ======
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider; // Get the service provider for the current scope, which allows us to resolve services that are registered with the dependency injection system
    var dbContext = services.GetRequiredService<AppDbContext>();  // Resolve the AppDbContext from the service provider, which will be used to interact with the database

    if (! dbContext.Sensors.Any())
    {
        Console.WriteLine("--- Generating test data ---");
        // 1. Create some sample locations
        var lab = new Location { Name = "Lab 1", Building = "Bldg. A" };
        var factory = new Location { Name = "Factory", Building = "Bldg. B" };
       
        dbContext.AddRange(lab, factory);   // Add the sample locations to the DbContext, which marks them for insertion into the database when SaveChanges is called
        
        dbContext.SaveChanges(); // Save the changes to the database, which will insert the new records for locations into the database and assign them primary key values (Ids) that can be used for relationships with sensors

        // 2. Create some sample tags
        var tagCritical = new Tag { Label = "Critical" };
        var tagMaintenance = new Tag { Label = "Maintenance" };
        
        dbContext.AddRange(tagCritical, tagMaintenance);  // Add the sample tags to the DbContext, which marks them for insertion into the database when SaveChanges is called
        
        dbContext.SaveChanges();// Save the changes to the database, which will insert the new records for tags into the database and assign them primary key values (Ids) that can be used for relationships with sensors

        // 3. Create some sample sensors and associate them with locations and tags
        var sensor1 = new SensorData
        {
            Name = "Sensor_Alpha",
            Value = 25.4,
            LocationId = lab.Id,
            Tags = new List<Tag> { tagCritical }
        };
        var sensor2 = new SensorData
        {
            Name = "Sensor_Beta",
            Value = 40.2,
            LocationId = factory.Id,
            Tags = new List<Tag> { tagMaintenance }
        };

        dbContext.Sensors.AddRange(sensor1, sensor2); // Add the sample sensors to the DbContext
        dbContext.SaveChanges(); // Save the changes to the database, which will insert the new records for locations, tags, and sensors

    }
}
//================================================================================

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
