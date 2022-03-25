using cinema_prototype_3;
using Spectre.Console;
using System.Collections;
using System.Globalization;
using System.Text;


namespace cinema_prototype_3
{
    internal class Film
    {
        public static List<Film> all = new List<Film>();

        public string name = "";
        public string ageRestriction = "";
        public List<Hall> halls = new List<Hall>();
        public List<Screening> screenings = new List<Screening>();

        public void SetName()
        {
            bool succeeded = false;
            Console.WriteLine($"\nВведите название фильма:");
            while (!succeeded)
            {
                string inputName = AnsiConsole.Prompt(new TextPrompt<string>("> "));

                bool alreadyExists = false;
                foreach (Film film in Film.all)
                {
                    if (inputName == film.name)
                    {
                        Console.WriteLine("Данный фильм уже есть в базе. Повторите попытку.");
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
                    Console.WriteLine("Неверное значение для названия фильма. Повторите попытку.");
            }
        }
        public void SetAgeRestriction()
        {
            Console.WriteLine($"\nВведите возрастное ограничение для фильма {name}:");
            string ageRest = AnsiConsole.Prompt(new TextPrompt<string>("")
                                                            .AddChoice("0+")
                                                            .AddChoice("6+")
                                                            .AddChoice("12+")
                                                            .AddChoice("16+")
                                                            .AddChoice("18+")
                                                            .InvalidChoiceMessage("Введен неверный вариант. Пожалуйста, попробуйте еще раз."));
            ageRestriction = ageRest;
        }
        public bool CanBeEdited()
        {
            bool canBeEdited = true;
            foreach (Screening screening in screenings)
            {
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
            return canBeEdited;
        }
        public Screening ChooseScreening()
        {
            List<Screening> relevantScreenings = screenings.FindAll(screening => screening.time > DateTime.Now);
            relevantScreenings.Sort((x, y) => x.time.CompareTo(y.time));

            if (relevantScreenings.Count == 0)
            {
                Console.WriteLine("Актуальных сеансов на данный фильм нет.");
                return new Screening();
            }
            TextPrompt<string> scrChoicePrompt = new TextPrompt<string>("").InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз.");

            for (int i = 0; i < relevantScreenings.Count; i++)
            {
                Console.WriteLine($"{i + 1,3}: {relevantScreenings[i].hall.name,10} {relevantScreenings[i].hall.type} | {relevantScreenings[i].time.ToString("dd/MM/yyyy HH:mm"),16}");
                scrChoicePrompt.AddChoice(Convert.ToString(i + 1));
            }

            Console.WriteLine("\nВведите номер одного выбранного сеанса:");
            int scrNum = int.Parse(AnsiConsole.Prompt(scrChoicePrompt)) - 1;

            return screenings[scrNum];
        }
        public static Film ChooseFilm(string after)
        {
            TextPrompt<string> filmChoicePrompt = new TextPrompt<string>("").InvalidChoiceMessage("Неверный вариант. Повторите попытку.");

            if (after == "screening count important") // убираем варианты, где у фильма нет сеансов
            {
                foreach (Film film in Film.all)
                {
                    if (film.screenings.Count != 0)
                        filmChoicePrompt.AddChoice(film.name);
                }
            }

            else // "screening count not important"
            {
                foreach (Film film in Film.all)
                    filmChoicePrompt.AddChoice(film.name);
            }

            string inputName = AnsiConsole.Prompt(filmChoicePrompt);

            foreach (Film film in Film.all)
            {
                if (film.name == inputName)
                    return film;
            }
            return new Film(); // dummyFilm
        }
        public void ProcessData(string line)
        {
            string[] parts = line.Split(',');  //Разделитель в CSV файле.
            name = parts[0];
            ageRestriction = parts[1];
            foreach (string hallname in parts[2].Split(';'))
            {
                foreach (Hall hall in Hall.all)
                {
                    if (hall.name == hallname)
                    {
                        halls.Add(hall);
                        break;
                    }
                }
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
                    Film film = new Film();
                    film.ProcessData(line);
                    all.Add(film);
                }
            }
        }
        public static void UpdateFile(string filename)
        {
            var csv = new StringBuilder();
            foreach (Film film in Film.all)
            {
                string hallnames = String.Join(';', film.halls.Select(hall => hall.name).ToList());
                string filmInfo = $"{film.name},{film.ageRestriction},{hallnames}";
                csv.AppendLine(filmInfo);
            }
            File.WriteAllText(filename, csv.ToString(), Encoding.GetEncoding(1251));
        }
    }
}
