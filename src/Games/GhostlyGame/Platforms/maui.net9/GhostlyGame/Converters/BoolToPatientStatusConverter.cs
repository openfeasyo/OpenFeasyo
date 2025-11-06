using Microsoft.Maui.Controls;
using System.Globalization;

namespace GhostlyGame.Converters
{
    internal class BoolToPatientStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool status)
                return status ? "Active" : "Non-Active";
            return "Non-Active";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
