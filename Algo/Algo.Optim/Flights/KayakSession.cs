using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using System.Configuration;

namespace Algo.Optim
{
    public class KayakSession
    {
        string _kayakUser;
        string _kayakKey;
        string _idsession; 
        const string _kayakBaseUrl = "http://api.kayak.com";

        public string SessionId 
        {
            get
            {
                if( _idsession == null )
                {
                    if( _kayakUser == null )
                    {
                        _kayakUser = ConfigurationManager.AppSettings["KayakUser"];
                        _kayakKey = ConfigurationManager.AppSettings["KayakKey"];
                    }
                    string url = _kayakBaseUrl + "/k/ident/apisession?version=1&token=" + _kayakKey;
                    XElement e = XElement.Load( url, LoadOptions.None );
                    _idsession = e.Descendants( "sid" ).First().Value;
                }
                return _idsession;
            }
        }

        public KayakSession()
        {
        }

        public KayakSession( string kayakUser, string kayakApiKey )
        {
            _kayakUser = kayakUser;
            _kayakKey = kayakApiKey;
        }


        public IList<SimpleFlight> SimpleFlightSearch( string fromAirport, string toAirport, DateTime departDate )
        {
            // Start Search
            string u = GetStartSearchUrl( fromAirport, toAirport, departDate );
            string idSearch = XElement.Load( u, LoadOptions.None ).Descendants("searchid").First().Value;
                        
            //Get Flight Results
            u = GetResultUrl( idSearch, 999 );

            List<SimpleFlight> flights = new List<SimpleFlight>();
            XElement result;
            for(;;)
            {
                result = XElement.Load( u, LoadOptions.None );
                foreach( XElement e in result.Descendants( "trip" ) )
                {
                    flights.Add( new SimpleFlight( e ) );
                }
                if( result.Descendants( "morepending" ).Any( e => e.Value == "true" ) == false ) break;
                Thread.Sleep( 3000 );
            }

            return flights;
        }

        private string GetStartSearchUrl( string fromAirport, string toAirport, DateTime departDate )
        {
            string u = _kayakBaseUrl + "/s/apisearch?";
            u += "basicmode=true&";
            u += "oneway=y&";
            u += "origin=" + fromAirport + "&";
            u += "destination=" + toAirport + "&";
            u += "depart_date=" + departDate.ToString( "MM/dd/yyyy" ) + "&";
            u += "depart_time=a&";
            u += "travelers=1&";
            u += "cabin=e&";
            u += "action=doFlights&";
            u += "apimode=1&";
            u += "_sid_=" + SessionId;
            return u;
        }

        private String GetResultUrl( string idSearch, int count )
        {
            return _kayakBaseUrl + "/s/basic/flight?searchid=" + idSearch + "&c="+ count +"&apimode=1&s=price&d=up&_sid_=" + SessionId;
        }
    }
}