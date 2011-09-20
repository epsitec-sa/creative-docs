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

		public string							LanguageId
		{
			get
			{
				return this.languageId ?? this.CultureInfo.TwoLetterISOLanguageName;
			}
		}

		public string							LanguageIdForDefault
		{
			get
			{
				return this.languageIdForDefault;
			}
		}

		public bool								IsActiveLanguageAlsoTheDefault
		{
			get
			{
				return this.LanguageId == this.languageIdForDefault;
			}
		}

		public bool								HasLanguageId
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

			var oldLanguageId = this.LanguageId;

			this.languageId = languageId;

			var newLanguageId = this.LanguageId;

			if (oldLanguageId != newLanguageId)
			{
				this.NotifyLanguageIdChanged (oldLanguageId, newLanguageId);
			}
		}

		public void DefineDefaultLanguage(string languageId)
		{
			this.languageIdForDefault = languageId;
		}

		public bool IsDefaultLanguage(string languageId)
		{
			if (string.IsNullOrEmpty (languageId))
			{
				return false;
			}

			if (MultilingualText.DefaultLanguageId == languageId)
			{
				return true;
			}
			if (this.languageIdForDefault == languageId)
			{
				return true;
			}

			return false;
		}


		private void NotifyLanguageIdChanged(string oldLanguageId, string newLanguageId)
		{
			Services.NotifyUpdateRequested (this);
		}



		private readonly CultureInfo			culture;
		private string							languageId;
		private string							languageIdForDefault;
	}
}
