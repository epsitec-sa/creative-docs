//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types.Exceptions
{
	/// <summary>
	/// The <c>InvalidTypeObjectException</c> is thrown by <c>TypeRosetta</c> when it
	/// cannot derive type information from a given type object.
	/// </summary>
	[System.Serializable]

	public class InvalidTypeObjectException : System.ApplicationException, System.Runtime.Serialization.ISerializable
	{
		public InvalidTypeObjectException()
		{
		}

		public InvalidTypeObjectException(string message)
			: base (message)
		{
		}

		public InvalidTypeObjectException(object type) : this (string.Format ("Invalid type object {0}", type == null ? "null" : type.ToString ()))
		{
		}

		public InvalidTypeObjectException(string message, System.Exception inner_exception)
			: base (message, inner_exception)
		{
		}


		#region ISerializable Members
		protected InvalidTypeObjectException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
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
