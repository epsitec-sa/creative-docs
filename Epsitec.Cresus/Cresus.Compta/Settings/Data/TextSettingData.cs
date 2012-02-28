//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Compta.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Settings.Data
{
	/// <summary>
	/// Données pour un réglage de type texte formaté.
	/// </summary>
	public class TextSettingData : AbstractSettingData
	{
		public TextSettingData(SettingsGroup group, SettingsType type, int maxLength, FormattedText defaultValue, bool skipCompareTo = false)
			: base (group, type, skipCompareTo)
		{
			this.MaxLength = maxLength;
			this.Value     = defaultValue;
		}

		public TextSettingData(SettingsGroup group, SettingsType type)
			: base (group, type)
		{
		}

		public int MaxLength
		{
			get;
			set;
		}


		public override bool CompareTo(AbstractSettingData other)
		{
			var o = (other as TextSettingData);

			if (this.Value.IsNullOrEmpty && o.Value.IsNullOrEmpty)
			{
				return true;
			}

			return this.Value == o.Value;
		}

		public override void CopyFrom(AbstractSettingData other)
		{
			this.Value = (other as TextSettingData).Value;
		}

		public FormattedText Value
		{
			get;
			set;
		}
	}
}