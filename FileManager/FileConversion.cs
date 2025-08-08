using NPrng;
using NPrng.Generators;
using System.Globalization;
using System.IO.Compression;
using System.Numerics;
using System.Text;

namespace FileManager
{
    /// <summary>
    /// Contains functions for encoding and decoding a text file.
    /// </summary>
    public static class FileConversion
    {
        #region Constants
        private static readonly BigInteger MaxModuloValue = new BigInteger(ulong.MaxValue) + 1;
        #endregion

        #region Public functions
        /// <param name="fileLine">The line that will be in the file.</param>
        /// <inheritdoc cref="EncodeFile(IEnumerable{string}, long, string, string, int, Encoding?, bool)"/>
        public static void EncodeFile(
            string fileLine,
            long seed = 1,
            string filePath = "file*",
            string fileExt = "savc",
            int version = 2,
            Encoding? encoding = null,
            bool zip = true
        )
        {
            EncodeFile([fileLine], seed, filePath, fileExt, version, encoding, zip);
        }

        /// <summary>
        /// Creates a file that has been encoded by a seed.<br/>
        /// version numbers:<br/>
        /// - 1: normal: weak<br/>
        /// - 2: secure: stronger<br/>
        /// - 3: super-secure: strogest(only works, if opened on the same location, with the same name)<br/>
        /// - 4: stupid secure: v3 but encription "expires" on the next day
        /// </summary>
        /// <param name="fileLines">The list of lines that will be in the file.</param>
        /// <param name="seed">The seed for encoding the file.</param>
        /// <param name="filePath">The path and the name of the file without the extension, that will be created. If the path contains a *, it will be replaced with the seed.</param>
        /// <param name="fileExt">The extension of the file that will be created.</param>
        /// <param name="version">The encription version.</param>
        /// <param name="encoding">The encoding of the input lines. By default it uses the UTF8 encoding. You shouldn't need to change this.</param>
        /// <param name="zip">Whether to zip the lines before encoding them.</param>
        public static void EncodeFile(
            IEnumerable<string> fileLines,
            long seed = 1,
            string filePath = "file*",
            string fileExt = "savc",
            int version=2,
            Encoding? encoding = null,
            bool zip = true
        )
        {
            static string NowTimeToNumberString()
            {
                var dateTimeStr = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                return dateTimeStr.Replace("/", "").Replace(" ", "").Replace(":", "");
            }
            
            encoding ??= Encoding.UTF8;
            var r = MakeRandom(MakeSeed(seed));

            using var f = File.Create($"{filePath.Replace(Utils.FILE_NAME_SEED_REPLACE_STRING, seed.ToString())}.{fileExt}");
            // v1
            if (version == 1)
            {
                WriteLine(f, EncodeLine("1", r, encoding));
                WriteLine(f, EncodeLine("-1", r, encoding));
                WriteLine(f, EncodeLine(Convert.ToInt32(zip).ToString(), r, encoding));
                var rr = MakeRandom(MakeSeed(seed));
                foreach (var line in fileLines)
                {
                    WriteLine(f, EncodeLine(line, rr, encoding, zip));
                }
                return;
            }

            //other versions
            //  version number
            WriteLine(f, EncodeLine(version.ToString(), r, encoding));

            //  intermediate seed
            BigInteger seedNum;
            // v2
            if (version == 2)
            {
                seedNum = BigInteger.Parse(NowTimeToNumberString()) / MakeSeed(seed, 17, 0.587);
            }
            // v3-4
            else if (version is 3 or 4)
            {
                var path = AppContext.BaseDirectory + $"{filePath.Replace(Utils.FILE_NAME_SEED_REPLACE_STRING, seed.ToString())}.{fileExt}";
                var pathBytes = Encoding.UTF8.GetBytes(path);
                var pathNum = 1;
                foreach (var by in pathBytes)
                {
                    pathNum *= by;
                    pathNum = int.Parse(pathNum.ToString().Replace("0", ""));
                }
                var nowNum = decimal.Parse(NowTimeToNumberString()) / (decimal)MakeSeed(seed, 2, 0.587);
                seedNum = new BigInteger(decimal.Parse((pathNum * nowNum).ToString().Replace("0", "").Replace("E+", "")) * 15439813);
            }
            else
            {
                seedNum = MakeSeed(seed);
            }
            WriteLine(f, EncodeLine(seedNum.ToString(), r, encoding));

            //  is zipped
            WriteLine(f, EncodeLine(Convert.ToInt32(zip).ToString(), r, encoding));
            //  write data
            // v4
            if (version == 4)
            {
                var now = DateTime.Now;
                seedNum *= now.Year + now.Month + now.Day;
            }
            var mainRandom = MakeRandom(seedNum);
            foreach (var line in fileLines)
            {
                WriteLine(f, EncodeLine(line, mainRandom, encoding, zip));
            }
        }

        /// <summary>
        /// Returns a list of strings, decoded fron the encoded file.<br/>
        /// </summary>
        /// <param name="seed">The seed for decoding the file.</param>
        /// <param name="filePath">The path and the name of the file without the extension, that will be decoded. If the path contains a *, it will be replaced with the seed.</param>
        /// <param name="fileExt">The extension of the file that will be decoded.</param>
        /// <param name="decodeUntil">Controlls how many lines the function should decode(strarting from the beginning, with 1). If it is set to -1, it will decode all the lines in the file.</param>
        /// <param name="encoding">The encoding of the output lines. By default it uses the UTF8 encoding. You shouldn't need to change this.</param>
        public static List<string> DecodeFile(
            long seed = 1,
            string filePath = "file*",
            string fileExt = "savc",
            int decodeUntil = -1,
            Encoding? encoding = null
        )
        {
            encoding ??= Encoding.UTF8;
            //get lines
            var fileBytes = File.ReadAllBytes($"{filePath.Replace(Utils.FILE_NAME_SEED_REPLACE_STRING, seed.ToString())}.{fileExt}");

            if (fileBytes.Length == 0)
            {
                throw new FormatException("The file is empty.");
            }

            var byteLines = new List<List<byte>>();
            var newL = new List<byte>();
            foreach (var by in fileBytes)
            {
                newL.Add(by);
                if (by == 10)
                {
                    byteLines.Add(newL);
                    newL = [];
                }
            }

            // get version
            var r = MakeRandom(MakeSeed(seed));
            var version = int.Parse(DecodeLine(byteLines.ElementAt(0), r, encoding, tooLongReturn: "-1"));
            var seedNum = BigInteger.Parse(DecodeLine(byteLines.ElementAt(1), r, encoding));
            var isZippedLineExists = false;
            var isZipped = false;
            if (byteLines.Count > 2)
            {
                try
                {
                    var isZippedLine = DecodeLine(byteLines.ElementAt(2), r, encoding, tooLongReturn: "-1");
                    isZipped = int.Parse(isZippedLine) == 1;
                    isZippedLineExists = isZippedLine != "-1";
                }
                catch (Exception)
                {

                }
            }
            // decode
            if (version == -1)
            {
                throw new FormatException("The seed of the file cannot be decoded.");
            }

            var linesDecoded = new List<string>();
            if (version == 4)
            {
                var now = DateTime.Now;
                seedNum *= now.Year + now.Month + now.Day;
            }
            else if (version is < 2 or > 4)
            {
                seedNum = MakeSeed(seed);
            }
            var mainRandom = MakeRandom(seedNum);
            var infoLineNum = isZippedLineExists ? 3 : 2;
            for (var x = infoLineNum; x < byteLines.Count; x++)
            {
                if (decodeUntil > -1 && x >= decodeUntil + infoLineNum)
                {
                    break;
                }
                linesDecoded.Add(DecodeLine(byteLines.ElementAt(x), mainRandom, encoding, isZipped));
            }
            return linesDecoded;
        }
        #endregion

        #region Private functions
        /// <summary>
        /// Zip the to-be encoded line's byte array.
        /// </summary>
        /// <param name="bytes">The bytes to zip.</param>
        public static byte[] Zip(byte[] bytes)
        {
            using var msi = new MemoryStream(bytes);
            using var mso = new MemoryStream();
            using (var gs = new GZipStream(mso, CompressionMode.Compress))
            {
                msi.CopyTo(gs);
            }

            return mso.ToArray();
        }

        /// <summary>
        /// Unzip the decoded line array.
        /// </summary>
        /// <param name="bytes">The bytes to unzip.</param>
        public static byte[] Unzip(byte[] bytes)
        {
            using var msi = new MemoryStream(bytes);
            using var mso = new MemoryStream();
            using (var gs = new GZipStream(msi, CompressionMode.Decompress))
            {
                gs.CopyTo(mso);
            }

            return mso.ToArray();
        }

        /// <summary>
        /// Writes a list of bytes to a file stream.
        /// </summary>
        /// <param name="file">The file stream to use.</param>
        /// <param name="byteList">The list of bytes to write.</param>
        private static void WriteLine(FileStream file, IEnumerable<byte> byteList)
        {
            var bytes = byteList.ToArray();
            file.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Encodes text into a list of bytes.
        /// </summary>
        /// <param name="text">The text to encode.</param>
        /// <param name="rand">A random number generator from NPrng.</param>
        /// <param name="encoding">The encoding that the text is in.</param>
        /// <param name="zip">Whether to zip the line before encoding.</param>
        /// <returns>The list of encoded bytes.</returns>
        private static List<byte> EncodeLine(
            string text,
            AbstractPseudoRandomGenerator rand,
            Encoding encoding,
            bool zip = false
        )
        {
            var encode64 = rand.GenerateInRange(2, 5);
            // encoding into bytes
            var lineEnc = encoding.GetBytes(text);
            // change encoding to utf-8
            var lineUtf8 = Encoding.Convert(encoding, Encoding.UTF8, lineEnc);
            if (zip)
            {
                lineUtf8 = Zip(lineUtf8);
            }
            // encode to base64 x times
            var lineBase64 = lineUtf8;
            for (int x = 0; x < encode64; x++)
            {
                lineBase64 = Encoding.UTF8.GetBytes(Convert.ToBase64String(lineBase64));
            }
            // shufling bytes
            var lineEncoded = new List<byte>();
            foreach (var byteBase64 in lineBase64)
            {
                var modByte = (byte)(byteBase64 + (int)rand.GenerateInRange(-32, 134));
                lineEncoded.Add(modByte);
            }
            // + \n
            lineEncoded.Add(10);
            return lineEncoded;
        }

        /// <summary>
        /// Decodes a list of bytes into text.
        /// </summary>
        /// <param name="bytes">The list of bytes.</param>
        /// <param name="rand">A random number generator from NPrng.</param>
        /// <param name="encoding">The encoding that the text is in.</param>
        /// <param name="unzip">Whether to unzip the line before encoding.</param>
        /// <param name="tooLongReturn">If not null and the encoded version of the line is longer than 100 characters, it retuns this instead.</param>
        /// <returns>The decoded text.</returns>
        private static string DecodeLine(
            IEnumerable<byte> bytes,
            AbstractPseudoRandomGenerator rand,
            Encoding encoding,
            bool unzip = false,
            string? tooLongReturn = null
        )
        {
            var encode64 = rand.GenerateInRange(2, 5);
            // deshufling bytes
            var lineDecoded = new List<byte>();
            foreach (var lineByte in bytes)
            {
                if (lineByte != 10)
                {
                    var modByte = (byte)(lineByte - (int)rand.GenerateInRange(-32, 134));
                    lineDecoded.Add(modByte);
                }
            }
            // encode to base64 x times
            var lineUtf8 = lineDecoded.ToArray();
            for (int x = 0; x < encode64; x++)
            {
                if (tooLongReturn is not null && lineUtf8.Length > 100)
                {
                    return tooLongReturn;
                }
                var e1 = lineUtf8.ToArray();
                var e2 = Encoding.UTF8.GetString(e1);
                lineUtf8 = Convert.FromBase64String(e2);
            }
            if (unzip)
            {
                lineUtf8 = Unzip(lineUtf8);
            }
            // change encoding from utf-8
            var lineBytes = Encoding.Convert(Encoding.UTF8, encoding, lineUtf8);
            // decode into string
            return encoding.GetString(lineBytes);
        }

        /// <summary>
        /// Generates a seed number from another number.
        /// </summary>
        /// <param name="baseSeed">The base seed to use to generate the seed.</param>
        /// <param name="powNum">A number to use to generate the seed.</param>
        /// <param name="plusNum">A number to use to generate the seed.</param>
        private static BigInteger MakeSeed(long baseSeed, int powNum = 73, double plusNum = 713853.587)
        {
            var seedA = new BigInteger(Math.Abs(baseSeed));
            var pi = new BigInteger(Math.PI);
            return (BigInteger.Pow(seedA * pi, powNum) * (new BigInteger(plusNum) + seedA * pi)).Sqrt();
        }

        /// <summary>
        /// Generates a random number generator from another number.
        /// </summary>
        /// <param name="seed">The number to use to generate the random number generator.</param>
        private static SplittableRandom MakeRandom(BigInteger seed)
        {
            return new SplittableRandom((ulong)(BigInteger.Abs(seed) % MaxModuloValue));
        }
        #endregion
    }
}
