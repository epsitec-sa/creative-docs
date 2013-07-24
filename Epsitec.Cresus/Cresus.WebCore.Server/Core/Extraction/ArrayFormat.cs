using System.Collections.Generic;

using System.IO;


namespace Epsitec.Cresus.WebCore.Server.Core.Extraction
{


	/// <summary>
	/// This class is used to configure the formal in which an array of entities will be serialized
	/// to a file.
	/// </summary>
	internal abstract class ArrayFormat
	{


		public abstract string Extension
		{
			get;
		}


		public abstract void Write(Stream stream, IList<string> headers, IList<IList<string>> rows);


	}


}
