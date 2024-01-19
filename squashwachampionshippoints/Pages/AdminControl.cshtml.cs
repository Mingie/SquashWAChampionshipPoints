using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using squashwachampionshippoints.Players;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace squashwachampionshippoints.Pages
{
    public class AdminControlModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public AdminControlModel(IConfiguration configuration)
        {
            _configuration = configuration;

            // Initialize the lists to avoid null reference exceptions
            MenPlayers = new List<PlayerInfo>();
            WomenPlayers = new List<PlayerInfo>();
        }

        public string DisplayedUsername { get; private set; }
        public string DisplayedPassword { get; private set; }
        public string EnteredUsername { get; private set; }
        public string EnteredPassword { get; private set; }

        // Properties for men and women players
        public List<PlayerInfo> MenPlayers { get; private set; }
        public List<PlayerInfo> WomenPlayers { get; private set; }

        public void OnGet(string username, string password)
        {
            // Retrieve correct credentials from appsettings.json
            DisplayedUsername = _configuration["AdminCredentials:Username"]?.Trim();
            DisplayedPassword = _configuration["AdminCredentials:Password"]?.Trim();

            // Retrieve entered credentials from the login page
            EnteredUsername = username;
            EnteredPassword = password;

            // Retrieve and populate MenPlayers and WomenPlayers from the database
            RetrievePlayersFromDatabase();
        }

        private void RetrievePlayersFromDatabase()
        {
            try
            {
                // Updated connection string without "Encrypt" and "Trust Server Certificate"
                String connectionString = "Data Source=HEDDWYNPC;Initial Catalog=playerregister;Integrated Security=True;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT Id, firstName, lastName, gender, championshipPoints FROM players";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PlayerInfo playerInfo = new PlayerInfo();
                                playerInfo.Id = reader.GetInt32(0);
                                playerInfo.firstName = reader.GetString(1);
                                playerInfo.lastName = reader.GetString(2);
                                playerInfo.gender = reader.GetString(3);
                                playerInfo.championshipPoints = reader.GetInt32(4);

                                // Log player information
                                //_logger.LogInformation($"Id: {playerInfo.Id}, First Name: {playerInfo.firstName}, Last Name: {playerInfo.lastName}, Gender: {playerInfo.gender}, Championship Points: {playerInfo.championshipPoints}");
                                
                                //MenPlayers.Add(playerInfo);

                                // Filter players by gender
                                if (playerInfo.gender == "Male")
                                {
                                    MenPlayers.Add(playerInfo);
                                }
                                else if (playerInfo.gender == "Female")
                                {
                                    WomenPlayers.Add(playerInfo);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                //_logger.LogError(ex, "An error occurred while retrieving players from the database.");
            }
        }


    }
}




