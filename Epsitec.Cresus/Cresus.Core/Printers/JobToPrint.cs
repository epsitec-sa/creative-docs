//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Printers
{
	/// <summary>
	/// Une instance de JobToPrint représente plusieurs sections (SectionToPrint) à imprimer sur une
	/// même imprimante physique, mais éventuellement sur plusieurs bacs.
	/// </summary>
	public class JobToPrint
	{
		public JobToPrint()
		{
			this.sections = new List<SectionToPrint> ();
		}

		public List<SectionToPrint> Sections
		{
			get
			{
				return this.sections;
			}
		}

		public string PrinterPhysicalName
		{
			//	Retourne le nom de l'imprimante physique de la première section.
			//	Toutes les sections doivent utiliser la même imprimante physique.
			//	Printer peut changer, s'il s'agit d'un autre bac, mais Printer.PhysicalName
			//	est identique dans toutes les sections.
			get
			{
				if (this.sections.Count == 0)
				{
					return null;
				}
				else
				{
					return this.sections[0].Printer.PhysicalName;
				}
			}
		}

		private List<SectionToPrint> sections;
	}
}
