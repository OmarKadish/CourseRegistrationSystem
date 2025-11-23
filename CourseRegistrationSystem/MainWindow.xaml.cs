using RegistrationEntityDAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CourseRegistrationSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            var userData = new UserData();
            var dlg = new LoginDialog();
            dlg.Owner = this;
            dlg.ShowDialog();
            DALUserInfo dal = new DALUserInfo();
            UserData user = dal.FetchUser(dlg.Email, dlg.Password, dlg.SelectedRole);
            if (dlg.DialogResult == true)
            {
                string role = dlg.SelectedRole;
                string email = dlg.Email;


                if (userData.LogIn(dlg.emailTextBox.Text, dlg.passwordTextBox.Password, dlg.SelectedRole) == true)
                {
                    if (user == null)
                    {
                        System.Windows.MessageBox.Show("Failed to load user profile.");
                        return;
                    }
                    Window dashboard = null;
                    switch (role)
                    {
                        case "Student":
                            dashboard = new StudentMainWindow(user);
                            break;

                        case "Instructor":
                            dashboard = new InstructorMainWindow(user);
                            break;

                        
                        case "Office Registrar":
                            dashboard = new AdminMainWindow(user);
                            break;

                        default:
                            System.Windows.MessageBox.Show("Unknown role. Cannot continue.");
                            return;
                    }
                    this.Hide();
                    dashboard.ShowDialog();
                    this.Show();
                }
            }
            else
                System.Windows.MessageBox.Show("You could not be verified. Please try again.");
        }
    

        private void exitButton_Click(object sender, RoutedEventArgs e) { this.Close(); }
        public MainWindow() { InitializeComponent(); }
        private void Window_Loaded(object sender, RoutedEventArgs e) { }
    }
}
