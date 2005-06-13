//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe AutoText g�re les textes automatiques (texte riches avec des
	/// propri�t�s), tels que les listes � puces.
	/// </summary>
	public sealed class AutoText
	{
		public AutoText(string name)
		{
			this.name = name;
		}
		
		
		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		
		
		private string							name;
	}
}
