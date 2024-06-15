using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SampleWebApp.Model
{
    public class DAL
    {
        private readonly IConfiguration _configuration;
        private readonly string keyVaultUrl = "https://eastuskeyvault01.vault.azure.net/"; // e.g., https://<YourKeyVaultName>.vault.azure.net
        private readonly string secretName = "<azuresql>";

        public DAL(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task<string> GetConnectionStringFromKeyVaultAsync()
        {
            var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
            KeyVaultSecret secret = await client.GetSecretAsync(secretName);
            return secret.Value;
        }

        public async Task<List<User>> GetUsers()
        {
            List<User> users = new List<User>();
            string connectionString = await GetConnectionStringFromKeyVaultAsync();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM TblUsers", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        User user = new User();
                        user.Id = Convert.ToString(dt.Rows[i]["Id"]);
                        user.FirstName = Convert.ToString(dt.Rows[i]["FirstName"]);
                        user.LastName = Convert.ToString(dt.Rows[i]["LastName"]);
                        users.Add(user);
                    }
                }
            }
            return users;
        }

        public async Task<int> AddUser(User user)
        {
            int i = 0;
            string connectionString = await GetConnectionStringFromKeyVaultAsync();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO TblUsers VALUES(@FirstName, @LastName)", con);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                con.Open();
                i = cmd.ExecuteNonQuery();
                con.Close();
            }
            return i;
        }

        public async Task<User> GetUser(string id)
        {
            User user = new User();
            string connectionString = await GetConnectionStringFromKeyVaultAsync();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM TblUsers WHERE ID = @ID", con);
                da.SelectCommand.Parameters.AddWithValue("@ID", id);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    user.Id = Convert.ToString(dt.Rows[0]["Id"]);
                    user.FirstName = Convert.ToString(dt.Rows[0]["FirstName"]);
                    user.LastName = Convert.ToString(dt.Rows[0]["LastName"]);
                }
            }
            return user;
        }

        public async Task<int> UpdateUser(User user)
        {
            int i = 0;
            string connectionString = await GetConnectionStringFromKeyVaultAsync();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("Update TblUsers SET FirstName = @FirstName, LastName = @LastName WHERE ID = @ID", con);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@ID", user.Id);
                con.Open();
                i = cmd.ExecuteNonQuery();
                con.Close();
            }
            return i;
        }

        public async Task<int> DeleteUser(string id)
        {   
            int i = 0;
            string connectionString = await GetConnectionStringFromKeyVaultAsync();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM TblUsers WHERE ID = @ID", con);
                cmd.Parameters.AddWithValue("@ID", id);
                con.Open();
                i = cmd.ExecuteNonQuery();
                con.Close();
            }
            return i;
        }
    }
}
