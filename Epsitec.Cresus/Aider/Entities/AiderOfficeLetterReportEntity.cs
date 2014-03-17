//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Data.Platform;

using System;

using System.Linq;
using System.Collections.Generic;

namespace Epsitec.Aider.Entities
{
	public partial class AiderOfficeLetterReportEntity
	{
		public override FormattedText GetCompactSummary()
		{			
			return TextFormatter.FormatText (this.Name);		
		}

		public override FormattedText GetSummary()
		{
			return new FormattedText (this.Name + "<br/><a href='" + this.ProcessorUrl +"'>Générer</a>");	
		}

		public string GetLetterContent()
		{
			char[] chars = new char[this.LetterContent.Length / sizeof (char)];
			System.Buffer.BlockCopy (this.LetterContent, 0, chars, 0, this.LetterContent.Length);
			return new string (chars);
		}

		public static AiderOfficeLetterReportEntity Create(BusinessContext context,AiderContactEntity recipient, AiderOfficeSenderEntity sender,string documentName,string letterContent)
		{
			var letter = context.CreateAndRegisterEntity<AiderOfficeLetterReportEntity> ();

			letter.Name				= documentName;
			letter.CreationDate		= Date.Today;
			letter.LetterContent	= AiderOfficeLetterReportEntity.ConvertLetterContent(letterContent);
			//TODO 
			letter.TownAndDate		= sender.Office.OfficeMainContact.Address.Town.Name + ", le " + letter.CreationDate.ToString("dd MMM yyyy", null);
			letter.RecipientContact = recipient;
			letter.Office			= sender.Office;

			//Add document to the sender office document management
			sender.Office.AddDocumentInternal (letter);
			return letter;
		}

		private static byte[] ConvertLetterContent(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof (char)];
			System.Buffer.BlockCopy (str.ToCharArray (), 0, bytes, 0, bytes.Length);
			return bytes;
		}
	}
}
