using CopyToSapApproved.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CopyToSapApproved.Views
{
    /// <summary>
    /// Interaction logic for FinalNotesWindow.xaml
    /// </summary>
    public partial class FinalNotesWindow : UserControl
    {
        private readonly DatabaseHelper _databaseHelper = new();
        private List<Dictionary<string , object>> finalNotes;
        public FinalNotesWindow()
        {
            InitializeComponent();
            ListData.ItemsSource = finalNotes = _databaseHelper.GetAllData("FinalNotes");
            Counter();
        }

        private void ListData_SelectionChanged(object sender , SelectionChangedEventArgs e)
        {
            try
            {
                var selectedItem = ListData.SelectedItem;
                var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                mainWindow.EditorRichBox.Document.Blocks.Clear();
                mainWindow.EditorRichBox.AppendText($"{((dynamic)selectedItem)["Notes"]}");
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

        private void SearchData()
        {
            ListData.ItemsSource = finalNotes = DatabaseHelper.SearchFinalNotes(txtService.Text);
            Counter();
        }

        private void Counter()
        {
            txtSearch.Text = $"({finalNotes.Count()})";
        }

        public string GetSelectedRadioButtonContent(Panel container)
        {
            // التكرار على جميع العناصر داخل الحاوية
            foreach(var child in container.Children)
            {
                // التحقق من أن العنصر هو RadioButton
                if(child is RadioButton radioButton && radioButton.IsChecked == true)
                {
                    // إعادة محتوى الزر المختار
                    return radioButton.Name.ToString();
                }
            }

            // إعادة null إذا لم يكن هناك أي زر راديو مختار
            return null;
        }

        private void ChBAll_Click(object sender , RoutedEventArgs e)
        {
            if(ChBAll.IsChecked == true)
            {
                ListData.ItemsSource = finalNotes = _databaseHelper.GetAllData("FinalNotes");
                Counter();
                return;
            }

            switch(GetSelectedRadioButtonContent(StackService))
            {
                case "ChBInstallation":
                    ListData.ItemsSource = finalNotes = DatabaseHelper.SearchFinalNotes("تركيب");
                    break;
                case "ChBMaintenance":
                    ListData.ItemsSource = finalNotes = DatabaseHelper.SearchFinalNotes("كارتة");
                    break;
                case "ChBNotFinish":
                    ListData.ItemsSource = finalNotes = DatabaseHelper.SearchFinalNotes("سحب");
                    break;
                case
                    "ChBFreeOfCharge":
                    ListData.ItemsSource = finalNotes = DatabaseHelper.SearchFinalNotes("شحن");
                    break;
            }
            Counter();
        } 
    }
}