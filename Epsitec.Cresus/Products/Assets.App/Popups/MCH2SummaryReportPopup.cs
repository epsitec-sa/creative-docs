//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Data;
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
				DateRangeCategory     = DateRangeCategory.Mandat,
				Label                 = "Etat initial au",
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Mandat,
				Label                 = "Etat final au",
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = "Effectuer un groupement",
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.GroupGuid,
				Label                 = "",
				Width                 = 200 + (int) AbstractScroller.DefaultBreadth,
				Height                = 150,
			});

			list.Add (new StackedControllerDescription  // 4
			{
				StackedControllerType = StackedControllerType.Int,
				Label                 = "Jusqu'au niveau",
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = "Voir";
			this.defaultControllerRankFocus = 0;
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

		private bool							GroupEnable
		{
			get
			{
				var controller = this.GetController (2) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (2) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		public Guid								GroupGuid
		{
			get
			{
				if (this.GroupEnable)
				{
					var controller = this.GetController (3) as GroupGuidStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					return controller.Value;
				}
				else
				{
					return Guid.Empty;
				}
			}
			set
			{
				var controller = this.GetController (3) as GroupGuidStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
				this.GroupEnable = !value.IsEmpty;
			}
		}

		public int?								Level
		{
			get
			{
				var controller = this.GetController (4) as IntStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (4) as IntStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}


		public override void CreateUI()
		{
			base.CreateUI ();

			{
				var controller = this.GetController (3) as GroupGuidStackedController;
				controller.Level = 1;
			}
		}

		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			this.SetVisibility (3, this.GroupEnable);
			this.SetVisibility (4, this.GroupEnable);

			this.okButton.Enable = this.InitialDate.HasValue
								&& this.FinalDate.HasValue
								&& this.InitialDate < this.FinalDate
								&& (!this.GroupEnable || !this.GroupGuid.IsEmpty)
								&& !this.HasError;
		}
	}
}