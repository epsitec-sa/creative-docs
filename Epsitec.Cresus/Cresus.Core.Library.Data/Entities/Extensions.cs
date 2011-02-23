//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public static class Extensions
	{
		public static EntityStatus GetEntityStatus(this string text)
		{
			if (string.IsNullOrWhiteSpace (text))
			{
				return EntityStatus.Empty;
			}
			else
			{
				return EntityStatus.Valid;
			}
		}

		public static EntityStatus GetEntityStatus(this FormattedText text)
		{
			if (text.IsNullOrWhiteSpace)
			{
				return EntityStatus.Empty;
			}
			else
			{
				return EntityStatus.Valid;
			}
		}

		public static EntityStatus TreatAsOptional(this EntityStatus status)
		{
			if ((status & EntityStatus.Empty) != 0)
			{
				status |= EntityStatus.Valid;
			}

			return status;
		}
	}
}
