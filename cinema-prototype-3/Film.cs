﻿using cinema_prototype_3;
using Newtonsoft.Json;
using Spectre.Console;
using System.Collections;
using System.Globalization;
using System.Text;


namespace cinema_prototype_3
{
    internal class Film
    {
        public static List<Film> all { get { return _all; } set { _all = value; } }
        private static List<Film> _all = new List<Film>();

        public string name { get { return _name; } set { _name = value; } }
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
                    _name = inputName;
                    succeeded = true;
                }
                else
                    Console.WriteLine("Неверное значение для названия фильма. Повторите попытку.");
            }
        }
        private string _name = "";

        public string ageRestriction { get { return _ageRestriction; } set { _ageRestriction = value; } }
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
            _ageRestriction = ageRest;
        }
        private string _ageRestriction = "";

        public List<Hall> halls { get { return _halls; } set { _halls = value; } }
        private List<Hall> _halls = new List<Hall>();

        public List<Screening> screenings { get { return _screenings; } set { _screenings = value; } }
        private List<Screening> _screenings = new List<Screening>();


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
        public Screening ChooseScreening(string whoChooses) // user | administrator
        {
            List<Screening> relevantScreenings = screenings.FindAll(screening => screening.time > DateTime.Now);
            if (whoChooses == "user")
            {
                relevantScreenings = relevantScreenings.FindAll(screening => (screening.time - DateTime.Now).Days <= screening.maxBookingPeriod);
            }
            relevantScreenings.Sort((x, y) => x.time.CompareTo(y.time));

            if (relevantScreenings.Count == 0)
            {
                Console.WriteLine("Актуальных сеансов на данный фильм нет.");
                return null;
            }
            TextPrompt<string> scrChoicePrompt = new TextPrompt<string>("").InvalidChoiceMessage("Введена неверная команда. Пожалуйста, попробуйте еще раз.");

            for (int i = 0; i < relevantScreenings.Count; i++)
            {
                relevantScreenings[i].Print(i);
                scrChoicePrompt.AddChoice(Convert.ToString(i + 1));
            }

            Console.WriteLine("\nВведите номер одного выбранного сеанса:");
            int scrNum = int.Parse(AnsiConsole.Prompt(scrChoicePrompt)) - 1;

            return relevantScreenings[scrNum];
        }
        public static Tuple<Film, int> ChooseFilm(string after)
        {
            TextPrompt<string> filmChoicePrompt = new TextPrompt<string>("").InvalidChoiceMessage("Неверный вариант. Повторите попытку.");
            int optionCount = 0;
            if (after == "screening count important") // убираем варианты, где у фильма нет сеансов
            {
                foreach (Film film in Film.all)
                {
                    if (film.screenings.Count != 0)
                    {
                        filmChoicePrompt.AddChoice(film.name);
                        optionCount++;
                    }
                }
            }
            else // "screening count not important"
            {
                foreach (Film film in Film.all)
                {
                    filmChoicePrompt.AddChoice(film.name);
                    optionCount++;
                }
            }

            if (optionCount == 0)
            {
                Console.WriteLine("Выбрать фильм невозможно;");
                return Tuple.Create(new Film(), -1); // код -1 --> фильм не возвращен
            }

            string inputName = AnsiConsole.Prompt(filmChoicePrompt);

            foreach (Film film in Film.all)
            {
                if (film.name == inputName)
                    return Tuple.Create(film, 1);// код 1 --> фильм возвращен
            }
            return Tuple.Create(new Film(), -1); // код -1 --> фильм не возвращен

        }

        public static void ReadFile(string filename)
        {
            using (var sr = new StreamReader(filename))
            {
                using (var jsonReader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer()
                    { TypeNameHandling = TypeNameHandling.Auto, PreserveReferencesHandling = PreserveReferencesHandling.Objects };

                    Film.all = serializer.Deserialize<List<Film>>(jsonReader);
                }
            }

        }
        public static void UpdateFile(string filename)
        {
            using (var sw = new StreamWriter(filename))
            {
                using (var jsonWriter = new JsonTextWriter(sw))
                {
                    jsonWriter.Formatting = Formatting.Indented;

                    var serializer = new JsonSerializer()
                    { TypeNameHandling = TypeNameHandling.Auto, PreserveReferencesHandling = PreserveReferencesHandling.Objects };

                    serializer.Serialize(jsonWriter, Film.all);
                }
            }
        }

    }
}
