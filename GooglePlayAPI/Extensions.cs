using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GooglePlayApi
{
    internal static class Extensions
    {
        public static T[] SubArray<T>(this T[] array, int offset, int length)
        {
            T[] result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }

        public static void Add(this HttpRequestHeaders target, Dictionary<string, string> source)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            foreach (var element in source)
                target.Add(element.Key, element.Value);
        }

        // https://stackoverflow.com/a/3982463/5299903
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            foreach (var element in source)
                target.Add(element);
        }

        // https://stackoverflow.com/a/53453722/5299903
        //public static Task<Stream> ReadAsStreamAsync(this HttpContent content, bool isChunked)
        //{
        //    if (!isChunked)
        //    {
        //        return content.ReadAsStreamAsync();
        //    }
        //    else
        //    {
        //        var task = content.ReadAsStreamAsync()
        //        .ContinueWith<Stream>((streamTask) =>
        //        {
        //            var outputStream = new MemoryStream();
        //            var buffer = new char[1024 * 1024];
        //            var stream = streamTask.Result;

        //        // No using() so that we don't dispose stream.
        //        var tr = new StreamReader(stream);
        //            var tw = new StreamWriter(outputStream);

        //            while (!tr.EndOfStream)
        //            {
        //                var chunkSizeStr = tr.ReadLine().Trim();
        //                var chunkSize = int.Parse(chunkSizeStr, System.Globalization.NumberStyles.HexNumber);

        //                tr.ReadBlock(buffer, 0, chunkSize);
        //                tw.Write(buffer, 0, chunkSize);
        //                tr.ReadLine();
        //            }

        //            return outputStream;
        //        });

        //        return task;
        //    }
        //}
    }
}
