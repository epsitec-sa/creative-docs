using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epsitec.Data.Platform;
using Epsitec.Data.Platform.MatchSort;

namespace TestMatchSortLoader
{
	class Program
	{
		static void Main(string[] args)
		{
			//var csvFilePath = args[0];
			//var metaData = MatchSortLoader.GetFileMetaData (csvFilePath);
			SwissPost.Initialize ();

			var fleurier = SwissPostStreetRepository.Current.Streets.Where (s => s.Zip.ZipCode == 2114);
		}
	}
}
