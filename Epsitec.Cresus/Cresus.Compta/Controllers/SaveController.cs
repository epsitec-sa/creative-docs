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
	/// Ce contrôleur gère l'enregistrement de la comptabilité.
	/// </summary>
	public class SaveController : AbstractController
	{
		public SaveController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
		}


		protected override ViewSettingsList DirectViewSettingsList
		{
			get
			{
				return this.mainWindowController.GetViewSettingsList ("Présentation.Save.ViewSettings");
			}
		}


		protected override ControllerType ControllerType
		{
			get
			{
				return Controllers.ControllerType.Save;
			}
		}

		protected override void UpdateTitle()
		{
			this.SetTitle ();
		}

		public override bool AcceptPériodeChanged
		{
			get
			{
				return false;
			}
		}


		public override bool HasArray
		{
			get
			{
				return false;
			}
		}

		public override bool HasSearchPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasFilterPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasOptionsPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasInfoPanel
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
//				BackColor     = Color.FromHexa ("e1e1f9"),  // violet clair
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
					Text    = "Enregistrer",
					Dock    = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 4),
				};

				var box = new FrameBox
				{
					Parent        = column1,
					DrawFullFrame = true,
					Dock          = DockStyle.Fill,
				};

				this.saveButton = UIBuilder.CreateButton (box, Res.Commands.File.Save, "Enregistrer");
				this.saveAsButton = UIBuilder.CreateButton (box, Res.Commands.File.SaveAs, "Enregistrer sous");
				this.closeButton = UIBuilder.CreateButton (box, Res.Commands.File.Close, "Fermer la comptabilité en cours");
			}

			{
				new StaticText
				{
					Parent  = column2,
					Text    = "Exporter",
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


		private Button				saveButton;
		private Button				saveAsButton;
		private Button				closeButton;
	}
}
