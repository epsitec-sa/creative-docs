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
	/// Données pour un réglage de type booléen.
	/// </summary>
	public class BoolSettingsData : AbstractSettingsData
	{
		public BoolSettingsData(SettingsGroup group, SettingsType type, bool defaultValue)
			: base (group, type)
		{
			this.Value = defaultValue;
		}

		public BoolSettingsData(SettingsGroup group, SettingsType type)
			: base (group, type)
		{
		}


		public override bool CompareTo(AbstractSettingsData other)
		{
			return this.Value == (other as BoolSettingsData).Value;
		}

		public override void CopyFrom(AbstractSettingsData other)
		{
			this.Value = (other as BoolSettingsData).Value;
		}

		public bool Value
		{
			get;
			set;
		}
	}
}