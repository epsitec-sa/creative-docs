//	Copyright © 2008-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library.UI
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

			var oldLanguageId = this.LanguageId;

			this.languageId = languageId;

			var newLanguageId = this.LanguageId;

			if (oldLanguageId != newLanguageId)
			{
				this.NotifyLanguageIdChanged (oldLanguageId, newLanguageId);
			}
		}

		private void NotifyLanguageIdChanged(string oldLanguageId, string newLanguageId)
		{
			Services.NotifyUpdateRequested (this);
		}



		private System.Globalization.CultureInfo	culture;
		private string languageId;
	}
}
