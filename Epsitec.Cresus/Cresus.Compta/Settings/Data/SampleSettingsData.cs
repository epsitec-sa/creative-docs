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
	/// Données pour un exemple de réglage.
	/// </summary>
	public class SampleSettingsData : AbstractSettingsData
	{
		public SampleSettingsData(string name, SettingsList settingsList)
			: base (name)
		{
			this.settingsList = settingsList;
		}

		public SampleSettingsData(string name)
			: base (name)
		{
		}

		private readonly SettingsList settingsList;
	}
}