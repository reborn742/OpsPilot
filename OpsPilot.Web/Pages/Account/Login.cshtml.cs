using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpsPilot.Infrastructure.Identity;

namespace OpsPilot.Web.Pages.Account;

public sealed class LoginModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LoginModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [BindProperty]
    public LoginInputModel Input { get; set; } = new();

    [BindProperty]
    public string? ReturnUrl { get; set; }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : "/";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var email = Input.Email.Trim();

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null || !user.IsActive)
        {
            ModelState.AddModelError(
                string.Empty,
                "Invalid email or password.");

            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            Input.Password,
            Input.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            var destination = Url.IsLocalUrl(ReturnUrl)
                ? ReturnUrl!
                : "/";

            return LocalRedirect(destination);
        }

        ModelState.AddModelError(
            string.Empty,
            result.IsLockedOut
                ? "Your account is temporarily locked. Try again later."
                : "Invalid email or password.");

        return Page();
    }

    public sealed class LoginInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
