﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public enum ErrorType
	{
		Ok,
		Unknown,

		AmortissementGenerate,
		AmortissementRemove,

		AmortissementAlreadyDone,
		AmortissementUndefined,
		AmortissementInvalidRate,
		AmortissementInvalidType,
		AmortissementInvalidPeriod,
		AmortissementEmptyAmount,
		AmortissementOutObject,
	}
}