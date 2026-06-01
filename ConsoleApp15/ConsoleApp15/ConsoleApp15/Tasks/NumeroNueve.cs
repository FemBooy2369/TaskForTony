using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleApp15.Tasks
{
    public class NumeroNueve
    {
        public static void Execute()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== ПОИСК ДУБЛИКАТОВ ПО ХЕШУ ===");
            Console.ResetColor();

            string defaultPath = GetDefaultPath();

            Console.Write($"Введите путь к папке (Enter = task9_files): ");
            string inputPath = Console.ReadLine()?.Trim('\"', ' ');

            if (string.IsNullOrEmpty(inputPath))
                inputPath = defaultPath;

            if (!Directory.Exists(inputPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Папка не существует!");
                Console.ResetColor();
                return;
            }

            Console.WriteLine("\nСканирование папки...");
            var duplicates = FindDuplicates(inputPath);

            if (duplicates.Count == 0)
            {
                Console.WriteLine("Дубликаты не найдены.");
                return;
            }

            string reportPath = SaveReport(duplicates, inputPath);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Найдено групп дубликатов: {duplicates.Count}");
            Console.WriteLine($"Отчет сохранен: {reportPath}");
            Console.ResetColor();
        }

        private static Dictionary<string, List<string>> FindDuplicates(string rootPath)
        {
            // Группировка по размеру файла (оптимизация)
            var sizeGroups = new Dictionary<long, List<string>>();

            foreach (string file in Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories))
            {
                long size = new FileInfo(file).Length;
                if (!sizeGroups.ContainsKey(size))
                    sizeGroups[size] = new List<string>();

                sizeGroups[size].Add(file);
            }

            var duplicates = new Dictionary<string, List<string>>(); // hash -> files

            using (MD5 md5 = MD5.Create())
            {
                foreach (var group in sizeGroups)
                {
                    if (group.Value.Count < 2)
                        continue; // если только один файл такого размера - пропускаем

                    var hashGroups = new Dictionary<string, List<string>>();

                    foreach (string file in group.Value)
                    {
                        string hash = GetFileHash(file, md5);
                        if (!hashGroups.ContainsKey(hash))
                            hashGroups[hash] = new List<string>();

                        hashGroups[hash].Add(file);
                    }

                    foreach (var hashGroup in hashGroups)
                    {
                        if (hashGroup.Value.Count > 1)
                        {
                            duplicates[hashGroup.Key] = hashGroup.Value;
                        }
                    }
                }
            }

            return duplicates;
        }

        private static string GetFileHash(string filePath, MD5 md5)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                byte[] hashBytes = md5.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private static string SaveReport(Dictionary<string, List<string>> duplicates, string rootPath)
        {
            string reportPath = Path.Combine(GetKorzinaPath(), "duplicates.txt");
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("=== ОТЧЕТ О ДУБЛИКАТАХ ===");
            sb.AppendLine("Папка сканирования: " + rootPath);
            sb.AppendLine("Дата: " + DateTime.Now);
            sb.AppendLine("=====================================\n");

            int groupNumber = 1;
            foreach (var group in duplicates)
            {
                sb.AppendLine($"Группа {groupNumber} (хеш: {group.Key})");
                foreach (string file in group.Value)
                {
                    sb.AppendLine("  " + file);
                }
                sb.AppendLine();
                groupNumber++;
            }

            File.WriteAllText(reportPath, sb.ToString());
            return reportPath;
        }

        private static string GetDefaultPath()
        {
            return @"C:\Users\МКА-ученик\source\repos\ConsoleApp15\ConsoleApp15\DataTests\task9_files";
        }

        private static string GetKorzinaPath()
        {
            string path = @"C:\Users\МКА-ученик\source\repos\ConsoleApp15\ConsoleApp15\Korzina";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}