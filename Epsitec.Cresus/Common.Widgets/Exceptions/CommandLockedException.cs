//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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

		public CommandLockedException(string message, System.Exception innerException)
			: base (message, innerException)
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
