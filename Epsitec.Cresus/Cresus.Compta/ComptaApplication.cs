//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta
{
	public class ComptaApplication : CoreInteractiveApp
	{
		public override string ShortWindowTitle
		{
			get
			{
				return "Crésus MCH-2";
			}
		}
		public override string ApplicationIdentifier
		{
			get
			{
				return "Cr.MCH-2";
			}
		}

		protected override void CreateManualComponents(IList<System.Action> initializers)
		{
		}

		protected override void InitializeEmptyDatabase()
		{
		}

		protected override System.Xml.Linq.XDocument LoadApplicationState()
		{
			return null;
		}

		protected override void SaveApplicationState(System.Xml.Linq.XDocument doc)
		{
		}
	}
}
