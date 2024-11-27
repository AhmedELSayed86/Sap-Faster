using CopyToSapApproved.Models;
using CopyToSapApproved.Views;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CopyToSapApproved.Controllers
{
    public static class MyMessageService
    {
        private static ConcurrentQueue<MyMessage> _messageQueue = new ConcurrentQueue<MyMessage>();
        public static ObservableCollection<MyMessage> _messageList = new ObservableCollection<MyMessage>();
        private static bool _isShowingMessage = false;

        public static async Task ShowMessage(string content , Brush color , int priority = 3 , bool isFlashing = true , int duration = 1000)
        {
            var message = new MyMessage
            {
                Content = content ,
                Priority = priority ,
                IsFlashing = isFlashing ,
                MyColor = color ,
                Duration = duration
            };

            _messageQueue.Enqueue(message);
            _messageList.Add(message);

            // بدء معالجة الرسائل فقط إذا لم تكن هناك معالجة حالية
            if(!_isShowingMessage)
            {
                await ProcessMessages();
            }
        }

        private static async Task ProcessMessages()
        {
            _isShowingMessage = true;

            while(_messageQueue.TryDequeue(out var messageToShow))
            {
                await DisplayMessage(messageToShow.Content , messageToShow.MyColor , messageToShow.IsFlashing , messageToShow.Duration);

                int elapsed = 0;
                while(elapsed < messageToShow.Duration)
                {
                    await Task.Delay(200); // تحقق كل 200 ملي ثانية
                    elapsed += 200;
                }
            }

            _isShowingMessage = false;
        }

        private static async Task DisplayMessage(string message , Brush color , bool isFlashing , int duration)
        {
            var mainWindow = Application.Current.Dispatcher.Invoke(() =>
                Application.Current.Windows.OfType<MainWindow>().FirstOrDefault());

            if(mainWindow != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    mainWindow.LblMessage.Foreground = color;
                    mainWindow.LblMessage.Text = message;
                });

                if(isFlashing)
                {
                    int elapsed = 0;
                    bool isOriginalColor = true;
                    Brush originalBackground = Brushes.Black;

                    while(elapsed < duration)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            mainWindow.borderLblMessage.BorderBrush = isOriginalColor ? color : originalBackground;
                        });

                        isOriginalColor = !isOriginalColor;
                        await Task.Delay(500); // زمن تبديل اللون
                        elapsed += 200;
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        mainWindow.borderLblMessage.Background = originalBackground;
                    });
                }

                await Task.Delay(duration);
            }
        }
    }
}
