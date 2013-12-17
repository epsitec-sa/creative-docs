//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Labels
{
	public abstract class LabelTextFactory<T> : LabelTextFactory
		where T : AbstractEntity, new ()
	{
		public override FormattedText GetLabelText(AbstractEntity entity)
		{
			return this.GetLabelText ((T) entity);
		}

		public abstract FormattedText GetLabelText(T entity);
	}
}
