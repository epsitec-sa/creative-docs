using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;

namespace Epsitec.Cresus.Strings
{
	internal class EditResourceSmartTag : SmartTag
	{
		public EditResourceSmartTag(ReadOnlyCollection<SmartTagActionSet> actionSets) :
			base (SmartTagType.Factoid, actionSets)
		{
		}
	}
}
