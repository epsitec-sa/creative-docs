//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 30/08/2004

namespace Epsitec.Common.Support.Data
{
	/// <summary>
	/// L'interface IDisablableChangedSource permet de désactiver la
	/// notification des messages 'Changed' de IChangedSource.
	/// </summary>
	public interface IDisablableChangedSource : IChangedSource
	{
		bool ChangedEventDisabled	{ get; }
		
		void DisableChangedEvent();
		void EnableChangedEvent();
		void RaiseChangedEvent();
	}
}
