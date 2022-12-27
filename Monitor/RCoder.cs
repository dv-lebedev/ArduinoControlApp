using System;
using System.Diagnostics;

namespace ArduinoDecoder
{
    //Протокол передачи данных по USB
    /*
	--------------------------------
	название		| кол-во байт
	--------------------------------
	Packet Header 	| 2 
	Address			| 1
	PacketLength	| 2
	HeaderCRC		| 1
	PacketDataLength| N
	OverallCRC		| 1
	--------------------------------
	*/

    public static class CRC8ATM
    {
        static readonly byte[] table = new byte[256] {
 0x00, 0x07, 0x0e, 0x09, 0x1c, 0x1b, 0x12, 0x15, 0x38, 0x3f, 0x36, 0x31,
 0x24, 0x23, 0x2a, 0x2d, 0x70, 0x77, 0x7e, 0x79, 0x6c, 0x6b, 0x62, 0x65,
 0x48, 0x4f, 0x46, 0x41, 0x54, 0x53, 0x5a, 0x5d, 0xe0, 0xe7, 0xee, 0xe9,
 0xfc, 0xfb, 0xf2, 0xf5, 0xd8, 0xdf, 0xd6, 0xd1, 0xc4, 0xc3, 0xca, 0xcd,
 0x90, 0x97, 0x9e, 0x99, 0x8c, 0x8b, 0x82, 0x85, 0xa8, 0xaf, 0xa6, 0xa1,
 0xb4, 0xb3, 0xba, 0xbd, 0xc7, 0xc0, 0xc9, 0xce, 0xdb, 0xdc, 0xd5, 0xd2,
 0xff, 0xf8, 0xf1, 0xf6, 0xe3, 0xe4, 0xed, 0xea, 0xb7, 0xb0, 0xb9, 0xbe,
 0xab, 0xac, 0xa5, 0xa2, 0x8f, 0x88, 0x81, 0x86, 0x93, 0x94, 0x9d, 0x9a,
 0x27, 0x20, 0x29, 0x2e, 0x3b, 0x3c, 0x35, 0x32, 0x1f, 0x18, 0x11, 0x16,
 0x03, 0x04, 0x0d, 0x0a, 0x57, 0x50, 0x59, 0x5e, 0x4b, 0x4c, 0x45, 0x42,
 0x6f, 0x68, 0x61, 0x66, 0x73, 0x74, 0x7d, 0x7a, 0x89, 0x8e, 0x87, 0x80,
 0x95, 0x92, 0x9b, 0x9c, 0xb1, 0xb6, 0xbf, 0xb8, 0xad, 0xaa, 0xa3, 0xa4,
 0xf9, 0xfe, 0xf7, 0xf0, 0xe5, 0xe2, 0xeb, 0xec, 0xc1, 0xc6, 0xcf, 0xc8,
 0xdd, 0xda, 0xd3, 0xd4, 0x69, 0x6e, 0x67, 0x60, 0x75, 0x72, 0x7b, 0x7c,
 0x51, 0x56, 0x5f, 0x58, 0x4d, 0x4a, 0x43, 0x44, 0x19, 0x1e, 0x17, 0x10,
 0x05, 0x02, 0x0b, 0x0c, 0x21, 0x26, 0x2f, 0x28, 0x3d, 0x3a, 0x33, 0x34,
 0x4e, 0x49, 0x40, 0x47, 0x52, 0x55, 0x5c, 0x5b, 0x76, 0x71, 0x78, 0x7f,
 0x6a, 0x6d, 0x64, 0x63, 0x3e, 0x39, 0x30, 0x37, 0x22, 0x25, 0x2c, 0x2b,
 0x06, 0x01, 0x08, 0x0f, 0x1a, 0x1d, 0x14, 0x13, 0xae, 0xa9, 0xa0, 0xa7,
 0xb2, 0xb5, 0xbc, 0xbb, 0x96, 0x91, 0x98, 0x9f, 0x8a, 0x8d, 0x84, 0x83,
 0xde, 0xd9, 0xd0, 0xd7, 0xc2, 0xc5, 0xcc, 0xcb, 0xe6, 0xe1, 0xe8, 0xef,
 0xfa, 0xfd, 0xf4, 0xf3
};

        public static byte Get(byte[] bytes, int len, int start = 0, byte crc = 0x00)
        {
            for (int i = start; i < start + len; i++)
            {
                crc = table[crc ^ bytes[i]];
            }

            return crc;
        }

        public static byte Get(byte value, byte prevCrc = 0x00)
        {
            return table[prevCrc ^ value];
        }
    }

    public class Package
    {
        public DateTime Timestamp { get;  set; }
        public byte Addr { get;  set; }
        public int Size { get;  set; }
        public byte CrcHeader { get;  set; }
        public byte[] Data { get;  set; }
        public ushort CrcOverall { get;  set; }
        public bool CrcHeaderErr { get; internal set; }
        public bool OverallCrcErr { get; internal set; }
    }

    public class RCoder
    {
        const int PACKET_HEADER_SIZE = 2;
        const int ADDRESS_SIZE = 1;
        const int PACKET_LENGTH_SIZE = 2;
        const int HEADER_CRC_SIZE = 1;
        const int OVERALL_CRC_SIZE = 1;
        const int MIN_PACKAGE_SIZE = OVERALL_HEADER_SIZE + 2;

        const int OVERALL_HEADER_SIZE =
            PACKET_HEADER_SIZE +
            ADDRESS_SIZE +
            PACKET_LENGTH_SIZE +
            HEADER_CRC_SIZE;

        const ushort HEADER = 0x5E4D;

        private readonly byte[] _buffer = new byte[ushort.MaxValue];
        private int _size;

        public event EventHandler<Package> OnPackageReceived;
        public event EventHandler OnCrcHeaderError;
        public event EventHandler OnCrcOverallError;

        public RCoder()
        {
        }

        void Push(byte[] data, int offset, int count)
        {
            if (_size + count >= _buffer.Length) // TODO >= || >
            {
                throw new IndexOutOfRangeException();
            }

            Buffer.BlockCopy(data, offset, _buffer, _size, count);
            _size += count;
        }

        void LeftShift(int offset)
        {
            if (offset == 0)
            {
                return;
            }

            int count = _size - offset;
            Buffer.BlockCopy(_buffer, offset, _buffer, 0, count);
            _size = count;

            for (int i = _size; i < _buffer.Length; i++)
            {
                _buffer[i] = 0;
            }
        }

        public void Decode(byte[] data, int offset, int count)
        {
            Push(data, offset, count);

            if (_size < MIN_PACKAGE_SIZE) return;

            int ptr = 0;

            for (; ptr < _size - MIN_PACKAGE_SIZE;)
            {
                if ((_buffer[ptr] << 8 | _buffer[ptr + 1]) == HEADER)
                {
                    int packageSize = _buffer[ptr + 3] << 8 | _buffer[ptr + 4];

                    int lastByteIndex = ptr + 5 + packageSize + 1; // последний байт пакета

                    if (lastByteIndex >= _size)
                    {
                        LeftShift(ptr);
                        return;
                    }

                    byte headerCrc = CRC8ATM.Get(_buffer, 5, ptr);
                    byte overallCrc = CRC8ATM.Get(_buffer, packageSize, ptr + 6);

                    bool headerCrcErr = headerCrc != _buffer[ptr + 5];
                    bool overallCrcErr = overallCrc != _buffer[lastByteIndex];

                    if (headerCrcErr) Debug.WriteLine("[crc header]");
                    if (overallCrcErr) Debug.WriteLine("[crc overall");

                    if (headerCrcErr || overallCrcErr)
                    {
                        ptr++;
                        continue;
                    }

                    Package p = new Package();

                    p.Timestamp = DateTime.Now;
                    p.Addr = _buffer[ptr + 2];
                    p.Size = packageSize;
                    p.CrcHeader = headerCrc;
                    p.Data = new byte[packageSize];
                    p.CrcOverall = overallCrc;
 
                    Buffer.BlockCopy(_buffer, ptr + 6, p.Data, 0, packageSize);

                    OnPackageReceived?.Invoke(this, p);

                    ptr = lastByteIndex + 1;
                }
                else
                {
                    ptr++;
                    Debug.WriteLine("[garbage] " + ptr);
                }
            }

            LeftShift(ptr);
        }

        public static byte[] CreatePackage(byte addr, byte[] data)
        {
            byte[] res = new byte[OVERALL_HEADER_SIZE + data.Length + OVERALL_CRC_SIZE];

            res[0] = 0x5E;
            res[1] = 0x4D;

            res[2] = addr; // адрес

            byte[] size = BitConverter.GetBytes((ushort)data.Length); //размер
            Array.Reverse(size, 0, size.Length);
            Buffer.BlockCopy(size, 0, res, 3, size.Length);

            res[5] = CRC8ATM.Get(res, 5, 0); // crc для начала пакета

            Buffer.BlockCopy(data, 0, res, 6, data.Length);  //данные

            res[res.Length - 1] = CRC8ATM.Get(data, data.Length); // crc данные

            return res;
        }
    }
}
