using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using squashwachampionshippoints.Players;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Extensions.Logging; // Add this for logging


namespace squashwachampionshippoints.Pages.Players
{
    public class TournamentIndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TournamentIndexModel> _logger;

        public List<PlayerInfo> MenPlayers { get; private set; }
        public List<PlayerInfo> WomenPlayers { get; private set; }
        public List<TournamentInfo> listTournament = new List<TournamentInfo>();
        public TournamentInfo tournamentInfo = new TournamentInfo();


        public TournamentIndexModel(IConfiguration configuration, ILogger<TournamentIndexModel> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Initialize the lists to avoid null reference exceptions
            MenPlayers = new List<PlayerInfo>();
            WomenPlayers = new List<PlayerInfo>();
        }

        public void OnGet()
        {

            RetrievePlayersFromDatabase();
            RetrieveTournamentsFromDatabase();
        }


        public IActionResult OnPost()
        {
            try
            {
                // Check if the form is attempting to delete a player
                if (Request.Form.ContainsKey("Delete"))
                {
                    int playerIdToDelete;
                    if (int.TryParse(Request.Form["Delete"], out playerIdToDelete))
                    {
                        // Call the delete handler
                        //DeletePlayerFromDatabase(playerIdToDelete);

                        // Reload the page after deletion
                        return Page();
                    }
                }

                // Check if all required fields are filled out
                if (string.IsNullOrEmpty(Request.Form["date"]) ||
                    string.IsNullOrEmpty(Request.Form["tournamentName"]))

                {
                    // Return a bad request response with an error message
                    TempData["ErrorMessage"] = "All fields are required.";
                    return Page();
                }
                
                // If all fields are filled out, proceed to create the TournamentInfo
                tournamentInfo.date = DateTime.Parse(Request.Form["date"]);
                tournamentInfo.tournamentName = Request.Form["tournamentName"];
                tournamentInfo.level = Request.Form["level"];
                tournamentInfo.drawSize = int.Parse(Request.Form["drawSize"]);
                tournamentInfo.champion = Request.Form["champion"];
                tournamentInfo.runnerUp = Request.Form["runnerUp"];
                tournamentInfo.sf1 = Request.Form["sf1"].FirstOrDefault() ?? null;
                tournamentInfo.sf2 = Request.Form["sf2"].FirstOrDefault() ?? null;
                tournamentInfo.qf1 = Request.Form["qf1"].FirstOrDefault() ?? null;
                tournamentInfo.qf2 = Request.Form["qf2"].FirstOrDefault() ?? null;
                tournamentInfo.qf3 = Request.Form["qf3"].FirstOrDefault() ?? null;
                tournamentInfo.qf4 = Request.Form["qf4"].FirstOrDefault() ?? null;
                tournamentInfo.plateWinner = Request.Form["plateWinner"].FirstOrDefault() ?? null;





                // Save the playerInfo to the database
                SaveTournamentToDatabase();

                // Return a success response
                TempData["SuccessMessage"] = "Tournament saved successfully";
                RetrievePlayersFromDatabase();
                RetrieveTournamentsFromDatabase();
                return Page();
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while processing the form submission.");
                TempData["ErrorMessage"] = "An error occurred while saving tournament information.";
                RetrievePlayersFromDatabase();
                RetrieveTournamentsFromDatabase();
                return Page();
            }


        }

        public IActionResult OnGetEditor(int tournamentId)
        {
            _logger.LogInformation("OnPostEditor is being called");
            try
            {
                // Updated connection string without "Encrypt" and "Trust Server Certificate"
                string connectionString = "Data Source=HEDDWYNPC;Initial Catalog=playerregister;Integrated Security=True;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM tournaments WHERE id = @tournamentId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@tournamentId", tournamentId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // To display in DD/MM/YYYY format without time
                                string formattedDate = tournamentInfo.date.ToString("dd/MM/yyyy");

                                tournamentInfo.id = reader.GetInt32(0);
                                tournamentInfo.date = reader.GetDateTime(1).Date;
                                tournamentInfo.tournamentName = reader.GetString(2);
                                tournamentInfo.level = reader.GetString(3);
                                tournamentInfo.drawSize = reader.GetInt32(4);
                                tournamentInfo.champion = reader.GetString(5);
                                tournamentInfo.runnerUp = reader.GetString(6);
                                tournamentInfo.sf1 = reader.IsDBNull(7) ? null : reader.GetString(7);
                                tournamentInfo.sf2 = reader.IsDBNull(8) ? null : reader.GetString(8);
                                tournamentInfo.qf1 = reader.IsDBNull(9) ? null : reader.GetString(9);
                                tournamentInfo.qf2 = reader.IsDBNull(10) ? null : reader.GetString(10);
                                tournamentInfo.qf3 = reader.IsDBNull(11) ? null : reader.GetString(11);
                                tournamentInfo.qf4 = reader.IsDBNull(12) ? null : reader.GetString(12);
                                tournamentInfo.plateWinner = reader.IsDBNull(13) ? null : reader.GetString(13);


                                // Log tournament information
                                _logger.LogInformation($"id: {tournamentInfo.id}, Name: {tournamentInfo.tournamentName}");


                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while fetching tournament information by ID.");
                return null; // Handle the error gracefully based on your requirements
            }

            return null; // Handle the case where the tournament with the specified ID is not found
        }
    



        public IActionResult OnPostEdit()
        {

            _logger.LogInformation("OnEdit is being called");

            try
            {
                string connectionString = "Data Source=HEDDWYNPC;Initial Catalog=playerregister;Integrated Security=True;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlUpdate = "UPDATE tournaments " +
                                       "SET date = @date, tournamentName = @tournamentName, level = @level, drawSize = @drawSize, " +
                                       "champion = @champion, runnerUp = @runnerUp, sf1 = @sf1, sf2 = @sf2, qf1 = @qf1, " +
                                       "qf2 = @qf2, qf3 = @qf3, qf4 = @qf4, plateWinner = @plateWinner " +
                                       "WHERE id = @id";

                    using (SqlCommand command = new SqlCommand(sqlUpdate, connection))
                    {

                        command.Parameters.AddWithValue("@date", tournamentInfo.date.ToString("dd/MM/yyyy"));
                        command.Parameters.AddWithValue("@tournamentName", tournamentInfo.tournamentName);
                        command.Parameters.AddWithValue("@level", tournamentInfo.level);
                        command.Parameters.AddWithValue("@drawSize", tournamentInfo.drawSize);
                        command.Parameters.AddWithValue("@champion", tournamentInfo.champion);
                        command.Parameters.AddWithValue("@runnerUp", tournamentInfo.runnerUp);
                        command.Parameters.AddWithValue("@sf1", tournamentInfo.sf1 ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@sf2", tournamentInfo.sf2 ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@qf1", tournamentInfo.qf1 ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@qf2", tournamentInfo.qf2 ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@qf3", tournamentInfo.qf3 ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@qf4", tournamentInfo.qf4 ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@plateWinner", tournamentInfo.plateWinner ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@id", tournamentInfo.id);

                        command.ExecuteNonQuery();
                        // Log success
                        _logger.LogInformation("Tournament edited on the database successfully.");
                    }
                }

                RetrievePlayersFromDatabase();
                RetrieveTournamentsFromDatabase();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the tournament.");
                return Page(); // Handle the error appropriately, redirect to an error page, or display a user-friendly error message
            }
        }


        private void SaveTournamentToDatabase()
        {
            try
            {
                String connectionString = "Data Source=HEDDWYNPC;Initial Catalog=playerregister;Integrated Security=True;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "INSERT INTO tournaments (date, tournamentName, level, drawSize, champion, runnerUp, sf1, sf2, qf1, qf2, qf3, qf4, plateWinner) " +
                                 "VALUES (@date, @tournamentName, @level, @drawSize, @champion, @runnerUp, @sf1, @sf2, @qf1, @qf2, @qf3, @qf4, @plateWinner);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@date", tournamentInfo.date);
                        command.Parameters.AddWithValue("@tournamentName", tournamentInfo.tournamentName);
                        command.Parameters.AddWithValue("@level", tournamentInfo.level);
                        command.Parameters.AddWithValue("@drawSize", tournamentInfo.drawSize.ToString());
                        command.Parameters.AddWithValue("@champion", tournamentInfo.champion);
                        command.Parameters.AddWithValue("@runnerUp", tournamentInfo.runnerUp);
                        command.Parameters.Add("@sf1", SqlDbType.VarChar, 150).Value = (object)tournamentInfo.sf1 ?? DBNull.Value;
                        command.Parameters.Add("@sf2", SqlDbType.VarChar, 150).Value = (object)tournamentInfo.sf2 ?? DBNull.Value;
                        command.Parameters.Add("@qf1", SqlDbType.VarChar, 150).Value = (object)tournamentInfo.qf1 ?? DBNull.Value;
                        command.Parameters.Add("@qf2", SqlDbType.VarChar, 150).Value = (object)tournamentInfo.qf2 ?? DBNull.Value;
                        command.Parameters.Add("@qf3", SqlDbType.VarChar, 150).Value = (object)tournamentInfo.qf3 ?? DBNull.Value;
                        command.Parameters.Add("@qf4", SqlDbType.VarChar, 150).Value = (object)tournamentInfo.qf4 ?? DBNull.Value;
                        command.Parameters.Add("@plateWinner", SqlDbType.VarChar, 150).Value = (object)tournamentInfo.plateWinner ?? DBNull.Value;

                        command.ExecuteNonQuery();

                        // Log success
                        _logger.LogInformation("Tournament saved and NOT EDITED.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while saving tournament information.");
                throw; // Rethrow the exception to propagate it up the stack
            }
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
                _logger.LogError(ex, "An error occurred while processing the form submission.");

            }
        }

        private void RetrieveTournamentsFromDatabase()
        {
            try
            {
                // Updated connection string without "Encrypt" and "Trust Server Certificate"
                String connectionString = "Data Source=HEDDWYNPC;Initial Catalog=playerregister;Integrated Security=True;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM tournaments";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TournamentInfo tournamentInfo = new TournamentInfo();
                                tournamentInfo.id = reader.GetInt32(0);
                                tournamentInfo.date = reader.GetDateTime(1).Date;
                                tournamentInfo.tournamentName = reader.GetString(2);
                                tournamentInfo.level = reader.GetString(3);
                                tournamentInfo.drawSize = reader.GetInt32(4);
                                tournamentInfo.champion = reader.GetString(5);
                                tournamentInfo.runnerUp = reader.GetString(6);
                                tournamentInfo.sf1 = reader.IsDBNull(7) ? null : reader.GetString(7);
                                tournamentInfo.sf2 = reader.IsDBNull(8) ? null : reader.GetString(8);
                                tournamentInfo.qf1 = reader.IsDBNull(9) ? null : reader.GetString(9);
                                tournamentInfo.qf2 = reader.IsDBNull(10) ? null : reader.GetString(10);
                                tournamentInfo.qf3 = reader.IsDBNull(11) ? null : reader.GetString(11);
                                tournamentInfo.qf4 = reader.IsDBNull(12) ? null : reader.GetString(12);
                                tournamentInfo.plateWinner = reader.IsDBNull(13) ? null : reader.GetString(13);

                                // To display in DD/MM/YYYY format without time
                                string formattedDate = tournamentInfo.date.ToString("dd/MM/yyyy");

                                // Log player information
                                //_logger.LogInformation($"Id: {tournamentInfo.Id}, Name: {tournamentInfo.tournamentName}");

                                listTournament.Add(tournamentInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while processing the form submission.");

            }
        }

        public class TournamentInfo
        {
            public int id { get; set; }
            public DateTime date { get; set; }
            public string tournamentName { get; set; }
            public string level { get; set; }
            public int drawSize { get; set; }

            // Participants tournament
            public string champion { get; set; }
            public string runnerUp { get; set; }
            public string sf1 { get; set; }
            public string sf2 { get; set; }
            public string qf1 { get; set; }
            public string qf2 { get; set; }
            public string qf3 { get; set; }
            public string qf4 { get; set; }
            public string plateWinner { get; set; }
        }

        public IActionResult OnPostDelete(int id)
        {
            try
            {
                // Delete the tournament with the specified ID from the database
                DeleteTournamentFromDatabase(id);

                // Return a success response
                TempData["SuccessMessage"] = "Tournament deleted successfully";
                RetrievePlayersFromDatabase();
                RetrieveTournamentsFromDatabase();
                return Page();
            }
            catch (Exception ex)
            {
                // Log the exception
                //_logger.LogError(ex, "An error occurred while deleting the player from the database.");
                TempData["ErrorMessage"] = "An error occurred while deleting the tournament.";
                RetrievePlayersFromDatabase();
                RetrieveTournamentsFromDatabase();
                return Page();
            }
        }

        private void DeleteTournamentFromDatabase(int tournamentId)
        {
            String connectionString = "Data Source=HEDDWYNPC;Initial Catalog=playerregister;Integrated Security=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                String sql = "DELETE FROM tournaments WHERE Id = @tournamentId;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@tournamentId", tournamentId);
                    command.ExecuteNonQuery();
                }
            }
            // Sample code to delete from the listPlayers collection for demonstration purposes
            var playerToDelete = listTournament.FirstOrDefault(p => p.id == tournamentId);
            if (playerToDelete != null)
            {
                listTournament.Remove(playerToDelete);
            }
        }

    }
}

