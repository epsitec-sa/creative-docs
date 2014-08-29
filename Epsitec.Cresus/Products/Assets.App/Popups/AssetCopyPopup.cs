//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Choix de la date à considérer pour copier un objet d'immobilisation.
	/// </summary>
	public class AssetCopyPopup : StackedPopup
	{
		private AssetCopyPopup(DataAccessor accessor, DataObject obj)
			: base (accessor)
		{
			this.obj = obj;

			this.title = Res.Strings.Popup.AssetCopy.Title.ToString ();

			var list = new List<StackedControllerDescription> ();


			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = this.Labels,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Mandat,
				Label                 = "",
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Copy.ToString ();
			this.defaultControllerRankFocus = 0;
		}


		private bool								InputState
		{
			get
			{
				var controller = this.GetController (0) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value == 0;
			}
			set
			{
				var controller = this.GetController (0) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value ? 0 : 1;
			}
		}

		private System.DateTime?					Date
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


		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			this.SetVisibility (1, !this.InputState);

			this.okButton.Enable = this.IsEnable;
		}


		private bool IsEnable
		{
			get
			{
				return this.InputState || this.Date.HasValue;
			}
		}


		private string Labels
		{
			//	Retourne le texte pour les boutons radio.
			get
			{
				var date = this.InputDate;
				string label;

				if (string.IsNullOrEmpty (date))
				{
					label = Res.Strings.Popup.AssetCopy.StateGlobal.ToString ();
				}
				else
				{
					label = string.Format (Res.Strings.Popup.AssetCopy.StateInput.ToString (), date);
				}

				return string.Join ("<br/>", label, Res.Strings.Popup.AssetCopy.StateDate.ToString ());
			}
		}

		private string InputDate
		{
			//	Retourne la date d'entrée de l'objet.
			get
			{
				if (this.obj.EventsCount > 0)
				{
					var e = this.obj.GetEvent (0);
					var date = e.Timestamp.Date;
					return TypeConverters.DateToString (date);
				}

				return null;
			}
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, DataObject obj,
			System.Action<System.DateTime> action)
		{
			if (target != null)
			{
				var popup = new AssetCopyPopup (accessor, obj)
				{
					InputState = true,
					Date       = obj.GetEvent (0).Timestamp.Date,
				};

				popup.Create (target);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "ok")
					{
						action (popup.Date.Value);
					}
				};
			}
		}
		#endregion


		private DataObject						obj;
	}
}