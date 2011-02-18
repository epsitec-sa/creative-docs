using System.Collections.Generic;


namespace Epsitec.Common.Support
{


	public sealed class ArrayEqualityComparer<T> : EqualityComparer<T[]>
	{


		public override int GetHashCode(T[] obj)
		{
			int result = 0;

			if (obj != null)
			{
				result = 1000000007;

				result = 37 * result + obj.Length.GetHashCode ();

				for (int i = 0; i < obj.Length; i++)
				{
					int c = (obj[i] != null) ? obj[i].GetHashCode () : 0;

					result = 37 * result + c;
				}
			}

			return result;
		}


		public override bool Equals(T[] x, T[] y)
		{
			bool different = x == null || y == null || x.Length != y.Length;

			for (int i = 0; !different && i < x.Length; i++)
			{
				different = !x[i].Equals (y[i]);
			}

			return !different;
		}


	}


}
