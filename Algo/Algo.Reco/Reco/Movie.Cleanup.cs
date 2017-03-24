using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Algo
{
    public partial class Movie
    {
        static public void ReadMovies( string path, out Dictionary<int, Movie> first, out Dictionary<int, List<Movie>> duplicates )
        {
            first = new Dictionary<int, Movie>();
            duplicates = new Dictionary<int, List<Movie>>();
            using( TextReader r = File.OpenText( path ) )
            {
                string line;
                while( (line = r.ReadLine()) != null )
                {
                    Movie exists, u = new Movie( line );
                    if( first.TryGetValue( u.MovieID, out exists ) )
                    {
                        List<Movie> list;
                        if( !duplicates.TryGetValue( u.MovieID, out list ) )
                        {
                            list = new List<Movie>();
                            duplicates.Add( u.MovieID, list );
                        }
                        list.Add( u );
                    }
                    else first.Add( u.MovieID, u );
                }
            }
        }
    }
    
}
