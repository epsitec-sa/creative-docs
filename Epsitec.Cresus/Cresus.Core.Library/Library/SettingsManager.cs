//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Library
{
	public struct SettingsTuple
	{
		public SettingsTuple(string key, string value)
		{
			this.key = key;
			this.value = value;
		}

		public string Key
		{
			get
			{
				return this.key;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		private readonly string key;
		private readonly string value;
	}

	public class SettingsCollection : IEnumerable<SettingsTuple>
	{
		public SettingsCollection()
		{
			this.data = new Dictionary<string, string> ();
		}

		public SettingsCollection(IEnumerable<SettingsTuple> tuples)
			: this ()
		{
			foreach (var tuple in tuples)
			{
				this.data.Add (tuple.Key, tuple.Value);
			}
		}
		
		public string this[string key]
		{
			get
			{
				string value;

				if (this.data.TryGetValue (key, out value))
				{
					return value;
				}

				return null;
			}
			set
			{
				if (value == null)
				{
					this.data.Remove (key);
				}
				else
				{
					this.data[key] = value;
				}
			}
		}

		public int Count
		{
			get
			{
				return this.data.Count;
			}
		}

		public void Clear()
		{
			this.data.Clear ();
		}

		public void AddRange(IEnumerable<SettingsTuple> collection)
		{
			collection.ForEach (x => this[x.Key] = x.Value);
		}

		public bool Remove(string key)
		{
			return this.data.Remove (key);
		}

		private IEnumerable<SettingsTuple> GetItems()
		{
			return this.data.Select (x => new SettingsTuple (x.Key, x.Value));
		}

		#region IEnumerable<SettingTuple> Members

		public IEnumerator<SettingsTuple> GetEnumerator()
		{
			return this.GetItems ().GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetItems ().GetEnumerator ();
		}

		#endregion

		private readonly Dictionary<string, string> data;
	}


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
			this.settings[key] = value;
		}

		public SettingsCollection ExtractSettings(string startKey)
		{
			return new SettingsCollection (this.settings.Where (x => x.Key.StartsWith (startKey)));
		}

		public void MergeSettings(string startKey, SettingsCollection settings)
		{
			//	Met à jour tous les réaglages d'une catégorie donnée.

			this.RemoveSettings (startKey);
			this.settings.AddRange (settings);
		}

		private void RemoveSettings(string startKey)
		{
			this.settings
				.Select (x => x.Key)
				.Where (key => key.StartsWith (startKey))
				.ToArray ()
				.ForEach (key => this.settings.Remove (key));
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

		#region Factory Class

		private sealed class Factory : Epsitec.Cresus.Core.Factories.DefaultCoreAppComponentFactory<SettingsManager>
		{
		}

		#endregion


		private readonly SettingsCollection		settings;
	}
}
