//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>ActionInfo</c> class represents a method which can be called to perform
	/// an action. It has an associated caption.
	/// </summary>
	public sealed class ActionInfo
	{
		internal ActionInfo(ActionAttribute attribute, System.Action<AbstractEntity> action)
		{
			this.attribute = attribute;
			this.action    = action;
		}


		public ActionClasses					ActionClass
		{
			get
			{
				return this.attribute.ActionClass;
			}
		}

		public Druid							CaptionId
		{
			get
			{
				return this.attribute.CaptionId;
			}
		}

		public double							Weight
		{
			get
			{
				return this.attribute.Weight;
			}
		}


		public void ExecuteAction(AbstractEntity entity)
		{
			if (this.action == null)
			{
				throw new System.ArgumentNullException ("action");
			}
			else
			{
				this.action (entity);
			}
		}


		private readonly ActionAttribute				attribute;
		private readonly System.Action<AbstractEntity>	action;
	}
}
