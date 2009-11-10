//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	public class Mapper<T1,T2> : System.IDisposable
	{
		public Mapper(System.Func<IEnumerable<T1>, IEnumerable<T2>> mapFunction)
		{
			this.mapperEnumerable = mapFunction (this.GetPushCollection ());
			this.mapperEnumerator = this.mapperEnumerable.GetEnumerator ();
		}

		public T2 Map(T1 value)
		{
			this.value = value;
			this.ready = true;

			if (this.mapperEnumerator.MoveNext ())
			{
				return this.mapperEnumerator.Current;
			}
			else
			{
				return default (T2);
			}
		}

		private IEnumerable<T1> GetPushCollection()
		{
			while (true)
			{
				if (this.dispose)
				{
					yield break;
				}

				if (this.ready)
				{
					this.ready = false;
					yield return this.value;
				}
				else
				{
					throw new System.InvalidOperationException ();
				}
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.dispose = true;
			bool hasMore = this.mapperEnumerator.MoveNext ();
			System.Diagnostics.Debug.Assert (hasMore == false);
			this.mapperEnumerator.Dispose ();
		}

		#endregion

		readonly IEnumerable<T2> mapperEnumerable;
		readonly IEnumerator<T2> mapperEnumerator;
		
		T1 value;
		bool dispose;
		bool ready;
	}
}
