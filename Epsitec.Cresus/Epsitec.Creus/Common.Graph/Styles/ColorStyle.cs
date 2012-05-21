//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Common.Graph.Styles
{
	public class ColorStyle : AbstractStyle, IEnumerable<Color>
	{
		public ColorStyle(string name)
		{
			this.colors = new List<Color> ();
			this.Name = name;
		}


		public int Count
		{
			get
			{
				return this.colors.Count;
			}
		}

		public Color this[int index]
		{
			get
			{
				int n = this.colors.Count;
				
				if (n == 0)
				{
					return Color.Empty;
				}
				else
				{
					return this.colors[index % n];
				}
			}
		}
		
		
		public void Add(string name)
		{
			this.colors.Add (Color.FromName (name));
		}

		public void Add(Color color)
		{
			this.colors.Add (color);
		}

		public void Clear()
		{
			this.colors.Clear ();
		}

		public void DefineColor(int index, Color color)
		{
			if (index >= this.colors.Count)
			{
				int n = this.colors.Count;
				int i = n-1;

				while (i++ < index)
				{
					this.colors.Add (this.colors[i - n]);
				}
			}

			this.colors[index] = color;
		}


		public override void ApplyStyle(int index, IPaintPort port)
		{
			Color color = this.colors[this.ConstrainIndex (index, this.colors.Count)];

			port.Color = color;
		}

		public override XElement SaveSettings(XElement xml)
		{
			xml = base.SaveSettings (xml);

			xml.Add (new XElement ("colors",
				from color in this.colors
				select new XElement ("color", new XAttribute ("value", Color.ToHexa (color)))));

			return xml;
		}

		public override void RestoreSettings(XElement xml)
		{
			base.RestoreSettings (xml);

			var colors = xml.Element ("colors");

			this.colors.Clear ();
			this.colors.AddRange (from node in colors.Elements ("color")
								  select Color.FromHexa ((string) node.Attribute ("value")));
		}

		
		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.colors.GetEnumerator ();
		}

		#endregion

		#region IEnumerable<Color> Members

		IEnumerator<Color> IEnumerable<Color>.GetEnumerator()
		{
			return this.colors.GetEnumerator ();
		}

		#endregion

		
		private readonly List<Color> colors;
	}
}
