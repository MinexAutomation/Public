﻿using System;
using System.Text;


namespace Public.Common.Lib.Code.Physical
{
    /// <summary>
    /// Sets the encoding to UTF-8 with byte-order-mark.
    /// </summary>
    public class WriteCodeFile : WriteFile
    {
        public const string DefaultIndentSegment = @"    ";


        private StringBuilder zBuilder { get; set; }
        public string Indent { get; private set; }
        public string IndentSegment { get; set; }


        public WriteCodeFile()
            : this(null)
        {
        }

        public WriteCodeFile(string path)
            : base(path, new UTF8Encoding(true))
        {
            this.zBuilder = new StringBuilder();

            this.DecreaseIndent();
        }

        public void IncreaseIndent()
        {
            this.zBuilder.Append(this.IndentSegment);

            this.Indent = this.zBuilder.ToString();
        }

        public void DecreaseIndent()
        {
            if(0 < this.zBuilder.Length)
            {
                this.zBuilder.Remove(this.zBuilder.Length - this.IndentSegment.Length, this.IndentSegment.Length);

                this.Indent = this.zBuilder.ToString();
            }
        }

        public void WriteBlankIndentedLine()
        {
            this.Writer.WriteLine(this.Indent);
        }

        public void WriteIndentedLine(char chr)
        {
            this.Writer.Write(this.Indent);
            this.Writer.WriteLine(chr);
        }

        public void WriteIndentedLine(string line)
        {
            this.Writer.Write(this.Indent);
            this.Writer.WriteLine(line);
        }

        public void WriteIndented(char chr)
        {
            this.Writer.Write(this.Indent);
            this.Writer.Write(chr);
        }

        public void WriteIndented(string str)
        {
            this.Writer.Write(this.Indent);
            this.Writer.Write(str);
        }
    }
}
