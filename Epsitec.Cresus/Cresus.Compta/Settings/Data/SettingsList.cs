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
			this.Initialize ();
		}


		private void Initialize()
		{
			this.Add (new TextSettingsData ("Global.Titre",       "vide"));
			this.Add (new TextSettingsData ("Global.Description", ""));

			this.Add (new BoolSettingsData ("Ecriture.AutoPièces",      true));
			this.Add (new TextSettingsData ("Ecriture.ProchainePièce",  "1"));
			this.Add (new IntSettingsData  ("Ecriture.IncrémentPièce",  1));
			this.Add (new BoolSettingsData ("Ecriture.PlusieursPièces", false));
			this.Add (new BoolSettingsData ("Ecriture.ForcePièces",     false));

			this.Add (new EnumSettingsData ("Nombres.Décimales", "2",   "0", "1", "2", "3", "4", "5"));
			this.Add (new EnumSettingsData ("Nombres.SépFrac",   ".",   ".", ","));
			this.Add (new EnumSettingsData ("Nombres.Milliers",  "'",   "Aucun", "'", "Espace", ",", "."));
			this.Add (new EnumSettingsData ("Nombres.Nul",       "00",  "00", "0t", "t0", "tt"));
			this.Add (new EnumSettingsData ("Nombres.Négatif",   "-",   "Nég", "()"));

			this.Add (new EnumSettingsData ("Dates.Sép",   ".",   ".", "/", "-"));
			this.Add (new EnumSettingsData ("Dates.Année", "4",   "4", "2"));
			this.Add (new EnumSettingsData ("Dates.Ordre", "jma", "jma", "amj"));
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


		private readonly Dictionary<string, AbstractSettingsData>	settings;
	}
}