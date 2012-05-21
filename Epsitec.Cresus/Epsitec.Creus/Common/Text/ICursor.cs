//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
