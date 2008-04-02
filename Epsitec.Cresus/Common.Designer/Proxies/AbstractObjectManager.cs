using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Proxies
{
	/// <summary>
	/// Cette classe gère les objets associés à un proxy.
	/// </summary>
	public abstract class AbstractObjectManager
	{
		public AbstractObjectManager(object objectModifier)
		{
			this.objectModifier = objectModifier;
		}

		public virtual List<AbstractValue> GetValues(Widget selectedObject)
		{
			//	Retourne la liste des valeurs nécessaires pour représenter un objet.
			return null;
		}


		protected bool IsSuspended
		{
			get
			{
				return this.suspendChanges != 0;
			}
		}

		protected bool IsNotSuspended
		{
			get
			{
				return this.suspendChanges == 0;
			}
		}

		protected void SuspendChanges()
		{
			//	Suspend les changements jusqu'au prochain ResumeChanges.
			this.suspendChanges++;
		}

		protected void ResumeChanges()
		{
			//	Reprend les changements après un SuspendChanges.
			this.suspendChanges--;
			System.Diagnostics.Debug.Assert(this.suspendChanges >= 0);
		}


		protected object objectModifier;
		protected int suspendChanges;
	}
}
