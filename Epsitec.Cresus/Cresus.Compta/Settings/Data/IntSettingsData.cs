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
	/// Données pour un réglage de type entier.
	/// </summary>
	public class IntSettingsData : AbstractSettingsData
	{
		public IntSettingsData(string name, int defaultValue)
			: base (name)
		{
			this.Value = defaultValue;
		}

		public IntSettingsData(string name)
			: base (name)
		{
		}

		public int Value
		{
			get;
			set;
		}
	}
}