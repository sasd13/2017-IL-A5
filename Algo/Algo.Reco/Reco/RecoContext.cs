using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Algo
{
    public class RecoContext
    {
        static string path = @"C:\Users\ssaidali2\Projects\Intech\2017-IL-A5\Algo\ThirdParty\MovieData\";

        public User[] Users { get; private set; }
        public Movie[] Movies { get; private set; }

        public void LoadFrom( string folder )
        {
            Users = User.ReadUsers( Path.Combine( folder, "users.dat" ) );
            Movies = Movie.ReadMovies( Path.Combine( folder, "movies.dat" ) );
            User.ReadRatings( Users, Movies, Path.Combine( folder, "ratings.dat" ) );
        }

        public double DistNorm2( User u1, User u2 )
        {
            var squareDeltas = u1.Ratings.Select(mr1 => new
                        {
                            R1 = mr1.Value,
                            R2 = u2.Ratings.GetValueWithDefault(mr1.Key, -1)
                        })
                        .Where(r1r2 => r1r2.R2 >= 0)
                        .Select(r1r2 => r1r2.R1 - r1r2.R2)
                        .Select(delta => delta * delta);
            // By considering the users as being the same, we boost
            // the movies loved the most by all users.
            return squareDeltas.Any()
                    ? Math.Sqrt(squareDeltas.Sum())
                    : 0.0;
        }

        public double SimilarityNorm2(User u1, User u2)
        {
            return 1 / (1 + DistNorm2(u1, u2));
        }

        public double SimilarityPearson(User u1, User u2)
        {
            IEnumerable<Movie> common = u1.Ratings.Keys.Intersect(u2.Ratings.Keys);
            return SimilarityPearson( common.Select( m => new KeyValuePair<int,int>( u1.Ratings[m], u2.Ratings[m] ) ) );
        }

        static public double SimilarityPearson(params int[] values)
        {
            if(values == null || (values.Length & 1) != 0) throw new ArgumentException();
            return SimilarityPearson(Convert(values));
        }

        static IEnumerable<KeyValuePair<int,int>> Convert(int[] values)
        {
            Debug.Assert(values != null && (values.Length & 1) == 0);
            for (int i = 0; i < values.Length; i++)
            {
                yield return new KeyValuePair<int, int>(values[i], values[++i]);
            }
        }

        static public double SimilarityPearson(IEnumerable<int> v1, IEnumerable<int> v2)
        {
            return SimilarityPearson(v1.Zip(v2, (x, y) => new KeyValuePair<int, int>(x, y)));
        }

        static public double SimilarityPearson(IEnumerable<KeyValuePair<int,int>> values )
        {
            double sumX = 0.0;
            double sumY = 0.0;
            double sumXY = 0.0;
            double sumX2 = 0.0;
            double sumY2 = 0.0;

            int count = 0;
            foreach (var m in values)
            {
                count++;
                int x = m.Key;
                int y = m.Value;
                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumX2 += x * x;
                sumY2 += y * y;
            }
            if (count == 0) return 0.0;
            if( count == 1 )
            {
                var onlyOne = values.Single();
                double d = Math.Abs(onlyOne.Key - onlyOne.Value);
                return 1 / (1 + d);
            }
            checked
            {
                double numerator = sumXY - (sumX * sumY / count);
                double denumerator1 = sumX2 - (sumX * sumX / count);
                double denumerator2 = sumY2 - (sumY * sumY / count);
                return numerator / Math.Sqrt(denumerator1 * denumerator2);
            }
        }

        public IEnumerable<Movie> getBest(User u, int max)
        {
            List<Movie> results = new List<Movie>();

            List<KeyValuePair<User, double>> similarities = new List<KeyValuePair<User, double>>();

            foreach (User user in Users)
            {
                if (!user.UserID.Equals(u.UserID))
                {
                    similarities.Add(new KeyValuePair<User, double>(user, SimilarityPearson(u, user)));
                }
            }

            Dictionary<Movie, Coeff> moviesMayLiked = new Dictionary<Movie, Coeff>();
            Dictionary<Movie, int> moviesNotSeen;
            Coeff like;

            foreach (KeyValuePair<User, double> kvp in similarities)
            {
                moviesNotSeen = getNotSeen(u, kvp.Key);
                foreach (KeyValuePair<Movie, int> movieNotSeen in moviesNotSeen)
                {
                    like = moviesMayLiked.GetValueWithDefault(movieNotSeen.Key, null);

                    if (like == null)
                    {
                        moviesMayLiked.Add(movieNotSeen.Key, new Coeff(movieNotSeen.Value, kvp.Value));
                    }
                    else
                    {
                        like.Rating += movieNotSeen.Value;
                        like.Distance += kvp.Value;
                        like.Count++;
                    }
                }
            }

            IOrderedEnumerable<KeyValuePair<Movie, Coeff>> orderedEnum = moviesMayLiked.OrderByDescending(e => e.Value.Rating/e.Value.Distance);
            /*
             * pourquoi e.Value.Rating / e.Value.Distance ?
             * 
             * e.Value.Rating / e.Value.Count => rating moyen
             * e.Value.Distance / e.Value.Count => distance moyenne
             * le but est de classer les films selon par rating moyen decroissant et par distance moyenne croissante => quotient = e.Value.Rating / e.Value.Coeff
             * 
             */
            List<KeyValuePair<Movie, Coeff>> finalEnum = orderedEnum.ToList();
            
            foreach (KeyValuePair<Movie, Coeff> kvp in finalEnum)
            {
                results.Add(kvp.Key);
            }

            return results.Count >= max ? results.GetRange(0, max) : results;
        }

        public Dictionary<Movie, int> getNotSeen(User u1, User u2)
        {
            Dictionary<Movie, int> results = new Dictionary<Movie, int>();

            IEnumerable<Movie> distincts = u2.Ratings.Keys.Except(u1.Ratings.Keys);

            foreach (Movie movie in distincts)
            {
                if (u2.Ratings[movie] >= 3)
                {
                    results.Add(movie, u2.Ratings[movie]);
                }
            }

            return results;
        }
    }

    /*
     * Coefficient du film en tenant compte de 3 paramètres : la notation, la distance et le nombre de vues
     */
    public class Coeff
    {
        public int Rating { get; set; }
        public double Distance { get; set; }
        public int Count { get; set; }

        public Coeff(int rating, double distance)
        {
            Rating = rating;
            Distance = distance;
        }
    }

    public static class DictionaryExtension
    {
        public static TValue GetValueWithDefault<TKey, TValue>( this Dictionary<TKey,TValue> @this, TKey key, TValue def )
        {
            TValue v;
            return @this.TryGetValue(key, out v) ? v : def;
        }
    }
}
