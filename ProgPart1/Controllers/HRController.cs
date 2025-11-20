using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProgPart1.Models;
using ProgPart1.ViewModel;
using static ProgPart1.Models.HR;

[Authorize(Roles = "HR")]
public class HRController : Controller
{
    private readonly UserManager<AppUser> _userManager;

    public HRController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult CreateUser() => View();

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserVM model)
    {
        var user = new AppUser
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            UserName = model.Email,
            HourlyRate = model.HourlyRate
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Lecturer");
            return RedirectToAction("UserList");
        }

        return View(model);
    }
    async Task CreateDefaultHRUser(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

        var hr = await userManager.FindByEmailAsync("admin@hr.com");

        if (hr == null)
        {
            hr = new AppUser
            {
                UserName = "admin@hr.com",
                Email = "admin@hr.com",
                FirstName = "System",
                LastName = "Admin"
            };

            await userManager.CreateAsync(hr, "Admin@123");
            await userManager.AddToRoleAsync(hr, "HR");
        }
    }

    await CreateDefaultHRUser(app.Services);

}
