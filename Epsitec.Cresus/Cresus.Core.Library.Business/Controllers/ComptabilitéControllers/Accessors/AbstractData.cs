//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Business.Finance.Comptabilité;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	/// <summary>
	/// Données génériques pour la comptabilité.
	/// </summary>
	public abstract class AbstractData
	{
		public bool IsBold
		{
			get;
			set;
		}

		public bool IsItalic
		{
			get;
			set;
		}


		public FormattedText Typo(FormattedText value)
		{
			if (this.IsBold)
			{
				value = value.ApplyBold ();
			}

			if (this.IsItalic)
			{
				value = value.ApplyItalic ();
			}

			return value;
		}
	}
}