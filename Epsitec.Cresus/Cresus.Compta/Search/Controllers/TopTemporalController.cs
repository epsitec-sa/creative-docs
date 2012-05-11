//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Fields.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Search.Controllers
{
	/// <summary>
	/// Ce contrôleur gère la barre d'outil supérieure de filtre pour la comptabilité.
	/// </summary>
	public class TopTemporalController
	{
		public TopTemporalController(AbstractController controller)
		{
			this.controller = controller;

			this.compta          = this.controller.ComptaEntity;
			this.dataAccessor    = this.controller.DataAccessor;
			this.businessContext = this.controller.BusinessContext;
			this.data            = this.controller.DataAccessor.TemporalData;
		}


		public bool ShowPanel
		{
			get
			{
				return this.showPanel;
			}
			set
			{
				if (this.showPanel != value)
				{
					this.showPanel = value;
					this.toolbar.Visibility = this.showPanel;
				}
			}
		}

		public bool Specialist
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public void SearchClear()
		{
			this.data.Clear ();
			this.UpdateButtons ();
			this.temporalController.UpdateContent ();
		}


		public void CreateUI(FrameBox parent, System.Action searchStartAction)
		{
			this.searchStartAction = searchStartAction;

			this.toolbar = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = TopTemporalController.toolbarHeight,
				DrawFullFrame   = true,
				BackColor       = UIBuilder.TemporalBackColor,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 0, 5),
				Visibility      = false,
			};

			//	Crée les frames gauche, centrale et droite.
			var topPanelLeftFrame = new FrameBox
			{
				Parent         = this.toolbar,
				DrawFullFrame  = true,
				PreferredWidth = 20,
				Dock           = DockStyle.Left,
				Padding        = new Margins (5),
			};

			this.mainFrame = new FrameBox
			{
				Parent         = this.toolbar,
				Dock           = DockStyle.Fill,
				Padding        = new Margins (5, 5, 0, 0),
			};

			var topPanelRightFrame = new FrameBox
			{
				Parent         = this.toolbar,
				DrawFullFrame  = true,
				PreferredWidth = 20,
				Dock           = DockStyle.Right,
				Padding        = new Margins (5),
			};

			this.CreateFilterEnableButtonUI ();

			//	Remplissage de la frame gauche.
			this.topPanelLeftController = new TopPanelLeftController (this.controller);
			this.topPanelLeftController.CreateUI (topPanelLeftFrame, false, "Panel.Temporal", this.LevelChangedAction);
			this.topPanelLeftController.Specialist = false;

			//	Reemplissage de la frame centrale.
			new StaticText
			{
				Parent           = this.mainFrame,
				Text             = "Filtrer",
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				ContentAlignment = ContentAlignment.MiddleRight,
				PreferredWidth   = UIBuilder.LeftLabelWidth-10,
				PreferredHeight  = 20,
				Dock             = DockStyle.Left,
				Margins          = new Margins (0, 10, 0, 0),
			};

			new StaticText
			{
				Parent           = this.mainFrame,
				FormattedText    = "Période",
				ContentAlignment = ContentAlignment.MiddleRight,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				PreferredWidth   = 60,
				Dock             = DockStyle.Left,
				Margins          = new Margins (0, 10, 0, 0),
			};

			var temporalFrame = new FrameBox
			{
				Parent        = this.mainFrame,
				DrawFullFrame = true,
				Dock          = DockStyle.Fill,
				Padding       = new Margins (10, 5, 5, 5),
			};

			this.temporalController = new TemporalController (this.controller, this.data);
			this.temporalController.CreateUI (temporalFrame, this.SearchStartAction);

			this.resultLabel = new StaticText
			{
				Parent         = this.mainFrame,
				TextBreakMode  = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				PreferredWidth = 110-5,
				Dock           = DockStyle.Right,
				Margins        = new Margins (49+10, 0, 0, 0),
			};

			//	Remplissage de la frame droite.
			this.topPanelRightController = new TopPanelRightController (this.controller);
			this.topPanelRightController.CreateUI (topPanelRightFrame, "Termine le filtre", this.ClearAction, this.controller.MainWindowController.ClosePanelTemporal, this.LevelChangedAction);

			this.UpdateButtons ();
			this.temporalController.UpdateContent ();
		}

		private void SearchStartAction()
		{
			this.UpdateButtons ();
			this.searchStartAction ();
		}


		private void CreateFilterEnableButtonUI()
		{
			this.filterEnableButton = new CheckButton
			{
				Parent           = this.mainFrame,
				PreferredWidth   = 20,
				PreferredHeight  = 20,
				AutoToggle       = false,
				Anchor           = AnchorStyles.TopLeft,
				Margins          = new Margins (3, 0, 6, 0),
			};

			ToolTip.Default.SetToolTip (this.filterEnableButton, "Active ou désactive le filtre temporel");

			this.filterEnableButton.Clicked += delegate
			{
				this.data.Enable = !this.data.Enable;
				this.UpdateButtons ();
				this.searchStartAction ();
			};
		}



		public void UpdateContent()
		{
			this.UpdateButtons ();
			this.temporalController.UpdateContent ();
		}

		public void UpdateColumns()
		{
			//	Met à jour les widgets en fonction de la liste des colonnes présentes.
		}

		public void SetFilterCount(int dataCount, int count, int allCount)
		{
			if (count == allCount)
			{
				this.resultLabel.Text = string.Format ("{0} (tous)", allCount.ToString ());
			}
			else
			{
				this.resultLabel.Text = string.Format ("{0} sur {1}", count.ToString (), allCount.ToString ());
			}
		}


		private void LevelChangedAction()
		{
			this.UpdateButtons ();
			this.temporalController.UpdateContent ();
		}

		private void ClearAction()
		{
			this.data.Clear ();
			this.UpdateButtons ();
			this.temporalController.UpdateContent ();
			this.searchStartAction ();
		}


		private void UpdateButtons()
		{
			this.topPanelRightController.ClearEnable = !this.data.IsEmpty;
			this.filterEnableButton.ActiveState = this.data.Enable ? ActiveState.Yes : ActiveState.No;
		}


		private static readonly double					toolbarHeight = 20;

		private readonly AbstractController				controller;
		private readonly ComptaEntity					compta;
		private readonly BusinessContext				businessContext;
		private readonly AbstractDataAccessor			dataAccessor;
		private readonly TemporalData					data;

		private System.Action							searchStartAction;
		private FrameBox								mainFrame;
		private CheckButton								filterEnableButton;
		private TopPanelLeftController					topPanelLeftController;
		private TopPanelRightController					topPanelRightController;
		private TemporalController						temporalController;
		private FrameBox								toolbar;
		private StaticText								resultLabel;
		private bool									showPanel;
	}
}
