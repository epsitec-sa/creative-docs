namespace Epsitec.Common.Widgets
{
	public enum ButtonStyle
	{
		Flat,							//	pas de cadre, ni de relief
		Normal,							//	bouton normal
		DefaultActive,					//	bouton pour l'action par défaut (OK)
		
		ToolItem,						//	bouton pour barre d'outils
		MenuItem						//	bouton pour ligne de menu
	}
	
	/// <summary>
	/// La class Button représente un bouton standard.
	/// </summary>
	public class Button : AbstractButton
	{
		public Button()
		{
			this.button_style = ButtonStyle.Normal;
		}
		
		public ButtonStyle				ButtonStyle
		{
			get { return this.button_style; }
			set
			{
				if (this.button_style != value)
				{
					this.button_style = value;
					this.Invalidate ();
				}
			}
		}
		


		protected ButtonStyle			button_style;
	}
}
