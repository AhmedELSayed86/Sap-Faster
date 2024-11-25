using CopyToSapApproved.Controllers;
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
/// Interaction logic for PopupMessagesWindow.xaml
/// </summary>
public partial class PopupMessagesWindow : Window
{
    public ObservableCollection<MyMessage> Messages { get; set; }

    public PopupMessagesWindow()
    {
        InitializeComponent();
        // افتراضيا، جلب البيانات من الخدمة
        Messages = MessageService._messageList;

        // تعيين المصدر إلى ItemsControl
        CollectionMessages.ItemsSource = Messages;
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
