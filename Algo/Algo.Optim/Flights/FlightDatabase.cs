using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Algo.Optim
{
    public class FlightDatabase
    {
        string _path;
        Dictionary<string,IList<SimpleFlight>> _cache;
        KayakSession _kayak;


        public FlightDatabase( string path )
        {
            _path = path[path.Length-1] != '\\' ? path+'\\' : path;
            _cache = new Dictionary<string, IList<SimpleFlight>>();
            Airport.Initialize( Path.Combine( path, "airports.txt" ) );
        }

        KayakSession KayakSession
        {
            get { return _kayak ?? (_kayak = new KayakSession()); }
        }

        public IList<SimpleFlight> GetFlights( DateTime day, Airport from, Airport to )
        {
            IList<SimpleFlight> flights;
            string p = $"{_path}{day.Date:yyyy}\\{day.Date:MM}-{day.Date:dd}\\{from.Code}-{to.Code}.txt";
            if( !_cache.TryGetValue( p, out flights ) )
            {
                if( File.Exists( p ) )
                {
                    flights = SimpleFlight.Load( p );
                }
                else
                {
                    flights = KayakSession.SimpleFlightSearch( from.Code, to.Code, day );
                    Directory.CreateDirectory( Path.GetDirectoryName( p ) );
                    SimpleFlight.Save( flights, p );
                }
                _cache.Add( p, flights );
            }
            return flights;
        }


    
    }
}
