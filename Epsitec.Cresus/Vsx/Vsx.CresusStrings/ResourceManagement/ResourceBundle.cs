using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epsitec.Cresus.ResourceManagement
{
	public class ResourceBundle : ResourceElement, IResourceTable
	{
		public static ResourceBundle Load(string fileName, ResourceModule module = null, ResourceBundle neutralCultureBundle = null)
		{
			try
			{
				return new ResourceBundle (fileName, module, neutralCultureBundle);
			}
			catch
			{
				return null;
			}
		}

		public ResourceBundle(ResourceBundle bundle, IReadOnlyDictionary<string, ResourceItem> byId)
			: this (bundle.fileName, bundle.Element, bundle.Module, byId)
		{
		}


		public ResourceModule					Module
		{
			get
			{
				return this.module;
			}
		}
		public string							Name
		{
			get
			{
				return this.Element.Attribute ("name").GetString ();
			}
		}
		public string							Type
		{
			get
			{
				return this.Element.Attribute ("type").GetString ();
			}
		}
		public string							FileName
		{
			get
			{
				return this.fileName;
			}
		}
		public bool								HasNeutralCulture
		{
			get
			{
				var neutral = this.GetNeutralCultureBundle ();
				return neutral == null || neutral == this;
			}
		}

		public IReadOnlyDictionary<string, ResourceItem> ByName
		{
			get
			{
				return this.byName.Value;
			}
		}

		public ResourceBundle GetNeutralCultureBundle()
		{
			return this.module == null ? null : this.module.GetNeutralCultureBundle (this);
		}

		public string GetSymbolTail(ResourceItem item)
		{
			return item.Name;
		}

		#region Object Overrides

		public override string ToString()
		{
			return string.Join (", ", this.ToStringAtoms());
		}

		#endregion

		#region ResourceNode Overrides

		public override ResourceNode Accept(ResourceVisitor visitor)
		{
			return visitor.VisitBundle (this);
		}

		#endregion


		#region IResourceTable Members

		public CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
		}

		#endregion

		#region IReadOnlyDictionary<string,ResourceItem> Members

		public bool ContainsKey(string key)
		{
			return this.byId.ContainsKey (key);
		}

		public IEnumerable<string> Keys
		{
			get
			{
				return this.byId.Keys;
			}
		}

		public bool TryGetValue(string key, out ResourceItem value)
		{
			return this.byId.TryGetValue (key, out value);
		}

		public IEnumerable<ResourceItem> Values
		{
			get
			{
				return this.byId.Values;
			}
		}

		public ResourceItem this[string key]
		{
			get
			{
				return this.byId[key];
			}
		}

		#endregion

		#region IReadOnlyCollection<KeyValuePair<string,ResourceItem>> Members

		public int Count
		{
			get
			{
				return this.byId.Count;
			}
		}

		#endregion

		#region IEnumerable<KeyValuePair<string,ResourceItem>> Members

		public IEnumerator<KeyValuePair<string, ResourceItem>> GetEnumerator()
		{
			return this.byId.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}

		#endregion


		private static XDocument Load(string filename)
		{
			using (var reader = new StreamReader (filename))
			{
				//var text = reader.ReadToEnd ();
				var text = ResourceBundle.FixAmpersands (reader.ReadToEnd ());
				return XDocument.Parse (text, LoadOptions.SetLineInfo);
			}
		}
		private static string FixAmpersands(string text)
		{
			text = Regex.Replace (text, @"&(amp;)+", "&$1");
			text = Regex.Replace (text, @"&amp;(\w+;)", "&$1");
			return text;
		}

		private static IEnumerable<ResourceItem> LoadItems(XElement element, ResourceBundle bundle, ResourceBundle neutralCultureBundle)
		{
			return element.Elements ("data").Select (e => ResourceItem.Load (e, bundle, neutralCultureBundle));
		}


		private ResourceBundle(string fileName, ResourceModule module = null, ResourceBundle neutralCultureBundle = null)
			: this (fileName, ResourceBundle.Load (fileName).Root, module, neutralCultureBundle)
		{
		}

		private ResourceBundle(string fileName, XElement element, ResourceModule module, ResourceBundle neutralCultureBundle)
			: this (fileName, element, module, default (IReadOnlyDictionary<string, ResourceItem>))
		{
			this.byId = ResourceBundle.LoadItems (element, this, neutralCultureBundle).ToDictionary (i => i.Id);
		}

		private ResourceBundle(string fileName, XElement element, ResourceModule module, IReadOnlyDictionary<string, ResourceItem> byId)
			: base(element)
		{
			this.module					= module;
			this.fileName				= fileName;
			var cultureName				= this.Element.Attribute ("culture").GetString ();
			this.byId					= byId;
			this.byName					= new Lazy<IReadOnlyDictionary<string, ResourceItem>> (() => this.Values.ToDictionary (i => i.Name));

			if (string.IsNullOrEmpty (cultureName))
			{
				this.culture = CultureInfo.DefaultThreadCurrentUICulture;
			}
			else
			{
				this.culture = CultureInfo.CreateSpecificCulture (cultureName);
			}
		}

		private IEnumerable<string> ToStringAtoms()
		{
			yield return this.Name;
			yield return this.Culture.Name;
		}

		private readonly ResourceModule module;
		private readonly string fileName;
		private readonly CultureInfo culture;
		private readonly IReadOnlyDictionary<string, ResourceItem> byId;
		private readonly Lazy<IReadOnlyDictionary<string, ResourceItem>> byName;
	}
}
