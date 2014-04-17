//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class MCH2SummaryParamsPanel : AbstractParamsPanel
	{
		public MCH2SummaryParamsPanel(DataAccessor accessor)
			: base (accessor)
		{
			var now = Timestamp.Now;
			var it = new Timestamp (new System.DateTime (now.Date.Year, 1, 1), 0);
			var ft = new Timestamp (new System.DateTime (now.Date.Year, 12, 31), 0);

			this.reportParams = new MCH2SummaryParams (it, ft, Guid.Empty);
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateInitialTimestampUI (parent);
			this.CreateFinalTimestampUI (parent);
			this.CreateGroupUI (parent);

			this.UpdateUI ();
		}

		private void CreateInitialTimestampUI(Widget parent)
		{
			var text = "Initial";
			var width = text.GetTextWidth ();

			new StaticText
			{
				Parent           = parent,
				Text             = text,
				PreferredWidth   = width,
				ContentAlignment = ContentAlignment.MiddleRight,
				Dock             = DockStyle.Left,
				Margins          = new Margins (0, 5, 0, 0),
			};

			this.initialTimestampController = new StateAtController (this.accessor);

			var frame = this.initialTimestampController.CreateUI (parent);
			frame.Dock = DockStyle.Left;

			this.initialTimestampController.DateChanged += delegate
			{
				this.UpdateParams ();
			};
		}

		private void CreateFinalTimestampUI(Widget parent)
		{
			var text = "Final";
			var width = text.GetTextWidth ();

			new StaticText
			{
				Parent           = parent,
				Text             = text,
				PreferredWidth   = 20 + width,
				ContentAlignment = ContentAlignment.MiddleRight,
				Dock             = DockStyle.Left,
				Margins          = new Margins (0, 5, 0, 0),
			};

			this.finalTimestampController = new StateAtController (this.accessor);

			var frame = this.finalTimestampController.CreateUI (parent);
			frame.Dock = DockStyle.Left;

			this.finalTimestampController.DateChanged += delegate
			{
				this.UpdateParams ();
			};
		}

		private void CreateGroupUI(Widget parent)
		{
			this.groupButton = new Button
			{
				Parent         = parent,
				ButtonStyle    = ButtonStyle.Icon,
				AutoFocus      = false,
				PreferredWidth = 200,
				Dock           = DockStyle.Left,
				Margins        = new Margins (20, 0, 0, 0),
			};

			this.groupButton.Clicked += delegate
			{
				this.ShowFilter (this.groupButton);
			};
		}


		protected override void UpdateUI()
		{
			this.initialTimestampController.Date = this.Params.InitialTimestamp.Date;
			this.finalTimestampController  .Date = this.Params.FinalTimestamp.Date;

			if (this.groupGuid.IsEmpty)
			{
				this.groupButton.Text = "Sans groupement";
			}
			else
			{
				var text = GroupsLogic.GetShortName (this.accessor, this.groupGuid);
				this.groupButton.Text = "Grouper selon " + text;
			}
		}


		private void ShowFilter(Widget target)
		{
			var popup = new FilterPopup (this.accessor, this.Params.RootGuid);

			popup.Create (target, leftOrRight: true);

			popup.Navigate += delegate (object sender, Guid guid)
			{
				this.groupGuid = guid;
				this.UpdateParams ();
				this.UpdateUI ();
			};
		}


		private void UpdateParams()
		{
			if (this.finalTimestampController.Date.HasValue)
			{
				var initialTimestamp = new Timestamp (this.initialTimestampController.Date.Value, 0);
				var finalTimestamp   = new Timestamp (this.finalTimestampController  .Date.Value, 0);

				this.reportParams = new MCH2SummaryParams (initialTimestamp, finalTimestamp, this.groupGuid);

				this.OnParamsChanged ();
			}
		}


		private MCH2SummaryParams Params
		{
			get
			{
				return this.reportParams as MCH2SummaryParams;
			}
		}


		private StateAtController				initialTimestampController;
		private StateAtController				finalTimestampController;
		private Button							groupButton;
		private Guid							groupGuid;
	}
}
