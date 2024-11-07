using System.Windows;
using System.Windows.Controls;

namespace CopyToSapApproved
{
    /// <summary>
    /// Interaction logic for NumericUpDownControl.xaml
    /// </summary>
    public partial class NumericUpDownControl : UserControl
    {
        public NumericUpDownControl()
        {
            InitializeComponent();
        }

        private void btnUp_Click(object sender , RoutedEventArgs e)
        {
            if(int.TryParse(txtNumber.Text , out int value))
            {
                txtNumber.Text = (value + 1).ToString();
            }
        }

        private void btnDown_Click(object sender , RoutedEventArgs e)
        {
            if(int.TryParse(txtNumber.Text , out int value))
            {
                txtNumber.Text = (value - 1).ToString();
            }
        }

        private void txtNumber_PreviewTextInput(object sender , System.Windows.Input.TextCompositionEventArgs e)
        {
            // التحقق من أن المدخل هو رقم فقط
            e.Handled = !int.TryParse(e.Text , out _);
        }
    }
}
