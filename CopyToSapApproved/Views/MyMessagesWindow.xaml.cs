using CopyToSapApproved.Controllers;
using CopyToSapApproved.Helper;
using CopyToSapApproved.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CopyToSapApproved.Views;

/// <summary>
/// Interaction logic for MyMessagesWindow.xaml
/// </summary>
public partial class MyMessagesWindow : Window
{ 
    public ObservableCollection<MyMessage> _Messages { get; set; }

    public MyMessagesWindow()
    {
        InitializeComponent();

        #region تثبيت التطبيق في المقدمة
        Dispatcher.Invoke(() =>
        {
            this.Activate();
            this.WindowState = System.Windows.WindowState.Normal;
            this.Topmost = true;
            this.Focus();
        });
        #endregion

        // افتراضيا، جلب البيانات من الخدمة       
        RefreshGrid();
    }

    private void RefreshGrid()
    {
        CollectionMessages.ItemsSource = _Messages = MyMessageService._messageList;
    }

    private void Button_Clicked(object sender , RoutedEventArgs e)
    {
        this.Close();
    }

    private void AppSettings_Loaded(object sender , RoutedEventArgs e)
    {
        // احصل على دقة الشاشة
        var screenWidth = SystemParameters.WorkArea.Width;
        var screenHeight = SystemParameters.WorkArea.Height;

        // تعيين ارتفاع النافذة ليكون بملء الشاشة مع مراعاة شريط المهام
        //this.Height = screenHeight;

        // تعيين عرض النافذة إلى 400 بكسل5
        //this.Width = 400;

        // تعيين موقع النافذة إلى أقصى يمين الشاشة
        this.Left = screenWidth - this.Width;
        this.Top = (screenHeight - this.Height) / 2;
        //this.Top = 0;
    }
}
