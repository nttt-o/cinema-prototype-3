using cinema_prototype_3;
using Newtonsoft.Json;
using Spectre.Console;
using System.Collections;
using System.Globalization;
using System.Text;


namespace cinema_prototype_3
{
    internal class Screening
    {
        public Film film {get { return _film; } set { _film = value; } }
        protected Film _film;

        public Hall hall { get { return _hall; } set { _hall = value; } }
        protected Hall _hall;

        public DateTime time { get; set; }
        protected DateTime _time;

        public virtual int maxBookingPeriod { get { return 0; } }
        public static string type { get { return _type; } }
        private static string _type = "";

        public List<List<char>> seatsAvailability { get { return _seatsAvailability; } set { _seatsAvailability = value; }}
        protected List<List<char>> _seatsAvailability = new List<List<char>>();

        public List<List<int>> priceData { get { return _priceData; } set { _priceData = value; }}
        protected List<List<int>> _priceData = new List<List<int>>(); 

        public virtual void Print(int num)
        {
            Console.WriteLine($"{type,16} {hall.name,10} {hall.GetType()} | {time.ToString("dd/MM/yyyy HH:mm"),16}");
        }

        public void SetInitialAvailability()
        {
            for (int i = 0; i < hall.rowsNum; i++)
            {
                List<char> row = new List<char>();
                for (int j = 0; j < hall.seatsInRowNum; j++)
                    row.Add('0');
                seatsAvailability.Add(row);
            }
        }
        public void SetInitialPrices()
        {;
            Console.WriteLine("\nНапоминаем информацию о текущем зале.\n");
            hall.PrintCurrentHallInfoForAdministrator();

            Console.WriteLine("\nХотите установить цены на билеты по умолчанию (минимальная цена, указанная выше)?");
            string choice = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                .AddChoice("да")
                                                .AddChoice("нет")
                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
            if (choice == "да")
            {
                AutoPrices(hall.minPrice);
                return;
            }

            Console.WriteLine("\nВведите стоимость билетов на сеанс через пробел:");

            for (int i = 0; i < hall.rowsNum; i++)
            {
                List<int> rowPrices = new List<int>();
                bool arePricesValid = false;
                string[] validPrices = new string[hall.seatsInRowNum];

                do
                {
                    string[] rawRowPrices = AnsiConsole.Prompt(new TextPrompt<string>("Ряд " + Convert.ToString(i + 1) + "> ")).Split(' ');
                    try
                    {
                        if (rawRowPrices.Length != hall.seatsInRowNum)
                        {
                            AnsiConsole.Write(new Markup("Неверные значения для цен. Повторите попытку.\n"));
                            continue;
                        }

                        bool wrongPrice = false;
                        foreach (var strPrice in rawRowPrices)
                        {
                            int intPrice = int.Parse(strPrice);
                            if (intPrice < 0 | intPrice < hall.minPrice)
                            {
                                AnsiConsole.Write(new Markup("Неверные значения для цен. Повторите попытку.\n"));
                                wrongPrice = true;
                                break;
                            }
                        }

                        if (wrongPrice)
                            continue;

                        validPrices = rawRowPrices;
                        arePricesValid = true;
                    }
                    catch (Exception)
                    {
                        AnsiConsole.Write(new Markup("Неверные значения для цен. Повторите попытку.\n"));
                    }
                } while (arePricesValid == false);

                // сохраняю цены в матрицу
                foreach (var item in validPrices)
                    rowPrices.Add(int.Parse(item));
                priceData.Add(rowPrices);
            }
        }
        public void PrintHallData(string printChoice) // печать таблицы с ценами (по аргументу "prices") или доступностью мест (по аргументу "availability")
        {
            Table table = new Table().Border(TableBorder.DoubleEdge).AddColumn(new TableColumn("Место\nРяд"));

            for (int i = 1; i < hall.seatsInRowNum + 1; i++)
                table.AddColumn(new TableColumn(Convert.ToString(i)).Centered());

            for (int i = 0; i < hall.rowsNum; i++)
            {
                table.AddEmptyRow();
                table.UpdateCell(i, 0, new Text(Convert.ToString(i + 1)));

                for (int j = 0; j < hall.seatsInRowNum; j++)
                {
                    if (printChoice == "availability")
                        table.UpdateCell(i, j + 1, Convert.ToString(seatsAvailability[i][j]));
                    else if (printChoice == "prices")
                        table.UpdateCell(i, j + 1, Convert.ToString(priceData[i][j]));
                }
            }
            AnsiConsole.Write(table);

        }
        public void UpdateSeats(List<int> place)
        {
            int row = place[0]; int seat = place[1];
            seatsAvailability[row][seat] = 'x'; // вносим, что место было куплено
        }
        public void MarkSeatsAsTaken()
        {
            AnsiConsole.Write(new Markup("Данные о местах [green](0 - место доступно;[/] [red] x - место выкуплено)[/]\n"));
            this.PrintHallData("availability");
            string answer = "да";
            do
            {
                try
                {
                    Console.WriteLine("\nПожалуйста, выберите места, занятость которых вы хотите изменить.");
                    Console.WriteLine("Введите один номер места в формате '<номер ряда> <номер места>'.");

                    string[] seatData = AnsiConsole.Prompt(new TextPrompt<string>("> ")).Split(' ');
                    List<int> ticket = new List<int> { int.Parse(seatData[0]) - 1, int.Parse(seatData[1]) - 1 };
                    int areRowSeatValid = priceData[ticket[0]][ticket[1]]; // ловим IndexOutOfRangeException до добавления к заказу

                    if (seatsAvailability[ticket[0]][ticket[1]] == '0')
                        seatsAvailability[ticket[0]][ticket[1]] = 'x';
                    else
                        AnsiConsole.Write(new Markup("Данное место уже занято.\n"));

                    AnsiConsole.Write(new Markup("Хотите продолжить выбор мест?\n"));
                    answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                        .AddChoice("да")
                                                        .AddChoice("нет")
                                                        .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз."));
                }
                catch (Exception)
                {
                    AnsiConsole.Write(new Markup("Неверное значение для ряда и/или места. Повторите ввод.\n"));
                }
            } while (answer == "да");
        }
        public bool ChangePrices()
        {
            AnsiConsole.Write(new Markup("Текущие цены:\n"));
            PrintHallData("prices");
            AnsiConsole.Write(new Markup("Вы находитесь в режиме изменения цен.\n"));

            do
            {
                AnsiConsole.Write(new Markup("Введите один номер места в формате '<номер ряда> <номер места>'.\nЧтобы вернуться в режим изменения данных, введите 'м'.\n"));
                string input = AnsiConsole.Prompt(new TextPrompt<string>("> "));

                if (input == "м")
                    return true;

                try
                {
                    string[] seatData = input.Split(' ');
                    int row = int.Parse(seatData[0]) - 1;
                    int seat = int.Parse(seatData[1]) - 1;
                    int areRowSeatValid = priceData[row][seat]; // ловим IndexOutOfRangeException до ввода цены

                    do
                    {
                        AnsiConsole.Write(new Markup("Введите новую цену для выбранного места.\n"));
                        AnsiConsole.Write(new Markup("Если вы передумали менять цену на данное место, введите 'м'.\n"));
                        string strPrice = AnsiConsole.Prompt(new TextPrompt<string>("> "));

                        if (strPrice == "м")
                        {
                            Console.WriteLine();
                            break;
                        }

                        try
                        {
                            int newPrice = int.Parse(strPrice);
                            if (newPrice >= 0 && newPrice >= hall.minPrice)
                            {
                                priceData[row][seat] = newPrice;
                                return false;
                            }
                            else
                            {
                                AnsiConsole.Write(new Markup("Неверное значение для цены.Повторите ввод.\n"));
                                Console.WriteLine($"Напоминание: минимальная цена на билет в зале {hall.name} типа {hall.GetHallType()}: {hall.minPrice}");
                                continue;
                            }
                        }
                        catch (Exception)
                        {
                            AnsiConsole.Write(new Markup("Неверное значение для цены.Повторите ввод.[/]\n"));
                        }
                    } while (true);
                }
                catch (Exception)
                {
                    AnsiConsole.Write(new Markup("Неверное значение для ряда и/или места.Повторите ввод.\n\n"));
                }

            } while (true);
        }
        public bool CanTimeBeEdited()
        {
            bool canBeEdited = true;

            for (int i = 0; i < hall.rowsNum; i++)
            {
                for (int j = 0; j < hall.seatsInRowNum; j++)
                {
                    if (seatsAvailability[i][j] == 'x')
                    {
                        canBeEdited = false;
                        break;
                    }
                }
            }
            return canBeEdited;
        }
        public void AutoPrices(int price)
        {
            for (int i = 0; i < hall.rowsNum; i++)
            {
                List<int> row = new List<int>();
                for (int j = 0; j < hall.seatsInRowNum; j++)
                    row.Add(price);
                _priceData.Add(row);
            }
        }

    }

    internal class StandartScreening : Screening
    {
        public static string type { get { return _type; } }
        private static string _type = "Стандартный показ";

        public override int maxBookingPeriod { get { return _maxBookingPeriod; } }
        private static int _maxBookingPeriod = 14;
           

        public override void Print(int num)
        {
            if (num < 0)
                Console.WriteLine($"{type,16} | {hall.name,10} {hall.GetHallType()} | {time.ToString("dd/MM/yyyy HH:mm"),16}");
            else
                Console.WriteLine($"{num + 1,3}: {type,18} | {hall.name,12} | {hall.GetHallType(), 9} | {time.ToString("dd/MM/yyyy HH:mm"),16}");
        }

        public static void PrintScreeningTypeCharacteristics()
        {
            Console.WriteLine($"Тип показа: {_type}.");
            Console.WriteLine($"Предельное время, за которое можно забронировать билет на фильм: {_maxBookingPeriod}.");
            Console.WriteLine("Дополнительные характеристики отсутствуют.");
        }
    }

    internal class PremiereScreening : Screening
    {
        public static string type { get { return _type; } }
        private static string _type = "Премьерный показ";
        public override int maxBookingPeriod { get { return _maxBookingPeriod; } }
        private static int _maxBookingPeriod = 45;

        public string criticInvited { get { return _criticInvited; } set { _criticInvited = value; } }
        private string _criticInvited;
        public void SetCriticInvited()
        {
            Console.WriteLine("Введите имя приглашенного кинокритика.");
            while (true)
            {
                string inputName = AnsiConsole.Prompt(new TextPrompt<string>("> "));
                if(!inputName.ToCharArray().Any(c => char.IsLetter(c)))
                {
                    Console.WriteLine("Некорректный ввод. Повторите попытку. Имя должно содержать хотя бы одну букву.");
                    continue;
                }
                _criticInvited = inputName;
                break;

            }
        }

        public static void PrintScreeningTypeCharacteristics()
        {
            Console.WriteLine($"Тип показа: {_type}.");
            Console.WriteLine($"Предельное время, за которое можно забронировать билет на фильм: {_maxBookingPeriod}.");
            Console.WriteLine("Обязательно приглашение кинокритика.");
        }

        public static bool GetFreePoster()
        {
            Console.WriteLine($"Хотите получить один бесплатный постер к фильму в подарок?");
            string answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                .AddChoice("да")
                                                .AddChoice("нет")
                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
            if (answer == "да")
                return true;
            return false;
        }

        public override void Print(int num)
        {
            if (num < 0)
            {
                Console.WriteLine($"{type,16} | {hall.name,10} {hall.GetHallType()} | {time.ToString("dd/MM/yyyy HH:mm"),16}");
                Console.WriteLine($"                Приглашенный кинокритик: {criticInvited}");
            }
            else
            {
                Console.WriteLine($"{num + 1,3}: {type,18} | {hall.name,12} | {hall.GetHallType(), 9} | {time.ToString("dd/MM/yyyy HH:mm"),16}");
                Console.WriteLine($"{"", 23}   Приглашенный кинокритик: {criticInvited}");
            }
        }
    }

    internal class PressScreening : Screening
    {
        public static string type { get { return _type; } }
        private static string _type = "Пресс-показ";
        public override int maxBookingPeriod { get { return _maxBookingPeriod; } }
        private static int _maxBookingPeriod = 60;

        public string[] castMembersPresent { get { return _castMembersPresent; } set { _castMembersPresent = value; } }
        private string[] _castMembersPresent;
        public void SetCastMembersPresent()
        {
            while (true)
            {
                string[] inputNames = AnsiConsole.Prompt(new TextPrompt<string>("> ")).Split(", ");
                if (!inputNames.All(name => name.ToCharArray().Any(c => char.IsLetter(c))))
                {
                    Console.WriteLine("Некорректный ввод. Повторите попытку. Имена должны содержать хотя бы одну букву.");
                    continue;
                }
                _castMembersPresent = inputNames;
                break;
            }
        }

        public static void PrintScreeningTypeCharacteristics()
        {
            Console.WriteLine($"Тип показа: {_type}.");
            Console.WriteLine($"Предельное время, за которое можно забронировать билет на фильм: {_maxBookingPeriod}.");
            Console.WriteLine("Обязательно приглашение актеров, участвовавших в съемках.");
        }

        public static bool GetFreePoster()
        {
            Console.WriteLine("Хотите получить один бесплатный постер к фильму в подарок?");
            string answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                .AddChoice("да")
                                                .AddChoice("нет")
                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
            if (answer == "да")
                return true;
            
            return false;
        }
        public static bool GetCastAuthographs()
        {
            Console.WriteLine("Хотите получить автографы актеров в подарок?");
            string answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                .AddChoice("да")
                                                .AddChoice("нет")
                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
            if (answer == "да")
                return true;
            
            return false;
        }

        public override void Print(int num)
        {
            if (num < 0)
            {
                Console.WriteLine($"{type,16} | {hall.name,10} | {hall.GetHallType()} | {time.ToString("dd/MM/yyyy HH:mm"),16}");
                Console.WriteLine($"                Присутствующие актеры: {string.Join(", ", castMembersPresent)}");
            }
            else
            {
                Console.WriteLine($"{num + 1,3}: {type,18} | {hall.name,12} | {hall.GetHallType(), 9} | {time.ToString("dd/MM/yyyy HH:mm"),16}");
                Console.WriteLine($"{"",23}   Присутствующие актеры: {string.Join(", ", castMembersPresent)}");
            }
        }
        
    }
}
