import csv
import json
import pathlib
import os
import urllib.request 

URLs = {"ETHUSD": "Ethereum",
        "WAXUSD": "Wax",
        # The below symbols are only available in Binance, where USD -> BUSD
        "SOLBUSD": "Solana",
        "MATICBUSD": "Polygon",
        "THETABUSD": "Theta"}

base_link = "https://api2.cryptoslam.io/api/nft-indexes/"

destination_folder = pathlib.Path('/temp-output-directory/alternative/cryptoslam/nftsales')
# objectives:# download data from API -> temp folder or in memory. Output processed data to /temp-output-directory/alternative/cryptoslam/nftsales/symbol.csv
destination_folder.mkdir(parents=True, exist_ok=True)

def download_cryptoslam_nftsales(ticker: str):
    
    site = URLs[ticker]

    i = 1
    
    while i <= 5:
        try:
            # Fetch data
            result = json.load(urllib.request.urlopen(f"{base_link}{site}"))

            daily_result = [x['dailySummaries'] for x in result.values()]
            daily_result_dict = {key: value for d in daily_result for key, value in d.items()}

            with open(destination_folder / f"{ticker.lower()}.csv"), "w", newline='', encoding='utf-8') as csv_file:
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
