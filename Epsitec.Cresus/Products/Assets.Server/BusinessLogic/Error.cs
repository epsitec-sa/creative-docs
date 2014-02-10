//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public struct Error
	{
		public Error(ErrorType type, Guid objectGuid, int counter = 0)
		{
			this.Type       = type;
			this.ObjectGuid = objectGuid;
			this.Counter    = counter;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Type == ErrorType.Ok
					&& this.ObjectGuid.IsEmpty;
			}
		}

		public bool IsMessage
		{
			get
			{
				return this.Type == ErrorType.Ok
					|| this.Type == ErrorType.AmortizationGenerate
					|| this.Type == ErrorType.AmortizationRemove;
			}
		}

		public static readonly Error Empty = new Error (ErrorType.Ok, Guid.Empty);

		public readonly ErrorType				Type;
		public readonly Guid					ObjectGuid;
		public readonly int						Counter;
	}
}
