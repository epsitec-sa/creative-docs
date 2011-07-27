//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Core.Library.Business.ContentAccessors
{
	public enum DocumentItemAccessorError
	{
		OK,
		Unknown,
		TextEmpty,
		ArticleNotDefined,
		AdditionalQuantitiesTooHigh,
	}
}
