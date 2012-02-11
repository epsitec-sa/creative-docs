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
	/// Données pour un réglage de type texte formaté.
	/// </summary>
	public class TextSettingsData : AbstractSettingsData
	{
		public TextSettingsData(string name, FormattedText defaultValue)
			: base (name)
		{
			this.Value = defaultValue;
		}

		public TextSettingsData(string name)
			: base (name)
		{
		}

		public FormattedText Value
		{
			get;
			set;
		}
	}
}