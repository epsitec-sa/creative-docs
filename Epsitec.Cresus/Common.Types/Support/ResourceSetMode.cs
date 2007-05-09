//	Copyright © 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
	}
}
