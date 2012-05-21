//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

namespace Epsitec.Common.Types.Exceptions
{
	/// <summary>
	/// An instance of the <c>ReadOnlyException</c> should be thrown whenever an instance of
	/// <see cref="IReadOnly"/> in read only state is written to.
	/// </summary>
	public sealed class ReadOnlyException : System.InvalidOperationException
	{
		/// <summary>
		/// Builds a new instance of <c>ReadOnlyException</c>.
		/// </summary>
		/// <param name="obj">The <see cref="IReadOnly"/> object that has been written to.</param>
		/// <param name="message">The message of the exception.</param>
		public ReadOnlyException(IReadOnly obj = null, string message = null)
			: base (message ?? "")
		{
			this.readOnlyObject = obj;
		}


		/// <summary>
		/// The <see cref="IReadOnly"/> object that has been written to.
		/// </summary>
		private readonly IReadOnly readOnlyObject;
	}
}
