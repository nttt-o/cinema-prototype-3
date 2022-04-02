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
        } // все ок
        public static void AddNewHall()
        {
            Console.WriteLine("Ознакомьтесь с типами залов.\n");
            StandartHall.PrintHallTypeCharacteristics(); Console.WriteLine();
            LuxeHall.PrintHallTypeCharacteristics(); Console.WriteLine();
            BlackHall.PrintHallTypeCharacteristics(); Console.WriteLine();

            Console.WriteLine($"Выберите тип зала: 1 - {new StandartHall().GetHallType()}, 2 - {new LuxeHall().GetHallType()}, 3 - {new BlackHall().GetHallType()}");
            string answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                        .AddChoice("1")
                                        .AddChoice("2")
                                        .AddChoice("3")
                                        .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));

            if (answer == "1")
            {
                StandartHall newHall = new StandartHall();
                newHall.SetInitialHallData();
            }
            if (answer == "2")
            {
                LuxeHall newHall = new LuxeHall();
                newHall.SetInitialHallData();
            }
            if (answer == "3")
            {
                BlackHall newHall = new BlackHall();
                newHall.SetInitialHallData();
            }


        } // все ок

        public static string ChooseHallType()
        {
            Console.WriteLine("\nОзнакомьтесь с типами залов.\n");
            StandartHall.PrintHallTypeCharacteristics(); Console.WriteLine();
            LuxeHall.PrintHallTypeCharacteristics(); Console.WriteLine();
            BlackHall.PrintHallTypeCharacteristics(); Console.WriteLine();

            TextPrompt<string> hallChoicePrompt = new TextPrompt<string>("");

            if (StandartHall.all.Count == 0)
                Console.WriteLine($"Залов типа 1 - {new StandartHall().GetHallType()} в базе нет.");
            else
                hallChoicePrompt.AddChoice("1");

            if (LuxeHall.all.Count == 0)
                Console.WriteLine($"Залов типа 2 - {new LuxeHall().GetHallType()} в базе нет.");
            else
                hallChoicePrompt.AddChoice("2");

            if (BlackHall.all.Count == 0)
                Console.WriteLine($"Залов типа 3 - {new BlackHall().GetHallType()} в базе нет.");
            else
                hallChoicePrompt.AddChoice("3");

            Console.WriteLine($"Выберите тип зала: 1 - {new StandartHall().GetHallType()}, 2 - {new LuxeHall().GetHallType()}, 3 - {new BlackHall().GetHallType()}");
            string hallTypeChosen = AnsiConsole.Prompt(hallChoicePrompt.InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));

            return hallTypeChosen;

        }

        public static void AddNewScreening(Film currFilm)
        {            
            if (StandartHall.all.Count + LuxeHall.all.Count + BlackHall.all.Count == 0)
            {
                Console.WriteLine("Ошибка: в базе нет ни одного зала.");
                return;
            }

            Console.WriteLine("Ознакомьтесь с типами сеансов.\n");
            StandartScreening.PrintScreeningTypeCharacteristics(); Console.WriteLine();
            PremiereScreening.PrintScreeningTypeCharacteristics(); Console.WriteLine();
            PressScreening.PrintScreeningTypeCharacteristics(); Console.WriteLine();

            Console.WriteLine($"Выберите тип показа: 1 - {StandartScreening.type} , 2 - {PremiereScreening.type}, 3 - {PressScreening.type}");
            string screeningTypeChosen = AnsiConsole.Prompt(new TextPrompt<string>("")
                                        .AddChoice("1")
                                        .AddChoice("2")
                                        .AddChoice("3")
                                        .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));

            string hallTypeChosen = ChooseHallType();

            Console.WriteLine($"\nВыберите зал для показа фильма {currFilm.name}");
            bool validChoice = false;
            Hall chosenHall = null;

            while (!validChoice)
            {
                Hall currHall;
                if (hallTypeChosen == "1")
                    currHall = StandartHall.ChooseHall();
                else if (hallTypeChosen == "2")
                    currHall = LuxeHall.ChooseHall();
                else
                    currHall = BlackHall.ChooseHall();

                bool isOkayToChoose = true;
                foreach (Film film in Film.all)
                {
                    foreach (var hall in film.halls)
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
                    string choice = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                .AddChoice("да")
                                                .AddChoice("нет")
                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
                    Console.WriteLine();
                    if (choice == "нет")
                        return;
                }
            }
            
            currFilm.halls.Add(chosenHall);

            Console.WriteLine($"\nВыберите время для показа фильма {currFilm.name} в зале {chosenHall.name}.");

            DateTime showDate = Program.GetDateAndTime();
            foreach (Screening screening in currFilm.screenings)
            {
                if (screening.hall.name == chosenHall.name && screening.time == showDate)
                {
                    Console.WriteLine("Данный сеанс уже есть в базе. В добавлении отказано.");
                    return;
                }
            }

            Screening newScreening = null;
            if (screeningTypeChosen == "1")
            {
                StandartScreening scr = new StandartScreening { film = currFilm, hall = chosenHall, time = showDate };
                newScreening = scr;
            }
            if (screeningTypeChosen == "2")
            {
                PremiereScreening scr = new PremiereScreening { film = currFilm, hall = chosenHall, time = showDate };
                Console.WriteLine();
                scr.SetCriticInvited();
                newScreening = scr;
            }
            if (screeningTypeChosen == "3")
            {
                PressScreening scr = new PressScreening { film = currFilm, hall = chosenHall, time = showDate };
                Console.WriteLine();
                scr.SetCastMembersPresent();
                newScreening = scr;
            }
            newScreening.SetInitialAvailability();
            newScreening.SetInitialPrices();
            currFilm.screenings.Add(newScreening);
            return;
        } // все ок 
        public bool Check_Password() 
        {
            bool isAllowed = false;
            bool isReturnNeeded = false;
            AnsiConsole.Write(new Markup("\nВведите пароль (12345). Чтобы вернуться к меню выбора интерфейса, введите 'и'\n"));

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
        } // все ок
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
                            Film filmAddScreeningFor = Film.ChooseFilm("screening count not important").Item1;
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

                            Tuple<Film, int> choiceResult = Film.ChooseFilm("screening count not important");
                            int foundCode = choiceResult.Item2;
                            if (foundCode == -1)
                            {
                                Console.WriteLine("Изменение данных невозможно.\n");
                                continue;
                            }

                            Film filmToEdit = choiceResult.Item1;
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
                            if (StandartHall.all.Count + LuxeHall.all.Count + BlackHall.all.Count == 0)
                            {
                                Console.WriteLine("В базе нет залов. Изменить данные о конкретном зале невозможно.");
                                continue;
                            }

                            string hallTypeChosen = ChooseHallType();

                            if (hallTypeChosen == "1")
                            {
                                if (StandartHall.all.Count == 0)
                                {
                                    Console.WriteLine("В базе нет залов данного типа.");
                                    continue;
                                }

                                StandartHall chosenHall = StandartHall.ChooseHall();
                                chosenHall.EditData();
                            }
                            else if (hallTypeChosen == "2")
                            {
                                if (LuxeHall.all.Count == 0)
                                {
                                    Console.WriteLine("В базе нет залов данного типа.");
                                    continue;
                                }

                                LuxeHall chosenHall = LuxeHall.ChooseHall();
                                chosenHall.EditData();
                            }
                            else
                            {
                                if (BlackHall.all.Count == 0)
                                {
                                    Console.WriteLine("В базе нет залов данного типа.");
                                    continue;
                                }

                                BlackHall chosenHall = BlackHall.ChooseHall();
                                chosenHall.EditData();
                            }

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
                            Tuple<Film, int> choiceResult = Film.ChooseFilm("screening count important");
                            int foundCode = choiceResult.Item2;
                            if (foundCode == -1)
                            {
                                Console.WriteLine("Изменение данных невозможно.\n");
                                continue;
                            }
                            Screening chosenScreening = choiceResult.Item1.ChooseScreening("administrator");

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

                        } // изменить информацию о сеансе - все ок
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

                            Tuple<Film, int> choiceResult = Film.ChooseFilm("screening count not important");
                            int foundCode = choiceResult.Item2;
                            if (foundCode == -1)
                            {
                                Console.WriteLine("Удаление данных невозможно.\n");
                                continue;
                            }
                            Film filmToDelete = choiceResult.Item1;

                            bool canBeDeleted = filmToDelete.CanBeEdited();
                            if (!canBeDeleted)
                            {
                                Console.WriteLine("На этот фильм уже были куплены билеты. В удалении отказано.");
                                continue;
                            }

                            Film.all.Remove(filmToDelete);

                        } // удалить фильм - все ок

                        if (deleteCommand == "2")
                        {
                            if (StandartHall.all.Count + LuxeHall.all.Count + BlackHall.all.Count == 0)
                            {
                                Console.WriteLine("В базе нет залов. Удалить данные о конкретном зале невозможно.");
                                continue;
                            }

                            string hallTypeChosen = ChooseHallType();

                            if (hallTypeChosen == "1")
                            {
                                if (StandartHall.all.Count == 0)
                                {
                                    Console.WriteLine("В базе нет залов данного типа.");
                                    continue;
                                }

                                StandartHall chosenHall = StandartHall.ChooseHall();
                                bool canBeDeleted = chosenHall.CanBeEdited();
                                if (!canBeDeleted)
                                {
                                    Console.WriteLine("На сеансы в этом зале уже были куплены билеты. В удалении отказано.");
                                    continue;
                                }

                                StandartHall.all.Remove(chosenHall);
                                foreach (Film film in Film.all)
                                    film.screenings.RemoveAll(scr => scr.hall.name == chosenHall.name);
                            }
                            else if (hallTypeChosen == "2")
                            {
                                if (LuxeHall.all.Count == 0)
                                {
                                    Console.WriteLine("В базе нет залов данного типа.");
                                    continue;
                                }

                                LuxeHall chosenHall = LuxeHall.ChooseHall();
                                bool canBeDeleted = chosenHall.CanBeEdited();
                                if (!canBeDeleted)
                                {
                                    Console.WriteLine("На сеансы в этом зале уже были куплены билеты. В удалении отказано.");
                                    continue;
                                }

                                LuxeHall.all.Remove(chosenHall);
                                foreach (Film film in Film.all)
                                    film.screenings.RemoveAll(scr => scr.hall.name == chosenHall.name);
                            }
                            else
                            {
                                if (BlackHall.all.Count == 0)
                                {
                                    Console.WriteLine("В базе нет залов данного типа.");
                                    continue;
                                }

                                BlackHall chosenHall = BlackHall.ChooseHall();
                                bool canBeDeleted = chosenHall.CanBeEdited();
                                if (!canBeDeleted)
                                {
                                    Console.WriteLine("На сеансы в этом зале уже были куплены билеты. В удалении отказано.");
                                    continue;
                                }

                                BlackHall.all.Remove(chosenHall);
                                foreach (Film film in Film.all)
                                    film.screenings.RemoveAll(scr => scr.hall.name == chosenHall.name);
                            }

                        } //удалить зал - все ок

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
                            Tuple<Film, int> choiceResult = Film.ChooseFilm("screening count important");
                            int foundCode = choiceResult.Item2;
                            if (foundCode == -1)
                            {
                                Console.WriteLine("Удаление данных невозможно.\n");
                                continue;
                            }

                            Film chosenFilm = choiceResult.Item1;
                            Screening chosenScreening = chosenFilm.ChooseScreening("administrator");

                            bool canBeDeleted = chosenScreening.CanTimeBeEdited();
                            if (!canBeDeleted)
                            {
                                Console.WriteLine("На этот сеанс уже были куплены билеты. В удалении отказано.");
                                continue;
                            }

                            chosenFilm.screenings.Remove(chosenScreening);

                        } // удалить сеанс - все ок

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
        } // все ок
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
                    Tuple<Film, int> choiceResult = Film.ChooseFilm("screening count not important");
                    int foundCode = choiceResult.Item2;
                    if (foundCode == -1)
                    {
                        Console.WriteLine("Изменение данных невозможно.\n");
                        continue;
                    }
                    Film filmChosen = choiceResult.Item1;
                    allTickets = allTickets.Where(ticket => ticket.screening.film.name == filmChosen.name).ToList();
                }

                if (filters.Any(filter => filter == "Конкретный зал"))
                {
                    if (StandartHall.all.Count + LuxeHall.all.Count + BlackHall.all.Count == 0)
                        Console.WriteLine("В базе нет ни одного зала для выбора");
                    else
                    {
                        Console.WriteLine("Выберите зал.");
                        string hallTypeChosen = ChooseHallType();
                        Hall chosenHall = null;

                        if (hallTypeChosen == "1")
                        {
                            if (StandartHall.all.Count == 0)
                                Console.WriteLine("В базе нет залов данного типа.");
                            else
                                chosenHall = StandartHall.ChooseHall();
                        }
                        else if (hallTypeChosen == "2")
                        {
                            if (LuxeHall.all.Count == 0)
                                Console.WriteLine("В базе нет залов данного типа.");
                            else
                                chosenHall = LuxeHall.ChooseHall();
                        }
                        else
                        {
                            if (BlackHall.all.Count == 0)
                                Console.WriteLine("В базе нет залов данного типа.");
                            else
                                chosenHall = BlackHall.ChooseHall();
                        }

                        if (chosenHall is not null)
                            allTickets = allTickets.Where(ticket => ticket.screening.hall.name == chosenHall.name).ToList();
                    }
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
                    }
                    else
                    {
                        Console.WriteLine("\nВведите номер одного выбранного сеанса:");
                        TextPrompt<string> scrChoicePrompt = new TextPrompt<string>("").InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз.");

                        for (int i = 0; i < screeningsToChooseFrom.Count; i++)
                        {
                            screeningsToChooseFrom[i].Print(i);
                            scrChoicePrompt.AddChoice(Convert.ToString(i + 1));
                        }

                        int scrNum = int.Parse(AnsiConsole.Prompt(scrChoicePrompt)) - 1;
                        Screening screeningChosen = screeningsToChooseFrom[scrNum];

                        allTickets = allTickets.Where(ticket => ticket.screening.film.name == screeningChosen.film.name && ticket.screening.hall.name == screeningChosen.hall.name && ticket.screening.time == screeningChosen.time).ToList();
                    }
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
                    Console.WriteLine("\nВыберите фильм.");
                    if (Film.all.Count == 0)
                    {
                        Console.WriteLine("В базе нет ни одного фильма для выбора.");
                        continue;
                    }

                    Tuple<Film, int> choiceResult = Film.ChooseFilm("screening count not important");
                    int foundCode = choiceResult.Item2;
                    if (foundCode == -1)
                        Console.WriteLine("Выбор невозможен.\n");
                    else
                    {
                        Film filmChosen = choiceResult.Item1;
                        allScreenings = allScreenings.Where(screening => screening.film.name == filmChosen.name).ToList();
                    }
                    
                }

                if (filters.Any(filter => filter == "Конкретный зал"))
                {
                    Console.WriteLine("Выберите зал.");
                    if (StandartHall.all.Count + LuxeHall.all.Count + BlackHall.all.Count == 0)
                        Console.WriteLine("В базе нет ни одного зала для выбора");
                    else
                    {
                        string hallTypeChosen = ChooseHallType();
                        Hall chosenHall = null;

                        if (hallTypeChosen == "1")
                        {
                            if (StandartHall.all.Count == 0)
                                Console.WriteLine("В базе нет залов данного типа.");
                            else
                                chosenHall = StandartHall.ChooseHall();
                        }
                        else if (hallTypeChosen == "2")
                        {
                            if (LuxeHall.all.Count == 0)
                                Console.WriteLine("В базе нет залов данного типа.");
                            else
                                chosenHall = LuxeHall.ChooseHall();
                        }
                        else
                        {
                            if (BlackHall.all.Count == 0)
                                Console.WriteLine("В базе нет залов данного типа.");
                            else
                                chosenHall = BlackHall.ChooseHall();
                        }
                        
                        if (chosenHall is not null)
                            allScreenings = allScreenings.Where(screening => screening.hall.name == chosenHall.name).ToList();
                    }
                    
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
                    }
                    else
                    {
                        TextPrompt<string> scrChoicePrompt = new TextPrompt<string>("").InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз.");

                        for (int i = 0; i < screeningsToChooseFrom.Count; i++)
                        {
                            screeningsToChooseFrom[i].Print(i);
                            scrChoicePrompt.AddChoice(Convert.ToString(i + 1));
                        }

                        Console.WriteLine("\nВведите номер одного выбранного сеанса:");
                        int scrNum = int.Parse(AnsiConsole.Prompt(scrChoicePrompt)) - 1;
                        Screening screeningChosen = screeningsToChooseFrom[scrNum];

                        allScreenings = allScreenings.Where(screening => screening.film.name == screeningChosen.film.name && screening.hall.name == screeningChosen.hall.name && screening.time == screeningChosen.time).ToList();
                    }
                   
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
