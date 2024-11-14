using System;
using System.Windows;
using Npgsql;
using BCrypt.Net;

namespace homepageJUnpro
{
    public partial class login : Window
    {
        // Connection string to PostgreSQL database
        private string connstring = "Host=localhost;Port=5432;Username=postgres;Password=ininiya123;Database=JunproBener";

        public login()
        {
            InitializeComponent();
        }

        // Event handler for Log in button
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string inputUsername = usernameBox.Text;
            string inputPassword = PasswordBox.Password;

            LoginUser(inputUsername, inputPassword);
        }

        // Method to authenticate user
        private void LoginUser(string username, string password)
        {
            string query = "SELECT password, user_id FROM public.pengguna WHERE username = @username";

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
                                int loggedInUserId = (int)reader["user_id"];

                                if (BCrypt.Net.BCrypt.Verify(password, storedHashedPassword))
                                {
                                    MessageBox.Show("Log In successful");

                                    // Open MainWindow and pass loggedInUserId (int) not Pengguna object
                                    MainWindow home = new MainWindow(loggedInUserId); // Pass only the UserId (int)
                                    home.Show();
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

        // Event handler for Sign up button
        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            sign_in signUpPage = new sign_in();
            signUpPage.Show();
            this.Close();
        }
    }
}
