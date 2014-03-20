//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Reporting
{
	/// <summary>
	/// The <c>IContent</c> interface is used in conjunction with
	/// <see cref="AiderOfficeReportEntity"/> to serialize and deserialize
	/// blobs, and to produce content for text documents, such as letters.
	/// </summary>
	public interface IContent : IContentStore, IContentTextProducer
	{
	}
}