//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Choix de la date à considérer pour copier un objet d'immobilisation.
	/// </summary>
	public class AssetPastePopup : StackedPopup
	{
		public AssetPastePopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Coller l'objet d'immobilisation";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Mandat,
				Label                 = "Date d'entrée",
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
			this.okButton.Enable = this.IsEnable;
		}


		private bool IsEnable
		{
			get
			{
				return this.Date.HasValue;
			}
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor,
			System.Action<System.DateTime> action)
		{
			if (target != null)
			{
				var popup = new AssetPastePopup (accessor)
				{
					Date = accessor.Mandat.StartDate,
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
	}
}