//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	public class BindingChangedEventArgs : System.EventArgs
	{
		public BindingChangedEventArgs(DependencyProperty property)
		{
			this.property = property;
		}
		
		public DependencyProperty				Property
		{
			get
			{
				return this.property;
			}
		}
		
		private DependencyProperty				property;
	}
}
