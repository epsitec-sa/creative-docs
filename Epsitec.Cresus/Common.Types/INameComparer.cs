using System.Collections.Generic;


namespace Epsitec.Common.Types
{


	public sealed class INameComparer : EqualityComparer<IName>
	{


		public INameComparer() : base ()
		{
		}


		public override bool Equals(IName x, IName y)
		{
			return x != null
				&& y != null
				&& x.Name != null
				&& y.Name != null
				&& string.Equals (x.Name, y.Name);
		}


		public override int GetHashCode(IName obj)
		{
			int hash = 0;

			if (obj != null && obj.Name != null)
			{
				hash = obj.Name.GetHashCode ();
			}

			return hash;
		}


	}


}
