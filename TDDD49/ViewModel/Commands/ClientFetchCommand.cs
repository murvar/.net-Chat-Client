using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TDDD49.ViewModels.commands
{
    public class ClientFetchCommand : ICommand
    {
        public event AccessKeyPressedEventHandler CanExecuteChanged;

        public bool CanExecute(object paramenter)
        {
            throw new NotImplementedException();
          }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
