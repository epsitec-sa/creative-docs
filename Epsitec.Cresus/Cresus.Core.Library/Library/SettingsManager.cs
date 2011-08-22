//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>SettingsManager</c> class implements a simple settings manager, which stores
	/// the settings as key/value string pairs.
	/// </summary>
	public class SettingsManager : CoreAppComponent
	{
		private SettingsManager(CoreApp app)
			: base (app)
		{
			this.settings = new SettingsCollection ();
		}


		public string GetSettings(string key)
		{
			return this.settings[key];
		}

		public void SetSettings(string key, string value)
		{
			if (this.settings[key] != value)
			{
				this.settings[key] = value;
				this.OnSettingsChanged ();
			}
		}

		public SettingsCollection ExtractSettings(string startKey)
		{
			return new SettingsCollection (this.settings.Where (x => x.Key.StartsWith (startKey)));
		}

		public void MergeSettings(string startKey, SettingsCollection settings)
		{
			//	Met à jour tous les réaglages d'une catégorie donnée.

			bool changed = false;

			changed |= this.RemoveSettings (startKey);
			changed |= this.settings.AddRange (settings);

			if (changed)
			{
				this.OnSettingsChanged ();
			}
		}

		private bool RemoveSettings(string startKey)
		{
			return this.settings.RemoveAllStartingWith (startKey);
		}

		public XElement Save(string xmlNodeName)
		{
			return this.settings.Save (xmlNodeName);
		}

		public void Restore(XElement xml)
		{
			this.settings.Restore (xml);
		}

		private void OnSettingsChanged()
		{
			//	Make sure the settings get persisted in some near future.

			this.Host.PersistenceManager.AsyncSave ();
		}


		#region Factory Class

		private sealed class Factory : Epsitec.Cresus.Core.Factories.DefaultCoreAppComponentFactory<SettingsManager>
		{
		}

		#endregion


		private readonly SettingsCollection		settings;
	}
}
