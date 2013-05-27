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
