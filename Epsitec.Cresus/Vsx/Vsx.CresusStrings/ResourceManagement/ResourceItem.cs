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
		public static ResourceItem Load(XElement element, ResourceBundle sourceBundle)
		{
			var id = element.Attribute ("id").GetString ();
			var name = element.Attribute ("name").GetString ();

			ResourceItemErrors errors = 0;
			ResourceItem neutralItem = null;
			if (sourceBundle.IsNeutral)
			{
				errors = GetNeutralErrors (id, name);
			}
			else
			{
				neutralItem = ResourceItem.GetNeutralItem (id, name, element, sourceBundle.NeutralBundle);
				errors = GetNonNeutralErrors (id, name, neutralItem);
			}

			if (errors != 0)
			{
				return new ResourceItemError (errors, id, name, element, sourceBundle, neutralItem);
			}
			return new ResourceItem (id, name, element, sourceBundle, neutralItem);
		}

		protected ResourceItem(string id, string name, XElement element, ResourceBundle sourceBundle, ResourceItem neutralItem)
		{
			this.id = id;
			this.name = name;
			this.element = element;
			this.sourceBundle = sourceBundle;
			this.neutralItem = neutralItem;
		}

		public string Id
		{
			get
			{
				if (this.id != null)
				{
					return this.id;
				}
				else if (this.neutralItem != null)
				{
					return this.neutralItem.id;
				}
				return null;
			}
		}

		public string Name
		{
			get
			{
				if (this.name != null)
				{
					return this.name;
				}
				else if (this.neutralItem != null)
				{
					return this.neutralItem.name;
				}
				return null;
			}
		}

		public string Value
		{
			get
			{
				return this.element == null ? null : this.element.Value;
			}
		}

		public ResourceBundle Bundle
		{
			get
			{
				return this.sourceBundle;
			}
		}

		public bool IsNeutral
		{
			get
			{
				return this.neutralItem == null;
			}
		}

		public ResourceItem NeutralItem
		{
			get
			{
				return this.neutralItem;
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


		private static ResourceItem GetNeutralItem(string id, string name, XElement sourceElement, ResourceBundle neutralBundle)
		{
			ResourceItem neutralItem;
			if (id != null && neutralBundle.ById.TryGetValue (id, out neutralItem) || name != null && neutralBundle.ByName.TryGetValue (name, out neutralItem))
			{
				return neutralItem;
			}
			var errors = ResourceItemErrors.UndefinedResource | ResourceItem.GetNeutralErrors (id, name);
			return new ResourceItemError (errors, id, name, null, neutralBundle, null);
		}

		private static ResourceItemErrors GetNeutralErrors(string id, string name)
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

		private static ResourceItemErrors GetNonNeutralErrors(string id, string name, ResourceItem neutralItem)
		{
			ResourceItemErrors errors = 0;
			var neutralId = neutralItem.id;
			var neutralName = neutralItem.name;

			if (neutralItem is ResourceItemError)
			{
				errors |= ResourceItemErrors.NoNeutralResource;
			}
			if (id == null && neutralId == null)
			{
				errors |= ResourceItemErrors.NoId;
			}
			if (name == null && neutralName == null)
			{
				errors |= ResourceItemErrors.NoName;
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
		private readonly ResourceBundle sourceBundle;

		private readonly ResourceItem neutralItem;
	}
}
