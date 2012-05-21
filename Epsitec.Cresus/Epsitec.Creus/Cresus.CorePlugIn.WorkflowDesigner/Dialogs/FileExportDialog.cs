//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.CorePlugIn.WorkflowDesigner.Dialogs
{
	public class FileExportDialog : AbstractFileDialog
	{
		public FileExportDialog(Widget parent)
		{
			this.parent = parent;

			this.InitialDirectory        = FileExportDialog.initialDirectory;
			this.InitialFileName         = FileExportDialog.initialFilename;
			this.FileExtension           = ".xml";
			this.enableNavigation        = true;
			this.enableMultipleSelection = false;
		}


		public bool ExportAll
		{
			get
			{
				return FileExportDialog.exportAll;
			}
			set
			{
				if (FileExportDialog.exportAll != value)
				{
					FileExportDialog.exportAll = value;
					this.UpdateOptionsButtons ();
				}
			}
		}


		public void PathMemorize()
		{
			FileExportDialog.initialDirectory = System.IO.Path.GetDirectoryName (this.FileName);
			FileExportDialog.initialFilename  = System.IO.Path.GetFileName (this.FileName);
		}


		protected override void CreateWindow()
		{
			this.CreateUserInterface ("FileExport", new Size (720, 480), "Exportation d'un workflow", 20, this.parent.Window);
		}

		protected override FileDialogType FileDialogType
		{
			get
			{
				return Epsitec.Common.Dialogs.FileDialogType.Save;
			}
		}

		protected override Rectangle GetOwnerBounds()
		{
			//	Donne les frontières de l'application.
			var w = this.parent.Window;

			return new Rectangle (w.WindowLocation, w.WindowSize);
		}

		public override void PersistWindowBounds()
		{
			//	Sauve la fenêtre.
		}

		protected override void CreateFileExtensionDescriptions(Epsitec.Common.Dialogs.IFileExtensionDescription settings)
		{
			settings.Add (".xml", "Fichier XML");
		}

		protected override void FavoritesAddApplicationFolders()
		{
		}

		protected override IFavoritesSettings FavoritesSettings
		{
			get
			{
				return FileExportDialog.settings;
			}
		}

		protected override void CreateOptionsUserInterface()
		{
			//	Crée le panneau facultatif pour les options d'enregistrement.
			this.optionsContainer = new Widget (this.window.Root);
			this.optionsContainer.Margins = new Margins (0, 0, 8, 0);
			this.optionsContainer.Dock = DockStyle.Bottom;
			this.optionsContainer.TabNavigationMode = TabNavigationMode.None;
			this.optionsContainer.Visibility = FileExportDialog.showOptions;
			this.optionsContainer.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			this.optionsContainer.Name = "OptionsContainer";

			//	Options pour le zoom.
			GroupBox group = new GroupBox (this.optionsContainer);
			group.Text = "Options";
			group.PreferredWidth = 200;
			group.Padding = new Margins (10, 0, 0, 3);
			group.Dock = DockStyle.StackEnd;
			group.Margins = new Margins (0, 182, 0, 0);
			group.Name = "Options";

			this.optionsExportAllButton = new CheckButton (group);
			this.optionsExportAllButton.Text = "Exporte tous les workflows";
			this.optionsExportAllButton.AutoToggle = false;
			this.optionsExportAllButton.Dock = DockStyle.Top;
			this.optionsExportAllButton.Clicked += this.HandleOptionsClicked;
		}

		protected override void CreateFooterOptions(Widget footer)
		{
			this.optionsExtend = new GlyphButton (footer);
			this.optionsExtend.PreferredWidth = 16;
			this.optionsExtend.ButtonStyle = ButtonStyle.Slider;
			this.optionsExtend.AutoFocus = false;
			this.optionsExtend.TabNavigationMode = TabNavigationMode.None;
			this.optionsExtend.Dock = DockStyle.Left;
			this.optionsExtend.Margins = new Margins (0, 8, 3, 3);
			this.optionsExtend.Clicked += this.HandleOptionsExtendClicked;
			ToolTip.Default.SetToolTip (this.optionsExtend, "Montre ou cache les options");
		}

		protected override void UpdateOptions()
		{
			base.UpdateOptions ();

			if (this.optionsExtend != null)
			{
				this.optionsExtend.GlyphShape = this.optionsContainer.Visibility ? GlyphShape.ArrowDown : GlyphShape.ArrowUp;
			}

			this.UpdateOptionsButtons ();
		}

		protected void UpdateOptionsButtons()
		{
			if (this.optionsExportAllButton != null)
			{
				this.optionsExportAllButton.ActiveState = this.ExportAll ? ActiveState.Yes : ActiveState.No;
			}
		}

		private void HandleOptionsExtendClicked(object sender, MessageEventArgs e)
		{
			this.optionsContainer.Visibility = !this.optionsContainer.Visibility;
			FileExportDialog.showOptions = this.optionsContainer.Visibility;

			this.UpdateOptions ();
		}

		private void HandleOptionsClicked(object sender, MessageEventArgs e)
		{
			this.ExportAll = !this.ExportAll;
			this.UpdateOptionsButtons ();
		}


		public class Settings : IFavoritesSettings
		{
			public Settings()
			{
				this.favoritesBig = true;
				this.favoritesList = new List<string> ();
			}


			#region IFavoritesSettings Members

			public bool UseLargeIcons
			{
				get
				{
					return this.favoritesBig;
				}
				set
				{
					this.favoritesBig = value;
				}
			}

			public IList<string> Items
			{
				get
				{
					return this.favoritesList;
				}
			}

			#endregion
			
			
			protected bool							favoritesBig;
			protected List<string>					favoritesList;
		}


		//	Tous les réglages sont conservés dans des variables statiques,
		//	ce qui est une solution rapide et provisoire !
		private static Settings			settings = new Settings ();
		private static string			initialDirectory = null;
		private static string			initialFilename = null;
		private static bool				exportAll = false;
		private static bool				showOptions = true;

		private readonly Widget			parent;

		private GlyphButton				optionsExtend;
		private Widget					optionsContainer;
		private CheckButton				optionsExportAllButton;
	}
}
