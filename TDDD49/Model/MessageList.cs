using System.Collections.ObjectModel;
using TDDD49.Model;

public class MessageList : ObservableCollection<Message>
{
    public MessageList() : base()
    {
        Add(new Message("Willa", "12:00:13", "Lite längre meddelande!"));
        Add(new Message("Willa", "12:00:13", "Lite längre meddelande!"));
        Add(new Message("Willa", "12:00:13", "Lite längre meddelande!"));
        Add(new Message("Willa", "12:00:13", "Lite längre meddelande!"));
        Add(new Message("Willa", "12:00:13", "Lite längre meddelande!"));
        Add(new Message("Willa", "12:00:13", "Lite längre meddelande!"));
        Add(new Message("Willa", "12:00:13", "Lite längre meddelande!"));
        Add(new Message("Willa", "12:00:13", "Lite längre meddelande!"));

    }
}