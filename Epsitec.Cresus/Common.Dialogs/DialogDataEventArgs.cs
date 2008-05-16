//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.UI;

namespace Epsitec.Common.Dialogs
{
	public class DialogDataEventArgs : System.EventArgs
	{
		public DialogDataEventArgs(EntityFieldPath path, AbstractPlaceholder placeholder, object oldValue, object newValue)
		{
			this.path = path;
			this.placeholder = placeholder;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}


		public EntityFieldPath Path
		{
			get
			{
				return this.path;
			}
		}

		public object OldValue
		{
			get
			{
				return this.oldValue;
			}
		}

		public object NewValue
		{
			get
			{
				return this.newValue;
			}
		}


		public override string ToString()
		{
			return string.Format ("{0} : {1} -> {2}", path.ToString (), oldValue ?? "<null>", newValue ?? "<null>");
		}


		private readonly EntityFieldPath path;
		private readonly AbstractPlaceholder placeholder;
		private readonly object oldValue;
		private readonly object newValue;
	}
}
