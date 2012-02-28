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
	/// Données pour un réglage de type entier.
	/// </summary>
	public class IntSettingData : AbstractSettingData
	{
		public IntSettingData(SettingsGroup group, SettingsType type, int defaultValue)
			: base (group, type)
		{
			this.Value = defaultValue;
		}

		public IntSettingData(SettingsGroup group, SettingsType type)
			: base (group, type)
		{
		}


		public override bool CompareTo(AbstractSettingData other)
		{
			return this.Value == (other as IntSettingData).Value;
		}

		public override void CopyFrom(AbstractSettingData other)
		{
			this.Value = (other as IntSettingData).Value;
		}

		public int Value
		{
			get;
			set;
		}
	}
}