


using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagmentModule.DataAccess;
using UserManagmentModule.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//veritabaný baðlantýsýný servis olarak container'a ekleyelim.(DI)
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnect")));

//Identity hizmetlerini kullanmak için servis olarak container'a ekleyelim
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

//gelen isteklerin nasýl karþýlanacaðýný belirlenen yer!
app.MapDefaultControllerRoute();


app.Run();
