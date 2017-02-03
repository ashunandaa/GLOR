using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLOR_Demo.CustomControl;

namespace GLOR_Demo.DataClasses
{
    public class WebRegister
    {
        public List<NodeButton> nodeData { get; set; }

        public void addNode(NodeButton node)
        {
            if (nodeData == null)
                nodeData = new List<NodeButton>();

            nodeData.Add(node);
        }
    }
}
