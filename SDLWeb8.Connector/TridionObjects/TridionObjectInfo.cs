using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreServiceWithWeb8.TridionObjects
{
    public class TridionObjectInfo
    {
        public int ObjectCount { get; set; }

        public object TridionObject { get; set; }

        public string ObjectTitle { get; set; }

        public string TcmUri { get; set; }
    }
}
