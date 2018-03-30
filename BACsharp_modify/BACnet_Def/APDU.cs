using System;
using System.Collections.Generic;
using System.Text;

namespace BACsharp.BACnet_Def
{
    /// <summary>
    /// From BACnet.cs
    /// </summary>
    public static class APDU
    {
        public static byte APDUType;
        public static UInt16 ObjectType;
        public static UInt32 ObjectID;

        public static int /*APDU*/ ParseIAm(byte[] bytes, int offset)
        {
            // Look for and parse I-Am Packet
            int len = 0;
            ObjectID = 0;
            APDUType = bytes[offset];
            if ((APDUType == 0x10) && (bytes[offset + 1] == 0x00))
            {
                // Get the ObjectID
                if (BACnetTag.TagNumber(bytes[offset + 2]) != 12)
                    return 0;
                byte[] temp = new byte[4];
                temp[0] = bytes[offset + 6];
                temp[1] = bytes[offset + 5];
                temp[2] = (byte)((int)bytes[offset + 4] & 0x3F);
                temp[3] = 0;
                ObjectID = BitConverter.ToUInt32(temp, 0);
                len = 5; //PEP Make the APDU length ...
                return len;
            }
            else
                return 0;
        }

        public static int /*APDU*/ ParseRead(byte[] bytes, int offset, out int apptag)
        {
            // Look for and parse Read Property Complex ACK 
            apptag = 0xFF;
            int len = offset;
            if (bytes[len] != 0x30) return 0;   // APDU Complex ACK
            len += 2;
            if (bytes[len++] != 0x0C) return 0; // Read Property ACK

            //PEP Parse the Object ID
            //PEP 5 Bytes for Binary Object: 0x0C 0x00 0x0C 0x00 0x01
            //byte[] temp = new byte[4];
            //temp[0] = bytes[offset + 6];
            //temp[1] = bytes[offset + 5];
            //temp[2] = (byte)((int)bytes[offset + 4] & 0x3F);
            //temp[3] = 0;
            //ObjectID = BitConverter.ToUInt32(temp, 0);
            len += 5;

            // Parse the Property ID
            if (bytes[len] == 0x19)
                len += 2; // 1 byte Property ID
            else if (bytes[len] == 0x1A)
                len += 3; // 2 byte Property ID

            // Look for Array Index
            if (bytes[len] == 0x29)
                len += 2; // 1 byte Array Index
            else if (bytes[len] == 0x2A)
                len += 3; // 2 byte Array Index

            // Lok for Property Value
            len++; // 1 byte opening tag 0x3E
            apptag = bytes[len++]; // Look at Application Tag
            return len;
        }

        public static uint /*APDU*/ AppUInt(byte[] bytes, int offset)
        {
            // AppTag = 0x21
            return bytes[offset];
        }

        public static UInt16 /*APDU*/ AppUInt16(byte[] bytes, int offset)
        {
            // AppTag = 0x22
            byte[] temp = new byte[2];
            temp[1] = bytes[offset++];
            temp[0] = bytes[offset++];
            return BitConverter.ToUInt16(temp, 0);
        }

        public static UInt32 /*APDU*/ AppUInt24(byte[] bytes, int offset)
        {
            // AppTag = 0x23
            byte[] temp = new byte[4];
            temp[3] = 0;
            temp[2] = bytes[offset++];
            temp[1] = bytes[offset++];
            temp[0] = bytes[offset++];
            return BitConverter.ToUInt32(temp, 0);
        }

        public static UInt32 /*APDU*/ AppUInt32(byte[] bytes, int offset)
        {
            // AppTag = 0x24
            byte[] temp = new byte[4];
            temp[3] = bytes[offset++];
            temp[2] = bytes[offset++];
            temp[1] = bytes[offset++];
            temp[0] = bytes[offset++];
            return BitConverter.ToUInt32(temp, 0);
        }

        public static float /*APDU*/ AppSingle(byte[] bytes, int offset)
        {
            // Apptag = 0x44
            byte[] temp = new byte[4];
            temp[3] = bytes[offset++];
            temp[2] = bytes[offset++];
            temp[1] = bytes[offset++];
            temp[0] = bytes[offset++];
            return BitConverter.ToSingle(temp, 0);
        }

        public static byte[] /*APDU*/ AppOctet(byte[] bytes, int offset)
        {
            // AppTag = 0x65
            int length = bytes[offset++]; // length/value/type
            if ((offset > 0) && (length > 0))
            {
                byte[] octet = new byte[length];
                for (int i = 0; i < length; i++)
                    octet[i] = bytes[offset++];
                return octet;
            }
            else
                return null;
        }

        public static string /*APDU*/ AppString(byte[] bytes, int offset)
        {
            // AppTag = 0x75
            int length = bytes[offset] - 1; // length/value/type
            if ((offset > 0) && (length > 0))
                return Encoding.ASCII.GetString(bytes, offset + 2, length);
            else
                return "???";
        }

        public static void /*APDU*/ SetObjectID(ref List<byte> bytes,
          BACnetEnums.BACNET_OBJECT_TYPE type, uint instance)
        {
            // Assemble Object ID portion of APDU
            UInt32 value = 0;

            //PEP Context Specific Tag number could differ
            bytes.Add(0x0C);  // Tag number (BACnet Object ID)
                                  //bytes[pos++] = 0x01;
                                  //bytes[pos++] = 0x00;
                                  //bytes[pos++] = 0x00;
                                  //bytes[pos++] = 0x00;

            value = (UInt32)type;
            value = value & BACnetEnums.BACNET_MAX_OBJECT;
            value = value << BACnetEnums.BACNET_INSTANCE_BITS;
            value = value | (instance & BACnetEnums.BACNET_MAX_INSTANCE);
            //len = encode_unsigned32(apdu, value);
            byte[] temp4 = new byte[4];
            temp4 = BitConverter.GetBytes(value);
            bytes.Add(temp4[3]);
            bytes.Add(temp4[2]);
            bytes.Add(temp4[1]);
            bytes.Add(temp4[0]);
        }

        public static void /*APDU*/ SetPropertyID(ref List<byte> bytes,
          BACnetEnums.BACNET_PROPERTY_ID type)
        {
            // Assemble Property ID portion of APDU
            UInt32 value = (UInt32)type;
            if (value <= 255)
            {
                bytes.Add(0x19);  //PEP Context Specific Tag number, could differ
                bytes.Add((byte)type);
            }
            else if (value < 65535)
            {
                bytes.Add(0x1A);  //PEP Context Specific Tag number, could differ
                byte[] temp2 = new byte[2];
                temp2 = BitConverter.GetBytes(value);
                bytes.Add(temp2[1]);
                bytes.Add(temp2[0]);
            }
        }

        public static uint /*APDU*/ SetArrayIdx(ref byte[] bytes, uint pos, int aidx)
        {
            // Assemble Property ID portion of APDU
            UInt32 value = (UInt32)aidx;
            if (value <= 255)
            {
                bytes[pos++] = 0x29;  //PEP Context Specific Tag number, could differ
                bytes[pos++] = (byte)aidx;
            }
            else if (value < 65535)
            {
                bytes[pos++] = 0x2A;  //PEP Context Specific Tag number, could differ
                byte[] temp2 = new byte[2];
                temp2 = BitConverter.GetBytes(value);
                bytes[pos++] = temp2[1];
                bytes[pos++] = temp2[0];
            }
            return pos;
        }

        public static void /*APDU*/ SetProperty(ref List<byte> bytes, Property property)
        {
            // Convert property class into bytes
            if (property != null)
            {
                bytes.Add(0x3E);  // Tag Open
                switch (property.Tag)
                {
                    case BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_NULL:
                        bytes.Add(0x00);
                        break;
                    case BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_BOOLEAN:
                        if (property.ValueBool)
                            bytes.Add(0x11);
                        else
                            bytes.Add(0x10);
                        break;
                    case BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_UNSIGNED_INT:
                        // Tag could be 0x21, 0x22, 0x23, or 0x24
                        // We can't do Uint64?
                        UInt32 value = (UInt32)property.ValueUInt;
                        if (value <= 255) // 1 byte
                        {
                            bytes.Add(0x21);
                            bytes.Add((byte)value);
                        }
                        else if (value <= 65535)  // 2 bytes
                        {
                            bytes.Add(0x22);
                            byte[] temp2 = new byte[2];
                            temp2 = BitConverter.GetBytes(value);
                            bytes.Add(temp2[1]);
                            bytes.Add(temp2[0]);
                        }
                        else if (value <= 16777215) // 3 bytes
                        {
                            bytes.Add(0x23);
                            byte[] temp3 = new byte[3];
                            temp3 = BitConverter.GetBytes(value);
                            bytes.Add(temp3[2]);
                            bytes.Add(temp3[1]);
                            bytes.Add(temp3[0]);
                        }
                        else // 4 bytes
                        {
                            bytes.Add(0x24);
                            byte[] temp4 = new byte[4];
                            temp4 = BitConverter.GetBytes(value);
                            bytes.Add(temp4[3]);
                            bytes.Add(temp4[2]);
                            bytes.Add(temp4[1]);
                            bytes.Add(temp4[0]);
                        }
                        break;
                    case BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_SIGNED_INT:
                        // Tag could be 0x31, 0x32, 0x33, 0x34
                        break;
                    case BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_REAL:
                        // Tag is 0x44
                        bytes.Add(0x44);
                        byte[] temp5 = new byte[4];
                        temp5 = BitConverter.GetBytes(property.ValueSingle);
                        bytes.Add(temp5[3]);
                        bytes.Add(temp5[2]);
                        bytes.Add(temp5[1]);
                        bytes.Add(temp5[0]);
                        break;
                    case BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_DOUBLE:
                        // Tag is 0x55
                        break;
                    case BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_OCTET_STRING:
                        // Tag is 0x65, maximum 16 bytes!
                        bytes.Add(0x65);
                        int len = property.ValueOctet.Length;
                        bytes.Add((byte)len);
                        for (int i = 0; i < len; i++)
                            bytes.Add(property.ValueOctet[i]);
                        break;
                    case BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_CHARACTER_STRING:
                        // Tag is 0x75, maximum 15 chars!
                        bytes.Add(0x75);
                        int len1 = property.ValueString.Length;
                        bytes.Add((byte)(len1 + 1));  // Include character set byte
                        bytes.Add(0); // ANSI
                        for (int i = 0; i < len1; i++)
                            bytes.Add((byte)property.ValueString[i]);
                        break;
                    case BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_ENUMERATED:
                        // Tag could be 0x91, 0x92, 0x93, 0x94
                        bytes.Add(0x91);
                        bytes.Add((byte)property.ValueEnum);
                        break;
                    case BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_OBJECT_ID:
                        // Tag is 0xC4
                        bytes.Add(0xC4);
                        UInt32 id = ((UInt32)property.ValueObjectType) << 22;
                        id += (property.ValueObjectInstance & 0x3FFFFF);
                        byte[] temp6 = new byte[4];
                        temp6 = BitConverter.GetBytes(id);
                        bytes.Add(temp6[3]);
                        bytes.Add(temp6[2]);
                        bytes.Add(temp6[1]);
                        bytes.Add(temp6[0]);
                        break;
                }
                bytes.Add(0x3F);  // Tag Close
            }
        }

        public static uint /*APDU*/ ParseProperty(byte[] bytes, uint pos, out Property property)
        {
            // Convert bytes into Property
            byte tag;
            uint value;
            property = new Property();
            property.Tag = BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_NULL;
            //property
            uint debug_pos = pos;
            pos = BACnetBase.decode_tag_number_value(ref bytes, pos, out tag, out value);
            List<byte> octet = new List<byte>();
            List<byte> temp = new List<byte>();
            for (uint i = 0; i < value; i++) octet.Add(bytes[pos++]);
            temp.AddRange(octet);
            if (value < 8)//03-FF => 0-0-0-0-0-0-03-FF
                for (uint i = 0; i < 8 - value; i++)
                    temp.Insert(0, 0);
            temp.Reverse();//0-0-0-0-0-0-03-FF => FF-03-0-0-0-0-0-0 => int,uint.......
            switch (tag)
            {
                case 0x01:
                    property.Tag = BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_BOOLEAN;
                    property.ValueBool = (BitConverter.ToUInt32(temp.ToArray(), 0) == 1);
                    property.ToStringValue = property.ValueBool.ToString();
                    break;
                case 0x02:
                    property.Tag = BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_UNSIGNED_INT;
                    property.ValueUInt = BitConverter.ToUInt32(temp.ToArray(), 0);
                    property.ToStringValue = property.ValueUInt.ToString();
                    break;
                case 0x03:
                    property.Tag = BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_SIGNED_INT;
                    property.ValueInt = BitConverter.ToInt32(temp.ToArray(), 0);
                    property.ToStringValue = property.ValueInt.ToString();
                    break;
                case 0x04:
                    property.Tag = BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_REAL;
                    property.ValueSingle = BitConverter.ToSingle(temp.ToArray(), 0);
                    property.ToStringValue = property.ValueSingle.ToString();
                    break;
                case 0x05:
                    property.Tag = BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_DOUBLE;
                    property.ValueDouble = BitConverter.ToDouble(temp.ToArray(), 0);
                    property.ToStringValue = property.ValueDouble.ToString();
                    break;
                case 0x06:
                    property.Tag = BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_OCTET_STRING;
                    property.ValueOctet = octet.ToArray();
                    string s = "";
                    for (int i = 0; i < property.ValueOctet.Length; i++)
                        s = s + property.ValueOctet[i].ToString("X2");
                    property.ToStringValue = s;
                    break;
                case 0x07:
                    property.Tag = BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_CHARACTER_STRING;
                    octet.RemoveAt(0);//Character Set 00 = UTF-8
                    property.ValueString = Encoding.UTF8.GetString(octet.ToArray());
                    property.ToStringValue = property.ValueString;
                    break;
                case 0x09:
                    property.Tag = BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_ENUMERATED;
                    property.ValueEnum = BitConverter.ToUInt32(temp.ToArray(), 0);
                    property.ToStringValue = property.ValueUInt.ToString();
                    break;
                case 0x0C:
                    property.Tag = BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_OBJECT_ID;
                    value = BitConverter.ToUInt32(temp.ToArray(), 0);
                    property.ValueObjectType = (BACnetEnums.BACNET_OBJECT_TYPE)(value >> 22);
                    property.ValueObjectInstance = value & 0x3FFFFF;
                    property.ToStringValue = property.ValueObjectType.ToString() +
                                                                "[" + property.ValueObjectInstance + "]"; 
                    break;
                default: throw new Exception("Type Unknow 0x" + tag.ToString("X2") + " on " + debug_pos);
            }
            return pos;
        }
        public static void Decode_Read_Property(byte[] bytes, out List<Property> properties)
        {
            properties = new List<Property>();
            //bytes start behind service-choise 0x0C(not include)
            byte tag;
            uint value, pos = 0;
            //PEP Parse the Object ID
            //PEP 5 Bytes for Binary Object: 0x0C 0x00 0x0C 0x00 0x01
            pos += 5;
            //Property Identifier: progran-change(90), tag 1
            pos = BACnetBase.decode_tag_number_value(ref bytes, pos, out tag, out value);
            pos += value;
            //opening tag
            if (!BACnetBase.IS_OPENING_TAG(bytes[pos++]))
                throw new Exception("Not Opening Tag  on pos " + (pos - 1));
            while (pos < bytes.Length)
            {
                Property property = null;
                pos = ParseProperty(bytes, pos, out property);
                properties.Add(property);
                if (pos >= bytes.Length)
                    throw new Exception("Bytes Length Error Or Closing Tag Not Found " + pos);
                if (BACnetBase.IS_CLOSING_TAG(bytes[pos]))
                    break;
            }
        }

        public static void /*APDU*/ SetPriority(ref List<byte> bytes, int priority)
        {
            // Convert priority into bytes
            bytes.Add(0x49);  //PEP Why x49???
            bytes.Add((byte)priority);
        }

    }


    /// BACnetTag Routines(From BACnet.cs)
    public static class BACnetTag
    {
        public static byte TagNumber(byte tag)
        {
            int x = ((int)tag >> 4) & 0x0F;
            return (byte)x;
        }

        public static byte Class(byte tag)
        {
            int x = ((int)tag >> 3) & 0x01;
            return (byte)x;
        }
        public static byte LenValType(byte tag)
        {
            int x = (int)tag & 0x07;
            return (byte)x;
        }
    }
}
