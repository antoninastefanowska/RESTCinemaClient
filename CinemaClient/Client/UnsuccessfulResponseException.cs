using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaClient.Client
{
    public class UnsuccessfulResponseException : Exception
    {
        public int Code { get; private set; }

        public UnsuccessfulResponseException(int code, string message) : base(message)
        {
            Code = code;
        }
    }
}
