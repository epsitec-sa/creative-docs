using System.Collections.Generic;

using System.IO;


namespace Epsitec.Cresus.WebCore.Server.Core.Extraction
{


	internal abstract class ArrayFormat
	{


		public abstract string Extension
		{
			get;
		}


		public abstract void Write(Stream stream, IList<string> headers, IList<IList<string>> rows);


	}


}
