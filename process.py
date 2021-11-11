import csv
import json
import os
import urllib.request 

URLs = {"ETHUSD": "Ethereum",
        "WAXUSD": "Wax",
        # The below symbols are only available in Binance, where USD -> BUSD
        "SOLBUSD": "Solana",
        "MATICBUSD": "Polygon",
        "THETABUSD": "Theta"}

destination_folder = "/temp-output-directory"
for path in [destination_folder, 
            f"{destination_folder}/alternative", 
            f"{destination_folder}/alternative/cryptoslam", 
            f"{destination_folder}/alternative/cryptoslam/nftsales"]:
    if os.path.exists(path):
        pass
    else:
        os.mkdir(path)

base_link = "https://api2.cryptoslam.io/api/nft-indexes/"
output_directory = f"{destination_folder}/alternative/cryptoslam/nftsales/"

def download_cryptoslam_nftsales(ticker: str):
    
    site = URLs[ticker]

    i = 1
    
    while i <= 5:
        try:
            # Fetch data
            result = json.load(urllib.request.urlopen(f"{base_link}{site}"))

            daily_result = [x['dailySummaries'] for x in result.values()]
            daily_result_dict = {key: value for d in daily_result for key, value in d.items()}

            with open(os.path.join(output_directory, f"{ticker.lower()}.csv"), "w", newline='', encoding='utf-8') as csv_file:
                writer = csv.writer(csv_file)
                for key, value in daily_result_dict.items():
                    writer.writerow([key, value['totalTransactions'], value['uniqueBuyers'], value['uniqueSellers'], value['totalPriceUSD']])
            
            print(f"Downloaded '{site}' successfully")
            return
        
        except:
            if i == 5:
                print(f"Failed to download data from {base_link}{site} (5 / 5) - Exiting")
            else:
                print(f"Failed to download file: '{site}' ({i} / 5)")
            
        i += 1

for key in URLs:
    download_cryptoslam_nftsales(key)

# For saving in AWS cloud
cmd = f"aws s3 sync {destination_folder}/ s3://cache.quantconnect.com/"
os.system(cmd)
