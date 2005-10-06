using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Action permet de gérer les commandes du menu aide.
	/// </summary>
	[SuppressBundleSupport]
	public class Action : Abstract
	{
		public Action() : base()
		{
			this.title.Text = Res.Strings.Action.ActionMain;

			this.buttonSettings = this.CreateIconButton("Settings", Misc.Icon("Settings"), Res.Strings.Action.Settings);
			this.buttonInfos = this.CreateIconButton("Infos", Misc.Icon("Infos"), Res.Strings.Action.Infos);
			this.buttonPageStack = this.CreateIconButton("PageStack", Misc.Icon("PageStack"), Res.Strings.Action.PageStack);
			this.buttonKey = this.CreateIconButton("KeyApplication", Misc.Icon("Key"), Res.Strings.Action.Key);
			this.buttonUpdate = this.CreateIconButton("UpdateApplication", Misc.Icon("Update"), Res.Strings.Action.Update);
			this.buttonAbout = this.CreateIconButton("AboutApplication", Misc.Icon("About"), Res.Strings.Action.About);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		public override void SetDocument(DocumentType type, InstallType install, Settings.GlobalSettings gs, Document document)
		{
			base.SetDocument(type, install, gs, document);

			this.buttonKey.SetVisible(this.installType != InstallType.Freeware);
		}

		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return 8 + 22*3;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonSettings == null )  return;

			double dx = this.buttonSettings.DefaultWidth;
			double dy = this.buttonSettings.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonSettings.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonInfos.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonPageStack.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
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
