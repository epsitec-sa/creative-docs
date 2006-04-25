using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe File correspond au menu fichiers.
	/// </summary>
	[SuppressBundleSupport]
	public class File : Abstract
	{
		public File() : base()
		{
			this.title.Text = Res.Strings.Action.FileMain;

			this.buttonNew       = this.CreateIconButton("New");
			this.buttonOpen      = this.CreateIconButton("Open", "Large");
			this.buttonLastFiles = this.CreateMenuButton ("", Res.Strings.Action.LastFiles, new MessageEventHandler (this.HandleLastFilesPressed));
			this.buttonSave      = this.CreateIconButton("Save", "Large");
			this.buttonSaveAs    = this.CreateIconButton("SaveAs");
			this.buttonPrint     = this.CreateIconButton("Print", "Large");
			this.buttonExport    = this.CreateIconButton("Export");
			this.buttonCloseAll  = this.CreateIconButton("CloseAll");
			this.separator       = new IconSeparator(this);
			this.buttonOpenModel = this.CreateIconButton("OpenModel");
			this.buttonSaveModel = this.CreateIconButton("SaveModel");
			
			this.UpdateClientGeometry();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		public override double DefaultWidth
		{
			//	Retourne la largeur standard.
			get
			{
				return 8 + 22*1.5*3 + 4 + 22*2 + this.separatorWidth + 22;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonNew == null )  return;

			double dx = this.buttonNew.DefaultWidth;
			double dy = this.buttonNew.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Left += 22*1.5*3 + 4 + 22*2;
			rect.Width = this.separatorWidth;
			this.separator.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(0, dy*0.5);
			this.buttonOpen.Bounds = rect;
			rect.Offset(dx*1.5, 0);
			this.buttonSave.Bounds = rect;
			rect.Offset(dx*1.5, 0);
			this.buttonPrint.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*0.5;
			this.buttonLastFiles.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*3+4, dy+5);
			this.buttonNew.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonExport.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonOpenModel.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*3+4, 0);
			this.buttonSaveAs.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonCloseAll.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonSaveModel.Bounds = rect;
		}


		private void HandleLastFilesPressed(object sender, MessageEventArgs e)
		{
			//	Bouton pour ouvrir le menu des derniers fichiers cliqué.
			GlyphButton button = sender as GlyphButton;
			if ( button == null )  return;
			VMenu menu = this.BuildLastFilenamesMenu();
			if ( menu == null )  return;
			menu.Host = this;
			menu.MinWidth = button.Width;
			TextFieldCombo.AdjustComboSize(button, menu);
			menu.ShowAsComboList(button, Point.Zero, button);
		}

		protected VMenu BuildLastFilenamesMenu()
		{
			//	Construit le sous-menu des derniers fichiers ouverts.
			int total = this.globalSettings.LastFilenameCount;
			if ( total == 0 )  return null;

			VMenu menu = new VMenu();

			for ( int i=0 ; i<total ; i++ )
			{
				string cmd = "LastFile(this.Name)";
				string filename = string.Format("{0} {1}", (i+1)%10, this.globalSettings.LastFilenameGetShort(i));
				string name = this.globalSettings.LastFilenameGet(i);
				MenuItem item = new MenuItem(cmd, "", filename, "", name);
				menu.Items.Add(item);
			}

			menu.AdjustSize();
			return menu;
		}


		protected IconButton				buttonNew;
		protected IconButton				buttonOpen;
		protected IconButton				buttonSave;
		protected IconButton				buttonSaveAs;
		protected IconButton				buttonPrint;
		protected IconButton				buttonExport;
		protected IconButton				buttonCloseAll;
		protected IconSeparator				separator;
		protected IconButton				buttonOpenModel;
		protected IconButton				buttonSaveModel;
		protected GlyphButton				buttonLastFiles;
	}
}
