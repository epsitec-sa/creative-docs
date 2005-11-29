//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe OverlineProperty permet de régler les détails relatifs au
	/// surlignement du texte.
	/// </summary>
	public class OverlineProperty : AbstractXlineProperty
	{
		public OverlineProperty()
		{
		}
		
		public OverlineProperty(double position, SizeUnits position_units, double thickness, SizeUnits thickness_units, string line_class, string line_style) : base (position, position_units, thickness, thickness_units, line_class, line_style)
		{
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
