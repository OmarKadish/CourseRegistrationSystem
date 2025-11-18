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


namespace CourseRegistrationSystem
{
    public partial class InstructorMainWindow : Window
    {
        private UserData _currentUser;

        public InstructorMainWindow(UserData user)
        {
            InitializeComponent();
            _currentUser = user;

            InstructorNameText.Text = $"{user.FirstName} {user.LastName}";

            LoadClasses();
        }

        private void Sidebar_Click(object sender, RoutedEventArgs e)
        {
            Button clicked = sender as Button;

            if (clicked == BtnClasses) ShowPanel(ClassesPanel);
            else if (clicked == BtnRoster) ShowPanel(RosterPanel);
            else if (clicked == BtnProfile) ShowPanel(ProfilePanel);
        }

        private void ShowPanel(UIElement panel)
        {
            ClassesPanel.Visibility = Visibility.Collapsed;
            RosterPanel.Visibility = Visibility.Collapsed;
            ProfilePanel.Visibility = Visibility.Collapsed;

            panel.Visibility = Visibility.Visible;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LoadClasses()
        {
            // TODO: Fill later after your teammates finish Section CRUD
            // var dal = new DALInstructor();
            // ClassesGrid.ItemsSource = dal.GetInstructorClasses(_currentUser.UserID);
        }
    }
}
