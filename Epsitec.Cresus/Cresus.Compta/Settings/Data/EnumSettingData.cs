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
	public class EnumSettingData : AbstractSettingData
	{
		public EnumSettingData(SettingsGroup group, SettingsType type, SettingsEnum defaultValue, System.Func<FormattedText> validateAction, params SettingsEnum[] enums)
			: base (group, type, validateAction)
		{
			this.Value = defaultValue;
			this.enums = enums;
		}

		public EnumSettingData(SettingsGroup group, SettingsType type)
			: base (group, type)
		{
		}


		public override bool CompareTo(AbstractSettingData other)
		{
			return this.Value == (other as EnumSettingData).Value;
		}

		public override void CopyFrom(AbstractSettingData other)
		{
			this.Value = (other as EnumSettingData).Value;
		}

		public SettingsEnum Value
		{
			get;
			set;
		}

		public SettingsEnum EditedValue
		{
			get;
			set;
		}

		public IEnumerable<SettingsEnum> Enum
		{
			get
			{
				return this.enums;
			}
		}


		private readonly SettingsEnum[] enums;
	}
}