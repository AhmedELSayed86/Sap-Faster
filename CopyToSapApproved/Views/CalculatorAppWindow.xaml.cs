using CopyToSapApproved.Helper;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CopyToSapApproved.Views;

/// <summary>
/// Interaction logic for CalculatorAppWindow.xaml
/// </summary>
public partial class CalculatorAppWindow : UserControl
{
    private OperationClass _currentOperation = new OperationClass();
    private bool _isAppendingInput = true;
    private bool _isOperatorLastClicked = false;

    public CalculatorAppWindow()
    {
        InitializeComponent();
    }

    private void OnNumberClick(object sender , RoutedEventArgs e)
    {
        Button button = sender as Button;

        // إذا تم إدخال عملية حسابية، ابدأ مدخل جديد
        if(_isOperatorLastClicked)
        {
            MainTextBox.Text = ConvertArabicNumbersToEnglish(button.Content.ToString());
            _isOperatorLastClicked = false;
        }
        else
        {
            // أضف الرقم إلى المدخل الحالي
            MainTextBox.Text += ConvertArabicNumbersToEnglish(button.Content.ToString());
        }
        _isAppendingInput = true;
    }

    private void OnOperatorClick(object sender , RoutedEventArgs e)
    {
        Button button = sender as Button;
        string operatorText = button.Content.ToString();

        // إذا كان هناك رقم في الـ MainTextBox
        if(double.TryParse(MainTextBox.Text , out double operand))
        {
            if(_isOperatorLastClicked)
            {
                // إذا تم الضغط على زر العمليات بشكل متكرر، استبدل العملية السابقة فقط
                _currentOperation.Operator = operatorText;
                UpdateSecondTextBox();
                return;
            }

            // إذا كانت العملية السابقة قد انتهت بحساب نتيجة
            if(_currentOperation.Operator != null)
            {
                _currentOperation.Operand2 = operand;
                double result = _currentOperation.Execute();
                _currentOperation = new OperationClass
                {
                    Operand1 = result ,
                    Operator = operatorText
                };
                MainTextBox.Text = result.ToString();
                UpdateSecondTextBox();
            }
            else
            {
                // في حالة العملية الأولى
                _currentOperation.Operand1 = operand;
                _currentOperation.Operator = operatorText;
                UpdateSecondTextBox();
                MainTextBox.Clear();
            }
            _isOperatorLastClicked = true;
        }
    }

    private void OnAdvancedOperationClick(object sender , RoutedEventArgs e)
    {
        Button button = sender as Button;
        string operation = button.Content.ToString();

        if(double.TryParse(MainTextBox.Text , out double operand))
        {
            double result = operation switch
            {
                "√" => Math.Sqrt(operand),
                "sin" => Math.Sin(Math.PI * operand / 180.0),
                "cos" => Math.Cos(Math.PI * operand / 180.0),
                "tan" => Math.Tan(Math.PI * operand / 180.0),
                _ => throw new NotSupportedException("Operation not supported")
            };

            MainTextBox.Text = result.ToString();
            SecondTextBox.Text = $"{operation}({operand}) = {result}";
        }
        else
        {
            MessageBox.Show("Invalid input for advanced operation." , "Error" , MessageBoxButton.OK , MessageBoxImage.Warning);
        }
    }

    private void OnPercentageClick(object sender , RoutedEventArgs e)
    {
        if(double.TryParse(MainTextBox.Text , out double operand))
        {
            // تخزين الرقم الأول
            _currentOperation.Operand1 = operand;
            _currentOperation.Operator = "%";
            SecondTextBox.Text = $"{_currentOperation.Operand1} %";
            MainTextBox.Clear();
            _isAppendingInput = false;
        }
    }

    private void OnEqualClick(object sender , RoutedEventArgs e)
    {
        if(double.TryParse(MainTextBox.Text , out double operand))
        {
            _currentOperation.Operand2 = operand;

            try
            {
                double result = _currentOperation.Operator switch
                {
                    "%" => (_currentOperation.Operand1 / _currentOperation.Operand2) * 100,
                    _ => _currentOperation.Execute()
                };

                MainTextBox.Text = result.ToString();
                SecondTextBox.Text += $" {_currentOperation.Operand2} = {result}";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message , "Error" , MessageBoxButton.OK , MessageBoxImage.Error);
            }
            finally
            {
                _currentOperation = new OperationClass();
                _isAppendingInput = false;
            }
        }
    }

    private void OnClearAll(object sender , RoutedEventArgs e)
    {
        MainTextBox.Text = "0";
        SecondTextBox.Text = string.Empty;
        _currentOperation = new OperationClass();
        _isOperatorLastClicked = true;
    }

    private void OnBackSpaceClick(object sender , RoutedEventArgs e)
    {
        if(!string.IsNullOrEmpty(MainTextBox.Text))
        {
            MainTextBox.Text = MainTextBox.Text.Remove(MainTextBox.Text.Length - 1 , 1);

            if(string.IsNullOrEmpty(MainTextBox.Text))
            {
                MainTextBox.Text = "0";
            }
        }
    }

    private void OnDotClick(object sender , RoutedEventArgs e)
    {
        if(!MainTextBox.Text.Contains("."))
        {
            if(string.IsNullOrEmpty(MainTextBox.Text))
            {
                MainTextBox.Text = "0.";
            }
            else
            {
                MainTextBox.Text += ".";
            }
        }
    }

    private void UpdateSecondTextBox()
    {
        SecondTextBox.Text = $"{_currentOperation.Operand1} {_currentOperation.Operator}";
    }

    private void HighlightButton(string content)
    {
        var button = MainGrid.Children.OfType<Button>().FirstOrDefault(b => b.Content.ToString() == content);
        if(button != null)
        {
            button.Background = Brushes.Gold;
            Task.Delay(200).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() => button.Background = Brushes.Transparent);
            });
        }
    }

    private void TxtNumber_PreviewTextInput(object sender , System.Windows.Input.TextCompositionEventArgs e)
    {
        e.Handled = !int.TryParse(e.Text , out _);
        HighlightButton(e.Text);
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

}