using System.Diagnostics;
using System.Text.Json;
using CafeBisleriumPOS.common.helperClass;
using CafeBisleriumPOS.Common.model;
namespace CafeBisleriumPOS.common.helperServices;
public class ActionService
{
    private readonly AuthenticationService authentication;

    // Constructor to initialize the ActionService with an AuthenticationService instance.
    public ActionService(AuthenticationService authentication)
    {
        this.authentication = authentication;
    }
    // Session service instance for managing user session data.
    public SessionService sessionService = new SessionService();

    // Registers a user based on the provided data and saves it to the corresponding JSON file.
    // Supports both admin and staff user types.
    public async Task<CustomType> Register<T>(UserModel data) where T : UserModel, new()
    {
        try
        {
            // find save path based on user type.
            string savePath = data.userType == null ? "user.json" : (data.userType == UserType.admin ? "admin.json" : "staff.json");
            var path = new FileManagement().DirectoryPath("database", savePath);
            Trace.WriteLine("This is Path: " + path);
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            // Load existing user data.
            List<UserModel> userList = new List<UserModel>();
            if (File.Exists(path))
            {
                var existingData = await File.ReadAllTextAsync(path);
                userList = JsonSerializer.Deserialize<List<UserModel>>(existingData) ?? new List<UserModel>();
            }

            // Set a new unique ID for the user.
            int maxId = userList.Any() ? userList.Max(s => s.Id) : 0;
            data.Id = maxId + 1;
            sessionService.setId(data.Id);
            // Hash the password before storing it.
            if (data.Password != null)
            {
                data.Password = this.authentication.GenerateHash(data.Password);
            }
            Trace.WriteLine("This is PASSWORD: ", data.Password);
            // Add the new user to the list and save it back to the file.
            userList.Add(data);

            var jsonData = JsonSerializer.Serialize(userList);
            await File.WriteAllTextAsync(path, jsonData);
            Trace.WriteLine(path);
            return new CustomType { Success = true, Message = $"{data.userType} registered successfully" };
        }
        catch (Exception ex)
        {
            Trace.WriteLine("Exception: {0}", ex.Message);
            return new CustomType { Success = false, Message = ex.Message };
        }
    }
    //Login Logic
    public async Task<CustomType> Login<T>(UserModel data) where T : UserModel, new()
    {
        try
        {
            //Making sure converting to UpperCase before logging
            data.userType = (UserType)Enum.Parse(typeof(UserType), data.userType.ToString().ToLower());
            Trace.WriteLine("This is UserType: " + data.userType);
            string savePath = data.userType == UserType.admin ? "admin.json" : "staff.json";
            var path = new FileManagement().DirectoryPath("database", savePath);
            if (File.Exists(path))
            {
                Trace.WriteLine("This is Path: " + path);
                var existingData = await File.ReadAllTextAsync(path);
                Trace.WriteLine("This is Existing Data: " + existingData);
                if (!string.IsNullOrEmpty(existingData))
                {
                    var existingUsers = JsonSerializer.Deserialize<List<UserModel>>(existingData);
                    var existingUser = existingUsers.FirstOrDefault(user => user.Username == data.Username);

                    if (existingUser != null)
                    {
                        var isAuthenticatedUser = authentication.Authenticate(existingUser, data);
                        Trace.WriteLine("This is Authentication: " + isAuthenticatedUser);
                        if (isAuthenticatedUser)
                        {
                            return new CustomType { Success = true, Message = "Login Success" };
                        }
                        else
                        {
                            return new CustomType { Success = false, Message = "Invalid Credentials" };
                        }
                    }
                }
            }
            return new CustomType { Success = false, Message = "Please Make Sure User is Registered" };
        }
        catch (Exception ex)
        {
            Trace.WriteLine("This is Exception " + ex);
            return new CustomType { Success = false, Message = "Please Select User Type" };
        }
    }

    
    // Deletes an item with the specified ID from the JSON file.
    public async Task<CustomType> Delete<T>(int id, string fileName) where T : BaseModel, new()
    {
        try
        {
            // Determine the path for the file to delete from.
            var path = new FileManagement().DirectoryPath("database", fileName);
            // Check if the file exists and proceed with deletion logic.
            if (File.Exists(path))
            {
                // Find the item to delete based on the provided ID.
                var existingData = await File.ReadAllTextAsync(path);
                var list = JsonSerializer.Deserialize<List<T>>(existingData) ?? new List<T>();
                var itemToDelete = list.FirstOrDefault(c => c.Id == id);
                Trace.WriteLine("This is ItemtoDelete: " + itemToDelete);
                Trace.WriteLine(itemToDelete.Id);
                // If the item is found, remove it from the list and update the file.
                if (itemToDelete != null)
                {
                    list.Remove(itemToDelete);
                    var jsonData = JsonSerializer.Serialize(list);
                    await File.WriteAllTextAsync(path, jsonData);
                    return new CustomType { Success = true, Message = "Deleted" };
                }
                return new CustomType { Success = false, Message = "Item not found" };
            }
            else
            {
                return new CustomType { Success = false, Message = "File not found" };
            }
        }
        catch (Exception error)
        {
            return new CustomType { Success = false, Message = error.Message };
        }
    }
    // Logs out the user and returns a success message.
    public Task<CustomType> logOut()
    {
        return Task.FromResult(new CustomType { Success = true, Message = "Logout Success" });
    }

}