using cinema_prototype_3;
using Spectre.Console;
using System.Collections;
using System.Globalization;
using System.Text;


namespace cinema_prototype_3
{
    internal class Hall
    {
        public static List<Hall> all = new List<Hall>();

        public string name = "";
        public int rowsNum;
        public int seatsInRowNum;
        public string type = "";

        public void SetName()
        {
            bool succeeded = false;
            Console.WriteLine($"Введите название зала:");
            while (!succeeded)
            {
                string inputName = AnsiConsole.Prompt(new TextPrompt<string>("> "));

                bool alreadyExists = false;
                foreach (Hall hall in Hall.all)
                {
                    if (inputName == hall.name)
                    {
                        Console.Write("Данное название уже есть в базе. ");
                        alreadyExists = true;
                        break;
                    }
                }

                if (inputName.Length >= 1 && !alreadyExists)
                {
                    name = inputName;
                    succeeded = true;
                }
                else
                    Console.WriteLine("Неверное значение для названия зала. Повторите попытку.");
            }
        }
        public void SetType()
        {
            Console.WriteLine($"Введите тип зала {name}:");
            string typeCode = AnsiConsole.Prompt(new TextPrompt<string>("1 - стандартный, 2 - VIP")
                                                            .AddChoice("1")
                                                            .AddChoice("2")
                                                            .InvalidChoiceMessage("[red1]Введен неверный вариант. Пожалуйста, попробуйте еще раз.[/]"));
            Console.WriteLine();
            if (typeCode == "1")
                type = "стандартный";
            if (typeCode == "2")
                type = "VIP";
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
        public static Hall ChooseHall()
        {
            TextPrompt<string> hallChoicePrompt = new TextPrompt<string>("").InvalidChoiceMessage("Введен неверный вариант. Повторите попытку.");
            foreach (Hall hall in Hall.all)
                hallChoicePrompt.AddChoice(hall.name);
            string chHallName = AnsiConsole.Prompt(hallChoicePrompt);

            foreach (Hall hall in Hall.all)
            {
                if (hall.name == chHallName)
                    return hall;
            }
            return new Hall(); // dummyHall
        }
        public void ProcessData(string line)
        {
            string[] parts = line.Split(',');  //Разделитель в CSV файле.
            name = parts[0];
            rowsNum = int.Parse(parts[1]);
            seatsInRowNum = int.Parse(parts[2]);
            type = parts[3];
        }
        public static void ReadFile(string filename)
        {
            var encoding = Encoding.GetEncoding(1251);
            using (StreamReader sr = new StreamReader(filename, encoding: encoding))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Hall hall = new Hall();
                    hall.ProcessData(line);
                    all.Add(hall);
                }
            }
        }
        public static void UpdateFile(string filename)
        {
            var csv = new StringBuilder();
            foreach (Hall hall in Hall.all)
            {
                string hallInfo = $"{hall.name},{hall.rowsNum},{hall.seatsInRowNum},{hall.type}";
                csv.AppendLine(hallInfo);
            }
            File.WriteAllText(filename, csv.ToString(), Encoding.GetEncoding(1251));
        }
    }
}
