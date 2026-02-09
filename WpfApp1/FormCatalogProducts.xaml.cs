using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FormCatalogProducts : Window
    {
        SqlDataAdapter adapter;
        DataTable table;
        DataTable categoryTable;

        public FormCatalogProducts()
        {
            InitializeComponent();
            LoadCategories();
            LoadData();
        }

        private void LoadData()
        {
            adapter = new SqlDataAdapter(
                "SELECT * FROM Products",
                App.ConnectionString);

            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

            table = new DataTable();
            adapter.Fill(table);

            IBoxProducts.ItemsSource = table.DefaultView;

            LoadCategories();
        }

        private void LoadCategories()
        {
            using (SqlConnection conn = new SqlConnection(App.ConnectionString))
            {
                using SqlConnection db = new SqlConnection(App.ConnectionString);
                db.Open();

                SqlDataAdapter catAdapter = new SqlDataAdapter(
                    "SELECT Id, Name FROM ProductCategory", db);

                DataTable catTable = new DataTable();
                catAdapter.Fill(catTable);

                ComboBoxCategory.ItemsSource = catTable.DefaultView;
                ComboBoxCategory.DisplayMemberPath = "Name";
                ComboBoxCategory.SelectedValuePath = "Id";
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IBoxProducts.SelectedItem is DataRowView row)
            {
                tBoxId.Text = row["Id"].ToString();
                tBoxName.Text = row["Name"].ToString();
                tBoxBarcode.Text = row["Barcode"].ToString();
                tBoxPurchasePrice.Text = row["PurchasePrice"].ToString();
                tBoxPrice.Text = row["SellingPrice"].ToString();
                tBoxDescription.Text = row["Description"].ToString();

                ComboBoxCategory.SelectedValue = row["CategoryId"] == DBNull.Value
                    ? null
                    : row["CategoryId"];
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow row = table.NewRow();

                row["Name"] = tBoxName.Text;
                row["Barcode"] = tBoxBarcode.Text;
                row["PurchasePrice"] = decimal.TryParse(tBoxPurchasePrice.Text, out var pp) ? pp : 0;
                row["SellingPrice"] = decimal.TryParse(tBoxPrice.Text, out var pr) ? pr : 0;
                row["Description"] = tBoxDescription.Text;

                if (ComboBoxCategory.SelectedValue != null)
                    row["CategoryId"] = ComboBoxCategory.SelectedValue;
                else
                    row["CategoryId"] = DBNull.Value;

                table.Rows.Add(row);

                MessageBox.Show("Товар добавлен (пока не сохранён в БД)");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (IBoxProducts.SelectedItem is DataRowView row)
            {
                row["Name"] = tBoxName.Text;
                row["Barcode"] = tBoxBarcode.Text;
                row["PurchasePrice"] = decimal.TryParse(tBoxPurchasePrice.Text, out var pp) ? pp : 0;
                row["SellingPrice"] = decimal.TryParse(tBoxPrice.Text, out var pr) ? pr : 0;
                row["Description"] = tBoxDescription.Text;

                if (ComboBoxCategory.SelectedValue != null)
                    row["CategoryId"] = ComboBoxCategory.SelectedValue;
                else
                    row["CategoryId"] = DBNull.Value;

                MessageBox.Show("Изменения внесены (не сохранены в БД)");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (IBoxProducts.SelectedItem == null) return;

            DataRowView rowView = (DataRowView)IBoxProducts.SelectedItem;
            rowView.Row.Delete();

            MessageBox.Show("Товар помечен на удаление");
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

        private void BtnCategories_Click(object sender, RoutedEventArgs e)
        {
            new FormCreateCategories().Show();
        }
    }
}