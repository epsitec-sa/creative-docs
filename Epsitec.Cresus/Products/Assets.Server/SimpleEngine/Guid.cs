//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public struct Guid : System.IEquatable<Guid>
	{
		public Guid(System.Guid guid)
		{
			this.guid = guid;
		}


		public bool IsEmpty
		{
			get
			{
				return this == Guid.Empty;
			}
		}


		public static Guid Empty
		{
			get
			{
				return new Guid (System.Guid.Empty);
			}
		}


		public static Guid NewGuid()
		{
			while (true)
			{
				var guid  = System.Guid.NewGuid ();
				var array = guid.ToByteArray ();

				if ((array[0] != 0) &&
					(array[1] != 0) &&
					(array[2] != 0) &&
					(array[3] != 0) &&
					(array[4] != 0) &&
					(array[5] != 0) &&
					(array[6] != 0) &&
					(array[7] != 0))
				{
					return new Guid (guid);
				}
			}
		}

		public static Guid NewGuid(short value)
		{
			var guid = new System.Guid (0, 0, 0, 0, 0, 0, 0, 0, 0, (byte) (value >> 8), (byte) (value & 0x00ff));
			return new Guid (guid);
		}


		#region IEquatable<EqoGuid> Members
		public bool Equals(Guid other)
		{
			return this.guid == other.guid;
		}
		#endregion


		public static bool operator==(Guid a, Guid b)
		{
			return a.Equals (b);
		}

		public static bool operator!=(Guid a, Guid b)
		{
			return !a.Equals (b);
		}


		public override bool Equals(object obj)
		{
			if (obj is Guid)
			{
				return this.Equals ((Guid) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.guid.GetHashCode ();
		}

		public override string ToString()
		{
			return this.guid.ToString ("N");
		}


		public static Guid Parse(string text)
		{
			return new Guid (System.Guid.Parse (text));
		}


		private readonly System.Guid			guid;
	}
}
