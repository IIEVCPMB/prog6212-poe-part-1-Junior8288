using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProgPart1.Context;
using ProgPart1.Models.HR;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Add DbContext FIRST
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2️⃣ Add Identity with AppUser class
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 3️⃣ Add MVC controllers and views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 4️⃣ Seed Roles + Default HR User
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRoles(services);
    await SeedHRUser(services);
}

// 5️⃣ Configure the pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();   
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


]
async Task SeedRoles(IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "HR", "Lecturer" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

async Task SeedHRUser(IServiceProvider services)
{
    var userManager = services.GetRequiredService<UserManager<AppUser>>();

    var hr = await userManager.FindByEmailAsync("admin@hr.com");

    if (hr == null)
    {
        hr = new AppUser
        {
            UserName = "admin@hr.com",
            Email = "admin@hr.com",
            FirstName = "System",
            LastName = "Admin",
            HourlyRate = 0
        };

        await userManager.CreateAsync(hr, "Admin@123");
        await userManager.AddToRoleAsync(hr, "HR");
    }
}
