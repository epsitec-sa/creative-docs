//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.UI.Panels.EditPanel))]

namespace Epsitec.Common.UI.Panels
{
	internal class EditPanel : Panel
	{
		public EditPanel()
		{
		}
		
		public EditPanel(Panel owner)
		{
			this.owner = owner;
		}

		public override Panel Owner
		{
			get
			{
				return this.owner;
			}
		}

		public override Panel GetPanel(PanelMode mode)
		{
			return this.owner.GetPanel (mode);
		}

		public override PanelMode PanelMode
		{
			get
			{
				return PanelMode.Edition;
			}
		}

		private static object GetOwnerValue(DependencyObject obj)
		{
			EditPanel panel = (EditPanel) obj;
			return panel.Owner;
		}

		public static DependencyProperty OwnerProperty = DependencyProperty.RegisterReadOnly ("Owner", typeof (Panel), typeof (EditPanel), new DependencyPropertyMetadata (EditPanel.GetOwnerValue).MakeReadOnlySerializable ());
		
		private Panel owner;
	}
}
