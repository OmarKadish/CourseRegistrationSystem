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
    /// <summary>
    /// Interaction logic for AdminMainWindow.xaml
    /// </summary>
    public partial class AdminMainWindow : Window
    {
        private UserData _currentUser;

        public AdminMainWindow(UserData user)
        {
            InitializeComponent();
            _currentUser = user;
        }
        private void BtnOpenCourseWindow_Click(object sender, RoutedEventArgs e)
        {
            var win = new CourseWindow(_currentUser);
            win.Owner = this;
            win.ShowDialog();
        }
    }
}
