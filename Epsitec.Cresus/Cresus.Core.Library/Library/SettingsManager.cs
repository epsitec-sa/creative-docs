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
			var remove = (from setting in this.settings
						  let key = setting.Key
						  where key.StartsWith (startKey)
						  select key).ToArray ();

			if (remove.Length > 0)
			{
				remove.ForEach (key => this.settings.Remove (key));
				return true;
			}
			else
			{
				return false;
			}
		}

		public XElement Save(string xmlNodeName)
		{
			var nodes = this.settings.Select (x =>
				new XElement ("tuple",
					new XAttribute ("k", x.Key),
					new XAttribute ("v", x.Value)));

			return new XElement (xmlNodeName, nodes);
		}

		public void Restore(XElement xml)
		{
			this.settings.Clear ();

			if (xml == null)
			{
				return;
			}

			foreach (XElement node in xml.Elements ())
			{
				System.Diagnostics.Debug.Assert (node.Name == "tuple");

				string key   = (string) node.Attribute ("k");
				string value = (string) node.Attribute ("v");

				this.settings[key] = value;
			}
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
