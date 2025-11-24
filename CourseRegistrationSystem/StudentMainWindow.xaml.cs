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
            TxtFirstName.Text = user.FirstName;
            TxtLastName.Text = user.LastName;
            TxtEmail.Text = user.Email;
            TxtContact.Text = user.ContactNo;
            TxtAddress.Text = user.Address;
            TxtBirthDate.Text = user.BirthDate.ToString("yyyy-MM-dd");
            TxtMajor.Text = user.Major;
            TxtYear.Text = user.StudentYear.ToString();
            StudentNameText.Text = $"{user.FirstName} {user.LastName}";
            ProfileNameText.Text = $"{user.StudentID}";
            LoadCourses();
            LoadTerms();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Sidebar_Click(object sender, RoutedEventArgs e)
        {
            Button clicked = sender as Button;

            if (clicked == BtnSchedule) ShowPanel(SchedulePanel);
            else if (clicked == BtnBrowse) ShowPanel(BrowseCoursesGrid);
            else if (clicked == BtnCart) ShowPanel(CartPanel);

            else if (clicked == BtnDrop) ShowPanel(DropPanel);
            else if (clicked == BtnProfile) ShowPanel(ProfilePanel);
        }
        private void ShowPanel(UIElement panelToShow)
        {
            SchedulePanel.Visibility = Visibility.Collapsed;
            BrowseCoursesGrid.Visibility = Visibility.Collapsed;
            CartPanel.Visibility = Visibility.Collapsed;
            DropPanel.Visibility = Visibility.Collapsed;
            ProfilePanel.Visibility = Visibility.Collapsed;
            panelToShow.Visibility = Visibility.Visible;
        }
        private void BtnEnroll_Click(object sender, RoutedEventArgs e)
        {

            var enrollWindow = new EnrollmentWindow();
            enrollWindow.Owner = this;
            enrollWindow.ShowDialog();
        }
        private void LoadTerms()
        {
            var dal = new DALTermInfo();
            var terms = dal.GetAllTerms();
            foreach (var t in terms)
                TermCombo.Items.Add(t.DisplayName);
        }
        private void LoadStudentSchedule(string selectedTermText)
        {
            ComboBoxItem item = TermCombo.SelectedItem as ComboBoxItem;
            if (item == null || item.Tag == null) return;
            int termId = Convert.ToInt32(item.Tag);

            var dal = new DALCourseInfo();
            var scheduleList = dal.GetStudentSchedule(_currentUser.UserID, termId);

            ScheduleGrid.ItemsSource = scheduleList;
        }

        private void TermCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TermCombo.SelectedItem == null || TermCombo.SelectedIndex == 0)
                return;

            var selected = TermCombo.SelectedItem as ComboBoxItem;
            if (selected != null)
            {
                string termName = selected.Content.ToString();
                LoadStudentSchedule(termName);
            }
        }
        private void LoadCourses()
        {
            var dal = new DALCourseInfo();
            var courses = dal.GetAllCourses();
            BrowseCoursesGrid.ItemsSource = courses;
        }
        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            {

            }
        }

    }
}

