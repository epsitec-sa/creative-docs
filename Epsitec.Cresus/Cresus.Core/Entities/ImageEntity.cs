//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business.Finance;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class ImageEntity
	{
		public override FormattedText GetSummary()
		{
			string name = this.Name.IsNullOrWhiteSpace ? "<i>Inconnu</i>" : this.Name.ToString ();

			if (this.ImageBlob.IsNull ())
			{
				return TextFormatter.FormatText
					(
						"Nom :  ", name
					);
			}
			else
			{
				var dpi = System.Math.Ceiling (this.ImageBlob.Dpi);

				return TextFormatter.FormatText
					(
						"Nom :  ",        name, "\n",
						"Fichier :  ",    this.ImageBlob.FileName, "\n",
						"Dimensions :  ", this.ImageBlob.PixelWidth.ToString (), "×", this.ImageBlob.PixelHeight.ToString (), " pixels\n",
						"Résolution :  ", dpi.ToString (), " dpi\n",
						"Profondeur :  ", this.ImageBlob.BitsPerPixel.ToString (), "bits"
					);
			}
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override string[] GetEntityKeywords()
		{
			return new string[] { this.Name.ToSimpleText () };
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Name.GetEntityStatus ());
				a.Accumulate (this.Description.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.ImageGroups.Select (x => x.GetEntityStatus ()));
				a.Accumulate (this.ImageCategory.GetEntityStatus ());

				return a.EntityStatus;
			}
		}
	}
}
