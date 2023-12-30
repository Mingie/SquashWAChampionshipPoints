using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace squashwachampionshippoints.Pages
{
    public class AdminLoginModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminLoginModel> _logger;

        public AdminLoginModel(IConfiguration configuration, ILogger<AdminLoginModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnGet()
        {
            // This is the handler for the initial GET request to the page.
            return Page();
        }

        public IActionResult OnPost()
        {
            string correctUsername = _configuration["AdminCredentials:Username"]?.Trim();
            string correctPassword = _configuration["AdminCredentials:Password"]?.Trim();

            // Validate entered username and password
            if (Username?.Trim() == correctUsername && Password?.Trim() == correctPassword)
            {
                // Successful login, redirect to AdminControlModel page.
                return RedirectToPage("/AdminControl");
            }
            else
            {
                // Incorrect credentials, return to the login page with an error message.
                TempData["ErrorMessage"] = "Invalid username or password.";
                return Page();
            }
        }
    }
}
