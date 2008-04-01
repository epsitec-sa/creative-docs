using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Proxies
{
	/// <summary>
	/// Cette classe permet...
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
				return this.suspendChanges > 0;
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
			this.suspendChanges++;
		}

		protected void ResumeChanges()
		{
			this.suspendChanges--;
		}


		protected object objectModifier;
		protected int suspendChanges;
	}
}
