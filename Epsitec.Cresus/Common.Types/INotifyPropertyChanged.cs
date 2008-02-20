//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
