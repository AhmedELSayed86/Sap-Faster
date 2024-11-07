using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CopyToSapApproved.Helper;

public class SubstringConverter : IValueConverter
{
    public object Convert(object value , Type targetType , object parameter , System.Globalization.CultureInfo culture)
    {
        if(value is string text && parameter is string param)
        {
            var indices = param.Split('-');
            int start = 0;
            int length = text.Length;

            if(indices.Length == 2)
            {
                if(int.TryParse(indices[0] , out start))
                {
                    if(indices[1] == "") // إذا كان البارامتر الثاني فارغًا، فهذا يعني من البداية إلى النهاية
                    {
                        return text.Length > start ? text.Substring(start) : string.Empty;
                    }

                    if(int.TryParse(indices[1] , out int end))
                    {
                        length = Math.Min(end , text.Length) - start;
                    }
                }
            }

            return text.Length > start ? text.Substring(start , Math.Min(length , text.Length - start)) : string.Empty;
        }
        return value;
    }

    public object ConvertBack(object value , Type targetType , object parameter , System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}