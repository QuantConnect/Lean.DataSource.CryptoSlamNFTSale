/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using NodaTime;
using ProtoBuf;
using QuantConnect.Data;

namespace QuantConnect.DataSource
{
    /// <summary>
    /// CryptoSlam NFT Sales dataset
    /// </summary>
    [ProtoContract(SkipConstructor = true)]
    public class CryptoSlamNFTSales : BaseData
    {
        /// <summary>
        /// The number of NFT transaction made within this blockchain
        /// </summary>
        [ProtoMember(10)]
        public long TotalTransactions { get; set; }

        /// <summary>
        /// The number of unique buyers of NFT within this blockchain
        /// </summary>
        [ProtoMember(11)]
        public long UniqueBuyers { get; set; }

        /// <summary>
        /// The number of unique sellers of NFT within this blockchain
        /// </summary>
        [ProtoMember(12)]
        public long UniqueSellers { get; set; }

        /// <summary>
        /// The total transaction value (in USD) of NFT within this blockchain
        /// </summary>
        [ProtoMember(13)]
        public decimal TotalPriceUSD { get; set; }

        /// <summary>
        /// Return the URL string source of the file. This will be converted to a stream
        /// </summary>
        /// <param name="config">Configuration object</param>
        /// <param name="date">Date of this source file</param>
        /// <param name="isLiveMode">true if we're in live mode, false for backtesting mode</param>
        /// <returns>String URL of source file.</returns>
        public override SubscriptionDataSource GetSource(SubscriptionDataConfig config, DateTime date, bool isLiveMode)
        {
            return new SubscriptionDataSource(
                Path.Combine(
                    Globals.DataFolder,
                    "alternative",
                    "cryptoslam",
                    "nftsales",
                    $"{config.Symbol.Value.ToLowerInvariant()}.csv"
                ),
                SubscriptionTransportMedium.LocalFile
            );
        }

        /// <summary>
        /// Parses the data from the line provided and loads it into LEAN
        /// </summary>
        /// <param name="config">Subscription configuration</param>
        /// <param name="line">Line of data</param>
        /// <param name="date">Date</param>
        /// <param name="isLiveMode">Is live mode</param>
        /// <returns>New instance</returns>
        public override BaseData Reader(SubscriptionDataConfig config, string line, DateTime date, bool isLiveMode)
        {
            var csv = line.Split(',');
            var price = decimal.Parse(csv[4], NumberStyles.Any, CultureInfo.InvariantCulture);

            var parsedDate = Parse.DateTimeExact(csv[0], "yyyyMMdd");
            return new CryptoSlamNFTSales()
            {
                TotalTransactions = long.Parse(csv[1]),
                UniqueBuyers = long.Parse(csv[2]),
                UniqueSellers = long.Parse(csv[3]),
                TotalPriceUSD = price,

                Symbol = config.Symbol,
                Value = price,
                Time = parsedDate - TimeSpan.FromDays(1),
                EndTime = parsedDate
            };
        }

        /// <summary>
        /// Clones the data
        /// </summary>
        /// <returns>A clone of the object</returns>
        public override BaseData Clone()
        {
            return new CryptoSlamNFTSales()
            {
                Value = Value,
                Symbol = Symbol,
                Time = Time,
                EndTime = EndTime,
                TotalTransactions = TotalTransactions,
                UniqueBuyers = UniqueBuyers,
                UniqueSellers = UniqueSellers,
                TotalPriceUSD = TotalPriceUSD,
            };
        }

        /// <summary>
        /// Indicates whether the data source is tied to an underlying symbol and requires that corporate events be applied to it as well, such as renames and delistings
        /// </summary>
        /// <returns>false</returns>
        public override bool RequiresMapping()
        {
            return false;
        }

        /// <summary>
        /// Indicates whether the data is sparse.
        /// If true, we disable logging for missing files
        /// </summary> 
        /// <returns>true</returns>
        public override bool IsSparseData()
        {
            return true;
        }

        /// <summary>
        /// Converts the instance to string
        /// </summary>
        public override string ToString()
        {
            return $"{Symbol} - Transactions {TotalTransactions} - Unique Buyers {UniqueBuyers} - Unique Seller{UniqueSellers} - Sales {TotalPriceUSD}";
        }

        /// <summary>
        /// Gets the default resolution for this data and security type
        /// </summary>
        public override Resolution DefaultResolution()
        {
            return Resolution.Daily;
        }

        /// <summary>
        /// Gets the supported resolution for this data and security type
        /// </summary>
        public override List<Resolution> SupportedResolutions()
        {
            return DailyResolution;
        }

        /// <summary>
        /// Specifies the data time zone for this data type. This is useful for custom data types
        /// </summary>
        /// <returns>The <see cref="T:NodaTime.DateTimeZone" /> of this data type</returns>
        public override DateTimeZone DataTimeZone()
        {
            return DateTimeZone.Utc;
        }
    }
}
