//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractParamsPanel
	{
		public AbstractParamsPanel(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		public AbstractParams ReportParams
		{
			get
			{
				return this.reportParams;
			}
			set
			{
				this.reportParams = value;
				this.UpdateUI ();
			}
		}


		public virtual void CreateUI(Widget parent)
		{
		}

		protected virtual void UpdateUI()
		{
		}


		#region Events handler
		protected void OnParamsChanged()
		{
			this.ParamsChanged.Raise (this);
		}

		public event EventHandler ParamsChanged;
		#endregion


		protected readonly DataAccessor			accessor;

		protected AbstractParams				reportParams;
	}
}
