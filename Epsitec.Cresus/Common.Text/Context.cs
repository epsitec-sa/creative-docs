//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe Context d�crit un contexte (pour la d�s�rialisation) li� �
	/// un environnement 'texte'.
	/// </summary>
	public class Context
	{
		public Context()
		{
			this.style_list = new StyleList ();
		}
		
		
		public StyleList						StyleList
		{
			get
			{
				return this.style_list;
			}
		}
		
		
		private StyleList						style_list;
	}
}
