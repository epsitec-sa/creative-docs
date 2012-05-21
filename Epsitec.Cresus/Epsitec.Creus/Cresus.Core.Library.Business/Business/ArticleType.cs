//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Business
{
	[DesignerVisible]
	public enum ArticleType
	{
		None		 = 0,

		Goods		 = 10,	// marchandises
		Service		 = 20,  // service
		Subscription = 30,  // abonnement
		Charge		 = 40,  // charge ?

		Freight		 = 50,	// fret (port et emballage)
		Tax			 = 60,  // taxe
		Admin		 = 70,	// frais administratif
	}
}