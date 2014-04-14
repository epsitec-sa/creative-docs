//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Settings
{
	/// <summary>
	/// C'est ici que sont concentrés tous les réglages liés à l'utilisateur et à la UI.
	/// </summary>
	public static class LocalSettings
	{
		public static System.DateTime CreateMandatDate     = new System.DateTime (System.DateTime.Now.Year, 1, 1);
		public static System.DateTime CreateAssetDate      = new System.DateTime (System.DateTime.Now.Year, 1, 1);

		public static System.DateTime AmortizationDateFrom = new System.DateTime (System.DateTime.Now.Year, 1, 1);
		public static System.DateTime AmortizationDateTo   = new System.DateTime (System.DateTime.Now.Year, 12, 31);

		public static System.DateTime LockedDate           = Timestamp.Now.Date;
	}
}
