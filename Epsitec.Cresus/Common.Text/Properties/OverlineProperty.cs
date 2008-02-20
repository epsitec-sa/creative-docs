//	Copyright � 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe OverlineProperty permet de r�gler les d�tails relatifs au
	/// surlignement du texte.
	/// </summary>
	public class OverlineProperty : AbstractXlineProperty
	{
		public OverlineProperty()
		{
		}
		
		public OverlineProperty(double position, SizeUnits position_units, double thickness, SizeUnits thickness_units, string draw_class, string draw_style) : base (position, position_units, thickness, thickness_units, draw_class, draw_style)
		{
		}
		
		
		public static OverlineProperty			DisableOverride
		{
			get
			{
				OverlineProperty property = new OverlineProperty ();
				
				property.Disable ();
				
				return property;
			}
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.Overline;
			}
		}
		
		public override Property EmptyClone()
		{
			return new OverlineProperty ();
		}
	}
}
