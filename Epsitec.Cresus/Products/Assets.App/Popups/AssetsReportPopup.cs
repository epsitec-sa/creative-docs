//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Reports;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class AssetsReportPopup : AbstractStackedPopup
	{
		public AssetsReportPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.AssetsReport.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = Res.Strings.Popup.Report.CustomTitle.ToString (),
				Width                 = GroupGuidStackedController.ControllerWidth,
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Mandat,
				Label                 = Res.Strings.Popup.AssetsReport.State.ToString (),
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = Res.Strings.Popup.AssetsReport.Group.ToString (),
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.GroupGuid,
				Label                 = "",
				Width                 = GroupGuidStackedController.ControllerWidth,
				Height                = 150,
			});

			list.Add (new StackedControllerDescription  // 4
			{
				StackedControllerType = StackedControllerType.Int,
				Label                 = Res.Strings.Popup.AssetsReport.Level.ToString (),
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Show.ToString ();
			this.defaultControllerRankFocus = 0;
		}


		public AssetsParams						AssetsParams
		{
			get
			{
				var timestamp = new Timestamp (this.Date ?? Timestamp.Now.Date, 0);
				return new AssetsParams (this.CustomTitle, timestamp, this.GroupGuid, this.Level);
			}
			set
			{
				this.CustomTitle = value.CustomTitle;
				this.Date        = value.Timestamp.Date;
				this.GroupGuid   = value.RootGuid;
				this.Level       = value.Level;
			}
		}

		private string							CustomTitle
		{
			get
			{
				var controller = this.GetController (0) as TextStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (0) as TextStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private System.DateTime?				Date
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

		private Guid							GroupGuid
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

		private int?							Level
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


		protected override void CreateUI()
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

			this.okButton.Enable = this.Date.HasValue
								&& (!this.GroupEnable || !this.GroupGuid.IsEmpty)
								&& !this.HasError;
		}
	}
}