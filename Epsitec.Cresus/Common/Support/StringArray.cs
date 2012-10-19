//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Collections;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>StringArray</c> structure stores zero, one or several non-null strings in a space
	/// efficient manner (for 0, 1 or 2 strings, only 8 bytes on a 32-bit architecture).
	/// </summary>
	public struct StringArray : System.IEquatable<StringArray>
	{
		public StringArray(string value)
		{
			value.ThrowIfNull ("value");

			this.first = value;
			this.next  = null;
		}

		public StringArray(params string[] values)
		{
			values.ThrowIfNull ("values");
			values.ThrowIfAnyNull ("values");
			
			int length = values.Length;

			if (length == 0)
			{
				this.first = null;
				this.next  = null;
			}
			else if (length == 1)
			{
				this.first = values[0];
				this.next  = null;
			}
			else if (length == 2)
			{
				this.first = values[0];
				this.next  = values[1];
			}
			else
			{
				this.first = values[0];
				this.next  = values.Skip (1).ToArray ();
			}
		}

		public StringArray(IEnumerable<string> values)
			: this (values.ToArray ())
		{
		}

		public StringArray(StringArray array)
		{
			this.first = array.first;
			this.next  = array.next;
		}


		public static implicit operator StringArray(string value)
		{
			return new StringArray (value);
		}


		public bool								IsEmpty
		{
			get
			{
				return this.first == null;
			}
		}
		
		public int								Count
		{
			get
			{
				if (this.first == null)
				{
					return 0;
				}
				if (this.next == null)
				{
					return 1;
				}
				if (this.next is string)
				{
					return 2;
				}

				return ((string[])this.next).Length + 1;
			}
		}

		public IEnumerable<string>				Items
		{
			get
			{
				if (this.first == null)
				{
					return Enumerable.Empty<string> ();
				}
				if (this.next == null)
				{
					return new SingleValueEnumerable<string> (this.first);
				}
				if (this.next is string)
				{
					return new string[] { this.first, (string) this.next };
				}

				return new SingleValueEnumerable<string> (this.first).Concat ((string[])this.next);
			}
		}

		public string							this[int index]
		{
			get
			{
				if (index == 0)
				{
					if (this.first != null)
					{
						return this.first;
					}
				}
				else if (index == 1)
				{
					if (this.next is string)
					{
						return (string) this.next;
					}
				}
				else if (index > 1)
				{
					if (this.next is string[])
					{
						return ((string[])this.next)[index-1];
					}
				}

				throw new System.IndexOutOfRangeException ();
			}
		}


		public string[] ToArray()
		{
			int count = this.Count;
			var array = new string[count];

			if (count > 2)
			{
				var source = (string[]) this.next;

				array[0] = this.first;

				for (int i = 1; i < count; i++)
				{
					array[i] = source[i-1];
				}
			}
			else
			{
				for (int i = 0; i < count; i++)
				{
					array[i] = this[i];
				}
			}

			return array;
		}

		public string FirstOrDefault()
		{
			return this.first;
		}


		#region IEquatable<StringArray> Members

		public bool Equals(StringArray other)
		{
			if (this.first == other.first)
			{
				if (this.next == other.next)
				{
					return true;
				}

				var next1 = this.next as string[];
				var next2 = other.next as string[];

				if ((next1 == null) ||
					(next2 == null))
				{
					return false;
				}
				if (next1.Length != next2.Length)
				{
					return false;
				}

				return next1.SequenceEqual (next2);
			}

			return false;
		}

		#endregion

		
		public override bool Equals(object obj)
		{
			if (obj is StringArray)
			{
				return this.Equals ((StringArray) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}


		private readonly string					first;
		private readonly object					next;
	}
}
