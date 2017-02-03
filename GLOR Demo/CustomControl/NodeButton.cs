using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

using GLOR_Demo.DataClasses;
using GLOR_Demo.Drawing;

namespace GLOR_Demo.CustomControl
{
    public class NodeButton : Button
    {
        public int nodeID { get; set; }
        public AddressClass nodeAddress { get; set; }
        public List<NodeButton> neighbouringNodes { get; set; }
        public Panel drawingPanel { get; set; }

        public delegate void PacketReached(NodeButton nodeBtn, PacketClass packet);
        public event PacketReached OnPacketReachingDestination;

        public delegate void RouteStep(NodeButton nodeBtn, PacketClass packet);
        public event RouteStep OnManualStep;

        //For making round buttons
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            GraphicsPath grPath = new GraphicsPath();
            grPath.AddEllipse(0, 0, ClientSize.Width, ClientSize.Height);
            this.Region = new System.Drawing.Region(grPath);
            base.OnPaint(e);
        }

        //Initialize NodeButton with parameters
        public NodeButton(int nodeID, AddressClass nodeAddress, Panel routingPanel)
        {
            this.nodeID = nodeID;
            this.nodeAddress = nodeAddress;
            this.drawingPanel = routingPanel;
            this.Location = new Point(nodeAddress.x, nodeAddress.y);
            this.Size = new Size(50, 50);
            this.FlatStyle = FlatStyle.Flat;
            this.BackColor = Color.Transparent;
            this.FlatAppearance.BorderSize = 0;
            this.TextImageRelation = TextImageRelation.ImageAboveText;

            //Set random image from ImageList
            int imageIndex = GlobalVariable.RandomNumber(0, 4);
            this.Image = GlobalVariable.imageList[imageIndex];

            routingPanel.Controls.Add(this);
        }

        //Drawing line on panel
        private void drawLine(RouteLine line)
        {
            Graphics G = drawingPanel.CreateGraphics();
            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            G.DrawLine(line.pen, line.firstPoint, line.lastPoint);
        }


        //Routing Methods
        public void receivePacket(PacketClass packet)
        {
            int btnSizeMargin = 30;

            Point firstPoint = new Point(packet.originAddress.x + btnSizeMargin, packet.originAddress.y + btnSizeMargin);
            Point secondPoint = new Point(this.nodeAddress.x + btnSizeMargin, this.nodeAddress.y + btnSizeMargin);
            Point destinationPoint = new Point(packet.destinationAddress.x + btnSizeMargin, packet.destinationAddress.y + btnSizeMargin);

            if (packet.packetType == PacketClass.PacketType.Acknowledgement_Type)
            {
                //Line between two pens with custom pen: color + Dashed/Solid
                RouteLine line = new RouteLine(firstPoint, secondPoint, DrawingPen.Pen.redDashedPen);
                drawLine(line);

                if(GlobalVariable.IsBeelineEnabled)
                {
                    RouteLine beeline = new RouteLine(firstPoint, destinationPoint, DrawingPen.Pen.blackDashedPen);
                    drawLine(beeline);
                }
            }
            else
            {
                RouteLine line = new RouteLine(firstPoint, secondPoint, DrawingPen.Pen.orangeDashedPen);
                drawLine(line);

                if (GlobalVariable.IsBeelineEnabled)
                {
                    RouteLine beeline = new RouteLine(firstPoint, destinationPoint, DrawingPen.Pen.blueSolidPen);
                    drawLine(beeline);
                }
            }

            Console.WriteLine("Receiving packet at Node " + this.nodeID + " From OriginAddress x:" + packet.originAddress.x);
            packet.originAddress = this.nodeAddress;

            if (GlobalVariable.IsManual)
            {
                //Manual Routing
                if (packet.destinationAddress != this.nodeAddress)
                {
                    //Step by step route until reaches destination
                    OnManualStep(this, packet);
                }
                else
                {
                    OnPacketReachingDestination(this, packet);

                    if (packet.packetType == PacketClass.PacketType.Message_Type)
                    {
                        packet = new PacketClass(packet.sourceAddress, packet.destinationAddress, this.nodeAddress, GlobalVariable.KVAcknowledgment, PacketClass.PacketType.Acknowledgement_Type);
                        OnManualStep(this, packet);

                        //Resend packet to route acknowledgement
                    }
                }
  
            }
            else
            {
                //Automatic Routing
                if (packet.destinationAddress != this.nodeAddress)
                {
                    //Route until reaches destination
                    sendPacket(packet);
                }
                else
                {
                    OnPacketReachingDestination(this, packet);
                    
                    if (packet.packetType == PacketClass.PacketType.Message_Type)
                    {
                        packet = new PacketClass(packet.sourceAddress, packet.destinationAddress, this.nodeAddress, GlobalVariable.KVAcknowledgment, PacketClass.PacketType.Acknowledgement_Type);
                        this.sendPacket(packet);

                        //Resend packet to route acknowledgement
                    }
                }
            }
        }

        public void sendPacket(PacketClass packet)
        {
            Console.WriteLine("Sending packet...");

            NodeButton nextNode = findNextNode(packet.destinationAddress);

            if (nextNode != null)
            {
                nextNode.receivePacket(packet);
            }
            else
            {
                Console.WriteLine("Next Node not found!");
            }
        }

        public NodeButton findNextNode(AddressClass destinationAddress)
        {
            int x1 = this.nodeAddress.x;
            int y1 = this.nodeAddress.y;

            int x2 = destinationAddress.x;
            int y2 = destinationAddress.y;

            NodeButton closestNode = null;

            double distance = -1;
            List<NodeButton> towardsDestination = returnNodesTowardsDestination(destinationAddress);

            foreach (NodeButton node in towardsDestination)
            {
                AddressClass nodeAddress = node.nodeAddress;

                double x0 = nodeAddress.x;
                double y0 = nodeAddress.y;

                double x2_x1 = x2 - x1;
                double y1_y0 = y1 - y0;

                double x1_x0 = x1 - x0;
                double y2_y1 = y2 - y1;

                double numerator = System.Math.Abs(x2_x1 * y1_y0 - x1_x0 * y2_y1);

                double denominator = System.Math.Sqrt(x2_x1 * x2_x1 + y2_y1 * y2_y1);

                double tempDistance = numerator / denominator;

                if (distance == -1)
                {
                    closestNode = node;
                    distance = tempDistance;
                }
                else if (tempDistance < distance)
                {
                    closestNode = node;
                    distance = tempDistance;
                }
            }

            Console.WriteLine("Least Distance: " + distance);
            return closestNode;
        }

        private List<NodeButton> returnNodesTowardsDestination(AddressClass destination)
        {
            int x1 = this.nodeAddress.x;
            int y1 = this.nodeAddress.y;

            int x2 = destination.x;
            int y2 = destination.y;

            double currentNodeDistance = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
            Console.WriteLine("Current Distance :" + currentNodeDistance + " Of Node: " + this.nodeID);

            List<NodeButton> towardsDestination = new List<NodeButton>();

            foreach (NodeButton node in this.neighbouringNodes)
            {
                int x3 = node.nodeAddress.x;
                int y3 = node.nodeAddress.y;

                double dist = Math.Sqrt(Math.Pow(x2 - x3, 2) + Math.Pow(y2 - y3, 2));
                Console.WriteLine("Neighbour Distance :" + dist + " Of Node: " + node.nodeID);

                if (dist <= currentNodeDistance)
                {
                    towardsDestination.Add(node);
                }
            }

            return towardsDestination;
        }
    }
}
