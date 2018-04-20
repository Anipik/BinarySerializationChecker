using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BinarySerializationChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            string storedBLob = "AAEAAAD/////AQAAAAAAAAAEAQAAABtTeXN0ZW0uQ3VsdHVyZUF3YXJlQ29tcGFyZXICAAAADF9jb21wYXJlSW5mbwtfaWdub3JlQ2FzZQMAIFN5c3RlbS5HbG9iYWxpemF0aW9uLkNvbXBhcmVJbmZvAQkCAAAAAQQCAAAAIFN5c3RlbS5HbG9iYWxpemF0aW9uLkNvbXBhcmVJbmZvBAAAAAZtX25hbWUJd2luMzJMQ0lEB2N1bHR1cmUNbV9Tb3J0VmVyc2lvbgEAAAMICCBTeXN0ZW0uR2xvYmFsaXphdGlvbi5Tb3J0VmVyc2lvbgYDAAAAAAAAAAB/AAAACgs=";

            Console.WriteLine(ToBase64String(StringComparer.InvariantCultureIgnoreCase));
            string[] blobs = new string[] { "AAEAAAD/////AQAAAAAAAAAEAQAAABtTeXN0ZW0uQ3VsdHVyZUF3YXJlQ29tcGFyZXICAAAADF9jb21wYXJlSW5mbwtfaWdub3JlQ2FzZQMAIFN5c3RlbS5HbG9iYWxpemF0aW9uLkNvbXBhcmVJbmZvAQkCAAAAAQQCAAAAIFN5c3RlbS5HbG9iYWxpemF0aW9uLkNvbXBhcmVJbmZvAwAAAAZtX25hbWUNbV9Tb3J0VmVyc2lvbgdjdWx0dXJlAQMAIFN5c3RlbS5HbG9iYWxpemF0aW9uLlNvcnRWZXJzaW9uCAYDAAAAAAp/AAAACw==", "AAEAAAD/////AQAAAAAAAAAEAQAAABtTeXN0ZW0uQ3VsdHVyZUF3YXJlQ29tcGFyZXICAAAADF9jb21wYXJlSW5mbwtfaWdub3JlQ2FzZQMAIFN5c3RlbS5HbG9iYWxpemF0aW9uLkNvbXBhcmVJbmZvAQkCAAAAAQQCAAAAIFN5c3RlbS5HbG9iYWxpemF0aW9uLkNvbXBhcmVJbmZvBAAAAAZtX25hbWUJd2luMzJMQ0lEB2N1bHR1cmUNbV9Tb3J0VmVyc2lvbgEAAAMICCBTeXN0ZW0uR2xvYmFsaXphdGlvbi5Tb3J0VmVyc2lvbgYDAAAAAAAAAAB/AAAACgs=", "AAEAAAD/////AQAAAAAAAAAEAQAAABtTeXN0ZW0uQ3VsdHVyZUF3YXJlQ29tcGFyZXIDAAAADF9jb21wYXJlSW5mbwhfb3B0aW9ucwtfaWdub3JlQ2FzZQMDACBTeXN0ZW0uR2xvYmFsaXphdGlvbi5Db21wYXJlSW5mbyNTeXN0ZW0uR2xvYmFsaXphdGlvbi5Db21wYXJlT3B0aW9ucwEJAgAAAAT9////I1N5c3RlbS5HbG9iYWxpemF0aW9uLkNvbXBhcmVPcHRpb25zAQAAAAd2YWx1ZV9fAAgBAAAAAQQCAAAAIFN5c3RlbS5HbG9iYWxpemF0aW9uLkNvbXBhcmVJbmZvAwAAAAZtX25hbWUNbV9Tb3J0VmVyc2lvbgdjdWx0dXJlAQMAIFN5c3RlbS5HbG9iYWxpemF0aW9uLlNvcnRWZXJzaW9uCAYEAAAAAAp/AAAACw==", "AAEAAAD/////AQAAAAAAAAAEAQAAABtTeXN0ZW0uQ3VsdHVyZUF3YXJlQ29tcGFyZXIDAAAADF9jb21wYXJlSW5mbwtfaWdub3JlQ2FzZQhfb3B0aW9ucwMAAyBTeXN0ZW0uR2xvYmFsaXphdGlvbi5Db21wYXJlSW5mbwEjU3lzdGVtLkdsb2JhbGl6YXRpb24uQ29tcGFyZU9wdGlvbnMJAgAAAAEE/f///yNTeXN0ZW0uR2xvYmFsaXphdGlvbi5Db21wYXJlT3B0aW9ucwEAAAAHdmFsdWVfXwAIAQAAAAQCAAAAIFN5c3RlbS5HbG9iYWxpemF0aW9uLkNvbXBhcmVJbmZvBAAAAAZtX25hbWUJd2luMzJMQ0lEB2N1bHR1cmUNbV9Tb3J0VmVyc2lvbgEAAAMICCBTeXN0ZW0uR2xvYmFsaXphdGlvbi5Tb3J0VmVyc2lvbgYEAAAAAAAAAAB/AAAACgs=" };

            test(StringComparer.InvariantCultureIgnoreCase, blobs);
            Console.ReadLine();
        }

        public static byte[] ToByteArray(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static string ToBase64String(object obj)
        {
            byte[] raw = ToByteArray(obj);
            return Convert.ToBase64String(raw);
        }
        private static void SanityCheckBlob(object obj, string[] blobs)
        {
            int frameworkBlobNumber = blobs.Length - 1;
            if (frameworkBlobNumber < blobs.Length)
            {
                string runtimeBlob = ToBase64String(obj);

                string storedComparableBlob = CreateComparableBlobInfo(blobs[frameworkBlobNumber]);
                string runtimeComparableBlob = CreateComparableBlobInfo(runtimeBlob);

                if (storedComparableBlob == runtimeComparableBlob)
                    Console.WriteLine("Sanity check passed");
                else
                    throw new Exception();
                /*                  $"The stored blob for type {obj.GetType().FullName} is outdated and needs to be updated.{Environment.NewLine}{Environment.NewLine}" +
                                  $"-------------------- Stored blob ---------------------{Environment.NewLine}" +
                                  $"Encoded: {blobs[frameworkBlobNumber].Base64Blob}{Environment.NewLine}" +
                                  $"Decoded: {storedComparableBlob}{Environment.NewLine}{Environment.NewLine}" +
                                  $"--------------- Runtime generated blob ---------------{Environment.NewLine}" +
                                  $"Encoded: {runtimeBlob}{Environment.NewLine}" +
                                  $"Decoded: {runtimeComparableBlob}");
                  */
            }
        }

        private static string CreateComparableBlobInfo(string base64Blob)
        {
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();

            byte[] data = Convert.FromBase64String(base64Blob);
            base64Blob = Encoding.UTF8.GetString(data);

            return Regex.Replace(base64Blob, @"Version=\d.\d.\d.\d.", "Version=0.0.0.0", RegexOptions.Multiline)
                .Replace("\r\n", string.Empty)
                .Replace("\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace(lineSeparator, string.Empty)
                .Replace(paragraphSeparator, string.Empty);
        }

        public static object FromByteArray(byte[] raw)
        {
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.AssemblyFormat = FormatterAssemblyStyle.Full;
            Console.WriteLine(binaryFormatter.AssemblyFormat);
            using (var serializedStream = new MemoryStream(raw))
            {
                return binaryFormatter.Deserialize(serializedStream);
            }
        }

        public static object FromBase64String(string base64Str)
        {
            byte[] raw = Convert.FromBase64String(base64Str);
            return FromByteArray(raw);
        }

        public static void CheckEquals(object objA, object objB, bool isSamePlatform)
        {
            if (objA == null && objB == null)
                return;

            if (objA != null && objB != null)
            {
                object equalityResult = null;
                Type objType = objA.GetType();


                // Check if object.Equals(object) is overridden and if not check if there is a more concrete equality check implementation
                bool equalsNotOverridden = objType.GetMethod("Equals", new Type[] { typeof(object) }).DeclaringType == typeof(object);
                if (equalsNotOverridden)
                {
                    // If type doesn't override Equals(object) method then check if there is a more concrete implementation
                    // e.g. if type implements IEquatable<T>.
                    MethodInfo equalsMethod = objType.GetMethod("Equals", new Type[] { objType });
                    if (equalsMethod.DeclaringType != typeof(object))
                    {
                        equalityResult = equalsMethod.Invoke(objA, new object[] { objB });
                        Debug.Assert((bool)equalityResult);
                        return;
                    }
                }

            }
            if (objA.Equals(objB))
            {
                Console.WriteLine("obj are equal");
            }
            else
            {
                throw new Exception();

            }
        }

        public static void test(Object obj, string[] blobs)
        {
            for (int j = 0; j < blobs.Length - 1; j++)
            {
                for (int i = 0; i < blobs.Length; i++)
                {
                    // Check if the current blob is from the current running platform.
                    bool isSamePlatform = i == j;
                    SanityCheckBlob(obj, blobs);
                    CheckEquals(obj, FromBase64String(blobs[i]), isSamePlatform);
                }
            }
        }
    }
}
