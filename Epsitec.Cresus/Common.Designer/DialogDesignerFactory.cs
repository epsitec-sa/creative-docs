//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer
{
	/// <summary>
	///	La classe DialogDesignerFactory permet d'instancier un DialogDesigner qui
	/// sert d'intermédiaire entre un dialogue et le designer.
	/// </summary>
	public class DialogDesignerFactory : Epsitec.Common.Dialogs.IDialogDesignerFactory
	{
		private DialogDesignerFactory()
		{
			this.application = new Application ();
		}
		
		
		#region IDialogDesignerFactory Members
		public Epsitec.Common.Dialogs.IDialogDesigner CreateDialogDesigner(Common.UI.InterfaceType type)
		{
			return new DialogDesigner (this.application, type);
		}
		#endregion
		
		public static Epsitec.Common.Dialogs.IDialogDesignerFactory GetFactory()
		{
			if (DialogDesignerFactory.factory == null)
			{
				DialogDesignerFactory.factory = new DialogDesignerFactory ();
			}
			
			return DialogDesignerFactory.factory;
		}
		
		
		private static DialogDesignerFactory	factory;
		
		private Application						application;
	}
}
