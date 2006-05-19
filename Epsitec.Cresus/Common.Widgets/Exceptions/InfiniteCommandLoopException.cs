//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Exceptions
{
	/// <summary>
	/// The <c>InfiniteCommandLoopException</c> is thrown when a command loop is
	/// detected by the command dispatcher.
	/// </summary>
	[System.Serializable]

	public class InfiniteCommandLoopException : System.ApplicationException, System.Runtime.Serialization.ISerializable
	{
		public InfiniteCommandLoopException()
		{
		}

		public InfiniteCommandLoopException(string message)
			: base (message)
		{
		}

		public InfiniteCommandLoopException(string message, System.Exception inner_exception)
			: base (message, inner_exception)
		{
		}


		#region ISerializable Members
		protected InfiniteCommandLoopException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base (info, context)
		{
		}

		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
