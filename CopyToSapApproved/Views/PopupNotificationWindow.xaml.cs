using CopyToSapApproved.Controllers;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Data.SQLite;
using System.Media;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace CopyToSapApproved.Views;

/// <summary>
/// Interaction logic for PopupNotificationWindow.xaml
/// </summary>
public partial class PopupNotificationWindow : Window
{
    private readonly DispatcherTimer _timer;

    public PopupNotificationWindow(string title , string message , int durationSeconds = 15)
    {
        InitializeComponent();
        try
        {
            TitleText.Text = title;
            MessageText.Text = message;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(durationSeconds)
            };
            _timer.Tick += (s , e) =>
            {
                _timer.Stop();
                Close();
            };
            _timer.Start();
            ShowNotification();
        }
        catch(Exception ex)
        {
            _=MyMessageService.ShowMessage("خطأ4: " + ex.Message , Brushes.IndianRed);
        }
    }

    public void ShowNotification()
    {
        try
        {
            // Position the notification at the bottom-right corner of the screen
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width - 50;
            Top = desktopWorkingArea.Bottom - Height - 0;

            Show();
        }
        catch(Exception ex)
        {
            _=MyMessageService.ShowMessage("خطأ3: " + ex.Message , Brushes.IndianRed);
        }
    } 
}