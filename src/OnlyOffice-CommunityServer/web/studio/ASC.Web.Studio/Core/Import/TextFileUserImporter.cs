/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace ASC.Web.Studio.Core.Import
{

    public class MultiFormatTextFileUserImporter : TextFileUserImporter
    {
        public MultiFormatTextFileUserImporter(Stream stream)
            : base(stream)
        {
        }

        public MultiFormatTextFileUserImporter(string csvText)
            : base(csvText)
        {
        }

        protected override ContactInfo GetExportedUser(string line, IDictionary<int, PropertyInfo> mappedProperties, int fieldsCount)
        {
            try
            {
                var address = new MailAddress(line);
                var info = new ContactInfo { Email = address.Address };

                if (!string.IsNullOrEmpty(address.DisplayName))
                {
                    if (address.DisplayName.Contains(' '))
                    {
                        //Try split
                        info.FirstName = address.DisplayName.Split(' ')[0];
                        info.LastName = address.DisplayName.Split(' ')[1];
                    }
                    else
                    {
                        info.FirstName = address.DisplayName;
                    }
                }
                return info;

            }
            catch (Exception)
            {
                //thats bad. Failed to parse an address
            }
            return base.GetExportedUser(line, mappedProperties, fieldsCount);
        }
    }

    public class TextFileUserImporter : IUserImporter
    {
        private readonly Stream stream;
        private readonly string text;
        private readonly Encoding encoding;

        protected Dictionary<string, string> NameMapping { get; set; }
        protected IList<string> ExcludeList { get; private set; }

        public char[] Separators { get; set; }

        public bool HasHeader { get; set; }

        public string TextDelmiter { get; set; }

        public string DefaultHeader { get; set; }


        public TextFileUserImporter(Stream stream)
        {
            this.stream = stream;
            try
            {
                encoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.ANSICodePage);
            }
            catch
            {
                encoding = Encoding.UTF8;
            }
            HasHeader = false;
            Separators = new[] { ';', ',' };
            TextDelmiter = "\"";
            ExcludeList = new List<string> { "ID", "Status" };
        }

        public TextFileUserImporter(string csvText)
        {
            this.encoding = Encoding.UTF8;
            text = csvText;
            HasHeader = false;
            Separators = new[] { ';', ',' };
            TextDelmiter = "\"";
            ExcludeList = new List<string> { "ID", "Status" };
        }

        public IEnumerable<ContactInfo> GetDiscoveredUsers()
        {
            var users = new List<ContactInfo>();

            var fileLines = new List<string>();
            if (stream != null)
            {
                using (var reader = new StreamReader(stream, encoding, true))
                {
                    fileLines.AddRange(reader.ReadToEnd().Split(new[] { Environment.NewLine, "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
                }
            }
            else
            {
                fileLines.AddRange(text.Split(new[] { Environment.NewLine, "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
            }

            if (!string.IsNullOrEmpty(DefaultHeader))
            {
                fileLines.Insert(0, DefaultHeader);
            }

            if (0 < fileLines.Count)
            {
                var mappedProperties = new Dictionary<int, PropertyInfo>();
                var infos = typeof(ContactInfo).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var fieldsCount = GetFieldsMapping(fileLines[0], infos, mappedProperties);

                for (int i = 1; i < fileLines.Count; i++)
                {
                    users.Add(GetExportedUser(fileLines[i], mappedProperties, fieldsCount));
                }
            }
            return users;
        }

        protected virtual ContactInfo GetExportedUser(string line, IDictionary<int, PropertyInfo> mappedProperties, int fieldsCount)
        {
            var exportedUser = new ContactInfo();
            var dataFields = GetDataFields(line);
            for (int j = 0; j < Math.Min(fieldsCount, dataFields.Length); j++)
            {
                var propinfo = mappedProperties[j];
                if (propinfo != null)
                {
                    var value = ConvertFromString(dataFields[j], propinfo.PropertyType);
                    if (value != null)
                    {
                        value = Regex.Replace(value.ToString(), "(^')|(^\")|(\"$)|('$)", String.Empty);

                        propinfo.SetValue(exportedUser, value, null);
                    }
                }
            }

            try
            {
                if (string.IsNullOrEmpty(exportedUser.FirstName) && string.IsNullOrEmpty(exportedUser.LastName) && !string.IsNullOrEmpty(exportedUser.Email))
                {
                    var username = exportedUser.Email.Contains('@') ? exportedUser.Email.Substring(0, exportedUser.Email.IndexOf('@')) : exportedUser.Email;
                    if (username.Contains('.'))
                    {
                        exportedUser.FirstName = username.Split('.')[0];
                        exportedUser.LastName = username.Split('.')[1];
                    }
                }
            }
            catch { }

            return exportedUser;
        }

        private string[] GetDataFields(string line)
        {
            var pattern = String.Format("[{0}](?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", string.Join("|", Array.ConvertAll(Separators, c => c.ToString())));
            var result = Regex.Split(line, pattern);

            return Array.ConvertAll<string, string>(result,
                original =>
                {
                    return original.StartsWith(TextDelmiter) && original.EndsWith(TextDelmiter) ?
                        original.Substring(1, original.Length - 2) :
                        original;
                }
             );
        }

        private int GetFieldsMapping(string firstLine, IEnumerable<PropertyInfo> infos, IDictionary<int, PropertyInfo> mappedProperties)
        {
            var fields = firstLine.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                //Find apropriate field in UserInfo
                foreach (var info in infos)
                {
                    var propertyField = field.Trim();
                    propertyField = propertyField.Trim('"');
                    if (NameMapping != null && NameMapping.ContainsKey(propertyField))
                    {
                        propertyField = NameMapping[propertyField];
                    }
                    if (!string.IsNullOrEmpty(propertyField) && !ExcludeList.Contains(propertyField) && propertyField.Equals(info.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        //Add to map
                        mappedProperties.Add(i, info);
                    }
                }
                if (!mappedProperties.ContainsKey(i))
                {
                    //No property was found
                    mappedProperties.Add(i, null);
                }
            }

            if (!mappedProperties.Values.Any(p => p != null))
            {
                mappedProperties[2] = infos.First(p => p.Name == "Email");
                mappedProperties[0] = infos.First(p => p.Name == "FirstName");
                mappedProperties[1] = infos.First(p => p.Name == "LastName");
            }

            return fields.Length;
        }

        private static object ConvertFromString(string value, Type type)
        {
            var converter = TypeDescriptor.GetConverter(type);
            return converter != null && converter.CanConvertFrom(typeof(string)) ? converter.ConvertFromString(value) : null;
        }
    }
}