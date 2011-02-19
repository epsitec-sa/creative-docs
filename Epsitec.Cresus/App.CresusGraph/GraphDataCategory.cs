//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph
{
	public struct GraphDataCategory : System.IEquatable<GraphDataCategory>, System.IComparable<GraphDataCategory>
	{
		public GraphDataCategory(int index, string name)
		{
			this.index = index;
			this.name = name ?? "";
		}


		public string Name
		{
			get
			{
				return this.name ?? "";
			}
		}

		public int Index
		{
			get
			{
				return this.index;
			}
		}

		public bool IsGeneric
		{
			get
			{
				return this.index == 0 && this.name != null && this.name.Length == 0;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.name == null;
			}
		}

		
		public static readonly GraphDataCategory Generic = new GraphDataCategory (0, "");

		public static readonly GraphDataCategory Empty;

		
		#region IEquatable<GraphDataCategory> Members

		public bool Equals(GraphDataCategory other)
		{
			return this.index == other.index && this.name == other.name;
		}

		#endregion

		#region IComparable<GraphDataCategory> Members

		public int CompareTo(GraphDataCategory other)
		{
			int comp = this.index.CompareTo (other.index);
			
			if (comp == 0)
			{
				comp = this.name.CompareTo (other.name);
			}

			return comp;
		}

		#endregion

		
		public override bool Equals(object obj)
		{
			if (obj is GraphDataCategory)
			{
				return this.Equals ((GraphDataCategory) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.index ^ this.name.GetHashCode ();
		}

		public override string ToString()
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}:{1}", this.index, this.name);
		}

		
		private readonly int index;
		private readonly string name;
	}
}
