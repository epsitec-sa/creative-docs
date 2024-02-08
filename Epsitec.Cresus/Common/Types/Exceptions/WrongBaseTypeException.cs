//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types.Exceptions
{
	/// <summary>
	/// The <c>WrongBaseTypeException</c> is thrown when a class does not derive
	/// from <c>DependencyObject</c> and tries to register a <c>DependencyProperty</c>.
	/// </summary>
	public class WrongBaseTypeException : System.ApplicationException, System.Runtime.Serialization.ISerializable
	{
		public WrongBaseTypeException()
		{
		}

		public WrongBaseTypeException(string message)
			: base (message)
		{
		}

		public WrongBaseTypeException(System.Type type) : this (string.Format ("Wrong base type for type {0}", type.FullName))
		{
		}

		public WrongBaseTypeException(string message, System.Exception innerException)
			: base (message, innerException)
		{
		}
	}
}
