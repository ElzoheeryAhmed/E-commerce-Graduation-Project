import pandas as pd
import os, faker, json, datetime


# Initialize data generator
fake = faker.Faker("en_US")

# read the csv files
timestamps = pd.read_csv(os.path.join(os.path.dirname(__file__), "..", "Data", "InitialSeed", 'ProductsRatings.csv'))['timestamp']

pids = pd.read_csv(os.path.join(os.path.dirname(__file__), "..", "Data", "InitialSeed", 'ProductsRatings.csv'))['itemID']

# pids = pd.read_csv(os.path.join(os.path.dirname(__file__), "..", "Data", "InitialSeed", 'ProductsMetadata.csv'))['asin']

# # Find the oldest timestamp
# oldest_timestamp = min(timestamps)

# # Convert the oldest timestamp to a datetime object
# oldest_datetime = datetime.datetime.fromtimestamp(oldest_timestamp)

# # Print the oldest datetime object
# print(f"The oldest datetime among the {len(timestamps)} products is: {oldest_timestamp} -> {oldest_datetime}")
# # Result: "The oldest datetime among the 150794 products is: 971136000 -> 2000-10-10 02:00:00"



# Generating random datetime values less than the rating date of the product.
product_added_dates = dict()
current_timestamp = datetime.datetime.now().timestamp()

# i = 0
# Generating a random date less than the oldest review date.
for pid, timestamp in zip(pids, timestamps):
    # Generate random date between '1994-07-03 11:42:52' and the product's review date
    
    if timestamp < product_added_dates.get(pid, current_timestamp):
        product_added_dates[pid] = timestamp
    
    # i += 1
    # if i>=10:
    #     break
    # print(f"{timestamp} {type(timestamp)} -> {added_date} {type(added_date)}")

for pid, timestamp in product_added_dates.items():
    product_added_dates[pid] = fake.date_time_between(773228572, timestamp).strftime("%Y-%m-%d %H:%M:%S")

with open(os.path.join(os.path.dirname(__file__), "..", "Data", "InitialSeed", "product_added_dates.json"), "w") as users_file:
    json.dump(product_added_dates, users_file)

