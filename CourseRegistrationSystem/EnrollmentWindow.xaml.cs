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

        public EnrollmentWindow()
        {
            InitializeComponent();
            _dalEnrollment = new DALEnrollment();
            LoadTerms();
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

        private void EnrollButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is CourseSearchResult selectedCourse)
            {
                MessageBox.Show($"Enrollment logic pending for {selectedCourse.CourseCode}.", "Enroll", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void enrollmentListButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Enrollment list functionality is not implemented yet.", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
