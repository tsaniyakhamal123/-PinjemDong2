using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32; // Required for OpenFileDialog

namespace homepageJUnpro
{
    /// <summary>
    /// Interaction logic for Review.xaml
    /// </summary>
    public partial class Review : Window
    {
        public int _userId;

        // Constructor accepting userID and orderID
        public Review(int userID, int orderID)
        {
            InitializeComponent();
            _userId = userID;
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
                // Display selected image in the Image control
                string filePath = openFileDialog.FileName;
                imageControl.Source = new BitmapImage(new Uri(filePath));
            }
        }

        // Event handler for ReviewButton click (submit review)
        private void ReviewButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle the logic for submitting a review
            string description = NameTB.Text; // Text from the description field
            string rating = stockTB.Text; // Text from the rating field

            // Here you can implement your review submission logic
            MessageBox.Show($"Review submitted\nDescription: {description}\nRating: {rating}");
            History hist = new History(GetLoggedInUserId());
            hist.Show();
            this.Hide();
        }
    }
}
