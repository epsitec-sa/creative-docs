namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>IFieldProxy</c> interface is used as a placeholder for a value in the database which
	/// is not yet loaded in the memory.
	/// </summary>
	internal interface IValueProxy
	{
		/// <summary>
		/// Gets the real value represented by the current instance.
		/// </summary>
		/// <returns></returns>
		object GetValue();
	}
}
