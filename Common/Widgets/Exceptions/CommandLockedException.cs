//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Exceptions
{
    /// <summary>
    /// The <c>CommandLockedException</c> is thrown when an attempt to modify a
    /// locked command is done.
    /// </summary>
    public class CommandLockedException
        : System.ApplicationException,
            System.Runtime.Serialization.ISerializable
    {
        public CommandLockedException() { }

        public CommandLockedException(string message)
            : base(message) { }

        public CommandLockedException(string message, System.Exception innerException)
            : base(message, innerException) { }
    }
}
