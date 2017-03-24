using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace Algo.Optim
{
    public class SimpleFlight
    {
        internal SimpleFlight( XElement e )
        {
            Price = double.Parse( e.Descendants( "price" ).First().Value );
            Stops = int.Parse( e.Descendants( "stops" ).First().Value );
            Origin = Airport.FindByCode( e.Descendants( "orig" ).First().Value );
            DepartureTime = DateTime.Parse( e.Descendants( "depart" ).First().Value );
            Destination = Airport.FindByCode( e.Descendants( "dest" ).First().Value );
            ArrivalTime = DateTime.Parse( e.Descendants( "arrive" ).First().Value );
            Company = e.Descendants( "airline_display" ).First().Value;
        }

        public double Price { get; private set; }

        public DateTime DepartureTime { get; private set; }

        public int Stops { get; private set; }

        public string Company { get; private set; }

        public Airport Origin { get; private set; }
        
        public Airport Destination { get; private set; }
        
        public DateTime ArrivalTime { get; private set; }

        static public IList<SimpleFlight> Load( string path )
        {
            IList<SimpleFlight> results = new List<SimpleFlight>();
            using( TextReader r = File.OpenText( path ) )
            {
                foreach( var f in XElement.Load( r ).Descendants( "flight" ) )
                {
                    results.Add( new SimpleFlight( f ) );
                }
            }
            return results;
        }

        static public void Save( IEnumerable<SimpleFlight> flights, string path )
        {
            var settings = new XmlWriterSettings()
            {
                Indent = true
            };
            using( XmlWriter r = XmlWriter.Create( path, settings ) )
            {
                r.WriteStartDocument();
                r.WriteStartElement( "flights" );
                foreach( var f in flights )
                {
                    r.WriteStartElement( "flight" );
                    r.WriteElementString( "price", f.Price.ToString() );
                    r.WriteElementString( "stops", f.Stops.ToString() );
                    r.WriteElementString( "orig", f.Origin.Code.ToString() );
                    r.WriteElementString( "dest", f.Destination.Code.ToString() );
                    r.WriteElementString( "depart", f.DepartureTime.ToString( "s" ) );
                    r.WriteElementString( "arrive", f.ArrivalTime.ToString( "s" ) );
                    r.WriteElementString( "airline_display", f.Company );
                    r.WriteEndElement();
                }
                r.WriteEndDocument();
            }
        }

    }
}
