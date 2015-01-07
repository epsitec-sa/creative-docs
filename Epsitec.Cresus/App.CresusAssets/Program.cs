//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[System.STAThread]
		static void Main(string[] args)
		{
			//?Epsitec.Common.Support.Resources.OverrideDefaultTwoLetterISOLanguageName ("en");
			Epsitec.Cresus.Core.CoreProgram.Main (args);
		}
	}
}
