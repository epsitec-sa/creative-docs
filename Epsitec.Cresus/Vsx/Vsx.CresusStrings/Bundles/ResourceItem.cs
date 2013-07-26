using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epsitec.Cresus.Strings.Bundles
{
	public class ResourceItem
	{
		public ResourceItem(XElement element)
		{
			this.element = element;
		}

		public string Name
		{
			get
			{
				return this.element.Attribute ("name").GetString ();
			}
		}

		public string Id
		{
			get
			{
				return this.element.Attribute ("id").GetString ();
			}
		}

		public string Value
		{
			get
			{
				return this.element.Value;
			}
		}

		#region Object Overrides

		public override string ToString()
		{
			return string.Join (" ", this.ToStringAtoms ());
		}

		#endregion

		private IEnumerable<string> ToStringAtoms()
		{
			yield return string.Format ("[{0}]", this.Id);
			yield return string.Format ("{0} :", this.Name);
			yield return string.Format ("<{0}>", this.Value);
		}

		private readonly XElement element;
	}
}
