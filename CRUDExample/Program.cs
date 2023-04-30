using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts.Interfaces;
using Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

//add services into ioc controller
builder.Services.AddSingleton<ICountriesService, CountriesService>();
builder.Services.AddSingleton<IPersonService, PersonService>();

//add DbContext File
builder.Services.AddDbContext<PersonsDbContext>(options=>options.UseSqlServer());

var app = builder.Build();

if(builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
