using System;
using System.Windows;
using Npgsql;
using BCrypt.Net;

namespace homepageJUnpro
{
    /// <summary>
    /// Interaction logic for sign_in.xaml
    /// </summary>
    public partial class sign_in : Window
    {
        private NpgsqlConnection conn;
        private string connstring = "Host=localhost;Port=5432;Username=postgres;Password=ininiya123;Database=JunproBener";

        public sign_in()
        {
            InitializeComponent();
        }

        private void NameBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Optional: Handle name input changes here if needed
        }

        private void EmailBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Optional: Handle email input changes here if needed
        }

        private void UsernameBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Optional: Handle username input changes here if needed
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Optional: Handle password input changes here if needed
        }

        //private void SignButton_Click(object sender, RoutedEventArgs e)
        //{
            
        //}

        private void SignButton_Click_1(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text;
            string email = EmailBox.Text;
            string username = UsernameBox.Text;
            string password = passwordBox.Password;

            // Check if any fields are empty
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Hash the password before storing it
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            // SQL insert query
            string query = "INSERT INTO pengguna (name, email, username, password) VALUES (@name, @Email, @username, @password)";

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connstring))
                {
                    conn.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        // Adding parameters to the command
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", hashedPassword);

                        // Execute the command
                        cmd.ExecuteNonQuery();

                        // Notify user of successful sign-up
                        MessageBox.Show("Sign Up successful! You can now log in.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Optionally clear the input fields
                        NameBox.Clear();
                        EmailBox.Clear();
                        UsernameBox.Clear();
                        passwordBox.Clear();

                        // Show login window and close the current sign-up window
                        login loginForm = new login();
                        loginForm.Show();
                        this.Close();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            login loginPage = new login();
            loginPage.Show();
            this.Close();
        }
    }
}
