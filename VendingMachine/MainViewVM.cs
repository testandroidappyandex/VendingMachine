using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VendingMachine.Model;

namespace VendingMachine
{
    public class MainViewVM : BindableBase
    {
        public MainViewVM()
        {
            _user = new User();
            //преобразовать коллекцию в конструкторе
            UserWallet = new ObservableCollection<MoneyVM>(_user.UserWallet.Select(ms => new MoneyVM(ms)));
            //преобразовывать каждый добавленный или удаленный элемент из модели
            Watch(_user.UserWallet, UserWallet, um => um.MoneyStack);

            //покупки пользователя
            UserBuyings = new ObservableCollection<ProductVM>(_user.UserBuyings.Select(ub => new ProductVM(ub)));
            Watch(_user.UserBuyings, UserBuyings, ub => ub.ProductStack);
        }
        public int UserSumm => _user.UserSumm;
        private User _user;
        public ObservableCollection<MoneyVM> UserWallet { get; }
        public ObservableCollection<ProductVM> UserBuyings { get; }
        public DelegateCommand GetChange { get; }
        public int Credit { get; }
        public ReadOnlyObservableCollection<MoneyVM> AutomataBank { get; }
        public ReadOnlyObservableCollection<ProductVM> ProductsInAutomata { get; }

        private static void Watch<T, T2>
  (ReadOnlyObservableCollection<T> collToWatch, ObservableCollection<T2> collToUpdate, Func<T2, object> modelProperty)
        {
            ((INotifyCollectionChanged)collToWatch).CollectionChanged += (s, a) => {
                if (a.NewItems?.Count == 1) collToUpdate.Add((T2)Activator.CreateInstance(typeof(T2), (T)a.NewItems[0]));
                if (a.OldItems?.Count == 1) collToUpdate.Remove(collToUpdate.First(mv => modelProperty(mv) == a.OldItems[0]));
            };
        }
    }
    public class ProductVM
    {
        public ProductStack ProductStack { get; }
        public ProductVM(ProductStack productStack)
        {
            ProductStack = productStack;
        }
        public Visibility IsBuyVisible => BuyCommand == null ? Visibility.Collapsed : Visibility.Visible;
        public DelegateCommand BuyCommand { get; }
        public string Name => ProductStack.Product.Name;
        public string Price => $"({ProductStack.Product.Price} руб.)";
        public Visibility IsAmountVisible => BuyCommand == null ? Visibility.Collapsed : Visibility.Visible;
        public int Amount => ProductStack.Amount;
    }
    public class MoneyVM
    {
        public MoneyStack MoneyStack { get; }
        public MoneyVM(MoneyStack moneyStack)
        {
            MoneyStack = moneyStack;
        }
        public Visibility IsInsertVisible => InsertCommand == null ? Visibility.Collapsed : Visibility.Visible;
        public DelegateCommand InsertCommand { get; }
        public string Icon => MoneyStack.Banknote.IsCoin ? "..\\Images\\coin.jpg" : "..\\Images\\banknote.png";
        public string Name => MoneyStack.Banknote.Name;
        public int Amount => MoneyStack.Amount;
    }
}
