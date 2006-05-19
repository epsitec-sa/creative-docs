//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types.Exceptions
{
	/// <summary>
	/// The <c>WrongBaseTypeException</c> is thrown when a class does not derive
	/// from <c>DependencyObject</c> and tries to register a <c>DependencyProperty</c>.
	/// </summary>
	[System.Serializable]

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

		public WrongBaseTypeException(string message, System.Exception inner_exception)
			: base (message, inner_exception)
		{
		}


		#region ISerializable Members
		protected WrongBaseTypeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
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
