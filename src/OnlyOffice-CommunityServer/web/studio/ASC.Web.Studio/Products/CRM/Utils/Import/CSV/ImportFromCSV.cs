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


using ASC.Common.Threading.Progress;
using ASC.CRM.Core;
using ASC.Web.Studio.Utility;
using LumenWorks.Framework.IO.Csv;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ASC.Web.CRM.Classes
{

    public class ImportFromCSV
    {
        #region Members

        private static readonly Object _syncObj = new Object();

        private static readonly ProgressQueue _importQueue = new ProgressQueue(3, TimeSpan.FromSeconds(15), true);

        public static readonly int MaxRoxCount = 10000;

        #endregion

        public static int GetQuotas()
        {
            return MaxRoxCount;
        }

        public static CsvReader CreateCsvReaderInstance(Stream CSVFileStream, ImportCSVSettings importCsvSettings)
        {
            var result = new CsvReader(
                new StreamReader(CSVFileStream, importCsvSettings.Encoding, true),
                importCsvSettings.HasHeader, importCsvSettings.DelimiterCharacter, importCsvSettings.QuoteType, '"', '#', ValueTrimmingOptions.UnquotedOnly) { SkipEmptyLines = true, SupportsMultiline = true, DefaultParseErrorAction = ParseErrorAction.AdvanceToNextLine, MissingFieldAction = MissingFieldAction.ReplaceByEmpty };

            return result;
        }

        public static String GetRow(Stream CSVFileStream, int index, String jsonSettings)
        {
            var importCSVSettings = new ImportCSVSettings(jsonSettings);

            using (CsvReader csv = CreateCsvReaderInstance(CSVFileStream, importCSVSettings))
            {
                int countRows = 0;

                index++;

                while (countRows++ != index && csv.ReadNextRecord()) ;

                return new JObject(new JProperty("data", new JArray(csv.GetCurrentRowFields(false).ToArray())),
                                   new JProperty("isMaxIndex", csv.EndOfStream)).ToString();
            }
        }

        public static JObject GetInfo(Stream CSVFileStream, String jsonSettings)
        {
            var importCSVSettings = new ImportCSVSettings(jsonSettings);

            using (CsvReader csv = CreateCsvReaderInstance(CSVFileStream, importCSVSettings))
            {
                csv.ReadNextRecord();

                var firstRowFields = csv.GetCurrentRowFields(false);

                String[] headerRowFields = csv.GetFieldHeaders().ToArray();

                if (!importCSVSettings.HasHeader)
                    headerRowFields = firstRowFields;

                return new JObject(
                    new JProperty("headerColumns", new JArray(headerRowFields)),
                    new JProperty("firstContactFields", new JArray(firstRowFields)),
                    new JProperty("isMaxIndex", csv.EndOfStream)
                    );
            }
        }

        public static IProgressItem GetStatus(EntityType entityType)
        {
            var result = _importQueue.GetStatus(String.Format("{0}_{1}", TenantProvider.CurrentTenantID, (int)entityType));

            if (result == null)
            {
                return ImportDataCache.Get(entityType);
            }

            return result;
        }

        public static IProgressItem Start(EntityType entityType, String CSVFileURI, String importSettingsJSON)
        {
            lock (_syncObj)
            {
                var operation = GetStatus(entityType);

                if (operation == null)
                {
                    var fromCache = ImportDataCache.Get(entityType);

                    if (fromCache != null)
                        return fromCache;

                    operation = new ImportDataOperation(entityType, CSVFileURI, importSettingsJSON);

                    _importQueue.Add(operation);
                }

                if (!_importQueue.IsStarted)
                    _importQueue.Start(x => x.RunJob());

                return operation;
            }
        }
    }
}