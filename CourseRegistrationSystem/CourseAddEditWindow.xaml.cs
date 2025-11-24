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
using RegistrationEntityDAL;

namespace CourseRegistrationSystem
{
    /// <summary>
    /// Interaction logic for CourseAddEditWindow.xaml
    /// </summary>
    public partial class CourseAddEditWindow : Window
    {
        private DALCourseInfo _courseInfo = new DALCourseInfo();
        private CourseData _course;
        private readonly bool _isEdit;
        public CourseAddEditWindow(int courseId = 0)
        {
            InitializeComponent();
            if (courseId > 0)
            {
                _isEdit = true;
                LoadCourse(courseId);
            }
            else
            {
                _isEdit = false;
                _course = new CourseData();
            }
        }

        private void LoadCourse(int courseId)
        {
            try
            {
                // Load course
                var courseList = _courseInfo.GetAllCourses();
                _course = courseList.FirstOrDefault(c => c.CourseID == courseId);
                if (_course == null)
                {
                    MessageBox.Show("Course not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                // bind fields
                TxtCourseCode.Text = _course.CourseCode;
                TxtCourseName.Text = _course.CourseName;
                TxtCredits.Text = _course.Credits.ToString();
                TxtDescription.Text = _course.Description;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load course: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // basic validation
            if (string.IsNullOrWhiteSpace(TxtCourseCode.Text) || string.IsNullOrWhiteSpace(TxtCourseName.Text))
            {
                MessageBox.Show("Course code and name are required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtCredits.Text, out int credits))
            {
                MessageBox.Show("Credits must be an integer.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _course.CourseCode = TxtCourseCode.Text.Trim();
            _course.CourseName = TxtCourseName.Text.Trim();
            _course.Credits = credits;
            _course.Description = TxtDescription.Text?.Trim();

            try
            {
                if (_isEdit)
                {
                    _courseInfo.UpdateCourse(_course);
                    MessageBox.Show("Course updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var isAdded = _courseInfo.AddCourse(_course.CourseCode, _course.CourseName, _course.Description, _course.Credits);
                    if (isAdded > 0)
                    {
                        MessageBox.Show($"{isAdded} Course created.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void BtnManagePrereqs_Click(object sender, RoutedEventArgs e)
        {
            // TODO: manage prerequisites
        }
    }
}
