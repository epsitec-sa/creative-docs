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

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère l'enregistrement de la comptabilité.
	/// </summary>
	public class SaveController : AbstractController
	{
		public SaveController(Application app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
		}


		protected override void UpdateTitle()
		{
			this.SetTitle ("Enregistrement de la comptabilité");
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

		public override bool HasShowSearchPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasShowFilterPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasShowOptionsPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasShowInfoPanel
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
					Padding       = new Margins (10),
				};

				this.saveButton = UIBuilder.CreateButton (box, Res.Commands.File.Save, "Enregistrer");
				this.saveAsButton = UIBuilder.CreateButton (box, Res.Commands.File.SaveAs, "Enregistrer sous");
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
	}
}
