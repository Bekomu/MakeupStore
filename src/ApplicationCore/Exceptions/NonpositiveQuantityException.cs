using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Exceptions
{
    public class NonpositiveQuantityException : Exception
    {
        public NonpositiveQuantityException() : base("At least one item contains a nonpositive value.")
        {

        }
    }
}
