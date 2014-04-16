//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AssetsParamsPanel : AbstractParamsPanel
	{
		public AssetsParamsPanel(DataAccessor accessor)
			: base (accessor)
		{
			this.reportParams = new AssetsParams (Timestamp.Now, Guid.Empty);
		}


		public override void CreateUI(Widget parent)
		{
			this.timestampController = new StateAtController (this.accessor);
			this.timestampController.CreateUI (parent);

			this.timestampController.DateChanged += delegate
			{
				this.UpdateParams ();
			};
		}

		protected override void UpdateUI()
		{
			this.timestampController.Date = this.Params.Timestamp.Date;
		}

		private void UpdateParams()
		{
			if (this.timestampController.Date.HasValue)
			{
				var timestamp = new Timestamp (this.timestampController.Date.Value, 0);
				this.reportParams = new AssetsParams (timestamp, Guid.Empty);

				this.OnParamsChanged ();
			}
		}


		private AssetsParams Params
		{
			get
			{
				return this.reportParams as AssetsParams;
			}
		}


		private StateAtController				timestampController;
	}
}
