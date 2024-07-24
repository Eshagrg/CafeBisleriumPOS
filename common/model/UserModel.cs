using System.ComponentModel.DataAnnotations;
namespace CafeBisleriumPOS.Common.model;
public class UserModel : BaseModel
{
    // Gets or sets the user type associated with the user.
    public UserType? userType { get; set; }

    // Gets or sets the username for user identification.
    [StringLength(20, MinimumLength = 4, ErrorMessage = "User must be at least 4 characters long")]
    public string? Username { get; set; }

    // Gets or sets the password for user authentication.
    [StringLength(20, MinimumLength = 4, ErrorMessage = "Password must be at least 6 characters long")]
    public string? Password { get; set; }

    // Gets or sets the email address associated with the user.
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string? Email { get; set; }

    // Gets or sets the date until which the user is eligible for discounts.
    public DateTime? DiscountEligibleUntil { get; set; }
}