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
    public class Wz_Header
    {
        public Wz_Header(string signature, string copyright, string file_name, int head_size, long data_size, long file_size, long dataStartPosition)
        {
            this.Signature = signature;
            this.Copyright = copyright;
            this.FileName = file_name;
            this.HeaderSize = head_size;
            this.DataSize = data_size;
            this.FileSize = file_size;
            this.DataStartPosition = dataStartPosition;
            this.VersionChecked = false;
        }

        public string Signature { get; private set; }
        public string Copyright { get; private set; }
        public string FileName { get; private set; }

        public int HeaderSize { get; private set; }
        public long DataSize { get; private set; }
        public long FileSize { get; private set; }
        public long DataStartPosition { get; private set; }
        public long DirEndPosition { get; set; }

        public bool VersionChecked { get; set; }
        public Wz_Capabilities Capabilities { get; internal set; }

        public int WzVersion => this.versionDetector?.WzVersion ?? 0;
        public uint HashVersion => this.versionDetector?.HashVersion ?? 0;
        public bool TryGetNextVersion() => this.versionDetector?.TryGetNextVersion() ?? false;

        private IWzVersionDetector versionDetector;

        public void SetWzVersion(int wzVersion)
        {
            this.versionDetector = new FixedVersion(wzVersion);
        }

        public void SetOrdinalVersionDetector(int encryptedVersion)
        {
            this.versionDetector = new OrdinalVersionDetector(encryptedVersion);
        }

        public bool HasCapabilities(Wz_Capabilities cap)
        {
            return cap == (this.Capabilities & cap);
        }

        public static int CalcHashVersion(int wzVersion)
        {
            int sum = 0;
            string versionStr = wzVersion.ToString(System.Globalization.CultureInfo.InvariantCulture);
            for (int j = 0; j < versionStr.Length; j++)
            {
                sum <<= 5;
                sum += (int)versionStr[j] + 1;
            }
            
            return sum;
        }

        public interface IWzVersionDetector
        {
            bool TryGetNextVersion();
            int WzVersion { get; }
            uint HashVersion { get; }
        }

        public class FixedVersion : IWzVersionDetector
        {
            public FixedVersion(int wzVersion)
            {
                this.WzVersion = wzVersion;
                this.HashVersion = (uint)CalcHashVersion(wzVersion);
            }

            private bool hasReturned;


            public int WzVersion { get; private set; }

            public uint HashVersion { get; private set; }

            public bool TryGetNextVersion()
            {
                if (!hasReturned)
                {
                    hasReturned = true;
                    return true;
                }

                return false;
            }
        }

        public class OrdinalVersionDetector : IWzVersionDetector
        {
            public OrdinalVersionDetector(int encryptVersion)
            {
                this.EncryptedVersion = encryptVersion;
                this.versionTest = new List<int>();
                this.hasVersionTest = new List<uint>();
                this.startVersion = -1;
            }

            public int EncryptedVersion { get; private set; }

            private int startVersion;
            private List<int> versionTest;
            private List<uint> hasVersionTest;

            public int WzVersion
            {
                get
                {
                    int idx = this.versionTest.Count - 1;
                    return idx > -1 ? this.versionTest[idx] : 0;
                }
            }

            public uint HashVersion
            {
                get
                {
                    int idx = this.hasVersionTest.Count - 1;
                    return idx > -1 ? this.hasVersionTest[idx] : 0;
                }
            }

            public bool TryGetNextVersion()
            {
                for (int i = startVersion + 1; i < Int16.MaxValue; i++)
                {
                    int sum = CalcHashVersion(i);
                    int enc = 0xff
                        ^ ((sum >> 24) & 0xFF)
                        ^ ((sum >> 16) & 0xFF)
                        ^ ((sum >> 8) & 0xFF)
                        ^ ((sum) & 0xFF);

                    // if encver does not passed, try every version one by one
                    if (enc == this.EncryptedVersion)
                    {
                        this.versionTest.Add(i);
                        this.hasVersionTest.Add((uint)sum);
                        startVersion = i;
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
