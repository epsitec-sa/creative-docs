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
	public class EnumSettingsData : AbstractSettingsData
	{
		public EnumSettingsData(SettingsGroup group, SettingsType type, SettingsEnum defaultValue, params SettingsEnum[] enums)
			: base (group, type)
		{
			this.Value = defaultValue;
			this.enums = enums;
		}

		public EnumSettingsData(SettingsGroup group, SettingsType type)
			: base (group, type)
		{
		}


		public override bool CompareTo(AbstractSettingsData other)
		{
			return this.Value == (other as EnumSettingsData).Value;
		}

		public override void CopyFrom(AbstractSettingsData other)
		{
			this.Value = (other as EnumSettingsData).Value;
		}

		public SettingsEnum Value
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