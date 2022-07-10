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
using System.Text;
using System.Drawing;

namespace WzComparerR2.WzLib
{
    public class Wz_Vector
    {
        public Wz_Vector(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        private int x;
        private int y;

        /// <summary>
        /// 获取或设置向量的X值。
        /// </summary>
        public int X
        {
            get { return x; }
            set { x = value; }
        }

        /// <summary>
        /// 获取或设置向量的Y值。
        /// </summary>
        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public static implicit operator Point(Wz_Vector vector)
        {
            return vector == null ? new Point() : new Point(vector.x, vector.y);
        }
    }
}
