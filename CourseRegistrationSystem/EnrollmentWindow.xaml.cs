using RegistrationEntityDAL;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// <summary>
    /// Interaction logic for EnrollmentWindow.xaml
    /// </summary>
    public partial class EnrollmentWindow : Window
    {
        private readonly DALEnrollment _dalEnrollment;
        private UserData _currentUser;
        public EnrollmentWindow(UserData user)
        {
            InitializeComponent();
            _dalEnrollment = new DALEnrollment();
            _currentUser = user;
            LoadTerms();
            LoadCartItems();
        }

        private void LoadTerms()
        {
            var terms = _dalEnrollment.GetTerms();
            termComboBox.ItemsSource = terms;
            termComboBox.DisplayMemberPath = nameof(TermInfo.DisplayName);
            termComboBox.SelectedValuePath = nameof(TermInfo.TermId);

            if (terms != null && terms.Any())
            {
                termComboBox.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("No terms are currently available.", "Terms", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            int? termId = null;
            if (termComboBox.SelectedValue != null)
            {
                termId = Convert.ToInt32(termComboBox.SelectedValue);
            }

            string keyword = searchTextBox.Text?.Trim();
            var courses = _dalEnrollment.SearchCourses(termId, keyword);

            if (courses != null && courses.Any())
            {
                resultsGrid.ItemsSource = courses;
            }
            else
            {
                resultsGrid.ItemsSource = null;
                MessageBox.Show("No courses matched your search.", "No Results", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void LoadCartItems()
        {
            var dal = new DALCourseInfo();
            int studentId = _currentUser.UserID;
            var items = dal.GetCartItems(studentId);
            CartGrid.ItemsSource = items;
        }
        //not working
        private void EnrollButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn.Tag == null)
            {
                MessageBox.Show("Invalid section.");
                return;
            }

            int sectionId = Convert.ToInt32(btn.Tag);
            int studentId = _currentUser.UserID;

            string result = _dalEnrollment.EnrollStudent(studentId, sectionId);
            if (result == "OK")
            {
                MessageBox.Show("Enrolled successfully!");
                LoadCartItems();  
            }
            else
            {
                MessageBox.Show(result);
            }
        }

        private void EnrollFromCart_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn.Tag == null)
            {
                MessageBox.Show("Invalid section.");
                return;
            }

            int sectionId = Convert.ToInt32(btn.Tag);
            MessageBox.Show("Enrolling section: " + sectionId);
            int studentId = _currentUser.UserID;

            string result = _dalEnrollment.EnrollStudent(studentId, sectionId);

            if (result == "OK")
            {
                MessageBox.Show("Enrolled successfully!");
                LoadCartItems();   
            }
            else
            {
                MessageBox.Show(result);  
            }
        }




        private void enrollmentListButton_Click(object sender, RoutedEventArgs e)
        {
            int studentId = _currentUser.UserID;
            var items = _dalEnrollment.GetEnrollmentList(studentId);

            if (items == null || !items.Any())
            {
                MessageBox.Show("You are not enrolled in any courses.");
                return;
            }

            enrollmentListGrid.ItemsSource = items;
        }
        private void RemoveCartItem_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int cartId = Convert.ToInt32(btn.Tag);

            DALCourseInfo dal = new DALCourseInfo();
            dal.RemoveFromCart(cartId);

            LoadCartItems(); 
        }
    }
}
