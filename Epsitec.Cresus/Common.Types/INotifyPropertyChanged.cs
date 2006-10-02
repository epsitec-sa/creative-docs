//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Types
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs>;
	
	/// <summary>
	/// The <c>INotifyPropertyChanged</c> interface defines a <c>PropertyChanged</c> event.
	/// </summary>
	public interface INotifyPropertyChanged
	{
		event PropertyChangedEventHandler PropertyChanged;
	}
}
