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
    /// Логика взаимодействия для FormMain.xaml
    /// </summary>
    public partial class FormMain : Window
    {
        public FormMain()
        {
            InitializeComponent();
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
    }
}
