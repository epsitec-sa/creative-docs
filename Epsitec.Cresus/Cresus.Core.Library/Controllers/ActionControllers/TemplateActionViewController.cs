//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Factories;

using System;

namespace Epsitec.Cresus.Core.Controllers.ActionControllers
{
	public abstract class TemplateActionViewController<T, TAdditionalEntity> : ActionViewController<T>, ITemplateActionViewController
		where T : AbstractEntity, new ()
		where TAdditionalEntity : AbstractEntity, new ()
	{
		protected TemplateActionViewController()
		{
			this.additionalEntity = (TAdditionalEntity) EntityViewControllerFactory.Default.AdditionalEntity;
		}

		#region ITemplateActionViewController Members

		public virtual bool						RequiresAdditionalEntity
		{
			get
			{
				return true;
			}
		}

		#endregion

		
		protected TAdditionalEntity				AdditionalEntity
		{
			get
			{
				return this.additionalEntity;
			}
		}
		
		private readonly TAdditionalEntity		additionalEntity;

	}
}