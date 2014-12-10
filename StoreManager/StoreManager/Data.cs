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
