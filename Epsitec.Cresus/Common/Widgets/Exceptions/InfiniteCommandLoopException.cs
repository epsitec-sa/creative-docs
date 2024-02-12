//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Exceptions
{
    /// <summary>
    /// The <c>InfiniteCommandLoopException</c> is thrown when a command loop is
    /// detected by the command dispatcher.
    /// </summary>
    public class InfiniteCommandLoopException
        : System.ApplicationException,
            System.Runtime.Serialization.ISerializable
    {
        public InfiniteCommandLoopException() { }

        public InfiniteCommandLoopException(string message)
            : base(message) { }

        public InfiniteCommandLoopException(string message, System.Exception innerException)
            : base(message, innerException) { }
    }
}
