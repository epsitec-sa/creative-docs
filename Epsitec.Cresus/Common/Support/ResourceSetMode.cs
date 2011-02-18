//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>ResourceSetMode</c> is used to specify the action of a call to the
	/// <c>SetBundle</c> method of the resource manager.
	/// </summary>
	public enum ResourceSetMode
	{
		/// <summary>
		/// No action. This does nothing.
		/// </summary>
		None,
		
		/// <summary>
		/// Create the bundle if it does not existe; otherwise, generates an error.
		/// </summary>
		CreateOnly,
		
		/// <summary>
		/// Update the bundle if it exists; otherwise, generates an error.
		/// </summary>
		UpdateOnly,
		
		/// <summary>
		/// Create or update the bundle.
		/// </summary>
		Write,

		/// <summary>
		/// Create or update the bundle in memory only. This will not write the
		/// bundle to the resource provider, but just update the pool cache.
		/// </summary>
		InMemory,

		/// <summary>
		/// Remove the bundle. This will delete the associated file or entry in
		/// the database.
		/// </summary>
		Remove
	}
}
