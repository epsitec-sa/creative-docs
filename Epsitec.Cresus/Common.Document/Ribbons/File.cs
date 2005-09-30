using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe File permet de gérer les groupes.
	/// </summary>
	[SuppressBundleSupport]
	public class File : Abstract
	{
		public File() : base()
		{
			this.title.Text = "File";

			this.buttonNew = this.CreateIconButton("New", Misc.Icon("New"), Res.Strings.Action.New);
			this.buttonOpen = this.CreateIconButton("Open", Misc.Icon("Open2"), Res.Strings.Action.Open);
			this.buttonSave = this.CreateIconButton("Save", Misc.Icon("Save2"), Res.Strings.Action.Save);
			this.buttonSaveAs = this.CreateIconButton("SaveAs", Misc.Icon("SaveAs"), Res.Strings.Action.SaveAs);
			this.buttonPrint = this.CreateIconButton("Print", Misc.Icon("Print2"), Res.Strings.Action.Print);
			this.buttonExport = this.CreateIconButton("Export", Misc.Icon("Export"), Res.Strings.Action.Export);

			this.buttonLastFiles = new GlyphButton(this);
			this.buttonLastFiles.ButtonStyle = ButtonStyle.ToolItem;
			this.buttonLastFiles.GlyphShape = GlyphShape.Menu;
			this.buttonLastFiles.Clicked += new MessageEventHandler(this.HandleLastFilesClicked);
			ToolTip.Default.SetToolTip(this.buttonLastFiles, Res.Strings.Action.LastFiles);

			this.buttonOthers = new GlyphButton(this);
			this.buttonOthers.ButtonStyle = ButtonStyle.ToolItem;
			this.buttonOthers.GlyphShape = GlyphShape.Dots;
			this.buttonOthers.Clicked += new MessageEventHandler(this.HandleOthersClicked);
			ToolTip.Default.SetToolTip(this.buttonOthers, "Additional operations...");
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return 8 + 22*1.5*3 + 4 + 22*2;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonNew == null )  return;

			double dx = this.buttonNew.DefaultWidth;
			double dy = this.buttonNew.DefaultHeight;

			Rectangle rect = this.UsefulZone;
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
			rect.Offset(dx*1.5*3+4, dy);
			this.buttonNew.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonExport.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*3+4, 0);
			this.buttonSaveAs.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonOthers.Bounds = rect;
		}


		// Bouton pour ouvrir le menu des derniers fichiers cliqué.
		private void HandleLastFilesClicked(object sender, MessageEventArgs e)
		{
			GlyphButton button = sender as GlyphButton;
			if ( button == null )  return;
			Point pos = button.MapClientToScreen(new Point(0, 1));
			VMenu menu = this.BuildLastFilenamesMenu();
			if ( menu == null )  return;
			menu.Host = this;
			menu.ShowAsContextMenu(this.Window, pos);
		}

		// Construit le sous-menu des derniers fichiers ouverts.
		protected VMenu BuildLastFilenamesMenu()
		{
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


		// Bouton pour ouvrir le menu des autres opérations.
		private void HandleOthersClicked(object sender, MessageEventArgs e)
		{
			GlyphButton button = sender as GlyphButton;
			if ( button == null )  return;
			Point pos = button.MapClientToScreen(new Point(0, 1));
			VMenu menu = this.BuildOthersMenu();
			if ( menu == null )  return;
			menu.Host = this;
			menu.ShowAsContextMenu(this.Window, pos);
		}

		// Construit le sous-menu des autres opérations.
		protected VMenu BuildOthersMenu()
		{
			VMenu menu = new VMenu();

			this.MenuAdd(menu, "", "OpenModel", Res.Strings.Action.OpenModel, "");
			this.MenuAdd(menu, "", "SaveModel", Res.Strings.Action.SaveModel, "");
			this.MenuAdd(menu, "", "", "", "");
			this.MenuAdd(menu, "", "CloseAll", Res.Strings.Action.CloseAll, "");

			menu.AdjustSize();
			return menu;
		}


		protected IconButton				buttonNew;
		protected IconButton				buttonOpen;
		protected IconButton				buttonSave;
		protected IconButton				buttonSaveAs;
		protected IconButton				buttonPrint;
		protected IconButton				buttonExport;
		protected GlyphButton				buttonLastFiles;
		protected GlyphButton				buttonOthers;
	}
}
