using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLOR_Demo.DataClasses
{
    public class PacketClass
    {
        public enum PacketType
        {
            Message_Type, Acknowledgement_Type
        }

        public AddressClass destinationAddress { get; set; }
        public AddressClass sourceAddress { get; set; }
        public AddressClass originAddress { get; set; }
        public String message { get; set; }
        public PacketType packetType { get; set; }
        public PacketClass(AddressClass destinationAddress, AddressClass sourceAddress,
                            AddressClass originAddress, String message, PacketType packetType)
        {
            this.destinationAddress = destinationAddress;
            this.sourceAddress = sourceAddress;
            this.originAddress = originAddress;
            this.message = message;
            this.packetType = packetType;
        }


    }
}
