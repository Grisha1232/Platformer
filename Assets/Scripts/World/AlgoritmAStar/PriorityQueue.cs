using System.Collections.Generic;
using UnityEngine;

namespace AlgoritmAStar
{
    public class Node : System.IComparable<Node>
    {
        public Vector3Int Position;
        public int G; // Цена пути от старта
        public int F; // G + эвристика

        public Node(Vector3Int pos, int g, int f)
        {
            Position = pos;
            G = g;
            F = f;
        }

        public int CompareTo(Node other)
        {
            return F.CompareTo(other.F);
        }
    }
    public class PriorityQueue<T> where T : System.IComparable<T>
    {
        private List<T> data = new List<T>();

        public void Enqueue(T item)
        {
            data.Add(item);
            int ci = data.Count - 1;

            while (ci > 0)
            {
                int pi = (ci - 1) / 2;

                if (data[ci].CompareTo(data[pi]) >= 0)
                    break;

                (data[ci], data[pi]) = (data[pi], data[ci]);
                ci = pi;
            }
        }

        public T Dequeue()
        {
            int li = data.Count - 1;
            T frontItem = data[0];

            data[0] = data[li];
            data.RemoveAt(li);
            li--;

            int pi = 0;

            while (true)
            {
                int ci = pi * 2 + 1;
                if (ci > li) break;
                int rc = ci + 1;

                if (rc <= li && data[rc].CompareTo(data[ci]) < 0)
                    ci = rc;

                if (data[pi].CompareTo(data[ci]) <= 0)
                    break;

                (data[pi], data[ci]) = (data[ci], data[pi]);
                pi = ci;
            }

            return frontItem;
        }

        public int Count => data.Count;
    }
}