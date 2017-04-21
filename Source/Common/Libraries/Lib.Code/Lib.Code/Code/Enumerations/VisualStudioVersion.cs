﻿using System;

using Public.Common.Lib;


namespace Public.Common.Lib.Code.Physical
{
    // Ok.
    public enum VisualStudioVersion
    {
        VS2010,
        VS2013,
        VS2015,
        VS2017,
    }


    // Ok.
    public static class VisualStudioVersionExtensions
    {
        public const string VS2010 = @"VS2010";
        public const string VS2013 = @"VS2013";
        public const string VS2015 = @"VS2015";
        public const string VS2017 = @"VS2017";


        public static string ToDefaultString(this VisualStudioVersion visualStudioVersion)
        {
            string output;
            switch (visualStudioVersion)
            {
                case VisualStudioVersion.VS2010:
                    output = VisualStudioVersionExtensions.VS2010;
                    break;

                case VisualStudioVersion.VS2013:
                    output = VisualStudioVersionExtensions.VS2013;
                    break;

                case VisualStudioVersion.VS2015:
                    output = VisualStudioVersionExtensions.VS2015;
                    break;

                case VisualStudioVersion.VS2017:
                    output = VisualStudioVersionExtensions.VS2017;
                    break;

                default:
                    throw new UnexpectedEnumerationValueException<VisualStudioVersion>(visualStudioVersion);
            }

            return output;
        }

        public static VisualStudioVersion FromDefault(string visualStudioVersion)
        {
            VisualStudioVersion output;
            if (!VisualStudioVersionExtensions.TryFromDefault(visualStudioVersion, out output))
            {
#if (CSharp_6)
                throw new ArgumentException(@"Unrecognized Visual Studio version string.", nameof(output));
#else
                throw new ArgumentException(@"Unrecognized Visual Studio version string.", "output");
#endif  
            }

            return output;
        }

        public static bool TryFromDefault(string visualStudioVersion, out VisualStudioVersion value)
        {
            bool output = true;
            value = VisualStudioVersion.VS2015;

            switch (visualStudioVersion)
            {
                case VisualStudioVersionExtensions.VS2010:
                    value = VisualStudioVersion.VS2010;
                    break;

                case VisualStudioVersionExtensions.VS2013:
                    value = VisualStudioVersion.VS2013;
                    break;

                case VisualStudioVersionExtensions.VS2015:
                    value = VisualStudioVersion.VS2015;
                    break;

                case VisualStudioVersionExtensions.VS2017:
                    value = VisualStudioVersion.VS2017;
                    break;

                default:
                    output = false;
                    break;
            }

            return output;
        }
    }
}