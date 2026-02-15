using Microsoft.Data.SqlClient;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для FormCatalogSuppliers.xaml
    /// </summary>
    public partial class FormCatalogSuppliers : Window
    {
        SqlDataAdapter adapter;
        DataTable table;

        public FormCatalogSuppliers()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            adapter = new SqlDataAdapter(
                "SELECT * FROM Suppliers",
                App.ConnectionString);

            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

            table = new DataTable();
            adapter.Fill(table);

            ListBoxSuppliers.ItemsSource = table.DefaultView;
        }

        private void ListBoxSuppliers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxSuppliers.SelectedItem is DataRowView row)
            {
                tBoxId.Text = row["Id"].ToString();
                tBoxName.Text = row["Name"].ToString();
                tBoxPhone.Text = row["Phone"].ToString();
                tBoxEmail.Text = row["Email"].ToString();

                if (row["RegistrationDate"] != DBNull.Value)
                    DatePickerRegistrationDate.SelectedDate = Convert.ToDateTime(row["RegistrationDate"]);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow row = table.NewRow();

                row["Name"] = tBoxName.Text;
                row["Phone"] = tBoxPhone.Text;
                row["Email"] = tBoxEmail.Text;

                if (DatePickerRegistrationDate.SelectedDate != null)
                    row["RegistrationDate"] = DatePickerRegistrationDate.SelectedDate.Value;

                table.Rows.Add(row);

                MessageBox.Show("Поставщик добавлен (не сохранён в БД)");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxSuppliers.SelectedItem is DataRowView row)
            {
                row["Name"] = tBoxName.Text;
                row["Phone"] = tBoxPhone.Text;
                row["Email"] = tBoxEmail.Text;

                if (DatePickerRegistrationDate.SelectedDate != null)
                    row["RegistrationDate"] = DatePickerRegistrationDate.SelectedDate.Value;

                MessageBox.Show("Изменения внесены (не сохранены)");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxSuppliers.SelectedItem is DataRowView row)
            {
                row.Delete();
                MessageBox.Show("Поставщик удалён (не сохранено)");
            }
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

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
