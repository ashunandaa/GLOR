using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

using GLOR_Demo.DataClasses;
using GLOR_Demo.CustomControl;
using GLOR_Demo.Drawing;

namespace GLOR_Demo
{
    public partial class GLOR : Form
    {
        //Running functions on background thread
        BackgroundWorker backgroundWorker;

        //Keeping node data
        WebRegister register;

        //source and destination nodes
        NodeButton sourceNode, destinationNode;

        //packet info
        PacketClass packet;

        //keep track of routing
        bool isRoutingInProgress;

        public PacketClass.PacketType Message_Type { get; private set; }

        public GLOR()
        {
            InitializeComponent();
        }

        private void GLOR_Load(object sender, EventArgs e)
        {
            isRoutingInProgress = false;
            initGlobalVariable();
            initWebRegister();
            updateRoutingButton();

            initBackgroundWorker();
        }

        private void initGlobalVariable()
        {
            GlobalVariable.IsManual = false;
            GlobalVariable.IsBeelineEnabled = false; //Make it true for showing beeline

            Image laptop = GLOR_Demo.Properties.Resources.laptop;
            Image mobile1 = GLOR_Demo.Properties.Resources.mobile1;
            Image mobile2 = GLOR_Demo.Properties.Resources.mobile2;
            Image mobile3 = GLOR_Demo.Properties.Resources.mobile3;

            GlobalVariable.imageList = new List<Image>();
            GlobalVariable.imageList.Add(laptop);
            GlobalVariable.imageList.Add(mobile1);
            GlobalVariable.imageList.Add(mobile2);
            GlobalVariable.imageList.Add(mobile3);
        }

        private void initBackgroundWorker()
        {
            backgroundWorker = new BackgroundWorker();

            // Create a background worker thread 
            // Hook up the appropriate events.
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler
                    (backgroundWorker_Completed);
        }

        private void initWebRegister()
        {
            //Initializing nodes and adding buttons accordingly

            List<AddressClass> nodeAddresses = returnNodeAddresses();

            int nodeID = 0;

            register = new WebRegister();

            foreach (AddressClass address in nodeAddresses)
            {
                NodeButton node = new NodeButton(nodeID, address, this.panel1);

                node.OnPacketReachingDestination += OnPacketReachingDestination;
                node.OnManualStep += OnManualStep;
                node.Click += new EventHandler(node_Click);
                
                nodeID++;

                register.addNode(node);
            }

            findNeighbouringNodes();
        }

        private List<AddressClass> returnNodeAddresses()
        {
            List<AddressClass> nodeAddresses = new List<AddressClass>();

            Random rnd = new Random();

            int cellSize = 100;
            
            int horCells = panel1.Width/cellSize;
            int verCells = panel1.Height/cellSize;

            for (int i = 0; i < horCells; i++)
            {
                for (int j = 0; j < verCells; j++)
                {
                    int minX = i * cellSize;
                    int maxX = minX + cellSize;

                    int x;
                    do
                    {
                        x = rnd.Next(minX, maxX);
                    }
                    while (x > i * cellSize + cellSize/2); //To keep buttons away from each other

                    int minY = j * cellSize;
                    int maxY = minY + cellSize;

                    int y;

                    do
                    {
                        y = rnd.Next(minY, maxY);
                    }
                    while (y > j*cellSize + cellSize / 2);//To keep buttons away from each other

                    AddressClass address = new AddressClass(x, y);
                    nodeAddresses.Add(address);
                }
            }

            return nodeAddresses;
        }


        private void findNeighbouringNodes()
        {
            //Automatically add neighbouring nodes to node objects
            foreach (NodeButton node in register.nodeData)
            {
                node.neighbouringNodes = new List<NodeButton>();

                foreach (NodeButton node2 in register.nodeData)
                {
                    if (node.nodeID != node2.nodeID)
                    {
                        int x1 = node.nodeAddress.x;
                        int y1 = node.nodeAddress.y;

                        int x2 = node2.nodeAddress.x;
                        int y2 = node2.nodeAddress.y;

                        double dist = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

                        if (dist <= 150)
                        {
                            node.neighbouringNodes.Add(node2);
                        }
                    }
                }
            }
        }

        //Button events
        private void btnStart_Click(object sender, EventArgs e)
        {
            //Set routing mode: Automatic
            GlobalVariable.IsManual = false;

            //Disable start button
            btnStart.Enabled = false;
            btnStep.Enabled = false;

            //Start backgroundWorker
            backgroundWorker.RunWorkerAsync();
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            //Set routing mode: Manual
            GlobalVariable.IsManual = true;
            if(!isRoutingInProgress)
            {
                isRoutingInProgress = true;
                packet = new PacketClass(destinationNode.nodeAddress, sourceNode.nodeAddress, sourceNode.nodeAddress, GlobalVariable.KVMessage, Message_Type);
            }

            sourceNode.sendPacket(packet);
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            //Clear drawing and controls
            this.panel1.Controls.Clear();
            this.panel1.Invalidate();

            isRoutingInProgress = false;
            sourceNode = null;
            destinationNode = null;

            initWebRegister();
            updateRoutingButton();
        }

        //OtherMethods
        protected void initiateRouting()
        {
            if (!isRoutingInProgress)
            {
                isRoutingInProgress = true;
                packet = new PacketClass(destinationNode.nodeAddress, sourceNode.nodeAddress, sourceNode.nodeAddress, GlobalVariable.KVMessage, Message_Type);
            }

            sourceNode.sendPacket(packet);
        }

        private void updateRoutingButton()
        {
            if(sourceNode != null && destinationNode != null)
            {
                this.btnStart.Enabled = true;
                this.btnStep.Enabled = true;
            }
            else
            {
                this.btnStart.Enabled = false;
                this.btnStep.Enabled = false;
            }
        }

        //Background Worker events
        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            initiateRouting();
        }

        void backgroundWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            isRoutingInProgress = false;
            sourceNode = null;
            destinationNode = null;

            updateRoutingButton();
        }

        // Node events
        private void node_Click(object sender, EventArgs e)
        {
            NodeButton node = sender as NodeButton;

            if (sourceNode == null)
            {
                sourceNode = node;
                sourceNode.BackColor = Color.LightSteelBlue;
            }
            else if (destinationNode == null)
            {
                destinationNode = node;
                destinationNode.BackColor = Color.LightSteelBlue;
            }

            updateRoutingButton();
        }


        private void OnManualStep(NodeButton node, PacketClass packet)
        {
            sourceNode = node;
            this.packet = packet;
        }

        private void OnPacketReachingDestination(NodeButton node, PacketClass packet)
        {
            // Its another thread so invoke back to UI thread
            base.Invoke((Action)delegate
            {
                node.BackColor = Color.LightGreen;
                node.Text = packet.message;

                if(GlobalVariable.IsManual && packet.packetType == PacketClass.PacketType.Acknowledgement_Type)
                {
                    isRoutingInProgress = false;
                    sourceNode = null;
                    destinationNode = null;

                    updateRoutingButton();
                }

            });
        }
    }
}
