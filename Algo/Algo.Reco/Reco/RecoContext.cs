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
