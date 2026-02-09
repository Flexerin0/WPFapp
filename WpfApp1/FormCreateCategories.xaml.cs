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
    /// Логика взаимодействия для FormCreateCategories.xaml
    /// </summary>
    public partial class FormCreateCategories : Window
    {
        SqlDataAdapter adapter;
        DataTable table;

        public FormCreateCategories()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            adapter = new SqlDataAdapter(
                "SELECT * FROM ProductCategory",
                App.ConnectionString);

            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

            table = new DataTable();
            adapter.Fill(table);

            listBoxCategories.ItemsSource = table.DefaultView;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxCategories.SelectedItem is DataRowView row)
            {
                tBoxId.Text = row["Id"].ToString();
                tBoxName.Text = row["Name"].ToString();
                tBoxDescription.Text = row["Description"].ToString();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tBoxName.Text))
            {
                MessageBox.Show("Введите название категории");
                return;
            }

            DataRow row = table.NewRow();
            row["Name"] = tBoxName.Text;
            row["Description"] = tBoxDescription.Text;

            table.Rows.Add(row);

            MessageBox.Show("Категория добавлена (пока не сохранена в БД)");
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

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxCategories.SelectedItem is not DataRowView row)
                return;

            if (row["Id"] == DBNull.Value)
            {
                MessageBox.Show("Сначала сохраните категорию в базе данных");
                return;
            }

            row["Name"] = tBoxName.Text;
            row["Description"] = tBoxDescription.Text;

            MessageBox.Show("Изменения внесены (не сохранены в БД)");
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxCategories.SelectedItem is not DataRowView row)
                return;

            if (row["Id"] == DBNull.Value)
            {
                MessageBox.Show("Эта категория ещё не сохранена в БД");
                return;
            }

            row.Row.Delete();

            MessageBox.Show("Категория помечена на удаление");
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
