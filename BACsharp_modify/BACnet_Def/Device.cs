using System;
using System.Collections.Generic;
using System.Net;

namespace BACsharp.BACnet_Def
{
    /// <summary>
    /// from BACnet.cs
    /// </summary>
    public class Device
    {
        public string Name { get; set; }
        public int VendorID { get; set; }
        public IPEndPoint ServerEP { get; set; }
        public int Network { get; set; }
        public byte SourceLength { get; set; }
        public UInt32 Instance { get; set; }
        public UInt32 MACAddress { get; set; }
        
        public Device()
        {
            Name = "(no name)";
            VendorID = 0;
            ServerEP = null;
            Network = 0;
            SourceLength = 0;
            Instance = 0;
            MACAddress = 0;
        }

        public Device(string name, int vendorid, byte slen, IPEndPoint server, int network, UInt32 instance)
        {
            this.Name = name;
            this.VendorID = vendorid;
            this.SourceLength = slen;
            this.ServerEP = server;
            this.Network = network;
            this.Instance = instance;
        }
        
        public override string ToString()
        {
            return this.Name;
        }

    }
}
