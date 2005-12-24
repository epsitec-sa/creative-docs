using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Action permet de g�rer les commandes du menu aide.
	/// </summary>
	[SuppressBundleSupport]
	public class Action : Abstract
	{
		public Action() : base()
		{
			this.title.Text = Res.Strings.Action.ActionMain;

			this.buttonSettings  = this.CreateIconButton("Settings", "2");
			this.buttonInfos     = this.CreateIconButton("Infos");
			this.buttonPageStack = this.CreateIconButton("PageStack");
			this.buttonKey       = this.CreateIconButton("KeyApplication");
			this.buttonUpdate    = this.CreateIconButton("UpdateApplication");
			this.buttonAbout     = this.CreateIconButton("AboutApplication");
			
			this.UpdateClientGeometry();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		public override void SetDocument(DocumentType type, InstallType install, DebugMode debug, Settings.GlobalSettings gs, Document document)
		{
			base.SetDocument(type, install, debug, gs, document);

			this.buttonKey.Visibility = (this.installType != InstallType.Freeware);
		}

		public override double DefaultWidth
		{
			//	Retourne la largeur standard.
			get
			{
				return 8 + 22*1.5 + 4 + 22*3;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if ( this.buttonSettings == null )  return;

			double dx = this.buttonSettings.DefaultWidth;
			double dy = this.buttonSettings.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(0, dy*0.5);
			this.buttonSettings.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5+4, dy+5);
			this.buttonInfos.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonPageStack.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5+4, 0);
			this.buttonKey.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonUpdate.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonAbout.Bounds = rect;
		}


		protected IconButton				buttonSettings;
		protected IconButton				buttonInfos;
		protected IconButton				buttonPageStack;
		protected IconButton				buttonKey;
		protected IconButton				buttonUpdate;
		protected IconButton				buttonAbout;
	}
}
