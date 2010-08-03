//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.BusinessLogic
{
	[DesignerVisible]
	public enum ArticleType
	{
		None		 = 0,

		Goods		 = 10,	// marchandises
		Service		 = 20,
		Subscription = 30,
		Charge		 = 40,

		Freight		 = 50,	// fret
		Tax			 = 60,
	}
}
