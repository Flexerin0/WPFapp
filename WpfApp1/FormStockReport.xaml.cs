using Microsoft.Data.SqlClient;
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
using WpfApp1.Models;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для FormStockReport.xaml
    /// </summary>
    public partial class FormStockReport : Window
    {
        private List<StockReportItem> _currentReport;
        public FormStockReport()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCategories();
        }

        private void cbCategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_currentReport == null) return;

            if (cbCategoryFilter.SelectedIndex == 0)
            {
                dgStockReport.ItemsSource = _currentReport;
                CalculateTotals(_currentReport);
                return;
            }

            string categoryName = cbCategoryFilter.Text;

            var filtered = _currentReport
                .Where(x => x.CategoryName == categoryName)
                .ToList();

            dgStockReport.ItemsSource = filtered;
            CalculateTotals(filtered);
        }

        private void LoadCategories()
        {
            using (SqlConnection db = new SqlConnection(App.ConnectionString))
            {
                db.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT Id, Name FROM ProductCategory ORDER BY Name", db);

                SqlDataReader reader = cmd.ExecuteReader();

                cbCategoryFilter.Items.Clear();
                cbCategoryFilter.Items.Add("Все категории");

                while (reader.Read())
                {
                    cbCategoryFilter.Items.Add(new
                    {
                        Id = reader["Id"],
                        Name = reader["Name"].ToString()
                    });
                }

                cbCategoryFilter.SelectedIndex = 0;
            }
        }

        private void btnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            List<StockReportItem> list = new List<StockReportItem>();

            using (SqlConnection db = new SqlConnection(App.ConnectionString))
            {
                db.Open();

                string query = @"SELECT 
                        p.Name AS ProductName,
                        c.Name AS CategoryName,
                        ISNULL(r.TotalReceived, 0) AS TotalReceived,
                        ISNULL(s.TotalSold, 0) AS TotalSold,
                        ISNULL(r.TotalReceived, 0) - ISNULL(s.TotalSold, 0) AS StockBalance,
                        p.SellingPrice,
                        (ISNULL(r.TotalReceived, 0) - ISNULL(s.TotalSold, 0)) * p.SellingPrice AS TotalStockValue
                    FROM Products p
                    INNER JOIN ProductCategory c ON p.CategoryId = c.Id
                    LEFT JOIN (
                        SELECT ProductId, SUM(Quantity) AS TotalReceived
                        FROM Receipts
                        GROUP BY ProductId
                    ) r ON p.Id = r.ProductId
                    LEFT JOIN (
                        SELECT ProductId, SUM(Quantity) AS TotalSold
                        FROM Sales
                        GROUP BY ProductId
                    ) s ON p.Id = s.ProductId
                    ORDER BY p.Name";

                SqlCommand cmd = new SqlCommand(query, db);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new StockReportItem
                    {
                        ProductName = reader["ProductName"].ToString(),
                        CategoryName = reader["CategoryName"].ToString(),
                        TotalReceived = Convert.ToInt32(reader["TotalReceived"]),
                        TotalSold = Convert.ToInt32(reader["TotalSold"]),
                        StockBalance = Convert.ToInt32(reader["StockBalance"]),
                        SellingPrice = Convert.ToDecimal(reader["SellingPrice"]),
                        TotalStockValue = Convert.ToDecimal(reader["TotalStockValue"])
                    });
                }
            }

            // 🔥 ВОТ ГДЕ СОХРАНЯЕМ ДАННЫЕ
            _currentReport = list;

            // 🔥 И ТОЛЬКО ПОТОМ ПРИВЯЗЫВАЕМ
            dgStockReport.ItemsSource = _currentReport;

            CalculateTotals(_currentReport);
        }

        private void CalculateTotals(List<StockReportItem> list)
        {
            txtTotalProducts.Text = list.Count.ToString();

            decimal totalValue = list.Sum(x => x.TotalStockValue);
            txtTotalStockValue.Text = totalValue.ToString("N2");
        }

        private void btnRefreshReport_Click(object sender, RoutedEventArgs e)
        {
            btnGenerateReport_Click(sender, e);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (_currentReport == null) return;

            string searchText = tbSearch.Text.ToLower();

            var filtered = _currentReport
                .Where(x => x.ProductName.ToLower().Contains(searchText))
                .ToList();

            dgStockReport.ItemsSource = filtered;

            CalculateTotals(filtered);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
