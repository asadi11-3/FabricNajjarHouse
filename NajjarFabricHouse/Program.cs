

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NajjarFabricHouse.Data.DataBase;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // ????? ???? ???????


builder.Services.AddDbContext<AppDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("NajjarFabricHouse")));


builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=AccountPerson}/{id?}");

app.Run();
