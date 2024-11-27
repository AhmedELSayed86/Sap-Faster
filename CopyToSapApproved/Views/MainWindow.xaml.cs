using CopyToSapApproved.Controllers;
using CopyToSapApproved.Helper;

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace CopyToSapApproved.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly string isTableName = "";

    public string CentersCycleID = "";
    public string CentersCycleShortText = "";

    public string EmployeeCode = "";
    public string EmployeeName = "";
    public string EmployeeVendor = "";

    public string FinalNotesSAPStatus = "";
    public string FinalNotesNotse = "";

    private int CounterForZeker = 0;

    private bool _isUpdatingText = false;

    private readonly DatabaseHelper _databaseHelper = new();

    private readonly List<Dictionary<string , object>> employee;
    private readonly List<Dictionary<string , object>> centersCycle;

    //private readonly ExcelHelper excelHelper = new(_databaseHelper);
    private Window appWindow;

    private DispatcherTimer _notificationTimer;
    //private readonly List<Employee> _employees; // List to hold employee data

    public MainWindow()
    {
        InitializeComponent();

        #region تثبيت التطبيق في المقدمة
        Dispatcher.Invoke(() =>
        {
            this.WindowState = System.Windows.WindowState.Normal;
            this.Topmost = true;
            this.Activate();
            this.Focus();
        });
        #endregion

        //ChangeTableData("FinalNotes");

        ComboEmployee.ItemsSource = employee = _databaseHelper.GetAllData("Employee");

        ////ComboEmployee.Items.Add("Ahmed");
        //ListData.ItemsSource = finalNotes = _databaseHelper.GetAllData("FinalNotes");

        string appName = "SAP Faster";
        string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        StartupManager.AddToStartup(appName , exePath);

        //_sparePart = []; 
        //string alterTableQuery = "ALTER TABLE MyNotes ADD AlertTime DATETIME NULL;";
        //DatabaseHelper.ExecuteQuery(alterTableQuery);

        SetupNotificationTimer();
    }

    private void SetupNotificationTimer()
    {
        _notificationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(1) // تحقق كل دقيقة
        };
        _notificationTimer.Tick += CheckForNotifications;
        _notificationTimer.Start();
    }

    private void CheckForNotifications(object sender , EventArgs e)
    {
        string resourcesPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        string dbFilePath = System.IO.Path.Combine(resourcesPath , "SAPFaster.db3");
        string connectionString = $"Data Source={dbFilePath};Version=3;";

        try
        {
            using var conn = new SQLiteConnection(connectionString);
            conn.Open();

            // الاستعلام لجلب الملاحظات المؤهلة
            string query = "SELECT * FROM MyNotes WHERE AlertTime <= @now AND Alerted < 3";
            using var cmd = new SQLiteCommand(query , conn);
            cmd.Parameters.AddWithValue("@now" , DateTime.Now);

            using var reader = cmd.ExecuteReader();
            // التحقق من وجود نتائج قبل القراءة
            if(!reader.HasRows)
            {
                // إذا لم تكن هناك ملاحظات، اخرج من الدالة
                return;
            }

            // قراءة البيانات وتنفيذ الإشعارات
            while(reader.Read())
            {
                string id = reader["ID"]?.ToString();
                string title = reader["Title"]?.ToString();
                string content = reader["Content"]?.ToString();
                string alerted = reader["Alerted"]?.ToString();
                // عرض الإشعار
                ShowNotification(title , content);

                int.TryParse(alerted , out int alertedToint);

                alertedToint++;

                // تحديث الحالة إلى "تم الإشعار"
                using var updateCmd = new SQLiteCommand("UPDATE MyNotes SET Alerted = @alerted WHERE ID = @id" , conn);
                updateCmd.Parameters.AddWithValue("@alerted" , alertedToint);
                updateCmd.Parameters.AddWithValue("@id" , id);
                updateCmd.ExecuteNonQuery();
            }
        }
        catch(SQLiteException ex)
        {
            // تسجيل أو عرض الأخطاء المتعلقة بقاعدة البيانات          
            _ = MyMessageService.ShowMessage("SQLite خطأ: " + ex.Message , Brushes.IndianRed);
        }
        catch(Exception ex)
        {
            // تسجيل أو عرض أي أخطاء عامة 
            _ = MyMessageService.ShowMessage("خطأ1: " + ex.Message , Brushes.IndianRed);
        }
    }

    private void ShowNotification(string title , string content)
    {
        // تشغيل صوت التنبيه الافتراضي
        SystemSounds.Exclamation.Play();
        try
        {
            if(appWindow == null || !appWindow.IsVisible)
            {
                // إذا كانت النافذة غير موجودة أو غير مرئية، قم بإنشاءها وفتحها
                appWindow = new PopupNotificationWindow(title , content);
                appWindow.Closed += OnAppWindowClosed; // الاشتراك في حدث الإغلاق
                appWindow.Show();
            }
            else
            {
                // إذا كانت النافذة مفتوحة بالفعل
                if(appWindow.WindowState == WindowState.Minimized)
                {
                    // إذا كانت النافذة مصغرة، استعدها إلى الحالة الطبيعية
                    appWindow.WindowState = WindowState.Normal;
                }
                // قم بتنشيط النافذة لتظهر في المقدمة
                appWindow.Activate();
            }
        }
        catch(Exception ex)
        {
            _ = MyMessageService.ShowMessage("خطأ2: " + ex.Message , Brushes.IndianRed);
        }
    }


    private void MainWindows_Loaded(object sender , RoutedEventArgs e)
    {
        // احصل على دقة الشاشة
        var screenWidth = SystemParameters.WorkArea.Width;
        var screenHeight = SystemParameters.WorkArea.Height;

        // تعيين ارتفاع النافذة ليكون بملء الشاشة مع مراعاة شريط المهام
        this.Height = screenHeight;

        // تعيين عرض النافذة إلى 400 بكسل
        this.Width = 400;

        // تعيين موقع النافذة إلى أقصى يمين الشاشة
        this.Left = screenWidth - this.Width;
        this.Top = 0;
    }

    private void TxtNumber_PreviewTextInput(object sender , System.Windows.Input.TextCompositionEventArgs e)
    {
        // التحقق من أن المدخل هو رقم فقط
        e.Handled = !int.TryParse(e.Text , out _);
    }

    public static string ConvertArabicNumbersToEnglish(string input)
    {
        var result = new StringBuilder(input.Length);

        foreach(var ch in input)
        {
            // تحويل الأرقام العربية إلى الإنجليزية باستخدام قيم الـ Unicode
            if(ch >= '٠' && ch <= '٩')
            {
                result.Append((char)(ch - '٠' + '0'));
            }
            else
            {
                result.Append(ch);
            }
        }

        return result.ToString();
    }

    //public static string ConvertArabicNumbersToEnglish(string input)
    //{
    //    var arabicToEnglishDigits = new Dictionary<char , char>
    //{
    //    {'٠', '0'},
    //    {'١', '1'},
    //    {'٢', '2'},
    //    {'٣', '3'},
    //    {'٤', '4'},
    //    {'٥', '5'},
    //    {'٦', '6'},
    //    {'٧', '7'},
    //    {'٨', '8'},
    //    {'٩', '9'}
    //};

    //    var result = new StringBuilder(input.Length);
    //    foreach(var ch in input)
    //    {
    //        if(arabicToEnglishDigits.TryGetValue(ch , out char englishDigit))
    //        {
    //            result.Append(englishDigit);
    //        }
    //        else
    //        {
    //            result.Append(ch);
    //        }
    //    }

    //    return result.ToString();
    //}

    private void ComboEmployee_SelectionChanged(object sender , SelectionChangedEventArgs e)
    {
        string name = "", code = "";
        if(ComboEmployee.SelectedItem is Dictionary<string , object> selectedItem)
        {
            // استخراج القيم من القاموس
            name = selectedItem.ContainsKey("Name") ? selectedItem["Name"].ToString() : "N/A";
            code = selectedItem.ContainsKey("Code") ? selectedItem["Code"].ToString() : "N/A";
        }
        ComboEmployee.Text = "مدخل البيانات";

        TxtCompletedBy.Text = code;
        //TxtCompletedBy.Focus();
        return;
    }

    private void ComboEmployee_PreviewKeyUp(object sender , KeyEventArgs e)
    {
        TextBox textBox = (TextBox)ComboEmployee.Template.FindName("PART_EditableTextBox" , ComboEmployee);
        if(textBox != null)
        {
            string searchText = textBox.Text.ToLower();
            var view = CollectionViewSource.GetDefaultView(ComboEmployee.ItemsSource);
            view.Filter = item =>
            {
                if(item is Dictionary<string , object> employee)
                {
                    bool matchesName = employee.ContainsKey("Name") && employee["Name"].ToString().ToLower().Contains(searchText);
                    bool matchesCode = employee.ContainsKey("Code") && employee["Code"].ToString().ToLower().Contains(searchText);
                    return matchesCode || matchesName;
                }
                return false;
            };
            view.Refresh();
            ComboEmployee.IsDropDownOpen = true; // Keep the dropdown open during filtering
        }
    }

    private void OnTextChanged(object sender , TextChangedEventArgs e)
    {
        if(_isUpdatingText)
            return; // تجنب التكرار

        try
        {
            if(sender is RichTextBox richTextBox)
            {
                _isUpdatingText = true; // نبدأ التحديث

                // قراءة النص من RichTextBox مع إزالة الأسطر الجديدة الزائدة
                string text = new TextRange(richTextBox.Document.ContentStart , richTextBox.Document.ContentEnd).Text.Replace("\r\n" , "");

                // تحديث عدد الأحرف في TextBox
                TxtCount.Text = $"{text.Length}";
                if(text.Length > 40)
                {
                    TxtCount.Foreground = Brushes.Red;
                }
                else if(text.Length <= 40)
                {
                    TxtCount.Foreground = Brushes.Aquamarine;
                }

                // الحصول على المستند الحالي
                FlowDocument document = richTextBox.Document;

                // مسح الفقرات الحالية
                document.Blocks.Clear();

                // إنشاء الفقرة الجديدة
                Paragraph paragraph = new Paragraph();

                // إضافة النص مع التلوين
                if(text.Length > 0)
                {
                    int firstPartLength = Math.Min(40 , text.Length);
                    string firstPartText = text.Substring(0 , firstPartLength);
                    Run firstRun = new Run(firstPartText)
                    {
                        Foreground = Brushes.LawnGreen
                    };
                    paragraph.Inlines.Add(firstRun);

                    if(text.Length > 40)
                    {
                        string remainingText = text.Substring(40);
                        Run remainingRun = new Run(remainingText)
                        {
                            Foreground = Brushes.Red
                        };
                        paragraph.Inlines.Add(remainingRun);
                    }
                }

                // إضافة الفقرة الجديدة إلى المستند
                document.Blocks.Add(paragraph);

                // ضبط موقع المؤشر في نهاية النص
                richTextBox.CaretPosition = richTextBox.Document.ContentEnd;
            }
        }
        catch(Exception ex)
        {
            MessageBox.Show($"خطأ: {ex.Message}");
        }
        finally
        {
            _isUpdatingText = false; // ننتهي من التحديث
        }
    }

    private void CopyNotsToEditorRichTextBox()
    {
        string columnname = "";
        // الحصول على العنصر المحدد
        var selectedItem = "";// ListData.SelectedItem;

        switch(isTableName)
        {
            case "FinalNotes":
                columnname = "Notes";
                break;
            case "CentersCycle":
                columnname = "ShortText";
                break;
            case "Employee":
                columnname = "Name";
                break;
        }


        // التحقق من وجود عنصر محدد
        if(selectedItem != null)
        {
            // تحديث محتوى الـ TextBox بناءً على البيانات من العنصر المحدد
            // يمكنك تعديل السطر التالي بناءً على كيفية تنظيم البيانات في العناصر
            EditorRichBox.Document.Blocks.Clear();
            EditorRichBox.AppendText($"{((dynamic)selectedItem)[columnname]}");
        }
        else
        {
            // تفريغ محتوى الـ TextBox إذا لم يكن هناك عنصر محدد
            EditorRichBox.AppendText(string.Empty);
        }
    }

    private void CopyListDataToCentersCycle()
    {
        // الحصول على العنصر المحدد
        var selectedItem = "";// ListData.SelectedItem;

        // التحقق من وجود عنصر محدد
        if(selectedItem != null)
        {
            // تحديث محتوى الـ TextBox بناءً على البيانات من العنصر المحدد
            // يمكنك تعديل السطر التالي بناءً على كيفية تنظيم البيانات في العناصر
            EditorRichBox.Document.Blocks.Clear();
            EditorRichBox.AppendText($"{((dynamic)selectedItem)["Notes"]}");
        }
        else
        {
            // تفريغ محتوى الـ TextBox إذا لم يكن هناك عنصر محدد
            EditorRichBox.AppendText(string.Empty);
        }
    }

    public string GetSelectedRadioButtonContent(Panel container)
    {
        // التكرار على جميع العناصر داخل الحاوية
        foreach(var child in container.Children)
        {
            // التحقق من أن العنصر هو RadioButton
            if(child is RadioButton radioButton && radioButton.IsChecked == true)
            {
                // إعادة محتوى الزر المختار
                return radioButton.Content.ToString();
            }
        }

        // إعادة null إذا لم يكن هناك أي زر راديو مختار
        return null;
    }

    private void BtnCopyActivitiyToClipboard_Click(object sender , RoutedEventArgs e)
    {
        try
        {
            if(!CopyToSAPHelper.GetProcesses())
            {
                _ = MyMessageService.ShowMessage("لم أتمكن من العثور على نافذة SAP" , Brushes.IndianRed);
                return;
            }

            string Damaged = " ", dateText = "", text = "";

            DateTime StartDate, EndDate;

            text = new TextRange(EditorRichBox.Document.ContentStart , EditorRichBox.Document.ContentEnd).Text;

            if(string.IsNullOrWhiteSpace(OrderDate.datePicker.Text))
            {
                OrderDate.datePicker.Focus();
                _ = MyMessageService.ShowMessage("قيمة التاريخ فارغة" , Brushes.Red);
                return;
            }

            if(string.IsNullOrWhiteSpace(text))
            {
                _ = MyMessageService.ShowMessage("التتميم فارغ" , Brushes.Red);
                TabControl.SetIsSelected(FinalNotesTab , true);
                EditorRichBox.Focus();
                return;
            }

            if(!string.IsNullOrWhiteSpace(TxtDamaged.Text))
            {
                Damaged = TxtDamaged.Text;
            }

            _ = MyMessageService.ShowMessage("ملحوظة: التالف فارغ." , Brushes.Yellow);
            TxtDamaged.Focus();

            dateText = OrderDate.datePicker.Text;

            StartDate = EndDate = OrderDate.datePicker.SelectedDate.Value;
            StartDate = StartDate.AddDays(-1);

            // استخدم التاريخ بالشكل الصحيح
            CopyToSAPHelper.CopyActivitiyToClipboard(
            GetSelectedRadioButtonContent(StackPanelOperationsRadioButton) ,
            text ,
            Damaged ,
            StartDate.ToString("dd.MM.yyyy" , CultureInfo.InvariantCulture) ,
            EndDate.ToString("dd.MM.yyyy" , CultureInfo.InvariantCulture)
        );

            _ = MyMessageService.ShowMessage("تم نسخ التتميم بنجاح" , Brushes.LawnGreen);

            CopyToSAPHelper.PastToSAP();

            _ = MyMessageService.ShowMessage("تم لصق التتميم بنجاح" , Brushes.LawnGreen);
        }
        catch(Exception ex)
        {
            MessageBox.Show("حدث خطأ غير متوقع: " + ex.Message);
        }
    }

    private void BtnCopyTasksToClipboard_Click(object sender , RoutedEventArgs e)
    {
        try
        {
            if(!CopyToSAPHelper.GetProcesses())
            {
                _ = MyMessageService.ShowMessage("لم أتمكن من العثور على نافذة SAP" , Brushes.IndianRed);
                return;
            }

            if(string.IsNullOrWhiteSpace(TxtBillValue.Text))
            {
                TxtBillValue.Focus();
                _ = MyMessageService.ShowMessage("قيمة الفاتورة فارغة" , Brushes.Red);
                return;
            }

            if(string.IsNullOrWhiteSpace(TxtEmpolyeeCod.Text))
            {
                TabControl.SetIsSelected(EmployeeTab , true);
                TxtEmpolyeeCod.Focus();
                _ = MyMessageService.ShowMessage("كود الفني فارغ" , Brushes.Red);
                return;
            }

            if(TxtEmpolyeeCod.Text == "كود")
            {
                TabControl.SetIsSelected(EmployeeTab , true);
                TxtEmpolyeeCod.Focus();
                _ = MyMessageService.ShowMessage("كود الفني فارغ" , Brushes.Red);
                return;
            }

            if(string.IsNullOrWhiteSpace(TxtEmpolyeeName.Text))
            {
                TabControl.SetIsSelected(EmployeeTab , true);
                TxtEmpolyeeCod.Focus();
                _ = MyMessageService.ShowMessage("اسم الفني فارغ" , Brushes.Red);
                return;
            }

            if(TxtEmpolyeeName.Text == "الفني")
            {
                TabControl.SetIsSelected(EmployeeTab , true);
                TxtEmpolyeeCod.Focus();
                _ = MyMessageService.ShowMessage("اسم الفني فارغ" , Brushes.Red);
                return;
            }

            if(string.IsNullOrWhiteSpace(TxtCompletedBy.Text))
            {
                TxtCompletedBy.Focus();
                _ = MyMessageService.ShowMessage("كود مدخل البيانات فارغ" , Brushes.Red);
                return;
            }

            CopyToSAPHelper.CopyTasksToClipboard
                (
                GetSelectedRadioButtonContent(StackPanelOperationsRadioButton) ,
                TxtBillValue.Text ,
                TxtEmpolyeeCod.Text ,
                TxtEmpolyeeName.Text ,
                TxtCompletedBy.Text
                );
            _ = MyMessageService.ShowMessage("تم نسخ التاسك بنجاح" , Brushes.LawnGreen);

            CopyToSAPHelper.PastToSAP();

            _ = MyMessageService.ShowMessage("تم لصق التاسك بنجاح" , Brushes.LawnGreen);
        }
        catch(Exception ex)
        {
            _ = MyMessageService.ShowMessage("حدث خطأ: (" + ex + ")" , Brushes.Red);
            MessageBox.Show("حدث خطأ غير متوقع: " + ex.Message);
        }
    }

    private void BtnCopyOperationsToClipboard_Click(object sender , RoutedEventArgs e)
    {
        try
        {
            if(!CopyToSAPHelper.GetProcesses())
            {
                _ = MyMessageService.ShowMessage("لم أتمكن من العثور على نافذة SAP" , Brushes.IndianRed);
                return;
            }

            string CommissionOrFinancialReport = GetSelectedRadioButtonContent(StackPanelCommission);

            if(string.IsNullOrWhiteSpace(TxtBillNumber.Text))
            {
                TxtBillNumber.Focus();
                _ = MyMessageService.ShowMessage("رقم الفاتورة فارغة" , Brushes.Yellow);
            }

            if(string.IsNullOrWhiteSpace(TxtOrderNumber.Text))
            {
                TxtOrderNumber.Focus();
                _ = MyMessageService.ShowMessage("رقم امر الشغل فارغ" , Brushes.Red);
                return;
            }

            if(ChBCenters.IsChecked == true)
            {
                CommissionOrFinancialReport = "تقرير مالي";
            }

            CopyToSAPHelper.CopyOperationsToClipboard
                (
                "00" + GetSelectedRadioButtonContent(StackPanelOperationsRadioButton) ,
                "" ,
                 CommissionOrFinancialReport ,
                TxtBillNumber.Text ,
                TxtOrderNumber.Text
                );

            _ = MyMessageService.ShowMessage("تم نسخ الاوبريشن بنجاح" , Brushes.LawnGreen);

            CopyToSAPHelper.PastToSAP();

            _ = MyMessageService.ShowMessage("تم لصق الاوبريشن بنجاح" , Brushes.LawnGreen);
        }
        catch(Exception ex)
        {
            _ = MyMessageService.ShowMessage("حدث خطأ: (" + ex + ")" , Brushes.Red);
            MessageBox.Show("حدث خطأ غير متوقع: " + ex.Message);
        }
    }

    private void BtnCopyOperationsVisitToClipboard_Click(object sender , RoutedEventArgs e)
    {
        try
        {
            if(!CopyToSAPHelper.GetProcesses())
            {
                _ = MyMessageService.ShowMessage("لم أتمكن من العثور على نافذة SAP" , Brushes.IndianRed);
                return;
            }

            string Visits = "", controlKy = "";
            string No = GetSelectedRadioButtonContent(StackPanelOperationsRadioButton);

            if(ChBCenters.IsChecked == true)
            {
                ChBCenters.Focus();

                controlKy = "SM03";
            }

            switch(No)
            {
                case "20":
                    No = "10";
                    Visits = "";
                    break;
                case "40":
                    No = "30";
                    Visits = "الزيارة الثانية";
                    break;
                case "60":
                    No = "50";
                    Visits = "الزيارة الثالثة";
                    break;
                case "80":
                    No = "70";
                    Visits = "الزيارة الرابعة";
                    break;
                case "100":
                    No = "90";
                    Visits = "الزيارة الخامسة";
                    break;
                case "120":
                    No = "110";
                    Visits = "الزيارة السادسة";
                    break;
            }

            // الصف الاول في حالة بلاغات المراكز
            CopyToSAPHelper.CopyOperationsCenterToClipboard
                (
                 "00" + No ,
                controlKy ,
                Visits ,
                "" ,
                ""
                );

            _ = MyMessageService.ShowMessage("تم نسخ الزيارات بنجاح" , Brushes.LawnGreen);

            CopyToSAPHelper.PastToSAP();

            _ = MyMessageService.ShowMessage("تم لصق الزيارات بنجاح" , Brushes.LawnGreen);
        }
        catch(Exception ex)
        {
            _ = MyMessageService.ShowMessage("حدث خطأ: (" + ex + ")" , Brushes.Red);
            MessageBox.Show("حدث خطأ غير متوقع: " + ex.Message);
        }
    }

    private void BtnCopyCentersCycleToClipboard_Click(object sender , RoutedEventArgs e)
    {
        try
        {
            if(!CopyToSAPHelper.GetProcesses())
            {
                _ = MyMessageService.ShowMessage("لم أتمكن من العثور على نافذة SAP" , Brushes.IndianRed);
                return;
            }

            if(string.IsNullOrWhiteSpace(TxtID.Text))
            {
                TabControl.SetIsSelected(CentersCycleTab , true);
                _ = MyMessageService.ShowMessage("لم يتم اختيار نوع الخدمة" , Brushes.Red);
                return;
            }

            if(TxtID.Text == "ID")
            {
                TabControl.SetIsSelected(CentersCycleTab , true);
                _ = MyMessageService.ShowMessage("لم يتم اختيار نوع الخدمة" , Brushes.Red);
                return;
            }

            if(string.IsNullOrWhiteSpace(TxtShortText.Text))
            {
                TabControl.SetIsSelected(CentersCycleTab , true);
                _ = MyMessageService.ShowMessage("لم يتم اختيار نوع الخدمة" , Brushes.Red);
                return;
            }

            // استخدم التاريخ بالشكل الصحيح
            CopyToSAPHelper.CopyCentersCycleToClipboard(
                TxtID.Text ,
                TxtShortText.Text ,
                "1"
            );

            _ = MyMessageService.ShowMessage("تم نسخ التتميم بنجاح" , Brushes.LawnGreen);

            CopyToSAPHelper.PastToSAP();

            _ = MyMessageService.ShowMessage("تم لصق التتميم بنجاح" , Brushes.LawnGreen);
        }
        catch(Exception ex)
        {
            MessageBox.Show("حدث خطأ غير متوقع: " + ex.Message);
        }
    }

    private void BtnCopyServiceToClipboard_Click(object sender , RoutedEventArgs e)
    {
        try
        {
            if(!CopyToSAPHelper.GetProcesses())
            {
                _ = MyMessageService.ShowMessage("لم أتمكن من العثور على نافذة SAP" , Brushes.IndianRed);
                return;
            }

            string Damaged = " ", dateText = "", text = "";

            DateTime StartDate, EndDate;

            text = new TextRange(EditorRichBox.Document.ContentStart , EditorRichBox.Document.ContentEnd).Text;

            if(string.IsNullOrWhiteSpace(OrderDate.datePicker.Text))
            {
                OrderDate.datePicker.Focus();
                _ = MyMessageService.ShowMessage("قيمة التاريخ فارغة" , Brushes.Red);
                return;
            }

            if(string.IsNullOrWhiteSpace(text))
            {
                _ = MyMessageService.ShowMessage("التتميم فارغ" , Brushes.Red);
                EditorRichBox.Focus();
                return;
            }

            if(!string.IsNullOrWhiteSpace(TxtDamaged.Text))
            {
                Damaged = TxtDamaged.Text;
            }

            _ = MyMessageService.ShowMessage("التالف فارغ" , Brushes.Yellow);
            TxtDamaged.Focus();

            dateText = OrderDate.datePicker.Text;

            StartDate = EndDate = OrderDate.datePicker.SelectedDate.Value;
            StartDate = StartDate.AddDays(-1);

            if(string.IsNullOrEmpty(dateText))
            {
                OrderDate.datePicker.Focus();
                _ = MyMessageService.ShowMessage("قيمة التاريخ فارغة" , Brushes.Red);
                return;
            }

            // استخدم التاريخ بالشكل الصحيح
            CopyToSAPHelper.CopyActivitiyToClipboard(
                GetSelectedRadioButtonContent(StackPanelOperationsRadioButton) ,
                text ,
                Damaged ,
                StartDate.ToString("dd.MM.yyyy" , CultureInfo.InvariantCulture) ,
                EndDate.ToString("dd.MM.yyyy" , CultureInfo.InvariantCulture)
            );

            _ = MyMessageService.ShowMessage("تم نسخ التتميم بنجاح" , Brushes.LawnGreen);

            CopyToSAPHelper.PastToSAP();

            _ = MyMessageService.ShowMessage("تم لصق التتميم بنجاح" , Brushes.LawnGreen);
        }
        catch(Exception ex)
        {
            MessageBox.Show("حدث خطأ غير متوقع: " + ex.Message);
        }
    }

    public string EnsurePrefix()
    {
        // الحصول على النص الحالي من RichTextBox
        string text = new TextRange(EditorRichBox.Document.ContentStart , EditorRichBox.Document.ContentEnd).Text.Trim();

        // إزالة البادئات القديمة إذا كانت موجودة
        if(text.StartsWith("تم:"))
        {
            text = text.Replace("تم:" , "").Trim();
        }
        else if(text.StartsWith("لم يتم:"))
        {
            text = text.Replace("لم يتم:" , "").Trim();
        }

        return text;
    }

    private void ChcB_CheckedChanged_Click(object sender , RoutedEventArgs e)
    {
        // مسح المحتوى فقط بعد معالجة النص
        string newText = EnsurePrefix();

        // التحقق من حالة الـ CheckBox وإضافة البادئة المناسبة
        if(ChcB_Finished.IsChecked == true)
        {
            newText = "تم: " + newText;
        }
        else if(ChcB_Unfinished.IsChecked == true)
        {
            newText = "لم يتم: " + newText;
        }

        // مسح المحتوى القديم من RichTextBox
        EditorRichBox.Document.Blocks.Clear();

        // إضافة النص الجديد بعد معالجة البادئة
        EditorRichBox.AppendText(newText);
    }

    private void BtnExit_Click(object sender , RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void Btnadjust_Click(object sender , RoutedEventArgs e)
    {
        // تم: ضبط 
        //if(finalNotes.Count > 0)
        //{
        //    ////ComboEmployee.Items.Add("Ahmed");
        //    // ListData.ItemsSource = finalNotes;

        //    _=MyMessageService.ShowMessage("تم تحميل البيانات" , Brushes.LawnGreen);
        //}
        //else
        //{
        //    _=MyMessageService.ShowMessage("لا يوجد بيانات" , Brushes.IndianRed);
        //}
    }

    // الحدث الذي يتم تفعيله عند تغيير العنصر المحدد في الـ ListBox
    private void ListData_SelectionChanged(object sender , SelectionChangedEventArgs e)
    {
        CopyNotsToEditorRichTextBox();
    }

    private void BtnDamagedUp_Click(object sender , RoutedEventArgs e)
    {
        if(int.TryParse(TxtDamaged.Text , out int value))
        {
            if(value >= 3) return;
            TxtDamaged.Text = (value + 1).ToString();
        }
        else if(string.IsNullOrEmpty(TxtDamaged.Text))
        {
            TxtDamaged.Text = (value + 1).ToString();
        }
    }

    private void btnDamagedDown_Click(object sender , RoutedEventArgs e)
    {
        if(int.TryParse(TxtDamaged.Text , out int value))
        {
            if(value <= 0) return;
            TxtDamaged.Text = (value - 1).ToString();
        }
    }

    private void btnOrderNumberUp_Click(object sender , RoutedEventArgs e)
    {
        if(int.TryParse(TxtOrderNumber.Text , out int value))
        {
            TxtOrderNumber.Text = (value + 1).ToString();
        }
    }

    private void btnOrderNumberDown_Click(object sender , RoutedEventArgs e)
    {
        if(int.TryParse(TxtOrderNumber.Text , out int value))
        {
            TxtOrderNumber.Text = (value - 1).ToString();
        }
    }

    private void BtnSAPEnter_Click(object sender , RoutedEventArgs e)
    {
        CopyToSAPHelper.EnterOnSAP();
    }

    private void BtnSettingsWindow_Click(object sender , RoutedEventArgs e)
    {
        if(appWindow == null || !appWindow.IsVisible)
        {
            // إذا كانت النافذة غير موجودة أو غير مرئية، قم بإنشاءها وفتحها
            appWindow = new AppSettingsWindow();
            appWindow.Closed += OnAppWindowClosed; // الاشتراك في حدث الإغلاق
            appWindow.Show();
        }
        else
        {
            // إذا كانت النافذة مفتوحة بالفعل
            if(appWindow.WindowState == WindowState.Minimized)
            {
                // إذا كانت النافذة مصغرة، استعدها إلى الحالة الطبيعية
                appWindow.WindowState = WindowState.Normal;
            }
            // قم بتنشيط النافذة لتظهر في المقدمة
            appWindow.Activate();
        }
    }

    private void OnAppWindowClosed(object sender , EventArgs e)
    {
        // تنظيف الموارد وتحديث الحالة
        appWindow = null;
    }

    private void BtnSAPLogin_Click(object sender , RoutedEventArgs e)
    {
        CopyToSAPHelper.SAPLogin();
        _ = MyMessageService.ShowMessage("تم تسجيل الدخول الى SAP" , Brushes.LawnGreen);
    }

    private void BtnBtnSAPLogout_Click(object sender , RoutedEventArgs e)
    {
        CopyToSAPHelper.SAPLogout();
        _ = MyMessageService.ShowMessage("تم اغلاق كل نوافذ SAP" , Brushes.IndianRed);
    }

    private void WarntyOrFees_Click(object sender , RoutedEventArgs e)
    {
        WarntyOrFees();
    }

    private void WarntyOrFees()
    {
        switch(GetSelectedRadioButtonContent(StackPanelWarnty))
        {
            case "ضمان":
                TxtBillValue.Text = "125";
                break;
            case "رسوم":
                TxtBillValue.Text = "150";
                break;
            case "بدون":
                TxtBillValue.Text = "0";
                break;
        }
    }

    private FrameworkElementFactory CreateColumnDefinition(GridLength width)
    {
        FrameworkElementFactory columnDef = new(typeof(ColumnDefinition));
        columnDef.SetValue(ColumnDefinition.WidthProperty , width);
        return columnDef;
    }

    private void BtnExecute_Click(object sender , RoutedEventArgs e)
    {
        CopyToSAPHelper.ExecuteOnSAP();
    }

    private void CounterForZeker_MouseDown(object sender , MouseButtonEventArgs e)
    {
        TxtCounterZeker.Text = $"{++CounterForZeker} ذكر";
    }

    private void Minimized_MouseDown(object sender , MouseButtonEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void ChBCenters_Click(object sender , RoutedEventArgs e)
    {
        if(ChBCenters.IsChecked == true)
        {
            BtnCopyCentersCycle.IsEnabled = true;
        }
        else
        {
            BtnCopyCentersCycle.IsEnabled = false;
        }
    }

    private void BtnResetZeker_Click(object sender , RoutedEventArgs e)
    {
        CounterForZeker = 0;
        TxtCounterZeker.Text = "0 ذكر";
    }

    private void TxtCounterZeker_KeyDown(object sender , KeyEventArgs e)
    {
        if(e.Key == Key.Enter)
        {
            TxtCounterZeker.Text = $"{++CounterForZeker} ذكر";
            if(sender is TextBox textBox)
            {
                textBox.Focus(); // إعادة التركيز إلى الـ TextBox الذي استدعى الدالة
            }
        }
        else
        {

        }
    }

    private void ScrollViewer_MouseDoubleClick(object sender , MouseButtonEventArgs e)
    {
        if(appWindow == null || !appWindow.IsVisible)
        {
            // إذا كانت النافذة غير موجودة أو غير مرئية، قم بإنشاءها وفتحها
            appWindow = new MyMessagesWindow();
            appWindow.Closed += OnAppWindowClosed; // الاشتراك في حدث الإغلاق
            appWindow.Show();
        }
        else
        {
            // إذا كانت النافذة مفتوحة بالفعل
            if(appWindow.WindowState == WindowState.Minimized)
            {
                // إذا كانت النافذة مصغرة، استعدها إلى الحالة الطبيعية
                appWindow.WindowState = WindowState.Normal;
            }
            // قم بتنشيط النافذة لتظهر في المقدمة
            appWindow.Activate();
        }
    }
}