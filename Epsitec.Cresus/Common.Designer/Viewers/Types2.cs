using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public class Types2 : AbstractCaptions2
	{
		public Types2(Module module, PanelsContext context, ResourceAccess access, MainWindow mainWindow)
			: base (module, context, access, mainWindow)
		{
			this.table.ColumnHeader.SetColumnComparer(1, Types2.CompareTypeColumns);
			this.UpdateAll ();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}

		private static int CompareTypeColumns(object a, object b)
		{
			CultureMap itemA = a as CultureMap;
			CultureMap itemB = b as CultureMap;

			StructuredData dataA = itemA.GetCultureData(Common.Support.Resources.DefaultTwoLetterISOLanguageName);
			StructuredData dataB = itemB.GetCultureData(Common.Support.Resources.DefaultTwoLetterISOLanguageName);

			string codeA = dataA.GetValue(Support.Res.Fields.ResourceBaseType.TypeCode).ToString();
			string codeB = dataB.GetValue(Support.Res.Fields.ResourceBaseType.TypeCode).ToString();

			return codeA.CompareTo(codeB);
		}

		public override ResourceAccess.Type ResourceType
		{
			get
			{
				return ResourceAccess.Type.Types2;
			}
		}
	}
}
