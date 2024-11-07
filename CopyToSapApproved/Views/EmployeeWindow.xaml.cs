using CopyToSapApproved.Helper;
using CopyToSapApproved.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CopyToSapApproved.Views
{
    /// <summary>
    /// Interaction logic for EmployeeWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : UserControl
    {
        private readonly DatabaseHelper _databaseHelper = new();
        private List<Dictionary<string , object>> employee;
        public EmployeeWindow()
        {
            InitializeComponent();
            ListData.ItemsSource = employee = _databaseHelper.GetAllData("Employee");

            // تحميل البيانات من قاعدة البيانات إلى ComboBox
            CompoModels.ItemsSource = DatabaseHelper.GetBranchFromEmployee();
            Counter();
        }

        private void ListData_SelectionChanged(object sender , SelectionChangedEventArgs e)
        {
            try
            {
                var selectedItem = ListData.SelectedItem;
                var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                mainWindow.TxtEmpolyeeCod.Text = $"{((dynamic)selectedItem)["Code"]}";
                mainWindow.TxtEmpolyeeName.Text = $"{((dynamic)selectedItem)["Name"]}";
                mainWindow.TxtEmpolyeeVendor.Text = $"{((dynamic)selectedItem)["Vendor"]}";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtSearch_KeyDown(object sender , KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                SearchData();
                if(sender is TextBox textBox)
                {
                    textBox.Focus(); // إعادة التركيز إلى الـ TextBox الذي استدعى الدالة
                }
            }
        }

        private void Counter()
        {
            txtSearch.Text = $"({employee.Count()})";
        }

        private void SearchData()
        {
            ListData.ItemsSource = employee = DatabaseHelper.SearchEmployee(txtCode.Text , txtName.Text , txtBranch.Text);
            Counter();
        }
  
        // حدث يتعامل مع تغيير العنصر المختار في ComboBox
        private void CompoModels_SelectionChanged(object sender , SelectionChangedEventArgs e)
        {
            // التحقق من أن العنصر المحدد ليس فارغاً
            if(CompoModels.SelectedItem != null)
            {
                // الوصول إلى القيم المختارة بشكل آمن
                var selectedPart = (CompoModels.SelectedItem as Dictionary<string , object>)?["Branch"]?.ToString();

                // تعيين النص في ComboBox فقط إذا كانت القيمة غير فارغة
                if(!string.IsNullOrEmpty(selectedPart))
                {
                    CompoModels.Text = selectedPart;
                    txtBranch.Text = selectedPart;
                }
            }
        }

        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private new void TxtNumber_PreviewTextInput(object sender , TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
            txtCode.Focus();
        } 
    }
}