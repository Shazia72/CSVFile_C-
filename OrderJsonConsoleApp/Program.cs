using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace OrderJsonConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            apiCall(client).Wait();
        }
        static async Task apiCall(HttpClient client)
        {
            try
            {
                using (client) {
                    var response = await client.GetAsync("https://www.etlbox.net/demo/api/orders");
                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody =  response.Content.ReadAsStringAsync().Result;
                        var customers =  JsonConvert.DeserializeObject<CustomerDataResponse>(responseBody);

                        foreach (var cust in customers.CustomerData)
                        {
                            var order = new VM();
                            order.Description = cust.Description.Split(':')[0];
                            order.Quantity = Convert.ToInt32( cust.Description.Split(':')[1]);
                            order.OrderNumber = cust.OrderNumber;
                            order.CustomerId = cust.CustomerId;
                            Console.WriteLine(order);
                        }
                       
                    }
                }
            }
            catch
            {

            }
            
        }
               
    }
}