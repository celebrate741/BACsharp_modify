using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace BACsharp.BACnet_Def
{
    public class SimpleReadWrite
    {
        private int _UDPPort_local = 0;
        private int _UDPPort_dest = 47812;
        bool TimerDone = false;
        public int InvokeCounter = 0;

        UdpClient UDPClient = null;
        public IPEndPoint LocalEP = null;
        public IPEndPoint BroadcastEP = null;

        public delegate void logEventHandler(string txt);
        public logEventHandler LogEvent = null;
        private void Log(string txt)
        {
            if (LogEvent != null) LogEvent(txt);
        }
        public SimpleReadWrite(logEventHandler log_Event,int local_port,int dest_port)
        {
            _UDPPort_local = local_port;
            _UDPPort_dest = dest_port;
            constructure(log_Event);
        }
        public SimpleReadWrite(logEventHandler log_Event)
        {
            constructure(log_Event);
        }
        private void constructure(logEventHandler log_Event)
        {
            if (log_Event != null)
                LogEvent = log_Event;
            byte[] maskbytes = new byte[4];
            byte[] addrbytes = new byte[4];

            // Find the local IP address and Subnet Mask
            NetworkInterface[] Interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface Interface in Interfaces)
            {
                if (Interface.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;
                //MessageBox.Show(Interface.Description);
                UnicastIPAddressInformationCollection UnicastIPInfoCol = Interface.GetIPProperties().UnicastAddresses;
                foreach (UnicastIPAddressInformation UnicatIPInfo in UnicastIPInfoCol)
                {
                    //MessageBox.Show("\tIP Address is {0}" + UnicatIPInfo.Address);
                    //MessageBox.Show("\tSubnet Mask is {0}" + UnicatIPInfo.IPv4Mask);
                    if (UnicatIPInfo.IPv4Mask != null)
                    {
                        byte[] tempbytes = UnicatIPInfo.IPv4Mask.GetAddressBytes();
                        if (tempbytes[0] == 255)
                        {
                            // We found the correct subnet mask, and probably the correct IP address
                            addrbytes = UnicatIPInfo.Address.GetAddressBytes();
                            maskbytes = UnicatIPInfo.IPv4Mask.GetAddressBytes();
                            break;
                        }
                    }
                }
            }
            // Set up broadcast address
            if (maskbytes[3] == 0) maskbytes[3] = 255; else maskbytes[3] = addrbytes[3];
            if (maskbytes[2] == 0) maskbytes[2] = 255; else maskbytes[2] = addrbytes[2];
            if (maskbytes[1] == 0) maskbytes[1] = 255; else maskbytes[1] = addrbytes[1];
            if (maskbytes[0] == 0) maskbytes[0] = 255; else maskbytes[0] = addrbytes[0];

            IPAddress myip = new IPAddress(addrbytes);
            IPAddress broadcast = new IPAddress(maskbytes);

            LocalEP = new IPEndPoint(myip, UDPPort_local);
            BroadcastEP = new IPEndPoint(broadcast, UDPPort_dest);

            UDPClient = new UdpClient();
            UDPClient.ExclusiveAddressUse = false;
            UDPClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            UDPClient.Client.Bind(LocalEP);
        }

        /// <summary>
        /// set local udp port and initialize or get local udp port
        /// </summary>
        public int UDPPort_local
        {
            get { return _UDPPort_local; }
            set
            {
                _UDPPort_local = value;
                Log("change local udp port to " + value + "\n");
                LocalEP.Port = value;
                BroadcastEP.Port = value;

                Log("Initialize\n");
                if (UDPClient != null)
                {
                    UDPClient.Client.Close();
                    UDPClient.Close();
                    UDPClient = null;
                }
                UDPClient = new UdpClient();
                UDPClient.ExclusiveAddressUse = false;
                UDPClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                UDPClient.Client.Bind(LocalEP);
            }
        }
        /// <summary>
        /// set dest udp port and initialize or get dest udp port
        /// </summary>
        public int UDPPort_dest
        {
            get { return _UDPPort_dest; }
            set
            {
                _UDPPort_dest = value;
                Log("change dest udp port to " + value + "\n");
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimerDone = true;
        }

        public List<Device> GetDevice(int milliseconds, int low_limit,int high_limit)
        {
            Log("find device from " + low_limit + " to " + high_limit 
                + "(" + milliseconds/1000 + " sec)\n");
            Byte[] sendBytes = new Byte[18];
            Byte[] recvBytes = new Byte[512];
            List<Device> Devices = new List<Device>();

            // Create the timer
            Timer IAmTimer = new Timer();
            using (IAmTimer)
            {
                IAmTimer.Tick += new EventHandler(Timer_Tick);
                try
                {
                    //PEP Use NPDU.Create and APDU.Create (when written)
                    sendBytes[0] = BACnetEnums.BACNET_BVLC_TYPE_BIP;
                    sendBytes[1] = BACnetEnums.BACNET_BROADCAST_NPDU;
                    sendBytes[2] = 0;
                    sendBytes[3] = (byte)sendBytes.Length;
                    sendBytes[4] = BACnetEnums.BACNET_PROTOCOL_VERSION;
                    sendBytes[5] = 0x20;  // Control flags
                    sendBytes[6] = 0xFF;  // Destination network address (65535)
                    sendBytes[7] = 0xFF;
                    sendBytes[8] = 0;     // Destination MAC layer address length, 0 = Broadcast
                    sendBytes[9] = 0xFF;  // Hop count = 255

                    sendBytes[10] = (Byte)BACnetEnums.BACNET_PDU_TYPE.PDU_TYPE_UNCONFIRMED_SERVICE_REQUEST;
                    sendBytes[11] = (Byte)BACnetEnums.BACNET_UNCONFIRMED_SERVICE.SERVICE_UNCONFIRMED_WHO_IS;

                    sendBytes[12] = 0x0a;//Context Tag:0000 1 010
                    byte[] low = new byte[2];
                    low = BitConverter.GetBytes(low_limit);
                    sendBytes[13] = (Byte)low[1];
                    sendBytes[14] = (Byte)low[0];

                    sendBytes[15] = 0x1a;//Context Tag:0001 1 010
                    byte[] high = new byte[2];
                    high = BitConverter.GetBytes(high_limit);
                    sendBytes[16] = (Byte)high[1];
                    sendBytes[17] = (Byte)high[0];

                    UDPClient.EnableBroadcast = true;
                    UDPClient.Send(sendBytes, sendBytes.Length, BroadcastEP);

                    Socket sock = UDPClient.Client;
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                    // Start the timer so we can receive multiple responses
                    int count = 0;
                    TimerDone = false;
                    IAmTimer.Interval = milliseconds;
                    IAmTimer.Start();
                    while (!TimerDone)
                    {
                        Application.DoEvents();
                        // Process the response packets
                        if (sock.Available > 0)
                        {
                            recvBytes = UDPClient.Receive(ref RemoteIpEndPoint);
                            {
                                // Parse and save the BACnet data
                                int APDUOffset = NPDU.Parse(recvBytes, 4); // BVLL is always 4 bytes
                                if (APDU.ParseIAm(recvBytes, APDUOffset) > 0)
                                {
                                    Device device = new Device();
                                    device.Name = "Device";
                                    device.SourceLength = NPDU.SLEN;
                                    device.ServerEP = RemoteIpEndPoint;
                                    device.Network = NPDU.SNET;
                                    device.MACAddress = NPDU.SAddress;
                                    device.Instance = APDU.ObjectID;
                                    Devices.Add(device);
                                    // We should now have enough info to read/write properties for this device
                                }
                            }
                            // Restart the timer - as long as I-AM packets come, we'll wait
                            //ehow edit=>not restart
                            //IAmTimer.Stop();
                            //IAmTimer.Start();
                            Log("(" + Devices.Count + ")");
                        }
                        else
                        {
                            count = (count + 1) % 100;
                            if (count == 0) Log(".");
                        }
                    }
                }
                catch (Exception e)
                {
                    Log("find device fail: " + e.ToString() + "\n");
                }
                finally
                {
                    IAmTimer.Stop();
                }
            }
            Log("find device end\n");
            return Devices;
        }

        /// <summary>
        /// read property(if source length==0  Network,MACAddress will not use)
        /// </summary>
        /// <param name="ipAddr"></param>
        /// <param name="objtype"></param>
        /// <param name="instance"></param>
        /// <param name="objprop"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public bool SendReadProperty(
            string ipAddr,
            BACnetEnums.BACNET_OBJECT_TYPE objtype, uint instance,
            BACnetEnums.BACNET_PROPERTY_ID objprop,
            out List<Property> properties,
            UInt16 SourceLength = 0, UInt16 Network = 0, UInt16 MACAddress = 0
        )
        {
            properties = new List<Property>();
            Log("Read\n" + objtype.ToString() + " " + instance + "\n" +
                objprop.ToString() + "==============\n");
            IPAddress addr = IPAddress.Parse(ipAddr);
            IPEndPoint remoteEP = new IPEndPoint(addr, UDPPort_dest);
            if (remoteEP == null)
            {
                Log("remote EP Err(in read)\n");
                return false;
            }

            //uint instance = BACnetData.Devices[deviceidx].Instance;

            List<byte> sendBytes = new List<byte>();
            Byte[] recvBytes = new Byte[512];
            uint len;

            #region make BACnet packet
            List<byte> BaseBytes = BACnetBase.encode_BACnet_head(
                BACnetEnums.BACNET_SERVICES_SUPPORTED.SERVICE_SUPPORTED_READ_PROPERTY
                , ref InvokeCounter);
            sendBytes.AddRange(BaseBytes);
            // Service Request (var part of APDU):
            // Set up Object ID (Context Tag)
            APDU.SetObjectID(ref sendBytes, objtype, instance);

            // Set up Property ID (Context Tag)
            APDU.SetPropertyID(ref sendBytes, objprop);

            // Fix the BVLL length
            len = (uint)sendBytes.Count;
            sendBytes[3] = (byte)sendBytes.Count;
#endregion

            // Create the timer (we could use a blocking recvFrom instead ...)
            Timer ReadPropTimer = new Timer();
            ReadPropTimer.Tick += new EventHandler(Timer_Tick);
            ReadPropTimer.Interval = 1000;  // 100 ms
            UDPClient.EnableBroadcast = false;
            try
            {
                int Count = 0, logCount = 0;
                bool getResponse = false;
                while (Count < 3)
                {
                    bool needSeqACK = false, moreSeq = false;
                    List<byte> seq_buffer = new List<byte>();

                    // Start the timer
                    TimerDone = false;
                    ReadPropTimer.Start();
                    UDPClient.Send(sendBytes.ToArray(), (int)len, remoteEP);
                    while (!TimerDone)
                    {
                        // Wait for Confirmed Response
                        Application.DoEvents();
                        if (UDPClient.Available <= 0)
                        {
                            logCount = (logCount + 1) % 100;
                            if (logCount == 0) Log(".");
                        }
                        else
                        {
                            recvBytes = UDPClient.Receive(ref remoteEP);
                            if (recvBytes[0] != 0x81) Log("(warning)not BACnet(" +
                                recvBytes[0].ToString("X2") + "!=0x81)\n");
                            else if (recvBytes[1] != 0x0a && recvBytes[1] != 0x0b)
                                Log("(warning)not NPDU(maybt broadcast)" +
                                    recvBytes[1].ToString("X2") + "!=0x0a or 0x0b\n");
                            else
                            {
                                int APDUOffset = NPDU.Parse(recvBytes, 4); // BVLL is always 4 bytes

                                // Check for APDU response 
                                // 0x - Confirmed Request 
                                // 1x - Un-Confirmed Request
                                // 2x - Simple ACK
                                // 3x - Complex ACK
                                // 4x - Segment ACK
                                // 5x - Error
                                // 6x - Reject
                                // 7x - Abort
                                byte ACK = recvBytes[APDUOffset];
                                // Verify the Invoke ID is the same
                                byte ic = (byte)(InvokeCounter == 0 ? 255 : InvokeCounter - 1);
                                if (ic != recvBytes[APDUOffset + 1])
                                    Log("(warning)Invoke ID diff(read) " + recvBytes[APDUOffset + 1] + "/" + ic + "\n");
                                else if ((ACK & 0xF0) == 0x50)
                                {
                                    Log("(Error)ACK code(read) " + "bytes[" + APDUOffset + "]=" +
                                        "0x" + recvBytes[APDUOffset].ToString("X2") + "means Error\n");
                                    TimerDone = true;
                                    ReadPropTimer.Stop();
                                    Count = 3;
                                }
                                else if ((ACK & 0xF0) != 0x30)
                                    Log("(warning)ACK code(read) " + "bytes[" + APDUOffset + "]=" +
                                        "0x" + recvBytes[APDUOffset].ToString("X2") + "must 0x30\n");
                                else
                                {
                                    needSeqACK = (ACK & 0x08) == 0x08;
                                    moreSeq = (ACK & 0x04) == 0x04;
                                    int seqNum = -1, windowSize = -1;
                                    if (needSeqACK)
                                    {
                                        Log("Need Seq ACK\n");
                                        seqNum = recvBytes[APDUOffset + 2];
                                        windowSize = recvBytes[APDUOffset + 3];
                                        APDUOffset += 4;
                                    }
                                    else
                                    {
                                        Log("Not Seq\n");
                                        seqNum = windowSize = -1;
                                        APDUOffset += 2;
                                    }
                                    int service = recvBytes[APDUOffset++];
                                    if (service != (int)BACnetEnums.BACNET_SERVICES_SUPPORTED.SERVICE_SUPPORTED_READ_PROPERTY)
                                        Log("(warning)Service Choise Err=" + service + " must 0x06\n");
                                    else
                                    {
                                        getResponse = true;
                                        for (int i = APDUOffset; i < recvBytes.Length; i++)
                                            seq_buffer.Add(recvBytes[i]);
                                    }
                                    if (needSeqACK)
                                    {
                                        Log("Send Seq ACK\n");
                                        List<byte> seqACK = BACnetBase.encode_SeqACK(
                                            InvokeCounter - 1, seqNum, windowSize);
                                        UDPClient.Send(seqACK.ToArray(), seqACK.Count, remoteEP);
                                    }
                                    if (moreSeq)
                                    {
                                        Log("More Seq, Restart receive timer\n");
                                        ReadPropTimer.Stop();
                                        TimerDone = false;
                                        ReadPropTimer.Start();
                                    }
                                    else
                                    {
                                        Log("No More Seq, Stop receive timer\n");
                                        ReadPropTimer.Stop();
                                        TimerDone = true;
                                    }
                                }
                            }
                        }
                    }
                    Count++;
                    if (!getResponse) Log("Err no response" + Count + "\n");
                    else
                    {
                        APDU.Decode_Read_Property(seq_buffer.ToArray(), out properties);
                        break;
                    }
                }
                return getResponse;// This will still execute the finally
            }
            catch(Exception ex)
            {
                Log("read fail:" + ex.ToString() + "\n");
                return false;
            }
            finally
            {
                ReadPropTimer.Stop();
                ReadPropTimer.Tick -= Timer_Tick;
                ReadPropTimer.Dispose();
            }
        }

        /// <summary>
        /// write property(if source length==0  Network,MACAddress will not use)
        /// </summary>
        /// <param name="ipAddr"></param>
        /// <param name="objtype"></param>
        /// <param name="instance"></param>
        /// <param name="objprop"></param>
        /// <param name="property"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public bool SendWriteProperty(
            string ipAddr,
            BACnetEnums.BACNET_OBJECT_TYPE objtype, uint instance,
            BACnetEnums.BACNET_PROPERTY_ID objprop,
            Property property,
            int priority,
            UInt16 SourceLength = 0, UInt16 Network = 0, UInt16 MACAddress = 0
        )
        {
            Log("Write\n" + objtype.ToString() + " " + instance + "\n" + 
                objprop.ToString() + "==============\n");
            IPAddress addr = IPAddress.Parse(ipAddr);
            IPEndPoint remoteEP = new IPEndPoint(addr, UDPPort_dest);
            if (remoteEP == null) return false;
            if (property == null) return false;

            List<byte> sendBytes = new List<byte>();
            Byte[] recvBytes = new Byte[512];
            uint len;

            #region make BACnet packet
            List<byte> BaseBytes = BACnetBase.encode_BACnet_head(
                BACnetEnums.BACNET_SERVICES_SUPPORTED.SERVICE_SUPPORTED_WRITE_PROPERTY
                , ref InvokeCounter,
                SourceLength, Network, MACAddress);
            sendBytes.AddRange(BaseBytes);
            // Service Request (var part of APDU):
            // Set up Object ID (Context Tag)
            APDU.SetObjectID(ref sendBytes, objtype, instance);

            // Set up Property ID (Context Tag)
            APDU.SetPropertyID(ref sendBytes, objprop);

            // Optional array index goes here
            //if (arrayidx >= 0) len = APDU.SetArrayIdx(ref sendBytes, len, arrayidx);

            // Set the value to send
            APDU.SetProperty(ref sendBytes, property);

            //PEP Optional array index goes here

            // Set priority
            if (priority > 0)
                APDU.SetPriority(ref sendBytes, priority);

            // Fix the BVLL length
            len = (uint)sendBytes.Count;
            sendBytes[3] = (byte)len;
#endregion

            // Create the timer (we could use a blocking recvFrom instead ...)
            Timer ReadPropTimer = new Timer();
            try
            {
                using (ReadPropTimer)
                {
                    int Count = 0, logCount = 0;
                    ReadPropTimer.Tick += new EventHandler(Timer_Tick);

                    while (Count < 3)
                    {
                        UDPClient.EnableBroadcast = false;
                        UDPClient.Send(sendBytes.ToArray(), (int)len, remoteEP);

                        // Start the timer
                        TimerDone = false;
                        ReadPropTimer.Interval = 400; // 300;  // 100 ms
                        ReadPropTimer.Start();
                        while (!TimerDone)
                        {
                            // Wait for Confirmed Response
                            Application.DoEvents();
                            if (UDPClient.Client.Available > 0)
                            {
                                recvBytes = UDPClient.Receive(ref remoteEP);
                                if (recvBytes[0] != 0x81) Log("(warning)not BACnet(" +
                                    recvBytes[0].ToString("X2") + "!=0x81)\n");
                                else if (recvBytes[1] != 0x0a && recvBytes[1] != 0x0b)
                                    Log("(warning)not NPDU(maybt broadcast)" +
                                        recvBytes[1].ToString("X2") + "!=0x0a or 0x0b\n");
                                else
                                {
                                    int APDUOffset = NPDU.Parse(recvBytes, 4); // BVLL is always 4 bytes

                                    // Check for APDU response, and decide what to do
                                    // 0x - Confirmed Request 
                                    // 1x - Un-Confirmed Request
                                    // 2x - Simple ACK
                                    // 3x - Complex ACK
                                    // 4x - Segment ACK
                                    // 5x - Error
                                    // 6x - Reject
                                    // 7x - Abort
                                    if (recvBytes[APDUOffset] == 0x20)
                                    {
                                        // Verify the Invoke ID is the same
                                        byte ic = (byte)(InvokeCounter == 0 ? 255 : InvokeCounter - 1);
                                        if (ic == recvBytes[APDUOffset + 1])
                                        {
                                            return true; // This will still execute the finally
                                        }
                                        else Log("(warning)check code(write)\n");
                                        //else
                                        //{
                                        //  MessageBox.Show("Invoke Counter Error");
                                        //  return false;
                                        //}
                                    }
                                    else Log("(warning)ACK code(write) "
                                        + "bytes[" + APDUOffset + "]=" +
                                        "0x" + recvBytes[APDUOffset].ToString("X2") + "must 0x20\n");
                                }
                            }
                            else
                            {
                                logCount = (logCount + 1) % 100;
                                if (logCount == 0) Log(".");
                            }
                        }
                        Log("no response " + Count + "\n");
                        Count++;
                        //BACnetData.PacketRetryCount++;
                        ReadPropTimer.Stop(); // We'll start it over at the top of the loop
                    }
                    Log("Err no response \n");
                    return false; // This will still execute the finally
                }
            }catch(Exception ex)
            {
                Log("write fail: " + ex.ToString() + "\n");
                return false;
            }
            finally
            {
                ReadPropTimer.Stop();
            }
        }



        public bool SendDownloadDDC(string ipAddr, out byte[] file_data,
            UInt16 source_length = 0, UInt16 network = 0, UInt16 macAddr = 0, UInt16 instance = 1024)
        {
            file_data = null;
            Log("Download==============\n");
            IPAddress addr = IPAddress.Parse(ipAddr);
            IPEndPoint remoteEP = new IPEndPoint(addr, UDPPort_dest);
            if (remoteEP == null)
            {
                Log("remote EP Err(in read)\n");
                return false;
            }

            List<byte> sendBytes = new List<byte>();
            Byte[] recvBytes = new Byte[2048];

            bool isEnd = false;
            int file_start = 0;
            int seq_length = (source_length == 0) ? 1396 : 400;//BACSoft use 1396/400(MSTP);
            int log_count = 0, err_count = 0; ;
            Timer ReceiveTimer = new Timer();
            ReceiveTimer.Tick += new EventHandler(Timer_Tick);
            ReceiveTimer.Interval = 5000;
            UDPClient.EnableBroadcast = false;
            List<byte> file_data_list = new List<byte>();
            try{
                while (!isEnd && err_count < 10)
                {
                    List<byte> seq_buffer = new List<byte>();
                    List<byte> BaseBytes = BACnetBase.encode_BACnet_head(
                        BACnetEnums.BACNET_SERVICES_SUPPORTED.SERVICE_SUPPORTED_ATOMIC_READ_FILE
                        , ref InvokeCounter, source_length, network, macAddr);
                    //read file from file_start and one time length=seq_length
                    List<byte> encode = FileCoding.Read.EecodeAtomicReadFile(instance, file_start, seq_length);
                    sendBytes.Clear();
                    sendBytes.AddRange(BaseBytes);
                    sendBytes.AddRange(encode);
                    // Fix the BVLL length
                    byte[] BAC_len = BitConverter.GetBytes((UInt16)sendBytes.Count);
                    sendBytes[2] = BAC_len[1];
                    sendBytes[3] = BAC_len[0];

                    UDPClient.Send(sendBytes.ToArray(), sendBytes.Count, remoteEP);
                    //Receive
                    bool getResponse = false;
                    TimerDone = false;
                    ReceiveTimer.Start();
                    while (!TimerDone)
                    {
                        Application.DoEvents();
                        bool needSeqACK = false, moreSeq = false;
                        int seqNum = -1, windowSize = -1;
                        if (UDPClient.Available <= 0)
                        {
                            log_count = (log_count + 1) % 1000;
                            if (log_count == 0) Log(".");
                        }
                        else
                        {
                            recvBytes = UDPClient.Receive(ref remoteEP);

                            if (recvBytes[0] != 0x81) Log("(warning)not BACnet(" +
                                recvBytes[0].ToString("X2") + "!=0x81)\n");
                            else if (recvBytes[1] != 0x0a && recvBytes[1] != 0x0b)
                                Log("(warning)not NPDU(maybt broadcast)" +
                                    recvBytes[1].ToString("X2") + "!=0x0a or 0x0b\n");
                            else
                            {
                                int APDUOffset = NPDU.Parse(recvBytes, 4); // BVLL is always 4 bytes
                                // Check for APDU response 
                                // 0x - Confirmed Request 
                                // 1x - Un-Confirmed Request
                                // 2x - Simple ACK
                                // 3x - Complex ACK
                                // 4x - Segment ACK
                                // 5x - Error
                                // 6x - Reject
                                // 7x - Abort
                                byte ACK = recvBytes[APDUOffset];
                                byte ic = (byte)(InvokeCounter == 0 ? 255 : InvokeCounter - 1);
                                // Verify the Invoke ID is the same
                                if (ic != recvBytes[APDUOffset + 1])
                                    Log("(warning)Invoke ID diff(read)" + recvBytes[APDUOffset + 1] + " must " + ic + "\n");
                                else if ((ACK & 0xF0) != 0x30)
                                {
                                    Log("(Error)ACK code(read) " + "bytes[" + APDUOffset + "]=" +
                                        "0x" + recvBytes[APDUOffset].ToString("X2") + "must 0x30\n");
                                    ReceiveTimer.Stop();
                                    TimerDone = true;
                                }
                                else
                                {
                                    needSeqACK = (ACK & 0x08) == 0x08;
                                    moreSeq = (ACK & 0x04) == 0x04;
                                    if (needSeqACK)
                                    {
                                        seqNum = recvBytes[APDUOffset + 2];
                                        windowSize = recvBytes[APDUOffset + 3];
                                        Log("Need Seq ACK \n");
                                        APDUOffset += 4;
                                    }
                                    else
                                    {
                                        Log("Not Seq\n");
                                        seqNum = windowSize = -1;
                                        APDUOffset += 2;
                                    }
                                    int service = recvBytes[APDUOffset++];
                                    if (service != (int)BACnetEnums.BACNET_SERVICES_SUPPORTED.SERVICE_SUPPORTED_ATOMIC_READ_FILE)
                                        Log("(warning)Service Choise Err=" + service + " must 0x06\n");
                                    else
                                    {
                                        getResponse = true;
                                        for (int i = APDUOffset; i < recvBytes.Length; i++)
                                            seq_buffer.Add(recvBytes[i]);
                                    }
                                    if (needSeqACK)
                                    {
                                        Log("Send Seq ACK\n");
                                        List<byte> seqACK = BACnetBase.encode_SeqACK(
                                            InvokeCounter - 1, seqNum, windowSize);
                                        UDPClient.Send(seqACK.ToArray(), seqACK.Count, remoteEP);
                                    }
                                    if (moreSeq)
                                    {
                                        Log("More Seq, Restart receive timer\n");
                                        ReceiveTimer.Stop();
                                        TimerDone = false;
                                        getResponse = false;
                                        ReceiveTimer.Start();
                                    }
                                    else
                                    {
                                        Log("No More Seq, Stop receive timer\n");
                                        ReceiveTimer.Stop();
                                        TimerDone = true;
                                    }
                                }
                            }
                        }
                    }
                    Log("\n");
                    //receive end => error or decode
                    if (!getResponse)
                    {
                        Log("Err no response\n");
                        return false;  // This will still execute the finally
                    }
                    else
                    {
                        try
                        {
                            byte[] buffer;
                            isEnd = FileCoding.Read.DecodeAtomicReadFile(seq_buffer.ToArray(), out buffer);
                            file_data_list.AddRange(buffer);
                            file_start = file_data_list.Count;
                            Log("Decode finish, File End:" + isEnd + "\n");
                        }
                        catch (Exception ex)
                        {
                            Log(ex.ToString() + "\n");
                            return false;
                        }
                    }
                    err_count++;
                }
                if (err_count >= 10 && !isEnd)
                {
                    Log("Err receive 10 times but not file end\n");
                    return false;
                }
            }
            catch(Exception ex)
            {
                Log(ex.ToString() + "\n");
            }
            finally
            {
                ReceiveTimer.Stop();
                ReceiveTimer.Tick -= new EventHandler(Timer_Tick);
                ReceiveTimer.Dispose();
                file_data = file_data_list.ToArray();
            }
            return true;
        }

        /*
         * 1.write program 1024 program-change = (5) vendor-propriety value
         * 2.write file1024 = { start=0, data=....bytes }
         * 3.write program 1024 program-change = (1) load
        */
        public bool SendUploadDDC( string ipAddr, byte[] file_data,
            UInt16 source_length = 0, UInt16 network = 0, UInt16 macAddr = 0, UInt16 instance = 1024
        )
        {
            Log("Upload==============\n");
            IPAddress addr = IPAddress.Parse(ipAddr);
            IPEndPoint remoteEP = new IPEndPoint(addr, UDPPort_dest);
            if (remoteEP == null)
            {
                Log("remote EP Err(in read)\n");
                return false;
            }
            if (file_data == null)
            {
                Log("null file data\n");
                return false;
            }
            if (file_data.Length <= 0)
            {
                Log("file data size = 0\n");
                return false;
            }

            Property property = null;
            #region write program 1024 program-change = (5) vendor-propriety value
            property = new Property();
            property.Tag = BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_ENUMERATED;
            property.ValueEnum = 5;
            if (!SendWriteProperty(ipAddr,
                BACnetEnums.BACNET_OBJECT_TYPE.OBJECT_PROGRAM, instance,
                BACnetEnums.BACNET_PROPERTY_ID.PROP_PROGRAM_CHANGE,
                property, -1,
                source_length, network, macAddr))
            {
                Log("(warning)write program-change fail(vendor-propriety value)\n");
                return false;
            }
            #endregion
            System.Threading.Thread.Sleep(1000);

            #region write file 1024
            bool WriteSucc = false;
            List<byte> sendBytes = new List<byte>();
            Byte[] recvBytes = new Byte[512];
            Int16 seq_length = (Int16)((source_length == 0) ? 1396 : 400);//BACSoft use 1396;
            
            UDPClient.EnableBroadcast = false;
            Timer ReceiveTimer = new Timer();
            ReceiveTimer.Interval = 1000;
            ReceiveTimer.Tick += Timer_Tick;
            try
            {
                Log("File Size:" + file_data.Length + "\n");
                //Send
                for (Int16 pos = 0; pos < file_data.Length; pos += seq_length)
                {
                    WriteSucc = false;
                    int len = (pos + seq_length < file_data.Length) ?
                        seq_length : (file_data.Length - pos);
                    Log("Encode start=" + pos + " length=" + len + "\n");
                    byte[] seq_data = new byte[len];
                    Array.Copy(file_data, pos, seq_data, 0, len);
                    
                    List<byte> BaseBytes = BACnetBase.encode_BACnet_head(
                        BACnetEnums.BACNET_SERVICES_SUPPORTED.SERVICE_SUPPORTED_ATOMIC_WRITE_FILE
                        , ref InvokeCounter,
                        source_length, network, macAddr);
                    // Write File Encode (obj_indentify, start position, data length)
                    List<byte> encode = FileCoding.Write.EncodeAtomicWriteFile(instance, pos, seq_data);
                    sendBytes.Clear();
                    sendBytes.AddRange(BaseBytes);
                    sendBytes.AddRange(encode);
                    // Fix the BVLL length
                    byte[] temp = BitConverter.GetBytes((UInt16)sendBytes.Count);
                    sendBytes[2] = temp[1];
                    sendBytes[3] = temp[0];

                    UDPClient.Send(sendBytes.ToArray(), sendBytes.Count, remoteEP);
                    //Receive
                    int log_count = 0;
                    TimerDone = false;
                    ReceiveTimer.Start();
                    while (!TimerDone)
                    {
                        Application.DoEvents();
                        if (UDPClient.Available <= 0)
                        {
                            log_count = (log_count + 1) % 200;
                            if (log_count == 0) Log(".");
                        }
                        else
                        {
                            recvBytes = UDPClient.Receive(ref remoteEP);

                            if (recvBytes[0] != 0x81)
                                Log("(warning)not BACnet(" +
                                recvBytes[0].ToString("X2") + "!=0x81)\n");
                            else if (recvBytes[1] != 0x0a && recvBytes[1] != 0x0b)
                                Log("(warning)not NPDU(maybt broadcast)" +
                                    recvBytes[1].ToString("X2") + "!=0x0a or 0x0b\n");
                            else
                            {
                                int APDUOffset = NPDU.Parse(recvBytes, 4); // BVLL is always 4 bytes
                                // Check for APDU response 
                                // 0x - Confirmed Request 
                                // 1x - Un-Confirmed Request
                                // 2x - Simple ACK
                                // 3x - Complex ACK
                                // 4x - Segment ACK
                                // 5x - Error
                                // 6x - Reject
                                // 7x - Abort
                                byte ACK = recvBytes[APDUOffset];
                                byte ic = (byte)(InvokeCounter == 0 ? 255 : InvokeCounter - 1);
                                // Verify the Invoke ID is the same
                                if (ic != recvBytes[APDUOffset + 1])
                                    Log("(warning)check code" + recvBytes[APDUOffset + 1] + " must" + ic + "\n");
                                else if ((ACK & 0xF0) != 0x30)
                                { 
                                    Log("(Error)ACK code " + "bytes[" + APDUOffset + "]=" +
                                        "0x" + recvBytes[APDUOffset].ToString("X2") + "must 0x30\n");
                                    TimerDone = true;
                                    ReceiveTimer.Stop();
                                }
                                else if (recvBytes[APDUOffset + 2] != (byte)BACnetEnums.BACNET_SERVICES_SUPPORTED.SERVICE_SUPPORTED_ATOMIC_WRITE_FILE)
                                    Log("(warning)service Err 0x" + recvBytes[APDUOffset + 2].ToString("X2") + "\n");
                                else
                                {
                                    Log("Success pos=" + pos + "\n");
                                    WriteSucc = true;
                                    TimerDone = true;
                                    ReceiveTimer.Stop();
                                }
                            }
                        }
                    }
                    if (!WriteSucc)
                    {
                        Log("Error no response\n");
                        return false;
                    }
                }
            }catch(Exception ex)
            {
                Log(ex.ToString() + "\n");
            }
            finally
            {
                ReceiveTimer.Stop();
                ReceiveTimer.Tick -= Timer_Tick;
                ReceiveTimer.Dispose();
            }
            #endregion

            System.Threading.Thread.Sleep(1000);
            if (WriteSucc)
            {
                Log("Write File Success\n");
                #region write program 1024 program-change = (1) Load
                property = new Property();
                property.Tag = BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_ENUMERATED;
                property.ValueEnum = 1;
                if (!SendWriteProperty(ipAddr,
                    BACnetEnums.BACNET_OBJECT_TYPE.OBJECT_PROGRAM, instance,
                    BACnetEnums.BACNET_PROPERTY_ID.PROP_PROGRAM_CHANGE,
                    property, -1,
                    source_length, network, macAddr))
                {
                    Log("(warning)write program-change fail(load)\n");
                    return false;
                }
                #endregion
                return true;
            }
            else return false;
        }





        public void Dispose()
        {
            TimerDone = true;
            if (LogEvent != null)
                LogEvent = null;
            if (UDPClient != null)
            {
                UDPClient.Client.Close();
                UDPClient.Close();
                UDPClient = null;
            }
        }
    }
}
