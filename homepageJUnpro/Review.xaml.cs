using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32; // Required for OpenFileDialog
using Npgsql;

namespace homepageJUnpro
{
    /// <summary>
    /// Interaction logic for Review.xaml
    /// </summary>
    public partial class Review : Window
    {
        public int _userId;
        public int _orderId;
        private byte[]? imageBytes; // Nullable karena bisa tidak ada gambar yang dipilih
        private string connString = "Host=postgres-junpro.cpm48umoy5cj.ap-southeast-2.rds.amazonaws.com;Port=5432;Username=postgres;Password=PinjemDong!;Database=pinjemdong";

        // Constructor accepting userID and orderID
        public Review(int userID, Riwayat hist)
        {
            InitializeComponent();
            _userId = userID;
            _orderId = hist.Id;
        }

        // Method to retrieve logged-in user ID
        private int GetLoggedInUserId()
        {
            return _userId;
        }

        // Event handler for AddGambarButton click to add an image
        private void AddGambarButton_Click(object sender, RoutedEventArgs e)
        {
            // Open file dialog to allow the user to select an image
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"; // Image file filter
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                imageBytes = File.ReadAllBytes(filePath);

                // Menampilkan gambar di kontrol Image WPF
                imageControl.Source = new BitmapImage(new Uri(filePath));

                MessageBox.Show("Image selected successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Event handler for ReviewButton click (submit review)
        private void ReviewButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validasi input
                if (string.IsNullOrWhiteSpace(descTB.Text) ||
                    string.IsNullOrWhiteSpace(scoreTB.Text))
                {
                    MessageBox.Show("Please fill in all fields.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validasi angka
                if (!int.TryParse(scoreTB.Text, out int score) || score < 1 || score > 10)
                {
                    MessageBox.Show("Stock harus berupa angka di range 1-10.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validasi gambar
                if (imageBytes == null || imageBytes.Length == 0)
                {
                    MessageBox.Show("Silakan tambahkan gambar terlebih dahulu.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Simpan ke database
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string insertQuery = "INSERT INTO review (order_number, reviewer, description, score, gambar) " +
                                         "VALUES (@orderNum, @reviewer, @desc, @score, @gambar)";

                    using (var cmd = new NpgsqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@orderNum", _orderId);
                        cmd.Parameters.AddWithValue("@reviewer", _userId);
                        cmd.Parameters.AddWithValue("@desc", descTB.Text.Trim());
                        cmd.Parameters.AddWithValue("@score", score);
                        cmd.Parameters.AddWithValue("@gambar", imageBytes);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Review added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                History hist = new History(GetLoggedInUserId());
                hist.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding item: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
