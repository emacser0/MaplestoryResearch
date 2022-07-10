/*
 * The MIT License (MIT)
 * 
 * Copyright © 2015 Kagamia Studio
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WzComparerR2.WzLib
{
    public class Wz_Directory
    {
        public Wz_Directory(string name, int size, int cs32, uint hashOff, uint hashPos, Wz_File wz_f)
        {
            this.Name = name;
            this.WzFile = wz_f;
            this.Size = size;
            this.Checksum = cs32;
            this.HashedOffset = hashOff;
            this.HashedOffsetPosition = hashPos;
        }

        public string Name { get; set; }
        public Wz_File WzFile { get; set; }
        public int Size { get; set; }
        public int Checksum { get; set; }
        public uint HashedOffset { get; set; }
        public uint HashedOffsetPosition { get; set; }
        public long Offset { get; set; }
    }
}
