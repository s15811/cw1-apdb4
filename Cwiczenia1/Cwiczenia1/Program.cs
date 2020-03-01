using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cwiczenia1
{
    public class Program
    {
        public static async Task Main(string[] args)
        { 
            var httpClient = new HttpClient();
            var respose = await httpClient.GetAsync("https://pja.edu.pl");

            //JS Promise async/await
            //Java Future
            //C# Task async/await

            if (respose.IsSuccessStatusCode)
            {
                string html = await respose.Content.ReadAsStringAsync();
                var regex = new Regex("[a-z0-9]+@[a-z0-9]+\\.[a-z0-9]+\\.[a-z0-9]", RegexOptions.IgnoreCase);

                var matches = regex.Matches(html);

                foreach(var m in matches){
                    Console.WriteLine(m);
                }
                
            }


        }
    }
}
