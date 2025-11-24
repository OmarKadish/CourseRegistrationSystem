using RegistrationEntityDAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for CourseWindow.xaml
    /// </summary>
    public partial class CourseWindow : Window
    {
        public ObservableCollection<CourseData> Courses { get; set; } = new ObservableCollection<CourseData>();
        private UserData _currentUser;

        public CourseWindow(UserData user)
        {
            InitializeComponent();
            _currentUser = user;
            GridCourses.ItemsSource = Courses;
            LoadCourseList();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadCourseList();


        }

        private void LoadCourseList()
        {
            DALCourseInfo dalCourseInfo = new DALCourseInfo();
            try
            {
                Courses.Clear();
                var list = dalCourseInfo.GetAllCourses();
                foreach (var course in list)
                {
                    Courses.Add(course);

                }
                TxtStatus.Text = $"Loaded {Courses.Count} courses.";
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to load courses: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void TxtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            var keyword = TxtSearch.Text?.Trim().ToLower() ?? "";
            if (string.IsNullOrEmpty(keyword))
            {
                GridCourses.ItemsSource = Courses;
            }
            else
            {
                var filtered = new ObservableCollection<CourseData>(Courses.Where(c =>
                    (!string.IsNullOrEmpty(c.CourseCode) && c.CourseCode.ToLower().Contains(keyword))
                    || (!string.IsNullOrEmpty(c.CourseName) && c.CourseName.ToLower().Contains(keyword))
                ));
                GridCourses.ItemsSource = filtered;
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var win = new CourseAddEditWindow();
            if (win.ShowDialog() == true)
            {
                // if created, refresh
                LoadCourseList();
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (GridCourses.SelectedItem is CourseData selected)
            {
                var win = new CourseAddEditWindow(selected.CourseID);
                if (win.ShowDialog() == true)
                {
                    LoadCourseList();
                }
            }
            else
            {
                MessageBox.Show("Please select a course to edit.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (GridCourses.SelectedItem is CourseData selected)
            {
                var res = MessageBox.Show($"Delete course {selected.CourseCode}? This will remove dependent sections/enrollments.", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (res == MessageBoxResult.Yes)
                {
                    try
                    {
                        DALCourseInfo courseInfo = new DALCourseInfo();
                        courseInfo.DeleteCourse(selected.CourseID);
                        LoadCourseList();
                        MessageBox.Show("Course deleted.", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Delete failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Select a course to delete.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
