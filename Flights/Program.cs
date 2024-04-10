using Flights.Data;
using Microsoft.OpenApi.Models;
using Flights.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

////Add Db Context
builder.Services.AddDbContext<Entity>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen( c=>
{
    c.DescribeAllParametersInCamelCase(); //Ova linija koda postavlja konfiguraciju za Swagger
   //kako bi sve parametre opisao koriste?i camelCase notaciju.
   //CamelCase je konvencija u programiranju gde se re?i spajaju bez razmaka,
   //a svaka slede?a re? osim prve po?inje velikim slovom. Na primjer, "camelCaseNotacija".
    c.AddServer(new OpenApiServer
        {
            Description = "Development Server",
            Url = "http://localhost:5036"
        });

    c.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["action"]  + e.ActionDescriptor.RouteValues["controller"]}");
    
});


builder.Services.AddScoped<Entity>(); // Example of adding a singleton service

var app = builder.Build();

var entity = app.Services.CreateScope().ServiceProvider.GetService<Entity>(); //vrši dohvatanje
//servisa (objekta) tipa 'Entity' iz kontejnera servisa (DI container-a) ASP.NET Core aplikacije.

entity.Database.EnsureCreated();

var random = new Random();

if (!entity.Flights.Any())
{
    //kreirani NIZ letova koji postoje u nasoj aplikaciji
    Flight[] flightsToSeed = new Flight[]
    {
       new (   Guid.NewGuid(),
                "American Airlines",
                random.Next(90, 5000).ToString(),
                new TimePlace("Los Angeles, USA",DateTime.Now.AddHours(random.Next(1, 3))),
                new TimePlace("Istanbul, Turkey",DateTime.Now.AddHours(random.Next(4, 10))),
                2),
        new (   Guid.NewGuid(),
                "Deutsche BA",
                random.Next(90, 5000).ToString(),
                new TimePlace("Munchen, Germany",DateTime.Now.AddHours(random.Next(1, 10))),
                new TimePlace("Schiphol",DateTime.Now.AddHours(random.Next(4, 15))),
                random.Next(1, 853)),
        new (   Guid.NewGuid(),
                "British Airways",
                random.Next(90, 5000).ToString(),
                new TimePlace("London, England",DateTime.Now.AddHours(random.Next(1, 15))),
                new TimePlace("Vizzola-Ticino",DateTime.Now.AddHours(random.Next(4, 18))),
                random.Next(1, 853)),
        new (   Guid.NewGuid(),
                "Basiq Air",
                random.Next(90, 5000).ToString(),
                new TimePlace("Amsterdam, Netherlands",DateTime.Now.AddHours(random.Next(1, 21))),
                new TimePlace("Glasgow, Scotland",DateTime.Now.AddHours(random.Next(4, 21))),
                random.Next(1, 853)),
        new (   Guid.NewGuid(),
                "BB Heliag",
                random.Next(90, 5000).ToString(),
                new TimePlace("Zurich, Sui",DateTime.Now.AddHours(random.Next(1, 23))),
                new TimePlace("Baku, Azerbaijan",DateTime.Now.AddHours(random.Next(4, 25))),
                random.Next(1, 853)),
        new (   Guid.NewGuid(),
                "Adria Airways",
                random.Next(90, 5000).ToString(),
                new TimePlace("Ljubljana, Slovenia",DateTime.Now.AddHours(random.Next(1, 15))),
                new TimePlace("Warsaw, Poland",DateTime.Now.AddHours(random.Next(4, 19))),
                random.Next(1, 853)),
        new (   Guid.NewGuid(),
                "ABA Air",
                random.Next(90, 5000).ToString(),
                new TimePlace("Belgrade, Serbia",DateTime.Now.AddHours(random.Next(1, 55))),
                new TimePlace("Paris, France",DateTime.Now.AddHours(random.Next(4, 58))),
                random.Next(1, 853)),
        new (   Guid.NewGuid(),
                "AB Corporate Aviation",
                random.Next(90, 5000).ToString(),
                new TimePlace("Le Bourget, France",DateTime.Now.AddHours(random.Next(1, 58))),
                new TimePlace("Zagreb, Croatin",DateTime.Now.AddHours(random.Next(4, 60))),
                random.Next(1, 853))
    };
    entity.Flights.AddRange(flightsToSeed);
    //kada nesto promenimo to moramo da sacuvamo
    entity.SaveChanges();
}

app.UseCors(builder => builder.WithOrigins("*")
.AllowAnyMethod()
.AllowAnyHeader()
);
app.UseSwagger().UseSwaggerUI();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
