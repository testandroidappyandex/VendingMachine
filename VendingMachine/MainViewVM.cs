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
        private PurchaseManager _manager;
        public MainViewVM()
        {
            _manager = new PurchaseManager();
            _user = _manager.User;
            _automata = _manager.Automata;
            //преобразовать коллекцию в конструкторе
            UserWallet = new ObservableCollection<MoneyVM>(_user.UserWallet.Select(ms => new MoneyVM(ms)));
            //преобразовывать каждый добавленный или удаленный элемент из модели
            Watch(_user.UserWallet, UserWallet, um => um.MoneyStack);

            //покупки пользователя
            UserBuyings = new ObservableCollection<ProductVM>(_user.UserBuyings.Select(ub => new ProductVM(ub)));
            Watch(_user.UserBuyings, UserBuyings, ub => ub.ProductStack);
  
            //деньги автомата
            AutomataBank = new ObservableCollection<MoneyVM>(_automata.AutomataBank.Select(a => new MoneyVM(a)));
            Watch(_automata.AutomataBank, AutomataBank, a => a.MoneyStack);
            //товары автомата
            ProductsInAutomata = new ObservableCollection<ProductVM>(_automata.ProductsInAutomata.Select(ap => new ProductVM(ap)));
            Watch(_automata.ProductsInAutomata, ProductsInAutomata, p => p.ProductStack);
            //деньги автомата

            _user.PropertyChanged += (s, a) => { RaisePropertyChanged(nameof(UserSumm)); };
            _automata.PropertyChanged += (s, a) => { RaisePropertyChanged(nameof(Credit)); };

        }
        public int UserSumm => _user.UserSumm;
        private User _user;
        public ObservableCollection<MoneyVM> UserWallet { get; }
        public ObservableCollection<ProductVM> UserBuyings { get; }
        public DelegateCommand GetChange { get; }
        public int Credit { get; }
        public ObservableCollection<MoneyVM> AutomataBank { get; }
        public ObservableCollection<ProductVM> ProductsInAutomata { get; }
        private Automata _automata;

        private static void Watch<T, T2>
  (ReadOnlyObservableCollection<T> collToWatch, ObservableCollection<T2> collToUpdate, Func<T2, object> modelProperty)
        {
            ((INotifyCollectionChanged)collToWatch).CollectionChanged += (s, a) => {
                if (a.NewItems?.Count == 1) collToUpdate.Add((T2)Activator.CreateInstance(typeof(T2), (T)a.NewItems[0]));
                if (a.OldItems?.Count == 1) collToUpdate.Remove(collToUpdate.First(mv => modelProperty(mv) == a.OldItems[0]));
            };
        }
    }
    public class ProductVM : BindableBase
    {
        public ProductStack ProductStack { get; }
        public ProductVM(ProductStack productStack, PurchaseManager manager = null)
        {
            ProductStack = productStack;
            productStack.PropertyChanged += (s, a) => { RaisePropertyChanged(nameof(Amount)); };

            if (manager != null)
                BuyCommand = new DelegateCommand(() => {
                    manager.BuyProduct(ProductStack.Product);
                });

        }
        public Visibility IsBuyVisible => BuyCommand == null ? Visibility.Collapsed : Visibility.Visible;
        public DelegateCommand BuyCommand { get; }
        public string Name => ProductStack.Product.Name;
        public string Price => $"({ProductStack.Product.Price} руб.)";
        public Visibility IsAmountVisible => BuyCommand == null ? Visibility.Collapsed : Visibility.Visible;
        public int Amount => ProductStack.Amount;
    }
    public class MoneyVM : BindableBase
    {
        public MoneyStack MoneyStack;
        public MoneyVM(MoneyStack moneyStack, PurchaseManager manager = null)
        {
            MoneyStack = moneyStack;
            if (manager != null) //по умолчанию Null, если же нет, то тогда задаем DelegateCommand
                InsertCommand = new DelegateCommand(() => {
                    manager.InsertMoney(MoneyStack.Banknote);
                });
            moneyStack.PropertyChanged += (s, a) => { RaisePropertyChanged(nameof(Amount)); };
        }
        public Visibility IsInsertVisible => InsertCommand == null ? Visibility.Collapsed : Visibility.Visible;
        public DelegateCommand InsertCommand { get; }
        public string Icon => MoneyStack.Banknote.IsCoin ? "..\\Images\\coin.jpg" : "..\\Images\\banknote.png";
        public string Name => MoneyStack.Banknote.Name;
        public int Amount => MoneyStack.Amount;
    }
}
