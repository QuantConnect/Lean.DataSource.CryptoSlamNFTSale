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

using System.Linq;
using QuantConnect.Data;
using QuantConnect.Util;
using QuantConnect.Algorithm;
using QuantConnect.DataSource;

namespace QuantConnect.DataLibrary.Tests
{
    /// <summary>
    /// Example algorithm using NFT Sales data as a source of alpha
    /// </summary>
    public class CryptoSlamNFTSalesAlgorithm : QCAlgorithm
    {
        private Symbol _nftSalesSymbol;
        private Symbol _ethSymbol;
        private decimal? _lastAvgSales = null;

        /// <summary>
        /// Initialise the data and resolution required, as well as the cash and start-end dates for your algorithm. All algorithms must initialized.
        /// </summary>
        public override void Initialize()
        {
            SetStartDate(2020, 10, 07);  //Set Start Date
            SetEndDate(2020, 10, 11);    //Set End Date
            SetCash(10000);
            
            _ethSymbol = AddCrypto("ETHUSD", Resolution.Minute, Market.Bitfinex).Symbol; 
            // Requesting data
            _nftSalesSymbol = AddData<CryptoSlamNFTSales>(_ethSymbol).Symbol;

            // Historical data
            var history = History<CryptoSlamNFTSales>(_nftSalesSymbol, 60, Resolution.Daily);
            Debug($"We got {history.Count()} items from our history request for {_ethSymbol} CryptoSlam NFT Sales data");
        }

        /// <summary>
        /// OnData event is the primary entry point for your algorithm. Each new data point will be pumped in here.
        /// </summary>
        /// <param name="slice">Slice object keyed by symbol containing the stock data</param>
        public override void OnData(Slice slice)
        {
            // Retrieving data
            var data = slice.Get<CryptoSlamNFTSales>();
            if (!data.IsNullOrEmpty())
            {
                var currentAvgSales = data[_nftSalesSymbol].TotalPriceUSD / data[_nftSalesSymbol].TotalTransactions;

                // comparing the average sales changes, we will buy ethereum or hold cash
                if (_lastAvgSales != null && currentAvgSales > _lastAvgSales)
                {
                    SetHoldings(_ethSymbol, 1);
                }
                else
                {
                    SetHoldings(_ethSymbol, 0);
                }

                _lastAvgSales = currentAvgSales;
            }
        }
    }
}
