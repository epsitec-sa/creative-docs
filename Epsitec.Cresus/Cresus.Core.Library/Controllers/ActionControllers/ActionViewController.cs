//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

namespace Epsitec.Cresus.Core.Controllers.ActionControllers
{
	public abstract class ActionViewController<T> : EntityViewController<T>, IActionViewController, IActionExecutorProvider
		where T : AbstractEntity, new ()
	{
		protected virtual bool					NeedsInteraction
		{
			get
			{
				return true;
			}
		}

		
		protected sealed override void CreateBricks(BrickWall<T> wall)
		{
			var action = wall
				.AddBrick ()
				.DefineAction ();

			if (this.NeedsInteraction)
			{
				this.GetForm (action);
			}
		}

		protected virtual void GetForm(ActionBrick<T, SimpleBrick<T>> action)
		{
		}

		#region IActionViewController Members

		public virtual bool						IsEnabled
		{
			get
			{
				return true;
			}
		}

		public virtual bool						ExecuteInQueue
		{
			get
			{
				return false;
			}
		}

		public abstract FormattedText GetTitle();

		#endregion

		#region IActionExecutorProvider Members

		public abstract ActionExecutor GetExecutor();

		#endregion
	}
}