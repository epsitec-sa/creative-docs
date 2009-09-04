//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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

		public InvalidTypeObjectException(string message, System.Exception innerException)
			: base (message, innerException)
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
