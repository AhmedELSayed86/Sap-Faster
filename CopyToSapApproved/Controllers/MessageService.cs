using CopyToSapApproved.Models;
using CopyToSapApproved.Views;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CopyToSapApproved.Controllers;

public static class MessageService
{
    private static ConcurrentQueue<MyMessage> _messageQueue = [];
    private static bool _isShowingMessage = false;

    public static async Task ShowMessage(string content , Brush color , int priority = 3 , bool isFlashing = false , int duration = 10000)
    {
        var message = new MyMessage { Content = content , Priority = priority , IsFlashing = isFlashing , Color = color , Duration = duration };

        _messageQueue.Enqueue(message);

        // بدء معالجة الرسائل فقط إذا لم تكن هناك معالجة حالية
        if(!_isShowingMessage)
        {
            await ProcessMessages();
        }
    }

    private static async Task ProcessMessages()
    {
        _isShowingMessage = true;

        while(_messageQueue.Count > 0)
        {
            // استخراج الرسائل وترتيبها بناءً على الأولوية فقط إذا كان هناك أكثر من رسالة
            var messages = _messageQueue.ToList();
            var messageToShow = messages.OrderBy(m => m.Priority).FirstOrDefault();

            // إزالة الرسالة من القائمة
            _messageQueue = new ConcurrentQueue<MyMessage>(messages.Skip(1));

            if(messageToShow != null)
            {
                await DisplayMessage(messageToShow.Content , messageToShow.Color , messageToShow.IsFlashing , messageToShow.Duration);

                // عدم انتظار مدة الرسالة بالكامل، بدلاً من ذلك التحقق من المدة المتبقية أثناء العرض
                int elapsed = 0;
                while(elapsed < messageToShow.Duration)
                {
                    await Task.Delay(200); // التحقق كل 200 ملي ثانية
                    elapsed += 200;
                }
            }
        }

        _isShowingMessage = false;
    }

    private static async Task DisplayMessage(string message , Brush color , bool isFlashing , int duration)
    {
        var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

        if(mainWindow != null)
        {
            mainWindow.LblMessage.Foreground = color;
            mainWindow.LblMessage.Text = message;

            if(isFlashing)
            {
                int elapsed = 0;
                bool isOriginalColor = true;
                Brush originalBackground = mainWindow.LblMessage.Background;

                while(elapsed < duration)
                {
                    // تغيير الخلفية بالتناوب بين الأصلية و لون الفلاش
                    mainWindow.borderLblMessage.BorderBrush = isOriginalColor ? color : originalBackground;
                    isOriginalColor = !isOriginalColor;

                    // الانتظار لفترة قصيرة لضبط الفلاش
                    await Task.Delay(500);
                    elapsed += 500;
                }

                // إعادة اللون الأصلي بعد انتهاء الفلاش
                mainWindow.LblMessage.Background = originalBackground;
            }

            await Task.Delay(duration);
        }
    }
}