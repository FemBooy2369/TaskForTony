// Program.cs
using System;
using System.Collections.Generic;

namespace ConsoleApp15
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Меню заданий";

            List<MenuItem> menuItems = new List<MenuItem>
            {
                new MenuItem("5. Копировальщик с прогрессом", Tasks.NumeroCinco.Execute),
                new MenuItem("6. Простое шифрование XOR", Tasks.NumeroSeis.Execute),
                new MenuItem("7. Менеджер фотоархива", Tasks.NumeroSiete.Execute),
                new MenuItem("8. Сериализация книг (JSON + XML)", Tasks.NumeroOcho.Execute),
                new MenuItem("9. Поиск дубликатов по хешу", Tasks.NumeroNueve.Execute),
                new MenuItem("10. Транзакционная запись с восстановлением", Tasks.NumeroDiez.Execute),
                new MenuItem("Выход из программы", null),
               
            };

            int selectedIndex = 0;

            while (true)
            {
                DrawMenu(menuItems, selectedIndex);

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (selectedIndex > 0)
                            selectedIndex--;
                        break;

                    case ConsoleKey.DownArrow:
                        if (selectedIndex < menuItems.Count - 1)
                            selectedIndex++;
                        break;

                    case ConsoleKey.Enter:
                        if (menuItems[selectedIndex].Action == null)
                            return;

                        Console.Clear();
                        try
                        {
                            menuItems[selectedIndex].Action.Invoke();
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Ошибка: " + ex.Message);
                            Console.ResetColor();
                        }

                        Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
                        Console.ReadKey(true);
                        break;
                }
            }
        }

        static void DrawMenu(List<MenuItem> items, int selected)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===================== МЕНЮ ЗАДАНИЙ ======================");
            Console.ResetColor();
            Console.WriteLine();

            for (int i = 0; i < items.Count; i++)
            {
                if (i == selected)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" >> ");
                }
                else
                {
                    Console.Write("    ");
                }

                Console.WriteLine(items[i].Title);
                Console.ResetColor();
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Управление: ↑ ↓  — перемещение,  Enter  — выбрать");
            Console.ResetColor();
        }
    }

    class MenuItem
    {
        public string Title { get; }
        public Action Action { get; }

        public MenuItem(string title, Action action)
        {
            Title = title;
            Action = action;
        }
    }
}