using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Izenda.BI.CacheProvider.RedisCache.Utilities
{
    public static class StringCompressionExtension
    {
        /// <summary>
        /// Compressess the string
        /// </summary>
        /// <param name="value">The original value</param>
        /// <returns>The compressed value</returns>
        public static string Compress(this string value)
        {
            var inputBytes = Encoding.UTF8.GetBytes(value);
            var newValue = string.Empty;

            using (var outputStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    gzipStream.Write(inputBytes, 0, inputBytes.Length);
                }

                var outputBytes = outputStream.ToArray();
                newValue = Convert.ToBase64String(outputBytes);
            }

            return newValue;
        }

        /// <summary>
        /// Decompresses the string
        /// </summary>
        /// <param name="value">The compressed value</param>
        /// <returns>The original value</returns>
        public static string Decompress(this string value)
        {
            var inputBytes = Convert.FromBase64String(value);
            var newValue = string.Empty;

            using (var inputStream = new MemoryStream(inputBytes))
            using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(gzipStream))
            {
                newValue = streamReader.ReadToEnd();
            }

            return newValue;
        }

        /// <summary>
        /// Decompresses the string into a stream reader
        /// </summary>
        /// <param name="value">The compressed value</param>
        /// <returns>The original value's stream reader</returns>
        public static StreamReader DecompressToStreamReader(this string value)
        {
            var inputBytes = Convert.FromBase64String(value);

            MemoryStream inputStream = new MemoryStream(inputBytes);
            GZipStream gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);
            StreamReader streamReader = new StreamReader(gzipStream);

            return streamReader;
        }
    }
}
