//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.V11
{
	public struct V11Message
	{
		public V11Message(V11Error error, int? lineRank, FormattedText description)
		{
			this.error       = error;
			this.lineRank    = lineRank;
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


		public static V11Message OK      = new V11Message (V11Error.OK,      null, null);
		public static V11Message Aborted = new V11Message (V11Error.Aborted, null, null);


		private readonly V11Error			error;
		private readonly int?				lineRank;
		private readonly FormattedText		description;
	}
}
