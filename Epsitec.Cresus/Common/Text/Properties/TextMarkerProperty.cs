//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		public TextMarkerProperty(double position, SizeUnits positionUnits, double thickness, SizeUnits thicknessUnits, string drawClass, string drawStyle) : base (position, positionUnits, thickness, thicknessUnits, drawClass, drawStyle)
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
