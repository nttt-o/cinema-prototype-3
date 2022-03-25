using cinema_prototype_3;
using Spectre.Console;
using System.Collections;
using System.Globalization;
using System.Text;

namespace cinema_prototype_3
{
    internal class User
    {
        public static List<User> all = new List<User>();

        public int balance;
        public string username;
        public List<Ticket> orders = new List<Ticket>();

        public void SetUsername()
        {
            bool success = false;
            Console.WriteLine("\nВведите свое уникальное имя пользователя:");
            while (!success)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                if (input.Length >= 1)
                {
                    bool alreadyTaken = false;
                    foreach (User existingUser in User.all)
                    {
                        if (input == existingUser.username)
                        {
                            alreadyTaken = true;
                            break;
                        }
                    }

                    if (alreadyTaken)
                        Console.WriteLine("Данное имя уже занято.");

                    else
                    {
                        username = input;
                        success = true;
                    }
                }
                else
                    Console.WriteLine("Повторите ввод.");
            }
        }
        public void UpdateBalance()
        {
            Console.WriteLine("Введите сумму, на которую хотите пополнить баланс.");
            int toAdd = Program.GetPositiveInt();
            balance = balance + toAdd;
            Console.WriteLine("Пополнение прошло успешно!");
        }
        public Dictionary<Screening, List<List<int>>> ReadOneScreeningOrder(List<Ticket> alreadyReserved)
        {
            Dictionary<Screening, List<List<int>>> currOrder = new Dictionary<Screening, List<List<int>>>();
            string answer = "да";

            Console.WriteLine("\nВыберите фильм.");
            Film chosenFilm = Film.ChooseFilm("choose screening after");
            Console.WriteLine();

            Screening chosenScreening = chosenFilm.ChooseScreening();
            AnsiConsole.Write(new Markup("\nДоступные места [green](0 - место доступно;[/] [red] x - место выкуплено)[/]\n"));
            chosenScreening.PrintHallData("availability");
            AnsiConsole.Write(new Markup("Цены на билеты\n"));
            chosenScreening.PrintHallData("prices");

            // в currOrder записываются списки вида {<ряд>, <место>} c ключом Screening
            do
            {
                try
                {
                    Console.WriteLine("\nПожалуйста, выберите места, которые вы хотите выкупить.");
                    Console.WriteLine("Введите один номер места в формате '<номер ряда> <номер места>'.");

                    string[] seatData = AnsiConsole.Prompt(new TextPrompt<string>("> ")).Split(' ');

                    List<int> ticket = new List<int> { int.Parse(seatData[0]) - 1, int.Parse(seatData[1]) - 1 };
                    int areRowSeatValid = chosenScreening.priceData[ticket[0]][ticket[1]]; // ловим IndexOutOfRangeException до добавления к заказу

                    // проверка, было ли это же место ранее добавлено в текущий заказ пользователя
                    bool alreadyInOrder = false;

                    foreach (Ticket existingTicket in alreadyReserved)
                    {
                        if (existingTicket.screening == chosenScreening && existingTicket.seat[0] == ticket[0] && existingTicket.seat[0] == ticket[1])
                        {
                            alreadyInOrder = true;
                            break;
                        }
                    }

                    if (currOrder.ContainsKey(chosenScreening))
                    {
                        foreach (var existingTicket in currOrder[chosenScreening])
                        {
                            if (ticket[0] == existingTicket[0] && ticket[1] == existingTicket[1])
                            {
                                alreadyInOrder = true;
                                break;
                            }
                        }
                    }

                    if (alreadyInOrder)
                        AnsiConsole.Write(new Markup("Вы уже выбрали это место.\n"));

                    // проверка, свободно ли место
                    else if (chosenScreening.seatsAvailability[ticket[0]][ticket[1]] == '0')
                    {
                        // добавляем в заказ
                        if (currOrder.ContainsKey(chosenScreening))
                            currOrder[chosenScreening].Add(ticket);
                        else
                            currOrder.Add(chosenScreening, new List<List<int>> { ticket });
                    }
                    else
                        AnsiConsole.Write(new Markup("К сожалению, данное место уже куплено.\n"));

                    AnsiConsole.Write(new Markup("\nХотите продолжить покупку билетов на этот сеанс?\n"));
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

            return currOrder;
        }
        public bool Check_Balance(List<Ticket> reserved) // проверка, достаточно ли у пользователя средств
        {
            Console.WriteLine("\nВыполняется проверка...\n");
            int ticketPriceSum = 0;

            foreach (Ticket ticket in reserved)
            {
                int row = ticket.seat[0]; int seat = ticket.seat[1];
                ticket.SetPrice(ticket.screening.priceData[row][seat]);
                ticketPriceSum = ticketPriceSum + ticket.screening.priceData[row][seat];
            }
            bool verificationStatus = ticketPriceSum <= balance;
            return verificationStatus;
        }
        public void MakeOrder()
        {
            List<Ticket> reservedTickets = new List<Ticket>();

            if (Film.all.Count == 0)
            {
                Console.WriteLine("Доступных фильмов нет");
                return;
            }

            int totalScreeningCount = 0;
            foreach (Film film in Film.all)
                totalScreeningCount = totalScreeningCount + film.screenings.Count;

            if (totalScreeningCount == 0)
            {
                Console.WriteLine("Доступных сеансов нет.");
                return;
            }

            string answer = "да"; // резервируем билеты
            do
            {
                Dictionary<Screening, List<List<int>>> ticketsToReserve = ReadOneScreeningOrder(reservedTickets);
                foreach (KeyValuePair<Screening, List<List<int>>> kvp in ticketsToReserve)
                {
                    foreach (List<int> seatsData in kvp.Value)
                    {
                        Ticket currTicket = new Ticket(username, kvp.Key, seatsData);
                        reservedTickets.Add(currTicket);
                    }
                } // добавили в бронирование

                AnsiConsole.Write(new Markup("\nХотите выбрать другие фильмы или сеансы?\n"));
                answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                    .AddChoice("да")
                                                    .AddChoice("нет")
                                                    .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз."));
            } while (answer == "да");

            if (reservedTickets.Count == 0)
            {
                Console.WriteLine("Сожалеем, что вы не приобрели ни одного билета. Пожалуйста, приходите к нам ещё!\n");
                return;
            }

            bool isOkayToBuy = Check_Balance(reservedTickets);
            while (!isOkayToBuy)
            {
                Console.WriteLine("Ошибка: недостаточно средств для покупки. Хотите пополнить баланс?");
                string yn = AnsiConsole.Prompt(new TextPrompt<string>("")
                                            .AddChoice("да")
                                            .AddChoice("нет")
                                            .InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз.\n"));
                if (yn == "да")
                {
                    UpdateBalance();
                    isOkayToBuy = Check_Balance(reservedTickets);
                }
                else
                {
                    Console.WriteLine($"Ваша бронь билетов аннулирована. На вашем счету остается {balance} рублей.");
                    return;
                }

            } // пополняем баланс или снимаем бронь и выходим

            foreach (Ticket ticket in reservedTickets)
            {
                ticket.screening.UpdateSeats(ticket.seat);
                ticket.SetTimeBougth();
                orders.Add(ticket);
            } // вносим данные о покупке в системы

            Console.WriteLine("Покупка прошла успешно! Ваши билеты:"); // печатаем купленные билеты для пользователя
            foreach (Ticket ticket in reservedTickets)
            {
                ticket.Print();
                balance = balance - ticket.price;
            }

            Console.WriteLine($"\nНа вашем счету осталось {balance} рублей.");

        }
        public void PrintAllTickets()
        {
            if (orders.Count == 0)
                Console.Write("Билетов нет.\n");
            else
            {
                orders.Sort((x, y) => x.screening.time.CompareTo(y.screening.time));
                foreach (Ticket ticket in orders)
                    ticket.Print();
            }
        }
        public static void GreatestTicketNum(int topNum)
        {
            List<User> sortedByTicketNum = all.OrderByDescending(o => o.orders.Count).ToList();

            if (sortedByTicketNum.Count < topNum)
            {
                Console.WriteLine("Недостаточное число клиентов в базе.");
                return;
            }

            for (int i = 0; i <= topNum - 1; i++)
                Console.WriteLine($"{i + 1}. {sortedByTicketNum[i].username}");
        }
        public static void GreatestDistinctScreeningTicketNum(int topNum)
        {
            List<ArrayList> userData = new List<ArrayList>();
            foreach (User user in User.all)
            {
                List<Ticket> distinctScreeningTickets = user.orders
                    .GroupBy(p => new { p.screening.film, p.screening.hall, p.screening.time })
                    .Select(g => g.First())
                    .ToList();
                ArrayList userStats = new ArrayList { user.username, distinctScreeningTickets.Count };
                userData.Add(userStats);
            }
            List<ArrayList> sorted = userData.OrderByDescending(o => o[1]).ToList();

            if (sorted.Count < topNum)
            {
                Console.WriteLine("Недостаточное число клиентов в базе.");
                return;
            }

            for (int i = 0; i <= topNum - 1; i++)
                Console.WriteLine($"{i + 1}. {sorted[i]}");
        }
        public static void GreatestMoneyAmountSpent(int topNum)
        {
            List<ArrayList> userData = new List<ArrayList>();
            foreach (User user in User.all)
            {
                int total = user.orders.Sum(ticket => ticket.price);
                ArrayList userStats = new ArrayList { user.username, total };
                userData.Add(userStats);
            }
            List<ArrayList> sorted = userData.OrderByDescending(o => o[1]).ToList();

            if (sorted.Count < topNum)
            {
                Console.WriteLine("Недостаточное число клиентов в базе.");
                return;
            }

            for (int i = 0; i <= topNum - 1; i++)
                Console.WriteLine($"{i + 1}. {sorted[i]}");
        }
    }
}
