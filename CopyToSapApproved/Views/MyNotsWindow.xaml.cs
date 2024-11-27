using CopyToSapApproved.Controllers;
using CopyToSapApproved.Helper;
using CopyToSapApproved.Models;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CopyToSapApproved.Views;

/// <summary>
/// Interaction logic for MyNotsWindow.xaml
/// </summary>
public partial class MyNotesWindow : UserControl
{
    private readonly DatabaseHelper _databaseHelper = new();
    private ObservableCollection<MyNotes> _myNotes;
    private int IsEditing = 0;
    private DispatcherTimer _dbCheckTimer;

    public MyNotesWindow()
    {
        InitializeComponent();
        RefreshGrid();
        Counter();

        // إعداد المؤقت للتحقق من التغييرات في قاعدة البيانات كل 5 ثوانٍ
        _dbCheckTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(1)
        };
        _dbCheckTimer.Tick += (s , e) => CheckForUpdates();
        _dbCheckTimer.Start();
    }

    private void CheckForUpdates()
    {
        var latestData = _databaseHelper.GetAllData("MyNotes");

        // تحقق من الفرق بين البيانات الحالية والجديدة
        if(latestData.Count != _myNotes.Count)
        {
            RefreshGrid();
        }
    }

    private void txtSearch_KeyDown(object sender , KeyEventArgs e)
    {
        if(e.Key == Key.Enter)
        {
            SearchData();
            if(sender is TextBox textBox)
            {
                textBox.Focus(); // إعادة التركيز إلى الـ TextBox الذي استدعى الدالة
            }
        }
    }

    private void Counter()
    {
        txtCounter.Text = $"({_myNotes.Count()})";
    }

    private void RefreshGrid()
    {
        var notes = _databaseHelper.GetAllData("MyNotes")
         .Select(data => new MyNotes
         {
             ID = Convert.ToInt32(data["ID"]) ,
             Title = data["Title"].ToString() ,
             Content = data["Content"].ToString() ,
             CreatedAt = DateTime.Parse(data["CreatedAt"].ToString()) ,
             AlertTime = DateTime.Parse(data["AlertTime"].ToString()) ,
             Alerted = Convert.ToInt32(data["Alerted"])
         }).ToList();

        _myNotes = new ObservableCollection<MyNotes>(notes);
        DataGridData.ItemsSource = _myNotes;
        Counter();
    }

    private void SearchData()
    {
        var notes = DatabaseHelper.SearchMyNotes(txtTitle.Text , txtContent.Text)
        .Select(data => new MyNotes
        {
            ID = Convert.ToInt32(data["ID"]) ,
            Title = data["Title"].ToString() ,
            Content = data["Content"].ToString() ,
            CreatedAt = DateTime.Parse(data["CreatedAt"].ToString()) ,
            AlertTime = DateTime.Parse(data["AlertTime"].ToString()) ,
            Alerted = Convert.ToInt32(data["Alerted"])
        }).ToList();

        _myNotes = new ObservableCollection<MyNotes>(notes);
        DataGridData.ItemsSource = _myNotes;
        Counter();
    }

    private void DataGridData_SelectionChanged(object sender , SelectionChangedEventArgs e)
    {

    }

    private void BtnAddMyNote_Click(object sender , RoutedEventArgs e)
    {
        if(string.IsNullOrWhiteSpace(txtTitle.Text))
        {
            _=MyMessageService.ShowMessage("ملحوظة: عنوان الملحوظة فارغ." , Brushes.Yellow);
            txtTitle.Focus();
            return;
        }

        if(string.IsNullOrWhiteSpace(txtContent.Text))
        {
            _=MyMessageService.ShowMessage("ملحوظة: نص الملحوظة فارغ." , Brushes.Yellow);
            txtContent.Focus();
            return;
        }

        DateTime alertDate = dpAlertDate.SelectedDate ?? DateTime.MinValue;
        if(!DateTime.TryParse(txtAlertTime.Text , out var alertTime) || alertDate == DateTime.MinValue)
        {
            _=MyMessageService.ShowMessage("ملحوظة: يجب إدخال تاريخ ووقت التنبيه بشكل صحيح." , Brushes.Yellow);
            return;
        }

        DateTime fullAlertTime = alertDate.Date.Add(alertTime.TimeOfDay);

        if(IsEditing > 0)
        {
            DatabaseHelper.UpdateMyNoteById(IsEditing , txtTitle.Text , txtContent.Text , fullAlertTime);
            RefreshGrid();
            ClearTextBox();
            IsEditing = 0;
            return;
        }

        DatabaseHelper.AddMyNote(txtTitle.Text , txtContent.Text , fullAlertTime);
        ClearTextBox();
        RefreshGrid();
    }


    private void BtnDeletMyNote_Click(object sender , System.Windows.RoutedEventArgs e)
    {
        try
        {
            var selectedItem = DataGridData.SelectedItem;
            DatabaseHelper.DeleteRowFromMyNotes(Convert.ToInt32($"{((dynamic)selectedItem)["ID"]}"));
            RefreshGrid();
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void BtnEditMyNote_Click(object sender , RoutedEventArgs e)
    {
        try
        {
            var selectedItem = DataGridData.SelectedItem as MyNotes;
            if(selectedItem != null)
            {
                IsEditing = selectedItem.ID;
                txtTitle.Text = selectedItem.Title;
                txtContent.Text = selectedItem.Content;
                dpAlertDate.Text = selectedItem.AlertTime.ToString(); txtAlertTime.Text = selectedItem.AlertTime.ToShortTimeString();
                RefreshGrid();
            }
            else
            {
                _ = MyMessageService.ShowMessage("يجب تحديد ملحوظة أولا." , Brushes.Green);
            }
        }
        catch(Exception ex)
        {
            _ = MyMessageService.ShowMessage("خطأ: " + ex.Message , Brushes.IndianRed);
        }
    }


    private void ClearTextBox()
    {
        txtTitle.Text = "";
        txtContent.Text = "";
    }

   

    
}