//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Adapters
{
	/// <summary>
	/// Summary description for IAdapter.
	/// </summary>
	public interface IAdapter : Support.Data.IChangedSource
	{
		Binders.IBinder		Binder		{ get; set; }
		
		void SyncFromBinder();
	}
}
