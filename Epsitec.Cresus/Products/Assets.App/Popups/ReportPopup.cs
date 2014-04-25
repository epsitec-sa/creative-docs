//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class ReportPopup : StackedPopup
	{
		public ReportPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Choix d'un rapport";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = ReportsList.ReportNames,
			});

			this.SetDescriptions (list);
		}


		public ReportType						ReportType
		{
			get
			{
				var controller = this.GetController (0) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return ReportsList.GetReportType (controller.Value);
			}
			set
			{
				var controller = this.GetController (0) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = ReportsList.GetReportIndex (value);
			}
		}


		protected override void UpdateWidgets()
		{
			this.okButton.Text = "Voir";
			this.okButton.Enable = this.ReportType != ReportType.Unknown;
		}
	}
}