//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// Summary description for DataFolderController.
	/// </summary>
	public class DataFolderController : AbstractController
	{
		public DataFolderController()
		{
		}
		
		
		public override void CreateUI(Widget panel)
		{
			//	TODO: implémenter CreateUI.
			
			throw new System.NotImplementedException ("CreateUI not implemented (yet).");
		}
		
		public override void SyncFromAdapter(SyncReason reason)
		{
			Adapters.DataFolderAdapter adapter = this.Adapter as Adapters.DataFolderAdapter;
			
			if (adapter != null)
			{
				//	TODO: ...
			}
		}
		
		public override void SyncFromUI()
		{
			Adapters.DataFolderAdapter adapter = this.Adapter as Adapters.DataFolderAdapter;
			
			if (adapter != null)
			{
				//	TODO: ...
			}
		}
		
		
		private void HandleTextFieldTextChanged(object sender)
		{
			this.SyncFromUI ();
		}
	}
}
