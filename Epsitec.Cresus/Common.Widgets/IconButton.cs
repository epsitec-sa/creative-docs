namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe IconButton permet de dessiner de petits pictogrammes.
	/// </summary>
	public class IconButton : Button
	{
		public IconButton()
		{
			this.ButtonStyle = ButtonStyle.Flat;
		}
		
		public string					IconName
		{
			get { return this.icon_name; }
			set
			{
				if (this.icon_name != value)
				{
					this.icon_name = value;
					this.Invalidate ();
				}
			}
		}
		
		
		protected string				icon_name;
	}
}
