//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe TextMarkerProperty permet de régler les détails relatifs à
	/// la mise en évidence (au marquer "Stabylo") du texte.
	/// </summary>
	public class TextMarkerProperty : AbstractXlineProperty
	{
		public TextMarkerProperty()
		{
		}
		
		public TextMarkerProperty(double position, SizeUnits position_units, double thickness, SizeUnits thickness_units, string draw_class, string draw_style) : base (position, position_units, thickness, thickness_units, draw_class, draw_style)
		{
		}
		
		
		public static TextMarkerProperty		DisableOverride
		{
			get
			{
				TextMarkerProperty property = new TextMarkerProperty ();
				
				property.Disable ();
				
				return property;
			}
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.TextMarker;
			}
		}
		
		public override Property EmptyClone()
		{
			return new TextMarkerProperty ();
		}
	}
}
