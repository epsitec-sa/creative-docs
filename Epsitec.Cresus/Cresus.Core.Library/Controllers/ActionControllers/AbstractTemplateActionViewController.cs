//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Factories;

using System;

namespace Epsitec.Cresus.Core.Controllers.ActionControllers
{
	public abstract class AbstractTemplateActionViewController<T> : ActionViewController<T>, ITemplateActionViewController
		where T : AbstractEntity, new ()
	{
		protected AbstractTemplateActionViewController()
		{
			this.additionalEntity = EntityViewControllerFactory.Default.AdditionalEntity;
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

		
		protected AbstractEntity AdditionalEntity
		{
			get
			{
				return this.additionalEntity;
			}
		}

		private readonly AbstractEntity		additionalEntity;

	}
}