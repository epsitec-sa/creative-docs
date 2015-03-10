//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Helpers;

namespace Epsitec.Cresus.Assets.Data
{
	public struct Guid : System.IEquatable<Guid>
	{
		public Guid(System.Guid guid)
		{
			this.guid = guid;
		}

		public Guid(System.Xml.XmlReader reader)
		{
			this.guid = System.Guid.Empty;

			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Text)
				{
					var g = reader.Value.ParseGuid ();
					this.guid = g.guid;
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
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

				if (array[0] != 0 &&
					array[1] != 0 &&
					array[2] != 0 &&
					array[3] != 0 &&
					array[4] != 0 &&
					array[5] != 0 &&
					array[6] != 0 &&
					array[7] != 0)
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


		#region IEquatable<Guid> Members
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
			return new Guid (System.Guid.ParseExact (text, "N"));
		}


		public void Serialize(System.Xml.XmlWriter writer, string name)
		{
			writer.WriteElementString (name, this.ToStringIO ());
		}


		private readonly System.Guid			guid;
	}
}
