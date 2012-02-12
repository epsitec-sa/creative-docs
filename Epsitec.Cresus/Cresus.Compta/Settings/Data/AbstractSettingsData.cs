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
	/// Données génériques pour un réglage.
	/// </summary>
	public abstract class AbstractSettingsData
	{
		public AbstractSettingsData(SettingsGroup group, SettingsType type)
		{
			this.Group = group;
			this.Type  = type;
		}

		public SettingsGroup Group
		{
			get;
			private set;
		}

		public SettingsType Type
		{
			get;
			private set;
		}
	}
}