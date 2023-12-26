using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Collections.Generic;
using OptionChainMonitor.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace OptionChainMonitor.Service
{
    public class NSEOptionChainReader
    {
        public List<OptionChain> ReadOptionChain()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip | DecompressionMethods.Deflate;

            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(handler);

            System.Net.Http.HttpRequestMessage requestMessage = new System.Net.Http.HttpRequestMessage();
            requestMessage.RequestUri = new Uri("https://www.nseindia.com/api/option-chain-indices?symbol=NIFTY");

            requestMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");

            var responseMessage = client.SendAsync(requestMessage).Result;

            if (responseMessage.IsSuccessStatusCode)
            {
                var responsetext = responseMessage.Content.ReadAsStringAsync().Result;

                var optionsList = JObject.Parse(responsetext)["records"]["data"]
                    .Select(x => x.Children().OfType<JProperty>()).ToList();

                var optionChain = new List<OptionChain>();

                foreach (var item in optionsList)
                {
                    var option = new OptionChain();

                    foreach (var prop in item)
                    {
                        switch (prop.Name)
                        {
                            case "PE":
                                var c =  prop.Value.Children();
                                //double.TryParse(prop.Value.Children()., out var ask);
                                //option.PutAskPrice = ask;
                                break;
                            case "CE":
                                break;
                        }
                    }

                    optionChain.Add(option);
                }

                return optionChain;
            }

            return new List<OptionChain>();
        }

    }
}
