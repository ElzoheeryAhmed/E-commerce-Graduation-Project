using Microsoft.AspNetCore.Identity;

namespace GraduationProject.Models
{
	public class User : IdentityUser
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Gender { get; set; }
		public DateTime Birthdate { get; set; }
		
		public ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();
		public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
		
		public ICollection<Issue> Issues { get; set; }
        	public ICollection<Order> Orders { get; set; }
        	public ICollection<CartItem> CartItems { get; set; }
        	public ICollection<WishlistItem> WishlistItems { get; set; }
	}
}
