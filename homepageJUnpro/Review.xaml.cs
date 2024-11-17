using System;
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
    /// Interaction logic for Review.xaml
    /// </summary>
    public partial class Review : Window
    {
        public int _userId;
        public Review(int userID, int orderID)
        {
            InitializeComponent();
            _userId = userID;
        }

        private int GetLoggedInUserId()
        {
            return _userId;
        }

        private void ReviewButton_Click(object sender, RoutedEventArgs e)
        {
            History hist = new History(GetLoggedInUserId());
            hist.Show();
            this.Hide();
        }
    }
}
