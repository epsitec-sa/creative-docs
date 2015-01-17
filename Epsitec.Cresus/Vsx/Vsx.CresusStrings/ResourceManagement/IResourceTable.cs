using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec.Cresus.ResourceManagement
{
	public interface IResourceTable : IReadOnlyDictionary<string, ResourceItem>
	{
		CultureInfo Culture
		{
			get;
		}
	}
}
