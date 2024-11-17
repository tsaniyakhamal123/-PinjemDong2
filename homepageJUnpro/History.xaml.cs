﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace homepageJUnpro
{
    /// <summary>
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class History : Window
    {
        public int _userId;
        public int _orderID;
        public int _barangID;
        public History(int loggedInUserId)
        {
            InitializeComponent();
            _userId = loggedInUserId;
        }

        private int GetLoggedInUserId()
        {
            return _userId;
        }

        private void ReviewButton_Click(object sender, RoutedEventArgs e)
        {
            Review ulasan = new Review(GetLoggedInUserId(), _orderID);
            ulasan.Show();
            this.Hide();
        }

        private void RerentButton_Click(object sender, RoutedEventArgs e)
        {
            Checkout CO = new Checkout(GetLoggedInUserId(), _barangID);
            CO.Show();
            this.Hide();
        }
    }
}
