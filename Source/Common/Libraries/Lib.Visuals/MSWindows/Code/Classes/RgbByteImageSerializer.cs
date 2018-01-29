﻿using System.Drawing;
using SysImageFormat = System.Drawing.Imaging.ImageFormat;

using Public.Common.Lib.IO.Serialization;
using Public.Common.Lib.Logging;
using LoggingUtilities = Public.Common.Lib.Logging.Utilities;


namespace Public.Common.Lib.Visuals.MSWindows
{
    /// <summary>
    /// De/serializes RGB byte images to/from all formats supported by the MS Windows Bitmap object.
    /// </summary>
    public class RgbByteImageSerializer : IInstrumentedFileSerializer<RgbByteImage>
    {
        #region Static

        public static RgbByteImage Deserialize(string filePath, LoggingTiming loggingTimimg = default(LoggingTiming))
        {
            // Ensure we have a logger. From a static context, use the session log.
            var logger = Loggers.GetLoggerOrDefault(loggingTimimg.SessionLogger);

            // Timing node.
            var deserialize = StopwatchTimingNode.GetList($@"Deserialize RGB Byte Image: {filePath}", loggingTimimg.TimingNode);

            // Load the bitmap.
            logger.Info($@"Deserializing to RGB Byte Image: {filePath}...");

            var loadBitmap = StopwatchTimingNode.GetLeaf(@"Load image.", deserialize);

            Bitmap bitmap = new Bitmap(filePath);
            loadBitmap.Stop();

            LoggingUtilities.LogDuration(logger, loadBitmap, Level.Info, $@"Loaded image. Size: rows (height): {bitmap.Height.ToString()}, rolumns (width): {bitmap.Width.ToString()}.");

            // Create the output.
            logger.Info(@"Creating RGB Byte Image...");

            var createRgbByteImage = StopwatchTimingNode.GetLeaf(@"Create RGB Byte Image.", deserialize);

            RgbByteImage output = BitmapConverter.ToRgbByteImage(bitmap);

            createRgbByteImage.Stop();

            LoggingUtilities.LogDuration(logger, createRgbByteImage, Level.Info, @"Created RGB byte image.");

            deserialize.Stop();

            LoggingUtilities.LogDuration(logger, deserialize, Level.Info, $@"Deserialized RGB byte image: {filePath}");

            return output;
        }

        public static void Serialize(string filePath, RgbByteImage image, LoggingTiming logggingTimimg = default(LoggingTiming), bool overwrite = true)
        {
            int rows = image.Rows;
            int columns = image.Columns;

            Bitmap bitmap = BitmapConverter.ToBitmap(image);

            // Determine the image format.
            SysImageFormat imageFormat = FileExtensions.DetermineImageFormatFromFileExtension(filePath);

            bitmap.Save(filePath, imageFormat);
        }

        #endregion


        private ILogger Logger { get; }


        public RgbByteImageSerializer(ILogger logger)
        {
            this.Logger = logger;

            this.Logger.Info($@"Created new {nameof(RgbByteImageSerializer)}");
        }

        public RgbByteImageSerializer()
            :this(Loggers.GetDefaultLogger())
        {
        }

        public RgbByteImage this[string filePath, LoggingTiming loggingTiming = default(LoggingTiming), bool overwrite = true]
        {
            get
            {
                string endMessage = $@"Deserialized {filePath}";
                RgbByteImage output = LoggingUtilities.FunctionLogDuration(this.Logger, () => RgbByteImageSerializer.Deserialize(filePath, loggingTiming), Level.Info, endMessage);
                return output;
            }
            set
            {
                string endMessage = $@"Serialized {filePath}";
                LoggingUtilities.ActionLogDuration(this.Logger, () => RgbByteImageSerializer.Serialize(filePath, value, loggingTiming, overwrite), Level.Info, endMessage);
            }
        }
    }
}
