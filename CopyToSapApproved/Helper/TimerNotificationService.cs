using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CopyToSapApproved.Helper;

public class TimerNotificationService
{
    private readonly DispatcherTimer _timer;
    private readonly DatabaseHelper _databaseHelper;
    private readonly NotifyIconHelper _notifyIconHelper;

    public TimerNotificationService()
    {
        _databaseHelper = new DatabaseHelper();
        _notifyIconHelper = new NotifyIconHelper();

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(1) // فحص دوري كل دقيقة
        };
        _timer.Tick += CheckNotifications;
    }

    public void Start()
    {
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
    }

    private void CheckNotifications(object sender , EventArgs e)
    {
        var notes = _databaseHelper.GetPendingNotifications();
        foreach(var note in notes)
        {
            // عرض الإشعار
            _notifyIconHelper.ShowNotification(note["Title"].ToString() , note["MyNote"].ToString());

            // تحديث حالة الإشعار إلى "تم عرضه"
            _databaseHelper.MarkNotificationAsShown((int)note["ID"]);
        }
    }
}