


using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagmentModule.DataAccess;
using UserManagmentModule.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//veritaban� ba�lant�s�n� servis olarak container'a ekleyelim.(DI)
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnect")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<AppDbContext>();

//Identity hizmetlerini kullanmak i�in servis olarak container'a ekleyelim
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

//gelen isteklerin nas�l kar��lanaca��n� belirlenen yer!
app.MapDefaultControllerRoute();


app.Run();
