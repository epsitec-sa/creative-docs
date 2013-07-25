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
			this.name = element.Attribute ("name").GetString ();
			this.id = element.Attribute ("id").GetString ();
			this.value = element.Value;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
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
			yield return string.Format ("[{0}]", this.id);
			yield return string.Format ("{0} :", this.name);
			yield return string.Format ("<{0}>", this.value);
		}

		private string name;
		private string id;
		private string value;
	}
}
