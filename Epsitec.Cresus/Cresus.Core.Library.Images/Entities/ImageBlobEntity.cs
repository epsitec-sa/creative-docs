//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class ImageBlobEntity
	{
		public override FormattedText GetSummary()
		{
			var builder = new Epsitec.Common.IO.UriBuilder (this.FileUri);
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

			var dpi = System.Math.Ceiling (this.Dpi);

			return TextFormatter.FormatText
				(
					"Creation :  ",     this.CreationDate.ToString (), "\n",
					"Modification :  ", this.LastModificationDate.ToString (), "\n",
					"—\n",
					"Utilisateur :  ",  builder.UserName, "\n",
					"Ordinateur :  ",   builder.Host, "\n",
					"Dossier :  ",      directory, "\n",
					"Fichier :  ",      filename, "\n",
					//"Code :  ",         Common.Widgets.TextLayout.ConvertToTaggedText (this.Code), "\n",
					"—\n",
					"Dimensions :  ", this.PixelWidth.ToString (), "×", this.PixelHeight.ToString (), " pixels\n",
					"Résolution :  ",   dpi.ToString (), " dpi\n",
					"Profondeur :  ",   this.BitsPerPixel.ToString (), "bits"
				);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.FileName);
		}

		public override string[] GetEntityKeywords()
		{
			return new string[] { this.Code };
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Code.GetEntityStatus ());
				a.Accumulate (this.FileName.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.FileUri.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.FileMimeType.GetEntityStatus ());

				return a.EntityStatus;
			}
		}
	}
}
