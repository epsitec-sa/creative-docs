using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epsitec.Cresus.Strings.Bundles
{
	public class ResourceBundle : IEnumerable<ResourceItem>
	{
		public static ResourceBundle Load(string fileName)
		{
			var doc = XDocument.Load (fileName);
			return new ResourceBundle (doc.Root);
		}

		public ResourceBundle(XElement element)
		{
			this.name = element.Attribute ("name").GetString ();
			this.type = element.Attribute ("type").GetString ();
			this.culture = element.Attribute ("culture").GetString ();
			this.items = element.Elements ("data").Select (e => new ResourceItem (e)).ToList ();
			this.byName = new Lazy<IReadOnlyDictionary<string, ResourceItem>> (() => this.items.ToDictionary (i => i.Name));
			this.byId = new Lazy<IReadOnlyDictionary<string, ResourceItem>> (() => this.items.ToDictionary (i => i.Id));
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		public string Culture
		{
			get
			{
				return this.culture;
			}
		}

		public IReadOnlyDictionary<string, ResourceItem> ByName
		{
			get
			{
				return this.byName.Value;
			}
		}

		public IReadOnlyDictionary<string, ResourceItem> ById
		{
			get
			{
				return this.byId.Value;
			}
		}

		#region Object Overrides

		public override string ToString()
		{
			return string.Join (", ", this.ToStringAtoms());
		}

		#endregion

		#region IEnumerable<BundleItem> Members

		public IEnumerator<ResourceItem> GetEnumerator()
		{
			return this.items.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.items.GetEnumerator ();
		}

		#endregion

		private IEnumerable<string> ToStringAtoms()
		{
			yield return this.name;
			yield return this.culture;
		}

		private readonly string name;
		private readonly string type;
		private readonly string culture;
		private readonly List<ResourceItem> items;
		private readonly Lazy<IReadOnlyDictionary<string, ResourceItem>> byName;
		private readonly Lazy<IReadOnlyDictionary<string, ResourceItem>> byId;
	}
}
