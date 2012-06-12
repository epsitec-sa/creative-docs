//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Settings.Data;
using Epsitec.Cresus.Compta.Settings.Controllers;
using Epsitec.Cresus.Compta.ViewSettings.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère l'impression de la comptabilité.
	/// </summary>
	public class PrintController : AbstractController
	{
		public PrintController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
		}


		protected override ViewSettingsList DirectViewSettingsList
		{
			get
			{
				return this.mainWindowController.GetViewSettingsList (Présentations.GetViewSettingsKey (ControllerType.Print));
			}
		}


		public override ControllerType ControllerType
		{
			get
			{
				return Controllers.ControllerType.Print;
			}
		}

		public override bool AcceptPériodeChanged
		{
			get
			{
				return false;
			}
		}


		protected override void CreateSpecificUI(FrameBox parent)
		{
			var frame = new FrameBox
			{
				Parent        = parent,
				DrawFullFrame = true,
				BackColor     = Color.FromBrightness (0.95),
				Dock          = DockStyle.Fill,
				Padding       = new Margins (10),
			};

			var column1 = new FrameBox
			{
				Parent         = frame,
				PreferredWidth = 300,
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, 10, 0, 0),
			};

			var column2 = new FrameBox
			{
				Parent         = frame,
				PreferredWidth = 300,
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, 10, 0, 0),
			};

			{
				new StaticText
				{
					Parent  = column1,
					Text    = "Choix du document",
					Dock    = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 4),
				};

				var list = new ScrollList
				{
					Parent        = column1,
					Dock          = DockStyle.Fill,
					Padding       = new Margins (10),
				};

				//	Pour l'exemple !
				list.Items.Add (" Journal des écritures");
				list.Items.Add (" Plan comptable");
				list.Items.Add (" Balance de vérification");
				list.Items.Add (" Extrait de compte");
				list.Items.Add (" Bilan");
				list.Items.Add (" Pertes et Profits");
				list.Items.Add (" Compte d'exploitation");
				list.Items.Add (" Budgets");
				list.Items.Add (" Résumé TVA");
				list.Items.Add (" Décompte TVA");

				this.printButton = UIBuilder.CreateButton (column1, Res.Commands.File.Print, "Imprimer");
				this.printButton.Dock = DockStyle.Bottom;
				this.printButton.Margins = new Margins (0, 0, 10, 0);
			}

			{
				new StaticText
				{
					Parent  = column2,
					Text    = "Aperçu avant impression",
					Dock    = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 4),
				};

				var box = new FrameBox
				{
					Parent        = column2,
					DrawFullFrame = true,
					Dock          = DockStyle.Fill,
					Padding       = new Margins (10),
				};

			}
		}


		private Button				printButton;
	}
}
