using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Common.Data
{
	/// <summary>
	/// ATTENTION: L'aspect multimonnaie n'a pas été réfléchi en profondeur !
	/// </summary>
	public class Monnaie : AbstractObjetComptable
	{
		public FormattedText	CodeISO;
		public FormattedText	Description;
		public FormattedText	Commentaire;  // utile ?
		public int				Décimales;
		public decimal			Arrondi;
		public decimal			Cours;
		public int				Unité;  // ?
		public bool				Externe;  // ?
		public Compte			CompteGain;
		public Compte			ComptePerte;
	}
}
