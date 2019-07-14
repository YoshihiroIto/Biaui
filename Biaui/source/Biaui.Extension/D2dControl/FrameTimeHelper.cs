using System.Collections.Generic;
using System.Linq;

namespace D2dControl
{
    internal class FrameTimeHelper
    {
        private readonly int depth;
        private readonly Queue<double> queue;

        public FrameTimeHelper(int depth)
        {
            this.depth = depth;
            queue = new Queue<double>(this.depth + 1);
        }

        public double Push(double item)
        {
            queue.Enqueue(item);

            if (queue.Count > depth)
                queue.Dequeue();

            return queue.Average();
        }
    }
}