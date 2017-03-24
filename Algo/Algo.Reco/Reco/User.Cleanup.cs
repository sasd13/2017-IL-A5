using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Algo
{
    public partial class User
    {
        static public void ReadUsers( string path, out Dictionary<int,User> first, out Dictionary<int,List<User>> duplicates )
        {
            first = new Dictionary<int, User>();
            duplicates = new Dictionary<int, List<User>>();
            using( TextReader r = File.OpenText( path ) )
            {
                string line;
                while( (line = r.ReadLine()) != null )
                {
                    User exists, u = new User( line );
                    if( first.TryGetValue( u.UserID, out exists ) )
                    {
                        List<User> list;
                        if( !duplicates.TryGetValue( u.UserID, out list ) ) 
                        {
                            list = new List<User>();
                            duplicates.Add( u.UserID, list );
                        }
                        list.Add( u );
                    }
                    else first.Add( u.UserID, u );
                }
            }
        }

        static public int ReadRatings( string path, Dictionary<int,User> users, Dictionary<int,Movie> movies, out Dictionary<int,string> badLines )
        {
            int count = 0;
            badLines = new Dictionary<int, string>();
            using( TextReader r = File.OpenText( path ) )
            {
                int numLine = 0;
                string line;
                while( (line = r.ReadLine()) != null )
                {
                    string[] cells = line.Split( CellSeparator, StringSplitOptions.None );
                    bool badLine = true;
                    int idUser = 0;
                    int idMovie = 0;
                    int ranking = 0;
                    if( int.TryParse( cells[0], out idUser ) 
                        && int.TryParse( cells[1], out idMovie )
                        && int.TryParse( cells[2], out ranking ) )
                    {
                        User u;
                        Movie m;
                        users.TryGetValue( idUser, out u );
                        movies.TryGetValue( idMovie, out m );
                        if( u != null && m != null )
                        {
                            ++count;
                            badLine = false;
                            u.Ratings[m] = ranking;
                        }
                    }
                    if( badLine ) badLines.Add( numLine, line );
                    ++numLine;
                }
            }
            return count;
        }

    }

}
