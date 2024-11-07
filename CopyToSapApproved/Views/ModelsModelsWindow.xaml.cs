using CopyToSapApproved.Helper;
using CopyToSapApproved.Models;
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
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace CopyToSapApproved.Views;

/// <summary>
/// Interaction logic for ModelsModelsWindow.xaml
/// </summary>
public partial class ModelsModelsWindow : UserControl
{
    private readonly DatabaseHelper _databaseHelper = new();
    private List<Dictionary<string , object>> modelsModels;

    public ModelsModelsWindow()
    {
        InitializeComponent();
        ListData.ItemsSource = modelsModels = _databaseHelper.GetAllData("ModelsModels");

        // تحميل البيانات من قاعدة البيانات إلى ComboBox
        CompoModels.ItemsSource = DatabaseHelper.GetPartFromModelsModels();

        // تحديد عنصر معين بشكل برمجي (مثلاً العنصر الأول)
        CompoModels.SelectedIndex = 0; // لتحديد العنصر رقم 2
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
        txtSearch.Text = $"({modelsModels.Count()})";
    }

    private void SearchData()
    {
        ListData.ItemsSource = modelsModels = DatabaseHelper.SearchModelsModels(txtMModels.Text , txtPart.Text);
        Counter();
    }

    private void BtnSearch_Click(object sender , RoutedEventArgs e)
    {
        SearchData();
    }

    // حدث يتعامل مع تغيير العنصر المختار في ComboBox
    private void CompoModels_SelectionChanged(object sender , SelectionChangedEventArgs e)
    {
        // التحقق من أن العنصر المحدد ليس فارغاً
        if(CompoModels.SelectedItem != null)
        {
            // الوصول إلى القيم المختارة بشكل آمن
            var selectedPart = (CompoModels.SelectedItem as Dictionary<string , object>)?["Part"]?.ToString();

            // تعيين النص في ComboBox فقط إذا كانت القيمة غير فارغة
            if(!string.IsNullOrEmpty(selectedPart))
            {
                
                CompoModels.Text = selectedPart;
                txtPart.Text = selectedPart;
            }
        }
    }
}