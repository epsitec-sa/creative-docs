//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
