//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class MCH2ParamsPanel : AbstractParamsPanel
	{
		public MCH2ParamsPanel(DataAccessor accessor)
			: base (accessor)
		{
			this.reportParams = new MCH2Params (Timestamp.Now, Guid.Empty);
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateTimestampUI (parent);
			this.CreateGroupUI (parent);

			this.UpdateUI ();
		}

		private void CreateTimestampUI(Widget parent)
		{
			this.timestampController = new StateAtController (this.accessor);

			var frame = this.timestampController.CreateUI (parent);
			frame.Dock = DockStyle.Left;

			this.timestampController.DateChanged += delegate
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
			};

			this.groupButton.Clicked += delegate
			{
				this.ShowFilter (this.groupButton);
			};
		}


		protected override void UpdateUI()
		{
			this.timestampController.Date = this.Params.Timestamp.Date;

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
			if (this.timestampController.Date.HasValue)
			{
				var timestamp = new Timestamp (this.timestampController.Date.Value, 0);
				this.reportParams = new MCH2Params (timestamp, this.groupGuid);

				this.OnParamsChanged ();
			}
		}


		private MCH2Params Params
		{
			get
			{
				return this.reportParams as MCH2Params;
			}
		}


		private StateAtController				timestampController;
		private Button							groupButton;
		private Guid							groupGuid;
	}
}
