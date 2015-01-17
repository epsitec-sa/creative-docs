using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Data
{
	/// <summary>
	/// Cette classe permet de peupler une liste "intelligente" de libellés accessibles
	/// par le menu "combo" du champ libellé.
	/// </summary>
	public class Libellé : AbstractObjetComptable
	{
		public FormattedText	Texte;
		public bool				Permanent;
	}
}
