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
	/// Données génériques pour un réglage.
	/// </summary>
	public abstract class AbstractSettingData
	{
		public AbstractSettingData(SettingsGroup group, SettingsType type, bool skipCompareTo = false)
		{
			this.Group         = group;
			this.Type          = type;
			this.SkipCompareTo = skipCompareTo;
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

		public bool SkipCompareTo
		{
			get;
			private set;
		}

		public virtual bool CompareTo(AbstractSettingData other)
		{
			return true;
		}

		public virtual void CopyFrom(AbstractSettingData other)
		{
		}
	}
}