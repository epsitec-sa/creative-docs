﻿//	Copyright © 2008-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public static partial class UI
	{
		public sealed class CultureSettings
		{
			internal CultureSettings()
			{
				this.culture = System.Threading.Thread.CurrentThread.CurrentCulture;
			}

			public System.Globalization.CultureInfo CultureInfo
			{
				get
				{
					return this.culture;
				}
			}

			public string LanguageId
			{
				get
				{
					return this.languageId ?? this.CultureInfo.TwoLetterISOLanguageName;
				}
			}

			public bool HasLanguageId
			{
				get
				{
					return this.languageId != null;
				}
			}

			public void SelectLanguage(string languageId)
			{
				if ((string.IsNullOrEmpty (languageId)) ||
					(MultilingualText.DefaultLanguageId == languageId))
				{
					languageId = null;
				}

				this.languageId = languageId;
			}



			private System.Globalization.CultureInfo	culture;
			private string languageId;
		}
	}
}
