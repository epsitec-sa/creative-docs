//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe AutoText gère les textes automatiques (texte riches avec des
	/// propriétés), tels que les listes à puces.
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
