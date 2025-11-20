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

        public CourseWindow()
        {
            InitializeComponent();
            GridCourses.ItemsSource = Courses;
            LoadCourseList();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
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

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TxtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
