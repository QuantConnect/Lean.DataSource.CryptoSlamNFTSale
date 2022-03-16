# QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
# Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

from AlgorithmImports import *

class CryptoSlamNFTSalesAlgorithm(QCAlgorithm):
    def Initialize(self):
        self.SetStartDate(2020, 10, 07)  #Set Start Date
        self.SetEndDate(2020, 10, 11)    #Set End Date

        self.eth_symbol = self.AddCrypto("ETHUSD", Resolution.Minute, Market.Bitfinex).Symbol
        self.nft_sales_symbol = self.AddData(CryptoSlamNFTSales, "ETH").Symbol

        self.last_average_price = None

    def OnData(self, slice):
        data = slice.Get(CryptoSlamNFTSales)
        if data.ContainsKey(self.nft_sales_symbol):
            currentAvgSales = data[self.nft_sales_symbol].TotalPriceUSD / data[self.nft_sales_symbol].TotalTransactions

            # comparing the average sales changes, we will buy ethereum or hold cash
            if self.last_average_price != None and currentAvgSales > self.last_average_price:
                self.SetHoldings(self.eth_symbol, 1)
        else:
            self.SetHoldings(self.eth_symbol, 0)

        self.last_average_price = currentAvgSales;