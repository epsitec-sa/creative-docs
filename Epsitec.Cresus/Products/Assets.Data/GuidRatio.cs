//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public struct GuidRatio
	{
		public GuidRatio(Guid guid, decimal? ratio)
		{
			this.Guid  = guid;
			this.Ratio = ratio;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Guid.IsEmpty
					&& !this.Ratio.HasValue;
			}
		}


		public static bool operator ==(GuidRatio a, GuidRatio b)
		{
			return object.Equals (a, b);
		}

		public static bool operator !=(GuidRatio a, GuidRatio b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			var other = (GuidRatio) obj;

			return !object.ReferenceEquals (other, null)
				&& this.Guid  == other.Guid
				&& this.Ratio == other.Ratio;
		}

		public override int GetHashCode()
		{
			return this.Guid.GetHashCode ()
				 ^ this.Ratio.GetHashCode ();
		}


		public static GuidRatio Empty = new GuidRatio (Guid.Empty, null);

		public readonly Guid					Guid;
		public readonly decimal?				Ratio;
	}
}
