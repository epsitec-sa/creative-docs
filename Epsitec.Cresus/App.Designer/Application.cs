//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Designer;
using Epsitec.Common.UI.Data;

namespace Epsitec.Designer
{
	/// <summary>
	/// La classe Application démarre le designer.
	/// </summary>
	public class Application
	{
		#region Application Startup
		[System.STAThread] static void Main() 
		{
			Epsitec.Common.Widgets.Adorner.Factory.SetActive ("LookMetal");
			
			DialogDesigner designer = DialogDesignerFactory.GetFactory ().CreateDialogDesigner () as DialogDesigner;
			
			Record record = new Record ();
			
			Field field_a = new Field ("a", "");
			Field field_b = new Field ("b", 0);
			Field field_c = new Field ("c", Representation.None);
			
			field_a.DefineCaption ("Texte A");
			field_b.DefineCaption ("Valeur B");
			field_c.DefineCaption ("Enumération C");
			
			record.Add (field_a);
			record.Add (field_b);
			record.Add (field_c);
			
			designer.DialogData = record;
			
			designer.Application.MainWindow.Show ();
			designer.Application.MainWindow.Run ();
		}
		#endregion
	}
}
