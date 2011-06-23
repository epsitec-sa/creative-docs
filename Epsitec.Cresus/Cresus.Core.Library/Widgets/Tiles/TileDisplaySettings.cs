//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

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
			this.FieldVisibilityMode = other.FieldVisibilityMode;
			this.FieldEditionMode    = other.FieldEditionMode;
		}


		public TileVisibilityMode				FieldVisibilityMode
		{
			get;
			set;
		}

		public TileEditionMode					FieldEditionMode
		{
			get;
			set;
		}

		public Druid							FieldId
		{
			get;
			set;
		}


		public XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
				new XAttribute (Xml.FieldVisibilityMode, (int) this.FieldVisibilityMode),
				new XAttribute (Xml.FieldEditionMode, (int) this.FieldEditionMode),
				new XAttribute (Xml.FieldId, this.FieldId.ToString ()));
		}

		public static TileDisplaySettings Restore(XElement xml)
		{
			var fieldVisibilityMode = (int) xml.Attribute (Xml.FieldVisibilityMode);
			var fieldEditionMode    = (int) xml.Attribute (Xml.FieldEditionMode);
			var fieldId             = (string) xml.Attribute (Xml.FieldId);
			
			return new TileDisplaySettings ()
			{
				FieldVisibilityMode = (TileVisibilityMode) fieldVisibilityMode,
				FieldEditionMode    = (TileEditionMode) fieldEditionMode,
				FieldId             = Druid.Parse (fieldId),
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
					FieldVisibilityMode = a.FieldVisibilityMode | b.FieldVisibilityMode,
					FieldEditionMode    = a.FieldEditionMode    | b.FieldEditionMode,
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
				return this.FieldVisibilityMode.Simplify () == other.FieldVisibilityMode.Simplify ()
					&& this.FieldEditionMode.Simplify ()    == other.FieldEditionMode.Simplify ();
			}
		}

		#endregion
		
		private static class Xml
		{
			public const string FieldId				= "id";
			public const string FieldVisibilityMode	= "vis";
			public const string FieldEditionMode	= "ed";
		}
	}
}
