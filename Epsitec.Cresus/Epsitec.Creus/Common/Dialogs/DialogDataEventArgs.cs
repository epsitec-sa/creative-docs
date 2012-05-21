//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.UI;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>DialogDataEventArgs</c> class contains information about a field
	/// value change in a dialog.
	/// </summary>
	public class DialogDataEventArgs : System.EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DialogDataEventArgs"/> class.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="placeholder">The placeholder.</param>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		public DialogDataEventArgs(EntityFieldPath path, AbstractPlaceholder placeholder, object oldValue, object newValue)
		{
			this.path = path;
			this.placeholder = placeholder;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}


		/// <summary>
		/// Gets the path of the edited value.
		/// </summary>
		/// <value>The path.</value>
		public EntityFieldPath Path
		{
			get
			{
				return this.path;
			}
		}

		/// <summary>
		/// Gets the old value.
		/// </summary>
		/// <value>The old value.</value>
		public object OldValue
		{
			get
			{
				return this.oldValue;
			}
		}

		/// <summary>
		/// Gets the new value.
		/// </summary>
		/// <value>The new value.</value>
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


		private readonly EntityFieldPath		path;
		private readonly AbstractPlaceholder	placeholder;
		private readonly object					oldValue;
		private readonly object					newValue;
	}
}
