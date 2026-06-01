using System;
using System.Diagnostics;
using System.IO;

namespace ConsoleApp15.Tasks
{
    public class NumeroCinco
    {
        public static void Execute()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== КОПИРОВАЛЬЩИК С ПРОГРЕССОМ ===");
            Console.ResetColor();

            Console.Write("Введите путь к исходному файлу: ");
            string sourcePath = Console.ReadLine()?.Trim('\"', ' ');

            if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Файл не найден!");
                Console.ResetColor();
                return;
            }

            Console.Write("Введите имя копии (или Enter для имени с _copy): ");
            string fileNameInput = Console.ReadLine()?.Trim();

            string destPath = string.IsNullOrEmpty(fileNameInput)
                ? GetDefaultCopyPath(sourcePath)
                : Path.Combine(GetKorzinaPath(), fileNameInput);

            const int bufferSize = 4096;
            byte[] buffer = new byte[bufferSize];

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                using (FileStream source = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
                using (FileStream destination = new FileStream(destPath, FileMode.Create, FileAccess.Write))
                {
                    long totalBytes = source.Length;
                    long copiedBytes = 0;
                    int bytesRead;

                    while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        destination.Write(buffer, 0, bytesRead);
                        copiedBytes += bytesRead;

                        double progress = (double)copiedBytes / totalBytes * 100;
                        Console.Write($"\rПрогресс: {progress:F2}%  ({copiedBytes}/{totalBytes} байт)");
                    }
                }

                stopwatch.Stop();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Копирование завершено за {stopwatch.Elapsed.TotalSeconds:F2} секунд.");
                Console.WriteLine($"Файл сохранён: {destPath}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка копирования: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static string GetDefaultCopyPath(string sourcePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(sourcePath);
            string extension = Path.GetExtension(sourcePath);
            return Path.Combine(GetKorzinaPath(), fileName + "_copy" + extension);
        }

        private static string GetKorzinaPath()
        {
            return @"C:\Users\МКА-ученик\source\repos\ConsoleApp15\ConsoleApp15\Korzina";
        }
    }
}