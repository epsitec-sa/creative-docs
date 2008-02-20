//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Dialogs.Helpers
{
	/// <summary>
	/// L'interface IFilterCollectionHost permet d'offrir le support pour
	/// la classe FilterCollection.
	/// </summary>
	public interface IFilterCollectionHost
	{
		void FilterCollectionChanged();
	}
}
