using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using homepageJUnpro; // Menambahkan namespace homepageJUnpro jika Login ada di sini.
using Npgsql;

namespace homepageJUnpro
{
    /// <summary>
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class History : Window
    {
        public int _userId;
        public int _orderID;
        public Barang _barang;

        public History(int loggedInUserId)
        {
            InitializeComponent();
            _userId = loggedInUserId;
        }

        private int GetLoggedInUserId()
        {
            return _userId;
        }

        // Event handler untuk HomeButton
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            // Pindah ke halaman MainWindow (homepage/dashboard)
            MainWindow home = new MainWindow(GetLoggedInUserId());
            home.Show();
            this.Close();
        }

        // Event handler untuk HistoryButton
        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            // Tetap di halaman ini (opsional)
            MessageBox.Show("Anda sudah berada di halaman History.");
        }

        // Event handler untuk DashboardButton
        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            // Pindah ke halaman MainWindow (anggap ini juga dashboard Anda)
            MainWindow dashboard = new MainWindow(GetLoggedInUserId());
            dashboard.Show();
            this.Close();
        }

        // Event handler untuk LogOutButton
        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            {
                login loginPage = new login();
                loginPage.Show();
                this.Close();
            }
        }


        // Event handler untuk SearchBox TextChanged
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Logika untuk pencarian
            var searchText = (sender as TextBox)?.Text;
            if (!string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show($"Mencari: {searchText}");
            }
        }

        // Event handler untuk ReviewButton
        private void ReviewButton_Click(object sender, RoutedEventArgs e)
        {
            Review ulasan = new Review(GetLoggedInUserId(), _orderID);
            ulasan.Show();
            this.Hide();
        }

        // Event handler untuk RerentButton
        private void RerentButton_Click(object sender, RoutedEventArgs e)
        {
            Checkout CO = new Checkout(_barang, GetLoggedInUserId());
            CO.Show();
            this.Hide();
        }
    }
}
