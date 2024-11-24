using System;
using System.Data;
using System.Windows;

namespace homepageJUnpro
{
    /// <summary>
    /// Interaction logic for Account.xaml
    /// </summary>
    public partial class Account : Window
    {
        private readonly string ConnectionString = "Host=postgres-junpro.cpm48umoy5cj.ap-southeast-2.rds.amazonaws.com;Port=5432;Username=postgres;Password=PinjemDong!;Database=pinjemdong"; // Connection string global
        private int _userID; // ID pengguna yang sedang login

        public Account(int userID)
        {
            InitializeComponent();
            _userID = userID; // Assign user ID dari login
            LoadUserData(); // Load data user yang login
        }

        private void LoadUserData()
        {
            try
            {
                // Koneksi ke database
                using (var connection = new Npgsql.NpgsqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM pengguna WHERE user_id = @user_id";
                    using (var command = new Npgsql.NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@user_id", _userID);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Isi field dari database
                                NamaPenggunaTextBox.Text = reader["name"].ToString();
                                EmailTextBox.Text = reader["email"].ToString();
                                UsernameTextBox.Text = reader["username"].ToString();

                                // Untuk password, tidak menampilkan di UI
                                PasswordTextBox.Password = string.Empty; // Kosongkan field password
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user data: {ex.Message}");
            }
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var connection = new Npgsql.NpgsqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "UPDATE pengguna SET name = @name, email = @email, username = @username, password = @password WHERE user_id = @user_id";
                    using (var command = new Npgsql.NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", NamaPenggunaTextBox.Text);
                        command.Parameters.AddWithValue("@email", EmailTextBox.Text);
                        command.Parameters.AddWithValue("@username", UsernameTextBox.Text);

                        // Hash password baru jika diisi
                        if (!string.IsNullOrWhiteSpace(PasswordTextBox.Password))
                        {
                            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(PasswordTextBox.Password);
                            command.Parameters.AddWithValue("@password", hashedPassword);
                        }
                        else
                        {
                            // Jika password tidak diubah, gunakan password lama dari database
                            command.Parameters.AddWithValue("@password", GetOldPassword());
                        }

                        command.Parameters.AddWithValue("@user_id", _userID);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Profile updated successfully!");
                        }
                        else
                        {
                            MessageBox.Show("Failed to update profile.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating user data: {ex.Message}");
            }
        }

        private void BackButton_Click (object sender, EventArgs e)
        {
           MainWindow homepage = new MainWindow(_userID);
           homepage.Show();
           this.Close();
        }


        private string GetOldPassword()
        {
            string oldPassword = string.Empty;
            try
            {
                using (var connection = new Npgsql.NpgsqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT password FROM pengguna WHERE user_id = @user_id";
                    using (var command = new Npgsql.NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@user_id", _userID);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                oldPassword = reader["password"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching old password: {ex.Message}");
            }
            return oldPassword;
        }
    }

    
}
