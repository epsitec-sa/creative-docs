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
	public class IntSettingsData : AbstractSettingsData
	{
		public IntSettingsData(SettingsGroup group, SettingsType type, int defaultValue)
			: base (group, type)
		{
			this.Value = defaultValue;
		}

		public IntSettingsData(SettingsGroup group, SettingsType type)
			: base (group, type)
		{
		}

		public int Value
		{
			get;
			set;
		}
	}
}