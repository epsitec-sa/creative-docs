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
			this.element = element;
			this.items   = new Lazy<IEnumerable<ResourceItem>>(() => this.element.Elements ("data").Select (e => new ResourceItem (e)).ToList());
			this.byName  = new Lazy<IReadOnlyDictionary<string, ResourceItem>> (() => this.ToDictionary (i => i.Name));
			this.byId    = new Lazy<IReadOnlyDictionary<string, ResourceItem>> (() => this.ToDictionary (i => i.Id));
		}

		public string Name
		{
			get
			{
				return this.element.Attribute ("name").GetString ();
			}
		}

		public string Type
		{
			get
			{
				return this.element.Attribute ("type").GetString ();
			}
		}

		public string Culture
		{
			get
			{
				return this.element.Attribute ("culture").GetString ();
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
			return this.items.Value.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}

		#endregion

		private IEnumerable<string> ToStringAtoms()
		{
			yield return this.Name;
			yield return this.Culture;
		}

		private readonly XElement element;
		private readonly Lazy<IEnumerable<ResourceItem>> items;
		private readonly Lazy<IReadOnlyDictionary<string, ResourceItem>> byName;
		private readonly Lazy<IReadOnlyDictionary<string, ResourceItem>> byId;
	}
}
