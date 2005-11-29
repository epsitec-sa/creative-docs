//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe UnderlineProperty permet de régler les détails relatifs au
	/// soulignement du texte.
	/// </summary>
	public class UnderlineProperty : AbstractXlineProperty
	{
		public UnderlineProperty()
		{
		}
		
		public UnderlineProperty(double position, SizeUnits position_units, double thickness, SizeUnits thickness_units, string draw_class, string draw_style) : base (position, position_units, thickness, thickness_units, draw_class, draw_style)
		{
		}
		
		
		public static UnderlineProperty			DisableOverride
		{
			get
			{
				UnderlineProperty property = new UnderlineProperty ();
				
				property.Disable ();
				
				return property;
			}
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.Underline;
			}
		}
		
		public override Property EmptyClone()
		{
			return new UnderlineProperty ();
		}
	}
}
