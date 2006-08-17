//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Drawing
{
	public interface ITextBreakEngine
	{
		void SetText(string text, TextBreakMode mode);
		void SetRuns(ICollection<TextBreakRun> runs);
		void Rewind();
		bool GetNextBreak(double maxWidth, out string text, out double textWidth, out int textLength);
//		short[] GetHyphenationPositions(string text);
	}
}
