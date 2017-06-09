using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algo.Optim
{
    public abstract class SolutionInstance
    {
        readonly SolutionSpace _space;
        double _cost = -1.0;

        protected SolutionInstance( SolutionSpace space, int[] coord )
        {
            _space = space;
            Coordinates = coord;
        }

        public SolutionSpace Space => _space;

        public SolutionInstance FindBestAround()
        {
            return this;
        }

        public int[] Coordinates { get; }

        public double Cost => _cost >= 0 ? _cost : (_cost = ComputeCost());

        double ComputeCost()
        {
            double c = DoComputeCost();
            if (_space.BestSolution == null || c < _space.BestSolution.Cost)
            {
                _space.BestSolution = this;
            }
            if (_space.WorstSolution == null || c > _space.WorstSolution.Cost)
            {
                _space.WorstSolution = this;
            }
            return c;
        }

        public IEnumerable<SolutionInstance> MonteCarloPath()
        {
            SolutionInstance last = this;
            for(;;)
            {
                yield return last;
                var best = last.BestAmongNeighbors;
                if (best == last) break;
            }
        }

        SolutionInstance BestAmongNeighbors
        {
            get
            {
                SolutionInstance best = this;
                foreach( var n in Neighbors )
                {
                    if (n.Cost < best.Cost) best = n;
                }
                return best;
            }
        }

        public IEnumerable<SolutionInstance> Neighbors
        {
            get
            {
                for( int i = 0; i < _space.Dimension; ++i )
                {
                    int prevValue = Coordinates[i] - 1;
                    if( prevValue >= 0 )
                    {
                        int[] prevCoords = (int[])Coordinates.Clone();
                        prevCoords[i] = prevValue;
                        yield return _space.CreateSolutionInstance(prevCoords);
                    }
                    int nextValue = Coordinates[i] + 1;
                    if( nextValue < _space.Cardinalities[i] )
                    {
                        int[] nextCoords = (int[])Coordinates.Clone();
                        nextCoords[i] = nextValue;
                        yield return _space.CreateSolutionInstance(nextCoords);
                    }
                }
            }
        }

        protected abstract double DoComputeCost();
    }
}
