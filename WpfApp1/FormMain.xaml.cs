using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
    /// Логика взаимодействия для FormMain.xaml
    /// </summary>
    public partial class FormMain : Window
    {
        public FormMain()
        {
            InitializeComponent();
            LoadReceipts();
            LoadSales();
            LoadProducts(cbSaleProduct);
            LoadClients(cbSaleClient);
            LoadProducts(cbReceiptProduct);
            LoadSuppliers(cbReceiptSupplier);
        }

        private void LoadProducts(ComboBox comboBox)
        {
            comboBox.Items.Clear();

            using (SqlConnection db = new SqlConnection(App.ConnectionString))
            {
                db.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT Id, Name FROM Products", db);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    comboBox.Items.Add(new ComboItem
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }

            comboBox.DisplayMemberPath = "Name";
            comboBox.SelectedValuePath = "Id";
        }

        private void LoadReceipts()
        {
            using (SqlConnection db = new SqlConnection(App.ConnectionString))
            {
                db.Open();

                SqlCommand cmd = new SqlCommand(@"
            SELECT r.Id,
                   p.Name AS Product,
                   s.Name AS Supplier,
                   r.Quantity,
                   r.PricePerUnit,
                   r.ReceiptDate,
                   r.Description
            FROM Receipts r
            JOIN Products p ON r.ProductId = p.Id
            JOIN Suppliers s ON r.SupplierId = s.Id
            ORDER BY r.ReceiptDate DESC", db);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                System.Data.DataTable table = new System.Data.DataTable();
                adapter.Fill(table);

                dgReceipts.ItemsSource = table.DefaultView;
            }
        }

        private void LoadSuppliers(ComboBox comboBox)
        {
            comboBox.Items.Clear();

            using (SqlConnection db = new SqlConnection(App.ConnectionString))
            {
                db.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT Id, Name FROM Suppliers", db);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    comboBox.Items.Add(new ComboItem
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }

            comboBox.DisplayMemberPath = "Name";
            comboBox.SelectedValuePath = "Id";
        }

        private void LoadClients(ComboBox comboBox)
        {
            comboBox.Items.Clear();

            using (SqlConnection db = new SqlConnection(App.ConnectionString))
            {
                db.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT Id, FullName FROM Clients", db);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    comboBox.Items.Add(new ComboItem
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }

            comboBox.DisplayMemberPath = "Name";
            comboBox.SelectedValuePath = "Id";
        }

        private void LoadSales()
        {
            using (SqlConnection db = new SqlConnection(App.ConnectionString))
            {
                db.Open();

                SqlCommand cmd = new SqlCommand(@"
        SELECT s.Id,
               p.Name AS Product,
               c.FullName AS Client,
               s.Quantity,
               s.PricePerUnit,
               s.Discount,
               s.TotalAmount,
               s.SaleDateTime,
               s.CheckNumber
        FROM Sales s
        JOIN Products p ON s.ProductId = p.Id
        JOIN Clients c ON s.ClientId = c.Id
        ORDER BY s.SaleDateTime DESC", db);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                System.Data.DataTable table = new System.Data.DataTable();
                adapter.Fill(table);

                dgSales.ItemsSource = table.DefaultView;
            }
        }   

        private void cbSaleProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbSaleProduct.SelectedValue == null)
                return;

            using (SqlConnection db = new SqlConnection(App.ConnectionString))
            {
                db.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT Quantity FROM Products WHERE Id=@id", db);

                cmd.Parameters.AddWithValue("@id", cbSaleProduct.SelectedValue);

                int quantity = (int)cmd.ExecuteScalar();

                lblProductStock.Content = $"Остаток: {quantity}";
            }
        }

        private void btnAddReceipt_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection db = new SqlConnection(App.ConnectionString))
            {
                db.Open();
                SqlTransaction transaction = db.BeginTransaction();

                try
                {
                    if (cbReceiptProduct.SelectedValue == null || cbReceiptSupplier.SelectedValue == null)
                    {
                        MessageBox.Show("Выберите товар и поставщика");
                        return;
                    }
                    int productId = (int)cbReceiptProduct.SelectedValue;
                    int supplierId = (int)cbReceiptSupplier.SelectedValue;
                    int quantity = int.Parse(tbReceiptQuantity.Text);
                    decimal price = decimal.Parse(tbReceiptPrice.Text);

                    SqlCommand insertReceipt = new SqlCommand(
                        @"INSERT INTO Receipts 
                  (ProductId, SupplierId, Quantity, PricePerUnit, ReceiptDate, Description)
                  VALUES (@pId, @sId, @q, @price, GETDATE(), @desc)", db, transaction);

                    insertReceipt.Parameters.AddWithValue("@pId", productId);
                    insertReceipt.Parameters.AddWithValue("@sId", supplierId);
                    insertReceipt.Parameters.AddWithValue("@q", quantity);
                    insertReceipt.Parameters.AddWithValue("@price", price);
                    insertReceipt.Parameters.AddWithValue("@desc", tbReceiptDescription.Text);

                    insertReceipt.ExecuteNonQuery();

                    SqlCommand updateProduct = new SqlCommand(
                        "UPDATE Products SET Quantity = Quantity + @q WHERE Id=@id",
                        db, transaction);

                    updateProduct.Parameters.AddWithValue("@q", quantity);
                    updateProduct.Parameters.AddWithValue("@id", productId);

                    updateProduct.ExecuteNonQuery();

                    transaction.Commit();
                    MessageBox.Show("Поставка добавлена ✔");
                    LoadReceipts();

                }
                catch
                {
                    transaction.Rollback();
                    MessageBox.Show("Ошибка при добавлении поставки");
                }
            }
        }


        private void Directories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbDirectories.SelectedItem is ComboBoxItem item)
            {
                switch (item.Content.ToString())
                {
                    case "Сотрудники":
                        new FormCatalogEmployes().Show();
                        break;

                    case "Клиенты":
                        new FormCatalogClients().Show();
                        break;

                    case "Товары":
                        new FormCatalogProducts().Show();
                        break;
                    case "Поставщики":
                        new FormCatalogSuppliers().Show();
                        break;
                }

                cbDirectories.SelectedIndex = -1; // сброс выбора
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnAddSale_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection db = new SqlConnection(App.ConnectionString))
            {
                db.Open();
                SqlTransaction transaction = db.BeginTransaction();

                try
                {
                    if (cbSaleProduct.SelectedValue == null || cbSaleClient.SelectedValue == null)
                    {
                        MessageBox.Show("Выберите товар и клиента");
                        return;
                    }
                    int productId = (int)cbSaleProduct.SelectedValue;
                    int clientId = (int)cbSaleClient.SelectedValue;
                    int quantity = int.Parse(tbSaleQuantity.Text);
                    decimal price = decimal.Parse(tbSalePrice.Text);
                    decimal discount = string.IsNullOrEmpty(tbSaleDiscount.Text)
                        ? 0
                        : decimal.Parse(tbSaleDiscount.Text);

                    SqlCommand checkStock = new SqlCommand(
                        "SELECT Quantity FROM Products WHERE Id=@id",
                        db, transaction);

                    checkStock.Parameters.AddWithValue("@id", productId);

                    int stock = (int)checkStock.ExecuteScalar();

                    if (stock < quantity)
                    {
                        MessageBox.Show("Недостаточно товара!");
                        transaction.Rollback();
                        return;
                    }

                    decimal total = quantity * price;
                    total -= total * (discount / 100);

                    SqlCommand insertSale = new SqlCommand(
                        @"INSERT INTO Sales
                  (ProductId, ClientId, Quantity, PricePerUnit, Discount, TotalAmount, SaleDateTime, CheckNumber)
                  VALUES (@pId, @cId, @q, @price, @disc, @total, GETDATE(), @check)",
                        db, transaction);

                    insertSale.Parameters.AddWithValue("@pId", productId);
                    insertSale.Parameters.AddWithValue("@cId", clientId);
                    insertSale.Parameters.AddWithValue("@q", quantity);
                    insertSale.Parameters.AddWithValue("@price", price);
                    insertSale.Parameters.AddWithValue("@disc", discount);
                    insertSale.Parameters.AddWithValue("@total", total);
                    insertSale.Parameters.AddWithValue("@check", Guid.NewGuid().ToString().Substring(0, 8));

                    insertSale.ExecuteNonQuery();

                    SqlCommand updateProduct = new SqlCommand(
                        "UPDATE Products SET Quantity = Quantity - @q WHERE Id=@id",
                        db, transaction);

                    updateProduct.Parameters.AddWithValue("@q", quantity);
                    updateProduct.Parameters.AddWithValue("@id", productId);

                    updateProduct.ExecuteNonQuery();

                    transaction.Commit();
                    LoadSales();
                    MessageBox.Show("Продажа выполнена ✔");
                }
                catch
                {
                    transaction.Rollback();
                    MessageBox.Show("Ошибка при продаже");
                }
                cbSaleProduct_SelectionChanged(null, null);
            }
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            new FormStockReport().Show();
        }

        private void BtnBackup_Click(object sender, RoutedEventArgs e)
        {
            new FormDatabaseBackup().Show();
        }
    }
}
