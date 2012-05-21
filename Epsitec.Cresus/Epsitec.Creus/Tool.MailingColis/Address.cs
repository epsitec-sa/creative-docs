//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Tool.MailingColis
{
	public sealed class Address
	{
		public string Firme;
		public string Titre;
		public string Nom;
		public string Prénom;
		public string Adresse;
		public string NPA;
		public string Localité;


		public string GetStreet()
		{
			return this.Adresse.Split ('\n').Where (x => !x.ContainsAnyWords ("cp", "case postale", "postfach")).LastOrDefault ();
		}

		public string GetName1()
		{
			return this.GetNames ().FirstOrDefault ();
		}

		public string GetName2()
		{
			return this.GetNames ().Skip (1).FirstOrDefault ();
		}

		public string GetPOBox()
		{
			return this.Adresse.Split ('\n').Where (x => x.ContainsAnyWords ("cp", "case postale", "postfach")).FirstOrDefault ();
		}

		private IEnumerable<string> GetNames()
		{
			string name = "";

			name = string.Concat (name.Trim (), " ", this.Prénom.Trim ());
			name = string.Concat (name.Trim (), " ", this.Nom.Trim ());
			name = name.Trim ();

			if (name.Length > 0)
			{
				yield return name;
			}
			foreach (string firm in this.Firme.Split ('\n').Select (x => x.Trim ()))
			{
				if (string.IsNullOrEmpty (firm) == false)
				{
					yield return firm;
				}
			}
		}
	}
}
