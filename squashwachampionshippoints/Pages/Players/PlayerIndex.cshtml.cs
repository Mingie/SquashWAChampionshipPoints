using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging; // Add this for logging
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace squashwachampionshippoints.Players
{
    public class PlayerIndexModel : PageModel
    {
        private readonly ILogger<PlayerIndexModel> _logger;

        // Inject an ILogger instance in the constructor
        public PlayerIndexModel(ILogger<PlayerIndexModel> logger)
        {
            _logger = logger;
        }

        public List<PlayerInfo> listPlayers = new List<PlayerInfo>();
        public PlayerInfo playerInfo = new PlayerInfo();

        public void OnGet()
        {
            RetrievePlayersFromDatabase();
        }

        public IActionResult OnPost()
        {
            try
            {
                // Check if all required fields are filled out
                if (string.IsNullOrEmpty(Request.Form["firstName"]) ||
                    string.IsNullOrEmpty(Request.Form["lastName"]) ||
                    string.IsNullOrEmpty(Request.Form["gender"]))
                {
                    // Return a bad request response with an error message
                    TempData["ErrorMessage"] = "All fields are required.";
                    return RedirectToPage("/Players/PlayerIndex");
                }

                // If all fields are filled out, proceed to create the PlayerInfo
                playerInfo.firstName = Request.Form["firstName"];
                playerInfo.lastName = Request.Form["lastName"];
                playerInfo.gender = Request.Form["gender"];
                // Assuming championshipPoints is an integer property in PlayerInfo class
                if (int.TryParse(Request.Form["0"], out int championshipPoints))
                {
                    playerInfo.championshipPoints = championshipPoints;
                }
                else
                {
                    // Handle the case where the form value is not a valid integer
                    playerInfo.championshipPoints = null; // or set a default value
                }


                // Save the playerInfo to the database
                SavePlayerToDatabase(playerInfo);

                // Return a success response
                TempData["SuccessMessage"] = "Player saved successfully";
                return RedirectToPage("/Players/PlayerIndex");
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while processing the form submission.");
                TempData["ErrorMessage"] = "An error occurred while saving player information.";
                return RedirectToPage("/Players/PlayerIndex");
            }
        }

        private void SavePlayerToDatabase(PlayerInfo playerInfo)
        {
            String connectionString = "Data Source=HEDDWYNPC;Initial Catalog=playerregister;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                String sql = "INSERT INTO players (firstName, lastName, gender, championshipPoints) " +
                             "VALUES (@firstName, @lastName, @gender, @championshipPoints);";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@firstName", playerInfo.firstName);
                    command.Parameters.AddWithValue("@lastName", playerInfo.lastName);
                    command.Parameters.AddWithValue("@gender", playerInfo.gender);
                    command.Parameters.AddWithValue("@championshipPoints", playerInfo.championshipPoints);

                    command.ExecuteNonQuery();
                }
            }
        }

        private void RetrievePlayersFromDatabase()
        {
            try
            {
                String connectionString = "Data Source=HEDDWYNPC;Initial Catalog=playerregister;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM players";

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

                                // Handle nullable int (championshipPoints)
                                if (!reader.IsDBNull(4))
                                {
                                    playerInfo.championshipPoints = reader.GetInt32(4);
                                }
                                else
                                {
                                    playerInfo.championshipPoints = null;
                                }

                                listPlayers.Add(playerInfo);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while retrieving player information from the database.");
            }
        }
    }

    public class PlayerInfo
    {
        public int Id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string gender { get; set; }
        public int? championshipPoints { get; set; }
    }

}



