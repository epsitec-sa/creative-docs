//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	public sealed class DialogCentral : CoreAppComponent
	{
		private DialogCentral(CoreApp app)
			: base (app)
		{
			this.attachedDialogs = new List<Dialogs.IAttachedDialog> ();
		}
		
		public IList<Dialogs.IAttachedDialog> AttachedDialogs
		{
			get
			{
				return this.attachedDialogs.AsReadOnly ();
			}
		}

		public void AttachDialog(Dialogs.IAttachedDialog dialog)
		{
			if (!this.attachedDialogs.Contains (dialog))
			{
				this.attachedDialogs.Add (dialog);
			}
		}

		public void DetachDialog(Dialogs.IAttachedDialog dialog)
		{
			if (this.attachedDialogs.Contains (dialog))
			{
				this.attachedDialogs.Remove (dialog);
			}
		}

		private readonly List<Dialogs.IAttachedDialog>	attachedDialogs;

		#region Factory Class

		private sealed class Factory : Epsitec.Cresus.Core.Factories.DefaultCoreAppComponentFactory<DialogCentral>
		{
		}

		#endregion
	}
}
