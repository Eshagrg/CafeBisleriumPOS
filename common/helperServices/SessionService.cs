
using CafeBisleriumPOS.common.helperClass;

namespace CafeBisleriumPOS.common.helperServices;

// SessionService class responsible for managing user session data and application state.
public class SessionService
{

    public UserType CurrentUserType { get; private set; }
    private bool? needAuthorized;
    public event Action OnChange;
    public void NotifyStateChanged() => OnChange?.Invoke();
    // Gets or sets the current user type.
    private bool currentUser;
    // Default admin ID.
    private int adminId = 1;
    // Gets the current admin ID.
    public int getId()
    {
        return this.adminId;
    }
    // Sets the admin ID to the specified value.
    public void setId(int id)
    {
        this.adminId = id;
    }
    // Sets the current user type and notifies subscribers of the state change.
    public void SetCurrentUserType(UserType userType)
    {
        this.CurrentUserType = userType;
        OnChange?.Invoke();
    }

    public void SetCurrentUser(bool set)
    {
        this.currentUser = set;
        OnChange?.Invoke();
    }
    // Checks if a user is currently logged in.
    public bool IsUserLoggedIn()
    {
        return this.currentUser;
    }
    // Sets the admin registration status and notifies subscribers of the state change.
    public bool setAdminRegistered()
    {
        OnChange?.Invoke();
        return true;
    }
    // Checks if admin registration has been completed.
    public bool isAdminRegistered()
    {
        var path = new FileManagement().DirectoryPath("database", "admin.json");
        if (File.Exists(path))
        {
            return true;
        }
        return false;
    }
    public bool setNeedAuthorized(bool set)
    {
        this.needAuthorized = set;
        OnChange?.Invoke();
        return (bool)this.needAuthorized;
    }
    // Checks if authorization is required
    public bool isNeedAuthorized()
    {
        return (bool)this.needAuthorized;
    }
    public bool defaultNeedAuthorized()
    {
        if (this.needAuthorized.HasValue)
        {
            return (bool)this.needAuthorized;
        }
        else
        {
            this.needAuthorized = true;
            return (bool)this.needAuthorized;
        }
    }
    // Private field to store the option to allow only coffee orders.
    private bool isOnlyCoffee = false;
    public void setOnlyCoffee(bool set)
    {
        this.isOnlyCoffee = set;
        OnChange?.Invoke();
    }
    // Retrieves the current option to allow only coffee orders.
    public bool getOnlyCoffee()
    {
        return this.isOnlyCoffee;
    }

    private bool isOnlyCustomer = false;
    public void setCustomerOnly(bool set)
    {
        this.isOnlyCustomer = set;
        OnChange?.Invoke();
    }
    // Retrieves the current option to allow only customer-related actions.
    public bool getCustomerOnly()
    {
        return this.isOnlyCustomer;
    }
}