//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Compta.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Settings.Data
{
	/// <summary>
	/// Données pour un réglage de type texte formaté.
	/// </summary>
	public class TextSettingsData : AbstractSettingsData
	{
		public TextSettingsData(SettingsGroup group, SettingsType type, int maxLength, FormattedText defaultValue)
			: base (group, type)
		{
			this.MaxLength = maxLength;
			this.Value = defaultValue;
		}

		public TextSettingsData(SettingsGroup group, SettingsType type)
			: base (group, type)
		{
		}

		public int MaxLength
		{
			get;
			set;
		}

		public FormattedText Value
		{
			get;
			set;
		}
	}
}