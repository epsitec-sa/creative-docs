using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Epsitec.Cresus.ResourceManagement
{
	public class ResourceItem : ResourceNode, IXmlLineInfo
	{
		public static ResourceItem Load(XElement element, ResourceBundle neutralCultureBundle)
		{
			var id = element.Attribute ("id").GetString ();
			var name = element.Attribute ("name").GetString ();

			ResourceItemErrors errors = 0;
			ResourceItem neutralItem = null;
			if (neutralCultureBundle == null)
			{
				errors = GetKeysErrors (id, name);
			}
			else
			{
				neutralItem = ResourceItem.GetNeutralItem (id, name, element, neutralCultureBundle);
				if (id == null)
				{
					id = neutralItem.Id;
				}
				if (name == null)
				{
					name = neutralItem.Name;
				}
				errors = GetErrors (id, name, neutralItem);
			}

			if (errors != 0)
			{
				return new ResourceItemError (errors, id, name, element);
			}
			return new ResourceItem (id, name, element);
		}

		protected ResourceItem(string id, string name, XElement element)
		{
			this.id = id;
			this.name = name;
			this.element = element;
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Value
		{
			get
			{
				return this.element == null ? null : this.element.Value;
			}
		}


		#region Object Overrides

		public override string ToString()
		{
			return string.Join (" ", this.ToStringAtoms ());
		}

		#endregion

		#region ResourceNode Overrides

		public override ResourceNode Accept(ResourceVisitor visitor)
		{
			return visitor.VisitItem (this);
		} 

		#endregion


		#region IXmlLineInfo Members

		public bool HasLineInfo()
		{
			return this.LineInfo == null ? false : this.LineInfo.HasLineInfo ();
		}

		public int LineNumber
		{
			get
			{
				return this.HasLineInfo () ? this.LineInfo.LineNumber : 0;
			}
		}

		public int LinePosition
		{
			get
			{
				return this.HasLineInfo () ? this.LineInfo.LinePosition : 0;
			}
		}

		#endregion


		private static ResourceItem GetNeutralItem(string id, string name, XElement sourceElement, ResourceBundle neutralCultureBundle)
		{
			ResourceItem neutralItem;
			if (id   != null && neutralCultureBundle.TryGetValue (id, out neutralItem) ||
				name != null && neutralCultureBundle.ByName.TryGetValue (name, out neutralItem))
			{
				return neutralItem;
			}
			var errors = ResourceItemErrors.UndefinedResource | ResourceItem.GetKeysErrors (id, name);
			return new ResourceItemError (errors, id, name, null);
		}

		private static ResourceItemErrors GetKeysErrors(string id, string name)
		{
			ResourceItemErrors errors = 0;
			if (id == null)
			{
				errors |= ResourceItemErrors.NoId;
			}
			if (name == null)
			{
				errors |= ResourceItemErrors.NoName;
			}
			return errors;
		}

		private static ResourceItemErrors GetErrors(string id, string name, ResourceItem neutralItem)
		{
			ResourceItemErrors errors = ResourceItem.GetKeysErrors(id, name);
			var neutralId = neutralItem.id;
			var neutralName = neutralItem.name;

			if (neutralItem is ResourceItemError)
			{
				errors |= ResourceItemErrors.NoNeutralResource;
			}
			if (id != null && neutralId != null && id != neutralId)
			{
				errors |= ResourceItemErrors.IdMismatch;
			}
			if (name != null && neutralName != null && name != neutralName)
			{
				errors |= ResourceItemErrors.NameMismatch;
			}
			return errors;
		}

		private IXmlLineInfo LineInfo
		{
			get
			{
				return this.element as IXmlLineInfo;
			}
		}

		private IEnumerable<string> ToStringAtoms()
		{
			yield return string.Format ("[{0}]", this.Id ?? "?");
			yield return string.Format ("{0} :", this.Name ?? "?");
			yield return string.Format ("<{0}>", this.Value ?? string.Empty);
		}

		private readonly string id;
		private readonly string name;
		private readonly XElement element;
	}
}
