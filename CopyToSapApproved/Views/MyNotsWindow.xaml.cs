using CopyToSapApproved.Controllers;
using CopyToSapApproved.Helper;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CopyToSapApproved.Views;

/// <summary>
/// Interaction logic for MyNotsWindow.xaml
/// </summary>
public partial class MyNotesWindow : UserControl
{
    private readonly DatabaseHelper _databaseHelper = new();
    private List<Dictionary<string , object>> _myNotes;
    private int IsEditing = 0;
    public MyNotesWindow()
    {
        InitializeComponent();
        RefreshGrid();
        Counter();
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
        DataGridData.ItemsSource = _myNotes = _databaseHelper.GetAllData("MyNotes");
    }

    private void SearchData()
    {
        DataGridData.ItemsSource = _myNotes = DatabaseHelper.SearchMyNotes(txtTitle.Text , txtMyNote.Text);
        Counter(); 
    }

    private void DataGridData_SelectionChanged(object sender , SelectionChangedEventArgs e)
    {

    }

    private void BtnAddMyNote_Click(object sender , System.Windows.RoutedEventArgs e)
    {
        if(string.IsNullOrWhiteSpace(txtTitle.Text))
        {
            MessageService.ShowMessage("ملحوظة: عنوان الملحوظة فارغ." , Brushes.Yellow);
            txtTitle.Focus();
            return;
        }

        if(string.IsNullOrWhiteSpace(txtMyNote.Text))
        {
            MessageService.ShowMessage("ملحوظة: نص الملحوظة فارغ." , Brushes.Yellow);
            txtMyNote.Focus();
            return;
        }

        if(IsEditing > 0)
        {
            DatabaseHelper.UpdateMyNoteById(IsEditing , txtTitle.Text , txtMyNote.Text);
            RefreshGrid();
            ClearTextBox();
            IsEditing = 0;
            return;
        }

        DatabaseHelper.AddMyNote(txtTitle.Text , txtMyNote.Text);
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
            var selectedItem = DataGridData.SelectedItem;
            if(selectedItem is not null)
            {
                IsEditing = Convert.ToInt32($"{((dynamic)selectedItem)["ID"]}");
                txtTitle.Text = $"{((dynamic)selectedItem)["Title"]}";
                txtMyNote.Text = $"{((dynamic)selectedItem)["MyNote"]}";
                RefreshGrid();
            }
            else
            {
                MessageService.ShowMessage("يجب تحديد ملحوظة اولا." , Brushes.Green);
            }
        }
        catch(Exception ex)
        {
            MessageService.ShowMessage("يجب تحديد ملحوظة اولا.\n" + ex.Message , Brushes.IndianRed);
        }
    }

    private void ClearTextBox()
    {
        txtTitle.Text = "";
        txtMyNote.Text = "";
    }
}