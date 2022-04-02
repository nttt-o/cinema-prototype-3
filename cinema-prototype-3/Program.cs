using cinema_prototype_3;
using Newtonsoft.Json;
using Spectre.Console;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Reflection;

internal class Program
{

    public static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        Administrator admin = new Administrator();
        Console.WriteLine("С началом работы! Исходные данные были загружены из файлов.");
        StartAdministratorInterface();

        while (true)
        {
            // выбираем интерфейс для работы
            AnsiConsole.Write(new Markup("\nВы находитесь в меню выбора интерфейса.\n"));
            AnsiConsole.Write(new Markup("1 - перейти к интерфейсу администратора;\n2 - перейти к интерфейсу пользователя;\n3 - завершить работу.\n"));
            string command = AnsiConsole.Prompt(new TextPrompt<string>("")
                                            .AddChoice("1")
                                            .AddChoice("2")
                                            .AddChoice("3")
                                            .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз.\n"));

            if (command == "1")
            {
                bool needsToReturn = admin.Check_Password();
                Console.WriteLine();

                if (needsToReturn)
                    continue;

                ExtraAdministratorInterface();
            }

            else if (command == "2")
                UserInterface();

            else if (command == "3")
            {
                Console.WriteLine("Завершаю работу...");

                string userFilename = @"users.json";
                User.UpdateFile(userFilename);

                string standarthallFilename = @"standarthalls.json";
                string luxehallFilename = @"luxehalls.json";
                string blackhallFilename = @"blackhalls.json";

                StandartHall.UpdateFile(standarthallFilename);
                LuxeHall.UpdateFile(luxehallFilename);
                BlackHall.UpdateFile(blackhallFilename);

                string filmFilename = @"films.json";
                Film.UpdateFile(filmFilename);

                break;
            }
        }
        Console.ReadKey();
    }

    static void StartAdministratorInterface()
    {
        //List<StandartHall> stHalls = new List<StandartHall>();

        //StandartHall s1 = new StandartHall { name = "standart one" };
        //StandartHall s2 = new StandartHall { name = "standart two" };

        //stHalls.Add(s1);
        //stHalls.Add(s2);

        //List<LuxeHall> luHalls = new List<LuxeHall>();

        //LuxeHall l1 = new LuxeHall { name = "luxe one" };
        //LuxeHall l2 = new LuxeHall { name = "luxe two" };

        //luHalls.Add(l1);
        //luHalls.Add(l2);

        //List<BlackHall> blHalls = new List<BlackHall>();
        //BlackHall b1 = new BlackHall { name = "black one" };
        //blHalls.Add(b1);


        //List<Film> films = new List<Film>();
        //Film f1 = new Film { name = "Вверх", ageRestriction = "6+", halls = new List<Hall> { s1 }, screenings = new List<Screening>() };
        //Film f2 = new Film { name = "Вниз", ageRestriction = "0+", halls = new List<Hall> { l1 }, screenings = new List<Screening>() };
        //Film f3 = new Film { name = "Один дома", ageRestriction = "12+", halls = new List<Hall> { l2 }, screenings = new List<Screening>() };
        //Film f4 = new Film { name = "Искупление", ageRestriction = "18+", halls = new List<Hall> { b1 }, screenings = new List<Screening>() };

        //StandartScreening st1 = new StandartScreening { film = f1, hall = s1, time = DateTime.Parse("11.04.2022 17:30:00") };
        //st1.SetInitialAvailability();
        //st1.AutoPrices(s1.minPrice);
        //f1.screenings.Add(st1);

        //PremiereScreening premScr = new PremiereScreening { film = f2, hall = l1, time = DateTime.Parse("01.05.2022 17:30:00"), criticInvited = "Наталия Осина" };
        //premScr.AutoPrices(l1.minPrice);
        //premScr.SetInitialAvailability();
        //f2.screenings.Add(premScr);

        //PressScreening pressScr = new PressScreening { film = f3, hall = l2, time = DateTime.Parse("01.08.2022 17:30:00"), castMembersPresent = new string[] { "Маколей Калкин", "Дэниел Стерн" } };
        //pressScr.AutoPrices(l2.minPrice);
        //pressScr.SetInitialAvailability();
        //f3.screenings.Add(pressScr);

        //StandartScreening st2 = new StandartScreening { film = f4, hall = b1, time = DateTime.Parse("01.06.2022 17:30:00") };
        //st2.AutoPrices(b1.minPrice);
        //st2.SetInitialAvailability();
        //f4.screenings.Add(st2);

        //films.Add(f1);
        //films.Add(f2);
        //films.Add(f3);
        //films.Add(f4);

        //List<User> users = new List<User>();
        //string userpath = @"users.json";

        //using (var sw = new StreamWriter(userpath))
        //{
        //    using (var jsonWriter = new JsonTextWriter(sw))
        //    {
        //        jsonWriter.Formatting = Formatting.Indented;

        //        var serializer = new JsonSerializer()
        //        { TypeNameHandling = TypeNameHandling.Auto, PreserveReferencesHandling = PreserveReferencesHandling.Objects };

        //        serializer.Serialize(jsonWriter, users);
        //    }
        //}

        //string standart = @"standarthalls.json";
        //string luxe = @"luxehalls.json";
        //string black = @"blackhalls.json";

        //using (var sw = new StreamWriter(standart))
        //{
        //    using (var jsonWriter = new JsonTextWriter(sw))
        //    {
        //        jsonWriter.Formatting = Formatting.Indented;

        //        var serializer = new JsonSerializer()
        //        { TypeNameHandling = TypeNameHandling.Auto, PreserveReferencesHandling = PreserveReferencesHandling.Objects };

        //        serializer.Serialize(jsonWriter, stHalls);
        //    }
        //}

        //using (var sw = new StreamWriter(luxe))
        //{
        //    using (var jsonWriter = new JsonTextWriter(sw))
        //    {
        //        jsonWriter.Formatting = Formatting.Indented;

        //        var serializer = new JsonSerializer()
        //        { TypeNameHandling = TypeNameHandling.Auto, PreserveReferencesHandling = PreserveReferencesHandling.Objects };

        //        serializer.Serialize(jsonWriter, luHalls);
        //    }
        //}

        //using (var sw = new StreamWriter(black))
        //{
        //    using (var jsonWriter = new JsonTextWriter(sw))
        //    {
        //        jsonWriter.Formatting = Formatting.Indented;

        //        var serializer = new JsonSerializer()
        //        { TypeNameHandling = TypeNameHandling.Auto, PreserveReferencesHandling = PreserveReferencesHandling.Objects };

        //        serializer.Serialize(jsonWriter, blHalls);
        //    }
        //}

        //string filmpath = @"films.json";

        //using (var sw = new StreamWriter(filmpath))
        //{
        //    using (var jsonWriter = new JsonTextWriter(sw))
        //    {
        //        jsonWriter.Formatting = Formatting.Indented;

        //        var serializer = new JsonSerializer()
        //        { TypeNameHandling = TypeNameHandling.Auto, PreserveReferencesHandling = PreserveReferencesHandling.Objects };

        //        serializer.Serialize(jsonWriter, films);
        //    }
        //}


        string userFilename = @"users.json";
        User.ReadFile(userFilename);

        string standarthallFilename = @"standarthalls.json";
        string luxehallFilename = @"luxehalls.json";
        string blackhallFilename = @"blackhalls.json";

        StandartHall.ReadFile(standarthallFilename);
        LuxeHall.ReadFile(luxehallFilename);
        BlackHall.ReadFile(blackhallFilename);

        string filmFilename = @"C:films.json";
        Film.ReadFile(filmFilename);
    }
    static void UserInterface()
    {
        Console.WriteLine("\nВыберите опцию.");
        Console.WriteLine("1 - войти в личный кабинет пользователя;");
        Console.WriteLine("2 - зарегистрироваться;");

        User currUser = new User();
        string option = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                            .AddChoice("1")
                                                            .AddChoice("2")
                                                            .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
        if (option == "1")
        {
            string answer = "да";
            do
            {
                string name = AnsiConsole.Prompt(new TextPrompt<string>("\nВведите имя пользователя: "));
                string pwd = AnsiConsole.Prompt(new TextPrompt<string>("Введите пароль: ").Secret());

                bool foundUser = false;
                if (User.all is not null)
                {
                    foreach (User user in User.all)
                    {
                        if (user.username == name && user.password == pwd)
                        {
                            currUser = user;
                            foundUser = true;
                            break;
                        }
                    }
                }

                if (foundUser == true)
                {
                    Console.WriteLine($"\nВаш баланс составляет {currUser.balance}. Вы сможете пополнить его при покупке билетов.");
                    break;
                }

                Console.WriteLine("\nИмя пользователя и/или пароль неверны. Повторите попытку.");


                answer = AnsiConsole.Prompt(new TextPrompt<string>("Хотите продолжить?")
                                                    .AddChoice("да")
                                                    .AddChoice("нет")
                                                    .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));

                if (answer == "нет")
                    return;

            } while (answer == "да");
        }
        
        if (option == "2")
        {
            currUser.SetUsername();
            currUser.SetPassword();
            User.all.Add(currUser);
            Console.WriteLine("\nВведите баланс.");
            int initBalance = GetPositiveInt();
            currUser.SetBalance(initBalance);
        }

        while (true)
        {
            Console.WriteLine("\nВы находитесь в меню пользователя.");
            Console.WriteLine("1 - приобрести билеты;");
            Console.WriteLine("2 - показать все купленные билеты;");
            Console.WriteLine("3 - выйти из меню пользователя и вернуться к меню выбора интерфейса.");
            string command = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                            .AddChoice("1")
                                                            .AddChoice("2")
                                                            .AddChoice("3")
                                                            .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
            Console.WriteLine();

            if (command == "1")
                currUser.MakeOrder();

            else if (command == "2")
                currUser.PrintAllTickets();

            else if (command == "3")
                return;
        }

    } 
    static void ExtraAdministratorInterface()
    {
        while (true)
        {
            Console.WriteLine("\nВы находитесь в меню администратора.\n1 - посмотреть аналитику по продажам;\n2 - посмотреть загруженность залов;\n3 - посмотреть клиентскую аналитику;\n4 - изменить данные о кинотеатре;\n5 - выйти из аккаунта администратора и вернуться к меню выбора интерфейса."); ;
            string command = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                            .AddChoice("1")
                                                            .AddChoice("2")
                                                            .AddChoice("3")
                                                            .AddChoice("4")
                                                            .AddChoice("5")
                                                            .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
            if (command == "1") // аналитика по продажам
                Administrator.GetSalesStatistics();
            else if (command == "2")
                Administrator.TrackHallLoad();

            else if (command == "3") // клиентская аналитика
                Administrator.GetClientAnalytics();

            else if (command == "4") // изменение данных
                Administrator.EditCinemaData();

            else if (command == "5")
                return;
        }
    }
    public static int GetPositiveInt()
    {
        while (true)
        {
            string inputNum = AnsiConsole.Prompt(new TextPrompt<string>("> "));
            int num; bool successfullyParsed = int.TryParse(inputNum, out num);
            if (successfullyParsed && num > 0)
                return num;
            else
                Console.WriteLine("Неверное значение. Повторите попытку.");
        }
    }

    public static void ReadHallData(string standart, string luxe, string black)
    {
        using (var sr = new StreamReader(standart))
        {
            using (var jsonReader = new JsonTextReader(sr))
            {
                var serializer = new JsonSerializer()
                { TypeNameHandling = TypeNameHandling.Auto };

                StandartHall.all = serializer.Deserialize<List<StandartHall>>(jsonReader);
            }
        }

        using (var sr = new StreamReader(luxe))
        {
            using (var jsonReader = new JsonTextReader(sr))
            {
                var serializer = new JsonSerializer()
                { TypeNameHandling = TypeNameHandling.Auto };

                LuxeHall.all = serializer.Deserialize<List<LuxeHall>>(jsonReader);
            }
        }

        using (var sr = new StreamReader(black))
        {
            using (var jsonReader = new JsonTextReader(sr))
            {
                var serializer = new JsonSerializer()
                { TypeNameHandling = TypeNameHandling.Auto };

                BlackHall.all = serializer.Deserialize<List<BlackHall>>(jsonReader);
            }
        }
    }
    public static void WriteHallData(string standart, string luxe, string black)
    {
        using (var sw = new StreamWriter(standart))
        {
            using (var jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;
                var serializer = new JsonSerializer() {TypeNameHandling = TypeNameHandling.Auto };
                serializer.Serialize(jsonWriter, StandartHall.all);
            }
        }

        using (var sw = new StreamWriter(luxe))
        {
            using (var jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;
                var serializer = new JsonSerializer() { TypeNameHandling = TypeNameHandling.Auto };
                serializer.Serialize(jsonWriter, LuxeHall.all);
            }
        }

        using (var sw = new StreamWriter(black))
        {
            using (var jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;
                var serializer = new JsonSerializer() { TypeNameHandling = TypeNameHandling.Auto };
                serializer.Serialize(jsonWriter, BlackHall.all);
            }
        }
    }
    
    public static DateTime GetDateAndTime()
    {
        while (true)
        {
            Console.WriteLine("Введите дату в формате ДД.ММ.ГГГГ ЧЧ:ММ");
            Console.Write("> ");
            string dateString = Console.ReadLine();
            string format = "dd.MM.yyyy HH:mm";
            try
            {
                DateTime result = DateTime.ParseExact(dateString, format, CultureInfo.CurrentCulture);

                if (result > DateTime.Now)
                    return result;
                else
                    Console.WriteLine("Это время уже прошло. Повторите ввод.");
            }
            catch (FormatException)
            {
                Console.WriteLine("Некорректный формат. Повторите ввод.");
            }
        }
    }
    public static DateOnly GetDate()
    {
        while (true)
        {
            Console.WriteLine("Введите дату в формате ДД.ММ.ГГ");
            Console.Write("> ");
            string inputTime = Console.ReadLine();
            string format = "dd.MM.yyyy";
            CultureInfo invariant = CultureInfo.InvariantCulture;
            DateTime dt;
            if (DateTime.TryParseExact(inputTime, format, invariant, DateTimeStyles.None, out dt))
                return DateOnly.FromDateTime(dt);
            else
                Console.WriteLine("Неверный формат. Повторите попытку.");
        }
    }
    public static TimeOnly GetTime()
    {
        while (true)
        {
            Console.WriteLine("Введите время в формате ЧЧ:ММ");
            Console.Write("> ");
            string inputTime = Console.ReadLine();
            string format = "HH:mm";
            CultureInfo invariant = CultureInfo.InvariantCulture;
            DateTime dt;
            if (DateTime.TryParseExact(inputTime, format, invariant, DateTimeStyles.None, out dt))
                return TimeOnly.FromDateTime(dt);

            else
                Console.WriteLine("Неверный формат. Повторите попытку.");
        }
    }
    public static DateOnly GetRandomDate(DateOnly start, DateOnly end)
    {
        DateTime startDT = start.ToDateTime(TimeOnly.Parse("00:00:00"));
        DateTime endDT = end.ToDateTime(TimeOnly.Parse("23:59:59"));
        Random gen = new Random();
        int range = (endDT - startDT).Days;
        return DateOnly.FromDateTime(startDT.AddDays(gen.Next(range)));
    }
}