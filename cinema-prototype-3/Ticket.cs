using cinema_prototype_3;
using Spectre.Console;
using System.Collections;
using System.Globalization;
using System.Text;

namespace cinema_prototype_3
{
    internal class Ticket
    {
        public string username;
        public Screening screening;
        public List<int> seat;
        public int price;
        public DateTime timeBought;

        public Ticket(string username, Screening screening, List<int> seat)
        {
            this.username = username;
            this.screening = screening;
            this.seat = seat;
        }
        public void SetTimeBougth()
        {
            timeBought = DateTime.Now;
        }
        public void SetPrice(int somePrice)
        {
            price = somePrice;
        }
        public void Print()
        {
            Console.WriteLine($"Фильм {screening.film.name,15} | Зал {screening.hall.name,15} | {screening.time.ToString("dd/MM/yyyy HH:mm")} | Ряд {seat[0] + 1,2} | Место {seat[1] + 1,2}");
        }
    }
}
