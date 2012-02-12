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
	public class SettingsList
	{
		public SettingsList()
		{
			this.settings = new Dictionary<string, AbstractSettingsData> ();
			this.errors = new Dictionary<string, FormattedText> ();

			this.Initialize ();
		}


		private void Initialize()
		{
			this.Add (new TextSettingsData ("Global.Titre",       20, "vide"));
			this.Add (new TextSettingsData ("Global.Description", 100, ""));

			this.Add (new BoolSettingsData ("Ecriture.AutoPièces",      true));
			this.Add (new TextSettingsData ("Ecriture.ProchainePièce",  10, "1"));
			this.Add (new IntSettingsData  ("Ecriture.IncrémentPièce",  1));
			this.Add (new BoolSettingsData ("Ecriture.PlusieursPièces", false));
			this.Add (new BoolSettingsData ("Ecriture.ForcePièces",     false));

			this.Add (new EnumSettingsData ("Price.DecimalDigits",    "2",         "0", "1", "2", "3", "4", "5"));
			this.Add (new EnumSettingsData ("Price.DecimalSeparator", ".",         ".", ","));
			this.Add (new EnumSettingsData ("Price.GroupSeparator",   "'",         "None", "'", "Space", ",", "."));
			this.Add (new EnumSettingsData ("Price.NullParts",        "00",        "00", "0t", "t0", "tt"));
			this.Add (new EnumSettingsData ("Price.NegativeFormat",   "Negative",  "Negative", "()"));
			this.Add (new SampleSettingsData ("Price.Sample", this));

			this.Add (new EnumSettingsData ("Date.Separator", ".",    ".", "/", "-"));
			this.Add (new EnumSettingsData ("Date.Year",      "4",    "4", "2"));
			this.Add (new EnumSettingsData ("Date.Order",     "DMY",  "DMY", "YMD"));
			this.Add (new SampleSettingsData ("Date.Sample", this));
		}

		private void Add(AbstractSettingsData data)
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


		public string GetEnum(string name)
		{
			AbstractSettingsData data;
			if (this.settings.TryGetValue (name, out data))
			{
				return (data as EnumSettingsData).Value;
			}
			else
			{
				return null;
			}
		}

		public void SetEnum(string name, string value)
		{
			AbstractSettingsData data;
			if (this.settings.TryGetValue (name, out data))
			{
				(data as EnumSettingsData).Value = value;
			}
		}


		public void Validate()
		{
			this.errors.Clear ();
			this.errorCount = 0;

			if (this.GetEnum ("Price.DecimalSeparator") == this.GetEnum ("Price.GroupSeparator"))
			{
				this.AddError ("Price.DecimalSeparator", "Price.GroupSeparator", "Mêmes séparateurs");
			}

			if (this.GetEnum ("Price.NegativeFormat") == "Negative" &&
				this.GetEnum ("Price.NullParts")[0] == 't')
			{
				this.AddError ("Price.NegativeFormat", "Price.NullParts", "Choix incompatibles");
			}
		}

		private void AddError(string key1, string key2, FormattedText message)
		{
			this.errors.Add (key1, message);
			this.errors.Add (key2, message);

			this.errorCount++;
		}

		public bool HasError
		{
			get
			{
				return this.errorCount != 0;
			}
		}

		public int ErrorCount
		{
			get
			{
				return this.errorCount;
			}
		}

		public FormattedText GetError(string key)
		{
			FormattedText error;

			if (errors.TryGetValue (key, out error))
			{
				return error;
			}
			else
			{
				return FormattedText.Null;
			}
		}


		private readonly Dictionary<string, AbstractSettingsData>	settings;
		private readonly Dictionary<string, FormattedText>			errors;

		private int errorCount;
	}
}