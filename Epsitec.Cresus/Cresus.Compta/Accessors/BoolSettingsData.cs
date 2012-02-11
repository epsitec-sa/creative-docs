//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Compta.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données pour un réglage de type booléen.
	/// </summary>
	public class BoolSettingsData : AbstractSettingsData
	{
		public BoolSettingsData(string name, bool defaultValue)
			: base (name)
		{
			this.Value = defaultValue;
		}

		public BoolSettingsData(string name)
			: base (name)
		{
		}

		public bool Value
		{
			get;
			set;
		}
	}
}