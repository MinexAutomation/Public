﻿using System;


namespace Public.Common.Lib.Code
{
    /// <summary>
    /// The relative path of content file in a project that might be copied to the output directory.
    /// </summary>
    public class ContentProjectItem : ProjectItem
    {
        public CopyToOutputDirectory CopyToOutputDirectory { get; set; }


        public ContentProjectItem()
            : this(null)
        {
        }

        public ContentProjectItem(string includePath)
            : base(includePath)
        {
            this.CopyToOutputDirectory = CopyToOutputDirectory.Never;
        }

        public ContentProjectItem(string includePath, CopyToOutputDirectory copyToOutputDirectory)
            : base(includePath)
        {
            this.CopyToOutputDirectory = copyToOutputDirectory;
        }
    }
}
