using Microsoft.Data.SqlClient;
using Microsoft.Win32;
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

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для FormDatabaseBackup.xaml
    /// </summary>
    public partial class FormDatabaseBackup : Window
    {
        public FormDatabaseBackup()
        {
            InitializeComponent();
        }

        private void btnBrowseBackup_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Backup files (*.bak)|*.bak";
            dialog.FileName = "FlexClubBackup";

            if (dialog.ShowDialog() == true)
            {
                tbBackupPath.Text = dialog.FileName;
            }
        }

        private void btnCreateBackup_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbBackupPath.Text))
            {
                MessageBox.Show("Укажите путь для сохранения файла.");
                return;
            }

            try
            {
                using (SqlConnection db = new SqlConnection(App.ConnectionString))
                {
                    db.Open();

                    string query = $@"
                BACKUP DATABASE FlexClub
                TO DISK = '{tbBackupPath.Text}'
                WITH INIT";

                    SqlCommand cmd = new SqlCommand(query, db);
                    cmd.ExecuteNonQuery();
                }

                txtBackupStatus.Foreground = Brushes.Green;
                txtBackupStatus.Text = "Резервная копия успешно создана.";
            }
            catch (Exception ex)
            {
                txtBackupStatus.Foreground = Brushes.Red;
                txtBackupStatus.Text = ex.Message;
            }
        }

        private void btnBrowseRestore_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Backup files (*.bak)|*.bak";

            if (dialog.ShowDialog() == true)
            {
                tbRestorePath.Text = dialog.FileName;
            }
        }

        private void btnRestoreDatabase_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbRestorePath.Text))
            {
                MessageBox.Show("Выберите файл резервной копии.");
                return;
            }

            try
            {
                string masterConnection =
                    App.ConnectionString.Replace("Initial Catalog=FlexClub", "Initial Catalog=master");

                using (SqlConnection db = new SqlConnection(masterConnection))
                {
                    db.Open();

                    string query = $@"
                ALTER DATABASE FlexClub SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

                RESTORE DATABASE FlexClub
                FROM DISK = '{tbRestorePath.Text}'
                WITH REPLACE;

                ALTER DATABASE FlexClub SET MULTI_USER;";

                    SqlCommand cmd = new SqlCommand(query, db);
                    cmd.ExecuteNonQuery();
                }

                txtRestoreStatus.Foreground = Brushes.Green;
                txtRestoreStatus.Text = "База данных успешно восстановлена.";
            }
            catch (Exception ex)
            {
                txtRestoreStatus.Foreground = Brushes.Red;
                txtRestoreStatus.Text = ex.Message;
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
