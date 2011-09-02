//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs.SettingsTabPages
{
	/// <summary>
	/// Onglet de SettingsDialog pour effectuer la maintenance de la base de données.
	/// </summary>
	public class MaintenanceTabPage : AbstractSettingsTabPage
	{
		public MaintenanceTabPage(ISettingsDialog container)
			: base (container)
		{
		}


		public override void AcceptChanges()
		{
		}

		public override void RejectChanges()
		{
		}
		
		public override void CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (10),
			};

			this.CreateButton
			(
				frame,
				"Sauvegarder",
				"Sauvegarde toute la base de données dans un fichier unique sur le serveur (backup1).",
				false,
				this.ActionBackup
			);

			this.CreateButton
			(
				frame,
				"Restaurer",
				"Restaure toute la base de données, à partir du fichier de sauvegarde unique sur le serveur (backup1).",
				true,
				this.ActionRestore
			);

			// ---------------

			this.CreateButton
			(
				frame,
				"Exporter tout",
				"Exporte l'ensemble de la base de données dans un fichier.",
				false,
				this.ActionExport
			);

			this.CreateButton
			(
				frame,
				"Importer tout",
				"Importe l'ensemble de la base de données à partir d'un fichier, en écrasant tout.",
				true,
				this.ActionImport
			);

			// ---------------

			this.CreateButton
			(
				frame,
				"Créer",
				"Crée une nouvelle base de données vide, en important les données modèles.",
				false,
				this.ActionCreate
			);

			this.CreateButton
			(
				frame,
				"Mettre à jour",
				"Met à jour toutes les données modèles, en les réimportant.",
				false,
				this.ActionUpdate
			);
		}

		private void CreateButton(FrameBox parent, string title, string description, bool bottomSpace, System.Action action)
		{
			double margin = 10;

			var button = new ConfirmationButton
			{
				Parent = parent,
				Text = ConfirmationButton.FormatContent (title, description),
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, bottomSpace ? margin : 0),
			};

			if (bottomSpace)
			{
				new Separator
				{
					Parent = parent,
					Dock = DockStyle.Top,
					PreferredHeight = 1,
					Margins = new Margins (0, 0, 0, margin),
				};
			}

			button.Clicked += delegate
			{
				action ();
			};
		}



		private void ActionBackup()
		{
			// TODO: Remplacer "backup1" par un fichier à choix ?
			try
			{
				CoreData.BackupDatabase (Paths.BackupPath, CoreData.GetDatabaseAccess ());
			}
			catch (System.Exception ex)
			{
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, FormattedText.Escape (ex.Message)).OpenDialog (this.Container.DefaultOwnerWindow);
			}
		}

		private void ActionRestore()
		{
			this.Container.Data.Dispose ();

			// TODO: Remplacer "backup1" par un fichier à choix ?
			try
			{
				CoreData.RestoreDatabase (Paths.BackupPath, CoreData.GetDatabaseAccess ());
				MessageDialog.CreateOk ("Restitution de la base de données", DialogIcon.None, "La restitution s'est terminée correctement.<br/>L'application devra être relancée.").OpenDialog (this.Container.DefaultOwnerWindow);
				System.Environment.Exit (0);
			}
			catch (System.Exception ex)
			{
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, FormattedText.Escape (ex.Message)).OpenDialog (this.Container.DefaultOwnerWindow);
			}
		}

		private void ActionExport()
		{
			string filename = this.ShowExportFileDialog ();

			if (string.IsNullOrEmpty (filename))
			{
				return;
			}

			var fileInfo = new System.IO.FileInfo (filename);

			this.Container.Data.ExportDatabase (fileInfo, false);
		}

		private void ActionImport()
		{
			string filename = this.ShowImportFileDialog ();

			if (string.IsNullOrEmpty (filename))
			{
				return;
			}

			this.ImportFromXmlOrZipFile (filename, fileInfo => this.Container.Data.ImportUserDatabase (fileInfo));
		}

		private void ActionCreate()
		{
			string filename = this.ShowImportFileDialog ();

			if (string.IsNullOrEmpty (filename))
			{
				return;
			}

			this.ImportFromXmlOrZipFile (filename, fileInfo => this.Container.Data.CreateUserDatabase (fileInfo));
		}

		private void ActionUpdate()
		{
			string filename = this.ShowImportFileDialog ();

			if (string.IsNullOrEmpty (filename))
			{
				return;
			}

			this.ImportFromXmlOrZipFile (filename, fileInfo => this.Container.Data.ImportSharedData (fileInfo));
		}

		
		private void ImportFromXmlOrZipFile(string filename, System.Action<System.IO.FileInfo> importAction)
		{
			if (System.IO.Path.GetExtension (filename).ToLowerInvariant () == ".zip")
			{
				ZipFile zip = new ZipFile ();

				if (zip.TryLoadFile (filename))
				{
					var entries = zip.EntryNames.ToList ();

					if ((entries.Count == 1) ||
						(System.IO.Path.GetExtension (entries[0]).ToLowerInvariant () == ".xml"))
					{
						string temp = System.IO.Path.GetTempFileName ();
						System.IO.File.WriteAllBytes (temp, zip.Entries.First ().Data);
						this.ImportFromFile (filename, importAction, () => System.IO.File.Delete (temp));
					}
				}

				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, "L'importation a échoué (fichier ZIP incompatible).").OpenDialog (this.Container.DefaultOwnerWindow);
			}
			else
			{
				this.ImportFromFile (filename, importAction);
			}
		}
		
		private void ImportFromFile(string path, System.Action<System.IO.FileInfo> actionImport, System.Action actionBeforeExit = null)
		{
			try
			{
				var fileInfo = new System.IO.FileInfo (path);

				actionImport (fileInfo);

				MessageDialog.CreateOk ("Importation de la base de données", DialogIcon.None, "L'importation s'est terminée correctement.<br/>L'application devra être relancée.").OpenDialog (this.Container.DefaultOwnerWindow);
			}
			catch (System.Exception ex)
			{
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, FormattedText.Escape (ex.Message)).OpenDialog (this.Container.DefaultOwnerWindow);
			}
			
			if (actionBeforeExit != null)
			{
				actionBeforeExit ();
			}
			
			System.Environment.Exit (0);
		}
		

		private string ShowExportFileDialog()
		{
			var dialog = new FileSaveDialog ();

			dialog.InitialDirectory = MaintenanceTabPage.currentDirectory;
			dialog.Title = "Exportation d'une base de données";

			dialog.Filters.Add ("xml", "XML", "*.xml");
			dialog.Filters.Add ("any", "Tous les fichiers", "*.*");

			dialog.OwnerWindow = this.Container.DefaultOwnerWindow;
			dialog.OpenDialog ();

			if (dialog.Result != DialogResult.Accept)
			{
				return null;
			}

			MaintenanceTabPage.currentDirectory = System.IO.Path.GetDirectoryName (dialog.FileName);
			
			return dialog.FileName;
		}

		private string ShowImportFileDialog()
		{
			var dialog = new FileOpenDialog ();

			dialog.InitialDirectory = MaintenanceTabPage.currentDirectory;
			dialog.Title = "Importation d'une base de données";

			dialog.Filters.Add ("xml", "XML", "*.xml");
			dialog.Filters.Add ("zip", "XML comprimé", "*.zip");
			dialog.Filters.Add ("any", "Tous les fichiers", "*.*");

			dialog.AcceptMultipleSelection = false;
			dialog.OwnerWindow = this.Container.DefaultOwnerWindow;
			dialog.OpenDialog ();

			if (dialog.Result != DialogResult.Accept)
			{
				return null;
			}

			MaintenanceTabPage.currentDirectory = System.IO.Path.GetDirectoryName (dialog.FileName);
			
			return dialog.FileName;
		}

		#region Paths Class

		private static class Paths
		{
			public static string DatabaseFolderPath
			{
				get
				{
					return System.IO.Path.Combine (
						System.Environment.GetFolderPath (System.Environment.SpecialFolder.CommonApplicationData),
						"Epsitec", "Firebird Databases");
				}
			}

			public static string BackupPath
			{
				get
				{
					return System.IO.Path.Combine (Paths.DatabaseFolderPath, Paths.BackupFileName);
				}
			}

			private const string BackupFileName = "core-backup1.firebird-backup";
		}

		#endregion

		private static string			currentDirectory;
	}
}
