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
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window
    {
        public string Email => emailTextBox.Text.Trim();
        public string Password => passwordTextBox.Password.Trim();
        public string SelectedRole => ((ComboBoxItem)RoleCombo.SelectedItem).Content.ToString();

        public LoginDialog()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Email)||string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Please enter both email and password.");
                return;
            }
            MessageBox.Show($"Logging in as: {SelectedRole}");
            //CourseWindow courseWindow = new CourseWindow();
            //courseWindow.Show(); //I added these to test my window
            this.DialogResult = true;
        }

        private void RoleCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        //private void cancelButton_Click(object sender, RoutedEventArgs e)
        //{
        // this.DialogResult = false;
        //  //  this.Close();
        // }
    }
}
