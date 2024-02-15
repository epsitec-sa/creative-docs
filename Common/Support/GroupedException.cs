using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Epsitec.Common.Support.Extensions;

namespace Epsitec.Common.Support
{
    public sealed class GroupedException : Exception
    {
        internal GroupedException(IEnumerable<Exception> exceptions)
            : base()
        {
            this.exceptions = exceptions.AsReadOnlyCollection();
        }

        public ReadOnlyCollection<Exception> Exceptions
        {
            get { return this.exceptions; }
        }

        private readonly ReadOnlyCollection<Exception> exceptions;
    }
}
