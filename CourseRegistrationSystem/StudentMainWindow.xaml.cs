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

            if (clicked == BtnSchedule)
            {
                ShowPanel(SchedulePanel);
                LoadTerms();

            }
            else if (clicked == BtnBrowse) ShowPanel(BrowseCoursesGrid);
            else if (clicked == BtnCart)
            {
                ShowPanel(CartPanel);
                LoadCart();
            }
            else if (clicked == BtnDrop) {
                ShowPanel(DropPanel);
                LoadDropCourses();
            }
            
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

            var enrollWindow = new EnrollmentWindow(_currentUser);
            enrollWindow.Owner = this;
            enrollWindow.ShowDialog();
        }
        private void LoadTerms()
        {
           

            TermCombo.Items.Clear();

            var dal = new DALTermInfo();
            var terms = dal.GetAllTerms();

            foreach (var t in terms)
            {
                ComboBoxItem cb = new ComboBoxItem();
                cb.Content = t.DisplayName; // ex: "Fall 2024"
                cb.Tag = t.TermID;          // IMPORTANT: stores actual ID

                TermCombo.Items.Add(cb);
            }
        }

        private void LoadStudentSchedule()
        {
            ComboBoxItem item = TermCombo.SelectedItem as ComboBoxItem;
            if (item == null || item.Tag == null) return;

            int termId = Convert.ToInt32(item.Tag);

            var dal = new DALEnrollment();
            var scheduleList = dal.GetStudentSchedule(_currentUser.UserID, termId);

            ScheduleGrid.ItemsSource = scheduleList;
        }

        private void TermCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TermCombo.SelectedIndex > -1)
                LoadStudentSchedule();
        }

        private void LoadCourses()
        {
            var dal = new DALCourseInfo();
            var courses = dal.GetAllCourses();
            BrowseCoursesGrid.ItemsSource = courses;
        }
        private void LoadCart()
        {
            var dal = new DALCourseInfo();
            var items = dal.GetCartItems(_currentUser.UserID);

            CartGrid.ItemsSource = items;
        }
        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int courseId = Convert.ToInt32(btn.Tag); 
            int studentId = _currentUser.UserID;

            DALCourseInfo dal = new DALCourseInfo();

            try
            {
                bool result = dal.AddToCart(studentId, courseId);

                if (result)
                    MessageBox.Show("Course added to cart successfully!");
                else
                    MessageBox.Show("Course is already in your cart.", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add course to cart: " + ex.Message, "Error");
            }
        }
        private void RemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int cartId = Convert.ToInt32(btn.Tag);

            var dal = new DALCourseInfo();
            dal.RemoveFromCart(cartId);

            LoadCart();
        }

        private void LoadDropCourses()
        {
            ComboBoxItem item = TermCombo.SelectedItem as ComboBoxItem;
            if (item == null || item.Tag == null) return;

            int termId = Convert.ToInt32(item.Tag);

            var dal = new DALEnrollment();
            var list = dal.GetStudentSchedule(_currentUser.UserID, termId);

            DropGrid.ItemsSource = list;
        }

        private void DropCourse_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int sectionId = Convert.ToInt32(btn.Tag);
            int studentId = _currentUser.UserID;

            var dal = new DALEnrollment();
            bool result = dal.DropCourse(studentId, sectionId);

            if (result)
                MessageBox.Show("Course dropped successfully!");
            else
                MessageBox.Show("Failed to drop the course.");

            LoadStudentSchedule();
            LoadDropCourses();
        }

    }
}

