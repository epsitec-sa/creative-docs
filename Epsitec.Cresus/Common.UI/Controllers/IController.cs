//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// Summary description for IController.
	/// </summary>
	public interface IController
	{
		Adapters.IAdapter	Adapter		{ get; set; }
		string				Caption		{ get; set; }
		
		void CreateUI(Widget panel);
		void SyncFromUI();
		void SyncFromAdapter(SyncReason reason);
	}
}
