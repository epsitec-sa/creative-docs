using System.Collections.Generic;

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
	/// <summary>
	/// Décrit un champ dans un masque de saisie.
	/// </summary>
	public class FieldDescription
	{
		public FieldDescription()
		{
			//	Constructeur.
			this.fieldIds = new List<Druid>();
		}

		public List<Druid> FieldsIds
		{
			get
			{
				return this.fieldIds;
			}
		}

		public bool Separator
		{
			get
			{
				return this.separator;
			}
			set
			{
				this.separator = value;
			}
		}


		protected List<Druid> fieldIds;
		protected bool separator;
	}
}
