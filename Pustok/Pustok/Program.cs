using Microsoft.EntityFrameworkCore;
using Pustok;
using Pustok.DAL;
using Pustok.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddDbContext<PustokDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

//builder.Services.AddSingleton<LayoutService>();
builder.Services.AddScoped<LayoutService>();
//builder.Services.AddTransient<LayoutService>();

builder.Services.AddSession(x =>
{
    x.IdleTimeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddHttpContextAccessor();


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
