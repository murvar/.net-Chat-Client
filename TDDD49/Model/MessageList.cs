using System.Collections.ObjectModel;
using TDDD49.Model;

public class MessageList : ObservableCollection<Message>
{
    public MessageList() : base()
    {
        
    }

    public void Clear()
    {
        this.Items.Clear();
    }
}