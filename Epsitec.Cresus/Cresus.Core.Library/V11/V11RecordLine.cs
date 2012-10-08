//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.V11
{
	public class V11RecordLine : V11AbstractLine
	{
		public V11RecordLine(V11LineType type)
			: base (type)
		{
		}


		public V11LineCodeTransaction CodeTransaction
		{
			get;
			set;
		}

		public V11LineBVRTransaction BVRTransaction
		{
			get;
			set;
		}

		public V11LineOrigine Origine
		{
			get;
			set;
		}

		public string FormatedNoRéférence
		{
			//	Retourne le numéro au format "96 13070 01000 02173 50356 73892".
			get
			{
				string no = this.NoRéférence;

				if (!string.IsNullOrEmpty (no) && no.Length == 27)
				{
					string s1 = no.Substring (0, 2);
					string s2 = no.Substring (2, 5);
					string s3 = no.Substring (7, 5);
					string s4 = no.Substring (12, 5);
					string s5 = no.Substring (17, 5);
					string s6 = no.Substring (22, 5);

					return string.Concat (s1, " ", s2, " ", s3, " ", s4, " ", s5, " ", s6);
				}

				return no;
			}
		}

		public bool CheckNoRéférence
		{
			get
			{
				string no = this.NoRéférence;

				if (!string.IsNullOrEmpty (no) && no.Length == 27)
				{
					for (int i = 0; i < no.Length; i++)
					{
						char c = no[i];

						if (c < '0' || c > '9')
						{
							return false;
						}
					}

					return true;
				}

				return false;
			}
		}

		public string NoRéférence
		{
			//	Numéro de référence à 27 chiffres.
			get;
			set;
		}

		public string RéfDépot
		{
			get;
			set;
		}

		public Date? DateDépot
		{
			get;
			set;
		}

		public Date? DateTraitement
		{
			get;
			set;
		}

		public Date? DateCrédit
		{
			get;
			set;
		}

		public string NoMicrofilm
		{
			get;
			set;
		}

		public V11LineCodeRejet CodeRejet
		{
			get;
			set;
		}


		public override bool IsValid
		{
			get
			{
				if (!base.IsValid)
				{
					return false;
				}

				return
					this.CodeTransaction  != V11LineCodeTransaction.Unknown &&
					this.BVRTransaction   != V11LineBVRTransaction.Unknown &&
					this.Origine          != V11LineOrigine.Unknown &&
					!string.IsNullOrEmpty (this.NoRéférence) && 
					this.DateDépot        != null &&
					this.DateTraitement   != null &&
					this.DateCrédit       != null &&
					this.CodeRejet        != V11LineCodeRejet.Unknown;
			}
		}
	}
}
