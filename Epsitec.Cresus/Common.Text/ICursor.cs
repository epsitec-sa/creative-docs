//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'interface ICursor permet de décrire un curseur.
	/// </summary>
	public interface ICursor
	{
		int					CursorId			{ get; set; }
		CursorAttachment	Attachment			{ get; }
		int					Direction			{ get; set; }
		
		void Clear();
	}
}
