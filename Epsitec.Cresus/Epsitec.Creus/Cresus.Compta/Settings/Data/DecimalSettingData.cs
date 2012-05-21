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
	/// Données pour un réglage de type réel (montant).
	/// </summary>
	public class DecimalSettingData : AbstractSettingData
	{
		public DecimalSettingData(SettingsGroup group, SettingsType type, decimal defaultValue, decimal minValue, decimal maxValue)
			: base (group, type)
		{
			this.Value    = defaultValue;
			this.MinValue = minValue;
			this.MaxValue = maxValue;
		}

		public DecimalSettingData(SettingsGroup group, SettingsType type)
			: base (group, type)
		{
		}


		public override bool CompareTo(AbstractSettingData other)
		{
			return this.Value == (other as DecimalSettingData).Value;
		}

		public override void CopyFrom(AbstractSettingData other)
		{
			this.Value = (other as DecimalSettingData).Value;
		}

		public decimal Value
		{
			//	Valeur finale, forcément valide.
			get;
			set;
		}

		public decimal EditedValue
		{
			//	Valeur en édition, pouvant être invalide.
			get;
			set;
		}

		public decimal MinValue
		{
			get;
			private set;
		}

		public decimal MaxValue
		{
			get;
			private set;
		}
	}
}