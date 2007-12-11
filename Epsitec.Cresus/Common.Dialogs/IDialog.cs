//	Copyright © 2004-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// L'interface IDialog est commune à tous les dialogues.
	/// </summary>
	public interface IDialog
	{
		void OpenDialog();
		
		Window			Owner	{ get; set; }
		DialogResult	Result	{ get; }
	}
}
