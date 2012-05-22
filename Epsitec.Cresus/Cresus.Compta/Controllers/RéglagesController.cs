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
	/// Ce contrôleur gère les réglages de la comptabilité.
	/// </summary>
	public class RéglagesController : AbstractController
	{
		public RéglagesController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.groups = new List<SettingsGroup> ();
			this.controllers = new List<AbstractSettingController> ();

			this.OpenSettings ();
		}


		protected override ViewSettingsList DirectViewSettingsList
		{
			get
			{
				return this.mainWindowController.GetViewSettingsList ("Présentation.Réglages.ViewSettings");
			}
		}


		protected override void UpdateTitle()
		{
			this.SetGroupTitle (Présentations.GetGroupName (ControllerType.Réglages));
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


		public override void Dispose()
		{
			this.CloseSettings ();
			base.Dispose ();
		}


		private void OpenSettings()
		{
			this.settingsList.SetText (SettingsType.GlobalTitre,       this.compta.Nom);
			this.settingsList.SetText (SettingsType.GlobalDescription, this.compta.Description);

			Converters.ExportSettings (this.settingsList);
		}

		private void CloseSettings()
		{
			this.compta.Nom         = this.settingsList.GetText (SettingsType.GlobalTitre);
			this.compta.Description = this.settingsList.GetText (SettingsType.GlobalDescription);

			Converters.ImportSettings (this.settingsList);
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


			var topFrame = new FrameBox
			{
				Parent         = frame,
				Dock           = DockStyle.Fill,
			};

			this.infoFrame = new FrameBox
			{
				Parent          = frame,
				DrawFullFrame   = true,
				PreferredHeight = 20,
				Dock            = DockStyle.Bottom,
				Margins         = new Margins (0, 0, -1, 0),
			};

			this.infoField = new StaticText
			{
				Parent         = this.infoFrame,
				Dock           = DockStyle.Fill,
				Margins        = new Margins (10, 10, 0, 0),
			};


			var leftFrame = new FrameBox
			{
				Parent         = topFrame,
				PreferredWidth = 200,
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, -1, 0, 0),
			};

			var rightFrame = new FrameBox
			{
				Parent         = topFrame,
				Dock           = DockStyle.Fill,
			};

			//	Partie gauche.
			new StaticText
			{
				Parent         = leftFrame,
				Text           = "Catégories des réglages avancés",
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
				Text           = "Détails des réglages avancés",
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
				this.UpdateTitle ();
				this.UpdateMain ();
			};
		}


		public override int SelectedArrayLine
		{
			get
			{
				return this.scrollList.SelectedItemIndex;
			}
			set
			{
				this.scrollList.SelectedItemIndex = value;
			}
		}

	
		private void UpdateList()
		{
			//	Met à jour la liste de gauche.
			this.groups.Clear ();
			this.scrollList.Items.Clear ();

			foreach (var group in this.settingsList.List.Select (x => x.Group))
			{
				if (!this.groups.Contains (group))
				{
					this.groups.Add (group);
					this.scrollList.Items.Add (" " + VerboseSettings.GetDescription (group));
				}
			}

			this.UpdateMain ();
		}

		private void UpdateMain()
		{
			//	Construit l'interface dans la partie principale de droite, en fonction du groupe sélectionné.
			var group = this.SelectedGroup;

			this.mainFrame.Children.Clear ();
			this.controllers.Clear ();

			foreach (var settings in this.settingsList.List.Where (x => x.Group == group))
			{
				this.CreateController (settings);
			}

			//	Pied de page.
			var footer = new FrameBox
			{
				Parent = this.mainFrame,
				Dock   = DockStyle.Bottom,
			};

			this.defaultButton = new Button
			{
				Parent         = footer,
				Text           = "Remet les valeurs standards",
				PreferredWidth = 200,
				Dock           = DockStyle.Left,
			};

			ToolTip.Default.SetToolTip (this.defaultButton, "Remet les valeurs standards de la catégorie");

			this.defaultButton.Clicked += delegate
			{
				this.settingsList.CopyFrom (this.MainWindowController.DefaultSettingsList, this.SelectedGroup);
				this.UpdateMain ();  // on reconstruit tout
			};

			this.UpdateControllers ();
		}

		private void CreateController(AbstractSettingData data)
		{
			AbstractSettingController controller = null;

			if (data is BoolSettingData)
			{
				controller = new BoolSettingController (data, this.ActionChanged);
			}

			if (data is IntSettingData)
			{
				controller = new IntSettingsController (data, this.ActionChanged);
			}

			if (data is DecimalSettingData)
			{
				controller = new DecimalSettingsController (data, this.ActionChanged);
			}

			if (data is TextSettingData)
			{
				controller = new TextSettingController (data, this.ActionChanged);
			}

			if (data is EnumSettingData)
			{
				controller = new EnumSettingController (data, this.ActionChanged);
			}

			if (data is SampleSettingData)
			{
				controller = new SampleSettingController (data, this.ActionChanged);
			}

			if (controller != null)
			{
				controller.CreateUI (this.mainFrame);
			}

			this.controllers.Add (controller);
		}


		private void ActionChanged()
		{
			this.UpdateControllers ();
		}

		private void UpdateControllers()
		{
			this.Validate ();
			this.CloseSettings ();  // doit être fait avant la mise à jour des échantillons

			//	Met à jour les échantillons.
			foreach (var controller in this.controllers)
			{
				controller.Update ();
			}

			//	Met à jour le bouton "Remet les valeurs standards".
			var group = this.SelectedGroup;
			this.defaultButton.Enable = !this.settingsList.Compare (this.MainWindowController.DefaultSettingsList, group);
			this.defaultButton.Visibility = (group != SettingsGroup.Unknown);
		}


		private void Validate()
		{
			//	Valide tous les contrôleurs de la page actuelle. Les autres réglages n'ont pas besoin
			//	d'être validés, car ils sont forcément ok.
			foreach (var controller in this.controllers)
			{
				controller.Validate ();
			}

			int count = this.settingsList.ErrorCount;
			if (count == 0)  // ok ?
			{
				this.infoFrame.BackColor = Color.Empty;
				this.infoField.Text = null;
			}
			else
			{
				this.infoFrame.BackColor = UIBuilder.ErrorColor;
				this.infoField.Text = string.Format ("Il y a {0} erreur{1}", count.ToString (), (count == 1)?"":"s");
			}
		}


		private SettingsGroup SelectedGroup
		{
			//	Retourne le groupe sélectionné dans la liste de gauche.
			get
			{
				int sel = this.scrollList.SelectedItemIndex;

				if (sel >= 0 && sel < this.groups.Count)
				{
					return this.groups[sel];
				}
				else
				{
					return SettingsGroup.Unknown;
				}
			}
		}


		private readonly List<SettingsGroup>				groups;
		private readonly List<AbstractSettingController>	controllers;

		private ScrollList									scrollList;
		private FrameBox									mainFrame;
		private Button										defaultButton;
		private FrameBox									infoFrame;
		private StaticText									infoField;
	}
}
