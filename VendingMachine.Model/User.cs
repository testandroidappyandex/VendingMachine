using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendingMachine.Model
{
    public class User
    {
        public User()
        {
            //кошелек пользователя
            _userWallet = new ObservableCollection<MoneyStack>
               (Banknote.Banknotes.Select(b => new MoneyStack(b, 10)));
            UserWallet = new ReadOnlyObservableCollection<MoneyStack>(_userWallet);
            //продукты пользователя
            UserBuyings = new ReadOnlyObservableCollection<ProductStack>(_userBuyings);
        }
        public ReadOnlyObservableCollection<ProductStack> UserBuyings { get; }
        private readonly ObservableCollection<ProductStack> _userBuyings = new ObservableCollection<ProductStack>();
        public ReadOnlyObservableCollection<MoneyStack> UserWallet { get; }
        private readonly ObservableCollection<MoneyStack> _userWallet;
        public int UserSumm { get; set; }
    }
}
