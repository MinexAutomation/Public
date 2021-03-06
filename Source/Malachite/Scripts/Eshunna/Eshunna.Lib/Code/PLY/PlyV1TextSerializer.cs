﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Public.Common.Lib;
using Public.Common.Lib.Extensions;
using Public.Common.Lib.IO;
using Public.Common.Lib.IO.Serialization;


namespace Eshunna.Lib.PLY
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Assumptions:
    /// *. All comment lines will be at the top of the file, just below the file format line, and above the first element definition.
    /// </remarks>
    public class PlyV1TextSerializer : IFileSerializer<PlyFile>
    {
        #region Static

        public static readonly char[] Separators = new char[] { ' ' };


        public static bool TryEnsureFileMarker(LineReader reader, out string fileMarkerFound)
        {
            // Ensure we have a PLY file.
            fileMarkerFound = reader.ReadLine();

            bool output = PlyFile.PlyFileMarker == fileMarkerFound;
            return output;
        }

        public static void EnsureFileMarker(LineReader reader)
        {
            bool fileMarkerEnsured = PlyV1TextSerializer.TryEnsureFileMarker(reader, out string fileMarkerFound);
            if (!fileMarkerEnsured)
            {
                string message = $@"PLY file marker not found. Found: {fileMarkerFound}, expected: {PlyFile.PlyFileMarker}";
                throw new InvalidDataException(message);
            }
        }

        public static PlyFile Deserialize(string filePath)
        {
            using (LineReader reader = new LineReader(filePath))
            {
                PlyV1TextSerializer.EnsureFileMarker(reader);

                // Read the header.
                PlyFileHeader header = PlyV1TextSerializer.DeserializeHeader(reader);

                // Build the output.
                PlyFile output = PlyFile.Build(header);

                // Build the value converters.
                ValueConverter[][] elementValueConverters = PlyFile.BuildValueConverters(output);

                // Fill the output.
                int nElements = header.Elements.Count;
                for (int iElement = 0; iElement < nElements; iElement++)
                {
                    var elementDescriptor = header.Elements[iElement];
                    var element = output.Values[elementDescriptor.Name];
                    ValueConverter[] valueConverters = elementValueConverters[iElement];

                    int nProperties = elementDescriptor.PropertyDescriptors.Count;
                    int nElementValues = elementDescriptor.Count;
                    for (int iElementValue = 0; iElementValue < nElementValues; iElementValue++)
                    {
                        string elementLine = reader.ReadLine();
                        string[] tokens = elementLine.Split(PlyV1TextSerializer.Separators, StringSplitOptions.None);
                        int nTokens = tokens.Length;
                        int iToken = 0;
                        for (int iProperty = 0; iProperty < nProperties; iProperty++)
                        {
                            PlyPropertyDescriptor propertyDescriptor = elementDescriptor.PropertyDescriptors[iProperty];
                            ValueConverter valueConverter = valueConverters[iProperty];
                            if (propertyDescriptor.IsList)
                            {
                                string listLengthToken = tokens[iToken];
                                int listLength = PlyV1TextSerializer.GetListLength(listLengthToken, propertyDescriptor.ListLengthValueDataType);
                                string[] listTokens = tokens.Copy(iToken + 1, iToken + listLength);
                                iToken += (listLength + 1);

                                valueConverter.ConvertAndAdd(listTokens, iElementValue);
                            }
                            else
                            {
                                string token = tokens[iToken];
                                iToken++;

                                valueConverter.ConvertAndAdd(token, iElementValue);
                            }
                        }
                    }
                }

                return output;
            }
        }

        private static int GetListLength(string token, PlyDataType listLengthValueDataType)
        {
            int output;
            switch(listLengthValueDataType)
            {
                default:
                    output = Convert.ToInt32(token);
                    break;
            }
            return output;
        }
        
        public static PlyFileHeader DeserializeHeader(LineReader reader)
        {
            PlyFileDataFormat fileDataFormat = PlyV1TextSerializer.DeserializeFormat(reader, out Version fileFormatVersion);

            PlyFileHeader output = new PlyFileHeader()
            {
                FileDataFormat = fileDataFormat,
                FileFormatVersion = fileFormatVersion,
            };

            string line = reader.ReadLine();

            while(PlyFile.commentKeyword == line.Substring(0, PlyFile.commentKeyword.Length))
            {
                string comment = line.Substring(PlyFile.commentKeyword.Length + 1);
                output.Comments.Add(comment);

                line = reader.ReadLine();
            }
            
            while(PlyFile.PlyFileHeaderEnd != line.Substring(0, PlyFile.PlyFileHeaderEnd.Length))
            {
                PlyElementDescriptor elementDescriptor = PlyV1TextSerializer.DeserializeElementDescriptor(ref line, reader);
                output.Elements.Add(elementDescriptor);
            }

            return output;
        }

        public static string SerializeComment(string comment)
        {
            string output = $@"{PlyFile.commentKeyword} {comment}";
            return output;
        }

        private static PlyElementDescriptor DeserializeElementDescriptor(ref string line, LineReader reader)
        {
            var elementTokens = line.Split(PlyV1TextSerializer.Separators, StringSplitOptions.None);

            string elementKeywordToken = elementTokens[0];
            if (PlyFile.elementKeyword != elementKeywordToken)
            {
                string message = $@"Invalid element keyword token found. Expected: {PlyFile.elementKeyword}, found: {elementKeywordToken}.";
                throw new InvalidDataException(message);
            }
            string elementNameToken = elementTokens[1];
            string countToken = elementTokens[2];

            int count = Convert.ToInt32(countToken);

            PlyElementDescriptor output = new PlyElementDescriptor()
            {
                Count = count,
                Name = elementNameToken,
            };

            line = reader.ReadLine();
            while(PlyFile.propertyKeyword == line.Substring(0, PlyFile.propertyKeyword.Length))
            {
                var propertyTokens = line.Split(PlyV1TextSerializer.Separators, StringSplitOptions.None);
                string propertyKeywordToken = propertyTokens[0];
                if (PlyFile.propertyKeyword != propertyKeywordToken)
                {
                    string message = $@"Invalid property keyword token found. Expected: {PlyFile.propertyKeyword}, found: {propertyKeywordToken}.";
                    throw new InvalidDataException(message);
                }
                bool isList = propertyTokens[1] == PlyFile.listKeyword;
                PlyDataType listLengthValueDataType;
                string dataTypeToken;
                string propertyNameToken;
                if(isList)
                {
                    string listLengthValueDataTypeToken = propertyTokens[2];
                    dataTypeToken = propertyTokens[3];
                    propertyNameToken = propertyTokens[4];

                    listLengthValueDataType = listLengthValueDataTypeToken.ToPlyDataType();
                }
                else
                {
                    dataTypeToken = propertyTokens[1];
                    propertyNameToken = propertyTokens[2];

                    listLengthValueDataType = PlyDataType.None;
                }

                PlyDataType dataType = dataTypeToken.ToPlyDataType();

                PlyPropertyDescriptor propertyDescriptor = new PlyPropertyDescriptor(propertyNameToken, dataType, isList, listLengthValueDataType);
                output.PropertyDescriptors.Add(propertyDescriptor);

                line = reader.ReadLine();
            }

            return output;
        }

        public static void SerializeElementDescriptor(StreamWriter writer, PlyElementDescriptor elementDescriptor)
        {
            string elementLine = $@"{PlyFile.elementKeyword} {elementDescriptor.Name} {elementDescriptor.Count.ToString()}";
            writer.WriteLine(elementLine);

            foreach(var propertyDescriptor in elementDescriptor.PropertyDescriptors)
            {
                PlyV1TextSerializer.SerializePropertyDescriptor(writer, propertyDescriptor);
            }
        }

        private static void SerializePropertyDescriptor(StreamWriter writer, PlyPropertyDescriptor propertyDescriptor)
        {
            string propertyLine;
            if (propertyDescriptor.IsList)
            {
                propertyLine = $@"{PlyFile.propertyKeyword} {PlyFile.listKeyword} {propertyDescriptor.ListLengthValueDataType.ToPlyFileDataTypeToken()} {propertyDescriptor.DataType.ToPlyFileDataTypeToken()} {propertyDescriptor.Name}";
            }
            else
            {
                propertyLine = $@"{PlyFile.propertyKeyword} {propertyDescriptor.DataType.ToPlyFileDataTypeToken()} {propertyDescriptor.Name}";
            }
            writer.WriteLine(propertyLine);
        }

        private static PlyFileDataFormat DeserializeFormat(LineReader reader, out Version fileVersion)
        {
            string formatLine = reader.ReadLine();
            if(6 > formatLine.Length || PlyFile.formatKeyword != formatLine.Substring(0, 6))
            {
                throw new InvalidDataException(@"Invalid PLY file format line.");
            }

            var tokens = formatLine.Split(PlyV1TextSerializer.Separators, StringSplitOptions.None);
            string formatKeywordToken = tokens[0];
            string fileDataFormatToken = tokens[1];
            string versionToken = tokens[2];

            fileVersion = Version.Parse(versionToken);

            PlyFileDataFormat output = fileDataFormatToken.ToPlyFileDataFormat();
            return output;
        }

        public static void SerializeFormat(StreamWriter writer, PlyFileDataFormat plyFileDataFormat, Version version)
        {
            var line = $@"{PlyFile.formatKeyword} {plyFileDataFormat.ToPlyFileDataFormatToken()} {version.ToString()}";
            writer.WriteLine(line);
        }

        public static void Serialize(string filePath, PlyFile plyFile, bool overwrite = true)
        {
            FileMode fileMode = FileMode.Create;
            if (!overwrite)
            {
                fileMode = FileMode.CreateNew;
            }

            using (FileStream fStream = new FileStream(filePath, fileMode)) // Will allow the familiar error to percolate upwards if no overwriting.
            using (StreamWriter writer = new StreamWriter(fStream)) // For text writing.
            {
                writer.WriteLine(PlyFile.PlyFileMarker);

                PlyFileHeader header = plyFile.Header;

                PlyV1TextSerializer.SerializeFormat(writer, header.FileDataFormat, header.FileFormatVersion);

                foreach(var comment in header.Comments)
                {
                    string commentLine = PlyV1TextSerializer.SerializeComment(comment);
                    writer.WriteLine(commentLine);
                }

                foreach(var elementDescriptor in header.Elements)
                {
                    PlyV1TextSerializer.SerializeElementDescriptor(writer, elementDescriptor);
                }

                writer.WriteLine(PlyFile.PlyFileHeaderEnd);

                // Build the value converters.
                ValueConverter[][] elementValueConverters = PlyFile.BuildValueConverters(plyFile);

                // Now write values.
                StringBuilder builder = new StringBuilder();
                int nElements = header.Elements.Count;
                for (int iElement = 0; iElement < nElements; iElement++)
                {
                    var elementDescriptor = header.Elements[iElement];
                    ValueConverter[] valueConverters = elementValueConverters[iElement];

                    int nElementValues = elementDescriptor.Count;
                    int nProperties = elementDescriptor.PropertyDescriptors.Count;
                    for (int iElementValue = 0; iElementValue < nElementValues; iElementValue++)
                    {
                        builder.Clear();
                        for (int iProperty = 0; iProperty < nProperties; iProperty++)
                        {
                            var propertyDescriptor = elementDescriptor.PropertyDescriptors[iProperty];
                            ValueConverter valueConverter = valueConverters[iProperty];

                            if (propertyDescriptor.IsList)
                            {
                                int length = valueConverter.GetListLength(iElementValue);
                                string lengthAppendix = $@"{length.ToString()} ";
                                builder.Append(lengthAppendix);
                            }
                            string value = valueConverter.ToString(iElementValue);
                            string appendix = $@"{value} ";
                            builder.Append(appendix);
                        }
                        builder.RemoveLast();

                        string line = builder.ToString();
                        writer.WriteLine(line);
                    }
                }
            }
        }

        #endregion


        public PlyFile this[string filePath, bool overwrite = true]
        {
            get
            {
                var output = PlyV1TextSerializer.Deserialize(filePath);
                return output;
            }
            set
            {
                PlyV1TextSerializer.Serialize(filePath, value, overwrite);
            }
        }
    }
}
