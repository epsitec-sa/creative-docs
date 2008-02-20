//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
