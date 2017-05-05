using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algo.Optim
{
    public class MeetingInstance : SolutionInstance
    {
        public MeetingInstance(Meeting m, int[] coord)
            : base( m, coord )
        {
        }

        protected override double DoComputeCost()
        {
            return 0.0;
        }
    }
}
