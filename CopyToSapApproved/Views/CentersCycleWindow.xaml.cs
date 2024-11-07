using CopyToSapApproved.Helper;
using CopyToSapApproved.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace CopyToSapApproved.Views;

/// <summary>
/// Interaction logic for CentersCycleWindow.xaml
/// </summary>
public partial class CentersCycleWindow : UserControl
{
    private readonly DatabaseHelper _databaseHelper = new();
    private List<Dictionary<string , object>> centersCycle;

    public CentersCycleWindow()
    {
        InitializeComponent();
        ListData.ItemsSource = centersCycle = _databaseHelper.GetAllData("CentersCycle");
        Counter();
    }

    private void ListData_SelectionChanged(object sender , SelectionChangedEventArgs e)
    {
        try
        {
            var selectedItem = ListData.SelectedItem;
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mainWindow.TxtID.Text = $"{((dynamic)selectedItem)["ID"]}";
            mainWindow.TxtShortText.Text = $"{((dynamic)selectedItem)["ShortText"]}";
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
        ListData.ItemsSource = centersCycle = DatabaseHelper.SearchCentersCycle(txtService.Text);
        Counter();
    }

    private void Counter()
    {
        txtSearch.Text = $"({centersCycle.Count()})";
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
            ListData.ItemsSource = centersCycle = _databaseHelper.GetAllData("CentersCycle");
            Counter();
            return;
        }

        switch(GetSelectedRadioButtonContent(StackService))
        {
            case "ChBInstallation":
                ListData.ItemsSource = centersCycle = DatabaseHelper.SearchCentersCycle("تركيب");
                break;
            case "ChBMaintenance":
                ListData.ItemsSource = centersCycle = DatabaseHelper.SearchCentersCycle("ص ");
                break;
            case "ChBNotFinish":
                ListData.ItemsSource = centersCycle = DatabaseHelper.SearchCentersCycle("منتهي");
                break;
            case
                "ChBFreeOfCharge":
                ListData.ItemsSource = centersCycle = DatabaseHelper.SearchCentersCycle("مقابل");
                break;
        }
        Counter();
    }

    private void TxtNumber_PreviewTextInput(object sender , System.Windows.Input.TextCompositionEventArgs e)
    {
        // التحقق من أن المدخل هو رقم فقط
        e.Handled = !int.TryParse(e.Text , out _);
    }
}