namespace Epsitec.Common.Widgets
{
	public enum TextFieldStyle
	{
		Flat,							//	pas de cadre, ni de relief
		Normal,							//	ligne �ditable normale
	}
	
	/// <summary>
	/// La classe TextField impl�mente la ligne �ditable, tout en permettant
	/// aussi de r�aliser l'�quivalent de la ComboBox Windows.
	/// </summary>
	public class TextField : Widget
	{
		public TextField()
		{
		}
		
		public virtual double			LeftMargin
		{
			get { return this.left_margin; }
		}
		
		public virtual double			RightMargin
		{
			get { return this.right_margin; }
		}
		
		public TextFieldStyle			TextFieldStyle
		{
			get { return this.text_style; }
			set
			{
				if (this.text_style != value)
				{
					this.text_style = value;
					this.Invalidate ();
				}
			}
		}
		
		
		public static TextField CreateCombo()
		{
			TextField text_field = new TextField ();
			
			//	TODO: cr��e la ligne �ditable, r�gle la marge de droite et ajoute un
			//	widget de type bouton, plus le c�blage n�cessaire pour g�rer le menu.
			
			text_field.right_margin = 17;
			
			return text_field;
		}
		
		public static TextField CreateUpDown()
		{
			TextField text_field = new TextField ();
			
			//	TODO: cr��e la ligne �ditable, r�gle la marge de droite et ajoute les
			//	widgets de type bouton permettant de passer au suivant/pr�c�dent dans
			//	la liste.
			
			text_field.right_margin = 17;
			
			return text_field;
		}
		
		
		protected double				left_margin;
		protected double				right_margin;
		protected TextFieldStyle		text_style;
	}
}
