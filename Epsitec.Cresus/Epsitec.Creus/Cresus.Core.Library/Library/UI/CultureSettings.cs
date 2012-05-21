//	Copyright © 2008-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Library.UI
{
	public sealed class CultureSettings
	{
		internal CultureSettings()
		{
			this.culture = System.Threading.Thread.CurrentThread.CurrentCulture;
		}

		
		public CultureInfo						CultureInfo
		{
			get
			{
				return this.culture;
			}
		}

		public string							TwoLetterISOLanguageName
		{
			get
			{
				return this.twoLetterISOLanguageName ?? this.CultureInfo.TwoLetterISOLanguageName;
			}
		}

		public string							TwoLetterISOLanguageNameForDefault
		{
			get
			{
				return this.twoLetterISOLanguageNameForDefault;
			}
		}

		public bool								IsActiveLanguageAlsoTheDefault
		{
			get
			{
				return this.TwoLetterISOLanguageName == this.twoLetterISOLanguageNameForDefault;
			}
		}

		public bool								HasTwoLetterISOLanguageName
		{
			get
			{
				return this.twoLetterISOLanguageName != null;
			}
		}


		public void SelectLanguage(string twoLetterISOLanguageName)
		{
			if (string.IsNullOrEmpty (twoLetterISOLanguageName) ||
				MultilingualText.DefaultTwoLetterISOLanguageName == twoLetterISOLanguageName)
			{
				twoLetterISOLanguageName = null;
			}

			var oldTwoLetter = this.TwoLetterISOLanguageName;

			this.twoLetterISOLanguageName = twoLetterISOLanguageName;

			var newTwoLetter = this.TwoLetterISOLanguageName;

			if (oldTwoLetter != newTwoLetter)
			{
				this.NotifyTwoLetterISOLanguageNameChanged (oldTwoLetter, newTwoLetter);
			}
		}

		public void DefineDefaultLanguage(string twoLetterISOLanguageName)
		{
			this.twoLetterISOLanguageNameForDefault = twoLetterISOLanguageName;
		}

		public bool IsDefaultLanguage(string twoLetterISOLanguageName)
		{
			if (string.IsNullOrEmpty (twoLetterISOLanguageName))
			{
				return false;
			}

			if (MultilingualText.DefaultTwoLetterISOLanguageName == twoLetterISOLanguageName)
			{
				return true;
			}
			if (this.twoLetterISOLanguageNameForDefault == twoLetterISOLanguageName)
			{
				return true;
			}

			return false;
		}


		private void NotifyTwoLetterISOLanguageNameChanged(string oldTwoLetterISOLanguageName, string newTwoLetterISOLanguageName)
		{
			Services.NotifyUpdateRequested (this);
		}



		private readonly CultureInfo			culture;
		private string							twoLetterISOLanguageName;
		private string							twoLetterISOLanguageNameForDefault;
	}
}
