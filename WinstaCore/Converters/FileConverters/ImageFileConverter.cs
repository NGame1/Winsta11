using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;

namespace WinstaCore.Converters.FileConverters
{
    public static class ImageFileConverter
    {
        /// <summary>
        /// Converts source image file to jpeg of defined quality (0.85)
        /// </summary>
        /// <param name="sourceFile">Source StorageFile</param>
        /// <param name="outputFile">Target StorageFile</param>
        /// <returns></returns>
        public static async Task<StorageFile> ConvertImageToJpegAsync(this StorageFile sourceFile, StorageFile outputFile)
        {
            //you can use WinRTXamlToolkit StorageItemExtensions.GetSizeAsync to get file size (if you already plugged this nuget in)
            var sourceFileProperties = await sourceFile.GetBasicPropertiesAsync();
            var fileSize = sourceFileProperties.Size;
            var imageStream = await sourceFile.OpenReadAsync();
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            using (imageStream)
            {
                var decoder = await BitmapDecoder.CreateAsync(imageStream);
                var pixelData = await decoder.GetPixelDataAsync();
                var detachedPixelData = pixelData.DetachPixelData();
                pixelData = null;
                //0.85d
                double jpegImageQuality = 1;
                //since we're using MvvmCross, we're outputing diagnostic info to MvxTrace, you can use System.Diagnostics.Debug.WriteLine instead
                //Mvx.TaggedTrace(MvxTraceLevel.Diagnostic, "ImageService", $"Source image size: {fileSize}, trying Q={jpegImageQuality}");
                var imageWriteableStream = await outputFile.OpenAsync(FileAccessMode.ReadWrite);
                ulong jpegImageSize = 0;
                using (imageWriteableStream)
                {
                    var propertySet = new BitmapPropertySet();
                    var qualityValue = new BitmapTypedValue(jpegImageQuality, Windows.Foundation.PropertyType.Single);
                    propertySet.Add("ImageQuality", qualityValue);
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, imageWriteableStream, propertySet);
                    //key thing here is to use decoder.OrientedPixelWidth and decoder.OrientedPixelHeight otherwise you will get garbled image on devices on some photos with orientation in metadata
                    encoder.SetPixelData(decoder.BitmapPixelFormat, decoder.BitmapAlphaMode, decoder.OrientedPixelWidth, decoder.OrientedPixelHeight, decoder.DpiX, decoder.DpiY, detachedPixelData);
                    await encoder.FlushAsync();
                    await imageWriteableStream.FlushAsync();
                    jpegImageSize = imageWriteableStream.Size;
                }
                //Mvx.TaggedTrace(MvxTraceLevel.Diagnostic, "ImageService", $"Final image size now: {jpegImageSize}");
            }
            //stopwatch.Stop();
            //Mvx.TaggedTrace(MvxTraceLevel.Diagnostic, "ImageService", $"Time spent optimizing image: {stopwatch.Elapsed}");
            return outputFile;
        }

        public static async Task<byte[]> ConvertImageToJpegAsync(this StorageFile sourceFile)
        {
            //you can use WinRTXamlToolkit StorageItemExtensions.GetSizeAsync to get file size (if you already plugged this nuget in)
            var sourceFileProperties = await sourceFile.GetBasicPropertiesAsync();
            var fileSize = sourceFileProperties.Size;
            var imageStream = await sourceFile.OpenReadAsync();
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            using (imageStream)
            {
                var decoder = await BitmapDecoder.CreateAsync(imageStream);
                var pixelData = await decoder.GetPixelDataAsync();
                var detachedPixelData = pixelData.DetachPixelData();
                pixelData = null;
                //0.85d
                double jpegImageQuality = 1;
                //since we're using MvvmCross, we're outputing diagnostic info to MvxTrace, you can use System.Diagnostics.Debug.WriteLine instead
                //Mvx.TaggedTrace(MvxTraceLevel.Diagnostic, "ImageService", $"Source image size: {fileSize}, trying Q={jpegImageQuality}");
                var imageWriteableStream = new InMemoryRandomAccessStream();
                using (imageWriteableStream)
                {
                    var propertySet = new BitmapPropertySet();
                    var qualityValue = new BitmapTypedValue(jpegImageQuality, Windows.Foundation.PropertyType.Single);
                    propertySet.Add("ImageQuality", qualityValue);
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, imageWriteableStream, propertySet);
                    //key thing here is to use decoder.OrientedPixelWidth and decoder.OrientedPixelHeight otherwise you will get garbled image on devices on some photos with orientation in metadata
                    encoder.SetPixelData(decoder.BitmapPixelFormat, decoder.BitmapAlphaMode, decoder.OrientedPixelWidth, decoder.OrientedPixelHeight, decoder.DpiX, decoder.DpiY, detachedPixelData);
                    await encoder.FlushAsync();
                    await imageWriteableStream.FlushAsync();
                    return await FileConverter.ConvertToBytesArray(imageWriteableStream.CloneStream());
                }
                //Mvx.TaggedTrace(MvxTraceLevel.Diagnostic, "ImageService", $"Final image size now: {jpegImageSize}");
            }
            //stopwatch.Stop();
            //Mvx.TaggedTrace(MvxTraceLevel.Diagnostic, "ImageService", $"Time spent optimizing image: {stopwatch.Elapsed}");
        }

    }
}
