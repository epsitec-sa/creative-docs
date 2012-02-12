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
	/// Données pour un exemple de réglage.
	/// </summary>
	public class SampleSettingsData : AbstractSettingsData
	{
		public SampleSettingsData(SettingsGroup group, SettingsType type, SettingsList settingsList)
			: base (group, type)
		{
			this.settingsList = settingsList;
		}

		public SampleSettingsData(SettingsGroup group, SettingsType type)
			: base (group, type)
		{
		}

		public SettingsList SettingsList
		{
			get
			{
				return this.settingsList;
			}
		}

		private readonly SettingsList settingsList;
	}
}