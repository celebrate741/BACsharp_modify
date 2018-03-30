using System;
using System.Collections.Generic;
using System.IO;

namespace BACsharp.BACnet_Def
{
    public static class FileCoding
    {
        public static class Read
        {
            public static List<byte> EecodeAtomicReadFile(uint instance, int file_start, int seq_length)
            {
                List<byte> bytes = new List<byte>();
                // Service Request (var part of APDU):
                #region ObjectIdentifier
                //Application Tag Number=12(BACnetObjectIndetifier)<<4 | Application Tag=0<<3 | Length=4
                bytes.Add(0xC4);

                //file(10)<<22 | instance(1024)
                UInt32 value = (Int32)BACnetEnums.BACNET_OBJECT_TYPE.OBJECT_FILE;
                value = value << BACnetEnums.BACNET_INSTANCE_BITS;
                value |= (instance & BACnetEnums.BACNET_MAX_INSTANCE);
                byte[] temp = new byte[4];
                temp = BitConverter.GetBytes(value);
                bytes.Add(temp[3]);
                bytes.Add(temp[2]);
                bytes.Add(temp[1]);
                bytes.Add(temp[0]);
                #endregion

                #region Stream Access
                //opening tag(context specific=0x08 | context tag number=0 | opening tag=0x06)
                bytes.Add(0x08 | 0x00 | 0x06);

                //File Start Position = 0(Application signed)
                temp = BitConverter.GetBytes(file_start);
                Array.Reverse(temp);
                byte start_tag = (byte)temp.Length;
                start_tag |= (int)BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_SIGNED_INT << 4;
                start_tag |= 0;//Application Tag
                bytes.Add(start_tag);
                bytes.AddRange(temp);

                temp = BitConverter.GetBytes(seq_length);
                Array.Reverse(temp);
                bytes.Add((byte)
                    (temp.Length//Length(ex: 0x05A6.length=2)
                    | (int)BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_UNSIGNED_INT << 4
                    | 0x00)//Application Tag
                );
                bytes.AddRange(temp);

                //closing tag(context specific=0x08 | context tag number=0 | closing tag=0x07)
                bytes.Add(0x08 | 0x00 | 0x07);
                #endregion

                return bytes;
            }

            public static bool DecodeAtomicReadFile(byte[] bytes, out byte[] file_buffer)
            {
                uint pos = 0;
                byte tag_num;
                uint value;

                #region check end_of_file
                pos = BACnetBase.decode_tag_number_value(ref bytes, pos, out tag_num, out value);
                if (tag_num != (int)BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_BOOLEAN)
                {
                    Exception ex = new Exception("(warning)Decode Err(end_of_file is not boolean)(file)\n" +
                        "tag number=" + tag_num.ToString("X2") + " must 0x01");
                    throw ex;
                }
                bool isFileEnd = (value == 1);
                #endregion

                #region Stream Access
                //opening Tag
                if (!BACnetBase.IS_OPENING_TAG(bytes[pos++]))
                    throw new Exception("(waring)Tag Err-opening tag(file)");

                //file start
                pos = BACnetBase.decode_tag_number_value(ref bytes, pos, out tag_num, out value);
                if (tag_num != (int)BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_SIGNED_INT)
                    throw new Exception("(waring)Application Tag Type Err-signed-" + tag_num + "(file)");
                if (value >= 4)
                    throw new Exception("(waring)file start decode Err , length=" + value + "cannot convert to Int32");
                byte[] temp = new byte[4] { 0, 0, 0, 0 };
                for (uint i = 4 - value; i < 4; i++)
                    temp[i] = bytes[pos++];
                Array.Reverse(temp);
                uint file_start = BitConverter.ToUInt32(temp, 0);//unuse

                //file data
                pos = BACnetBase.decode_tag_number_value(ref bytes, pos, out tag_num, out value);
                if (tag_num != (int)BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_OCTET_STRING)
                    throw new Exception("(waring)Application Tag Type Err-octet string-" + tag_num + "(file)");
                uint data_len = value;//unuse
                #endregion
                if (bytes.Length <= pos + data_len)
                    throw new Exception("(warning)data length Err: " +
                        "total=" + bytes.Length + " from " + pos + " length " + data_len);
                file_buffer = new byte[data_len];
                Array.Copy(bytes, pos, file_buffer, 0, data_len);
                return isFileEnd;
            }
        }

        public static class Write
        {
            public static List<byte> EncodeAtomicWriteFile(uint instance, Int16 file_start, byte[] file_data)
            {
                List<byte> bytes = new List<byte>();
                // Service Request (var part of APDU):
                #region ObjectIdentifier
                //Application Tag Number=12(BACnetObjectIndetifier)<<4 | Application Tag=0<<3 | Length=4
                bytes.Add(0xC4);

                //file(10)<<22 | instance(1024)
                UInt32 value = (Int32)BACnetEnums.BACNET_OBJECT_TYPE.OBJECT_FILE;
                value = value << BACnetEnums.BACNET_INSTANCE_BITS;
                value |= (instance & BACnetEnums.BACNET_MAX_INSTANCE);
                byte[] temp = new byte[4];
                temp = BitConverter.GetBytes(value);
                Array.Reverse(temp);
                bytes.AddRange(temp);
                #endregion

                #region Stream Access
                //opening tag(context specific=0x08 | context tag number=0 | opening tag=0x06)
                bytes.Add(0x08 | 0x00 | 0x06);

                //Start Position
                temp = BitConverter.GetBytes(file_start);
                Array.Reverse(temp);
                bytes.Add((byte)(
                    temp.Length//Length(1396.length=2)
                    | (int)BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_SIGNED_INT << 4
                    | 0x00//Application Tag
                ));
                bytes.AddRange(temp);

                //File Data =Extended tag+Data
                // (Extended tag=data length(bytes.length))
                byte extended = 0x05//Extended Tag
                    | ((int)BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_OCTET_STRING << 4)
                    | 0x00;//Application Tag
                bytes.Add(extended);
                temp = BitConverter.GetBytes((UInt16)file_data.Length);
                Array.Reverse(temp);
                bytes.Add(0xfe);//(extend)16 bits data behind
                bytes.AddRange(temp);

                bytes.AddRange(file_data);

                //closing tag(context specific=0x08 | context tag number=0 | closing tag=0x07)
                bytes.Add(0x08 | 0x00 | 0x07);
                #endregion

                return bytes;
            }
        }
    }
}
