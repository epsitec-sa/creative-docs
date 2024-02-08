//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types.Exceptions
{
	/// <summary>
	/// The <c>InvalidTypeObjectException</c> is thrown by <c>TypeRosetta</c> when it
	/// cannot derive type information from a given type object.
	/// </summary>
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
	}
}
