﻿using System;
using System.Collections.Generic;

using Public.Common.Augustus.Lib.Extensions;


namespace Public.Common.Augustus.Lib
{
    public class BuildItem
    {
        public const char TokenSeparator = '|';


        #region Static

        public static List<BuildItem> GetBuildItems(IEnumerable<string> buildItemSpecifications)
        {
            var output = new List<BuildItem>();
            foreach (string buildItemSpecification in buildItemSpecifications)
            {
                var buildItem = BuildItem.Parse(buildItemSpecification);
                output.Add(buildItem);
            }

            return output;
        }

        public static BuildItem Parse(string buildItemSpecification)
        {
            var output = BuildItem.Parse(buildItemSpecification, BuildItem.TokenSeparator);
            return output;
        }

        public static BuildItem Parse(string buildItemSpecification, char tokenSeparator)
        {
            string buildFilePath;
            OsEnvironment platform;
            Platform architecture;
            BuildItem.Parse(buildItemSpecification, tokenSeparator, out buildFilePath, out platform, out architecture);

            var output = new BuildItem(buildFilePath, platform, architecture);
            return output;
        }

        private static void Parse(string buildItemSpecification, char tokenSeparator, out string buildFilePath, out OsEnvironment platform, out Platform architecture)
        {
            string[] tokens = buildItemSpecification.Split(tokenSeparator);
            if (2 > tokens.Length)
            {
                var message = String.Format(@"Missing build file path or platform for build item specification: '{0}'.", buildItemSpecification);
                throw new ArgumentException(message, "buildItemSpecification");
            }

            buildFilePath = BuildItem.GetBuildFilePathToken(tokens);

            var platformToken = BuildItem.GetOsEnvironmentToken(tokens);
            platform = OsEnvironmentExtensions.FromDefault(platformToken);

            architecture = Platform.Default;
            if (2 < tokens.Length)
            {
                string architectureToken = BuildItem.GetPlatformToken(tokens);
                architecture = PlatformExtensions.FromDefault(architectureToken);

                if (OsEnvironment.Cygwin == platform && Platform.Default != architecture)
                {
                    var message = String.Format(@"Specification of architecture other than {0} not allowed for platform {1}.", Platform.Default.ToDefaultString(), platform.ToDefaultString());
                    throw new ArgumentException(message, "buildItemSpecification");
                }
            }
        }

        private static string GetBuildFilePathToken(string[] tokens)
        {
            var output = tokens[0];
            return output;
        }

        private static string GetOsEnvironmentToken(string[] tokens)
        {
            var output = tokens[1];
            return output;
        }

        private static string GetPlatformToken(string[] tokens)
        {
            var output = tokens[2];
            return output;
        }

        #endregion

        public string FilePath { get; set; }
        public OsEnvironment OsEnvironment { get; set; }
        public Platform Platform { get; set; }


        public BuildItem()
        {
        }

        public BuildItem(string buildFilePath, OsEnvironment osEnvironment, Platform platform)
        {
            this.FilePath = buildFilePath;
            this.OsEnvironment = osEnvironment;
            this.Platform = platform;
        }

        public override string ToString()
        {
            var output = String.Format(@"{0}{1}{2}{1}{3}", this.FilePath, BuildItem.TokenSeparator, this.OsEnvironment.ToDefaultString(), this.Platform.ToDefaultString());
            return output;
        }
    }
}
