﻿using System;
using System.Collections.Generic;
using System.IO;

using Public.Common.Lib;
using Public.Common.Lib.Code;
using CodeUtilities = Public.Common.Lib.Code.Utilities;
using Public.Common.Lib.Code.Logical;
using Public.Common.Lib.Code.Physical;
using Public.Common.Lib.Extensions;
using Public.Common.Lib.IO;
using Public.Common.Lib.IO.Extensions;
using Public.Common.Lib.Organizational;

using AvonUtilities = Public.Common.Avon.Lib.Utilities;


namespace Public.Common.Avon
{
    public class Configuration
    {
        private const char VisualStudioVersionTokenSeparator = '|';

        public const string CreateNewSolutionSetFunctionName = @"CreateNewSolutionSet";
        public const string CreateSolutionSetFromInitialVsVersionSolutionFunctionName = @"CreateSolutionSetFromInitialVsVersionSolution";
        public const string DistributeChangesFromDefaultVsVersionSolutionFunctionName = @"DistributeChangesFromDefaultVsVersionSolution";
        public const string DistributeChangesFromSpecificVsVersionSolutionFunctionName = @"DistributeChangesFromSpecificVsVersionSolution";
        public const string EnsureVsVersionedBinAndObjPropertiesFunctionName = @"EnsureVsVersionedBinAndObjProperties";
        public const string SetDefaultVsVersionSolutionFunctionName = @"SetDefaultVsVersionSolution";


        #region Static

        public static bool TryParseArguments(string[] args, IOutputStream outputStream, out Configuration configuration)
        {
            bool output = true;

            configuration = new Configuration();
            try
            {
                string functionToken = args[0];

                string[] functionArgs = new string[args.Length - 1];
                Array.Copy(args, 1, functionArgs, 0, functionArgs.Length);

                switch (functionToken)
                {
                    case Configuration.CreateNewSolutionSetFunctionName:
                        output = Configuration.HandleCreateNewSolutionSet(configuration, outputStream, functionArgs);
                        break;

                    case Configuration.CreateSolutionSetFromInitialVsVersionSolutionFunctionName:
                        output = Configuration.HandleCreateSolutionSetFromInitialVsVersionSolution(configuration, outputStream, functionArgs);
                        break;

                    case Configuration.DistributeChangesFromDefaultVsVersionSolutionFunctionName:
                        output = Configuration.HandleDistributeChangesFromDefaultVsVersionSolution(configuration, outputStream, functionArgs);
                        break;

                    case Configuration.DistributeChangesFromSpecificVsVersionSolutionFunctionName:
                        output = Configuration.HandleDistributeChangesFromSpecificVsVersionSolution(configuration, outputStream, functionArgs);
                        break;

                    case Configuration.EnsureVsVersionedBinAndObjPropertiesFunctionName:
                        output = Configuration.HandleEnsureVsVersionedBinAndObjProperties(configuration, outputStream, functionArgs);
                        break;

                    case Configuration.SetDefaultVsVersionSolutionFunctionName:
                        output = Configuration.HandleSetDefaultVsVersionSolution(configuration, outputStream, functionArgs);
                        break;

                    default:
                        string message = String.Format(@"Unrecognized function name: {0}.", functionToken);
                        throw new InvalidOperationException(message);
                }
            }
            catch (Exception ex)
            {
                output = false;

                outputStream.WriteLineAndBlankLine(@"ERROR parsing input arguments.");
                outputStream.WriteLineAndBlankLine(ex.Message);

                Configuration.DisplayUsage(outputStream);
            }

            return output;
        }

        private static bool HandleSetDefaultVsVersionSolution(Configuration configuration, IOutputStream outputStream, string[] functionArgs)
        {
            bool output = true;

            try
            {
                int numberOfArgs = functionArgs.Length;
                if (2 == numberOfArgs)
                {
                    string solutionDirectoryPathToken = functionArgs[0];
                    string visualStudioVersionToken = functionArgs[1];

                    VisualStudioVersion vsVersion = VisualStudioVersionExtensions.FromDefault(visualStudioVersionToken);
                    VisualStudioVersion actualVsVersion = vsVersion;
                    if (VisualStudioVersion.VS_LATEST == vsVersion)
                    {
                        actualVsVersion = VisualStudioVersion.VS2017;
                    }

                    configuration.Action = () =>
                    {
                        string solutionDirectoryPath = solutionDirectoryPathToken;
                        VisualStudioVersion defaultVersion = actualVsVersion;

                        AvonUtilities.SetDefaultVisualStudioVersion(solutionDirectoryPath, defaultVersion);
                    };
                }
                else
                {
                    string message = String.Format(@"Invalid number of arguments found: {0}. Expected 2.", numberOfArgs);
                    throw new ArgumentException(message);
                }
            }
            catch (Exception ex)
            {
                output = false;

                outputStream.WriteLineAndBlankLine(ex.Message);

                Configuration.DisplaySetDefaultVsVersionSolutionUsage(outputStream);
            }

            return output;
        }

        private static void DisplaySetDefaultVsVersionSolutionUsage(IOutputStream outputStream)
        {
            string programName = Constants.ProgramName;
            string functionName = Configuration.SetDefaultVsVersionSolutionFunctionName;
            string line = String.Format(@"{0} {1}", programName, functionName);
            outputStream.WriteLine(line);
            outputStream.WriteLine();

            line = String.Format(@"Usage: {0}.exe {1} SolutionDirectoryPath VisualStudioVersion", programName, functionName);
            outputStream.WriteLine(line);
            outputStream.WriteLine();

            outputStream.WriteLine(@"Example:");
            line = String.Format(@"{0}.exe {1} C:\Organizations\Minex\Repositories\Public\Source\Common\Experiments\Nahant VS2010", programName, functionName);
            outputStream.WriteLine(line);
            outputStream.WriteLine();
        }

        private static bool HandleEnsureVsVersionedBinAndObjProperties(Configuration configuration, IOutputStream outputStream, string[] functionArgs)
        {
            bool output = true;

            try
            {
                int numberOfArgs = functionArgs.Length;
                if (1 < numberOfArgs)
                {
                    string solutionDirectoryPathToken = functionArgs[0];

                    HashSet<string> projectPaths = new HashSet<string>();
                    for (int iArg = 1; iArg < functionArgs.Length; iArg++)
                    {
                        projectPaths.Add(functionArgs[iArg]);
                    }

                    configuration.Action = () =>
                    {
                        string solutionDirectoryPath = solutionDirectoryPathToken;
                        HashSet<string> projectDirectoryPathsAllowedToChange = new HashSet<string>(projectPaths);

                        CodeUtilities.EnsureVsVersionedBinAndObjProperties(solutionDirectoryPath, projectDirectoryPathsAllowedToChange);
                    };
                }
                else
                {
                    string message = String.Format(@"Invalid number of arguments found: {0}. Expected at least 2.", numberOfArgs);
                    throw new ArgumentException(message);
                }
            }
            catch (Exception ex)
            {
                output = false;

                outputStream.WriteLineAndBlankLine(ex.Message);

                Configuration.DisplayEnsureVsVersionedBinAndObjPropertiesUsage(outputStream);
            }

            return output;
        }

        private static void DisplayEnsureVsVersionedBinAndObjPropertiesUsage(IOutputStream outputStream)
        {
            string programName = Constants.ProgramName;
            string functionName = Configuration.EnsureVsVersionedBinAndObjPropertiesFunctionName;
            string line = String.Format(@"{0} {1}", programName, functionName);
            outputStream.WriteLine(line);
            outputStream.WriteLine();

            line = String.Format(@"Usage: {0}.exe {1} SolutionDirectoryPath ProjectDirectoryPath1 ProjectDirectoryPath2 ...", programName, functionName);
            outputStream.WriteLine(line);
            outputStream.WriteLine();

            outputStream.WriteLine(@"Example:");
            line = String.Format(@"{0}.exe {1} C:\Organizations\Minex\Repositories\Public\Source\Common\Experiments\Nahant C:\Organizations\Minex\Repositories\Public\Source\Common\Experiments\Nahant\Nahant C:\Organizations\Minex\Repositories\Public\Source\Common\Experiments\Nahant\Construction", programName, functionName);
            outputStream.WriteLine(line);
            outputStream.WriteLine();
        }

        private static bool HandleDistributeChangesFromSpecificVsVersionSolution(Configuration configuration, IOutputStream outputStream, string[] functionArgs)
        {
            bool output = true;

            try
            {
                int numberOfArgs = functionArgs.Length;
                if (2 == numberOfArgs)
                {
                    string solutionDirectoryPathToken = functionArgs[0];
                    string sourceVsVersionToken = functionArgs[1];

                    VisualStudioVersion sourceVsVersionTemp = VisualStudioVersionExtensions.FromDefault(sourceVsVersionToken);

                    configuration.Action = () =>
                    {
                        string solutionDirectoryPath = solutionDirectoryPathToken;
                        VisualStudioVersion sourceVsVersion = sourceVsVersionTemp;

                        AvonUtilities.DistributeChangesFromSpecifiedVsVersionSolution(solutionDirectoryPath, sourceVsVersion);
                    };
                }
                else
                {
                    string message = String.Format(@"Invalid number of arguments found: {0}. Expected 2.", numberOfArgs);
                    throw new ArgumentException(message);
                }
            }
            catch (Exception ex)
            {
                output = false;

                outputStream.WriteLineAndBlankLine(ex.Message);

                Configuration.DisplayDistributeChangesFromSpecificVsVersionSolutionUseage(outputStream);
            }

            return output;
        }

        private static void DisplayDistributeChangesFromSpecificVsVersionSolutionUseage(IOutputStream outputStream)
        {
            string programName = Constants.ProgramName;
            string functionName = Configuration.DistributeChangesFromSpecificVsVersionSolutionFunctionName;
            string line = String.Format(@"{0} {1}", programName, functionName);
            outputStream.WriteLine(line);
            outputStream.WriteLine();

            line = String.Format(@"Usage: {0}.exe {1} SolutionDirectoryPath VisualStudioVersion", programName, functionName);
            outputStream.WriteLine(line);
            outputStream.WriteLine();

            outputStream.WriteLine(@"Example:");
            line = String.Format(@"{0}.exe {1} C:\Organizations\Minex\Repositories\Public\Source\Common\Experiments\Nahant VS2010", programName, functionName);
            outputStream.WriteLine(line);
            outputStream.WriteLine();
        }

        private static bool HandleDistributeChangesFromDefaultVsVersionSolution(Configuration configuration, IOutputStream outputStream, string[] functionArgs)
        {
            outputStream.WriteLineAndBlankLine(@"Distributing changes from default VS version solution.");

            bool output = true;

            try
            {
                int numberOfArgs = functionArgs.Length;
                if (1 == numberOfArgs)
                {
                    string solutionDirectoryPathToken = functionArgs[0];
                    outputStream.WriteLineAndBlankLine(String.Format(@"Solution directory: {0}", solutionDirectoryPathToken));

                    configuration.Action = () =>
                    {
                        string solutionDirectoryPath = solutionDirectoryPathToken;

                        AvonUtilities.DistributeChangesFromDefaultVsVersionSolution(solutionDirectoryPath);
                    };
                }
                else
                {
                    string message = String.Format(@"Invalid number of arguments found: {0}. Expected 1.", numberOfArgs);
                    throw new ArgumentException(message);
                }

                outputStream.WriteLine(@"Successfully distributed changes.");
            }
            catch (Exception ex)
            {
                output = false;

                outputStream.WriteLine(@"Failed to distribute changes:");
                outputStream.WriteLineAndBlankLine(ex.Message);

                Configuration.DisplayDistributeChangesFromDefaultVsVersionSolutionUsage(outputStream);
            }            

            return output;
        }

        private static void DisplayDistributeChangesFromDefaultVsVersionSolutionUsage(IOutputStream outputStream)
        {
            string programName = Constants.ProgramName;
            string functionName = Configuration.DistributeChangesFromDefaultVsVersionSolutionFunctionName;
            string line = String.Format(@"{0} {1}", programName, functionName);
            outputStream.WriteLine(line);
            outputStream.WriteLine();

            line = String.Format(@"Usage: {0}.exe {1} SolutionDirectoryPath", programName, functionName);
            outputStream.WriteLine(line);
            outputStream.WriteLine();

            outputStream.WriteLine(@"Example:");
            line = String.Format(@"{0}.exe {1} C:\Organizations\Minex\Repositories\Public\Source\Common\Experiments\Nahant", programName, functionName);
            outputStream.WriteLine(line);
            outputStream.WriteLine();
        }

        private static bool HandleCreateSolutionSetFromInitialVsVersionSolution(Configuration configuration, IOutputStream outputStream, string[] functionArgs)
        {
            bool output = true;

            try
            {
                int numberOfArgs = functionArgs.Length;
                if(0 < numberOfArgs && 4 > numberOfArgs)
                {
                    string solutionFilePath = functionArgs[0];
                    string vsVersionsToken = numberOfArgs > 1 ? functionArgs[1] : Configuration.GetAllVsVersionStrings();
                    string destinationDirectoryPath = numberOfArgs > 2 ? functionArgs[2] : Path.GetDirectoryName(solutionFilePath);

                    configuration.Action = () =>
                    {
                        string initialSolutionFilePath = solutionFilePath;
                        VisualStudioVersion[] desiredVsVersions = Configuration.GetVsVersions(vsVersionsToken);
                        
                        Creation.CreateSolutionSet(initialSolutionFilePath, desiredVsVersions, destinationDirectoryPath);
                    };
                }
                else
                {
                    string message = String.Format(@"Invalid number of arguments found: {0}. Expected 1 or 2.", numberOfArgs);
                    throw new ArgumentException(message);
                }
            }
            catch(Exception ex)
            {
                output = false;

                outputStream.WriteLineAndBlankLine(ex.Message);

                Configuration.DisplayCreateSolutionSetFromInitialVsVersionSolutionUsage(outputStream);
            }

            return output;
        }

        private static void DisplayCreateSolutionSetFromInitialVsVersionSolutionUsage(IOutputStream outputStream)
        {
            string programName = Constants.ProgramName;
            string functionName = Configuration.CreateSolutionSetFromInitialVsVersionSolutionFunctionName;
            string line = String.Format(@"{0} {1}", programName, functionName);
            outputStream.WriteLine(line);
            outputStream.WriteLine();

            line = String.Format(@"Usage: {0}.exe {1} SolutionFilePath [VisualStudioVersion1|VisualStudioVersion2|... or VS_ALL] [Destination directory path]", programName, functionName);
            outputStream.WriteLine(line);
            outputStream.WriteLine();

            outputStream.WriteLine(@"Example:");
            line = String.Format(@"{0}.exe {1} C:\Organizations\Minex\Repositories\Public\Source\Common\Experiments\Nahant\Nahant.VS2015.sln VS2010|VS2013|VS2015|VS2017 C:\temp", programName, functionName);
            outputStream.WriteLine(line);
            outputStream.WriteLine();
        }

        private static VisualStudioVersion[] GetVsVersions(string vsVersionsToken)
        {
            string[] vsVersions = vsVersionsToken.Split(Configuration.VisualStudioVersionTokenSeparator);

            VisualStudioVersion[] output;
            if (1 == vsVersions.Length && VisualStudioVersionExtensions.VS_ALL == vsVersions[0])
            {
                output = VisualStudioVersionExtensions.GetAllVisualStudioVersions();
            }
            else
            {
                output = new VisualStudioVersion[vsVersions.Length];
                for (int iVsVersion = 0; iVsVersion < vsVersions.Length; iVsVersion++)
                {
                    output[iVsVersion] = VisualStudioVersionExtensions.FromDefault(vsVersions[iVsVersion]);
                }
            }

            return output;
        }

        private static string GetAllVsVersionStrings()
        {
            string[] vsVersionStrings = VisualStudioVersionExtensions.GetAllVisualStudioVersionStrings();

            string output = vsVersionStrings.Concatenate(Configuration.VisualStudioVersionTokenSeparator);
            return output;
        }

        private static bool HandleCreateNewSolutionSet(Configuration configuration, IOutputStream outputStream, string[] functionArgs)
        {
            bool output = true;

            try
            {
                int numberOfArgs = functionArgs.Length;
                if (2 < numberOfArgs || 9 > numberOfArgs)
                {
                    string solutionTypeStr = functionArgs[0];
                    string solutionName = functionArgs[1];
                    string projectTypeStr = functionArgs[2];
                    string domainName = numberOfArgs > 3 ? functionArgs[3] : CommonDomain.DomainName;
                    string repositoryName = numberOfArgs > 4 ? functionArgs[4] : PublicRepository.RepositoryName;
                    string vsVersionStr = numberOfArgs > 5 ? functionArgs[5] : VisualStudioVersionExtensions.VS2010;
                    string organizationName = numberOfArgs > 6 ? functionArgs[6] : MinexOrganization.OrganizationName;
                    string organizationsDirectoryPath = numberOfArgs > 7 ? functionArgs[7] : OrganizationalPaths.DefaultOrganizationsDirectoryPath;

                    SolutionType solutionType = SolutionTypeExtensions.FromDefault(solutionTypeStr);
                    ProjectType projectType = ProjectTypeExtensions.FromDefault(projectTypeStr);
                    VisualStudioVersion vsVersion = VisualStudioVersionExtensions.FromDefault(vsVersionStr);

                    configuration.Action = () =>
                    {
                        NewSolutionSpecification specification = new NewSolutionSpecification(
                            organizationsDirectoryPath,
                            organizationName,
                            repositoryName,
                            domainName,
                            solutionType,
                            solutionName,
                            projectType,
                            vsVersion,
                            Language.CSharp);

                        Construction.CreateNewSolutionSet(specification);
                    };
                }
                else
                {
                    string message = String.Format(@"Invalid number of arguments found: {0}. Expected between 3 and 8.", numberOfArgs);
                    throw new ArgumentException(message);
                }
            }
            catch (Exception ex)
            {
                output = false;

                outputStream.WriteLineAndBlankLine(ex.Message);

                Configuration.DisplayCreateNewSolutionSetUsage(outputStream);
            }

            return output;
        }

        private static void DisplayCreateNewSolutionSetUsage(IOutputStream outputStream)
        {
            string programName = Constants.ProgramName;
            string functionName = Configuration.CreateNewSolutionSetFunctionName;
            string line = String.Format(@"{0} {1}", programName, functionName);
            outputStream.WriteLine(line);
            outputStream.WriteLine();

            line = String.Format(@"Usage: {0}.exe {1} SolutionType SolutionName ProjectType [Domain=Common] [Repository=Public] [DefaultVisualStudioVersion=VS2010] [Organization=Minex] [OrganizationsDirectory=C:\Organizations] ...", programName, functionName);
            outputStream.WriteLine(line);
            outputStream.WriteLine();

            outputStream.WriteLine(@"Example:");
            line = String.Format(@"{0}.exe {1} Script Avon Console Common Public VS2015 Minex C:\Organizations", programName, functionName);
            outputStream.WriteLine(line);
            outputStream.WriteLine();

            Configuration.DisplaySolutionTypes(outputStream);
            Configuration.DisplayProjectTypes(outputStream);
        }

        private static void DisplayUsage(IOutputStream outputStream)
        {
            string line;

            string programName = Constants.ProgramName;
            outputStream.WriteLine(programName);
            outputStream.WriteLine();

            line = String.Format(@"Usage: {0}.exe FunctionName FunctionArg1 [FunctionArg2] ...", programName);
            outputStream.WriteLine(line);
            outputStream.WriteLine();

            string[] functionNames = Configuration.GetAllFunctionNames();
            outputStream.WriteLine(@"Available functions:");
            foreach (string functionName in functionNames)
            {
                outputStream.WriteLine(functionName);
            }
            outputStream.WriteLine();
        }

        private static void DisplayProjectTypes(IOutputStream outputStream)
        {
            outputStream.WriteLine(@"Project Types:");

            string[] solutionTypes = ProjectTypeExtensions.GetAllStrings();
            string list = solutionTypes.Concatenate(", ");

            string line = String.Format(@"[{0}]", list);
            outputStream.WriteLine(line);
            outputStream.WriteLine();
        }

        private static void DisplaySolutionTypes(IOutputStream outputStream)
        {
            outputStream.WriteLine(@"Solution Types:");

            string[] solutionTypes = SolutionTypeExtensions.GetAllStrings();
            string list = solutionTypes.Concatenate(", ");

            string line = String.Format(@"[{0}]", list);
            outputStream.WriteLine(line);
            outputStream.WriteLine();
        }

        private static string[] GetAllFunctionNames()
        {
            string[] output = new string[]
            {
                Configuration.CreateNewSolutionSetFunctionName,
                Configuration.CreateSolutionSetFromInitialVsVersionSolutionFunctionName,
                Configuration.DistributeChangesFromDefaultVsVersionSolutionFunctionName,
                Configuration.DistributeChangesFromSpecificVsVersionSolutionFunctionName,
                Configuration.EnsureVsVersionedBinAndObjPropertiesFunctionName,
                Configuration.SetDefaultVsVersionSolutionFunctionName,
            };

            return output;
        }

        #endregion


        public Action Action { get; set; }


        public Configuration()
        {
        }
    }
}
