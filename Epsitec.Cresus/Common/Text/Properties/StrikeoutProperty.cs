//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		public StrikeoutProperty(double position, SizeUnits positionUnits, double thickness, SizeUnits thicknessUnits, string drawClass, string drawStyle) : base (position, positionUnits, thickness, thicknessUnits, drawClass, drawStyle)
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
