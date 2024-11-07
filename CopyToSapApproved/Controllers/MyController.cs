using CopyToSapApproved.Views;
using System.Linq;
using System.Windows;

namespace CopyToSapApproved.Controllers
{
    public class MyController
    {
        public static void SparePartsCopyToNots(string AppendText)
        {             // استخدام LINQ للعثور على MainWindow مباشرة
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if(mainWindow != null)
            {
                if(!string.IsNullOrEmpty(AppendText))
                {
                    mainWindow.EditorRichBox.AppendText(AppendText);
                }
                else
                {
                    mainWindow.EditorRichBox.Document.Blocks.Clear();
                }
            }

        }

        //public static void LblMessgeVisibility(string message , Brush color , int time = 10000)
        //{
        //    // استخدام LINQ للعثور على MainWindow مباشرة
        //    var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

        //    if(mainWindow != null)
        //    {
        //        mainWindow.LblMessage.Foreground = color;
        //        mainWindow.LblMessage.Text = message;

        //        // تعريف المؤقت لتغيير الخلفية كل فترة وجيزة
        //        var timer = new DispatcherTimer
        //        {
        //            Interval = TimeSpan.FromMilliseconds(500) // وميض كل نصف ثانية
        //        };

        //        int elapsed = 0;
        //        bool isOriginalColor = true;
        //        Brush originalBackground = Brushes.Black; // حفظ الخلفية الأصلية

        //        // تنفيذ كل مرة يتم تفعيل المؤقت
        //        timer.Tick += (sender , args) =>
        //        {
        //            if(elapsed >= time)
        //            {
        //                // إيقاف المؤقت بعد مرور الوقت المحدد
        //                timer.Stop();
        //                mainWindow.LblMessage.Background = originalBackground; // استعادة الخلفية الأصلية
        //                //mainWindow.LblMessage.Text = ""; // تفريغ الرسالة بعد انتهاء الوقت
        //            }
        //            else
        //            {
        //                // تبديل الخلفية بين اللون الأصلي واللون الجديد
        //                mainWindow.borderLblMessage.BorderBrush = isOriginalColor ? color : originalBackground;
        //                isOriginalColor = !isOriginalColor;
        //                elapsed += (int)timer.Interval.TotalMilliseconds;
        //            }
        //        };

        //        // بدء المؤقت
        //        timer.Start();

        //        //if(Time > 0)
        //        //{
        //        //    Thread.Sleep(Tim);
        //        //    LblMessage.Content = "";
        //        //}
        //    }
        //}
    }
}