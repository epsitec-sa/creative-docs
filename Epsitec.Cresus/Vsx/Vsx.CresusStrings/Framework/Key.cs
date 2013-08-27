using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec
{
	public interface IKey : IEnumerable<IKey>, IEquatable<IKey>
	{
	}

	public static class Key
	{
		public static IKey Create(string value)
		{
			return new Key<string> (value);
		}
		public static ICompositeKey Split(string value, char separator = '.')
		{
			value.ThrowIfNull ();
			return new CompositeKey (value.Split (separator).Select (s => Key.Create (s)));
		}
	}

	public class Key<T> : IKey
	{
		public Key(T value)
		{
			value.ThrowIfNull ();
			this.Value = value;
		}

		public T Value
		{
			get;
			private set;
		}

		#region Object Overrides

		public override int GetHashCode()
		{
			return this.Value.GetHashCode ();
		}

		public override bool Equals(object obj)
		{
			return this.Equals (obj as IKey);
		}

		public override string ToString()
		{
			return this.Value.ToString ();
		}

		#endregion

		#region IEquatable<IKey> Members

		public bool Equals(IKey other)
		{
			var otherKey = other as Key<T>;
			return otherKey != null && (this == otherKey || object.Equals (this.Value, otherKey.Value));
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
	}
}
