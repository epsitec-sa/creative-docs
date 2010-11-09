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

namespace Epsitec.Cresus.WorkflowDesigner.Dialogs
{
	public class FileExportDialog : AbstractFileDialog
	{
		public FileExportDialog(Widget parent)
		{
			this.parent = parent;

			this.InitialDirectory = FileExportDialog.initialDirectory;
			this.InitialFileName  = FileExportDialog.initialFilename;
			this.FileExtension    = ".xml";
			this.enableNavigation = true;
			this.enableMultipleSelection = false;
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

		private readonly Widget			parent;
	}
}
