using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec
{
	public interface IKey : IEnumerable<IKey>, IEquatable<IKey>
	{
		IEnumerable<object> Values
		{
			get;
		}
	}

	public class Key : IKey
	{
		public static IKey Create(object key)
		{
			return Key.ToKey(key);
		}

		public static ICompositeKey Create(IEnumerable<object> subkeys)
		{
			return new CompositeKey (subkeys.Select (s => Key.ToKey (s)).Where(k => k != null));
		}

		public static ICompositeKey Create(object key, params object[] subkeys)
		{
			return Key.Create(key.AsSequence().Concat(subkeys));
		}


		#region Object Overrides

		public override int GetHashCode()
		{
			return this.value.GetHashCode ();
		}

		public override bool Equals(object obj)
		{
			return this.Equals (obj as IKey);
		}

		public override string ToString()
		{
			return this.value.ToString ();
		}

		#endregion

		#region IKey Members

		public IEnumerable<object> Values
		{
			get
			{
				yield return this.value;
			}
		}

		#endregion

		#region IEquatable<IKey> Members

		public bool Equals(IKey other)
		{
			var otherKey = other as Key;
			return otherKey != null && (this == otherKey || object.Equals (this.value, otherKey.value));
		}

		#endregion

		#region IEnumerable<IKey> Members

		public IEnumerator<IKey> GetEnumerator()
		{
			return this.AsSequence ().GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}

		#endregion

		private static IKey ToKey(object o)
		{
			if (o is IKey)
			{
				return o as IKey;
			}
			else if (o is IEnumerable<object>)
			{
				return Key.Create (o as IEnumerable<object>);
			}
			else if (o == null)
			{
				return null;
			}
			else
			{
				return new Key (o);
			}
		}

		private Key(object value)
		{
			value.ThrowIfNull ();
			this.value = value;
		}

		private readonly object value;
	}
}
