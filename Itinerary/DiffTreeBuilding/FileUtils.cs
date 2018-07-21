using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Itinerary.DiffTreeBuilding
{
    public static class FileUtils
    {
        public static bool FileContentsAreEqual(string fileA, string fileB)
        {
            if (fileA == null || fileB == null)
                return false;

            if (new FileInfo(fileA).Length != new FileInfo(fileB).Length)
                return false;
            var checksumA = GetChecksum(fileA);
            var checksumB = GetChecksum(fileB);
            return checksumA.SequenceEqual(checksumB);
        }

        private static IEnumerable<byte> GetChecksum(string filename)
        {
            using (var stream = File.OpenRead(filename))
            {
                var sha = new SHA256Managed();
                return sha.ComputeHash(stream);
            }
        }
    }
}
