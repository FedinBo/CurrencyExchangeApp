using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleAppDolar
{
    public class ExchangeRate
    {
        public string ccy { get; set; }
        public string base_ccy { get; set; }
        public string buy { get; set; }
        public string sale { get; set; }
    }

   public class Program
    {
        static async Task Main()
        {
            await DisplayExchangeRates();
        }

        public static async Task DisplayExchangeRates()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string apiUrl = "https://api.privatbank.ua/p24api/pubinfo?exchange&coursid=5";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        ExchangeRate[] exchangeRates = JsonSerializer.Deserialize<ExchangeRate[]>(responseData);

                        if (exchangeRates != null)
                        {
                            Console.WriteLine("Курс валют online:");

                            foreach (var currency in exchangeRates)
                            {
                                Console.WriteLine($"Валюта: {currency.ccy}");
                                Console.WriteLine($"Базова валюта: {currency.base_ccy}");
                                Console.WriteLine($"Купівля: {currency.buy}");
                                Console.WriteLine($"Продаж: {currency.sale}");
                                Console.WriteLine("--------------");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Помилка при десеріалізації даних.");
                        }

                        // Зберігаємо данні в файл
                        string jsonFilePath = "D:\\temp\\CurrencyExchangeApp\\onlineJson.txt";
                        File.WriteAllText(jsonFilePath, responseData);
                        Console.WriteLine($"Дані також збережені у файл: {jsonFilePath}");
                    }
                    else
                    {
                        Console.WriteLine($"Помилка при запиті: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка: {ex.Message}");
                }
            }
        }
    }
}
