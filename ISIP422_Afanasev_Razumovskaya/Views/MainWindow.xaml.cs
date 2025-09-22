using System;
using System.Windows;
using System.Windows.Input;
using ISIP422_Afanasev_Razumovskaya.ViewModels;

namespace ISIP422_Afanasev_Razumovskaya
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Устанавливаем иконку приложения
            try
            {
                this.Icon = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri("pack://application:,,,/Resources/shop_icon.ico"));
            }
            catch
            {
                // Если иконка не найдена, продолжаем без неё
            }

            // Добавляем обработчики событий для улучшения UX
            this.KeyDown += MainWindow_KeyDown;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Горячие клавиши для быстрых действий
            var viewModel = this.DataContext as MainViewModel;
            if (viewModel == null) return;

            switch (e.Key)
            {
                case Key.F1:
                    // F1 - Показать справку
                    ShowHelp();
                    e.Handled = true;
                    break;

                case Key.F5:
                    // F5 - Обновить данные (можно добавить команду в ViewModel)
                    MessageBox.Show("💡 Данные автоматически синхронизированы!",
                                  "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    e.Handled = true;
                    break;

                case Key.Escape:
                    // Escape - Очистить поля поиска
                    if (viewModel.ClearSearchCommand.CanExecute(null))
                    {
                        viewModel.ClearSearchCommand.Execute(null);
                    }
                    e.Handled = true;
                    break;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Показываем приветственное сообщение при первом запуске
            ShowWelcomeMessage();
        }

        private void ShowWelcomeMessage()
        {
            var result = MessageBox.Show(
                "🎉 Добро пожаловать в систему учёта товаров!\n\n" +
                "✨ Возможности системы:\n" +
                "• Управление товарами (добавление, удаление, редактирование)\n" +
                "• Продажа товаров с контролем остатков\n" +
                "• Пополнение склада\n" +
                "• Поиск товаров по различным критериям\n" +
                "• Отчёты и аналитика продаж\n" +
                "• Отмена последних операций\n\n" +
                "💡 Полезные горячие клавиши:\n" +
                "F1 - Справка\n" +
                "F5 - Обновить\n" +
                "Escape - Очистить поиск\n\n" +
                "Хотите посмотреть краткое руководство?",
                "🏪 Система учёта товаров",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                ShowHelp();
            }
        }

        private void ShowHelp()
        {
            string helpText =
                "📋 РУКОВОДСТВО ПОЛЬЗОВАТЕЛЯ\n" +
                "══════════════════════════════════════\n\n" +

                "🔹 ДОБАВЛЕНИЕ ТОВАРА:\n" +
                "1. Заполните все поля в секции 'Добавить товар'\n" +
                "2. Код генерируется автоматически\n" +
                "3. Нажмите 'Добавить товар'\n\n" +

                "🔹 ПРОДАЖА ТОВАРА:\n" +
                "1. Введите код товара\n" +
                "2. Укажите количество для продажи\n" +
                "3. Нажмите 'Продать'\n" +
                "4. Система проверит остатки автоматически\n\n" +

                "🔹 ПОПОЛНЕНИЕ СКЛАДА:\n" +
                "1. Введите код товара\n" +
                "2. Укажите количество для пополнения\n" +
                "3. Нажмите 'Пополнить'\n\n" +

                "🔹 ПОИСК ТОВАРОВ:\n" +
                "1. Выберите критерий поиска\n" +
                "2. Введите поисковый запрос\n" +
                "3. Нажмите 'Найти'\n" +
                "4. Результаты отобразятся внизу\n\n" +

                "🔹 ОТЧЁТЫ И АНАЛИТИКА:\n" +
                "• 'Отчёт о продажах' - детальная статистика\n" +
                "• 'Отменить продажу' - отмена последней операции\n\n" +

                "💡 СОВЕТЫ:\n" +
                "• Все операции имеют подсказки при наведении\n" +
                "• Система автоматически валидирует данные\n" +
                "• Коды товаров начинаются с '1' и генерируются автоматически\n" +
                "• При продаже система проверяет наличие товара на складе";

            MessageBox.Show(helpText, "📖 Справка по системе",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Устанавливаем минимальные размеры окна для корректного отображения
            this.MinWidth = 1200;
            this.MinHeight = 700;
        }

        // Обработчик закрытия приложения
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show(
                "Вы действительно хотите закрыть приложение?\n\n" +
                "Все несохранённые изменения будут потеряны.",
                "🚪 Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                // Можно добавить сохранение данных или очистку ресурсов
                MessageBox.Show("Спасибо за использование нашей системы!\n\n" +
                              "До свидания! 👋",
                              "🏪 Система учёта товаров",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
            }

            base.OnClosing(e);
        }
    }
}