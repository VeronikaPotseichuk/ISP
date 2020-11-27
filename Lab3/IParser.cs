using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    class IParser
    {
        internal Options ParseOptions();

        public static implicit operator IParser(XML_Parser v)
        {
            throw new NotImplementedException();
        }
    }
}
