using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epsitec.Cresus.ResourceManagement
{
	public class ResourceBundle : ResourceElement, IResourceTable
	{
		public static ResourceBundle Create(string fileName, ResourceBundle neutralCultureBundle = null)
		{
			try
			{
				return new ResourceBundle (fileName, neutralCultureBundle);
			}
			catch
			{
				return null;
			}
		}

		public ResourceBundle(string fileName, ResourceBundle neutralCultureBundle = null)
			: this (fileName, XDocument.Load (fileName, LoadOptions.SetLineInfo).Root, neutralCultureBundle)
		{
		}

		public ResourceBundle(ResourceBundle bundle, IReadOnlyDictionary<string, ResourceItem> byId)
			: this (bundle.fileName, bundle.Element, byId)
		{
		}


		public string Name
		{
			get
			{
				return this.Element.Attribute ("name").GetString ();
			}
		}

		public string Type
		{
			get
			{
				return this.Element.Attribute ("type").GetString ();
			}
		}

		public IReadOnlyDictionary<string, ResourceItem> ByName
		{
			get
			{
				return this.byName.Value;
			}
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
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


		private static IEnumerable<ResourceItem> LoadItems(XElement element, ResourceBundle neutralCultureBundle)
		{
			return element.Elements ("data").Select (e => ResourceItem.Load (e, neutralCultureBundle));
		}

	
		private ResourceBundle(string fileName, XElement element, ResourceBundle neutralCultureBundle)
			: this (fileName, element, ResourceBundle.LoadItems (element, neutralCultureBundle).ToDictionary (i => i.Id))
		{
		}

		private ResourceBundle(string fileName, XElement element, IReadOnlyDictionary<string, ResourceItem> byId)
			: base(element)
		{
			this.fileName	= fileName;
			var cultureName = this.Element.Attribute ("culture").GetString ();
			if (string.IsNullOrEmpty (cultureName))
			{
				this.culture = CultureInfo.DefaultThreadCurrentUICulture;
			}
			else
			{
				this.culture = CultureInfo.CreateSpecificCulture (cultureName);
			}

			this.byId		= byId;
			this.byName		= new Lazy<IReadOnlyDictionary<string, ResourceItem>> (() => this.Values.ToDictionary (i => i.Name));
		}

		private IEnumerable<string> ToStringAtoms()
		{
			yield return this.Name;
			yield return this.Culture.Name;
		}

		private readonly string fileName;
		private readonly CultureInfo culture;
		private readonly IReadOnlyDictionary<string, ResourceItem> byId;
		private readonly Lazy<IReadOnlyDictionary<string, ResourceItem>> byName;
	}
}
