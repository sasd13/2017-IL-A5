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

        public int[] Coordinates { get; }

        public double Cost => _cost >= 0 ? _cost : (_cost = ComputeCost());

        protected abstract double ComputeCost();
    }
}
