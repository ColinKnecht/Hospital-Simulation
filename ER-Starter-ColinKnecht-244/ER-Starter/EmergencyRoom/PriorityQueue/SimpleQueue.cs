using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// COLIN KNECHT -- FINAL PROGRAM -- CPT 244
/// </summary>
namespace PriorityQueue
{
    public class SimpleQueue<T> : Queue<T>, IQueue<T>
    {
        public void Add(int priority, T element)
        {
            Enqueue(element);
        }

        public T Remove()
        {
            return Dequeue();
        }
    }
}
