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
using static squashwachampionshippoints.Pages.Players.TournamentIndexModel;

namespace squashwachampionshippoints.Pages.Players
{
    public class EditPageModel : PageModel
    {
        public TournamentInfo tournamentInfo = new TournamentInfo();
        public List<PlayerInfo> MenPlayers { get; private set; }
        public List<PlayerInfo> WomenPlayers { get; private set; }

        public void OnGet()
        {
            String tournamentID = Request.Query["id"];
            try
            {
                // Updated connection string without "Encrypt" and "Trust Server Certificate"
                string connectionString = "Data Source=HEDDWYNPC;Initial Catalog=playerregister;Integrated Security=True;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM tournaments WHERE id = @tournamentId";
                    string sqlPlayers = "SELECT Id, firstName, lastName, gender, championshipPoints FROM players";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        //command.Parameters.AddWithValue("@id", id);

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
                                //_logger.LogInformation($"id: {tournamentInfo.id}, Name: {tournamentInfo.tournamentName}");


                            }
                        }
                    }
                    using (SqlCommand command = new SqlCommand(sqlPlayers, connection))
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
                //_logger.LogError(ex, "An error occurred while fetching tournament information by ID.");
                //return null; // Handle the error gracefully based on your requirements
            }

            //return null; // Handle the case where the tournament with the specified ID is not found
        }

        public void OnPost()
        {

            //_logger.LogInformation("OnEdit is being called");
            tournamentInfo.id = int.TryParse(Request.Form["id"], out int parsedId) ? parsedId : 0;
            tournamentInfo.date = DateTime.Parse(Request.Form["date"]);
            tournamentInfo.tournamentName = Request.Form["tournamentName"];
            tournamentInfo.level = Request.Form["level"];
            tournamentInfo.drawSize = int.TryParse(Request.Form["drawSize"], out int drawSize) ? drawSize : 0; // Default value is used if parsing fails
            tournamentInfo.champion = Request.Form["champion"];
            tournamentInfo.runnerUp = Request.Form["runnerUp"];
            tournamentInfo.sf1 = Request.Form["sf1"];
            tournamentInfo.sf2 = Request.Form["sf2"];
            tournamentInfo.qf1 = Request.Form["qf1"];
            tournamentInfo.qf2 = Request.Form["qf2"];
            tournamentInfo.qf3 = Request.Form["qf3"];
            tournamentInfo.qf4 = Request.Form["qf4"];
            tournamentInfo.plateWinner = Request.Form["plateWinner"];


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
                        //_logger.LogInformation("Tournament edited on the database successfully.");
                    }
                }

                Response.Redirect("/Players/TournamentIndex");
                //return Page();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An error occurred while updating the tournament.");
                //return Page(); // Handle the error appropriately, redirect to an error page, or display a user-friendly error message
            }
        }
    }

    
}
