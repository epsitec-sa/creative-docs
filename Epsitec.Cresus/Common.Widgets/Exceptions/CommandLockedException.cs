//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Exceptions
{
	/// <summary>
	/// The <c>CommandLockedException</c> is thrown when an attempt to modify a
	/// locked command is done.
	/// </summary>
	[System.Serializable]

	public class CommandLockedException : System.ApplicationException, System.Runtime.Serialization.ISerializable
	{
		public CommandLockedException()
		{
		}

		public CommandLockedException(string message)
			: base (message)
		{
		}

		public CommandLockedException(string message, System.Exception inner_exception)
			: base (message, inner_exception)
		{
		}


		#region ISerializable Members
		protected CommandLockedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
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
