﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Settings.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Settings.Controllers
{
	public abstract class AbstractSettingController
	{
		public AbstractSettingController(AbstractSettingData data, System.Action changedAction)
		{
			this.data           = data;
			this.changedAction  = changedAction;
		}

		public virtual void CreateUI(Widget parent)
		{
		}

		public virtual void Update()
		{
		}

		public SettingsType Type
		{
			get
			{
				return this.data.Type;
			}
		}


		public virtual void Validate()
		{
		}


		protected void SetError(FormattedText error)
		{
			this.data.SetError (error);

			if (this.errorField != null)
			{
				if (this.data.HasError)
				{
					this.errorField.BackColor = UIBuilder.ErrorColor;
					this.errorField.FormattedText = "  " + this.data.Error;
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
				FormattedText    = VerboseSettings.GetDescription (this.data.Type),
				ContentAlignment = ContentAlignment.MiddleRight,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				PreferredWidth   = AbstractSettingController.labelWidth-10,
				Dock             = DockStyle.Left,
				Margins          = new Margins (0, 10, 0, 3),  // bottom = 3 permet d'aligner les lignes de base !
			};
		}

		protected void CreateError(Widget parent)
		{
			this.errorField = new StaticText
			{
				Parent        = parent,
				TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Dock          = DockStyle.Fill,
				Margins       = new Margins (5, 0, 0, 3),  // bottom = 3 permet d'aligner les lignes de base !
			};
		}


		protected readonly static int labelWidth = 210;

		protected readonly AbstractSettingData			data;
		protected readonly System.Action				changedAction;

		protected StaticText							errorField;
		protected int									tabIndex;
	}
}
