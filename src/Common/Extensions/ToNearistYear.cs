namespace BookRec.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static partial class Extensions
    {
        public static int ToNearistYear(this DateTime source) => (int)Math.Round(source.Year / 100d, 0) * 100;
    }
}
