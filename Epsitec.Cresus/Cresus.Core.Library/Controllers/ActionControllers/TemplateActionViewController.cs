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

		protected TAdditionalEntity AdditionalEntity
		{
			get
			{
				return this.additionalEntity;
			}
		}
		
		private readonly TAdditionalEntity additionalEntity;

		#region ITemplateActionViewController Members

		public abstract bool RequiresAdditionalEntity();

		#endregion
	}
}