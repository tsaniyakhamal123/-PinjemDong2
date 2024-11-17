using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Npgsql;
using System.Windows.Media.Imaging;

namespace homepageJUnpro
{
    public partial class TambahBarang : Window
    {
        private byte[]? imageBytes; // Nullable karena bisa tidak ada gambar yang dipilih
        private int userId; // ID user yang akan jadi owner_id barang
        private string connString = "Host=postgres-junpro.cpm48umoy5cj.ap-southeast-2.rds.amazonaws.com;Port=5432;Username=postgres;Password=PinjemDong!;Database=pinjemdong";
        public TambahBarang(int userId)
        {
            InitializeComponent();
            this.userId = userId;

            // Periksa apakah user sudah ada di tabel pemilik, jika belum, tambahkan
            InsertPemilik();
        }

        // Fungsi untuk menambahkan pemilik jika belum ada
        private void InsertPemilik()
        {
            try
            {
                string checkPemilikQuery = "INSERT INTO pemilik (owner_id) " +
                                           "SELECT @userId " +
                                           "WHERE NOT EXISTS (SELECT 1 FROM pemilik WHERE owner_id = @userId)";
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(checkPemilikQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inserting or updating pemilik: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event handler untuk menambahkan barang
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validasi input
                if (string.IsNullOrWhiteSpace(NameTB.Text) ||
                    string.IsNullOrWhiteSpace(stockTB.Text) ||
                    string.IsNullOrWhiteSpace(kategoriTB.Text) ||
                    string.IsNullOrWhiteSpace(hargaTB.Text) ||
                    string.IsNullOrWhiteSpace(deskripsiTB.Text) ||
                    string.IsNullOrWhiteSpace(alamatTB.Text))
                {
                    MessageBox.Show("Please fill in all fields.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validasi angka
                if (!int.TryParse(stockTB.Text, out int stock) || stock <= 0)
                {
                    MessageBox.Show("Stock harus berupa angka positif.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(hargaTB.Text, out decimal harga) || harga <= 0)
                {
                    MessageBox.Show("Harga harus berupa angka positif.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validasi gambar
                if (imageBytes == null || imageBytes.Length == 0)
                {
                    MessageBox.Show("Silakan pilih gambar terlebih dahulu.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Simpan ke database
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string insertQuery = "INSERT INTO barang (owner_id, nama_barang, stock, ulasan, kategori, harga, deskripsi, alamat, gambar) " +
                                         "VALUES (@ownerId, @namaBarang, @stock, @ulasan, @kategori, @harga, @deskripsi, @alamat, @gambar)";

                    using (var cmd = new NpgsqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ownerId", userId);
                        cmd.Parameters.AddWithValue("@namaBarang", NameTB.Text.Trim());
                        cmd.Parameters.AddWithValue("@stock", stock);
                        cmd.Parameters.AddWithValue("@ulasan", ""); // Placeholder untuk ulasan
                        cmd.Parameters.AddWithValue("@kategori", kategoriTB.Text.Trim());
                        cmd.Parameters.AddWithValue("@harga", harga);
                        cmd.Parameters.AddWithValue("@deskripsi", deskripsiTB.Text.Trim());
                        cmd.Parameters.AddWithValue("@alamat", alamatTB.Text.Trim());
                        cmd.Parameters.AddWithValue("@gambar", imageBytes);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Item added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding item: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                imageBytes = File.ReadAllBytes(filePath);

                // Menampilkan gambar di kontrol Image WPF
                imageControl.Source = new BitmapImage(new Uri(filePath));

                MessageBox.Show("Image selected successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            int loggedInUserId = userId; // Ambil ID user yang sudah ada
            MainWindow homepage = new MainWindow(loggedInUserId);
            homepage.Show();
            this.Close();
        }
    }
}
