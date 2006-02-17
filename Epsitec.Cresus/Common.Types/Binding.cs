//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class Binding
	{
		public Binding()
		{
		}
		
		public BindingMode						Mode
		{
			get
			{
				return this.mode;
			}
			set
			{
				this.mode = value;
			}
		}
		public object							Source
		{
			get
			{
				return this.source;
			}
			set
			{
				this.source = value;
			}
		}
		public PropertyPath						Path
		{
			get
			{
				return this.path;
			}
			set
			{
				this.path = value;
			}
		}
		
		public static readonly object			DoNothing = new object ();

		private BindingMode						mode;
		private object							source;
		private PropertyPath					path;
	}
}
