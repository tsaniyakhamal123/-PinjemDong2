﻿using System;
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
        private string connString = "Host=localhost;Port=5432;Username=postgres;Password=ininiya123;Database=JunproBener";

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
                // Pastikan data yang diperlukan sudah terisi
                if (string.IsNullOrEmpty(NameTB.Text) ||
                    string.IsNullOrEmpty(stockTB.Text) ||
                    string.IsNullOrEmpty(kategoriTB.Text) ||
                    string.IsNullOrEmpty(hargaTB.Text))
                {
                    MessageBox.Show("Please fill in all fields.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Konversi data input
                string namaBarang = NameTB.Text;
                int stock = int.Parse(stockTB.Text);
                string kategori = kategoriTB.Text;
                decimal harga = decimal.Parse(hargaTB.Text);

                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string insertQuery = "INSERT INTO barang (owner_id, nama_barang, stock, ulasan, kategori, harga, gambar) " +
                                         "VALUES (@ownerId, @namaBarang, @stock, @ulasan, @kategori, @harga, @gambar)";

                    using (var cmd = new NpgsqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ownerId", userId); // gunakan userId sebagai owner_id
                        cmd.Parameters.AddWithValue("@namaBarang", namaBarang);
                        cmd.Parameters.AddWithValue("@stock", stock);
                        cmd.Parameters.AddWithValue("@ulasan", ""); // Placeholder untuk 'ulasan'
                        cmd.Parameters.AddWithValue("@kategori", kategori);
                        cmd.Parameters.AddWithValue("@harga", harga);
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
    }
}
