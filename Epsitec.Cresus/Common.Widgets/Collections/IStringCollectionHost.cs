//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// L'interface IStringCollectionHost permet d'offrir le support pour
	/// la classe StringCollection.
	/// </summary>
	public interface IStringCollectionHost
	{
		void StringCollectionChanged();
		
		StringCollection	Items	{ get; }
	}
}
