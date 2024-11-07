using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace CopyToSapApproved;

/// <summary>
/// Interaction logic for DatePickerWithButtons.xaml
/// </summary>
public partial class DatePickerWithButtons : UserControl
{
    public DatePickerWithButtons()
    {
        InitializeComponent();

        // تعيين التاريخ الحالي في DatePicker إذا لم يكن محددًا
        if(datePicker.SelectedDate == null)
        {
            datePicker.SelectedDate = DateTime.UtcNow;
        }
        datePicker.Text = datePicker.SelectedDate?.ToString("dd.MM.yyyy" , CultureInfo.InvariantCulture);
    }

    private void btnIncreaseDay_Click(object sender , RoutedEventArgs e)
    {
        // زيادة اليوم بمقدار واحد
        if(datePicker.SelectedDate.HasValue)
        {
            datePicker.SelectedDate = datePicker.SelectedDate.Value.AddDays(1);
            datePicker.Text = datePicker.SelectedDate?.ToString("dd.MM.yyyy" , CultureInfo.InvariantCulture);
        }
    }

    private void btnDecreaseDay_Click(object sender , RoutedEventArgs e)
    {
        // نقصان اليوم بمقدار واحد
        if(datePicker.SelectedDate.HasValue)
        {
            datePicker.SelectedDate = datePicker.SelectedDate.Value.AddDays(-1);
            datePicker.Text = datePicker.SelectedDate?.ToString("dd.MM.yyyy" , CultureInfo.InvariantCulture);
        }
    }
}