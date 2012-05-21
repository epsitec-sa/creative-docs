//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// The <c>ILanguageRecognizer</c> interface is used to map a character code
	/// to a language (locale) and hyphenation parameter.
	/// </summary>
	public interface ILanguageRecognizer
	{
		bool GetLanguage(ulong[] text, int offset, out double hyphenation, out string locale);
	}
}
