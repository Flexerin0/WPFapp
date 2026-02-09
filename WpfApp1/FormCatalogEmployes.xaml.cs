using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using WpfApp1.Models;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для FormCatalogEmployes.xaml
    /// </summary>
    public partial class FormCatalogEmployes : Window
    {
        private List<EmployeeImage> employeesImagesList = new List<EmployeeImage>();
        private int seletImg = 0;
        SqlDataAdapter adapter;
        DataTable table;
        public FormCatalogEmployes()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            adapter = new SqlDataAdapter(
                "SELECT * FROM Employees",
                App.ConnectionString
                );

            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

            table = new DataTable();
            adapter.Fill(table);

            IBoxEmployees.ItemsSource = table.DefaultView;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow row = table.NewRow();

                string[] fio = tBoxFio.Text.Split(' ');

                row["Name"] = fio.Length > 0 ? fio[0] : "";
                row["SName"] = fio.Length > 1 ? fio[1] : "";
                row["Patronomic"] = fio.Length > 2 ? fio[2] : "";
                row["Login"] = tBoxLogin.Text;
                row["Password"] = tBoxPassword.Text;
                row["BirthDate"] = dpBirthDate.SelectedDate ?? DateTime.Now;
                row["Phone"] = tBoxPhone.Text;
                row["Email"] = tBoxEmail.Text;
                row["CreatedAt"] = DateTime.Now;
                row["Note"] = tBoxNote.Text;

                table.Rows.Add(row);

                MessageBox.Show("Сотрудник добавлен (пока не сохранён в БД)");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IBoxEmployees.SelectedItem is DataRowView row)
            {
                tBoxId.Text = row["Id"].ToString();
                tBoxFio.Text = $"{row["Name"]} {row["SName"]} {row["Patronomic"]}";
                tBoxLogin.Text = row["Login"].ToString();
                tBoxPassword.Text = row["Password"].ToString();
                dpBirthDate.SelectedDate = Convert.ToDateTime(row["BirthDate"]);
                tBoxPhone.Text = row["Phone"].ToString();
                tBoxEmail.Text = row["Email"].ToString();
                tBoxNote.Text = row["Note"].ToString();

                if (row["Id"] == DBNull.Value)
                {
                    // Сотрудник ещё не сохранён и изображений быть не может
                    employeesImagesList.Clear();
                    imageViewer.Source = null;
                    labelCountImg.Content = "0/0";
                    return;
                }

                int employeeId = Convert.ToInt32(row["Id"]);
                LoadImages(employeeId);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                adapter.Update(table);
                MessageBox.Show("Изменения сохранены в базе данных");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (IBoxEmployees.SelectedItem == null) return;

            DataRowView rowView = (DataRowView)IBoxEmployees.SelectedItem;
            rowView.Row.Delete();

            MessageBox.Show("Сотрудник помечен на удаление");
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (IBoxEmployees.SelectedItem is DataRowView row)
            {
                string[] fio = tBoxFio.Text.Split(' ');

                row["Name"] = fio.Length > 0 ? fio[0] : "";
                row["SName"] = fio.Length > 1 ? fio[1] : "";
                row["Patronomic"] = fio.Length > 2 ? fio[2] : "";
                row["Login"] = tBoxLogin.Text;
                row["Password"] = tBoxPassword.Text;
                row["BirthDate"] = dpBirthDate.SelectedDate ?? DateTime.Now;
                row["Phone"] = tBoxPhone.Text;
                row["Email"] = tBoxEmail.Text;
                row["Note"] = tBoxNote.Text;

                MessageBox.Show("Изменения внесены (не сохранены в БД)");
            }
        }

        private void LoadImages(int employeeId)
        {
            employeesImagesList.Clear();
            seletImg = 0;

            using (SqlConnection db = new SqlConnection(App.ConnectionString))
            {
                db.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT Id, ImageData FROM EmployeeImages WHERE EmployeeId=@id", db);
                cmd.Parameters.AddWithValue("@id", employeeId);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    byte[] bytes = (byte[])reader["ImageData"];

                    BitmapImage image = new BitmapImage();
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = ms;
                        image.EndInit();
                    }

                    employeesImagesList.Add(new EmployeeImage
                    {
                        Id = (int)reader["Id"],
                        EmployeeId = employeeId,
                        Image = image
                    });
                }
            }

            ShowFirstImage();
        }

        private void ShowImage()
        {
            if (seletImg > 0 && seletImg <= employeesImagesList.Count)
            {
                imageViewer.Source = employeesImagesList[seletImg - 1].Image;
                labelCountImg.Content = $"{seletImg}/{employeesImagesList.Count}";
            }
            else
            {
                imageViewer.Source = null;
                labelCountImg.Content = "0/0";
            }
        }

        private void ShowFirstImage()
        {
            if (employeesImagesList.Count > 0)
            {
                seletImg = 1;
                ShowImage();
            }
            else
            {
                imageViewer.Source = null;
                labelCountImg.Content = "0/0";
            }
        }

        private void BtnImgNext_Click(object sender, RoutedEventArgs e)
        {
            if (seletImg < employeesImagesList.Count)
            {
                seletImg++;
                ShowImage();
            }
        }

        private void BtnImgPrev_Click(object sender, RoutedEventArgs e)
        {
            if (seletImg > 1)
            {
                seletImg--;
                ShowImage();
            }
        }

        private void BtnImgFirst_Click(object sender, RoutedEventArgs e)
        {
            if (employeesImagesList.Count > 0)
            {
                seletImg = 1;
                ShowImage();
            }
        }

        private void BtnImgLast_Click(object sender, RoutedEventArgs e)
        {
            if (employeesImagesList.Count > 0)
            {
                seletImg = employeesImagesList.Count;
                ShowImage();
            }
        }

        private void BtnImgAdd_Click(object sender, RoutedEventArgs e)
        {
            if (IBoxEmployees.SelectedItem is not DataRowView row) return;

            if (row["Id"] == DBNull.Value)
            {
                MessageBox.Show("Сначала сохраните сотрудника в базе данных, затем можно добавлять изображения.");
                return;
            }

            int employeeId = Convert.ToInt32(row["Id"]);

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Images|*.jpg;*.png;*.jpeg";

            if (dlg.ShowDialog() == true)
            {
                byte[] bytes = File.ReadAllBytes(dlg.FileName);

                using (SqlConnection db = new SqlConnection(App.ConnectionString))
                {
                    db.Open();

                    SqlCommand cmd = new SqlCommand(
                        "INSERT INTO EmployeeImages (EmployeeId, ImageData) VALUES (@emp, @img)", db);

                    cmd.Parameters.AddWithValue("@emp", employeeId);
                    cmd.Parameters.AddWithValue("@img", bytes);
                    cmd.ExecuteNonQuery();
                }

                LoadImages(employeeId);
            }
        }

        private void BtnImgDelete_Click(object sender, RoutedEventArgs e)
        {
            if (seletImg == 0) return;
            if (IBoxEmployees.SelectedItem is not DataRowView row) return;

            if (row["Id"] == DBNull.Value)
            {
                MessageBox.Show("Сначала сохраните сотрудника в базе данных, затем можно добавлять изображения.");
                return;
            }

            var img = employeesImagesList[seletImg - 1];

            using (SqlConnection db = new SqlConnection(App.ConnectionString))
            {
                db.Open();

                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM EmployeeImages WHERE Id=@id", db);
                cmd.Parameters.AddWithValue("@id", img.Id);
                cmd.ExecuteNonQuery();
            }

            LoadImages(img.EmployeeId);
        }
    }
}
