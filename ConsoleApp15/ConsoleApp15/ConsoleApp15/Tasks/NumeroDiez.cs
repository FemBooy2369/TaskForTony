// Tasks/NumeroDiez.cs
using System;
using System.IO;

namespace ConsoleApp15.Tasks
{
    public class SafeFileWriter
    {
        private readonly string filePath;
        private readonly string bakPath;
        private readonly string tmpPath;
        private static readonly string LogPath;

        static SafeFileWriter()
        {
            string korzinaPath = GetKorzinaPath();
            LogPath = Path.Combine(korzinaPath, "transactions.log");
        }

        public SafeFileWriter(string filePath)
        {
            this.filePath = filePath;
            this.bakPath = filePath + ".bak";
            this.tmpPath = filePath + ".tmp";

            // Создаём директорию, если её нет
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public bool Write(string content)
        {
            Log("START", filePath);

            try
            {
                // 1. Создаём резервную копию
                if (File.Exists(filePath))
                {
                    File.Copy(filePath, bakPath, overwrite: true);
                    Log("BACKUP_CREATED", filePath);
                }

                // 2. Записываем данные во временный файл
                File.WriteAllText(tmpPath, content);
                Log("TMP_WRITTEN", filePath);

                // 3. Атомарная замена файла
                File.Replace(tmpPath, filePath, bakPath);
                Log("SUCCESS", filePath);

                Console.WriteLine("Транзакция успешно завершена.");
                return true;
            }
            catch (IOException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка ввода/вывода: " + ex.Message);
                Console.ResetColor();
                Log("ERROR", filePath + " - " + ex.Message);
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка доступа: " + ex.Message);
                Console.ResetColor();
                Log("ERROR", filePath + " - " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Неизвестная ошибка: " + ex.Message);
                Console.ResetColor();
                Log("ERROR", filePath + " - " + ex.Message);
                return false;
            }
            finally
            {
                // Гарантированно удаляем временный файл
                try
                {
                    if (File.Exists(tmpPath))
                        File.Delete(tmpPath);
                }
                catch { }
            }
        }

        public void RecoverIfNeeded()
        {
            if (!File.Exists(LogPath))
                return;

            string lastLine = GetLastLogLine();
            if (string.IsNullOrEmpty(lastLine))
                return;

            if (lastLine.Contains("START") || lastLine.Contains("TMP_WRITTEN") || lastLine.Contains("ERROR"))
            {
                Console.WriteLine("Обнаружена незавершенная транзакция. Выполняется восстановление...");

                try
                {
                    if (File.Exists(bakPath))
                    {
                        File.Copy(bakPath, filePath, overwrite: true);
                        Console.WriteLine("Восстановление из резервной копии выполнено.");
                        Log("RECOVERED", filePath);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Не удалось восстановить файл: " + ex.Message);
                    Console.ResetColor();
                }
            }
        }

        private static void Log(string status, string message)
        {
            try
            {
                string entry = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | " + status + " | " + message;
                File.AppendAllText(LogPath, entry + Environment.NewLine);
            }
            catch { }
        }

        private string GetLastLogLine()
        {
            try
            {
                if (!File.Exists(LogPath)) return "";
                var lines = File.ReadAllLines(LogPath);
                return lines.Length > 0 ? lines[lines.Length - 1] : "";
            }
            catch { return ""; }
        }

        private static string GetKorzinaPath()
        {
            string path = @"C:\Users\МКА-ученик\source\repos\ConsoleApp15\ConsoleApp15\Korzina";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }

    public class NumeroDiez
    {
        public static void Execute()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== ТРАНЗАКЦИОННАЯ ЗАПИСЬ В ФАЙЛ ===");
            Console.ResetColor();

            Console.Write("Введите путь к файлу (Enter = test10.txt): ");
            string filePath = Console.ReadLine()?.Trim('\"', ' ');

            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(GetKorzinaPath(), "test10.txt");
            }

            SafeFileWriter writer = new SafeFileWriter(filePath);

            // Проверка и восстановление после возможного сбоя
            writer.RecoverIfNeeded();

            Console.WriteLine("\nВведите текст для записи в файл (Enter - завершить):");
            string content = Console.ReadLine();

            if (!string.IsNullOrEmpty(content))
            {
                writer.Write(content);
            }
            else
            {
                Console.WriteLine("Запись отменена.");
            }
        }

        private static string GetKorzinaPath()
        {
            return @"C:\Users\МКА-ученик\source\repos\ConsoleApp15\ConsoleApp15\Korzina";
        }
    }
}