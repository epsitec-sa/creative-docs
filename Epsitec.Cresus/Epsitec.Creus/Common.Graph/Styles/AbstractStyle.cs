//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using System.Xml.Linq;

namespace Epsitec.Common.Graph.Styles
{
	public abstract class AbstractStyle
	{
		protected AbstractStyle()
		{
		}


		public string Name
		{
			get;
			protected set;
		}
		
		public abstract void ApplyStyle(int index, IPaintPort port);

		
		public virtual XElement SaveSettings(XElement xml)
		{
			xml.Add (new XAttribute ("name", this.Name ?? ""));
			return xml;
		}

		public virtual void RestoreSettings(XElement xml)
		{
			this.Name = (string) xml.Attribute ("name");
		}



		protected int ConstrainIndex(int index, int count)
		{
			if (count == 0)
			{
				throw new System.InvalidOperationException ("No style items defined");
			}

			if (index >= count)
			{
				index = index % count;
			}

			return index;
		}
	}
}
