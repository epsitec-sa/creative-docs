//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>ICollectionType</c> interface describes a collection type.
	/// </summary>
	public interface ICollectionType
	{
		/// <summary>
		/// Gets the type used by the items in the collection.
		/// </summary>
		/// <value>The type used by the items in the collection.</value>
		INamedType ItemType
		{
			get;
		}
	}
}
