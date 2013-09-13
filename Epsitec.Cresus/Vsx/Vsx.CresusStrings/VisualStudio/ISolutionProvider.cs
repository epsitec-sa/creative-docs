using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers;
using Roslyn.Services;

namespace Epsitec.VisualStudio
{
	public interface ISolutionProvider
	{
		ISolution Solution
		{
			get;
		}

		ISolution UpdateSolution(DocumentId documentId, IText text);
	}
}
