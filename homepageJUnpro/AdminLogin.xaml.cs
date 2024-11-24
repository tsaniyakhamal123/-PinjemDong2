using System;
using System.Windows;
using Npgsql;
using BCrypt.Net;

namespace homepageJUnpro
{
    public partial class AdminLogin : Window
    {
        // Connection string to PostgreSQL database
        private string connstring = "Host=postgres-junpro.cpm48umoy5cj.ap-southeast-2.rds.amazonaws.com;Port=5432;Username=postgres;Password=PinjemDong!;Database=pinjemdong";

        public AdminLogin()
        {
            InitializeComponent();
        }

        // Event handler untuk tombol Login as User
        private void UserLoginButton_Click(object sender, RoutedEventArgs e)
        {
            login userLoginPage = new login(); // Membuka halaman login pengguna
            userLoginPage.Show();
            this.Close(); // Menutup halaman login
        }

        // Event handler for Log in button
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string inputUsername = usernameBox.Text;
            string inputPassword = PasswordBox.Password;

            LoginAdmin(inputUsername, inputPassword);
        }

        // Event handler untuk klik "Log in as User"
        private void UserLoginText_Click(object sender, RoutedEventArgs e)
        {
            login userLoginPage = new login(); // Membuka halaman login pengguna
            userLoginPage.Show();
            this.Close(); // Menutup halaman login admin
        }

        // Method to authenticate admin
        private void LoginAdmin(string username, string password)
        {
            string query = "SELECT password, admin_id FROM public.admin WHERE username = @username";

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connstring))
                {
                    conn.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHashedPassword = reader["password"]?.ToString() ?? string.Empty;
                                int loggedInAdminId = (int)reader["admin_id"];

                                // Verifying password
                                if (BCrypt.Net.BCrypt.Verify(password, storedHashedPassword))
                                {
                                    MessageBox.Show("Log In successful");

                                    // Open AdminPage and pass loggedInAdminId
                                    AdminPage adminPage = new AdminPage(loggedInAdminId);  // Pass admin ID
                                    adminPage.Show();
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Username or password is incorrect", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Username or password is incorrect", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
