//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	public struct TileFieldDisplaySettings : System.IEquatable<TileFieldDisplaySettings>
	{
		public TileFieldDisplaySettings(TileVisibilityMode visibility, TileEditionMode edition)
		{
			this.visibility = visibility;
			this.edition    = edition;
		}

		public TileVisibilityMode FieldVisibilityMode
		{
			get
			{
				return this.visibility;
			}
		}

		public TileEditionMode FieldEditionMode
		{
			get
			{
				return this.edition;
			}
		}


		public XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
				new XAttribute (Xml.FieldVisibilityMode, (int) this.FieldVisibilityMode),
				new XAttribute (Xml.FieldEditionMode, (int) this.FieldEditionMode));
		}

		public static TileFieldDisplaySettings Restore(XElement xml)
		{
			var fieldVisibilityMode = (int) xml.Attribute (Xml.FieldVisibilityMode);
			var fieldEditionMode    = (int) xml.Attribute (Xml.FieldEditionMode);
							
			return new TileFieldDisplaySettings ((TileVisibilityMode) fieldVisibilityMode, (TileEditionMode) fieldEditionMode);
		}


		public static TileFieldDisplaySettings operator+(TileFieldDisplaySettings a, TileFieldDisplaySettings b)
		{
			return new TileFieldDisplaySettings (a.FieldVisibilityMode | b.FieldVisibilityMode, a.FieldEditionMode | b.FieldEditionMode);
		}

		public static TileFieldDisplaySettings operator-(TileFieldDisplaySettings a, TileFieldDisplaySettings b)
		{
			return new TileFieldDisplaySettings (a.FieldVisibilityMode & ~b.FieldVisibilityMode, a.FieldEditionMode & ~b.FieldEditionMode);
		}


		public override bool Equals(object obj)
		{
			if (obj is TileFieldDisplaySettings)
			{
				return this.Equals ((TileFieldDisplaySettings) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.visibility.GetHashCode () ^ this.edition.GetHashCode ();
		}

		#region IEquatable<FieldDisplaySettings> Members

		public bool Equals(TileFieldDisplaySettings other)
		{
			return this.visibility.Simplify () == other.visibility.Simplify ()
								&& this.edition.Simplify ()    == other.edition.Simplify ();
		}

		#endregion

		private static class Xml
		{
			public const string FieldVisibilityMode	= "v";
			public const string FieldEditionMode	= "e";
		}


		private readonly TileVisibilityMode visibility;
		private readonly TileEditionMode edition;
	}
}
