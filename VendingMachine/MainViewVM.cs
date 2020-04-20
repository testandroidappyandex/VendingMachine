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
            ((INotifyCollectionChanged)_user.UserWallet).CollectionChanged += (s, a) => {
                if (a.NewItems?.Count == 1) UserWallet.Add(new MoneyVM(a.NewItems[0] as MoneyStack));
                if (a.OldItems?.Count == 1) UserWallet.Remove(UserWallet.First(mv => mv.MoneyStack == a.OldItems[0]));
            };
        }
        public int UserSumm => _user.UserSumm;
        private User _user;
        public ObservableCollection<MoneyVM> UserWallet { get; }
        public ObservableCollection<ProductVM> UserBuyings { get; }
        public DelegateCommand GetChange { get; }
        public int Credit { get; }
        public ReadOnlyObservableCollection<MoneyVM> AutomataBank { get; }
        public ReadOnlyObservableCollection<ProductVM> ProductsInAutomata { get; }
    }
    public class ProductVM
    {
        public Visibility IsBuyVisible { get; }
        public DelegateCommand BuyCommand { get; }
        public string Name { get; }
        public string Price { get; }
        public int Amount { get; }
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
