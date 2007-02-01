//	Copyright � 2005-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe TextBoxProperty permet de r�gler les d�tails relatifs �
	/// l'encadrement du texte.
	/// </summary>
	public class TextBoxProperty : AbstractXlineProperty
	{
		public TextBoxProperty()
		{
		}
		
		public TextBoxProperty(double position, SizeUnits position_units, double thickness, SizeUnits thickness_units, string draw_class, string draw_style) : base (position, position_units, thickness, thickness_units, draw_class, draw_style)
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
