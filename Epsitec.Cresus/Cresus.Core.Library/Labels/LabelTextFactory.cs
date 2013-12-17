//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Labels
{
	public abstract class LabelTextFactory
	{
		public abstract FormattedText GetLabelText(AbstractEntity entity);
	}
}
