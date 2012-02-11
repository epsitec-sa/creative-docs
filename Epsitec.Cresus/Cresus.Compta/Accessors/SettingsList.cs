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
	public class SettingsList
	{
		public SettingsList()
		{
			this.settings = new Dictionary<string, AbstractSettingsData> ();
		}


		public void Add(AbstractSettingsData data)
		{
			this.settings.Add (data.Name, data);
		}

		public IEnumerable<AbstractSettingsData> List
		{
			get
			{
				return this.settings.Values;
			}
		}


		public bool GetBool(string name)
		{
			AbstractSettingsData data;
			if (this.settings.TryGetValue (name, out data))
			{
				return (data as BoolSettingsData).Value;
			}
			else
			{
				return false;
			}
		}

		public void SetBool(string name, bool value)
		{
			AbstractSettingsData data;
			if (this.settings.TryGetValue (name, out data))
			{
				(data as BoolSettingsData).Value = value;
			}
		}


		public int? GetInt(string name)
		{
			AbstractSettingsData data;
			if (this.settings.TryGetValue (name, out data))
			{
				return (data as IntSettingsData).Value;
			}
			else
			{
				return null;
			}
		}

		public void SetInt(string name, int value)
		{
			AbstractSettingsData data;
			if (this.settings.TryGetValue (name, out data))
			{
				(data as IntSettingsData).Value = value;
			}
		}


		public FormattedText GetText(string name)
		{
			AbstractSettingsData data;
			if (this.settings.TryGetValue (name, out data))
			{
				return (data as TextSettingsData).Value;
			}
			else
			{
				return FormattedText.Null;
			}
		}

		public void SetText(string name, FormattedText value)
		{
			AbstractSettingsData data;
			if (this.settings.TryGetValue (name, out data))
			{
				(data as TextSettingsData).Value = value;
			}
		}


		private readonly Dictionary<string, AbstractSettingsData>	settings;
	}
}