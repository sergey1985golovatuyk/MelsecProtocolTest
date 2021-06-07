using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MelsecProtocolTest
{
        public enum ErrorCode
        {
            NoError = 0,
            ConnectionError = 1,
            IPAddressNotAvailable = 2,
            WrongVarFromat = 3,
            WrongNumberReceivedBytes = 4

        }

    public enum VarType
    {
        Word
    }

    }
