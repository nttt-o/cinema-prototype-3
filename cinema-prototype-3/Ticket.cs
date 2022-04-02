using cinema_prototype_3;
using Spectre.Console;
using System.Collections;
using System.Globalization;
using System.Text;

namespace cinema_prototype_3
{
    internal class Ticket
    {
        public string username { get { return _username; } }
        private string _username;

        public Screening screening { get { return _screening; } }
        private Screening _screening;

        public List<int> seat { get { return _seat; } }
        private List<int> _seat;

        public int price { get { return _price; } }
        public void SetPrice(int somePrice)
        {
            _price = somePrice;
        }
        public int _price;

        public DateTime timeBought { get { return _timeBought; } }
        public void SetTimeBougth()
        {
            _timeBought = DateTime.Now;
        }
        private DateTime _timeBought;

        public bool posterNeeded { get { return _posterNeeded; } set { _posterNeeded = value; }}
        private bool _posterNeeded;

        public bool authographsNeeded { get { return _authographsNeeded; } set { _authographsNeeded = value; } }
        private bool _authographsNeeded;

        public List<string> foodOrdered { get { return _foodOrdered; } set { _foodOrdered = value; } }
        private List<string> _foodOrdered;

        public List<string> drinksOrdered { get { return _drinksOrdered; } set { _drinksOrdered = value; } }
        private List<string> _drinksOrdered;

        public bool pillowsNeeded { get { return _pillowsNeeded; } set { _pillowsNeeded = value; } }
        private bool _pillowsNeeded;

        public bool blanketsNeeded { get { return _blanketsNeeded; } set { _blanketsNeeded = value; } }
        private bool _blanketsNeeded;

        public Ticket(string username, Screening screening, List<int> seat)
        {
            _username = username;
            _screening = screening;
            _seat = seat;
        }

        public void Print()
        {
            Console.WriteLine($"Фильм {screening.film.name,15} | Зал {screening.hall.name,15} | {screening.hall.GetHallType(), 12} | {screening.time.ToString("dd/MM/yyyy HH:mm")} | Ряд {seat[0] + 1,2} | Место {seat[1] + 1,3}");
        }
    }
}
