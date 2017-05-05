using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Algo
{

    public partial class User
    {
        internal static string[] CellSeparator = new string[] { "::" };

        UInt16 _userId;
        byte _age;
        bool _male;
        string _occupation;
        string _zipCode;
        
        /// <summary>
        /// User information is in the file "users.dat" and is in the following
        /// format:
        /// UserID::Gender::Age::Occupation::Zip-code
        /// </summary>
        /// <param name="line"></param>
        User( string line )
        {
            string[] cells = line.Split( CellSeparator, StringSplitOptions.None );
            _userId = UInt16.Parse( cells[0] );
            _male = cells[1] == "M";
            _age = Byte.Parse( cells[2] );
            _occupation = String.Intern( cells[3] );
            _zipCode = String.Intern( cells[4] );
            Ratings = new Dictionary<Movie, int>();
        }

        static public User[] ReadUsers( string path )
        {
            List<User> u = new List<User>();
            using( TextReader r = File.OpenText( path ) )
            {
                string line;
                while( (line = r.ReadLine()) != null ) u.Add( new User( line ) );
            }
            return u.ToArray();
        }

        static public int ReadRatings( User[] users, Movie[] movies, string path )
        {
            int count = 0;
            using( TextReader r = File.OpenText( path ) )
            {
                string line;
                while( (line = r.ReadLine()) != null )
                {
                    string[] cells = line.Split( CellSeparator, StringSplitOptions.None );
                    int idUser = int.Parse( cells[0] ) - 1;
                    int idMovie = int.Parse( cells[1] ) - 1;
                    Debug.Assert(idMovie >= 0 && idMovie < movies.Length);
                    Debug.Assert(idUser >= 0 && idUser < users.Length);
                    users[idUser].Ratings.Add( movies[idMovie], int.Parse( cells[2] ) );
                    ++count;
                }
            }
            return count;
        }

        public int UserID { get { return (int)_userId; } set { _userId = (UInt16)value; } }

        public bool Male { get { return _male; } }

        public int Age { get { return (int)_age; } }
        
        public string Occupation { get { return _occupation; } }

        public string ZipCode { get { return _zipCode; } }

        public Dictionary<Movie, int> Ratings { get; private set; }

    }
    

}
