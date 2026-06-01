using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

namespace ConsoleApp15.Tasks
{
    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public int Pages { get; set; }

        public Book() { }

        public Book(string title, string author, int year, int pages)
        {
            Title = title;
            Author = author;
            Year = year;
            Pages = pages;
        }

        public override string ToString()
        {
            return Title + " - " + Author + " (" + Year + " г.), " + Pages + " стр.";
        }
    }

    public class NumeroOcho
    {
        private static string GetKorzinaPath()
        {
            return @"C:\Users\МКА-ученик\source\repos\ConsoleApp15\ConsoleApp15\Korzina";
        }

        private static string JsonPath => Path.Combine(GetKorzinaPath(), "books.json");
        private static string XmlPath => Path.Combine(GetKorzinaPath(), "books.xml");

        public static void Execute()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== СЕРИАЛИЗАЦИЯ КОЛЛЕКЦИИ КНИГ (JSON + XML) ===");
            Console.ResetColor();

            List<Book> books = LoadBooks();

            Console.WriteLine("Загружено книг: " + books.Count);

            while (true)
            {
                Console.WriteLine("\n=== Меню задания 8 ===");
                Console.WriteLine("1. Добавить новую книгу");
                Console.WriteLine("2. Показать все книги");
                Console.WriteLine("3. Сохранить в JSON и XML");
                Console.WriteLine("4. Выход в главное меню");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        AddBook(books);
                        break;
                    case "2":
                        ShowBooks(books);
                        break;
                    case "3":
                        SaveBooks(books);
                        Console.WriteLine("Данные успешно сохранены в books.json и books.xml");
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            }
        }

        private static List<Book> LoadBooks()
        {
            Console.WriteLine("\nВыберите формат загрузки:");
            Console.WriteLine("1. JSON");
            Console.WriteLine("2. XML");
            Console.Write("Ваш выбор (Enter = JSON): ");
            string choice = Console.ReadLine()?.Trim();

            if (choice == "2")
                return LoadFromXml();
            else
                return LoadFromJson();
        }

        private static List<Book> LoadFromJson()
        {
            try
            {
                if (File.Exists(JsonPath))
                {
                    string json = File.ReadAllText(JsonPath);
                    var books = JsonSerializer.Deserialize<List<Book>>(json);
                    Console.WriteLine("Данные загружены из JSON.");
                    return books ?? new List<Book>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка загрузки JSON: " + ex.Message);
            }
            return new List<Book>();
        }

        private static List<Book> LoadFromXml()
        {
            try
            {
                if (File.Exists(XmlPath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Book>));
                    using (FileStream fs = new FileStream(XmlPath, FileMode.Open))
                    {
                        var books = (List<Book>)serializer.Deserialize(fs);
                        Console.WriteLine("Данные загружены из XML.");
                        return books ?? new List<Book>();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка загрузки XML: " + ex.Message);
            }
            return new List<Book>();
        }

        private static void AddBook(List<Book> books)
        {
            Console.Write("\nНазвание книги: ");
            string title = Console.ReadLine();

            Console.Write("Автор: ");
            string author = Console.ReadLine();

            Console.Write("Год издания: ");
            int.TryParse(Console.ReadLine(), out int year);

            Console.Write("Количество страниц: ");
            int.TryParse(Console.ReadLine(), out int pages);

            books.Add(new Book(title, author, year, pages));
            Console.WriteLine("Книга успешно добавлена!");
        }

        private static void ShowBooks(List<Book> books)
        {
            if (books.Count == 0)
            {
                Console.WriteLine("Коллекция книг пуста.");
                return;
            }

            Console.WriteLine("\nСписок книг:");
            for (int i = 0; i < books.Count; i++)
            {
                Console.WriteLine((i + 1) + ". " + books[i]);
            }
        }

        private static void SaveBooks(List<Book> books)
        {
            try
            {
                string json = JsonSerializer.Serialize(books, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(JsonPath, json);

                XmlSerializer serializer = new XmlSerializer(typeof(List<Book>));
                using (FileStream fs = new FileStream(XmlPath, FileMode.Create))
                {
                    serializer.Serialize(fs, books);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка сохранения: " + ex.Message);
            }
        }
    }
}