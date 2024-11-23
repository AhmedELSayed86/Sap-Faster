using System;
using System.Windows;
using System.Windows.Threading;

namespace CopyToSapApproved.Views;

/// <summary>
/// Interaction logic for PopupNotificationWindow.xaml
/// </summary>
public partial class PopupNotificationWindow : Window
{
    private readonly DispatcherTimer _timer;

    public PopupNotificationWindow(string title , string message , int durationSeconds = 5)
    {
        InitializeComponent();

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
    }

    public void ShowNotification()
    {
        // Position the notification at the bottom-right corner of the screen
        var desktopWorkingArea = SystemParameters.WorkArea;
        Left = desktopWorkingArea.Right - Width - 10;
        Top = desktopWorkingArea.Bottom - Height - 10;

        Show();
        _timer.Start();
    }
}