using System;
using System.Collections.Generic;
using UnityEngine;

namespace LegacySC
{
    public class GRPfile
    {
        private byte[] _data = null;

        public int FrameCount { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public GRPsprite[] Sprites { get; private set; } = null;
        public short[] FrameIndices { get; private set; } = null;

        // sprite by Frame Index
        public GRPsprite this[int i] {
            get { return Sprites[FrameIndices[i]]; }
        }

        public GRPfile(byte[] data)
        {
            _data = data;

            FrameCount = BitConverter.ToInt16(data, 0);
            Width = BitConverter.ToInt16(data, 2);
            Height = BitConverter.ToInt16(data, 4);

            int pixelDataOffset = 6 + FrameCount * 8;

            FrameIndices = new short[FrameCount];

            var spriteList = new List<GRPsprite>();

            for(int i=0; i<FrameCount; i++)
            {
                var spr = new GRPsprite(data, 6 + i * 8, pixelDataOffset);
                var similar = spriteList.FindIndex((a) => a.Equals(spr));

                if (similar == -1)
                {
                    spriteList.Add(spr);
                    FrameIndices[i] = (short)(spriteList.Count - 1);
                }
                else
                    FrameIndices[i] = (short)similar;
            }

            Sprites = spriteList.ToArray();
        }
    }
}
