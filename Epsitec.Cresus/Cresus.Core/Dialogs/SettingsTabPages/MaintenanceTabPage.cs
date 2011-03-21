//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

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
				CoreData.BackupDatabase ("backup1", CoreData.GetDatabaseAccess ());
			}
			catch (System.Exception ex)
			{
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, ex.Message).OpenDialog ();
			}
		}

		private void ActionRestore()
		{
			this.Container.Data.Dispose ();

			// TODO: Remplacer "backup1" par un fichier à choix ?
			try
			{
				CoreData.RestoreDatabase ("backup1", CoreData.GetDatabaseAccess ());
			}
			catch (System.Exception ex)
			{
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, ex.Message).OpenDialog ();
			}
		}

		private void ActionExport()
		{
			string filename = this.ExportFileDialog ();

			if (string.IsNullOrEmpty (filename))
			{
				return;
			}

			var fileInfo = new System.IO.FileInfo (filename);

			this.Container.Data.ExportDatabase (fileInfo, false);
		}

		private void ActionImport()
		{
			string filename = this.ImportFileDialog ();

			if (string.IsNullOrEmpty (filename))
			{
				return;
			}

			var fileInfo = new System.IO.FileInfo (filename);

			this.Container.Data.ImportDatabase (fileInfo);
		}

		private void ActionCreate()
		{
			string filename = this.ImportFileDialog ();

			if (string.IsNullOrEmpty (filename))
			{
				return;
			}

			var fileInfo = new System.IO.FileInfo (filename);

			this.Container.Data.CreateUserDatabase (fileInfo);
		}

		private void ActionUpdate()
		{
			string filename = this.ImportFileDialog ();

			if (string.IsNullOrEmpty (filename))
			{
				return;
			}

			var fileInfo = new System.IO.FileInfo (filename);

			this.Container.Data.ImportSharedData (fileInfo);
		}


		private string ExportFileDialog()
		{
			var dialog = new FileSaveDialog ();

			dialog.InitialDirectory = MaintenanceTabPage.currentDirectory;
			dialog.Title = "Exportation d'une base de données";

			dialog.Filters.Add ("xml", "Xml", "*.xml");
			dialog.Filters.Add ("any", "Tous les fichiers", "*.*");

			dialog.OwnerWindow = CoreProgram.Application.Window;
			dialog.OpenDialog ();

			if (dialog.Result != DialogResult.Accept)
			{
				return null;
			}

			MaintenanceTabPage.currentDirectory = System.IO.Path.GetDirectoryName (dialog.FileName);
			return dialog.FileName;
		}

		private string ImportFileDialog()
		{
			var dialog = new FileOpenDialog ();

			dialog.InitialDirectory = MaintenanceTabPage.currentDirectory;
			dialog.Title = "Importation d'une base de données";

			dialog.Filters.Add ("xml", "Xml", "*.xml");
			dialog.Filters.Add ("any", "Tous les fichiers", "*.*");

			dialog.AcceptMultipleSelection = false;
			dialog.OwnerWindow = CoreProgram.Application.Window;
			dialog.OpenDialog ();

			if (dialog.Result != DialogResult.Accept)
			{
				return null;
			}

			MaintenanceTabPage.currentDirectory = System.IO.Path.GetDirectoryName (dialog.FileName);
			return dialog.FileName;
		}


		private static string			currentDirectory;
	}
}
