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
using WpfApp1.Services;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для FormConnection.xaml
    /// </summary>
    public partial class FormConnection : Window
    {
        private string cs = "";
        private UserSettings settings;

        public FormConnection()
        {
            InitializeComponent();

            settings = SettingsService.Load();

            if (settings.RememberMe)
            {
                tBoxLogin.Text = settings.SavedLogin;
                tBoxPassword.Text = settings.SavedPassword;
                cBoxRememberMe.IsChecked = true;
            }

            //cBoxServer.Items.Add(@".\SQLEXPRESS");
            cBoxServer.Items.Add(@"(localdb)\MSSQLLocalDB");
            cBoxServer.SelectedIndex = 0;
        }

        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            labelError.Foreground = Brushes.Gray;
            labelError.Text = "Идёт проверка...";

            btnCreateDb.IsEnabled = false;
            btnConnect.IsEnabled = false;

            cs = $"Data Source={cBoxServer.Text};Initial Catalog=FlexClub;Integrated Security=True;TrustServerCertificate=True;";

            try
            {
                using (SqlConnection db = new SqlConnection(cs))
                {
                    db.Open();

                    SqlCommand cmd = new SqlCommand(
                        "SELECT name FROM sys.databases", db);

                    SqlDataReader reader = cmd.ExecuteReader();

                    bool exists = false;

                    while (reader.Read())
                    {
                        if (reader[0].ToString() == "FlexClub")
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (exists)
                    {
                        labelError.Foreground = Brushes.Green;
                        labelError.Text = "База данных найдена ✔";
                        App.ConnectionString = cs;

                        btnConnect.IsEnabled = true;
                        btnCreateDb.IsEnabled = false;
                    }
                    else
                    {
                        labelError.Foreground = Brushes.Orange;
                        labelError.Text = "База не найдена. Можно создать.";

                        btnCreateDb.IsEnabled = true;
                        btnConnect.IsEnabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                labelError.Foreground = Brushes.Red;
                labelError.Text = ex.Message;
            }
        }

        private void BtnCreateDB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection db = new SqlConnection(cs))
                {
                    db.Open();

                    SqlCommand cmd = new SqlCommand(
                        "CREATE DATABASE FlexClub", db);
                    cmd.ExecuteNonQuery();
                }

                cs = cs.Replace("master", "FlexClub");

                using (SqlConnection db = new SqlConnection(cs))
                {
                    db.Open();

                    string command = @"
                    CREATE TABLE ProductCategory (
                        Id INT IDENTITY PRIMARY KEY,
                        Name NVARCHAR(100) NOT NULL,
                        Description NVARCHAR(500)
                    );
                    
                    CREATE TABLE Products (
                        Id INT IDENTITY PRIMARY KEY,
                        Name NVARCHAR(200) NOT NULL,
                        Barcode NVARCHAR(100) UNIQUE,
                        PurchasePrice DECIMAL(10,2) NOT NULL,
                        CategoryId INT NOT NULL,
                        SellingPrice DECIMAL(10,2) NOT NULL,
                        Description NVARCHAR(1000),
                        FOREIGN KEY (CategoryId) REFERENCES ProductCategory(Id)
                    );
                    
                    CREATE TABLE Clients (
                        Id INT IDENTITY PRIMARY KEY,
                        FullName NVARCHAR(200),
                        Phone NVARCHAR(20) UNIQUE,
                        Email NVARCHAR(100),
                        RegistrationDate DATE DEFAULT GETDATE(),
                        Balance DECIMAL(10,2) DEFAULT 0,
                        BonusPoints INT DEFAULT 0,
                        Description NVARCHAR(500)
                    );
                    
                    CREATE TABLE Suppliers (
                        Id INT IDENTITY PRIMARY KEY,
                        Name NVARCHAR(200) NOT NULL,
                        Phone NVARCHAR(20),
                        Email NVARCHAR(100),
                        RegistrationDate DATE DEFAULT GETDATE()
                    );
                    
                    CREATE TABLE Employees (
                        Id INT IDENTITY PRIMARY KEY,
                        Name NVARCHAR(20) NOT NULL,
                        SName NVARCHAR(20) NOT NULL,
                        Patronomic NVARCHAR(20),
                        Login NVARCHAR(50) UNIQUE NOT NULL,
                        Password NVARCHAR(255) NOT NULL,
                        BirthDate DATE,
                        Phone NVARCHAR(20),
                        Email NVARCHAR(100),
                        CreatedAt DATETIME DEFAULT GETDATE(),
                        Note NVARCHAR(MAX)
                    );
                    
                    CREATE TABLE Receipts (
                        Id INT IDENTITY PRIMARY KEY,
                        ProductId INT NOT NULL,
                        SupplierId INT NOT NULL,
                        Quantity INT NOT NULL,
                        PricePerUnit DECIMAL(10,2) NOT NULL,
                        ReceiptDate DATETIME DEFAULT GETDATE(),
                        Description NVARCHAR(500),
                        FOREIGN KEY (ProductId) REFERENCES Products(Id),
                        FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id)
                    );
                    
                    CREATE TABLE Sales (
                        Id INT IDENTITY PRIMARY KEY,
                        ProductId INT NOT NULL,
                        ClientId INT NOT NULL,
                        Quantity INT NOT NULL,
                        PricePerUnit DECIMAL(10,2) NOT NULL,
                        Discount DECIMAL(5,2) DEFAULT 0,
                        TotalAmount DECIMAL(10,2) NOT NULL,
                        SaleDateTime DATETIME DEFAULT GETDATE(),
                        CheckNumber NVARCHAR(50),
                        FOREIGN KEY (ProductId) REFERENCES Products(Id),
                        FOREIGN KEY (ClientId) REFERENCES Clients(Id)
                    );

                    CREATE TABLE EmployeeImages
                    (
                        Id INT IDENTITY PRIMARY KEY,
                        EmployeeId INT NOT NULL,
                        ImageData VARBINARY(MAX) NOT NULL,
                        FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE
                    );
                    ";

                    SqlCommand cmd = new SqlCommand(command, db);
                    cmd.ExecuteNonQuery();
                }

                labelError.Foreground = Brushes.Green;
                labelError.Text = "База данных успешно создана!";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (cBoxRememberMe.IsChecked == true)
            {
                settings.SavedLogin = tBoxLogin.Text;
                settings.SavedPassword = tBoxPassword.Text;
                settings.RememberMe = true;
            }
            else
            {
                settings = new UserSettings();
            }

            string login = tBoxLogin.Text;
            string password = tBoxPassword.Text;

            string hashedPassword = PasswordHelper.HashPassword(password);

            using (SqlConnection db = new SqlConnection(App.ConnectionString))
            {
                db.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Employees WHERE Login=@login AND Password=@password", db);

                cmd.Parameters.AddWithValue("@login", login);
                cmd.Parameters.AddWithValue("@password", hashedPassword);

                int count = (int)cmd.ExecuteScalar();

                if (count == 1)
                {
                    FormMain main = new FormMain();
                    main.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль");
                }
            }
        }
    }
}

