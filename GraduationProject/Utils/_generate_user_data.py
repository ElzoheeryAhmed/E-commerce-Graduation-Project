import pandas as pd, numpy as np
import os, faker, json

# Initialize data generator
fake = faker.Faker("en_US")

# Read the csv file
df = pd.read_csv(os.path.join(os.path.dirname(__file__), "..", "Data", "InitialSeed", 'ProductsRatings.csv'))

unique_users = df['userID'].unique()
with open(os.path.join(os.path.dirname(__file__), "..", "Data", "InitialSeed", 'missingUsers.json'), 'r') as f:
    review_user_data = json.load(f)

unique_users = np.unique(np.append(unique_users, list(review_user_data.values())))
users_dict = dict()

print("Generating user names...")
usernames = set()
while len(usernames) < len(unique_users):
    usernames.add(fake.user_name())

print("Generating user emails...")
emails = set()
while len(emails) < len(unique_users):
    emails.add(fake.email())

print("Starting generating user data...")
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

print("Saving generating user data to a json file...")
with open(os.path.join(os.path.dirname(__file__), "..", "Data", "InitialSeed", "users9.json"), "w") as users_file:
    json.dump(users_dict, users_file)
