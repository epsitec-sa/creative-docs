using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Epsitec.Data.Platform.Directories
{
	static class DirectoriesResponseChecker
	{
		public static bool RequestHasError(XElement Response)
		{
			if (Response.Element ("ErrorInfo").Attribute ("ErrorCode").Value=="0")
			{
				return false;
			}
			else
			{
				return true;
			}
		}

        public static bool NoResult(XElement Response)
        {
            if (Response.Element("ErrorInfo").Attribute("ErrorCode").Value == "200")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

		public static bool SessionIsInvalid(XElement Response)
		{

			if (Response.Element ("ErrorInfo").Attribute ("ErrorCode").Value=="308")
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
