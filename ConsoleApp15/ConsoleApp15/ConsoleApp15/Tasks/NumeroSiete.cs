using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp15.Tasks
{
    public class NumeroSiete
    {
        public static void Execute()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== МЕНЕДЖЕР ФОТОАРХИВА ===");
            Console.ResetColor();

            string defaultPath = GetDefaultInputPath();
            string targetFolder = GetTargetFolderPath();

            Console.Write($"Введите путь к папке с фото (Enter = task7_photos): ");
            string userInput = Console.ReadLine()?.Trim('\"', ' ');

            string inputPath = string.IsNullOrEmpty(userInput) ? defaultPath : userInput;

            Console.WriteLine($"\nПроверяем путь: {inputPath}");

            if (!Directory.Exists(inputPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Папка не найдена!");
                Console.ResetColor();
                return;
            }

            // Создаём целевую папку
            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            var filesToMove = new List<(string source, string destination)>();
            string[] extensions = { ".jpg", ".jpeg", ".png", ".bmp" };

            Console.WriteLine("Сканирование...");

            try
            {
                string[] allFiles = Directory.GetFiles(inputPath, "*.*", SearchOption.AllDirectories);
                Console.WriteLine($"Всего файлов найдено: {allFiles.Length}");

                int imageCount = 0;
                foreach (string file in allFiles)
                {
                    string ext = Path.GetExtension(file).ToLowerInvariant();
                    Console.WriteLine($"Файл: {Path.GetFileName(file)} | Расширение: {ext}");

                    if (Array.Exists(extensions, e => e == ext))
                    {
                        imageCount++;
                        string targetPath = Path.Combine(targetFolder, Path.GetFileName(file));
                        filesToMove.Add((file, targetPath));
                    }
                }

                Console.WriteLine($"Из них изображений: {imageCount}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка доступа: {ex.Message}");
                Console.ResetColor();
            }

            if (filesToMove.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nИзображения не найдены!");
                Console.WriteLine("Убедитесь, что:");
                Console.WriteLine("1. Файлы лежат именно в папке task7_photos");
                Console.WriteLine("2. У файлов есть расширения .jpg .png .bmp");
                Console.WriteLine("3. Папка не пустая");
                Console.ResetColor();
                return;
            }

            Console.WriteLine($"\nНайдено изображений: {filesToMove.Count}");
            Console.Write("\nПереместить? (да / нет): ");
            string confirm = Console.ReadLine()?.Trim().ToLower();

            if (confirm == "да" || confirm == "y")
                ExecuteMove(filesToMove, targetFolder);
            else
                Console.WriteLine("Отменено.");
        }

        private static void ExecuteMove(List<(string source, string destination)> filesToMove, string targetFolder)
        {
            int success = 0;
            foreach (var item in filesToMove)
            {
                try
                {
                    string finalDest = item.destination;
                    int counter = 1;
                    while (File.Exists(finalDest))
                    {
                        string name = Path.GetFileNameWithoutExtension(item.destination);
                        string ext = Path.GetExtension(item.destination);
                        finalDest = Path.Combine(targetFolder, $"{name} ({counter}){ext}");
                        counter++;
                    }

                    File.Move(item.source, finalDest);
                    success++;
                    Console.WriteLine($"✓ Перемещено: {Path.GetFileName(item.source)}");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"✗ Ошибка: {Path.GetFileName(item.source)}");
                    Console.ResetColor();
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nГотово! Перемещено: {success} файлов.");
            Console.ResetColor();
        }

        private static string GetDefaultInputPath()
        {
            return @"C:\Users\МКА-ученик\source\repos\ConsoleApp15\ConsoleApp15\DataTests\task7_photos";
        }

        private static string GetTargetFolderPath()
        {
            return Path.Combine(GetKorzinaPath(), "Anapa 2007");
        }

        private static string GetKorzinaPath()
        {
            return @"C:\Users\МКА-ученик\source\repos\ConsoleApp15\ConsoleApp15\Korzina";
        }
    }
}