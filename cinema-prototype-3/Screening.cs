using cinema_prototype_3;
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
        {
            Console.WriteLine($"\nВ зале {hall.rowsNum} рядов по {hall.seatsInRowNum} мест.");
            Console.WriteLine("Введите стоимость билетов на сеанс через пробел:");

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
                            AnsiConsole.Write(new Markup("Неверные значения для цен. Повторите попытку.[/]\n"));
                            continue;
                        }

                        bool wrongPrice = false;
                        foreach (var strPrice in rawRowPrices)
                        {
                            int intPrice = int.Parse(strPrice);
                            if (intPrice < 0 | intPrice < maxBookingLength)
                            {
                                AnsiConsole.Write(new Markup("Неверные значения для цен. Повторите попытку.[/]\n"));
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
                        AnsiConsole.Write(new Markup("[red1]Неверные значения для цен. Повторите попытку.[/]\n"));
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
                            if (newPrice >= 0)
                            {
                                priceData[row][seat] = newPrice;
                                return false;
                            }
                            else
                            {
                                AnsiConsole.Write(new Markup("Неверное значение для цены.Повторите ввод.\n"));
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

    }

    internal class StandartScreening : Screening
    {
        public static string type { get { return _type; } }
        private static string _type = "Стандартный показ";
        private static int _maxBookingPeriod { get { return 14; } }

        public override void Print(int num)
        {
            if (num < 0)
                Console.WriteLine($"{type,16} {hall.name,10} {hall.GetType()} | {time.ToString("dd/MM/yyyy HH:mm"),16}");
            else
                Console.WriteLine($"{num + 1,3}: {type,16} {hall.name,10} {hall.GetType()} | {time.ToString("dd/MM/yyyy HH:mm"),16}");
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
        private static int _maxBookingPeriod { get { return 45; } }

        public string criticInvited { get { return _criticInvited; } }
        private string _criticInvited;

        public static void PrintScreeningTypeCharacteristics()
        {
            Console.WriteLine($"Тип показа: {_type}.");
            Console.WriteLine($"Предельное время, за которое можно забронировать билет на фильм: {_maxBookingPeriod}.");
            Console.WriteLine("Обязательно приглашение кинокритика.");
        }

        public bool GetFreePoster()
        {
            Console.WriteLine($"Хотите получить один бесплатный постер к фильму {_film} в подарок?");
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
                Console.WriteLine($"{type,16} {hall.name,10} {hall.GetType()} | {time.ToString("dd/MM/yyyy HH:mm"),16}");
                Console.WriteLine($"    Приглашенный кинокритик: {criticInvited,25}");
            }
            else
            {
                Console.WriteLine($"{num + 1,3}: {type, 16} {hall.name,10} {hall.GetType()} | {time.ToString("dd/MM/yyyy HH:mm"),16}");
                Console.WriteLine($"    Приглашенный кинокритик: {criticInvited,25}");
            }
        }
            
        public static void ReadFile(string filename)
            {
                var encoding = Encoding.GetEncoding(1251);
                using (StreamReader sr = new StreamReader(filename, encoding: encoding))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Screening screening = new Screening();
                        screening.ProcessData(line);
                        screening.film.screenings.Add(screening);
                    }
                }
            } // ДОПИСАТЬ
        public static void UpdateFile(string filename)
            {
                var csv = new StringBuilder();

                foreach (Film film in Film.all)
                {
                    foreach (Screening screening in film.screenings)
                    {
                        List<string> priceList = new List<string>();
                        for (int i = 0; i < screening.hall.rowsNum; i++)
                        {
                            string row = String.Join('%', screening.priceData[i]);
                            priceList.Add(row);
                        }
                        string priceString = String.Join(';', priceList);

                        string screeningInfo = $"{screening.film.name},{screening.hall.name},{screening.time},{priceString}";
                        csv.AppendLine(screeningInfo);
                    }
                }

                File.WriteAllText(filename, csv.ToString(), Encoding.GetEncoding(1251));
            } // ДОПИСАТЬ

        }

    internal class PressScreening : Screening
    {
        public static string type { get { return _type; } }
        private static string _type = "Пресс-показ";
        private static int _maxBookingPeriod { get { return 60; } }

        public string[] castMembersPresent { get { return _castMembersPresent; } }
        private string[] _castMembersPresent;

        public static void PrintScreeningTypeCharacteristics()
        {
            Console.WriteLine($"Тип показа: {_type}.");
            Console.WriteLine($"Предельное время, за которое можно забронировать билет на фильм: {_maxBookingPeriod}.");
            Console.WriteLine("Обязательно приглашение актеров, участвовавших в съемках.");
        }

        public bool GetFreePoster()
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
        public bool GetCastAuthographs()
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
                Console.WriteLine($"{type,16} {hall.name,10} {hall.GetType()} | {time.ToString("dd/MM/yyyy HH:mm"),16}");
                Console.WriteLine($"    Присутствующие актеры: {string.Join(", ", castMembersPresent)}");
            }
            else
            {
                Console.WriteLine($"{num + 1,3}: {type,16} {hall.name,10} {hall.GetType()} | {time.ToString("dd/MM/yyyy HH:mm"),16}");
                Console.WriteLine($"    Присутствующие актеры: {string.Join(", ", castMembersPresent)}");
            }
        }

        public override void ProcessData(string line)
        {
            string[] parts = line.Split(',');  //Разделитель в CSV файле.

            string filmname = parts[0];
            foreach (Film existingFilm in Film.all)
            {
                if (filmname == existingFilm.name)
                {
                    film = existingFilm;
                    break;
                }
            }

            string hallname = parts[1];
            foreach (var existingHall in Hall.all)
            {
                if (hallname == existingHall.name)
                {
                    hall = existingHall;
                    break;
                }
            }

            time = DateTime.Parse(parts[2]);
            var pricesByRows = parts[3].Split(';');
            for (int i = 0; i < hall.rowsNum; i++)
                priceData.Add(pricesByRows[i].Split('%').Select(int.Parse).ToList());

        } // ДОПИСАТЬ
        public static void ReadFile(string filename)
        {
            var encoding = Encoding.GetEncoding(1251);
            using (StreamReader sr = new StreamReader(filename, encoding: encoding))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Screening screening = new Screening();
                    screening.ProcessData(line);
                    screening.film.screenings.Add(screening);
                }
            }
        } // ДОПИСАТЬ
        public static void UpdateFile(string filename)
        {
            var csv = new StringBuilder();

            foreach (Film film in Film.all)
            {
                foreach (Screening screening in film.screenings)
                {
                    List<string> priceList = new List<string>();
                    for (int i = 0; i < screening.hall.rowsNum; i++)
                    {
                        string row = String.Join('%', screening.priceData[i]);
                        priceList.Add(row);
                    }
                    string priceString = String.Join(';', priceList);

                    string screeningInfo = $"{screening.film.name},{screening.hall.name},{screening.time},{priceString}";
                    csv.AppendLine(screeningInfo);
                }
            }

            File.WriteAllText(filename, csv.ToString(), Encoding.GetEncoding(1251));
        } // ДОПИСАТЬ
        
    }
}
