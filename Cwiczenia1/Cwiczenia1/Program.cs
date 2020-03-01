using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections;

namespace Cwiczenia1
{
    public class Program
    {
        public static async Task Main(string[] args)
        { 
            var httpClient = new HttpClient();

            if (args.Length == 0)
            {
                throw new ArgumentNullException();
            }

            if (!CheckURLValid(args[0]))
            {
                throw new ArgumentException();
            }

            var respose = await httpClient.GetAsync(args[0]);

            //JS Promise async/await
            //Java Future
            //C# Task async/await

            if (respose.IsSuccessStatusCode)
            {
                string html = await respose.Content.ReadAsStringAsync();
                var regex = new Regex("[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,63}", RegexOptions.IgnoreCase);
                var matches = regex.Matches(html);
                Hashtable hash = new Hashtable();
                

                if (matches.Count == 0)
                {
                    Console.WriteLine("Nie znaleziono adresów email");
                }
                else
                {
                    foreach (Match m in matches)
                    {
                        string foundMatch = m.ToString();
                        if (hash.Contains(foundMatch) == false)
                        {
                            hash.Add(foundMatch, string.Empty);
                        }
                    }

                    foreach (DictionaryEntry element in hash)
                    {
                        Console.WriteLine(element.Key);
                    }
                }
                httpClient.Dispose();
            }
            else
            {
                Console.WriteLine("Błąd w czasie pobrania strony");
                httpClient.Dispose();
            }


        }

        public static bool CheckURLValid(string url)
        {
            if (url == null)
            {
                return false;
            }
            return Regex.IsMatch(url, "(http|https)://(([www\\.])?|([\\da-z-\\.]+))\\.([a-z\\.]{2,3})");
        }

    }
}
