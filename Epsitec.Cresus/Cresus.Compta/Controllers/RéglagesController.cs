//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Settings.Data;
using Epsitec.Cresus.Compta.Settings.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les réglages de la comptabilité.
	/// </summary>
	public class RéglagesController : AbstractController
	{
		public RéglagesController(Application app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.groups = new List<string> ();
		}


		protected override void UpdateTitle()
		{
			this.SetTitle ("Réglages");
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

			var leftFrame = new FrameBox
			{
				Parent         = frame,
				PreferredWidth = 200,
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, -1, 0, 0),
			};

			var rightFrame = new FrameBox
			{
				Parent         = frame,
				Dock           = DockStyle.Fill,
			};

			//	Partie gauche.
			new StaticText
			{
				Parent         = leftFrame,
				Text           = "Catégories des réglages :",
				Dock           = DockStyle.Top,
				Margins        = new Margins (5, 0, 0, 5),
			};

			this.scrollList = new ScrollList
			{
				Parent         = leftFrame,
				Dock           = DockStyle.Fill,
			};

			//	Partie droite.
			new StaticText
			{
				Parent         = rightFrame,
				Text           = "Détails des réglages :",
				Dock           = DockStyle.Top,
				Margins        = new Margins (10, 0, 0, 5),
			};

			this.mainFrame = new FrameBox
			{
				Parent        = rightFrame,
				DrawFullFrame = true,
				Dock          = DockStyle.Fill,
				Padding       = new Margins (10),
			};

			this.UpdateList ();

			this.scrollList.SelectedItemChanged += delegate
			{
				this.UpdateMain (this.groups[this.scrollList.SelectedItemIndex]);
			};
		}

		private void UpdateList()
		{
			this.groups.Clear ();
			this.scrollList.Items.Clear ();

			foreach (var settings in this.settingsList.List)
			{
				var group = settings.Group;

				if (!this.groups.Contains (group))
				{
					this.groups.Add (group);
					this.scrollList.Items.Add (" " + VerboseSettings.GetDescription (group));
				}
			}
		}

		private void UpdateMain(string group)
		{
			this.mainFrame.Children.Clear ();

			foreach (var settings in this.settingsList.List)
			{
				if (settings.Group == group)
				{
					this.CreateController (settings);
				}
			}
		}

		private void CreateController(AbstractSettingsData data)
		{
			AbstractSettingsController controller = null;

			if (data is BoolSettingsData)
			{
				controller = new BoolSettingsController (data);
			}

			if (data is IntSettingsData)
			{
				controller = new IntSettingsController (data);
			}

			if (data is TextSettingsData)
			{
				controller = new TextSettingsController (data);
			}

			if (data is EnumSettingsData)
			{
				controller = new EnumSettingsController (data);
			}

			if (controller != null)
			{
				controller.CreateUI (this.mainFrame);
			}
		}


		private readonly List<string>		groups;

		private ScrollList					scrollList;
		private FrameBox					mainFrame;
	}
}
