//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Print2
{
	public static class PrintEngine
	{
		static PrintEngine()
		{
			PrintEngine.RegisterFonts ();
		}

		public static void Setup()
		{
			//	Force execution of static constructor.
		}


		public static bool CanPrint(AbstractEntity entity)
		{
			return true;
		}


		public static string Print(CoreData coreData, IEnumerable<AbstractEntity> collection, OptionsDictionary options, PrintingUnitsDictionary printingUnits)
		{
			//	Imprime un document et retourne le source xml correspondant, sans aucune interaction.
			System.Diagnostics.Debug.Assert (coreData != null);
			System.Diagnostics.Debug.Assert (collection != null);
			System.Diagnostics.Debug.Assert (options != null);
			System.Diagnostics.Debug.Assert (printingUnits != null);

			return null;
		}


		private static void RegisterFonts()
		{
			using (var stream = System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream ("Epsitec.Creus.Core.Resources.OCR_BB.tff"))
			{
				Font.RegisterDynamicFont (stream);
			}
		}
	}
}
