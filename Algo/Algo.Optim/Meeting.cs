using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algo.Optim
{
    public class Guest
    {
        public string Name { get; set; }

        public Airport Location { get; set; }

        public List<SimpleFlight> ArrivalFlights { get; } = new List<SimpleFlight>();

        public List<SimpleFlight> DepartureFlights { get; } = new List<SimpleFlight>();

    }

    public class Meeting
    {
        public Meeting(string flightDatabasePath)
        {
            Database = new FlightDatabase(flightDatabasePath);
            Location = Airport.FindByCode("LHR");
            Guests.Add(new Guest()
            {
                Name = "Adolf",
                Location = Airport.FindByCode("BER")
            });
            Guests.Add(new Guest()
            {
                Name = "Adeline",
                Location = Airport.FindByCode("CDG")
            });
            Guests.Add(new Guest()
            {
                Name = "Marcel",
                Location = Airport.FindByCode("MRS")
            });
            Guests.Add(new Guest()
            {
                Name = "Léon",
                Location = Airport.FindByCode("LYS")
            });
            Guests.Add(new Guest()
            {
                Name = "Peter",
                Location = Airport.FindByCode("MAN")
            });
            Guests.Add(new Guest()
            {
                Name = "Jose",
                Location = Airport.FindByCode("BIO")
            });
            Guests.Add(new Guest()
            {
                Name = "Donald",
                Location = Airport.FindByCode("JFK")
            });
            Guests.Add(new Guest()
            {
                Name = "Youssef",
                Location = Airport.FindByCode("TUN")
            });
            Guests.Add(new Guest()
            {
                Name = "Mario",
                Location = Airport.FindByCode("MXP")
            });
            MaxArrivalDate = new DateTime(2010, 7, 27, 17, 0, 0);
            MinDepartureDate = new DateTime(2010, 8, 3, 15, 0, 0);
        }

        public FlightDatabase Database { get; }


        public List<Guest> Guests { get; } = new List<Guest>();

        public int MaxFlightCount = 50;

        public DateTime MaxArrivalDate { get; }

        public DateTime MinDepartureDate { get; }

        public Airport Location { get; private set; }

    }
}
