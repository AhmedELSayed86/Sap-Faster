using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Drawing;
using System;
using System.Windows;
using CopyToSapApproved.Views;

namespace CopyToSapApproved.Helper;

        public class NotifyIconHelper
        {


    public void ShowNotification(string title , string message)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var notification = new PopupNotificationWindow(title , message)
            {
                Left = SystemParameters.WorkArea.Width - 310 , // حافة الشاشة اليمنى
                Top = SystemParameters.WorkArea.Height - 110  // حافة الشاشة السفلية
            };
            notification.Show();
        });
    }
}
