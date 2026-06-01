using System;
using System.IO;

namespace ConsoleApp15.Tasks
{
    public class NumeroSeis
    {
        public static void Execute()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== ПРОСТОЕ ШИФРОВАНИЕ XOR ===");
            Console.ResetColor();

            Console.Write("Введите путь к файлу: ");
            string filePath = Console.ReadLine()?.Trim('\"', ' ');

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Файл не найден!");
                Console.ResetColor();
                return;
            }

            Console.Write("Введите ключ (строку): ");
            string key = Console.ReadLine();

            if (string.IsNullOrEmpty(key))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ключ не может быть пустым!");
                Console.ResetColor();
                return;
            }

            string outputPath = GetOutputPath(filePath);

            try
            {
                byte[] data = File.ReadAllBytes(filePath);
                byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);

                for (int i = 0; i < data.Length; i++)
                {
                    data[i] ^= keyBytes[i % keyBytes.Length];
                }

                File.WriteAllBytes(outputPath, data);

                Console.ForegroundColor = ConsoleColor.Green;
                if (filePath.EndsWith(".enc", StringComparison.OrdinalIgnoreCase))
                    Console.WriteLine($"Файл успешно расшифрован: {outputPath}");
                else
                    Console.WriteLine($"Файл успешно зашифрован: {outputPath}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static string GetOutputPath(string originalPath)
        {
            string korzinaPath = GetKorzinaPath();
            string fileName = Path.GetFileName(originalPath);

            if (originalPath.EndsWith(".enc", StringComparison.OrdinalIgnoreCase))
            {
                // Расшифровка
                string newName = fileName.Replace(".enc", "", StringComparison.OrdinalIgnoreCase);
                return Path.Combine(korzinaPath, newName);
            }
            else
            {
                // Шифрование
                return Path.Combine(korzinaPath, fileName + ".enc");
            }
        }

        private static string GetKorzinaPath()
        {
            return @"C:\Users\МКА-ученик\source\repos\ConsoleApp15\ConsoleApp15\Korzina";
        }
    }
}