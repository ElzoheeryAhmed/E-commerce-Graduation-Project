using GraduationProject.Models.ModelEnums;
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.Models.Dto
{
	public class UserLoginDto
	{
		[Required]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }
		
		[Required]
		[StringLength(30, ErrorMessage = "Use another password.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}

	public class UserCreateDto : UserLoginDto
	{
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		
		[DataType(DataType.PhoneNumber)]
		public string PhoneNumber { get; set; }
		
		public DateTime Birthdate { get; set; }
		public string Gender { get; set; }
	}
}
