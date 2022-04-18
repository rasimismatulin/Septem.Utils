using Microsoft.EntityFrameworkCore;
using Septem.Notifications.Core.Config;
using Serilog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddNotifications(options =>
{
    options.UseLoggerFactory(new SerilogLoggerFactory());
    options.UseNpgsql("Server=localhost;Port=5432;Uid=postgres;Pwd=Qwerty1;Database=NotificationTestDbIndexed",
        b => b.MigrationsAssembly("Septem.Notifications.Core"));
});


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
app.UseNotifications();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
