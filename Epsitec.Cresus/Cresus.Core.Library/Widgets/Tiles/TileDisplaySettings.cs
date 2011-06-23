//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	public sealed class TileDisplaySettings : System.IEquatable<TileDisplaySettings>
	{
		public TileDisplaySettings()
		{
		}

		public TileDisplaySettings(TileDisplaySettings other)
		{
			this.VisibilityMode = other.VisibilityMode;
			this.EditionMode    = other.EditionMode;
		}


		public TileVisibilityMode				VisibilityMode
		{
			get;
			set;
		}

		public TileEditionMode					EditionMode
		{
			get;
			set;
		}


		public XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
				new XAttribute (Xml.VisibilityMode, (int) this.VisibilityMode),
				new XAttribute (Xml.EditionMode, (int) this.EditionMode));
		}

		public static TileDisplaySettings Restore(XElement xml)
		{
			return new TileDisplaySettings ()
			{
				VisibilityMode = (TileVisibilityMode)(int)xml.Attribute (Xml.VisibilityMode),
				EditionMode    = (TileEditionMode)   (int)xml.Attribute (Xml.EditionMode)
			};
		}
		
		public static TileDisplaySettings Combine(TileDisplaySettings a, TileDisplaySettings b)
		{
			if (a == null)
			{
				if (b == null)
				{
					return null;
				}
				else
				{
					return new TileDisplaySettings (b);
				}
			}
			if (b == null)
			{
				return new TileDisplaySettings (a);
			}
			else
			{
				return new TileDisplaySettings ()
				{
					VisibilityMode = a.VisibilityMode | b.VisibilityMode,
					EditionMode    = a.EditionMode    | b.EditionMode,
				};
			}
		}

		#region IEquatable<TileDisplaySettings> Members

		public bool Equals(TileDisplaySettings other)
		{
			if (other == null)
			{
				return false;
			}
			else
			{
				return this.VisibilityMode.Simplify () == other.VisibilityMode.Simplify ()
					&& this.EditionMode.Simplify ()    == other.EditionMode.Simplify ();
			}
		}

		#endregion
		
		private static class Xml
		{
			public const string VisibilityMode	= "vis";
			public const string EditionMode		= "ed";
		}
	}
}
