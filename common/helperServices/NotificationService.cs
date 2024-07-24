namespace CafeBisleriumPOS.common.helperServices;
public class NotificationService
{
    public string? Message { get; set; }
    public string? MessageClass { get; set; }

    // Notifies the user with the specified message
    public void Notify(string message, string messageClass)
    {
        Message = message;
        MessageClass = messageClass;
    }

    // Clears the notification message and its associated CSS class
    public void Clear()
    {
        Message = "";
        MessageClass = "";
    }
}