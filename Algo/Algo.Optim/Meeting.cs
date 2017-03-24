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
    }

    public class Meeting
    {
        public Meeting()
        {
            Location = Airport.FindByCode( "LHR" );
        }

        public Airport Location { get; private set; }

    }
}
