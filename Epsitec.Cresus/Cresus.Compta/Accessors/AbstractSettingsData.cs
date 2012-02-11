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
	/// Données génériques pour un réglage.
	/// </summary>
	public abstract class AbstractSettingsData
	{
		public AbstractSettingsData(string name)
		{
			this.Name = name;
		}

		public string Name
		{
			get;
			private set;
		}

		public string Group
		{
			get
			{
				int i = this.Name.LastIndexOf ('.');

				if (i == -1)
				{
					return this.Name;
				}
				else
				{
					return this.Name.Substring (0, i);
				}
			}
		}
	}
}