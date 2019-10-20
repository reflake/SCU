using System;
using UnityEngine;

namespace LegacySC {

    public class GRPsprite : IEquatable<GRPsprite>
    {
        private byte[] _data;

        public byte OffsetX { get; private set; }
        public byte OffsetY { get; private set; }
        public byte Width { get; private set; }
        public byte Height { get; private set; }
        public int DataLineOffset { get; private set; }

        public GRPsprite(byte[] data, int offset, int pixelDataOffset)
        {
            _data = data;

            OffsetX = data[offset];
            OffsetY = data[offset + 1];
            Width = data[offset + 2];
            Height = data[offset + 3];
            DataLineOffset = BitConverter.ToInt32(data, offset + 4);
        }

        public void GetUncompressedData(ref byte[] data, int startIndex = 0, int stride = 0)
        {
            stride = stride == 0 ? Width * 2 : stride;

            for(int i=0; i<Height; i++)
            {
                int RLEoffset = BitConverter.ToUInt16(_data, i * 2 + DataLineOffset);
                RLEoffset += DataLineOffset;

                EncodeRLE(RLEoffset, startIndex, ref data);

                startIndex -= stride;
            }
        }

        private void EncodeRLE(int offset, int outputOffset, ref byte[] data)
        {
            int pixels = 0;
            int count;
            bool plainFlag;

            while (pixels < Width)
            {
                int instructionByte = _data[offset++];

                if ((instructionByte & 0x80) != 0)
                {
                    pixels += instructionByte & ~0x80;
                    continue;
                }

                plainFlag = (instructionByte & 0x40) != 0;
                count = pixels + (instructionByte & 0x3F);

                for (; pixels < count; pixels++)
                {
                    if (plainFlag)
                        data[outputOffset + pixels * 2] = _data[offset];
                    else
                        data[outputOffset + pixels * 2] = _data[offset++];

                    data[outputOffset + pixels * 2 + 1] = 0xFF;
                }

                if (plainFlag)
                    offset++;
            }

            Debug.Assert(pixels == Width);
        }

        bool IEquatable<GRPsprite>.Equals(GRPsprite other)
        {
            return OffsetX == other.OffsetX &&
                  OffsetY == other.OffsetY &&
                  Width == other.Width &&
                  Height == other.Height &&
                  DataLineOffset == other.DataLineOffset;
        }
    }

}
