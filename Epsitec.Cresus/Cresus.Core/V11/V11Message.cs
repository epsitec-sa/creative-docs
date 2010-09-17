//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.V11
{
	public struct V11Message
	{
		public V11Message(V11Error error, int? lineRank, string lineContent, FormattedText description)
		{
			this.error       = error;
			this.lineRank    = lineRank;
			this.lineContent = lineContent;
			this.description = description;
		}


		public V11Error Error
		{
			get
			{
				return this.error;
			}
		}

		public int? LineRank
		{
			get
			{
				return this.lineRank;
			}
		}

		public string LineContent
		{
			get
			{
				return this.lineContent;
			}
		}

		public FormattedText Description
		{
			get
			{
				return this.description;
			}
		}


		public bool IsOK
		{
			get
			{
				return this.error == V11Error.OK;
			}
		}

		public bool IsError
		{
			get
			{
				return this.error != V11Error.OK;
			}
		}


		public static FormattedText Descriptions(List<V11Message> errors)
		{
			if (errors.Count == 0)
			{
				return TextFormatter.FormatText ("Aucune erreur.");
			}
			else if (errors.Count == 1)
			{
				return errors[0].Description;
			}
			else
			{
				return TextFormatter.FormatText (errors.Count.ToString (), "erreurs");
			}
		}


		public static V11Message OK           = new V11Message (V11Error.OK,           null, null, null);
		public static V11Message Aborted      = new V11Message (V11Error.Aborted,      null, null, null);
		public static V11Message GenericError = new V11Message (V11Error.GenericError, null, null, null);


		private readonly V11Error			error;
		private readonly int?				lineRank;
		private readonly string				lineContent;
		private readonly FormattedText		description;
	}
}
