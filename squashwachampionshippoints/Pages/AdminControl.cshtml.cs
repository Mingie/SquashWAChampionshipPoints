using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace squashwachampionshippoints.Pages
{
    public class AdminControlModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public AdminControlModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string DisplayedUsername { get; private set; }
        public string DisplayedPassword { get; private set; }
        public string EnteredUsername { get; private set; }
        public string EnteredPassword { get; private set; }

        public void OnGet(string username, string password)
        {
            // Retrieve correct credentials from appsettings.json
            DisplayedUsername = _configuration["AdminCredentials:Username"]?.Trim();
            DisplayedPassword = _configuration["AdminCredentials:Password"]?.Trim();

            // Retrieve entered credentials from the login page
            EnteredUsername = username;
            EnteredPassword = password;
        }
    }
}

