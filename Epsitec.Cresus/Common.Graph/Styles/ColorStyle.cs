//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

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
		
		
		public void Add(string name)
		{
			this.colors.Add (Color.FromName (name));
		}

		public void Add(Color color)
		{
			this.colors.Add (color);
		}


		public void DefineColor(int index, Color color)
		{
			this.colors[index] = color;
		}


		public override void ApplyStyle(int index, IPaintPort port)
		{
			Color color = this.colors[this.ConstrainIndex (index, this.colors.Count)];

			port.Color = color;
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
