using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseService
{
    internal class EnumDef
    {
        public enum DBConnectionState : short
        {
            UNKNOWN = -1,
            CLOSED = 0,
            OPEN = 1,
            CONNECTING = 2,
            BROKEN = 16,
        }

        public enum DBErrorCode : short
        {
            UNKNOWN = 1,
            CONNECTION_ERROR = 2,
            CONNECTION_CLOSED = 3,
            CONNECTION_OPEN_INVALID_PARAMS = 4,
            EXECUTE_ERROR = 5,
            INVALID_SQL_OBJECT = 208,
            INVALID_COLUMN = 207
        }
    }
}
