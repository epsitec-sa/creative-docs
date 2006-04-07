//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// Summary description for CaptionOnlyController.
	/// </summary>
	public class CaptionOnlyController : AbstractController
	{
		public CaptionOnlyController()
		{
		}
		
		
		public override void CreateUI(Widget panel)
		{
			this.caption_label = new StaticText (panel);
			
			this.caption_label.Anchor        = AnchorStyles.Top | AnchorStyles.LeftAndRight;
			this.caption_label.Margins = new Drawing.Margins (0, 0, 8, 0);
			
			this.OnCaptionChanged ();
		}
		
		public override void SyncFromAdapter(SyncReason reason)
		{
		}
		
		public override void SyncFromUI()
		{
		}
	}
}
