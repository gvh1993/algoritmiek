using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntryPoint
{
    class Node
    {
        public Node left { get; set; }
        public Node right { get; set; }
        public Vector2 value { get; set; }
        public Node(Vector2 value)
        {
            this.value = value;
        }

    }
}
