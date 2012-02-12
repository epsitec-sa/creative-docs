//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Settings.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Settings.Controllers
{
	public abstract class AbstractSettingsController
	{
		public AbstractSettingsController(AbstractSettingsData data, System.Action actionChanged)
		{
			this.data = data;
			this.actionChanged = actionChanged;
		}

		public virtual void CreateUI(Widget parent)
		{
		}

		public virtual void Update()
		{
		}

		public string Name
		{
			get
			{
				return this.data.Name;
			}
		}

		public bool HasError
		{
			get
			{
				return !this.error.IsNullOrEmpty;
			}
		}

		public void SetError(FormattedText error)
		{
			if (this.error != error)
			{
				this.error = error;
				this.UpdateError ();
			}
		}

		private void UpdateError()
		{
			if (this.errorField != null)
			{
				if (this.HasError)
				{
					this.errorField.BackColor = Color.FromHexa ("ffb1b1");  // rouge pâle
					this.errorField.FormattedText = "  " + this.error;
				}
				else
				{
					this.errorField.BackColor = Color.Empty;
					this.errorField.FormattedText = FormattedText.Empty;
				}
			}
		}


		protected void CreateLabel(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				FormattedText    = VerboseSettings.GetDescription (this.data.Name),
				ContentAlignment = ContentAlignment.MiddleRight,
				PreferredWidth   = AbstractSettingsController.labelWidth-10,
				Dock             = DockStyle.Left,
				Margins          = new Margins (0, 10, 0, 3),  // bottom = 3 permet d'aligner les lignes de base !
			};
		}

		protected void CreateError(Widget parent)
		{
			this.errorField = new StaticText
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
				Margins = new Margins (5, 0, 0, 3),  // bottom = 3 permet d'aligner les lignes de base !
			};
		}


		protected readonly static int labelWidth = 210;

		protected readonly AbstractSettingsData		data;
		protected readonly System.Action			actionChanged;

		protected FormattedText						error;
		protected StaticText						errorField;
		protected int								tabIndex;
	}
}
