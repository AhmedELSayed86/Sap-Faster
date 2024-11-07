using CopyToSapApproved.Controllers;
using CopyToSapApproved.Helper;
using CopyToSapApproved.Models;
using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
    /// Interaction logic for SparePartsSearchWindow.xaml
    /// </summary>
    public partial class SparePartsSearchWindow : UserControl
    {
        private readonly DatabaseHelper _databaseHelper = new();
        private List<Dictionary<string , object>> _sparePart;

        public SparePartsSearchWindow()
        {
            InitializeComponent();
            DataGridData.ItemsSource = _sparePart = _databaseHelper.GetAllData("SparePart");
            Counter();
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
            txtSearch.Text = $"({_sparePart.Count()})";
        }

        private void SearchData()
        {
            DataGridData.ItemsSource = _sparePart = DatabaseHelper.SearchSpareParts(txtCode.Text , txtGrope.Text , txtModel.Text , txtDescriptionAR.Text);
            Counter();
        }

        private void DataGridData_SelectionChanged(object sender , SelectionChangedEventArgs e)
        {

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