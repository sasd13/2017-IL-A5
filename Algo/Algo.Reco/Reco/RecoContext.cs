using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Algo
{
    public class RecoContext
    {
        public User[] Users { get; private set; }
        public Movie[] Movies { get; private set; }

        public void LoadFrom( string folder )
        {
            Users = User.ReadUsers( Path.Combine( folder, "users.dat" ) );
            Movies = Movie.ReadMovies( Path.Combine( folder, "movies.dat" ) );
            User.ReadRatings( Users, Movies, Path.Combine( folder, "ratings.dat" ) );
        }
    }
}
