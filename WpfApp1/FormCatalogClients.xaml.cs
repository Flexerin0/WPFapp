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
    /// Логика взаимодействия для FormCatalogClients.xaml
    /// </summary>
    public partial class FormCatalogClients : Window
    {
        SqlDataAdapter adapter;
        DataTable table;

        public FormCatalogClients()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            adapter = new SqlDataAdapter(
                "SELECT * FROM Clients",
                App.ConnectionString);

            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

            table = new DataTable();
            adapter.Fill(table);

            ListBoxClients.ItemsSource = table.DefaultView;
        }

        private void ListBoxClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxClients.SelectedItem is DataRowView row)
            {
                tBoxId.Text = row["Id"].ToString();
                tBoxFullName.Text = row["FullName"].ToString();
                tBoxPhone.Text = row["Phone"].ToString();
                tBoxEmail.Text = row["Email"].ToString();
                tBoxBalance.Text = row["Balance"].ToString();
                tBoxBonusPoints.Text = row["BonusPoints"].ToString();
                tBoxDescription.Text = row["Description"].ToString();

                if (row["RegistrationDate"] != DBNull.Value)
                    DatePickerRegistrationDate.SelectedDate =
                        Convert.ToDateTime(row["RegistrationDate"]);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow row = table.NewRow();

                row["FullName"] = tBoxFullName.Text;
                row["Phone"] = tBoxPhone.Text;
                row["Email"] = tBoxEmail.Text;

                row["Balance"] =
                    decimal.TryParse(tBoxBalance.Text, out var balance)
                    ? balance : 0;

                row["BonusPoints"] =
                    int.TryParse(tBoxBonusPoints.Text, out var points)
                    ? points : 0;

                row["Description"] = tBoxDescription.Text;

                if (DatePickerRegistrationDate.SelectedDate != null)
                    row["RegistrationDate"] =
                        DatePickerRegistrationDate.SelectedDate.Value;

                table.Rows.Add(row);

                MessageBox.Show("Клиент добавлен (не сохранён в БД)");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxClients.SelectedItem is DataRowView row)
            {
                row["FullName"] = tBoxFullName.Text;
                row["Phone"] = tBoxPhone.Text;
                row["Email"] = tBoxEmail.Text;

                row["Balance"] =
                    decimal.TryParse(tBoxBalance.Text, out var balance)
                    ? balance : 0;

                row["BonusPoints"] =
                    int.TryParse(tBoxBonusPoints.Text, out var points)
                    ? points : 0;

                row["Description"] = tBoxDescription.Text;

                if (DatePickerRegistrationDate.SelectedDate != null)
                    row["RegistrationDate"] =
                        DatePickerRegistrationDate.SelectedDate.Value;

                MessageBox.Show("Изменения внесены (не сохранены)");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxClients.SelectedItem is DataRowView row)
            {
                row.Delete();
                MessageBox.Show("Клиент удалён (не сохранено)");
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
