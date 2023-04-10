import os, json

with open(os.path.join(os.path.dirname(__file__), "..", "Data", "InitialSeed", 'users9.json'), 'r') as f:
    user_data = json.load(f)

# user_data is a dictionary of users. Example of its items:
# {'id': 'A001170867ZBE9FORRQL', 'first_name': 'Sherry', 'last_name': 'Ritter', 'birthdate': '1964-02-14', 'gender': 1, 'username': 'reginaldmckee', 'email': 'ramoscourtney@example.com', 'phone': '6609872311278'}

# Storing the user_data into multiple files each containing 1000 entries.
# The files will be named users9_0.json, users9_1.json, users9_2.json, etc.
users_count = len(user_data)

counter = 0

users = list(user_data.items())
# ('0', {'id': 'A001170867ZBE9FORRQL', 'first_name': 'Sherry', 'last_name': 'Ritter', 'birthdate': '1964-02-14', 'gender': 1, 'username': 'reginaldmckee', 'email': 'ramoscourtney@example.com', 'phone': '6609872311278'})

while counter < users_count:
    with open(os.path.join(os.path.dirname(__file__), "..", "Data", "InitialSeed", "users", f"users9_{counter//1000}.json"), "w") as f:
        json.dump(dict(users[counter:counter+1000]), f)
        counter += 1000

