//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.UI;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>DialogFocusEventArgs</c> class contains information about a focus
	/// change related to dialog data fields.
	/// </summary>
	public class DialogFocusEventArgs : System.EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DialogFocusEventArgs"/> class.
		/// </summary>
		/// <param name="oldPath">The old path.</param>
		/// <param name="newPath">The new path.</param>
		public DialogFocusEventArgs(EntityFieldPath oldPath, EntityFieldPath newPath)
		{
			this.oldPath = oldPath;
			this.newPath = newPath;
		}


		/// <summary>
		/// Gets the old path.
		/// </summary>
		/// <value>The old path.</value>
		public EntityFieldPath OldPath
		{
			get
			{
				return this.oldPath;
			}
		}

		/// <summary>
		/// Gets the new path.
		/// </summary>
		/// <value>The new path.</value>
		public EntityFieldPath NewPath
		{
			get
			{
				return this.newPath;
			}
		}


		public override string ToString()
		{
			return string.Format ("{0} -> {1}", oldPath == null ? "<null>" : oldPath.ToString (), newPath == null ? "<null>" : newPath.ToString ());
		}


		private readonly EntityFieldPath		oldPath;
		private readonly EntityFieldPath		newPath;
	}
}
