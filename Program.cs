using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecureNetworks.DataBaseContext;
using SecureNetworks.Helpers;
using SecureNetworks.Models.DBModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add session services
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "IsAdminUserCookie";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

// Add ApplicationDbContext to the services
builder.Services.AddDbContext<SecureNetworkDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SecureNetworksDB")));

// Add Identity services
builder.Services.AddIdentity<ApplicationUserDBEntity, IdentityRole>()
    .AddEntityFrameworkStores<SecureNetworkDBContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserServiceHepler>();

builder.Services.AddAuthorization();

var app = builder.Build();

// Apply any pending migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<SecureNetworkDBContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Handle any errors
        Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use session middleware
app.UseSession();

// Use authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();