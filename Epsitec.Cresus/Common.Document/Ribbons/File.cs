using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe File correspond au menu fichiers.
	/// </summary>
	public class File : Abstract
	{
		public File() : base()
		{
			this.Title = Res.Strings.Action.FileMain;
			this.PreferredWidth = 8 + 22*1.5*4 + 4 + 22*2 + this.separatorWidth + 22;

			this.buttonNew        = this.CreateIconButton("New", "Large");
			this.buttonLastModels = this.CreateMenuButton("", Res.Strings.Action.LastFiles, new MessageEventHandler(this.HandleLastModelsPressed));
			this.buttonOpen       = this.CreateIconButton("Open", "Large");
			this.buttonLastFiles  = this.CreateMenuButton ("", Res.Strings.Action.LastFiles, new MessageEventHandler (this.HandleLastFilesPressed));
			this.buttonSave       = this.CreateIconButton("Save", "Large");
			this.buttonSaveAs     = this.CreateIconButton("SaveAs");
			this.buttonPrint      = this.CreateIconButton("Print", "Large");
			this.buttonExport     = this.CreateIconButton("Export");
			this.buttonCloseAll   = this.CreateIconButton("CloseAll");
			this.separator        = new IconSeparator(this);
			this.buttonOpenModel  = this.CreateIconButton("OpenModel");
			this.buttonSaveModel  = this.CreateIconButton("SaveModel");
			
//			this.UpdateClientGeometry();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonNew == null )  return;

			double dx = this.buttonNew.PreferredWidth;
			double dy = this.buttonNew.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Left += 22*1.5*4 + 4 + 22*2;
			rect.Width = this.separatorWidth;
			this.separator.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(0, dy*0.5);
			this.buttonNew.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonOpen.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonSave.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonPrint.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*0.5;
			this.buttonLastModels.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonLastFiles.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*4+4, dy+5);
			this.buttonExport.SetManualBounds(rect);
			rect.Offset(dx, 0);
			//this.buttonXyz.SetManualBounds(rect);
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonOpenModel.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*4+4, 0);
			this.buttonSaveAs.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonCloseAll.SetManualBounds(rect);
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonSaveModel.SetManualBounds(rect);
		}


		private void HandleLastModelsPressed(object sender, MessageEventArgs e)
		{
			//	Bouton pour ouvrir le menu des derniers modèles cliqué.
			GlyphButton button = sender as GlyphButton;
			if ( button == null )  return;
			VMenu menu = this.BuildLastModelsMenu();
			if ( menu == null )  return;
			menu.Host = this;
			menu.MinWidth = button.ActualWidth;
			TextFieldCombo.AdjustComboSize(button, menu);
			menu.ShowAsComboList(button, Point.Zero, button);
		}

		private void HandleLastFilesPressed(object sender, MessageEventArgs e)
		{
			//	Bouton pour ouvrir le menu des derniers fichiers cliqué.
			GlyphButton button = sender as GlyphButton;
			if ( button == null )  return;
			VMenu menu = this.BuildLastFilenamesMenu();
			if ( menu == null )  return;
			menu.Host = this;
			menu.MinWidth = button.ActualWidth;
			TextFieldCombo.AdjustComboSize(button, menu);
			menu.ShowAsComboList(button, Point.Zero, button);
		}

		protected VMenu BuildLastModelsMenu()
		{
			//	Construit le sous-menu des derniers modèles ouverts.
			int total = this.globalSettings.LastModelCount;
			if ( total == 0 )  return null;

			VMenu menu = new VMenu();

			for ( int i=0 ; i<total ; i++ )
			{
				string cmd = "LastModel(this.Name)";
				string filename = string.Format("{0} {1}", (i+1)%10, this.globalSettings.LastModelGetShort(i));
				string name = this.globalSettings.LastModelGet(i);
				MenuItem item = new MenuItem(cmd, "", filename, "", name);
				menu.Items.Add(item);
			}

			menu.AdjustSize();
			return menu;
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
		protected GlyphButton				buttonLastModels;
		protected GlyphButton				buttonLastFiles;
	}
}
