//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class AssetsReportPopup : StackedPopup
	{
		public AssetsReportPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Paramètres de la liste des objets d'immobilisations";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Date,
				Label                 = "Etat au",
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = "Effectue un groupement",
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.GroupGuid,
				Label                 = "",
				Width                 = 200 + (int) AbstractScroller.DefaultBreadth,
				Height                = 150,
			});

			this.SetDescriptions (list);
		}


		public System.DateTime?					Date
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

		private bool							GroupEnable
		{
			get
			{
				var controller = this.GetController (1) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (1) as BoolStackedController;
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
					var controller = this.GetController (2) as GroupGuidStackedController;
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
				var controller = this.GetController (2) as GroupGuidStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
				this.GroupEnable = !value.IsEmpty;
			}
		}


		public override void CreateUI()
		{
			base.CreateUI ();

			{
				var controller = this.GetController (2) as GroupGuidStackedController;
				controller.Level = 1;
			}

			this.okButton.Text = "Voir";
		}

		protected override void UpdateWidgets()
		{
			this.SetVisibility (2, this.GroupEnable);

			this.okButton.Enable = this.Date.HasValue
								&& (!this.GroupEnable || !this.GroupGuid.IsEmpty);
		}
	}
}