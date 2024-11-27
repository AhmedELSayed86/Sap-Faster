using CopyToSapApproved.Controllers;
using CopyToSapApproved.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using WindowsInput;
using WindowsInput.Native;

namespace CopyToSapApproved.Helper;

public class CopyToSAPHelper()
{
    // دالة خارجية لتعيين نافذة في المقدمة
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    private static readonly DatabaseHelper databaseHelper = new();
    private static List<ActivitiesModels> _ActivitieslsList;
    private static List<CentersCycleModels> _CentersCyclelsList;
    private static List<TasksModels> _TaskslsList;
    private static List<OperationsModels> _OperationslsList;
    private List<Dictionary<string , object>> finalNotes;
    private static string path_Exists = "";
    private static   IntPtr handle;
    
    public static void PastToSAP()
    {
        try
        {
            if(GetProcesses())
            {
                SetForegroundWindow(handle);

                Thread.Sleep(500);
                var sim = new InputSimulator();
                sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL , VirtualKeyCode.VK_V);
                //SendKeys.SendWait("^{v}"); // لصق ctrl+v
                //Thread.Sleep(2000);
                //SendKeys.SendWait("^(s)");
                //SendKeys.SendWait("^{f5}");
                //Thread.Sleep(2000);
                //SendKeys.SendWait("~"); // How to press enter? Clipboard.SetText("CSMAIN");
                //Thread.Sleep(2000);
                //SendKeys.SendWait("^{s}");
                //Thread.Sleep(5000);

                // محاكاة ضغط مفتاح Enter (الرمز ~ في SendKeys)
                sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                Thread.Sleep(2000);
                sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);

                //SendKeys.SendWait("~"); // How to press enter? Clipboard.SetText("CSMAIN");
                //SendKeys.SendWait("CSMAIN");
            }
        }
        catch(Exception ex)
        {
            throw;
        }
    }

    public static void SAPLogin()
    {
        if(GetProcesses())
        {
            SetForegroundWindow(handle);

            Thread.Sleep(500);
            var sim = new InputSimulator();
            Clipboard.SetText("CSMAIN");
            sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL , VirtualKeyCode.VK_V); Thread.Sleep(300);

            sim.Keyboard.KeyPress(VirtualKeyCode.TAB); Thread.Sleep(300);

            Clipboard.SetText("vhgdldj");
            sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL , VirtualKeyCode.VK_V);

            // محاكاة ضغط مفتاح Enter (الرمز ~ في SendKeys)
            sim.Keyboard.KeyPress(VirtualKeyCode.RETURN); Thread.Sleep(1500);
            sim.Keyboard.KeyPress(VirtualKeyCode.TAB); Thread.Sleep(300);
            sim.Keyboard.KeyPress(VirtualKeyCode.TAB); Thread.Sleep(300);
            sim.Keyboard.KeyPress(VirtualKeyCode.TAB); Thread.Sleep(300);

            sim.Keyboard.KeyPress(VirtualKeyCode.UP); Thread.Sleep(300);

            // محاكاة ضغط مفتاح Enter (الرمز ~ في SendKeys)
            sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
        }
    }

    public static void EnterOnSAP()
    {
        if(GetProcesses())
        {
            SetForegroundWindow(handle);

            Thread.Sleep(500);
            var sim = new InputSimulator();

            // محاكاة ضغط مفتاح Enter (الرمز ~ في SendKeys)
            sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
        }
    }

    public static void ExecuteOnSAP()
    {
        if(GetProcesses())
        {
            SetForegroundWindow(handle);

            Thread.Sleep(500);
            var sim = new InputSimulator();

            // محاكاة ضغط مفتاح Enter (الرمز ~ في SendKeys)
            sim.Keyboard.KeyPress(VirtualKeyCode.F8);
        }
    }

    //SAP Login
    private void testc()
    {
        try
        {
            string path64 = @"C:\Program Files (x86)\SAP\FrontEnd\SapGui\saplogon.exe";
            string path86 = @"C:\Program Files\SAP\FrontEnd\SAPgui\saplogon.exe";

            if(File.Exists(path64))
            {
                _ = Process.Start(path64);
                path_Exists = path64;
            }
            else if(File.Exists(path86))
            {
                _ = Process.Start(path86);
                path_Exists = path86;
            }
            else
            {
                _=MyMessageService.ShowMessage("لم يتم العثور على برنامج الساب!" , Brushes.IndianRed);
                Application.Current.Shutdown();
                return;
            }

            // wait 5 secs
            Thread.Sleep(10000);
            var sim = new InputSimulator();

            // محاكاة ضغط مفتاح Enter (الرمز ~ في SendKeys)
            sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);

            //SendKeys.SendWait("~"); // How to press enter?

            if(GetProcesses())
            {
                SetForegroundWindow(handle);

                //PasteToApplication("google chrome");


                Clipboard.SetText("CSMAIN");

                // wait 5 secs
                Thread.Sleep(5000);
                // go to MSPaint and wait

                //SendKeys.SendWait("^v"); // لصق ctrl+v

                // محاكاة كتابة النص "CSMAIN"
                sim.Keyboard.TextEntry("CSMAIN");
                //SendKeys.SendWait("CSMAIN");

                // محاكاة ضغط مفتاح Tab
                sim.Keyboard.KeyPress(VirtualKeyCode.TAB);

                //SendKeys.SendWait("\t");

                // wait 5 secs
                Thread.Sleep(5000);

                // محاكاة كتابة النص "vhgdldj"
                sim.Keyboard.TextEntry("vhgdldj");

                //SendKeys.SendWait("vhgdldj");

                // محاكاة ضغط مفتاح Enter (الرمز ~ في SendKeys)
                sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);

                //SendKeys.SendWait("~"); // How to press enter?           
            }
        }
        catch(Exception ex)
        {
            _=MyMessageService.ShowMessage(ex.Message , Brushes.IndianRed);
            Application.Current.Shutdown();
            return;
        }
    }

    // احضار SAP الى المقدمة
    public static bool GetProcesses()
    {
        try
        {
            Process[] processes = Process.GetProcessesByName("saplogon");

            if(processes.Length == 0)
            {
                _=MyMessageService.ShowMessage("رجاء افتح الساب اولا" , Brushes.Yellow);
                return false;
            }

            handle = processes[0].MainWindowHandle;

            if(handle == IntPtr.Zero)
            {
                _=MyMessageService.ShowMessage("لم أتمكن من العثور على نافذة SAP" , Brushes.IndianRed);
                return false;
            }           
        }
        catch(Exception ex)
        {
            _=MyMessageService.ShowMessage($"خطأ: {ex.Message}" , Brushes.IndianRed);
            return false;
        }
        return true;
    }
  
    public static void SAPLogout()
    {
        try
        {
            Process[] processName = Process.GetProcessesByName("saplogon");

            // إغلاق التطبيق
            foreach(var process in processName)
            {

                process.Kill(); // إنهاء العملية كما في "Task Manager"
                process.WaitForExit(); // الانتظار حتى يتم الإغلاق بشكل كامل
                Console.WriteLine("SAP process has been terminated.");
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Failed to close SAP: {ex.Message}");
        }
    }

    private List<Dictionary<string , object>> Btnadjust_Click(object sender , RoutedEventArgs e)
    {// تم: ضبط
        finalNotes = databaseHelper.GetAllData("FinalNotes");
        if(finalNotes.Count > 0)
        {
            //_=MyMessageService.ShowMessage("تم تحميل البيانات" , Brushes.LawnGreen);
            ////ComboEmployee.Items.Add("Ahmed");
            return finalNotes;
        }
        else
        {
            //_=MyMessageService.ShowMessage("لا يوجد بيانات" , Brushes.IndianRed);
            return finalNotes;
        }
    }

    public static List<ActivitiesModels> CopyActivitiyToClipboard(string No , string ActivityText , string QuantityFactor , string StartDate , string EndDate)
    {
        Clipboard.Clear();

        // تنظيف النصوص المدخلة
        No = No.Trim();
        ActivityText = TextCleaning(ActivityText.Trim());
        QuantityFactor = QuantityFactor.Trim();
        StartDate = StartDate.Trim();
        EndDate = EndDate.Trim();

        ActivitiesModels activity = new ActivitiesModels
        {
            No = Convert.ToInt32(No) ,
            ActivityText = ActivityText ,
            QuantityFactor = QuantityFactor ,
            StartDate = StartDate ,
            EndDate = EndDate
        };
        _ActivitieslsList = [];
        _ActivitieslsList.Clear();
        _ActivitieslsList.Add(activity);

        // إعداد النص المراد نسخه إلى الكليب بورد
        string textToCopy = $"{No}\t{ActivityText}\t{QuantityFactor}\t{StartDate}\t{""}\t{EndDate}";

        // نسخ النص إلى الكليب بورد
        Clipboard.SetDataObject(new DataObject(DataFormats.Text , textToCopy) , true);

        _=MyMessageService.ShowMessage("تم نسخ النموذج" , Brushes.LawnGreen);

        return _ActivitieslsList.ToList();
    }

    public static List<TasksModels> CopyTasksToClipboard(string No , string TaskText , string Responsible , string ListName , string CompletedBy)
    {
        Clipboard.Clear();

        // تنظيف النصوص المدخلة
        No = No.Trim();
        TaskText = TextCleaning(TaskText.Trim());
        Responsible = Responsible;
        ListName = ListName.Trim();
        CompletedBy = CompletedBy.Trim();

        TasksModels tasks = new TasksModels
        {
            No = Convert.ToInt32(No) ,
            TaskText = TaskText ,
            Responsible = Responsible ,
            ListName = ListName ,
            CompletedBy = CompletedBy
        };
        _TaskslsList = [];
        _TaskslsList.Clear();
        _TaskslsList.Add(tasks);

        // إعداد النص المراد نسخه إلى الكليب بورد
        string textToCopy = $"{No}\t{TaskText}\t{Responsible}\t{ListName}\t{CompletedBy}";

        // نسخ النص إلى الكليب بورد
        Clipboard.SetDataObject(new DataObject(DataFormats.Text , textToCopy) , true);

        _=MyMessageService.ShowMessage("تم نسخ النموذج" , Brushes.LawnGreen);

        return _TaskslsList.ToList();
    }

    public static List<OperationsModels> CopyOperationsToClipboard(string Opac , string ControlKy , string OperationShortText , string Recipient , string UnloadingPoint)
    {
        Clipboard.Clear();
        // تنظيف النصوص المدخلة
        Opac = Opac.Trim();
        ControlKy = TextCleaning(ControlKy.Trim());
        OperationShortText = OperationShortText;
        Recipient = Recipient.Trim();
        UnloadingPoint = UnloadingPoint.Trim();

        OperationsModels operations = new OperationsModels
        {
            Opac = Convert.ToInt32(Opac) ,
            ControlKy = ControlKy ,
            OperationShortText = OperationShortText ,
            Recipient = Recipient ,
            UnloadingPoint = UnloadingPoint
        };
        _OperationslsList = [];
        _OperationslsList.Clear();
        _OperationslsList.Add(operations);

        // إعداد النص المراد نسخه إلى الكليب بورد
        string textToCopy = $"{Opac}\t{""}\t{""}\t{""}\t{ControlKy}\t{""}\t{""}\t{OperationShortText}\t{Recipient}\t{""}\t{UnloadingPoint}";

        // نسخ النص إلى الكليب بورد
        Clipboard.SetDataObject(new DataObject(DataFormats.Text , textToCopy) , true);

        _=MyMessageService.ShowMessage("تم نسخ النموذج" , Brushes.LawnGreen);

        return _OperationslsList.ToList();
    }

    public static List<OperationsModels> CopyOperationsCenterToClipboard(string Opac , string ControlKy , string OperationShortText , string Recipient , string UnloadingPoint)
    {
        Clipboard.Clear();
        // تنظيف النصوص المدخلة
        Opac = Opac.Trim();
        ControlKy = TextCleaning(ControlKy.Trim());
        OperationShortText = OperationShortText;
        Recipient = Recipient.Trim();
        UnloadingPoint = UnloadingPoint.Trim();

        OperationsModels operations = new OperationsModels
        {
            Opac = Convert.ToInt32(Opac) ,
            ControlKy = ControlKy ,
            OperationShortText = OperationShortText ,
            Recipient = Recipient ,
            UnloadingPoint = UnloadingPoint
        };
        _OperationslsList = [];
        _OperationslsList.Clear();
        _OperationslsList.Add(operations);

        // إعداد النص المراد نسخه إلى الكليب بورد
        string textToCopy = $"{Opac}\t{""}\t{""}\t{""}\t{ControlKy}\t{""}\t{""}\t{OperationShortText}\t{Recipient}\t{""}\t{UnloadingPoint}";

        // نسخ النص إلى الكليب بورد
        Clipboard.SetDataObject(new DataObject(DataFormats.Text , textToCopy) , true);

        _=MyMessageService.ShowMessage("تم نسخ النموذج" , Brushes.LawnGreen);

        return _OperationslsList.ToList();
    }

    public static List<CentersCycleModels> CopyCentersCycleToClipboard(string No , string ShortText , string Quantity)
    {
        Clipboard.Clear();

        // تنظيف النصوص المدخلة
        No = No.Trim();
        ShortText = TextCleaning(ShortText.Trim());
        Quantity = Quantity.Trim();

        CentersCycleModels centersCycle = new CentersCycleModels
        {
            ID = Convert.ToInt32(No) ,
            ShortText = ShortText
        };

        _CentersCyclelsList = [];
        _CentersCyclelsList.Clear();
        _CentersCyclelsList.Add(centersCycle);

        // إعداد النص المراد نسخه إلى الكليب بورد
        string textToCopy = $"{No}\t{ShortText}\t{Quantity}";

        // نسخ النص إلى الكليب بورد
        Clipboard.SetDataObject(new DataObject(DataFormats.Text , textToCopy) , true);

        _=MyMessageService.ShowMessage("تم نسخ النموذج" , Brushes.LawnGreen);

        return _CentersCyclelsList.ToList();
    }

    public static string TextCleaning(string CleanText)
    {
        CleanText = RemoveParentheses(CleanText); // حذف الأقواس () من النص
        CleanText = RemoveExtraSpaces(CleanText); // حذف المسافات
        CleanText = RemoveNewLines(CleanText);    // حذف الانتقال إلى سطر جديد
        CleanText = RemoveTrailingDash(CleanText);    // حذف علامة '-' من نهاية النص
        return CleanText;
    }

    // دالة لحذف الأقواس () من النص مع الإبقاء على النص داخلها
    private static string RemoveParentheses(string input)
    {
        return Regex.Replace(input , @"\(|\)" , string.Empty);
    }

    // دالة لحذف المسافات الزائدة من النص
    private static string RemoveExtraSpaces(string input)
    {
        return Regex.Replace(input.Trim() , @"\s{2,}" , " ");
    }

    // دالة لحذف علامة '-' من نهاية النص
    public static string RemoveTrailingDash(string input)
    {
        // التحقق مما إذا كان النص ينتهي بعلامة '-'
        if(input.EndsWith("-"))
        {
            // حذف علامة '-' من النهاية
            return input.Substring(0 , input.Length - 1);
        }

        // إذا لم يكن هناك علامة '-' في النهاية، إرجاع النص كما هو
        return input;
    }

    // دالة لحذف الانتقال إلى سطر جديد من النص مع الإبقاء على النص كما هو
    private static string RemoveNewLines(string input)
    {
        // استخدم تعبيرًا منتظمًا لاستبدال جميع أحرف الانتقال إلى سطر جديد بسلسلة فارغة
        return Regex.Replace(input , @"\r?\n|\r" , string.Empty);
    }

    //تعبير عادي يتطابق مع النص غير المسموح به
    private static readonly Regex _regex = new Regex("[^0-9]+");
    private static bool IsTextAllowed(string text)
    {
        return !_regex.IsMatch(text);
    }

    private void TextBoxPasting(object sender , DataObjectPastingEventArgs e)
    {
        if(e.DataObject.GetDataPresent(typeof(String)))
        {
            String text = (String)e.DataObject.GetData(typeof(String));
            if(!IsTextAllowed(text))
            {
                e.CancelCommand();
            }
        }
        else
        {
            e.CancelCommand();
        }
    }
}