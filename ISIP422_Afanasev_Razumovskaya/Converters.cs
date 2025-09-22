using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ISIP422_Afanasev_Razumovskaya
{
    // Конвертер для преобразования bool в цвет
    public class BoolToColorConverter : IValueConverter
    {
        public static readonly BoolToColorConverter Instance = new BoolToColorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool inStock)
            {
                return inStock
                    ? new SolidColorBrush(Color.FromRgb(40, 167, 69))    // Зелёный для "В наличии"
                    : new SolidColorBrush(Color.FromRgb(220, 53, 69));   // Красный для "Нет в наличии"
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Конвертер для преобразования bool в текст
    public class BoolToTextConverter : IValueConverter
    {
        public static readonly BoolToTextConverter Instance = new BoolToTextConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool inStock)
            {
                return inStock ? "В наличии" : "Нет";
            }
            return "Неизвестно";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Конвертер для категорий в красивые названия
    public class CategoryDisplayConverter : IValueConverter
    {
        public static readonly CategoryDisplayConverter Instance = new CategoryDisplayConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Models.Category category)
            {
                // ИСПРАВЛЕНО: Заменяем switch expression на обычный switch statement
                switch (category)
                {
                    case Models.Category.Electronics:
                        return "🔌 Электроника";
                    case Models.Category.Clothing:
                        return "👕 Одежда";
                    case Models.Category.Food:
                        return "🍞 Продукты";
                    case Models.Category.Books:
                        return "📚 Книги";
                    case Models.Category.Home:
                        return "🏠 Для дома";
                    default:
                        return value.ToString();
                }
            }
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}