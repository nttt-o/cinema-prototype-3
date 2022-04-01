using cinema_prototype_3;
using Newtonsoft.Json;
using Spectre.Console;
using System.Collections;
using System.Globalization;
using System.Text;


namespace cinema_prototype_3
{
    internal abstract class Hall
    {
        public abstract string name { get; set; }
        public abstract int rowsNum { get; set; }
        public abstract int seatsInRowNum { get; set; }

        //public abstract Hall ChooseHall();
        public abstract void PrintHallInfoForCustomer();
        public abstract void PrintCurrentHallInfoForAdministrator();
        public abstract string GetHallType();
        public abstract void SetInitialHallData();

        public void SetName()
        {
            List<string> existingHallNames = new List<string>();
            existingHallNames.AddRange(StandartHall.all.Select(hall => hall.name).ToList());
            existingHallNames.AddRange(LuxeHall.all.Select(hall => hall.name).ToList());
            existingHallNames.AddRange(BlackHall.all.Select(hall => hall.name).ToList());

            bool succeeded = false;
            Console.WriteLine($"Введите название зала:");
            while (!succeeded)
            {
                string inputName = AnsiConsole.Prompt(new TextPrompt<string>("> "));

                if (existingHallNames.Any(filter => filter == inputName))
                {
                    Console.Write("Данное название уже есть в базе. ");
                    continue;
                }

                if (inputName.Length >= 1 && inputName != " ")
                {
                    name = inputName;
                    succeeded = true;
                }
                else
                    Console.WriteLine("Неверное значение для названия зала. Повторите попытку.");
            }
        }
        public bool CanBeEdited()
        {
            bool canBeEdited = true;

            foreach (Film film in Film.all)
            {
                foreach (Screening screening in film.screenings)
                {
                    if (screening.hall.name != name)
                        continue;

                    for (int i = 0; i < screening.hall.rowsNum; i++)
                    {
                        for (int j = 0; j < screening.hall.seatsInRowNum; j++)
                        {
                            if (screening.seatsAvailability[i][j] == 'x')
                            {
                                canBeEdited = false;
                                break;
                            }
                        }
                    }
                }
            }
            return canBeEdited;
        }

    }

    internal class StandartHall : Hall
    {
        public static List<StandartHall> all { get { return _all; } set { _all = value; } }
        private static List<StandartHall> _all = new List<StandartHall>();

        private string _name;
        public override string name { get { return _name; } set { _name = value; } }
        
        private static int _minPrice = 200;
        public static int minPrice { get { return _minPrice; }}

        private int _rowsNum = 10;
        public override int rowsNum { get { return _rowsNum; } set { _rowsNum = value; } }

        private int _seatsInRowNum = 20;
        public override int seatsInRowNum { get { return _seatsInRowNum; } set { _seatsInRowNum = value; } }
        public override string GetHallType()
                {
                    return "Standart";
                }
        public override void PrintHallInfoForCustomer()
        {
            Console.WriteLine($"Название зала: {_name}, тип зала: {this.GetHallType()}");
        }
        public override void PrintCurrentHallInfoForAdministrator()
        {
            Console.WriteLine($"Название зала: {_name}, тип зала: {this.GetHallType()}.");
            Console.WriteLine($"Размер зала: {_rowsNum} рядов, {_seatsInRowNum} мест.");
            Console.WriteLine($"Минимальная цена за билет: {_minPrice}.");
            Console.WriteLine("Дополнительных удобств для зрителей нет.");
;       }
        public override void SetInitialHallData()
        {
            this.SetName();
            Console.WriteLine($"Введите число рядов в зале {_name} типа {this.GetHallType()}:");
            int rows = Program.GetPositiveInt(); _rowsNum = rows;
            Console.WriteLine("Введите число мест в одном ряду:");
            int seats = Program.GetPositiveInt(); _seatsInRowNum = seats;
            StandartHall.all.Add(this);
        }

        public static void PrintHallTypeCharacteristics()
        {
            Console.WriteLine($"Тип зала: Standart.");
            Console.WriteLine($"Минимальная цена за билет: {_minPrice}.");
            Console.WriteLine("Дополнительных удобств для зрителей нет.");
        }
        public static StandartHall ChooseHall()
        {
            TextPrompt<string> hallChoicePrompt = new TextPrompt<string>("").InvalidChoiceMessage("Введен неверный вариант. Повторите попытку.");

            foreach (StandartHall hall in all)
                hallChoicePrompt.AddChoice(hall.name);
            string chHallName = AnsiConsole.Prompt(hallChoicePrompt);

            foreach (StandartHall hall in all)
            {
                if (hall._name == chHallName)
                    return hall;
            }
            return new StandartHall(); // dummyHall
        }


    }

    internal class LuxeHall : Hall
    {
        public override string GetHallType()
        {
            return "Luxe";
        }

        public static List<LuxeHall> all { get { return _all; } set { _all = value; } }
        private static List<LuxeHall> _all = new List<LuxeHall>();

        public override string name { get { return _name; } set { _name = value; } }
        private string _name;

        public static int minPrice { get { return _minPrice; } }
        private static int _minPrice = 450;

        public override int rowsNum { get { return _rowsNum; } set { _rowsNum = value; } }
        private int _rowsNum = 5;

        public override int seatsInRowNum { get { return _seatsInRowNum; } set { _seatsInRowNum = value; } }
        private int _seatsInRowNum = 6;

        public string[] foodMenu { get { return _foodMenu; } set { _foodMenu = value; } }
        private string[] _foodMenu = new string[] { "Пицца", "Суши", "Бургер", "Попкорн", "Мороженое" };

        public string[] drinksMenu { get { return _drinksMenu; } set { _drinksMenu = value; } }
        private string[] _drinksMenu = new string[] { "Листовой чай", "Кофе", "Лимонад", "Свежевыжатый сок"};

        public override void PrintHallInfoForCustomer()
        {
            Console.WriteLine($"Название зала: {_name}, тип зала: {this.GetHallType()}.\nВозможен заказ еды и напитков.");
        }
        public override void PrintCurrentHallInfoForAdministrator()
        {
            Console.WriteLine($"Название зала: {_name}, тип зала: {this.GetHallType()}.");
            Console.WriteLine($"Размер зала: {_rowsNum} рядов, {_seatsInRowNum} мест.");
            Console.WriteLine($"Минимальная цена за билет: {_minPrice}.");
            Console.WriteLine("Зрителям доступен заказ еды и напитков из стандартного меню.");
        }
        public static void PrintHallTypeCharacteristics()
        {
            Console.WriteLine($"Тип зала: Luxe.");
            Console.WriteLine($"Минимальная цена за билет: {_minPrice}.");
            Console.WriteLine("Зрителям доступен заказ еды и напитков из стандартного меню.");
        }
        public override void SetInitialHallData()
        {
            this.SetName();
            Console.WriteLine($"Введите число рядов в зале {_name} типа {this.GetHallType()}:");
            int rows = Program.GetPositiveInt(); _rowsNum = rows;
            Console.WriteLine("Введите число мест в одном ряду:");
            int seats = Program.GetPositiveInt(); _seatsInRowNum = seats;
            LuxeHall.all.Add(this);
        }
        public static LuxeHall ChooseHall()
        {
            TextPrompt<string> hallChoicePrompt = new TextPrompt<string>("").InvalidChoiceMessage("Введен неверный вариант. Повторите попытку.");

            foreach (LuxeHall hall in all)
                hallChoicePrompt.AddChoice(hall.name);
            string chHallName = AnsiConsole.Prompt(hallChoicePrompt);

            foreach (LuxeHall hall in all)
            {
                if (hall._name == chHallName)
                    return hall;
            }
            return new LuxeHall(); // dummyHall
        }


        public List<string> OrderFood()
        {
            var foodOrdered = AnsiConsole.Prompt(
                   new MultiSelectionPrompt<string>()
                       .Title("Хотите заказать еду? Счет будет предъявлен после сеанса.")
                       .NotRequired()
                       .PageSize(10)
                       .MoreChoicesText("[grey](Двигайтесь вверх и вниз, чтобы посмотреть меню)[/]")
                       .InstructionsText(
                           "[grey](Нажмите [blue]<пробел>[/] для выбора позиции, " +
                           "[green]<enter>[/] для завершения выбора)[/]")
                       .AddChoices(_foodMenu));

            return foodOrdered;
        }
        public List<string> OrderBeverages()
        {
            var beveragesOrdered = AnsiConsole.Prompt(
                   new MultiSelectionPrompt<string>()
                       .Title("Хотите заказать напитки? Счет будет предъявлен после сеанса.")
                       .NotRequired()
                       .PageSize(10)
                       .MoreChoicesText("[grey](Двигайтесь вверх и вниз, чтобы посмотреть меню)[/]")
                       .InstructionsText(
                           "[grey](Нажмите [blue]<пробел>[/] для выбора позиции, " +
                           "[green]<enter>[/] для завершения выбора)[/]")
                       .AddChoices(_drinksMenu));

            return beveragesOrdered;
        }

    }


    internal class BlackHall : Hall
    {
        public override string GetHallType()
        {
            return "Black";
        }
        public static List<BlackHall> all { get { return _all; } set { _all = value; } }
        private static List<BlackHall> _all = new List<BlackHall>();

        public override string name { get { return _name; } set { _name = value; } }
        private string _name;

        public static int minPrice { get { return _minPrice; } }
        private static int _minPrice = 1000;

        public override int rowsNum { get { return _rowsNum; } set { _rowsNum = value; } }
        private int _rowsNum = 3;

        public override int seatsInRowNum { get { return _seatsInRowNum; } set { _seatsInRowNum = value; } }
        private int _seatsInRowNum = 4;

        public string[] foodMenu { get { return _foodMenu; } set { _foodMenu = value; } }
        private string[] _foodMenu = new string[] { "Пицца", "Паста", "Стейк", "Суши", "Роллы", "Салат", "Бургер", "Попкорн", "Мороженое" };

        public string[] drinksMenu { get { return _drinksMenu; } set { _drinksMenu = value; } }
        private string[] _drinksMenu = new string[] { "Листовой чай", "Кофе", "Лимонад", "Свежевыжатый сок", "Пиво", "Сидр", "Вино", "Шампанское" };

        public override void PrintHallInfoForCustomer()
        {
            Console.WriteLine($"Название зала: {_name}, тип зала: Black.");
            Console.WriteLine("Возможен заказ еды и напитков, получение пледа и подушек для более комфортного просмотра");
        }
        public override void PrintCurrentHallInfoForAdministrator()
        {
            Console.WriteLine($"Название зала: {_name}, тип зала: {this.GetHallType()}.");
            Console.WriteLine($"Размер зала: {_rowsNum} рядов, {_seatsInRowNum} мест.");
            Console.WriteLine($"Минимальная цена за билет: {_minPrice}.");
            Console.WriteLine("Зрителям доступен заказ еды и напитков из расширенного меню, получение пледа и подушек для более комфортного просмотра.");
        }
        public override void SetInitialHallData()
        {
            this.SetName();
            Console.WriteLine($"Введите число рядов в зале {_name} типа {this.GetHallType()}:");
            int rows = Program.GetPositiveInt(); _rowsNum = rows;
            Console.WriteLine("Введите число мест в одном ряду:");
            int seats = Program.GetPositiveInt(); _seatsInRowNum = seats;
            BlackHall.all.Add(this);
        }


        public static void PrintHallTypeCharacteristics()
        {
            Console.WriteLine($"Тип зала: Black.");
            Console.WriteLine($"Минимальная цена за билет: {_minPrice}.");
            Console.WriteLine("Зрителям доступен заказ еды и напитков из расширенного меню, получение пледа и подушек для более комфортного просмотра.");
        }
        public static BlackHall ChooseHall()
        {
            TextPrompt<string> hallChoicePrompt = new TextPrompt<string>("").InvalidChoiceMessage("Введен неверный вариант. Повторите попытку.");

            foreach (BlackHall hall in all)
                hallChoicePrompt.AddChoice(hall.name);
            string chHallName = AnsiConsole.Prompt(hallChoicePrompt);

            foreach (BlackHall hall in all)
            {
                if (hall._name == chHallName)
                    return hall;
            }
            return new BlackHall(); // dummyHall
        }

        public List<string> OrderFood()
        {
            var foodOrdered = AnsiConsole.Prompt(
                   new MultiSelectionPrompt<string>()
                       .Title("Хотите заказать еду? Счет будет предъявлен после сеанса.")
                       .NotRequired()
                       .PageSize(10)
                       .MoreChoicesText("[grey](Двигайтесь вверх и вниз, чтобы посмотреть меню)[/]")
                       .InstructionsText(
                           "[grey](Нажмите [blue]<пробел>[/] для выбора позиции, " +
                           "[green]<enter>[/] для завершения выбора)[/]")
                       .AddChoices(_foodMenu));

            return foodOrdered;
        }
        public List<string> OrderBeverages()
        {
            var beveragesOrdered = AnsiConsole.Prompt(
                   new MultiSelectionPrompt<string>()
                       .Title("Хотите заказать напитки? Счет будет предъявлен после сеанса.")
                       .NotRequired()
                       .PageSize(10)
                       .MoreChoicesText("[grey](Двигайтесь вверх и вниз, чтобы посмотреть меню)[/]")
                       .InstructionsText(
                           "[grey](Нажмите [blue]<пробел>[/] для выбора позиции, " +
                           "[green]<enter>[/] для завершения выбора)[/]")
                       .AddChoices(_drinksMenu));

            return beveragesOrdered;
        }
        public static bool AddBlanket()
        {
            Console.WriteLine("Хотите получить плед на время сеанса?");
            string answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                .AddChoice("да")
                                                .AddChoice("нет")
                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
            if (answer == "да")
                return true;
            return false;
        }
        public static bool AddPillows()
        {
            Console.WriteLine("Хотите получить подушки на время сеанса?");
            string answer = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                .AddChoice("да")
                                                .AddChoice("нет")
                                                .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
            if (answer == "да")
                return true;
            return false;
        }
    }

}
