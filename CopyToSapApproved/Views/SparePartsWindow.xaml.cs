using CopyToSapApproved.Controllers;
using CopyToSapApproved.Helper;
using CopyToSapApproved.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Clipboard = System.Windows.Clipboard;

namespace CopyToSapApproved.Views
{
    /// <summary>
    /// Interaction logic for SpareParts.xaml
    /// </summary>
    public partial class SparePartsWindow : UserControl
    {
        readonly System.Windows.Controls.DataGrid dataGrid = new();
        //string path_Exists = "";
        //private IntPtr handle;

        private readonly DatabaseHelper _databaseHelper = new();
        private List<Dictionary<string , object>> _sparePart;
        private IList<Component> _ComponentlsList;

        [DllImport("User32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        public SparePartsWindow()
        {
            InitializeComponent();

            LoadData();
        }

        private void LoadData()
        {
            _sparePart = _databaseHelper.GetAllData("SparePart");
            _ComponentlsList = [];

            ComboStatuse.Items.Add("C1");
            ComboStatuse.Items.Add("C2");
            ComboStatuse.Items.Add("C3");
            ComboStatuse.SelectedIndex = 0;

            ComboStore.Items.Add("CSSP");
            ComboStore.Items.Add("CSBN");
            ComboStore.Items.Add("CSXP");
            ComboStore.Items.Add("CSCI");
            ComboStore.Items.Add("CSSH");
            ComboStore.SelectedIndex = 0;
        }

        void DataGrid_LoadingRow(object sender , DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private new void PreviewTextInput(object sender , TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
            txtSearch.Focus();
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

        private void BtnAdd_Click(object sender , RoutedEventArgs e)
        {
            AddDataC1();
            txtSearch.Text = "";
            txtSearch.Focus();
        }

        private void AddDataC1()
        {
            if(string.IsNullOrWhiteSpace(txtSearch.Text))
            {
               MessageService.ShowMessage("يجب اضافة كود" , Brushes.Yellow); 
                return;
            }

            if(CheckFoundC3(Convert.ToInt32(txtSearch.Text) , "C1"))
            {
               MessageService.ShowMessage("لا يمكن الاضافة، الكود بالفعل موجود بالجدول" , Brushes.Yellow); 
                return;
            }
            // جلب DescriptionAR باستخدام SapCode المدخل
            string descriptionAR = _databaseHelper.GetDescriptionARFromSparePart(txtSearch.Text);

            if(string.IsNullOrWhiteSpace(descriptionAR))
            {
               MessageService.ShowMessage("لم يتم العثور على البيانات" , Brushes.IndianRed);
                return;
            }

            Component Componentl = new Component
            {
                ID = Convert.ToInt32(txtSearch.Text) ,
                Name = descriptionAR ,
                Stor = ComboStore.Text ,
                Status = ComboStatuse.Text ,
                Quantity = 1 ,
                OPAC = "0010"
            };

            _ComponentlsList.Add(Componentl);
            DataGridView.ItemsSource = _ComponentlsList.ToList();
            SetCountr();
           MessageService.ShowMessage("تم اضافة الصرف" , Brushes.LawnGreen);
        }


        private void BtnCopyC1_Click(object sender , RoutedEventArgs e)
        {

            if(DataGridView.Items.Count <= 0)
            {
               MessageService.ShowMessage("لا يوجد بيانات" , Brushes.Yellow);
                return;
            }

            CopyDataGrid(true);
            //DataGridView.SelectedItems.Clear();
            //var Selct = _Component.Where(M => M.Status != "C3");
            //DataGridView.SelectedItems.Add(Selct);

            //DataGridView.ItemsSource = Selct.ToList();

            //DataGridView.SelectAll();
            //ApplicationCommands.Copy.Execute(null, DataGridView);
            //DataGridView.ItemsSource = _Component.ToList();
           MessageService.ShowMessage("تم النسخ" , Brushes.LawnGreen);
        }

        private void BtnCopyC3_Click(object sender , RoutedEventArgs e)
        {
            Clipboard.Clear();
            byte Counter = 0;
            string descriptionAR = "";
            DataGridView.SelectedItem = null;

            foreach(var item in _ComponentlsList.ToList())
            {
                if(CheckFoundC3(item.ID , "C3"))
                {
                   MessageService.ShowMessage("التالف موجود" , Brushes.Yellow);
                    Counter++;
                }
                else
                {
                    if(item.Status != "C3" && CheckDataC3(item.ID))
                    {  // جلب DescriptionAR باستخدام SapCode المدخل
                        descriptionAR = _databaseHelper.GetDescriptionARFromSparePart(item.ID.ToString());
                        AddDataC3(item.ID , descriptionAR , item.Quantity , item.Stor);
                       MessageService.ShowMessage("تم اضافة التالف" , Brushes.LawnGreen); Counter++;
                    }
                }
            }

            if(Counter == 0)
            {
               MessageService.ShowMessage("لا يوجد تالف" , Brushes.Yellow);
                return;
            }

            CopyDataGrid(false);

           MessageService.ShowMessage("تم النسخ" , Brushes.LawnGreen);
        }

        private void AddDataC3(int ID , string descriptionAR , short Quantity , string Stor)
        {
            int d = -Quantity;
            Component Componentl = new()
            {
                ID = Convert.ToInt32(ID) ,
                Name = descriptionAR ,
                Stor = CheckStor(Stor) ,
                Status = "C3" ,
                Quantity = (short)d ,
                OPAC = "0010"
            };

            _ComponentlsList.Add(Componentl);

            DataGridView.ItemsSource = _ComponentlsList.ToList();

            SetCountr();
        }

        private bool CheckFoundC3(int ID , string StutusC)
        {
            foreach(var item in _ComponentlsList.ToList())
            {
                if(item.ID == ID && item.Status == StutusC)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckDataC3(int SapCode)
        {
            DatabaseHelper databaseHelper = new();
            return databaseHelper.CheckIfDamagedExists("SparePart" , SapCode);
        }

        private string CheckStor(string Stor)
        {
            return Stor switch
            {
                "CSSP" => "CSSC",
                "CSBN" => "CSSB",
                "CSXP" => "CSSX",
                "CSCI" => "CSSI",
                "CSSH" => "CSRH",
                _ => "CSSC",
            };
        }

        private void SetCountr()
        {
            txtCount.Text = $"({_ComponentlsList.Count()})";
        }

        private void CopyDataGrid(bool C)
        {
            Clipboard.Clear();
            DataGridView.SelectedItems.Clear();
            var Selct = _ComponentlsList;

            if(C)
            {
                // تحديد المصدر لبيانات DataGridView
                DataGridView.ItemsSource = Selct.Where(M => M.Status != "C3").ToList();
                MyController.SparePartsCopyToNots("");
                foreach(Component item in DataGridView.ItemsSource)
                {
                    // يمكنك تخصيص أي خاصية تحتاجها من الكائن هنا، على سبيل المثال:
                    MyController.SparePartsCopyToNots(item.Name + "-"); // استبدل PropertyName بالخاصية المطلوبة من الكائن
                }
            }
            else
            {
                DataGridView.ItemsSource = Selct.Where(M => M.Status == "C3").ToList(); ;
            }

            DataGridView.SelectAll();
            ApplicationCommands.Copy.Execute(null , DataGridView);
            DataGridView.ItemsSource = _ComponentlsList.ToList();
            DataGridView.SelectedItems.Add(Selct);
        }

        private void NewDataGrid()
        {
            var colID = new DataGridTextColumn
            {
                Header = "SapCode" ,
                Binding = new System.Windows.Data.Binding("SapCode")
            };
            dataGrid.Columns.Add(colID);

            var colName = new DataGridTextColumn
            {
                Header = "Name" ,
                Binding = new System.Windows.Data.Binding("Name")
            };
            dataGrid.Columns.Add(colName);

            var colQuantity = new DataGridTextColumn
            {
                Header = "Quantity" ,
                Binding = new System.Windows.Data.Binding("Quantity")
            };
            dataGrid.Columns.Add(colQuantity);

            var colStor = new DataGridTextColumn
            {
                Header = "Stor" ,
                Binding = new System.Windows.Data.Binding("Stor")
            };
            dataGrid.Columns.Add(colStor);

            var colStatus = new DataGridTextColumn
            {
                Header = "Status" ,
                Binding = new System.Windows.Data.Binding("Status")
            };
            dataGrid.Columns.Add(colStatus);

            var colSOPAC = new DataGridTextColumn
            {
                Header = "OPAC" ,
                Binding = new System.Windows.Data.Binding("OPAC")
            };
            dataGrid.Columns.Add(colSOPAC);
        }

        private void BtnCler_Click(object sender , RoutedEventArgs e)
        {
            try
            {
                if(_ComponentlsList.Count() <= 0)
                {
                   MessageService.ShowMessage("لا يوجد بيانات لحذفها" , Brushes.Yellow);
                }
                else
                {
                    var currentRowIndex = DataGridView.Items.IndexOf(DataGridView.SelectedItem);

                    if(currentRowIndex < 0)
                    {
                       MessageService.ShowMessage("يجب تحديد صف لحذفه" , Brushes.Yellow);
                    }
                    else
                    {
                        _ComponentlsList.RemoveAt(currentRowIndex);

                        DataGridView.ItemsSource = _ComponentlsList.ToList();
                        SetCountr();
                    }
                }
            }
            catch(Exception)
            {
               MessageService.ShowMessage("لا يوجد بيانات لحذفها" , Brushes.Yellow);
            }
            txtSearch.Focus();
        }

        private void BtnClerAll_Click(object sender , RoutedEventArgs e)
        {
            MyController.SparePartsCopyToNots("");

            if(_ComponentlsList.Count() > 0)
            {
                _ComponentlsList.Clear();
                DataGridView.ItemsSource = _ComponentlsList.ToList();
                SetCountr();
            }
            else
            {
               MessageService.ShowMessage("لا يوجد بيانات لحذفها" , Brushes.Yellow);
            }
            txtSearch.Focus();
        }

        private void BtnPast_Click(object sender , RoutedEventArgs e)
        {
            CopyToSAPHelper.PastToSAP();
        }

        private void SparePartsWindows_Loaded(object sender , RoutedEventArgs e)
        {
            LoadData();
        }

        // التقاط زر الانتر لتنفيذ الكود
        private void txtSearch_KeyDown(object sender , KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                AddDataC1(); 
                txtSearch.Text = "";
            }
            txtSearch.Focus();
        }
    }
}