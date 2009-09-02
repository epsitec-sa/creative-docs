//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		public UnderlineProperty(double position, SizeUnits positionUnits, double thickness, SizeUnits thicknessUnits, string drawClass, string drawStyle) : base (position, positionUnits, thickness, thicknessUnits, drawClass, drawStyle)
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
