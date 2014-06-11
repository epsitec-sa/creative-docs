//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Choix de la date à laquelle coller l'événement dans le clipboard.
	/// </summary>
	public class EventPastePopup : StackedPopup
	{
		public EventPastePopup(DataAccessor accessor, DataObject obj, EventType eventType)
			: base (accessor)
		{
			this.obj       = obj;
			this.eventType = eventType;

			var ed = DataDescriptions.GetEventDescription (this.eventType);
			this.title = string.Format ("Coller l'événement \"{0}\"", ed);

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Mandat,
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

		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			this.okButton.Text = "Coller";
			this.okButton.Enable = this.IsEnable;

			this.OnDateChanged (this.Date);
		}


		private bool IsEnable
		{
			get
			{
				if (this.Date.HasValue)
				{
					var types = AssetCalculator.GetPlausibleEventTypes (this.obj, new Timestamp (this.Date.Value, 0));
					return types.ToArray ().Contains (this.eventType);
				}
				else
				{
					return false;
				}
			}
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor,
			DataObject obj, EventType eventType, System.DateTime? date,
			System.Action<System.DateTime?> dateChanged,
			System.Action<System.DateTime> action)
		{
			if (target != null)
			{
				var popup = new EventPastePopup (accessor, obj, eventType)
				{
					Date = date,
				};

				popup.Create (target);

				dateChanged (date);

				popup.DateChanged += delegate (object sender, System.DateTime? dateTime)
				{
					dateChanged (dateTime);
				};

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


		#region Events handler
		private void OnDateChanged(System.DateTime? dateTime)
		{
			this.DateChanged.Raise (this, dateTime);
		}

		public event EventHandler<System.DateTime?> DateChanged;
		#endregion


		private DataObject						obj;
		private EventType						eventType;
	}
}