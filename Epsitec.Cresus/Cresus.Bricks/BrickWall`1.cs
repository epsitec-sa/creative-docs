//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public class BrickWall<T> : BrickWall
		where T : AbstractEntity, new()
	{
		public SimpleBrick<T> AddBrick()
		{
			return this.AddSimpleBrick<T> (null);
		}

		public SimpleBrick<TField> AddBrick<TField>(Expression<System.Func<T, TField>> expression)
			where TField : AbstractEntity, new ()
		{
			return this.AddSimpleBrick<TField> (expression);
		}

		public SimpleBrick<TField> AddBrick<TField>(Expression<System.Func<T, IList<TField>>> expression)
			where TField : AbstractEntity, new ()
		{
			return this.AddSimpleBrick<TField> (expression);
		}

		private SimpleBrick<TBrick> AddSimpleBrick<TBrick>(Expression expression)
			where TBrick : AbstractEntity, new ()
		{
			var brick = new SimpleBrick<TBrick> (this, expression);

			this.Add (brick);
			this.NotifyBrickAdded (typeof (TBrick), brick);

			return brick;
		}
	}
}
