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
        public StudentMainWindow(UserData user)
        {
            InitializeComponent();
            var dlg = new DALUserInfo();
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
            else if (clicked == BtnRegister) ShowPanel(RegisterPanel);
            else if (clicked == BtnDrop) ShowPanel(DropPanel);
            else if (clicked == BtnProfile) ShowPanel(ProfilePanel);
        }
        private void ShowPanel(UIElement panelToShow)
        {
            // Hide all
            SchedulePanel.Visibility = Visibility.Collapsed;
            BrowsePanel.Visibility = Visibility.Collapsed;
            CartPanel.Visibility = Visibility.Collapsed;
            RegisterPanel.Visibility = Visibility.Collapsed;
            DropPanel.Visibility = Visibility.Collapsed;
            ProfilePanel.Visibility = Visibility.Collapsed;

            // Show requested
            panelToShow.Visibility = Visibility.Visible;
        }
    }
}
