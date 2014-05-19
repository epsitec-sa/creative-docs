//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class EventPastePopup : StackedPopup
	{
		public EventPastePopup(DataAccessor accessor, EventType eventType)
			: base (accessor)
		{
			var ed = DataDescriptions.GetEventDescription (eventType);
			this.title = string.Format ("Coller l'événement \"{0}\"", ed);

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Free,
				Label                 = "A cette date",
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


		public override void CreateUI()
		{
			base.CreateUI ();

			var controller = this.GetController (0);
			controller.SetFocus ();
		}

		protected override void UpdateWidgets()
		{
			this.okButton.Text = "Coller";
			this.okButton.Enable = this.Date.HasValue;
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, EventType eventType, System.DateTime date, System.Action<System.DateTime> action)
		{
			if (target != null)
			{
				var popup = new EventPastePopup (accessor, eventType)
				{
					Date = date,
				};

				popup.Create (target);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "ok" && popup.Date.HasValue)
					{
						action (popup.Date.Value);
					}
				};
			}
		}
		#endregion
	}
}