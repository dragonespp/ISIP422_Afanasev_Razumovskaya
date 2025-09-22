using ISIP422_Afanasev_Razumovskaya.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP422_Afanasev_Razumovskaya.Services
{
    public class ProductService
    {
        private ObservableCollection<Product> _products;
        private Stack<SalesHistory> _salesHistory;
        private int _nextCodeNumber = 1;

        public ProductService()
        {
            _products = new ObservableCollection<Product>();
            _salesHistory = new Stack<SalesHistory>();
            InitializeTestData();
        }

        public ObservableCollection<Product> GetProducts() => _products;
        public Stack<SalesHistory> GetSalesHistory() => _salesHistory;

        private void InitializeTestData()
        {
            AddProduct("Ноутбук ASUS", 45000, 5, Category.Electronics);
            AddProduct("Джинсы Levis", 3500, 10, Category.Clothing);
            AddProduct("Хлеб Бородинский", 45, 20, Category.Food);
            AddProduct("Война и мир", 800, 3, Category.Books);
            AddProduct("Лампа настольная", 2200, 7, Category.Home);
        }

        public void AddProduct(string name, decimal price, int quantity, Category category)
        {
            var product = new Product
            {
                Code = GenerateUniqueCode(),
                Name = name,
                Price = price,
                Quantity = quantity,
                Category = category
            };
            _products.Add(product);
        }

        private string GenerateUniqueCode()
        {
            return $"1{_nextCodeNumber++:D5}"; // Формат: 100001, 100002, etc.
        }

        public bool RemoveProduct(string code)
        {
            var product = _products.FirstOrDefault(p => p.Code == code);
            if (product != null)
            {
                _products.Remove(product);
                return true;
            }
            return false;
        }

        public bool RestockProduct(string code, int quantity)
        {
            var product = _products.FirstOrDefault(p => p.Code == code);
            if (product != null)
            {
                product.Quantity += quantity;
                return true;
            }
            return false;
        }

        public bool SellProduct(string code, int quantity)
        {
            var product = _products.FirstOrDefault(p => p.Code == code);
            if (product != null && product.Quantity >= quantity)
            {
                product.Quantity -= quantity;
                var sale = new SalesHistory(product, quantity);
                _salesHistory.Push(sale);
                return true;
            }
            return false;
        }

        public bool UndoLastSale()
        {
            if (_salesHistory.Count > 0)
            {
                var lastSale = _salesHistory.Pop();
                var product = _products.FirstOrDefault(p => p.Code == lastSale.ProductCode);
                if (product != null)
                {
                    product.Quantity += lastSale.QuantitySold;
                    return true;
                }
            }
            return false;
        }

       
        public List<Product> SearchProducts(string searchText, string searchType)
        {
            if (string.IsNullOrEmpty(searchText))
                return new List<Product>();

            searchText = searchText.ToLower();

            switch (searchType.ToLower())
            {
                case "code":
                    return _products.Where(p => p.Code.ToLower().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                case "name":
                    return _products.Where(p => p.Name.ToLower().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                case "category":
                    return _products.Where(p => p.Category.ToString().ToLower().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                default:
                    return new List<Product>();
            }
        }

        public decimal GetTotalSalesAmount()
        {
            return _salesHistory.Sum(s => s.TotalAmount);
        }

        public int GetTotalItemsSold()
        {
            return _salesHistory.Sum(s => s.QuantitySold);
        }

        // ДОПОЛНИТЕЛЬНЫЕ методы для валидации
        public bool IsProductCodeValid(string code)
        {
            return !string.IsNullOrWhiteSpace(code) && code.StartsWith("1");
        }

        public bool IsProductNameValid(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && name.Length >= 2;
        }

        public bool IsPriceValid(decimal price)
        {
            return price > 0 && price <= 1000000;
        }

        public bool IsQuantityValid(int quantity)
        {
            return quantity >= 0 && quantity <= 10000;
        }

        public List<SalesHistory> GetSalesHistoryList()
        {
            return _salesHistory.Reverse().ToList();
        }

        public Dictionary<Category, decimal> GetSalesByCategory()
        {
            return _salesHistory
                .GroupBy(s => s.Category)
                .ToDictionary(g => g.Key, g => g.Sum(s => s.TotalAmount));
        }

        public string GetDetailedSalesReport()
        {
            var sales = GetSalesHistoryList();
            var totalAmount = GetTotalSalesAmount();
            var totalItems = GetTotalItemsSold();
            var salesByCategory = GetSalesByCategory();

            var report = new StringBuilder();
            report.AppendLine("📊 ПОДРОБНЫЙ ОТЧЁТ О ПРОДАЖАХ");
            report.AppendLine("".PadRight(40, '='));
            report.AppendLine();

            report.AppendLine($"📈 Общая статистика:");
            report.AppendLine($"   • Всего продаж: {sales.Count}");
            report.AppendLine($"   • Всего товаров продано: {totalItems} шт.");
            report.AppendLine($"   • Общая сумма: {totalAmount:C}");
            report.AppendLine();

            if (salesByCategory.Any())
            {
                report.AppendLine($"📊 Продажи по категориям:");
                foreach (var category in salesByCategory)
                {
                    report.AppendLine($"   • {category.Key}: {category.Value:C}");
                }
                report.AppendLine();
            }

            if (sales.Any())
            {
                report.AppendLine($"🛒 Последние продажи:");
                var lastSales = sales.Take(5);
                foreach (var sale in lastSales)
                {
                    report.AppendLine($"   • {sale.SaleDate:dd.MM.yyyy HH:mm} - {sale.ProductName} " +
                                    $"({sale.QuantitySold} шт. × {sale.UnitPrice:C} = {sale.TotalAmount:C})");
                }
            }

            return report.ToString();
        }
    }
}
