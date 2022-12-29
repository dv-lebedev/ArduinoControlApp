/*
Copyright(c) 2022-2023 Denis Lebedev
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
    http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Diagnostics;

namespace ArduinoControlApp.Coder
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

        readonly byte[] _buffer = new byte[ushort.MaxValue];
        int _size;

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

                    var p = new Package { };

                    p.Timestamp = DateTime.Now;
                    p.Addr = _buffer[ptr + 2];
                    p.Size = packageSize;
                    p.CrcHeader = headerCrc;
                    p.Data = new byte[packageSize];
                    p.CrcOverall = overallCrc;
                    p.CrcHeaderErr = headerCrcErr;
                    p.OverallCrcErr = overallCrcErr;
 
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
