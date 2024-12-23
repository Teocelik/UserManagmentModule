


using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagmentModule.DataAccess;
using UserManagmentModule.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//veritabanı baglantısını servis olarak container'a ekleyelim.(DI)
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnect")));

//Identity hizmetleri
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<AppDbContext>();


//Kullanıcı bilgilerini depolamak ve yönetmek için Identity ile gelen IUserStore ve IUserEmailStore'ı servis olarak ekleyelim
builder.Services.AddScoped<IUserStore<IdentityUser>, UserOnlyStore<IdentityUser, AppDbContext>>(); 
builder.Services.AddScoped<IUserEmailStore<IdentityUser>, UserOnlyStore<IdentityUser, AppDbContext>>();

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

//gelen isteklerin nasil karsilanacağini belirten yer!
app.MapDefaultControllerRoute();


app.Run();
