using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epsitec.Cresus.ResourceManagement
{
	public class ResourceBundle : ResourceNode, IEnumerable<ResourceItem>
	{
		public ResourceBundle(string fileName, ResourceBundle neutralBundle)
			: this (fileName, XDocument.Load (fileName, LoadOptions.SetLineInfo).Root, neutralBundle)
		{
		}

		public ResourceBundle(ResourceBundle bundle, ResourceItem[] items)
			: this (bundle.fileName, bundle.element, bundle.NeutralBundle, _ => items)
		{
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

		public ResourceItem[] Items
		{
			get
			{
				return this.items;
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

		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		public bool IsNeutral
		{
			get
			{
				return this.neutralBundle == null;
			}
		}

		public ResourceBundle NeutralBundle
		{
			get
			{
				return this.neutralBundle;
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


		#region IEnumerable<BundleItem> Members

		public IEnumerator<ResourceItem> GetEnumerator()
		{
			return this.items.AsEnumerable<ResourceItem>().GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}

		#endregion

		private static IEnumerable<ResourceItem> LoadItems(XElement element, ResourceBundle sourceBundle)
		{
			return element.Elements ("data").Select (e => ResourceItem.Load (e, sourceBundle));
		}

		private ResourceBundle(string fileName, XElement element, ResourceBundle neutralBundle)
			: this (fileName, element, neutralBundle, sourceBundle => ResourceBundle.LoadItems (element, sourceBundle).ToArray ())
		{
		}

		private ResourceBundle(string fileName, XElement element, ResourceBundle neutralBundle, Func<ResourceBundle, ResourceItem[]> itemsFactory)
		{
			this.fileName		= fileName;
			this.element		= element;
			this.neutralBundle	= neutralBundle;
			this.items			= itemsFactory(this);
			this.byName			= new Lazy<IReadOnlyDictionary<string, ResourceItem>> (() => this.ToDictionary (i => i.Name));
			this.byId			= new Lazy<IReadOnlyDictionary<string, ResourceItem>> (() => this.ToDictionary (i => i.Id));
		}

		private IEnumerable<string> ToStringAtoms()
		{
			yield return this.Name;
			yield return this.Culture;
		}

		private readonly string fileName;
		private readonly ResourceBundle neutralBundle;
		private readonly XElement element;
		private readonly ResourceItem[] items;
		private readonly Lazy<IReadOnlyDictionary<string, ResourceItem>> byName;
		private readonly Lazy<IReadOnlyDictionary<string, ResourceItem>> byId;
	}
}
