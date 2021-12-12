using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TDDD49.Model;
using TDDD49.ViewModels;


namespace TDDD49.ViewModel.Commands
{
    public class ShowOldConversationCommand : ICommand
    {

        public ViewModelClient Vmc { get; set; }
        public ShowOldConversationCommand(ViewModelClient vmc)
        {
            this.Vmc = vmc;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            List<Message> obj = parameter as List<Message>;
            this.Vmc.ShowOldConversationMethod(obj);
        }
    }
}
