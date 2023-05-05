using GraduationProject.Models.Dto;
using Newtonsoft.Json;

namespace GraduationProject.Utils
{
	public static class UserSeeder {
		public static List<UserCreateDto> Seed(int file_index) {
			string jsonFilePath = Path.Combine(Environment.CurrentDirectory,
									"Data",
									"InitialSeed",
									"users",
									$"users9_{file_index}.json");
			
			// Create an empty list to hold all the users.
			List<UserCreateDto> users = new List<UserCreateDto>();
			
			// Parsing the json file and creating the appropriate objects.
			using (StreamReader r = new StreamReader(jsonFilePath)) {
				// Parse the JSON into a dynamic object.
				dynamic extractedUsers = JsonConvert.DeserializeObject(r.ReadToEnd());
				
				// int counter = 0;
				foreach (var user in extractedUsers) {
					string id = user.Value.id;
					string firstName = user.Value.first_name;
					string lastName = user.Value.last_name;
					string username = user.Value.username;
					string birthdate = user.Value.birthdate;
					string gender = user.Value.gender == 1 ? "Male" : "Female";
					string email = user.Value.email;
					string phone = user.Value.phone;
					
					UserCreateDto newUser = new UserCreateDto { Id = id, FirstName = firstName, LastName = lastName, UserName = username, Birthdate = DateTime.Parse(birthdate), Gender = gender, Email = email, PhoneNumber = phone };
					users.Add(newUser);
					
					// // Below code is for debugging.
					// if (counter++ >= 10)
					// 	break;
				}
			}
			
			return users;
		}
	}
}
