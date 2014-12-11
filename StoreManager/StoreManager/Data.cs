//The MIT License (MIT)

//Copyright (c) 2014 Richard Ranft

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoreManager
{
    public struct Data
    {
        // general data structure.  kitchen-sink to allow
        // use as parameter for delegate functions
        public string srcURI;           // /f (full network path)
        public string storeURI;         // /s (full network path)
        public string srcPath;          // /f (local path)
        public string storePath;        // /s (local path)
        public string srcShare;         // /g
        public string prefix;           // /m
        public string symbolVisibility; // /h {PUB|PRI}
        public string iD;               // /i
        public string usePointers;      // /p
        public string message;          // -:MSG
        public string relative;         // -:REL (requires /p)
        public string recurse;          // /r
        public string product;          // /t
        public string version;          // /v
        public string comment;          // /c
        public string logfile;          // /d
        public string verbose;          // /o
        public string indexfile;        // /x
        public string appendindex;      // /a
        public string readIndex;        // /y
        public string appendindexfile;  // /yi
        public string onlyVisibility;   // /z {PUB|PRI}
        public string compress;         // /compress
        public List<String> stringList; // additional data
        public int integervalue;        // additional data
    };
}
