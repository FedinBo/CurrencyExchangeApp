using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CurrencyExchangeApp
{
    public class CurrencyRate // простий клас, який представляє валюту з властивостями
    {
        public string CurrencyCode { get; set; }
        public decimal ExchangeRateBuy { get; set; }
        public decimal ExchangeRateSell { get; set; }
    }



    public class Program // Головний клас, що містить логіку програми
    {
        private static List<CurrencyRate> currencyRates = new List<CurrencyRate>();

        static void Main()
        {
            // Встановлення кодування для консолі на UTF-8
            Console.OutputEncoding = Encoding.UTF8;
            // Завантаження даних з файлу
            LoadDataFromFile();

            while (true)
            {
                DisplayMenu();

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DisplayCurrencyData();
                        break;
                    case "2":
                        AddCurrencyFromConsole();
                        break;
                    case "3":
                        AddCurrencyFromFile();
                        break;
                    case "4":
                        DeleteCurrency();
                        break;
                    case "5":
                        CurrencyConverter();
                        break;
                    case "6":
                        SaveDataToFile();
                        break;
                    case "7":
                        _ = GetOnlineExchangeRate();
                        break;
                    case "8":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                        break;
                }
            }
        }

        private static void DisplayMenu() //Меню виводить список опцій меню у консоль
        {
            Console.WriteLine("\tОберіть опцію з Курсом Валют:");
            Console.WriteLine("1. Вивести дані на екран.");
            Console.WriteLine("2. Додати дані з клавіатури.");
            Console.WriteLine("3. Додати дані з файлу.");
            Console.WriteLine("4. Видаляти дані.");
            Console.WriteLine("5. Операція конвертор валют.");
            Console.WriteLine("6. Зберегти дані у файл.");
            Console.WriteLine("7. Онлайн курс валют.");
            Console.WriteLine("8. Вийти.");
        }
        // Операції
        private static void DisplayCurrencyData() // виводить список курсів валют, збережених у списку currencyRates
        {
            Console.WriteLine("\tКурси валют:");
            foreach (var currency in currencyRates)
            {
                Console.WriteLine($"{currency.CurrencyCode}: \tКупівля - {currency.ExchangeRateBuy:F2}, \tПродаж - {currency.ExchangeRateSell:F2}");
            }
        }

        private static void AddCurrencyFromConsole() //  Дозволяє користувачу додати нову валюту з курсами обміну з консолі.
        {
            Console.Write("Введіть код валюти: ");
            string code = Console.ReadLine();

            if (string.IsNullOrEmpty(code))
            {
                Console.WriteLine("Код валюти не може бути порожнім.");
                return;
            }

            Console.Write("Введіть курс валюти купівлі: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal buyRate))
            {
                Console.WriteLine("Невірний формат курсу валюти покупки.");
                return;
            }

            Console.Write("Введіть курс валюти продажі: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal sellRate))
            {
                Console.WriteLine("Невірний формат курсу валюти продажу.");
                return;
            }

            // Перевірка, чи вже існує валюта із таким кодом
            var existingCurrency = currencyRates.FirstOrDefault(c => c.CurrencyCode == code);

            if (existingCurrency != null)
            {
                // Валюта із таким кодом вже існує, перезаписуємо дані
                existingCurrency.ExchangeRateBuy = buyRate;
                existingCurrency.ExchangeRateSell = sellRate;

                Console.WriteLine("Дані успішно перезаписані.");
            }
            else
            {
                // Валюти із таким кодом немає, додаємо нову
                currencyRates.Add(new CurrencyRate { CurrencyCode = code, ExchangeRateBuy = buyRate, ExchangeRateSell = sellRate });

                Console.WriteLine("Дані успішно додані.");
            }
        }

        private static void AddCurrencyFromFile() //  Дозволяє користувачу додати курси валют з файлу JSON.
        {
            Console.Write("Введіть шлях до файлу з даними: ");
            string filePath = Console.ReadLine();

            if (File.Exists(filePath))
            {
                try
                {
                    string jsonContent = File.ReadAllText(filePath);
                    List<CurrencyRate> newRates = JsonSerializer.Deserialize<List<CurrencyRate>>(jsonContent);

                    currencyRates.AddRange(newRates);

                    Console.WriteLine("Дані успішно додані з файлу.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка при читанні файлу: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Файл не знайдено.");
            }
        }

        // операція дозволяє користувачу видалити валюту зі списку.
        private static void DeleteCurrency()
        {
            Console.Write("Введіть код валюти для видалення: ");
            string codeToDelete = Console.ReadLine();

            var currencyToDelete = currencyRates.FirstOrDefault(c => c.CurrencyCode == codeToDelete);

            if (currencyToDelete != null)
            {
                currencyRates.Remove(currencyToDelete);
                Console.WriteLine($"Валюта з кодом {codeToDelete} успішно видалена.");
            }
            else
            {
                Console.WriteLine($"Валюта з кодом {codeToDelete} не знайдена.");
            }
        }

        private static void CurrencyConverter() //  Конвертує суму з однієї валюти в іншу на основі курсів обміну.
        {
            Console.Write("Введіть суму: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                Console.WriteLine("Невірний формат суми.");
                return;
            }

            Console.Write("Введіть код валюти, з якої конвертуємо: ");
            string sourceCurrencyCode = Console.ReadLine();

            Console.Write("Введіть код валюти, в яку конвертуємо: ");
            string targetCurrencyCode = Console.ReadLine();

            var sourceCurrency = currencyRates.FirstOrDefault(c => c.CurrencyCode == sourceCurrencyCode);
            var targetCurrency = currencyRates.FirstOrDefault(c => c.CurrencyCode == targetCurrencyCode);

            if (sourceCurrency != null && targetCurrency != null)
            {
                decimal convertedAmount = amount * (sourceCurrency.ExchangeRateBuy / targetCurrency.ExchangeRateSell);
                convertedAmount = Math.Round(convertedAmount, 2);

                Console.WriteLine($"{amount} {sourceCurrency.CurrencyCode} конвертовано в {convertedAmount} {targetCurrency.CurrencyCode}.");
            }
            else
            {
                Console.WriteLine("Невірний код валюти.");
            }
        }

        private static void SaveDataToFile() //  Зберігає поточний список курсів валют у файл JSON.
        {
            Console.Write("Введіть шлях для збереження даних: ");
            string filePath = Console.ReadLine();

            try
            {
                string jsonContent = JsonSerializer.Serialize(currencyRates, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, jsonContent);

                Console.WriteLine("Дані успішно збережені у файл.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при збереженні даних у файл: {ex.Message}");
            }
        }

        private static void LoadDataFromFile() // Завантажує курси валют з файлу JSON у список currencyRates
        {
            string filePath = "currency_data.json";

            if (File.Exists(filePath))
            {
                try
                {
                    string jsonContent = File.ReadAllText(filePath);
                    currencyRates = JsonSerializer.Deserialize<List<CurrencyRate>>(jsonContent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка при читанні файлу: {ex.Message}");
                }
            }
        }


        public static async Task GetOnlineExchangeRate() // Метод викликає інший метод DisplayExchangeRates з іншого класу ConsoleAppDolar.Program для отримання онлайн-курсів обміну
        {
            try
            {
                await ConsoleAppDolar.Program.DisplayExchangeRates();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка під час виконання запиту: {ex.Message}");
            }
        }

    }
}
