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
			string desc = this.Description.IsNullOrWhiteSpace ? "<i>Inconnu</i>" : this.Description.ToString ();

			if (this.ImageBlob.IsNull ())
			{
				return TextFormatter.FormatText
					(
						"Nom :  ",          name, "\n",
						"Description :  ",  desc
					);
			}
			else
			{
				var builder = new Epsitec.Common.IO.UriBuilder (this.ImageBlob.FileUri);
				// builder.Scheme     == "file"
				// builder.UserName   == "daniel"
				// builder.Host       == "daniel-pc"
				// builder.Path       == "C:/Users/Daniel/Documents/t.jpg"
				// builder.Fragment   == null
				// builder.Password   == null
				// builder.Query      == null
				// builder.PortNumber == 0

				string directory = System.IO.Path.GetDirectoryName (builder.Path);
				string filename  = System.IO.Path.GetFileName      (builder.Path);

				var dpi = System.Math.Ceiling (this.ImageBlob.Dpi);

				return TextFormatter.FormatText
					(
						"Nom :  ",          name, "\n",
						"Description :  ",  desc, "\n",
						"—\n",
						"Creation :  ", this.ImageBlob.CreationDate.ToString (), "\n",
						"Modification :  ", this.ImageBlob.LastModificationDate.ToString (), "\n",
						"—\n",
						"Utilisateur :  ",  builder.UserName, "\n",
						"Ordinateur :  ",   builder.Host, "\n",
						"Dossier :  ",      directory, "\n",
						"Fichier :  ",      filename, "\n",
						"Code :  ",         Common.Widgets.TextLayout.ConvertToTaggedText (this.ImageBlob.Code), "\n",
						"—\n",
						"Dimensions :  ",   this.ImageBlob.PixelWidth.ToString (), "×", this.ImageBlob.PixelHeight.ToString (), " pixels\n",
						"Résolution :  ",   dpi.ToString (), " dpi\n",
						"Profondeur :  ",   this.ImageBlob.BitsPerPixel.ToString (), "bits"
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
