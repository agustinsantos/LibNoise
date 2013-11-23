using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noise
{
    /// Invalid parameter exception
    ///
    /// An invalid parameter was passed to a libnoise function or method.
    public class ExceptionInvalidParam : Exception
    {
        public ExceptionInvalidParam()
        {
        }

        public ExceptionInvalidParam(string message)
            : base(message)
        {
        }
    }

    /// No module exception
    ///
    /// Could not retrieve a source module from a noise module.
    ///
    /// @note If one or more required source modules were not connected to a
    /// specific noise module, and its GetValue() method was called, that
    /// method will raise a debug assertion instead of this exception.  This
    /// is done for performance reasons.
    public class ExceptionNoModule : Exception
    {
        public ExceptionNoModule()
        {
        }

        public ExceptionNoModule(string message)
            : base(message)
        {
        }
    }

    /// Out of memory exception
    ///
    /// There was not enough memory to perform an action.
    public class ExceptionOutOfMemory : Exception
    {
        public ExceptionOutOfMemory()
        {
        }

        public ExceptionOutOfMemory(string message)
            : base(message)
        {
        }
    }

    /// Unknown exception
    ///
    /// libnoise raised an unknown exception.
    public class ExceptionUnknown : Exception
    {
        public ExceptionUnknown()
        {
        }

        public ExceptionUnknown(string message)
            : base(message)
        {
        }
    }
}
