using CopyToSapApproved.Helper;
using System;
using System.Collections.Generic;
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

namespace CopyToSapApproved.Views
{
    /// <summary>
    /// Interaction logic for AppSettings.xaml
    /// </summary>
    public partial class AppSettingsWindow : Window
    {
        private static readonly DatabaseHelper databaseHelper = new();
        private readonly ExcelHelper excelHelper = new(databaseHelper);
        public AppSettingsWindow()
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

        private async void ExportDatabase(object sender , RoutedEventArgs e)
        {
            await ExecuteWithProgressBarAsync(async () =>
            {
                await Task.Run(() => DatabaseHelper.ExportDatabase());
            });
        }


        private async void ImportDatabase(object sender , RoutedEventArgs e)
        {
            await ExecuteWithProgressBarAsync(async () =>
            {
                await Task.Run(() => DatabaseHelper.ImportDatabase());
            });
        }

        private async void UploadSpareParts(object sender , RoutedEventArgs e)
        {
            await ExecuteWithProgressBarAsync(async () =>
            {
                await Task.Run(() => excelHelper.ImportSparePartsExcelFile());
            });
        }

        // Similar changes for other button event handlers...
        private async Task ExecuteWithProgressBarAsync(Func<Task> action)
        {
            try
            {
                OperationProgressBar.Visibility = Visibility.Visible; // إظهار البروجريس بار
                await action(); // تنفيذ العملية الطويلة
            }
            catch(Exception ex)
            {
                MessageBox.Show($"حدث خطأ: {ex.Message}" , "خطأ" , MessageBoxButton.OK , MessageBoxImage.Error);
            }
            finally
            {
                OperationProgressBar.Visibility = Visibility.Collapsed; // إخفاء البروجريس بار بعد الانتهاء
            }
        }
        private async void UploadEmployee(object sender , RoutedEventArgs e)
        {
            await ExecuteWithProgressBarAsync(async () =>
            {
                await Task.Run(() => excelHelper.ImportEmployeeExcelFile());
            });

        }

        private async void UploadFinalNotes(object sender , RoutedEventArgs e)
        {
            await ExecuteWithProgressBarAsync(async () =>
            {
                await Task.Run(() => excelHelper.ImportFinalNotesExcelFile());
            });
        }

        private async void UploadCentersCycle(object sender , RoutedEventArgs e)
        {
            await ExecuteWithProgressBarAsync(async () =>
            {
                await Task.Run(() => excelHelper.ImportCentersCycleExcelFile());
            });
        }

        private async void ClearSparPart(object sender , RoutedEventArgs e)
        {
            await ExecuteWithProgressBarAsync(async () =>
            {
                await Task.Run(() => excelHelper.ClearTable("SparePart"));
            });
        }

        private async void ClearTecnical(object sender , RoutedEventArgs e)
        {
            await ExecuteWithProgressBarAsync(async () =>
            {
                await Task.Run(() => excelHelper.ClearTable("Employee"));
            });
        }

        private async void ClearFinalNotes(object sender , RoutedEventArgs e)
        {
            await ExecuteWithProgressBarAsync(async () =>
            {
                await Task.Run(() => excelHelper.ClearTable("FinalNotes"));
            });
        }

        private async void ClearCentersCycle(object sender , RoutedEventArgs e)
        {
            await ExecuteWithProgressBarAsync(async () =>
            {
                await Task.Run(() => excelHelper.ClearTable("CentersCycle"));
            });
        }

        private async void UploadModels(object sender , RoutedEventArgs e)
        {
            await ExecuteWithProgressBarAsync(async () =>
            {
                await Task.Run(() => excelHelper.ImportModelsModelsExcelFile());
            });
        }

        private async void ClearModels(object sender , RoutedEventArgs e)
        {
            await ExecuteWithProgressBarAsync(async () =>
            {
                await Task.Run(() => excelHelper.ClearTable("ModelsModels"));
            });
        }

        private async void ClearMyNotes(object sender , RoutedEventArgs e)
        {
            await ExecuteWithProgressBarAsync(async () =>
            {
                await Task.Run(() => excelHelper.ClearTable("MyNotes"));
            });
        }

        private void DeleteDatabase(object sender , RoutedEventArgs e)
        {
            DatabaseHelper.DeleteDatabase();
        }
    }
}