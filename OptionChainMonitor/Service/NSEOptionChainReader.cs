using System;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using OptionChainMonitor.Model;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace OptionChainMonitor.Service
{
    public class NSEOptionChainReader
    {
        public List<string> Cookie { get; set; }
        public async Task<List<OptionChain>> GetOptionChainAsync(DateTime snapshotTime)
        {
            HttpResponseMessage responseMessage = await RequestOptionChainAsync();

            var optionList = new List<OptionChain>();

            if (responseMessage.IsSuccessStatusCode)
            {
                var responsetext = responseMessage.Content.ReadAsStringAsync().Result;

                var optionsList = JObject.Parse(responsetext)["records"]["data"]
                    .Select(x => x.Children().OfType<JProperty>()).ToList();

                var optionChain = new List<OptionChain>();

                foreach (var item in optionsList)
                {
                    var option = new OptionChain() { SnapshotTime = snapshotTime };

                    foreach (var prop in item)
                    {
                        PopulateOptionChain(option, prop);
                    }

                    optionList.Add(option);
                }

            }

            return optionList;
        }

        private async Task<HttpResponseMessage> RequestOptionChainAsync()
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            HttpClient client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromMinutes(1);

            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri("https://www.nseindia.com/api/option-chain-indices?symbol=NIFTY")
            };

            requestMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
            requestMessage.Headers.TryAddWithoutValidation("Connection", "keep-alive");

            if (Cookie!=null && Cookie.Any())
                requestMessage.Headers.TryAddWithoutValidation("Cookie", string.Concat("AKA_A2=A;", string.Join(";", Cookie)));

            var responseMessage = await client.SendAsync(requestMessage);

            if(responseMessage.StatusCode == HttpStatusCode.OK)
            {
                var setCookieHeader = responseMessage.Headers.GetValues("set-cookie").ToList();
                Cookie = setCookieHeader.Select(c => c.Split(';').First()).ToList();
            }

            return responseMessage;
        }


        private static void PopulateOptionChain(OptionChain option, JProperty prop)
        {
            switch (prop.Name)
            {
                case "strikePrice": option.StrikePrice = prop.Value.ToObject<double>();
                    break;
                case "PE":
                    var c = prop.Value.Children().OfType<JProperty>();
                    option.PutAskPrice = c.FirstOrDefault(p => p.Name == "askPrice").Value.ToObject<double>();
                    option.PutBidprice = c.FirstOrDefault(p => p.Name == "bidprice").Value.ToObject<double>();
                    option.PutLastPrice = c.FirstOrDefault(p => p.Name == "lastPrice").Value.ToObject<double>();
                    option.PutOpenInterest = c.FirstOrDefault(p => p.Name == "openInterest").Value.ToObject<double>();
                    option.PutTotalTradedVolume = c.FirstOrDefault(p => p.Name == "totalTradedVolume").Value.ToObject<double>();
                    break;
                case "CE":
                    c = prop.Value.Children().OfType<JProperty>();
                    option.CallAskPrice = c.FirstOrDefault(p => p.Name == "askPrice").Value.ToObject<double>();
                    option.CallBidprice = c.FirstOrDefault(p => p.Name == "bidprice").Value.ToObject<double>();
                    option.CallLastPrice = c.FirstOrDefault(p => p.Name == "lastPrice").Value.ToObject<double>();
                    option.CallOpenInterest = c.FirstOrDefault(p => p.Name == "openInterest").Value.ToObject<double>();
                    option.CallTotalTradedVolume = c.FirstOrDefault(p => p.Name == "totalTradedVolume").Value.ToObject<double>();
                    break;
            }
        }
    }
}
