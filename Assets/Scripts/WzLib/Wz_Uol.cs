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

namespace WzComparerR2.WzLib
{
    public class Wz_Uol
    {
        public Wz_Uol(string uol)
        {
            this.uol = uol;
        }

        private string uol;

        /// <summary>
        /// 获取或设置连接路径字符串。
        /// </summary>
        public string Uol
        {
            get { return uol; }
            set { uol = value; }
        }

        public Wz_Node HandleUol(Wz_Node currentNode)
        {
            if (currentNode == null || currentNode.ParentNode == null || string.IsNullOrEmpty(uol))
                return null;
            string[] dirs = this.uol.Split('/');
            currentNode = currentNode.ParentNode;

            bool outImg = false;

            for (int i = 0; i < dirs.Length; i++)
            {
                string dir = dirs[i];
                if (dir == "..")
                {
                    if (currentNode.ParentNode == null)
                    {
                        Wz_Image img = currentNode.GetValueEx<Wz_Image>(null);
                        if (img != null)
                        {
                            currentNode = img.OwnerNode.ParentNode;
                            outImg = true;
                        }
                        else
                        {
                            currentNode = null;
                        }
                    }
                    else
                    {
                        currentNode = currentNode.ParentNode;
                    }
                }
                else
                {
                    var dirNode = currentNode.FindNodeByPath(dir);

                    if (dirNode == null && outImg)
                    {
                        dirNode = currentNode.FindNodeByPath(true, dir + ".img");
                        if (dirNode.GetValueEx<Wz_Image>(null) != null)
                        {
                            outImg = false;
                        }
                    }

                    currentNode = dirNode;
                }
                if (currentNode == null)
                    return null;
            }
            return currentNode;
        }
    }
}
