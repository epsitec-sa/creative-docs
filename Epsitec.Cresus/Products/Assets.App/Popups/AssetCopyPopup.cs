﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public class AssetCopyPopup : StackedPopup
	{
		public AssetCopyPopup(DataAccessor accessor, DataObject obj)
			: base (accessor)
		{
			this.obj = obj;

			this.title = "Copier l'objet d'immobilisation";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = "Etat d'entrée<br/>Etat au :",
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Mandat,
				Label                 = "",
			});

			this.SetDescriptions (list);
		}


		public bool								InputState
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

		public System.DateTime?					Date
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


		public override void CreateUI()
		{
			base.CreateUI ();

			var controller = this.GetController (0);
			controller.SetFocus ();
		}

		protected override void UpdateWidgets()
		{
			this.SetVisibility (1, !this.InputState);

			this.okButton.Text = "Copier";
			this.okButton.Enable = this.IsEnable;
		}


		private bool IsEnable
		{
			get
			{
				return this.InputState || this.Date.HasValue;
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