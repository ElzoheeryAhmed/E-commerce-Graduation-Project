import pandas as pd
import os, faker, json

# Initialize data generator
fake = faker.Faker("en_US")

# Read the csv file
df = pd.read_csv(os.path.join(os.path.dirname(__file__), "..", "Data", "InitialSeed", 'ProductsRatings.csv'))


# count the number of unique records in the userID column
# total_users_num  = len(df['userID'])
# num_unique_users = len(df['userID'].unique())

# # print the result
# start = time()
# print("Total number of user records:", total_users_num)
# print("Number of unique user records:", num_unique_users)
# print("Number of duplicate user records:", total_users_num - num_unique_users)
# print(time()-start)

unique_users = df['userID'].unique()

users_dict = dict()

usernames = set()
while len(usernames) < len(unique_users):
    usernames.add(fake.user_name())

emails = set()
while len(emails) < len(unique_users):
    emails.add(fake.email())

for i in range(len(unique_users)):
    user_profile = fake.profile()
    
    id = unique_users[i]
    
    name = user_profile["name"].rsplit(' ', 1)
    first_name = name[0]
    last_name = name[-1] if len(name) > 1 else ""
    
    birthdate = fake.date_between(start_date='-60y', end_date='-18y').isoformat()
    
    gender = 0 if user_profile["sex"] == "M" else 1
    
    username = usernames.pop()
    
    email = emails.pop()
    
    # phone = fake.msisdn()
    
    phone = fake.numerify(text="%############") # https://faker.readthedocs.io/en/master/providers/baseprovider.html#faker.providers.BaseProvider.numerify
    
    users_dict[i] = {"id": id, "first_name": first_name, "last_name": last_name, "birthdate": birthdate, "gender": gender, "username": username, "email": email, "phone": phone}
    
    # print(f"Id:\t\t{id}\nFirst Name:\t{first_name}\nLast Name:\t{last_name}\nBirthdate:\t{birthdate}\nGender:\t\t{gender}\nUsername:\t{username}\nEmail:\t\t{email}\nPhone:\t\t{phone}", end="\n\n")
    
    # if i==5:
    #     break

with open(os.path.join(os.path.dirname(__file__), "..", "Data", "InitialSeed", "users8.json"), "w") as users_file:
    json.dump(users_dict, users_file)

# Id
# Username
# FirstName
# LastName
# Age
# Gender
# Email
# Phone

# print()
# print(type(df['userID'].unique()))



# user_profile = fake.profile()

# users_dict["id"].append(unique_users[i])

# name = user_profile["name"].rsplit(' ', 1)
# users_dict["first_name"].append(name[0])
# users_dict["last_name"].append(name[-1] if len(name) > 1 else "")

# users_dict["birthdate"].append(fake.date_between(start_date='-60y', end_date='-18y').isoformat())

# users_dict["gender"].append(0 if user_profile["sex"] == "M" else 1)

# users_dict["username"].append(user_profile["username"])

# users_dict["email"].append(user_profile["mail"])

# # phone = fake.msisdn()

# users_dict["phone"].append(fake.numerify(text="%############")) # https://faker.readthedocs.io/en/master/providers/baseprovider.html#faker.providers.BaseProvider.numerify

# # users_dict[i] = {"id": id, "first_name": first_name, "last_name": last_name, "birthdate": birthdate, "gender": gender, "username": username, "email": email, "phone": phone}

# # print(f"Id:\t\t{id}\nFirst Name:\t{first_name}\nLast Name:\t{last_name}\nBirthdate:\t{birthdate}\nGender:\t\t{gender}\nEmail:\t\t{email}\nPhone:\t\t{phone}", end="\n\n")
