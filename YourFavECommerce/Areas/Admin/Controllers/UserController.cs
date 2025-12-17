using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Threading.Tasks;
using YourFavECommerce.Models;
using YourFavECommerce.Utilites;
using YourFavECommerce.ViewModels;

namespace YourFavECommerce.Areas.Admin.Controllers
{
    //[Route("[area]/[controller]")]
    //[ApiController]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }

        public IActionResult Index(string? name, int page = 1)
        {
            var users = _userManager.Users.AsQueryable();

            if (name is not null)
                users = users.Where(e => e.Name.ToLower().Contains(name.ToLower().Trim()));

            double totalPages = Math.Ceiling(users.Count() / 5.0);
            int currentPage = page;
            users = users.Skip((page - 1) * 5).Take(5);

            return View(new UserWithRelatedVM
            {
                ApplicationUsers = users.ToList(),
                CurrentPage =  currentPage,
                TotalPages = totalPages
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            var roles = _roleManager.Roles.AsQueryable();

            return View(roles.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserVM createUserVM)
        {
            ApplicationUser user = new()
            {
                Name = createUserVM.name,
                Email = createUserVM.email,
                PhoneNumber = createUserVM.phoneNumber,
                UserName = createUserVM.userName,
                EmailConfirmed = createUserVM.EmailConfirmation
            };

            var result = await _userManager.CreateAsync(user, createUserVM.password);

            if (!result.Succeeded)
            {
                // Print Errors
                TempData["error-notification"] = result.Errors.Select(e => e.Description);

                return View(createUserVM);
            }

            if (!createUserVM.EmailConfirmation)
            {
                // Send Email Confirmation
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action("Confirm", "Account", new { area = "Identity", user.Id, token }, Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Pleas Confirm Your Account In Ecommerce Code Academy App",
                    $"<h1>Please Confirm You Account By clicking <a href='{link}'>Here</a></h1>");
            }

            foreach (var item in createUserVM.Roles)
                await _userManager.AddToRoleAsync(user, item);

            // Print Success msg
            TempData["success-notification"] = "Add Account Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = _userManager.Users.FirstOrDefault(e => e.Id == id);

            if (user is null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            var roles = _roleManager.Roles.AsQueryable();

            return View(new UserRolesVM
            {
                UserId = user.Id,
                UserName = user.UserName,
                Name = user.Name,
                Email = user.Email,
                UserRoles = userRoles.ToList(),
                Roles = roles.ToList(),
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateUserRoleVM updateUserRoleVM)
        {
            var user = _userManager.Users.FirstOrDefault(e => e.Id == updateUserRoleVM.UserId);

            if (user is null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, userRoles);

            await _userManager.AddToRolesAsync(user, updateUserRoleVM.Roles);

            TempData["success-notification"] = "Change Roles Successfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> LockUnLock(string id)
        {
            var user = _userManager.Users.FirstOrDefault(e => e.Id == id);

            if (user is null) return NotFound();

            user.LockoutEnabled = !user.LockoutEnabled;

            if(user.LockoutEnabled)
                user.LockoutEnd = null;
            else
                user.LockoutEnd = DateTime.Now.AddDays(10);

            await _userManager.UpdateAsync(user);

            TempData["success-notification"] = "Change Status Successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
