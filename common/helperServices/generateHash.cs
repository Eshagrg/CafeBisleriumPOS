using System.Security.Cryptography;
using System.Text;
using CafeBisleriumPOS.Common.model;

namespace CafeBisleriumPOS.common.helperServices;
public class AuthenticationService
{
    public string GenerateHash(string data)
    {
        // Create a SHA256 hashing algorithm instance.
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            // Convert the hashed bytes to a lowercase hexadecimal string.
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }

    // Authenticates a user based on provided existing and incoming user data.
    // Compares hashed passwords and usernames for equality.
    // Returns true if authentication is successful, false otherwise.
    public bool Authenticate(UserModel existingData, UserModel comingData)
    {
        // Hash the password from the incoming data.
        var hashedPassword = this.GenerateHash(comingData.Password);
        // Check if usernames and hashed passwords match for authentication.
        if (existingData.Username == comingData.Username && existingData.Password == hashedPassword)
        {
            return true;  // Authentication successful.
        }
        else
        {
            return false; // Authentication failed.
        }
    }
}
