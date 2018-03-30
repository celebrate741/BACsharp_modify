using System;
using System.Collections.Generic;
using System.Text;

namespace BACsharp.BACnet_Def
{
    /*
     *  from Yet Another Bacnet Explorer
     *  (https://sourceforge.net/projects/yetanotherbacnetexplorer/)
     *  project BasicReadWrite.BACnetBase.cs
    */
    public static class BACnetBase
    {
        public static bool IS_OPENING_TAG(byte b)
        {
            return (b & 0x07) == 6;
        }
        public static bool IS_CLOSING_TAG(byte b)
        {
            return (b & 0x07) == 7;
        }
        public static bool IS_EXTENDED_TAG_NUMBER(byte b)
        {
            return (b & 0xF0) == 0xF0;
        }
        public static bool IS_EXTENDED_VALUE(byte b)
        {
            return (b & 0x07) == 0x05;
        }
        public static bool IS_CONTEXT_SPECIFIC(byte b)
        {
            return (b & 0x08) == 0x08;
        }

        public static uint decode_tag_number(byte[] bytes, uint pos, out byte tag_number)
        {
            if (IS_EXTENDED_TAG_NUMBER(bytes[pos]))
                tag_number = bytes[++pos];
            else tag_number = (byte)(bytes[pos] >> 4);
            return pos;
        }
        public static bool is_context_tag(byte[] buffer, uint offset, byte tag_number)
        {
            byte my_tag_number = 0;

            decode_tag_number(buffer, offset, out my_tag_number);
            return (bool)(IS_CONTEXT_SPECIFIC(buffer[offset]) && (my_tag_number == tag_number));
        }
        public static uint decode_tag_number_value(ref byte[] bytes, uint pos, out byte tag_number, out uint value)
        {
            uint len = 1;
            pos = decode_tag_number(bytes, pos, out tag_number);
            if (IS_EXTENDED_VALUE(bytes[pos]))
            {
                /* tagged as uint32_t */
                if (bytes[pos + 1] == 255)
                {
                    len = 2;
                    byte[] temp = new byte[4];
                    Array.Copy(bytes, pos + len, temp, 0, 4);
                    Array.Reverse(temp);
                    value = BitConverter.ToUInt32(temp, 0);
                    len += 4;
                }
                /* tagged as uint16_t */
                else if (bytes[pos + 1] == 254)
                {
                    len = 2;
                    byte[] temp = new byte[2];
                    Array.Copy(bytes, pos + len, temp, 0, 2);
                    Array.Reverse(temp);
                    value = BitConverter.ToUInt16(temp, 0);
                    len += 2;
                }
                /* no tag - must be uint8_t */
                else
                {
                    value = bytes[pos + len];
                    len++;
                }
            }
            else if (IS_OPENING_TAG(bytes[pos]) || IS_CLOSING_TAG(bytes[pos]))
            {
                len++;
                value = 0;
            }
            else value = (uint)(bytes[pos] & 0x07);

            return pos + len;
        }

        public static List<byte> encode_BACnet_head(
            BACnetEnums.BACNET_SERVICES_SUPPORTED service, ref int InvokeCounter,
            UInt16 SourceLength = 0, UInt16 Network = 0, UInt16 MACAddress = 0)
        {
            List<byte> bytes = new List<byte>();
            // BVLL
            bytes.Add(BACnetEnums.BACNET_BVLC_TYPE_BIP);
            bytes.Add(BACnetEnums.BACNET_UNICAST_NPDU);
            bytes.Add(0x00);
            bytes.Add(0x00);  // BVLL Length, fix later (24?)

            // NPDU
            bytes.Add(BACnetEnums.BACNET_PROTOCOL_VERSION);
            if (SourceLength == 0)
                bytes.Add(0x04);  // Control flags, no destination address
            else bytes.Add(0x24);  // Control flags, with broadcast or destination address

            if (SourceLength > 0)
            {
                // Get the (MSTP) Network number (2001)
                byte[] temp = BitConverter.GetBytes(Network);
                bytes.Add(temp[1]);
                bytes.Add(temp[0]);

                // Get the MAC address (0x0D)
                //sendBytes[8] = 0x01;  // MAC address length
                //sendBytes[9] = 0x0D;  // Destination MAC layer address
                temp = BitConverter.GetBytes(MACAddress);
                bytes.Add(0x01);  // MAC address length - adjust for other lengths ...
                bytes.Add(temp[0]);
                bytes.Add(0xFF);  // Hop count = 255
            }

            // APDU
            bytes.Add(0x02);  // Control flags
                                  // Max Response Greater than 64 (。111。。。。)
            bytes.Add(0x75);  // Max APDU length (1476)(。。。。0101)

            // Create invoke counter
            bytes.Add((byte)(InvokeCounter));
            InvokeCounter = ((InvokeCounter + 1) & 0xFF);

            // Service Choice: Atomic Read File
            bytes.Add((byte)service);

            return bytes;
        }

        public static List<byte> encode_SeqACK(int InvokeCounter, int seqNum, int windowSize)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(BACnetEnums.BACNET_BVLC_TYPE_BIP);
            bytes.Add(BACnetEnums.BACNET_UNICAST_NPDU);
            bytes.Add(0x00);
            bytes.Add(0x00);  // BVLL Length, fix later (24?)
            
            bytes.Add(BACnetEnums.BACNET_PROTOCOL_VERSION);
            bytes.Add(0x00);  // Control flags, no expection reply
            
            bytes.Add(0x40);  // Control flags, Seq ACK

            // Create invoke counter
            bytes.Add((byte)(InvokeCounter));

            //Seq Number
            bytes.Add((byte)seqNum);
            //propose window size
            bytes.Add((byte)windowSize);

            //Fix BVLL Length
            bytes[3] = (byte)bytes.Count;

            return bytes;
        }
    }
}
