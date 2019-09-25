using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace FETHArchiveManager
{
    [ValueConversion(typeof(long), typeof(long))]
    public class HexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            long size;
            if (!long.TryParse(value.ToString(), out size))
                return Binding.DoNothing;
            
            StringBuilder sb = new StringBuilder(size.ToString("X8"), 10);
            sb.Insert(0, "0x");
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            if (String.IsNullOrEmpty(strValue))
                return Binding.DoNothing;

            Regex hexRegex = new Regex("^0x[0-9A-F]{1,8}$");
            Match match = hexRegex.Match(strValue);
            if (match.Success)
                return System.Convert.ToInt64(strValue, 16);
            else
            {
                if (IsAllDigits(strValue))
                    return strValue;
                else
                    return Binding.DoNothing;
            }
                
            //throw new NotImplementedException();
        }

        bool IsAllDigits(string s)
        {
            foreach (char c in s)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }
    }
}
