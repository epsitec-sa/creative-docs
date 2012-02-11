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
	/// Données pour un réglage de type texte formaté.
	/// </summary>
	public class EnumSettingsData : AbstractSettingsData
	{
		public EnumSettingsData(string name, string defaultValue, params string[] enums)
			: base (name)
		{
			this.Value = defaultValue;
			this.enums = enums;
		}

		public EnumSettingsData(string name)
			: base (name)
		{
		}

		public string Value
		{
			get;
			set;
		}

		public IEnumerable<string> Enum
		{
			get
			{
				return this.enums;
			}
		}


		private readonly string[] enums;
	}
}