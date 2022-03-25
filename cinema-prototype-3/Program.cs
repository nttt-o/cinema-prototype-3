using cinema_prototype_3;
using Spectre.Console;
using System.Collections;
using System.Globalization;
using System.Text;

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
                string hallFilename = @"halls.csv";
                Hall.UpdateFile(hallFilename);

                string filmFilename = @"films.csv";
                Film.UpdateFile(filmFilename);

                string screeningFilename = @"screenings.csv";
                Screening.UpdateFile(screeningFilename);
                break;
            }
        }
        Console.ReadKey();
    }

    static void StartAdministratorInterface()
    {
        string hallFilename = @"C:halls.csv";
        Hall.ReadFile(hallFilename);

        string filmFilename = @"C:films.csv";
        Film.ReadFile(filmFilename);

        string screeningFilename = @"screenings.csv";
        Screening.ReadFile(screeningFilename);
        foreach (Film film in Film.all)
        {
            foreach (Screening screening in film.screenings)
                screening.SetInitialAvailability();
        }

    }
    static void UserInterface()
    {

        User currUser = new User();
        User.all.Add(currUser);
        currUser.SetUsername();

        Console.WriteLine("Введите начальный баланс.");
        int initBalance = GetPositiveInt();
        currUser.balance = initBalance;

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
    public static DateTime GetDateAndTime()
    {
        while (true)
        {
            Console.WriteLine("Введите дату в формате ДД.ММ.ГГГГ ЧЧ:ММ");
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