using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algo.Optim
{
    public abstract class SolutionSpace
    {
        readonly Random _random;

        protected SolutionSpace(int seed )
        {
            _random = new Random(seed);
        }

        protected void Initialize(int[] cardinalities)
        {
            Cardinalities = cardinalities;
        }

        public int Dimension => Cardinalities.Length;

        public int[] Cardinalities { get; private set; }

        public SolutionInstance GetRandomInstance()
        {
            int[] coord = new int[Dimension];
            for(int i = 0; i < Dimension; ++i )
            {
                coord[i] = _random.Next(Cardinalities[i]);
            }
            return CreateSolutionInstance(coord);
        }

        protected abstract SolutionInstance CreateSolutionInstance(int[] coord);
    }
}
