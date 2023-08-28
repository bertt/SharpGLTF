using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGLTF.Schema2
{
    public static class BinaryTable
    {
        public static byte[] GetStringAsBytesWithOffsetBuffer(List<string> strings)
        {
            var offsetBuffer = GetOffsetBuffer(strings);
            var offsetBytes = GetIntsAsBytes(offsetBuffer);
            var stringBytes = GetStringsAsBytes(strings);

            return offsetBytes.Concat(stringBytes).ToArray();
        }

        public static byte[] GetIntsAsBytes(List<uint> values)
        {
            var dstData = new Byte[values.Count * 4];
            var dstArray = new Memory.IntegerArray(dstData, IndexEncodingType.UNSIGNED_INT);
            for (int i = 0; i < values.Count; ++i) { dstArray[i] = values[i]; }
            return dstData;
        }


        private static List<uint> GetOffsetBuffer(List<string> strings)
        {
            var offsets = new List<uint>() { 0 };
            foreach (string s in strings)
            {
                var length = (uint)Encoding.UTF8.GetByteCount(s);

                offsets.Add(offsets.Last() + length);
            }
            return offsets;
        }

        private static byte[] GetStringsAsBytes(List<string> values)
        {
            var res = String.Join("", values);
            return Encoding.UTF8.GetBytes(res);
        }

    }
}
