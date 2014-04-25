//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class MCH2SummaryReportPopup : StackedPopup
	{
		public MCH2SummaryReportPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Paramètres du tableau des immobilisations MCH2";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Date,
				Label                 = "Etat initial au",
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Date,
				Label                 = "Etat final au",
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.GroupGuid,
				Label                 = "Groupement",
				Width                 = 200 + (int) AbstractScroller.DefaultBreadth,
				Height                = 150,
			});

			this.SetDescriptions (list);
		}


		public System.DateTime?					InitialDate
		{
			get
			{
				var controller = this.GetController (0) as DateStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (0) as DateStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		public System.DateTime?					FinalDate
		{
			get
			{
				var controller = this.GetController (1) as DateStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (1) as DateStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		public Guid								GroupGuid
		{
			get
			{
				var controller = this.GetController (2) as GroupGuidStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (2) as GroupGuidStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}


		protected override void UpdateWidgets()
		{
			this.okButton.Text = "Voir";

			this.okButton.Enable = this.InitialDate.HasValue
								&& this.FinalDate.HasValue
								&& !this.GroupGuid.IsEmpty;
		}

	}
}