using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP422_Afanasev_Razumovskaya.Services
{
    using ISIP422_Afanasev_Razumovskaya.Models;
    using System.ComponentModel;

    namespace ISIP719_Gorlanov.Models
    {
        public class Product : INotifyPropertyChanged
        {
            private string _code;
            private string _name;
            private decimal _price;
            private int _quantity;
            private bool _inStock;
            private Category _category;

            public string Code
            {
                get => _code;
                set { _code = value; OnPropertyChanged(nameof(Code)); }
            }

            public string Name
            {
                get => _name;
                set { _name = value; OnPropertyChanged(nameof(Name)); }
            }

            public decimal Price
            {
                get => _price;
                set { _price = value; OnPropertyChanged(nameof(Price)); }
            }

            public int Quantity
            {
                get => _quantity;
                set
                {
                    _quantity = value;
                    InStock = value > 0;
                    OnPropertyChanged(nameof(Quantity));
                }
            }

            public bool InStock
            {
                get => _inStock;
                private set { _inStock = value; OnPropertyChanged(nameof(InStock)); }
            }

            public Category Category
            {
                get => _category;
                set { _category = value; OnPropertyChanged(nameof(Category)); }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
