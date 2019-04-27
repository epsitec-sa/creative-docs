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

namespace Epsitec.Cresus.Core.Dialogs
{
	public class TableDesignerFileExportDialog : AbstractFileDialog
	{
		public TableDesignerFileExportDialog(Widget parent, string title)
		{
			this.parent                  = parent;
			this.title                   = title;
			this.owner                   = this.parent.Window;
			this.InitialDirectory        = TableDesignerFileExportDialog.initialDirectory;
			this.InitialFileName         = TableDesignerFileExportDialog.initialFilename;
			this.enableNavigation        = true;
			this.enableMultipleSelection = false;
			this.hasOptions              = true;
			this.fileDialogType          = FileDialogType.Save;

			this.Filters.Add (new FilterItem ("x", "Fichier texte", ".txt"));
		}


		public bool UseColumns
		{
			get
			{
				return TableDesignerFileExportDialog.useColumns;
			}
			set
			{
				if (TableDesignerFileExportDialog.useColumns != value)
				{
					TableDesignerFileExportDialog.useColumns = value;
					this.UpdateOptionsButtons ();
				}
			}
		}

		public bool UseRows
		{
			get
			{
				return TableDesignerFileExportDialog.useRows;
			}
			set
			{
				if (TableDesignerFileExportDialog.useRows != value)
				{
					TableDesignerFileExportDialog.useRows = value;
					this.UpdateOptionsButtons ();
				}
			}
		}


		public void PathMemorize()
		{
			TableDesignerFileExportDialog.initialDirectory = System.IO.Path.GetDirectoryName (this.FileName);
			TableDesignerFileExportDialog.initialFilename  = System.IO.Path.GetFileName (this.FileName);
		}


		protected override string ActionButtonName
		{
			get
			{
				return "Exporter";
			}
		}

		protected override Rectangle GetOwnerBounds()
		{
			//	Donne les frontières de l'application.
			var w = this.parent.Window;

			return new Rectangle (w.WindowLocation, w.WindowSize);
		}

		protected override void FavoritesAddApplicationFolders()
		{
		}

		protected override IFavoritesSettings FavoritesSettings
		{
			get
			{
				return TableDesignerFileExportDialog.settings;
			}
		}


		protected override void CreateOptionsUserInterface()
		{
			//	Crée le panneau facultatif pour les options d'enregistrement.
			this.optionsContainer = new Widget (this.window.Root);
			this.optionsContainer.Margins = new Margins (0, 0, 8, 0);
			this.optionsContainer.Dock = DockStyle.Bottom;
			this.optionsContainer.TabNavigationMode = TabNavigationMode.None;
			this.optionsContainer.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			this.optionsContainer.Name = "OptionsContainer";

			//	Options.
			GroupBox group = new GroupBox (this.optionsContainer);
			group.Text = "Options";
			group.PreferredWidth = 300;
			group.Padding = new Margins (10, 0, 0, 3);
			group.Dock = DockStyle.StackEnd;
			group.Name = "Options";

			this.optionCheckButtonColumns = new CheckButton (group);
			this.optionCheckButtonColumns.Text = "La première ligne contient les noms des points";
			this.optionCheckButtonColumns.Dock = DockStyle.Top;
			this.optionCheckButtonColumns.AutoToggle = false;
			this.optionCheckButtonColumns.Clicked += this.HandleOptionsClicked;

			this.optionCheckButtonRows = new CheckButton (group);
			this.optionCheckButtonRows.Text = "La première colonne contient les noms des points";
			this.optionCheckButtonRows.Dock = DockStyle.Top;
			this.optionCheckButtonRows.AutoToggle = false;
			this.optionCheckButtonRows.Clicked += this.HandleOptionsClicked;

			this.UpdateOptionsButtons ();
		}

		protected void UpdateOptionsButtons()
		{
			if (this.optionCheckButtonColumns != null)
			{
				this.optionCheckButtonColumns.ActiveState = (TableDesignerFileExportDialog.useColumns) ? ActiveState.Yes : ActiveState.No;
				this.optionCheckButtonRows.ActiveState    = (TableDesignerFileExportDialog.useRows   ) ? ActiveState.Yes : ActiveState.No;
			}
		}

		private void HandleOptionsClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton radio pour le mode d'inclusion des polices a été cliqué.
			var button = sender as CheckButton;

			button.ActiveState = (button.ActiveState == ActiveState.Yes) ? ActiveState.No : ActiveState.Yes;

			if (sender == this.optionCheckButtonColumns)
			{
				this.UseColumns = (button.ActiveState == ActiveState.Yes);
			}

			if (sender == this.optionCheckButtonRows)
			{
				this.UseRows = (button.ActiveState == ActiveState.Yes);
			}
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
		private static bool				useColumns = true;
		private static bool				useRows = true;

		private readonly Widget			parent;
		private readonly new string		title;

		private Widget					optionsContainer;
		private CheckButton				optionCheckButtonColumns;
		private CheckButton				optionCheckButtonRows;
	}
}
