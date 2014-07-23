//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.App.Views
{
	public struct ObjectSummaryControllerTile
	{
		public ObjectSummaryControllerTile(string text)
		{
			//	Texte fixe.
			this.Text  = text;
			this.Field = ObjectField.Unknown;
		}

		public ObjectSummaryControllerTile(ObjectField field)
		{
			//	Contenu d'un champ.
			this.Text  = null;
			this.Field = field;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Text == null
					&& this.Field == ObjectField.Unknown;
			}
		}

		public static ObjectSummaryControllerTile Empty = new ObjectSummaryControllerTile ();

		public readonly string				Text;
		public readonly ObjectField			Field;
	}
}
