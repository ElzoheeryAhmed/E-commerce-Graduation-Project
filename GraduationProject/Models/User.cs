using Microsoft.AspNetCore.Identity;

namespace GraduationProject.Models
{
	public class User : IdentityUser
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Gender { get; set; }
		public DateTime Birthdate { get; set; }
		
		public ICollection<Rating> Ratings { get; set; }
		public ICollection<Review> Reviews { get; set; }
		
		public User()
		{
			this.Ratings = new List<Rating>();
			this.Reviews = new List<Review>();
		}
	}
}
