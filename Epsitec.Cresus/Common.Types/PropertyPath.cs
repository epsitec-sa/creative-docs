//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class PropertyPath : System.IEquatable<PropertyPath>
	{
		public PropertyPath()
		{
		}
		public PropertyPath(string path)
		{
			this.path = path;
		}
		public PropertyPath(string path, params Property[] elements)
		{
			this.path = path;
			this.elements = Copier.CopyArray (elements);
		}

		public string							Path
		{
			get
			{
				return this.path;
			}
			set
			{
				this.path = value;
			}
		}
		public ReadOnlyArray<Property>			Elements
		{
			get
			{
				return new ReadOnlyArray<Property> (this.elements);
			}
			set
			{
				this.elements = value.ToArray ();
			}
		}

		#region IEquatable<PropertyPath> Members
		public bool Equals(PropertyPath other)
		{
			return this == other;
		}
		#endregion
		
		public override bool Equals(object obj)
		{
			return this.Equals (obj as PropertyPath);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}
		
		public static bool operator==(PropertyPath p1, PropertyPath p2)
		{
			if (System.Object.ReferenceEquals (p1, p2))
			{
				return true;
			}
			
			if (p1 == null)
			{
				return false;
			}
			if (p2 == null)
			{
				return false;
			}

			if (p1.path != p2.path)
			{
				return false;
			}

			return Comparer.EqualObjects (p1.elements, p2.elements);
		}
		public static bool operator!=(PropertyPath p1, PropertyPath p2)
		{
			return (p1 == p2) ? false : true;
		}
		
		private string							path;
		private Property[]						elements;

	}
}
