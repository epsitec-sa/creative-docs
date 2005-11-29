//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe StrikeoutProperty permet de régler les détails relatifs au
	/// biffé du texte.
	/// </summary>
	public class StrikeoutProperty : AbstractXlineProperty
	{
		public StrikeoutProperty()
		{
		}
		
		public StrikeoutProperty(double position, SizeUnits position_units, double thickness, SizeUnits thickness_units, string draw_class, string draw_style) : base (position, position_units, thickness, thickness_units, draw_class, draw_style)
		{
		}
		
		
		public static StrikeoutProperty			DisableOverride
		{
			get
			{
				StrikeoutProperty property = new StrikeoutProperty ();
				
				property.Disable ();
				
				return property;
			}
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.Strikeout;
			}
		}
		
		public override Property EmptyClone()
		{
			return new StrikeoutProperty ();
		}
	}
}
