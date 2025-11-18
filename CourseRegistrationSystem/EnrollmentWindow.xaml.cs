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
    /// Interaction logic for EnrollmentWindow.xaml
    /// </summary>
    public partial class EnrollmentWindow : Window
    {
        private UserData _currentUser;
        public EnrollmentWindow(UserData user)
        {
            InitializeComponent();
        }
    }
}
