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

            ProfileNameText.Text = $"{user.FirstName} {user.LastName}";

            TxtFirstName.Text = user.FirstName;
            TxtLastName.Text = user.LastName;
            TxtEmail.Text = user.Email;
            TxtDepartment.Text = user.Department;
            TxtOffice.Text = user.OfficeLocation;
            InstructorNameText.Text = $"{user.FirstName} {user.LastName}";
        }

        private void Sidebar_Click(object sender, RoutedEventArgs e)
        {
            Button clicked = sender as Button;

            if (clicked == BtnClasses)
            {
                ShowPanel(ClassesPanel);
                LoadInstructorClasses();
            }
            else if (clicked == BtnRoster)
            {
                ShowPanel(RosterPanel);
                LoadClassDropdown();
            }
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

        private void LoadInstructorClasses()
        {
            var dal = new DALInstructorInfo();
            int instructorId = _currentUser.UserID;

            var classes = dal.GetInstructorClasses(instructorId);

            ClassesGrid.ItemsSource = classes;
        }
        private void ClassSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClassSelector.SelectedValue == null)
                return;

            int sectionId = (int)ClassSelector.SelectedValue;

            var dal = new DALInstructorInfo();
            var roster = dal.GetRoster(sectionId);

            RosterGrid.ItemsSource = roster;
        }

        private void LoadClassDropdown()
        {
            var dal = new DALInstructorInfo();
            int instructorId = _currentUser.UserID;

            var classes = dal.GetInstructorClasses(instructorId);

            ClassSelector.ItemsSource = classes;
            ClassSelector.DisplayMemberPath = "DisplayName";
            ClassSelector.SelectedValuePath = "SectionID";
            ClassSelector.SelectedIndex = -1;
            ClassSelector.Text = "— Select a class —";
        }



    }
}
