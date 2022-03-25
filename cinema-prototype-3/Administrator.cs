using cinema_prototype_3;
using Spectre.Console;
using System.Collections;
using System.Globalization;
using System.Text;


namespace cinema_prototype_3
{
    internal class Administrator
    {
        private string hardcodedPassword = "12345";

        public static void AddNewFilm()
        {
            Film newFilm;
            newFilm = new Film();
            newFilm.SetName();
            newFilm.SetAgeRestriction();
            Film.all.Add(newFilm);
        }
        public static void AddNewHall()
        {
            Hall newHall;
            newHall = new Hall();
            newHall.SetName();
            Console.WriteLine("Введите число рядов в зале:");
            int rows = Program.GetPositiveInt(); newHall.rowsNum = rows;
            Console.WriteLine("Введите число мест в одном ряду:");
            int seats = Program.GetPositiveInt(); newHall.seatsInRowNum = seats;
            newHall.SetType();
            Hall.all.Add(newHall);
        }
        public static void AddNewScreening(Film currFilm)
        {
            if (Hall.all.Count == 0)
            {
                Console.WriteLine("Ошибка: в базе нет ни одного зала.");
                return;
            }

            Console.WriteLine($"\nВыберите зал для показа фильма {currFilm.name}");

            bool validChoice = false;
            Hall chosenHall = new Hall();

            while (!validChoice)
            {
                Hall currHall = Hall.ChooseHall();

                bool isOkayToChoose = true;
                foreach (Film film in Film.all)
                {
                    foreach (Hall hall in film.halls)
                    {
                        if (hall.name == currHall.name && film.name != currFilm.name)
                        {
                            isOkayToChoose = false;
                            break;
                        }
                    }
                }

                if (isOkayToChoose)
                {
                    chosenHall = currHall;
                    validChoice = true;
                }
                else
                {
                    Console.WriteLine("Выберите другой зал, этот уже зарезервирован для другого фильма.");
                    AnsiConsole.Write(new Markup("Хотите продолжить выбор зала?\n"));
                    string answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                .AddChoice("да")
                                                .AddChoice("нет")
                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                    Console.WriteLine();
                    if (answer == "нет")
                        return;
                }
            }
            currFilm.halls.Add(chosenHall);

            Console.WriteLine($"\nВыберите время для показа фильма {currFilm.name} в зале {chosenHall.name}.");

            DateTime showDate = Program.GetDateAndTime();
            foreach (Screening screening in currFilm.screenings)
            {
                if (screening.hall == chosenHall && screening.time == showDate)
                {
                    Console.WriteLine("Данный сеанс уже есть в базе. В добавлении отказано.");
                    return;
                }
            }
            Screening newScreening = new Screening { film = currFilm, hall = chosenHall, time = showDate };
            newScreening.SetInitialAvailability();
            newScreening.SetInitialPrices();
            currFilm.screenings.Add(newScreening);
            return;
        }
        public bool Check_Password() // проверка пароля для входа в интерфейс администратора
        {
            bool isAllowed = false;
            bool isReturnNeeded = false;
            AnsiConsole.Write(new Markup("\nВведите пароль. Чтобы вернуться к меню выбора интерфейса, введите 'и'\n"));

            do
            {
                string pwd = AnsiConsole.Prompt(new TextPrompt<string>("> ").Secret());
                if (pwd == hardcodedPassword)
                    isAllowed = true;
                else if (pwd == "и")
                {
                    isReturnNeeded = true;
                    break;
                }
                else
                    AnsiConsole.Write(new Markup("Ошибка: неверный пароль. Повторите ввод.\n"));
            } while (!isAllowed);

            return isReturnNeeded;
        }
        public static void EditCinemaData()
        {
            while (true)
            {
                Console.WriteLine("\nВы находитесь в режиме изменения данных о кинотеатре.");
                Console.WriteLine("1 - добавить данные;\n2 - изменить данные;\n3 - удалить данные;\n4 - выйти из режима изменения данных и вернуться к меню администратора.");
                string command = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("2").AddChoice("3").AddChoice("4")
                                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                if (command == "1")
                {
                    string answer = "да";
                    do
                    {
                        Console.WriteLine("\nВы находитесь в режиме добавления данных о кинотеатре.");
                        Console.WriteLine("1 - добавить фильм;\n2 - добавить зал;\n3 - добавить сеанс;\n4 - выйти из режима добавления данных и вернуться к режиму изменения данных.");
                        string editCommand = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("2").AddChoice("3")
                                                                        .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                        if (editCommand == "1")
                            AddNewFilm();

                        if (editCommand == "2")
                            AddNewHall();

                        if (editCommand == "3")
                        {
                            if (Film.all.Count == 0)
                            {
                                Console.WriteLine("В базе нет фильмов. Добавить сеанс к конкретному фильму невозможно.");
                                continue;
                            }

                            Console.WriteLine("Выберите фильм.");
                            Film filmAddScreeningFor = Film.ChooseFilm("screening count not important");
                            AddNewScreening(filmAddScreeningFor);
                        }

                        if (editCommand == "4")
                            break;

                        AnsiConsole.Write(new Markup("\nХотите продолжить добавлять данные?\n"));
                        answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                    .AddChoice("да")
                                                    .AddChoice("нет")
                                                    .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                        Console.WriteLine();

                    } while (answer == "да");

                } // добавление данных

                else if (command == "2")
                {
                    string answer = "да";
                    do
                    {
                        Console.WriteLine("\nВы находитесь режиме изменения данных о кинотеатре.");
                        Console.WriteLine("1 - изменить информацию о фильме;\n2 - изменить информацию о зале;\n3 - изменить информацию о сеансе;\n4 - выйти из режима изменения данных и вернуться к меню.");
                        string editCommand = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("2").AddChoice("3").AddChoice("4")
                                                                        .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                        if (editCommand == "1")
                        {
                            if (Film.all.Count == 0)
                            {
                                Console.WriteLine("В базе нет фильмов. Изменить данные о конкретном фильме невозможно.");
                                continue;
                            }

                            Film filmToEdit = Film.ChooseFilm("screening count not important");
                            bool canBeEdited = filmToEdit.CanBeEdited();
                            if (!canBeEdited)
                            {
                                Console.WriteLine("На этот фильм уже были куплены билеты. В изменении данных отказано.");
                                continue;
                            }

                            Console.WriteLine("\nВыберите действие.");
                            Console.WriteLine("1 - изменить название;");
                            Console.WriteLine("2 - изменить возрастное ограничение.");
                            string action = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("2")
                                                                            .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));

                            if (action == "1")
                                filmToEdit.SetName();

                            if (action == "2")
                                filmToEdit.SetAgeRestriction();


                        } // изменить информацию о фильме
                        if (editCommand == "2")
                        {
                            if (Hall.all.Count == 0)
                            {
                                Console.WriteLine("В базе нет залов. Изменить данные о конкретном зале невозможно.");
                                continue;
                            }

                            Hall chosenHall = Hall.ChooseHall();
                            bool canBeEdited = chosenHall.CanBeEdited();
                            if (!canBeEdited)
                            {
                                Console.WriteLine("На сеансы в этом зале уже были куплены билеты. В изменении данных отказано.");
                                continue;
                            }

                            Console.WriteLine("\nВыберите действие.\n1 - изменить название;\n2 - изменить тип;\n");
                            string action = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("2")
                                                                            .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                            if (action == "1")
                                chosenHall.SetName();
                            if (action == "2")
                                chosenHall.SetType();

                        } // изменить информацию о зале
                        if (editCommand == "3")
                        {
                            int totalScreeningCount = 0;
                            foreach (Film film in Film.all)
                                totalScreeningCount = totalScreeningCount + film.screenings.Count;

                            if (totalScreeningCount == 0)
                            {
                                Console.WriteLine("Доступных сеансов нет.");
                                continue;
                            }

                            Console.WriteLine("\nВыберите фильм.");
                            Film chosenFilm = Film.ChooseFilm("screening count important");
                            Screening chosenScreening = chosenFilm.ChooseScreening();



                            Console.WriteLine("\nВыберите действие.\n1 - пометить места как занятые (не учитываются для аналитики, в отличие от купленных пользователями);\n2 - изменить цены;\n3 - изменить время");
                            string action = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("2").AddChoice("3")
                                                                            .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                            if (action == "1")
                                chosenScreening.MarkSeatsAsTaken();

                            if (action == "2")
                            {
                                string reply = "да";
                                do
                                {
                                    bool isReturnNeeded = chosenScreening.ChangePrices();

                                    if (isReturnNeeded)
                                    {
                                        Console.WriteLine();
                                        break;
                                    }

                                    AnsiConsole.Write(new Markup("Хотите продолжить изменение цен?\n"));
                                    reply = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                                .AddChoice("да")
                                                                .AddChoice("нет")
                                                                .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз.\n"));

                                } while (reply == "да");
                                Console.WriteLine();
                            }

                            if (action == "3")
                            {
                                bool canBeEdited = chosenScreening.CanTimeBeEdited();
                                if (!canBeEdited)
                                {
                                    Console.WriteLine("На этот сеанс уже были куплены билеты. В изменении времени показа отказано.");
                                    continue;
                                }
                                chosenScreening.time = Program.GetDateAndTime();
                            }

                        } // изменить информацию о сеансе
                        if (editCommand == "4")
                            break;

                        AnsiConsole.Write(new Markup("\nХотите продолжить изменять данные?\n"));
                        answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                    .AddChoice("да")
                                                    .AddChoice("нет")
                                                    .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                        Console.WriteLine();

                    } while (answer == "да");

                } // изменение данных

                else if (command == "3")
                {
                    string answer = "да";

                    do
                    {
                        Console.WriteLine("\nВы находитесь режиме удаления данных.");
                        Console.WriteLine("1 - удалить фильм;\n2 - удалить зал;\n3 - удалить сеанс;\n4 - выйти из режима удаления данных и вернуться к режиму изменения данных.");
                        string deleteCommand = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("2").AddChoice("3").AddChoice("4")
                                                                        .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));

                        if (deleteCommand == "1")
                        {
                            if (Film.all.Count == 0)
                            {
                                Console.WriteLine("В базе и так нет фильмов.");
                                continue;
                            }

                            Film filmToDelete = Film.ChooseFilm("screening count not important");
                            bool canBeDeleted = filmToDelete.CanBeEdited();
                            if (!canBeDeleted)
                            {
                                Console.WriteLine("На этот фильм уже были куплены билеты. В удалении отказано.");
                                continue;
                            }

                            Film.all.Remove(filmToDelete);

                        } // удалить фильм

                        if (deleteCommand == "2")
                        {
                            if (Hall.all.Count == 0)
                            {
                                Console.WriteLine("В базе и так нет залов.");
                                continue;
                            }

                            Hall chosenHall = Hall.ChooseHall();
                            bool canBeDeleted = chosenHall.CanBeEdited();
                            if (!canBeDeleted)
                            {
                                Console.WriteLine("На сеансы в этом зале уже были куплены билеты. В удалении отказано.");
                                continue;
                            }

                            Hall.all.Remove(chosenHall);

                        } //удалить зал

                        if (deleteCommand == "3")
                        {
                            int totalScreeningCount = 0;
                            foreach (Film film in Film.all)
                                totalScreeningCount = totalScreeningCount + film.screenings.Count;

                            if (totalScreeningCount == 0)
                            {
                                Console.WriteLine("В базе и так нет сеансов.");
                                continue;
                            }

                            Console.WriteLine("\nВыберите фильм");
                            Film chosenFilm = Film.ChooseFilm("screening count important");
                            Screening chosenScreening = chosenFilm.ChooseScreening();

                            bool canBeDeleted = chosenScreening.CanTimeBeEdited();
                            if (!canBeDeleted)
                            {
                                Console.WriteLine("На этот сеанс уже были куплены билеты. В удалении отказано.");
                                continue;
                            }

                            chosenFilm.screenings.Remove(chosenScreening);

                        } // удалить сеанс

                        if (deleteCommand == "4")
                            break;

                        AnsiConsole.Write(new Markup("\nХотите продолжить удаление данных?\n"));
                        answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                    .AddChoice("да")
                                                    .AddChoice("нет")
                                                    .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                        Console.WriteLine();

                    } while (answer == "да");
                } // удаление данных

                else if (command == "4")
                    return;
            }
        }
        public static void GetClientAnalytics()
        {
            while (true)
            {
                Console.WriteLine("\nВы находитесь в режиме просмотра клиентской аналитики.");
                Console.WriteLine("1 - вывести информацию о клиентах, купивших наибольшее количество билетов;");
                Console.WriteLine("2 - вывести информацию о клиентах, купивших билеты на наибольшее количество разных сеансов;");
                Console.WriteLine("3 - вывести информацию о клиентах, потративших наибольшую сумму денег;");
                Console.WriteLine("4 - выйти из режима просмотра клиентской аналитики и вернуться к меню администратора.");
                string command = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("2").AddChoice("3").AddChoice("4")
                                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                if (command == "1")
                {
                    Console.WriteLine("Введите, сколько клиентов вас интересует (топ-1, топ-3 или топ-5)");
                    string topNum = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("3").AddChoice("5")
                                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                    User.GreatestTicketNum(int.Parse(topNum));
                } // клиенты, купившие наибольшее количество билетов

                else if (command == "2")
                {
                    Console.WriteLine("Введите, сколько клиентов вас интересует (топ-1, топ-3 или топ-5)");
                    string topNum = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("3").AddChoice("5")
                                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                    User.GreatestDistinctScreeningTicketNum(int.Parse(topNum));
                } // клиенты, купившие билеты на наибольшее количество разных сеансов

                else if (command == "3")
                {
                    Console.WriteLine("Введите, сколько клиентов вас интересует (топ-1, топ-3 или топ-5)");
                    string topNum = AnsiConsole.Prompt(new TextPrompt<string>("").AddChoice("1").AddChoice("3").AddChoice("5")
                                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                    User.GreatestMoneyAmountSpent(int.Parse(topNum));
                } // клиенты, потратившие наибольшую сумму денег

                else if (command == "4")
                    return;
            }
        }
        public static void GetSalesStatistics()
        {
            while (true)
            {
                Console.WriteLine("\nВы находитесь в режиме просмотра аналитики по продажам.");
                Console.WriteLine("1 - перейти к просмотру аналитики;");
                Console.WriteLine("2 - выйти из режима просмотра аналитики по продажам и вернуться к меню администратора.");
                string command = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                                .AddChoice("1")
                                                                .AddChoice("2")
                                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                if (command == "2")
                    break;

                var filters = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("Хотите выбрать фильтры?")
                    .NotRequired()
                    .PageSize(10)
                    .MoreChoicesText("[grey](Двигайтесь вверх и вниз, чтобы посмотреть доступные фильтры)[/]")
                    .InstructionsText(
                        "[grey](Нажмите [blue]<пробел>[/] для выбора фильтра, " +
                        "[green]<enter>[/] для завершения выбора)[/]")
                    .AddChoices(new[] {
                        "Конкретный фильм", "Конкретный зал", "Конкретный сеанс",
                        "Конкретный промежуток времени (дни)", "Период дня (часы)", "Конкретный возрастной рейтинг"
                    }));

                List<Ticket> allTickets = new List<Ticket>();
                foreach (User user in User.all)
                    allTickets.AddRange(user.orders);

                if (filters.Any(filter => filter == "Конкретный фильм"))
                {
                    if (Film.all.Count == 0)
                    {
                        Console.WriteLine("В базе нет ни одного фильма для выбора");
                        continue;
                    }

                    Console.WriteLine("\nВыберите фильм");
                    Film filmChosen = Film.ChooseFilm("screening count not important");
                    allTickets = allTickets.Where(ticket => ticket.screening.film.name == filmChosen.name).ToList();
                }

                if (filters.Any(filter => filter == "Конкретный зал"))
                {
                    if (Hall.all.Count == 0)
                    {
                        Console.WriteLine("В базе нет ни одного зала для выбора");
                        continue;
                    }

                    Console.WriteLine("\nВыберите зал.");
                    Hall hallChosen = Hall.ChooseHall();
                    allTickets = allTickets.Where(ticket => ticket.screening.hall.name == hallChosen.name).ToList();
                }

                if (filters.Any(filter => filter == "Конкретный сеанс"))
                {
                    List<Screening> allScreenings = new List<Screening>();
                    foreach (Film film in Film.all)
                        allScreenings.AddRange(film.screenings);
                    allScreenings.Sort((x, y) => x.time.CompareTo(y.time));

                    if (allScreenings.Count == 0)
                    {
                        Console.WriteLine("В базе нет ни одного сеанса для выбора");
                        continue;
                    }

                    TextPrompt<string> scrChoicePrompt = new TextPrompt<string>("").InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз.");

                    for (int i = 0; i < allScreenings.Count; i++)
                    {
                        Console.WriteLine($"{i + 1,4}: {allScreenings[i].film.name,20} {allScreenings[i].hall.name,10} {allScreenings[i].time.ToString("dd/MM/yyyy HH:mm"),16}");
                        scrChoicePrompt.AddChoice(Convert.ToString(i + 1));
                    }

                    Console.WriteLine("\nВведите номер одного выбранного сеанса:");
                    int scrNum = int.Parse(AnsiConsole.Prompt(scrChoicePrompt)) - 1;
                    Screening screeningChosen = allScreenings[scrNum];

                    allTickets = allTickets.Where(ticket => ticket.screening.film.name == screeningChosen.film.name && ticket.screening.hall.name == screeningChosen.hall.name && ticket.screening.time == screeningChosen.time).ToList();
                }

                if (filters.Any(filter => filter == "Конкретный промежуток времени (дни)"))
                {
                    while (true)
                    {
                        Console.WriteLine("\nВведите начальную дату.");
                        DateOnly startDay = Program.GetDate();
                        Console.WriteLine("Введите конечную дату.");
                        DateOnly endDay = Program.GetDate();
                        if (startDay > endDay)
                            Console.WriteLine("Начальная дата не может быть позднее конечной. Повторите ввод.");
                        else
                        {
                            DateTime start = startDay.ToDateTime(TimeOnly.Parse("00:00:00"));
                            DateTime end = endDay.ToDateTime(TimeOnly.Parse("23:59:59"));
                            allTickets = allTickets.Where(ticket => ticket.timeBought >= start && ticket.timeBought <= end).ToList();
                            break;
                        }
                    } // получаем даты
                }

                if (filters.Any(filter => filter == "Период дня (часы)"))
                {
                    while (true)
                    {
                        Console.WriteLine("\nВведите начальное время.");
                        TimeOnly startTime = Program.GetTime();
                        Console.WriteLine("Введите конечное время.");
                        TimeOnly endTime = Program.GetTime();
                        if (startTime > endTime)
                            Console.WriteLine("Начальная дата не может быть позднее конечной. Повторите ввод.");
                        else
                        {
                            if (allTickets.Count == 0)
                                break;

                            DateOnly randomDate = Program.GetRandomDate(DateOnly.FromDateTime(allTickets[0].timeBought), DateOnly.FromDateTime(allTickets[-1].timeBought));
                            DateTime start = randomDate.ToDateTime(startTime);
                            DateTime end = randomDate.ToDateTime(endTime);
                            allTickets = allTickets.Where(ticket => ticket.timeBought >= start && ticket.timeBought <= end).ToList();
                            break;
                        }
                    }
                }

                if (filters.Any(filter => filter == "Конкретный возрастной рейтинг"))
                {
                    Console.WriteLine("\nВведите возрастной рейтинг.");
                    string chosenAgeRest = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                        .AddChoice("0+")
                                                        .AddChoice("6+")
                                                        .AddChoice("12+")
                                                        .AddChoice("16+")
                                                        .AddChoice("18+")
                                                        .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                    allTickets = allTickets.Where(ticket => ticket.screening.film.ageRestriction == chosenAgeRest).ToList();
                }

                int revenue = 0;

                foreach (Ticket ticket in allTickets)
                    revenue = revenue + ticket.price;

                Console.WriteLine($"\nВыручка составляет {revenue} рублей.");

            }
        }
        public static void TrackHallLoad()
        {
            while (true)
            {
                Console.WriteLine("\nВы находитесь в режиме просмотра загруженности залов.");
                Console.WriteLine("1 - перейти к просмотру аналитики;");
                Console.WriteLine("2 - выйти из режима просмотра загруженности залов и вернуться к меню администратора.");
                string command = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                                .AddChoice("1")
                                                                .AddChoice("2")
                                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                if (command == "2")
                    break;

                var filters = AnsiConsole.Prompt(
                   new MultiSelectionPrompt<string>()
                       .Title("Хотите выбрать фильтры?")
                       .NotRequired()
                       .PageSize(10)
                       .MoreChoicesText("[grey](Двигайтесь вверх и вниз, чтобы посмотреть доступные фильтры)[/]")
                       .InstructionsText(
                           "[grey](Нажмите [blue]<пробел>[/] для выбора фильтра, " +
                           "[green]<enter>[/] для завершения выбора)[/]")
                       .AddChoices(new[] {
                            "Конкретный фильм", "Конкретный зал", "Конкретный сеанс",
                            "Конкретный промежуток времени (дни)", "Период дня (часы)", "Конкретный возрастной рейтинг"
                       }));

                List<Screening> allScreenings = new List<Screening>();
                foreach (Film film in Film.all)
                    allScreenings.AddRange(film.screenings);

                if (filters.Any(filter => filter == "Конкретный фильм"))
                {
                    if (Film.all.Count == 0)
                    {
                        Console.WriteLine("\nВыберите фильм");
                        Console.WriteLine("В базе нет ни одного фильма для выбора");
                        continue;
                    }

                    Film filmChosen = Film.ChooseFilm("screening count not important");
                    allScreenings = allScreenings.Where(screening => screening.film.name == filmChosen.name).ToList();
                }

                if (filters.Any(filter => filter == "Конкретный зал"))
                {
                    if (Hall.all.Count == 0)
                    {
                        Console.WriteLine("В базе нет ни одного зала для выбора");
                        continue;
                    }

                    Console.WriteLine("\nВыберите зал.");
                    Hall hallChosen = Hall.ChooseHall();
                    allScreenings = allScreenings.Where(screening => screening.hall.name == hallChosen.name).ToList();
                }

                if (filters.Any(filter => filter == "Конкретный сеанс"))
                {
                    List<Screening> screeningsToChooseFrom = new List<Screening>();
                    foreach (Film film in Film.all)
                        screeningsToChooseFrom.AddRange(film.screenings);
                    screeningsToChooseFrom.Sort((x, y) => x.time.CompareTo(y.time));

                    if (screeningsToChooseFrom.Count == 0)
                    {
                        Console.WriteLine("В базе нет ни одного сеанса для выбора");
                        continue;
                    }

                    TextPrompt<string> scrChoicePrompt = new TextPrompt<string>("").InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз.");

                    for (int i = 0; i < screeningsToChooseFrom.Count; i++)
                    {
                        Console.WriteLine($"{i + 1,4}: {screeningsToChooseFrom[i].film.name,20} {screeningsToChooseFrom[i].hall.name,10} {screeningsToChooseFrom[i].time.ToString("dd/MM/yyyy HH:mm"),16}");
                        scrChoicePrompt.AddChoice(Convert.ToString(i + 1));
                    }

                    Console.WriteLine("\nВведите номер одного выбранного сеанса:");
                    int scrNum = int.Parse(AnsiConsole.Prompt(scrChoicePrompt)) - 1;
                    Screening screeningChosen = screeningsToChooseFrom[scrNum];

                    allScreenings = allScreenings.Where(screening => screening.film.name == screeningChosen.film.name && screening.hall.name == screeningChosen.hall.name && screening.time == screeningChosen.time).ToList();
                }

                if (filters.Any(filter => filter == "Конкретный промежуток времени (дни)"))
                {
                    while (true)
                    {
                        Console.WriteLine("\nВведите начальную дату.");
                        DateOnly startDay = Program.GetDate();
                        Console.WriteLine("Введите конечную дату.");
                        DateOnly endDay = Program.GetDate();
                        if (startDay > endDay)
                            Console.WriteLine("Начальная дата не может быть позднее конечной. Повторите ввод.");
                        else
                        {
                            DateTime start = startDay.ToDateTime(TimeOnly.Parse("00:00:00"));
                            DateTime end = endDay.ToDateTime(TimeOnly.Parse("23:59:59"));
                            allScreenings = allScreenings.Where(screening => screening.time >= start && screening.time <= end).ToList();
                            break;
                        }
                    } // получаем даты
                }

                if (filters.Any(filter => filter == "Период дня (часы)"))
                {
                    while (true)
                    {
                        Console.WriteLine("\nВведите начальное время.");
                        TimeOnly startTime = Program.GetTime();
                        Console.WriteLine("Введите конечное время.");
                        TimeOnly endTime = Program.GetTime();
                        if (startTime > endTime)
                            Console.WriteLine("Начальная дата не может быть позднее конечной. Повторите ввод.");
                        else
                        {
                            if (allScreenings.Count == 0)
                                break;

                            DateOnly randomDate = Program.GetRandomDate(DateOnly.FromDateTime(allScreenings[0].time), DateOnly.FromDateTime(allScreenings[-1].time));
                            DateTime start = randomDate.ToDateTime(startTime);
                            DateTime end = randomDate.ToDateTime(endTime);
                            allScreenings = allScreenings.Where(screening => screening.time >= start && screening.time <= end).ToList();
                            break;
                        }
                    }
                }

                if (filters.Any(filter => filter == "Конкретный возрастной рейтинг"))
                {
                    Console.WriteLine("\nВведите возрастной рейтинг.");
                    string chosenAgeRest = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                        .AddChoice("0+")
                                                        .AddChoice("6+")
                                                        .AddChoice("12+")
                                                        .AddChoice("16+")
                                                        .AddChoice("18+")
                                                        .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                    allScreenings = allScreenings.Where(screening => screening.film.ageRestriction == chosenAgeRest).ToList();
                }

                int seatsSold = 0;
                int seatsEmpty = 0;

                foreach (Screening screening in allScreenings)
                {
                    for (int i = 0; i < screening.hall.rowsNum; i++)
                    {
                        for (int j = 0; j < screening.hall.seatsInRowNum; j++)
                        {
                            if (screening.seatsAvailability[i][j] == '0')
                                seatsEmpty++;
                            else
                                seatsSold++;
                        }
                    }
                }

                Console.WriteLine($"\nПродано {seatsSold} мест.");
                Console.WriteLine($"Свободно {seatsEmpty} мест.");
            }
        }
    }
}
