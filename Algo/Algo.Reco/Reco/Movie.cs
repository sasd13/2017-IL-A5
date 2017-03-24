using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Algo
{
    public partial class Movie
    {
        int _movieId;
        string _title;
        string[] _categories;
        
        /// <summary>
        /// Movie information is in the file "movies.dat" and is in the following
        /// format: MovieID::Title::Genres
        /// </summary>
        /// <param name="line"></param>
        Movie( string line )
        {
            string[] cells = line.Split( User.CellSeparator, StringSplitOptions.None );
            _movieId = Int32.Parse( cells[0] );
            _title = cells[1];
            _categories = cells[2].Split( '|' ).Select( s => String.Intern( s ) ).ToArray();
        }

        static public Movie[] ReadMovies( string path )
        {
            List<Movie> u = new List<Movie>();
            using( TextReader r = File.OpenText( path ) )
            {
                string line;
                while( (line = r.ReadLine()) != null ) u.Add( new Movie( line ) );
            }
            return u.ToArray();
        }

        public int MovieID { get { return _movieId; } set { _movieId = value; } }

        public string Title { get { return _title; } }
        
        public string[] Categories { get { return _categories; } }

    }
    
}
