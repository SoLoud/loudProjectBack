using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace HttpUtils
{
    public class HttpMultipartParser
    {
        public HttpMultipartParser(Stream stream)
            : this(stream, Encoding.UTF8)
        {
        }

        public HttpMultipartParser(Stream stream, Encoding encoding)
        {
            this.Encoding = encoding;
            this.Parse(stream);
        }

        private void Parse(Stream stream)
        {
            this.Success = false;

            foreach (var section in getSections(stream))
            {
                if (!isValidMultipartSection(section.AsString)) continue;

                if (isFile(section.AsString))
                {
                    var File = new SoLoud.Models.File();
                    var success = tryGetFile(section, out File);

                    if (success)
                        Files.Add(File);
                }
                else //is Parameter
                {
                    string parameterName, parameterValue;
                    var success = tryGetParameter(section, out parameterName, out parameterValue);

                    if (success)
                        Parameters.Add(parameterName, parameterValue);
                }
            }

            if (Files.Count != 0 || Parameters.Count != 0)
                this.Success = true;
        }

        private bool isValidMultipartSection(string section)
        {
            return section.Contains("Content-Disposition");
        }

        private bool isFile(string section)
        {
            Regex re = new Regex(@"(?<=Content\-Type: )(?<ContentType>.*?)(?=\r\n)");
            Match contentTypeMatch = re.Match(section);

            if(!contentTypeMatch.Success) return false;

            if (contentTypeMatch.Groups["ContentType"].Value.StartsWith("text/plain")) return false;

            return true;
        }

        private class Section
        {
            public string AsString { get; set; }
            public byte[] AsBytes { get; set; }
        }

        private List<Section> getSections(Stream stream)
        {
            var sections = new List<Section>();

            var data = Misc.ToByteArray(stream);

            string content = Encoding.GetString(data);

            // The first line should contain the delimiter
            int delimiterEndIndex = content.IndexOf("\r\n");

            if (delimiterEndIndex == -1) return sections;

            delimiter = content.Substring(0, delimiterEndIndex);

            foreach (var sectionData in Misc.Split(data, Encoding.GetBytes(delimiter)))
            {
                var section = new Section()
                {
                    AsBytes = sectionData,
                    AsString = Encoding.GetString(sectionData)
                };

                sections.Add(section);
            }

            return sections;
        }

        private bool tryGetParameter(Section Section, out string Name, out string Value)
        {
            Name = ""; Value = "";

            Regex parameterNameRegex = new Regex(@"(?<=name=\"")(?<Name>.*?)(?>\""\r\n)");
            Regex parameterRegex = new Regex(@"(?>\r\n\r\n)(?<Value>.*?)(?=\r\n)");

            Match parameterNameMatch = parameterNameRegex.Match(Section.AsString);
            Match parameterMatch = parameterRegex.Match(Section.AsString);

            if (!parameterMatch.Success || !parameterNameMatch.Success) return false;

            Name = parameterNameMatch.Groups["Name"].Value;
            Value = parameterMatch.Groups["Value"].Value;

            return true;
        }

        private bool tryGetFile(Section Section, out SoLoud.Models.File File)
        {
            File = new SoLoud.Models.File();

            Regex fileNameRegex = new Regex(@"(?<=filename=\"")(?<FileName>.*?)(?>\""\r\n)", RegexOptions.Singleline);
            Regex contentTypeRegex = new Regex(@"(?<=Content\-Type:[\s]+)(?<ContentType>.*?)(?>\r\n)", RegexOptions.Singleline);
            Regex contentsRegex = new Regex(@"(?>\r\n\r\n)(?<Contents>.*)", RegexOptions.Singleline);

            Match fileNameMatch = fileNameRegex.Match(Section.AsString);
            Match contentTypeMatch = contentTypeRegex.Match(Section.AsString);
            Match contentsMatch = contentsRegex.Match(Section.AsString);

            if (!fileNameMatch.Success || !contentTypeMatch.Success || !contentsMatch.Success) return false;

            var FileName = fileNameMatch.Groups["FileName"].Value;
            var ContentType = contentTypeMatch.Groups["ContentType"].Value;

            //This is the old way to get the content length. Which is wrong. This assumes that Every character in the encoding we are using is 1 byte. 
            //That is not true in some encodings especialy for foreign languagues like greek. So the ContentStartIndex is not the string index for the last capturing group.
            //var ContentStartIndex = FileMatch.Groups["Contents"].Index;

            var stringBeforeTheContents = Section.AsString.Substring(0, contentsMatch.Groups["Contents"].Index);
            var ContentStartIndex = Encoding.GetByteCount(stringBeforeTheContents);

            var ContentLength = Section.AsBytes.Length - ContentStartIndex;

            File.Content = new byte[ContentLength];
            Array.Copy(Section.AsBytes, ContentStartIndex, File.Content, 0, ContentLength);

            File.ContentType = ContentType;
            File.FileName = FileName;

            return true;
        }

        public IDictionary<string, string> Parameters = new Dictionary<string, string>();

        public bool Success { get; private set; }

        public List<SoLoud.Models.File> Files = new List<SoLoud.Models.File>();

        private string delimiter { get; set; }

        public Encoding Encoding { get; private set; }
    }
}