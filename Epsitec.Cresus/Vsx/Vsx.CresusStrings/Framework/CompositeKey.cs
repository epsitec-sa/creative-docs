using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec
{
	public interface ICompositeKey : IKey
	{
		ICompositeKey Concat(IKey key);
	}

	public class CompositeKey : ICompositeKey
	{
		public CompositeKey(IEnumerable<IKey> keys)
		{
			this.keys = keys.ToArray ();
		}

		public CompositeKey(params IKey[] keys)
		{
			this.keys = keys;
		}

		public static ICompositeKey Empty
		{
			get
			{
				return CompositeKey.empty;
			}
		}

		#region Object Overrides

		public override int GetHashCode()
		{
			return this.Aggregate (0, (h, k) => h ^ k.GetHashCode ());
		}

		public override bool Equals(object obj)
		{
			return this.Equals (obj as IKey);
		}

		public override string ToString()
		{
			return '{' + string.Join (".", this.Select (k => k.ToString ())) + '}';
		}

		#endregion

		#region ICompositeKey Members

		public ICompositeKey Concat(IKey key)
		{
			return new CompositeKey (this.keys.Concat (key.AsSequence ()));
		}

		#endregion

		#region IEquatable<IKey> Members

		public bool Equals(IKey other)
		{
			var otherKey = other as CompositeKey;
			return otherKey != null && (this == otherKey || this.SequenceEqual (otherKey));
		}

		#endregion

		#region IEnumerable<IKey> Members

		public IEnumerator<IKey> GetEnumerator()
		{
			return this.keys.SelectMany(k => k).GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}

		#endregion

		private class EmptyCompositeKey : ICompositeKey
		{
			#region Object Overrides

			public override int GetHashCode()
			{
				return 0;
			}

			public override bool Equals(object obj)
			{
				return this.Equals (obj as IKey);
			}

			public override string ToString()
			{
				return "{}";
			}

			#endregion

			#region ICompositeKey Members

			public ICompositeKey Concat(IKey key)
			{
				return new CompositeKey (key.AsSequence ());
			}

			public IKey[] ToArray()
			{
				return new IKey[0];
			}

			#endregion

			#region IEquatable<IKey> Members

			public bool Equals(IKey other)
			{
				return this == other;
			}

			#endregion

			#region IEnumerable<IKey> Members

			public IEnumerator<IKey> GetEnumerator()
			{
				return Enumerable.Empty<IKey> ().GetEnumerator ();
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator ();
			}

			#endregion
		}

		private static readonly ICompositeKey empty = new EmptyCompositeKey ();
		private readonly IEnumerable<IKey> keys;
	}
}
