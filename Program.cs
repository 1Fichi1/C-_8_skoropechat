using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Numerics;

namespace SpeedTest
{
    internal class Menu
    {
        static public void MainMenu()
        {
            int MaxVerPos = 2;
            int MinVerPos = 1;
            int VerPos = 1;
            ConsoleKeyInfo key;

            do
            {
                WriteMenu();
                WriteCursor(VerPos);
                key = Console.ReadKey(true);

                VerPos = UpdateCursorPos(VerPos, MaxVerPos, MinVerPos, key.Key);
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        if (VerPos == 1)
                        {
                            Console.Clear();
                            new Test().Start();
                        }
                        else if (VerPos == 2)
                        {
                            Console.Clear();
                            Table.GetUsers();
                        }
                        break;

                }
                Console.Clear();
            } while (key.Key != ConsoleKey.F9);
        }
        static private int UpdateCursorPos(int VerPos, int MaxVerPos, int MinVerPos, ConsoleKey key)
        {
            switch (key)
            {
                case (ConsoleKey.W):
                    VerPos--;
                    if (VerPos < MinVerPos)
                    {
                        VerPos = MinVerPos;
                    }
                    break;
                case (ConsoleKey.S):
                    VerPos++;
                    if (VerPos > MaxVerPos)
                    {
                        VerPos = MaxVerPos;
                    };
                    break;
            }
            return VerPos;
        }
        static private void WriteCursor(int VerPos)
        {
            Console.SetCursorPosition(0, VerPos);
            Console.WriteLine("-->");
        }
        static private void WriteMenu()
        {
            Console.WriteLine();
            Console.WriteLine("       Начать тест на скоропечатание");
            Console.WriteLine("       Посмотреть таблицу рекордов");
            Console.WriteLine();
            Console.WriteLine("Чтобы выйти из программы необходимо нажать F9");
        }
    }
}
namespace SpeedTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Menu.MainMenu();
        }
    }
}
namespace SpeedTest
{
    internal class Table
    {
        static public void GetUsers()
        {
            List<User> users;
            string table = File.ReadAllText("C:\\mpt\\С#\\SpeedTest\\Table.json");
            users = JsonConvert.DeserializeObject<List<User>>(table);

            Console.WriteLine("Таблица рекордов");
            Console.WriteLine("Имя пользователя Кол-во/минута Кол-во/секунда");
            foreach (User user in users)
            {
                Console.WriteLine($"{user.name} {user.SymbolsPerMinute} {user.SymbolsPerSecond}");
            }
            Console.WriteLine("");
            Console.WriteLine("Чтобы вернуться в главное меню нажмите любую клавишу");
            Console.ReadKey();
        }
    }
}
namespace SpeedTest
{
    internal class Test
    {
        bool timer;
        private int Letters = 0;
        string username;
        ConsoleKey key;
        public void Start()
        {
            do
            {
                Console.WriteLine("Введите ваше имя для таблицы рекордов");
                username = Console.ReadLine();
                Console.Clear();
            } while (username == "");

            char[] text = GetText();
            foreach (char letter in text)
            {
                Console.Write(letter);
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Нажмите Enter, чтобы начать тест");

            while (key != ConsoleKey.Enter)
            {
                key = Console.ReadKey(true).Key;
            }

            int HorPos = 0;
            int VerPos = 0;
            int LetterNum = 0;

            new Thread(Timer).Start();
            Console.SetCursorPosition(0, 0);
            while (timer != true)
            {
                Console.CursorVisible = true;
                char InputKey = Console.ReadKey(true).KeyChar;
                if (InputKey == text[LetterNum])
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(text[LetterNum]);
                    Console.ResetColor();
                    LetterNum++;
                    Letters++;

                    try
                    {
                        Console.SetCursorPosition(HorPos, VerPos);
                        HorPos++;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        HorPos = 1;
                        VerPos++;
                        Console.SetCursorPosition(HorPos, VerPos);
                    }
                }
            }
            Console.CursorVisible = false;
            Record(username, Letters, Letters / 60);

            Console.Clear();
            Console.WriteLine("Ваши результаты");
            Console.WriteLine(username);
            Console.WriteLine(Letters);
            Console.WriteLine(Letters / 60);

            Console.WriteLine();
            Console.WriteLine("Нажмите любую клавишк, чтобы вернуться в меню");
            Console.ReadKey();
        }
        private void Timer()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (60 - stopwatch.ElapsedMilliseconds / 1000 >= 0)
            {
                Console.CursorVisible = false;

                Console.SetCursorPosition(0, 10);
                Console.WriteLine("Осталось: " + (60 - (stopwatch.ElapsedMilliseconds) / 1000));
                Thread.Sleep(1000);
            }

            stopwatch.Stop();
            stopwatch.Reset();

            timer = true;
        }
        private char[] GetText()
        {
            string text = "Иногда кажется, что расстояние всегда играет против.Особенно, когда на него накладывается время.Есть что-то абсолютно правильное в предоставлении другому свободы. Без истерик. Без пыток типа а где ты был? а кто там был ещё? а что?.. а зачем?..Но что-то безжалостно размазывает этот замок по плоскости расстояния и времени. Как замок из песка, накрытый волной - думали, делали, старались, но - грохот разбившей его волны откатывает, чтобы открыть идеально ровный песчаный пляж: давайте, стройте заново...Не могу понять - это иллюзия или действительно неправильно - слишком много времени, проведённого не вместе.";
            char[] chars = text.ToCharArray();

            return chars;
        }
        public List<User> GetUsers()
        {
            List<User> users;
            string table = File.ReadAllText("C:\\mpt\\С#\\SpeedTest\\Table.json");
            return JsonConvert.DeserializeObject<List<User>>(table);
        }
        public void Record(string name = "Имя", int SymbolsPerMinute = 0, int SymbolsPerSecond = 0)
        {

            User user = new User();
            user.name = name;
            user.SymbolsPerMinute = SymbolsPerMinute;
            user.SymbolsPerSecond = SymbolsPerSecond;

            List<User> users = GetUsers();
            if (users != null)
            {
                users.Add(user);
                string json = JsonConvert.SerializeObject(users);
                File.WriteAllText("C:\\mpt\\С#\\SpeedTest\\Table.json", json);
            }
            else
            {
                string json = JsonConvert.SerializeObject(user);
                File.WriteAllText("C:\\mpt\\С#\\SpeedTest\\Table.json", json);
            }
        }
    }
}

namespace SpeedTest
{
    internal class User
    {
        public string name;
        public int SymbolsPerMinute;
        public int SymbolsPerSecond;
    }
}