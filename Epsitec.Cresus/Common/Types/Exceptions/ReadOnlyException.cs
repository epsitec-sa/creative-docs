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
		/// <param name="readOnlyObject">The <see cref="IReadOnly"/> object that has been written to.</param>
		/// <param name="message">The message of the exception.</param>
		public ReadOnlyException(IReadOnly readOnlyObject = null, string message = null)
			: base (message ?? "")
		{
			this.readOnlyObject = readOnlyObject;
		}


		/// <summary>
		/// The <see cref="IReadOnly"/> object that has been written to.
		/// </summary>
		private readonly IReadOnly readOnlyObject;
		

	}


}
