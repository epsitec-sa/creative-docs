using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Support
{
	public interface IDocumentInfo
	{
		string GetDescription();
		Drawing.Image GetThumbnail();
		void GetAsyncThumbnail(SimpleCallback<Drawing.Image> callback);
	}
}
