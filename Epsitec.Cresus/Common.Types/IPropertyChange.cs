//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IPropertyChange définit un événement PropertyChanged.
	/// </summary>
	public interface IPropertyChange
	{
		event Support.PropertyChangedEventHandler		PropertyChanged;
	}
}
