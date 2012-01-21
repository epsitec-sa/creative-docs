//	Copyright � 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.IO
{
	/// <summary>
	/// Cette classe s'occupe de cr�er une nouvelle comptabilit� de toutes pi�ces.
	/// </summary>
	public class NewComptabilit�
	{
		public void NewEmpty(Comptabilit�Entity comptabilit�)
		{
			var now = Date.Today;
			var beginDate = new Date(now.Year,  1,  1);  // 1 janvier
			var endDate   = new Date(now.Year, 12, 31);  // 31 d�cembre

			comptabilit�.Name          = "vide";
			comptabilit�.Description   = null;
			comptabilit�.BeginDate     = beginDate;
			comptabilit�.EndDate       = endDate;
			comptabilit�.Derni�reDate  = beginDate;
			comptabilit�.Derni�rePi�ce = "0";

			comptabilit�.Journal.Clear ();
			comptabilit�.PlanComptable.Clear ();
		}

		public void NewModel(Comptabilit�Entity comptabilit�)
		{
			// TODO...
			this.NewEmpty (comptabilit�);
		}
	}
}
