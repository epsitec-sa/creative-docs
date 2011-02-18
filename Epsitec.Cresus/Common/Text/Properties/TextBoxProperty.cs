//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe TextBoxProperty permet de régler les détails relatifs à
	/// l'encadrement du texte.
	/// </summary>
	public class TextBoxProperty : AbstractXlineProperty
	{
		public TextBoxProperty()
		{
		}
		
		public TextBoxProperty(double position, SizeUnits positionUnits, double thickness, SizeUnits thicknessUnits, string drawClass, string drawStyle) : base (position, positionUnits, thickness, thicknessUnits, drawClass, drawStyle)
		{
		}
		
		
		public static TextBoxProperty			DisableOverride
		{
			get
			{
				TextBoxProperty property = new TextBoxProperty ();
				
				property.Disable ();
				
				return property;
			}
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.TextBox;
			}
		}
		
		public override Property EmptyClone()
		{
			return new TextBoxProperty ();
		}
	}
}
