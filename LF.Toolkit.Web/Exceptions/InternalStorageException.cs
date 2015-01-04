using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LF.Toolkit.Web.Exceptions
{
    public class InternalStorageException : Exception
    {
        public Type TargetType { get; private set; }

        public InternalStorageException()
        {
        }

        public InternalStorageException(Type targetType, Exception e)
            : base(e.Message)
        {
            this.TargetType = targetType;
        }
    }
}
