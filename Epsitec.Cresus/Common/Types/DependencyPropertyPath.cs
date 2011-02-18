//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class DependencyPropertyPath : System.IEquatable<DependencyPropertyPath>
	{
		public DependencyPropertyPath()
		{
		}
		public DependencyPropertyPath(string path)
		{
			this.path = path;
		}
		public DependencyPropertyPath(string path, params DependencyProperty[] elements)
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
		public Collections.ReadOnlyList<DependencyProperty> Elements
		{
			get
			{
				return new Collections.ReadOnlyList<DependencyProperty> (this.elements);
			}
			set
			{
				this.elements = value.ToArray ();
			}
		}
		public bool								IsEmpty
		{
			get
			{
				string full = this.GetFullPath ();

				return (full == null) || (full.Length == 0);
			}
		}

		public string GetFullPath()
		{
			if ((this.elements == null) ||
				(this.elements.Length == 0))
			{
				return this.path;
			}
			else
			{
				string[] args = new string[this.elements.Length];
				System.Text.StringBuilder full = new System.Text.StringBuilder ();

				for (int i = 0; i < args.Length; i++)
				{
					args[i] = this.elements[i].Name;
					
					if (full.Length > 0)
					{
						full.Append (".");
					}
					
					full.Append (args[i]);
				}

				while (this.path.Contains ("{*}"))
				{
					this.path = this.path.Replace ("{*}", full.ToString ());
				}

				return string.Format (System.Globalization.CultureInfo.InvariantCulture, this.path, args);
			}
		}

		#region IEquatable<DependencyPropertyPath> Members
		public bool Equals(DependencyPropertyPath other)
		{
			return this == other;
		}
		#endregion
		
		public override bool Equals(object obj)
		{
			return this.Equals (obj as DependencyPropertyPath);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}
		
		public static bool operator==(DependencyPropertyPath p1, DependencyPropertyPath p2)
		{
			if (System.Object.ReferenceEquals (p1, p2))
			{
				return true;
			}
			if (System.Object.ReferenceEquals (p1, null))
			{
				return false;
			}
			if (System.Object.ReferenceEquals (p2, null))
			{
				return false;
			}

			if (p1.path != p2.path)
			{
				return false;
			}

			return Comparer.EqualObjects (p1.elements, p2.elements);
		}
		public static bool operator!=(DependencyPropertyPath p1, DependencyPropertyPath p2)
		{
			return (p1 == p2) ? false : true;
		}

		public static DependencyPropertyPath Combine(DependencyPropertyPath p1, DependencyPropertyPath p2)
		{
			if (p1 == null)
			{
				return p2;
			}
			if (p2 == null)
			{
				return p1;
			}

			string path;

			if ((p1.path == null) ||
				(p1.path.Length == 0))
			{
				path = p2.GetFullPath ();
			}
			else if ((p2.path == null) ||
				     (p2.path.Length == 0))
			{
				path = p1.GetFullPath ();
			}
			else
			{
				path = string.Concat (p1.GetFullPath (), ".", p2.GetFullPath ());
			}

			return new DependencyPropertyPath (path);
		}

		public static string Combine(string p1, string p2)
		{
			if (p1 == null)
			{
				return p2;
			}
			if (p2 == null)
			{
				return p1;
			}

			if (p1.Length == 0)
			{
				return p2;
			}
			if (p2.Length == 0)
			{
				return p1;
			}
			
			return string.Concat (p1, ".", p2);
		}

		private string							path;
		private DependencyProperty[]			elements;
	}
}
