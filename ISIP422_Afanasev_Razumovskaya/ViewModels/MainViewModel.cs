using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ISIP422_Afanasev_Razumovskaya.Models;
using ISIP422_Afanasev_Razumovskaya.Services;
using ISIP422_Afanasev_Razumovskaya.Utils;
using System.Collections.Generic;

namespace ISIP422_Afanasev_Razumovskaya.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ProductService _productService;
        private ObservableCollection<Product> _products;
        private string _newProductName;
        private decimal _newProductPrice;
        private int _newProductQuantity;
        private Category _newProductCategory;
        private string _searchText;
        private string _selectedSearchType = "name";
        private string _selectedProductCode;
        private int _sellQuantity;
        private int _restockQuantity;
        private ObservableCollection<Product> _searchResults;

        public MainViewModel()
        {
            _productService = new ProductService();
            _products = _productService.GetProducts();
            _searchResults = new ObservableCollection<Product>();

            // Команды
            AddProductCommand = new RelayCommand(ExecuteAddProduct, CanExecuteAddProduct);
            RemoveProductCommand = new RelayCommand(ExecuteRemoveProduct, CanExecuteRemoveProduct);
            SellProductCommand = new RelayCommand(ExecuteSellProduct, CanExecuteSellProduct);
            RestockProductCommand = new RelayCommand(ExecuteRestockProduct, CanExecuteRestockProduct);
            SearchProductsCommand = new RelayCommand(ExecuteSearchProducts);
            UndoLastSaleCommand = new RelayCommand(ExecuteUndoLastSale);
            ShowSalesReportCommand = new RelayCommand(ExecuteShowSalesReport);
            ClearSearchCommand = new RelayCommand(ExecuteClearSearch);
        }

        #region Properties
        public ObservableCollection<Product> Products
        {
            get => _products;
            set { _products = value; OnPropertyChanged(nameof(Products)); }
        }

        public ObservableCollection<Product> SearchResults
        {
            get => _searchResults;
            set { _searchResults = value; OnPropertyChanged(nameof(SearchResults)); }
        }

        public string NewProductName
        {
            get => _newProductName;
            set { _newProductName = value; OnPropertyChanged(nameof(NewProductName)); }
        }

        public decimal NewProductPrice
        {
            get => _newProductPrice;
            set { _newProductPrice = value; OnPropertyChanged(nameof(NewProductPrice)); }
        }

        public int NewProductQuantity
        {
            get => _newProductQuantity;
            set { _newProductQuantity = value; OnPropertyChanged(nameof(NewProductQuantity)); }
        }

        public Category NewProductCategory
        {
            get => _newProductCategory;
            set { _newProductCategory = value; OnPropertyChanged(nameof(NewProductCategory)); }
        }

        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(nameof(SearchText)); }
        }

        public string SelectedSearchType
        {
            get => _selectedSearchType;
            set { _selectedSearchType = value; OnPropertyChanged(nameof(SelectedSearchType)); }
        }

        public string SelectedProductCode
        {
            get => _selectedProductCode;
            set { _selectedProductCode = value; OnPropertyChanged(nameof(SelectedProductCode)); }
        }

        public int SellQuantity
        {
            get => _sellQuantity;
            set { _sellQuantity = value; OnPropertyChanged(nameof(SellQuantity)); }
        }

        public int RestockQuantity
        {
            get => _restockQuantity;
            set { _restockQuantity = value; OnPropertyChanged(nameof(RestockQuantity)); }
        }
        #endregion

        #region Commands
        public ICommand AddProductCommand { get; }
        public ICommand RemoveProductCommand { get; }
        public ICommand SellProductCommand { get; }
        public ICommand RestockProductCommand { get; }
        public ICommand SearchProductsCommand { get; }
        public ICommand UndoLastSaleCommand { get; }
        public ICommand ShowSalesReportCommand { get; }
        public ICommand ClearSearchCommand { get; }
        #endregion

        #region Command Methods
        private bool CanExecuteAddProduct(object parameter)
        {
            return !string.IsNullOrWhiteSpace(NewProductName) &&
                   NewProductPrice > 0 &&
                   NewProductQuantity >= 0;
        }

        private void ExecuteAddProduct(object parameter)
        {
            try
            {
                if (!_productService.IsProductNameValid(NewProductName))
                {
                    MessageBox.Show("Название товара должно содержать минимум 2 символа!",
                                  "⚠️ Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!_productService.IsPriceValid(NewProductPrice))
                {
                    MessageBox.Show("Цена должна быть больше 0 и не превышать 1 000 000!",
                                  "⚠️ Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!_productService.IsQuantityValid(NewProductQuantity))
                {
                    MessageBox.Show("Количество должно быть от 0 до 10 000!",
                                  "⚠️ Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _productService.AddProduct(NewProductName, NewProductPrice, NewProductQuantity, NewProductCategory);
                ClearNewProductFields();
                MessageBox.Show("Товар успешно добавлен в систему!",
                              "✅ Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении товара: {ex.Message}",
                               "❌ Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanExecuteRemoveProduct(object parameter)
        {
            return !string.IsNullOrWhiteSpace(SelectedProductCode);
        }

        private void ExecuteRemoveProduct(object parameter)
        {
            try
            {
                var result = MessageBox.Show($"Вы действительно хотите удалить товар с кодом {SelectedProductCode}?",
                                           "🗑️ Подтверждение удаления",
                                           MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (_productService.RemoveProduct(SelectedProductCode))
                    {
                        MessageBox.Show("Товар успешно удалён из системы!",
                                      "✅ Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        SelectedProductCode = string.Empty;
                    }
                    else
                    {
                        MessageBox.Show("Товар с указанным кодом не найден!",
                                      "❌ Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении товара: {ex.Message}",
                               "❌ Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanExecuteSellProduct(object parameter)
        {
            return !string.IsNullOrWhiteSpace(SelectedProductCode) && SellQuantity > 0;
        }

        private void ExecuteSellProduct(object parameter)
        {
            try
            {
                if (_productService.SellProduct(SelectedProductCode, SellQuantity))
                {
                    var product = _products.FirstOrDefault(p => p.Code == SelectedProductCode);
                    var totalAmount = product != null ? product.Price * SellQuantity : 0;

                    MessageBox.Show($"🛒 Продажа завершена!\n\n" +
                                  $"Товар: {product?.Name}\n" +
                                  $"Количество: {SellQuantity} шт.\n" +
                                  $"Сумма: {totalAmount:C}",
                                  "✅ Успешная продажа", MessageBoxButton.OK, MessageBoxImage.Information);
                    SellQuantity = 0;
                    SelectedProductCode = string.Empty;
                }
                else
                {
                    MessageBox.Show("❌ Недостаточно товара на складе или товар не найден!",
                                  "Ошибка продажи", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при продаже товара: {ex.Message}",
                               "❌ Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanExecuteRestockProduct(object parameter)
        {
            return !string.IsNullOrWhiteSpace(SelectedProductCode) && RestockQuantity > 0;
        }

        private void ExecuteRestockProduct(object parameter)
        {
            try
            {
                if (_productService.RestockProduct(SelectedProductCode, RestockQuantity))
                {
                    MessageBox.Show($"📦 Пополнение выполнено!\n\n" +
                                  $"Добавлено: {RestockQuantity} шт. товара",
                                  "✅ Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    RestockQuantity = 0;
                    SelectedProductCode = string.Empty;
                }
                else
                {
                    MessageBox.Show("❌ Товар с указанным кодом не найден!",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при пополнении товара: {ex.Message}",
                               "❌ Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteSearchProducts(object parameter)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    SearchResults.Clear();
                    return;
                }

                var results = _productService.SearchProducts(SearchText, SelectedSearchType);
                SearchResults.Clear();
                foreach (var product in results)
                {
                    SearchResults.Add(product);
                }

                if (results.Count == 0)
                {
                    MessageBox.Show("🔍 По вашему запросу ничего не найдено.\n\nПопробуйте изменить критерий поиска.",
                                  "Результаты поиска", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске: {ex.Message}",
                               "❌ Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteClearSearch(object parameter)
        {
            SearchText = string.Empty;
            SearchResults.Clear();
        }

        private void ExecuteUndoLastSale(object parameter)
        {
            try
            {
                if (_productService.UndoLastSale())
                {
                    MessageBox.Show("↶ Последняя продажа успешно отменена!\n\nТовар возвращён на склад.",
                                  "✅ Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("ℹ️ Нет продаж для отмены!",
                                  "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отмене продажи: {ex.Message}",
                               "❌ Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteShowSalesReport(object parameter)
        {
            try
            {
                var report = _productService.GetDetailedSalesReport();
                MessageBox.Show(report, "📊 Подробный отчёт о продажах",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчёта: {ex.Message}",
                               "❌ Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        private void ClearNewProductFields()
        {
            NewProductName = string.Empty;
            NewProductPrice = 0;
            NewProductQuantity = 0;
            NewProductCategory = Category.Electronics;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}