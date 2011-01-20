﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
using Epsitec.Cresus.Core.Printers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs.SettingsTabPages
{
	/// <summary>
	/// Onglet de SettingsDialog pour effecture la maintenance de la base de données.
	/// </summary>
	public class MaintenanceTabPage : AbstractSettingsTabPage
	{
		public MaintenanceTabPage(CoreApplication application)
			: base (application)
		{
		}


		public override void AcceptChangings()
		{
		}

		public override void CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (50),
			};

			this.CreateButton
			(
				frame,
				"Exporter les données",
				"Exporte l'ensemble de la base de données dans un fichier.",
				this.ActionExport
			);

			this.CreateButton
			(
				frame,
				"Importer les données",
				"Importe l'ensemble de la base de données à partir d'un fichier, en écrasant tout.",
				this.ActionImport
			);

			this.CreateButton
			(
				frame,
				"Créer une base de données",
				"Crée une base de données vide, en important les données modèles.",
				this.ActionCreate
			);

			this.CreateButton
			(
				frame,
				"Mettre à jour les données modèles",
				"Met à jour toutes les données modèles, en les réimportant.",
				this.ActionUpdate
			);
		}

		private void CreateButton(FrameBox parent, string buttonText, string description, System.Action action)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 10),
			};

			var button = new Button
			{
				Parent = frame,
				Text = buttonText,
				PreferredWidth = 200,
				Dock = DockStyle.Left,
			};

			var text = new StaticText
			{
				Parent = frame,
				Text = description,
				Dock = DockStyle.Fill,
				Margins = new Margins (20, 0, 0, 0),
			};

			button.Clicked += delegate
			{
				action ();
			};
		}



		private void ActionExport()
		{
			string filename = this.ExportFileDialog ();

			if (string.IsNullOrEmpty (filename))
			{
				return;
			}

			var fileInfo = new System.IO.FileInfo (filename);
			var dbAccess = CoreData.GetDatabaseAccess ();

			CoreData.ExportDatabase (fileInfo, dbAccess, false);
		}

		private void ActionImport()
		{
			string filename = this.ImportFileDialog ();

			if (string.IsNullOrEmpty (filename))
			{
				return;
			}

			var fileInfo = new System.IO.FileInfo (filename);
			var dbAccess = CoreData.GetDatabaseAccess ();

			CoreData.ImportDatabase (fileInfo, dbAccess);
		}

		private void ActionCreate()
		{
			string filename = this.ImportFileDialog ();

			if (string.IsNullOrEmpty (filename))
			{
				return;
			}

			var fileInfo = new System.IO.FileInfo (filename);
			var dbAccess = CoreData.GetDatabaseAccess ();

			CoreData.CreateUserDatabase (fileInfo, dbAccess);
		}

		private void ActionUpdate()
		{
			string filename = this.ImportFileDialog ();

			if (string.IsNullOrEmpty (filename))
			{
				return;
			}

			var fileInfo = new System.IO.FileInfo (filename);
			var dbAccess = CoreData.GetDatabaseAccess ();

			CoreData.ImportSharedData (fileInfo, dbAccess);
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
