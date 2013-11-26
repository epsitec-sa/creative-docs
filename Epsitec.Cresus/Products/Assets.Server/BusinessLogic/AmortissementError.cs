//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public struct AmortissementError
	{
		public AmortissementError(AmortissementErrorType errorType, Guid objectGuid, int counter = 0)
		{
			this.ErrorType  = errorType;
			this.ObjectGuid = objectGuid;
			this.Counter    = counter;
		}

		public bool IsEmpty
		{
			get
			{
				return this.ErrorType == AmortissementErrorType.Ok
					&& this.ObjectGuid.IsEmpty;
			}
		}

		public static readonly AmortissementError Empty = new AmortissementError (AmortissementErrorType.Ok, Guid.Empty);

		public readonly AmortissementErrorType	ErrorType;
		public readonly Guid					ObjectGuid;
		public readonly int						Counter;
	}
}
