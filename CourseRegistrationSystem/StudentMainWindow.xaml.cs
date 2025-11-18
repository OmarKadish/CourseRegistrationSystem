using RegistrationEntityDAL;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace CourseRegistrationSystem
{
    /// <summary>
    /// Interaction logic for StudentMainWindow.xaml
    /// </summary>
    public partial class StudentMainWindow : Window
    {
        private UserData _currentUser;
        
        public StudentMainWindow(UserData user)
        {
            InitializeComponent();
            _currentUser = user;
            StudentNameText.Text = $"{user.FirstName} {user.LastName}"; 
            
            
            
            

        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Sidebar_Click(object sender, RoutedEventArgs e)
        {
            Button clicked = sender as Button;

            if (clicked == BtnSchedule) ShowPanel(SchedulePanel);
            else if (clicked == BtnBrowse) ShowPanel(BrowsePanel);
            else if (clicked == BtnCart) ShowPanel(CartPanel);
            
            else if (clicked == BtnDrop) ShowPanel(DropPanel);
            else if (clicked == BtnProfile) ShowPanel(ProfilePanel);
        }
        private void ShowPanel(UIElement panelToShow)
        {
            SchedulePanel.Visibility = Visibility.Collapsed;
            BrowsePanel.Visibility = Visibility.Collapsed;
            CartPanel.Visibility = Visibility.Collapsed;
            DropPanel.Visibility = Visibility.Collapsed;
            ProfilePanel.Visibility = Visibility.Collapsed;

            // Show requested
            panelToShow.Visibility = Visibility.Visible;
        }
        private void BtnEnroll_Click(object sender, RoutedEventArgs e)
        {

            var enrollWindow = new EnrollmentWindow(_currentUser);
            enrollWindow.Owner = this;
            enrollWindow.ShowDialog();
        }
    }
}
