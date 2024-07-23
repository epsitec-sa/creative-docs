/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Common.Support.Extensions
{
    /// <summary>
    /// The <c>DateTimeExtensions</c> class provides extension methods for <see cref="System.DateTime"/>.
    /// </summary>
    public static class DateTimeExtensions
    {
        public static bool InRange(
            this System.DateTime date,
            System.DateTime? beginDate,
            System.DateTime? endDate
        )
        {
            var dateUtc = date.ToUniversalTime();

            if (beginDate.HasValue && beginDate.Value.ToUniversalTime() > dateUtc)
            {
                return false;
            }
            if (endDate.HasValue && endDate.Value.ToUniversalTime() <= dateUtc)
            {
                return false;
            }

            return true;
        }

        public static bool InRange(this Date date, Date? beginDate, Date? endDate)
        {
            if (beginDate.HasValue && beginDate.Value > date)
            {
                return false;
            }
            if (endDate.HasValue && endDate.Value <= date)
            {
                return false;
            }

            return true;
        }

        public static System.DateTime? ToLocalTime(this System.DateTime? date)
        {
            if (date == null)
            {
                return null;
            }

            return date.Value.ToLocalTime();
        }
    }
}
