using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Labels
{
	public abstract class LabelTextFactory
	{
		public abstract FormattedText GetLabelText(AbstractEntity entity);
	}
}
