using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Epsitec.Data.Platform.Directories.Entity
{
    public class DirectoriesResultInfo
    {

        public DirectoriesResultInfo()
        {
            this.DeliveredEntries = 0;
            this.DeliveredUnits = 0;
            this.MatchedEntries = 0;
        }

        public DirectoriesResultInfo(XElement ResultInfo)
        {
            this.DeliveredEntries = Convert.ToInt32(ResultInfo.Attribute("DeliveredEntries").Value);
            this.DeliveredUnits = Convert.ToInt32(ResultInfo.Attribute("DeliveredUnits").Value);
            this.MatchedEntries = Convert.ToInt32(ResultInfo.Attribute("MatchedEntries").Value);
        }

        public int DeliveredEntries;
        public int DeliveredUnits;
        public int MatchedEntries;
    }
}
