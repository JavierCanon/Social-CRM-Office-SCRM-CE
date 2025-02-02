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
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ASC.Common.Data.Sql;

namespace ASC.Common.Data
{
    public static class DataExtensions
    {
        public static List<object[]> ExecuteList(this DbConnection connection, string sql, params object[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                return command.ExecuteList(sql, parameters);
            }
        }

        public static T ExecuteScalar<T>(this DbConnection connection, string sql, params object[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                return command.ExecuteScalar<T>(sql, parameters);
            }
        }

        public static int ExecuteNonQuery(this DbConnection connection, string sql, params object[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                return command.ExecuteNonQuery(sql, parameters);
            }
        }

        public static DbCommand CreateCommand(this DbConnection connection, string sql, params object[] parameters)
        {
            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.AddParameters(parameters);
            return command;
        }


        public static DbCommand AddParameter(this DbCommand command, string name, object value)
        {
            var p = command.CreateParameter();
            if (!string.IsNullOrEmpty(name))
            {
                p.ParameterName = name.StartsWith("@") ? name : "@" + name;
            }

            p.Value = GetParameterValue(value);

            command.Parameters.Add(p);
            return command;
        }

        public static object GetParameterValue(object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }

            var @enum = value as Enum;
            if (@enum != null)
            {
                return @enum.ToString("d");
            }

            if (value is DateTime)
            {
                var d = (DateTime)value;
                return new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second, DateTimeKind.Unspecified);
            }
            return value;
        }

        public static DbCommand AddParameters(this DbCommand command, params object[] parameters)
        {
            if (parameters == null) return command;
            foreach (var value in parameters)
            {
                if (value != null && IsAnonymousType(value.GetType()))
                {
                    foreach (var p in value.GetType().GetProperties())
                    {
                        command.AddParameter(p.Name, p.GetValue(value, null));
                    }
                }
                else
                {
                    command.AddParameter(null, value);
                }
            }
            return command;
        }

        public static List<object[]> ExecuteList(this DbCommand command)
        {
            return ExecuteList(command, command.CommandText, null);
        }

        public static Task<List<object[]>> ExecuteListAsync(this DbCommand command)
        {
            return ExecuteListAsync(command, command.CommandText, null);
        }

        public static List<object[]> ExecuteList(this DbCommand command, string sql, params object[] parameters)
        {
            return ExecuteListReader(command.PrepareCommand(sql, parameters));
        }

        public static Task<List<object[]>> ExecuteListAsync(this DbCommand command, string sql, params object[] parameters)
        {
            return ExecuteListReaderAsync(command.PrepareCommand(sql, parameters));
        }

        private static List<object[]> ExecuteListReader(DbCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                return ExecuteListReaderResult(reader);
            }
        }

        private static async Task<List<object[]>> ExecuteListReaderAsync(DbCommand command)
        {
            using (var reader = await command.ExecuteReaderAsync())
            {
                return ExecuteListReaderResult(reader);
            }
        }

        private static List<object[]> ExecuteListReaderResult(IDataReader reader)
        {
            var result = new List<object[]>();
            var fieldCount = reader.FieldCount;
            while (reader.Read())
            {
                var row = new object[fieldCount];
                for (var i = 0; i < fieldCount; i++)
                {
                    row[i] = reader[i];
                    if (DBNull.Value.Equals(row[i])) row[i] = null;
                }
                result.Add(row);
            }
            return result;
        }

        public static T ExecuteScalar<T>(this DbCommand command)
        {
            return ExecuteScalar<T>(command, command.CommandText, null);
        }

        public static T ExecuteScalar<T>(this DbCommand command, string sql, params object[] parameters)
        {
            command.PrepareCommand(sql, parameters);

            var scalar = command.ExecuteScalar();

            if (scalar == null || scalar == DBNull.Value)
            {
                return default(T);
            }
            var scalarType = typeof(T);
            if (scalarType == typeof(object))
            {
                return (T)scalar;
            }
            if (scalarType.Name == "Nullable`1")
            {
                scalarType = scalarType.GetGenericArguments()[0];
            }
            return (T)Convert.ChangeType(scalar, scalarType);
        }

        public static async Task<T> ExecuteScalarAsync<T>(this DbCommand command, string sql, params object[] parameters)
        {
            command.PrepareCommand(sql, parameters);

            var scalar = await command.ExecuteScalarAsync();

            if (scalar == null || scalar == DBNull.Value)
            {
                return default(T);
            }
            var scalarType = typeof(T);
            if (scalarType == typeof(object))
            {
                return (T)scalar;
            }
            if (scalarType.Name == "Nullable`1")
            {
                scalarType = scalarType.GetGenericArguments()[0];
            }
            return (T)Convert.ChangeType(scalar, scalarType);
        }

        public static int ExecuteNonQuery(this DbCommand command, string sql, params object[] parameters)
        {
            command.PrepareCommand(sql, parameters);
            return command.ExecuteNonQuery();
        }

        public static Task<int> ExecuteNonQueryAsync(this DbCommand command, string sql, params object[] parameters)
        {
            command.PrepareCommand(sql, parameters);
            return command.ExecuteNonQueryAsync();
        }

        public static List<object[]> ExecuteList(this DbCommand command, ISqlInstruction sql, ISqlDialect dialect)
        {
            ApplySqlInstruction(command, sql, dialect);
            return command.ExecuteList();
        }

        public static Task<List<object[]>> ExecuteListAsync(this DbCommand command, ISqlInstruction sql, ISqlDialect dialect)
        {
            ApplySqlInstruction(command, sql, dialect);
            return command.ExecuteListAsync();
        }

        public static List<T> ExecuteList<T>(this DbCommand command, ISqlInstruction sql, ISqlDialect dialect, Converter<IDataRecord, T> mapper)
        {
            ApplySqlInstruction(command, sql, dialect);
            var result = new List<T>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(mapper(reader));
                }
            }
            return result;
        }

        public static T ExecuteScalar<T>(this DbCommand command, ISqlInstruction sql, ISqlDialect dialect)
        {
            ApplySqlInstruction(command, sql, dialect);
            return command.ExecuteScalar<T>();
        }

        public static int ExecuteNonQuery(this DbCommand command, ISqlInstruction sql, ISqlDialect dialect)
        {
            ApplySqlInstruction(command, sql, dialect);
            return command.ExecuteNonQuery();
        }

        private static void ApplySqlInstruction(DbCommand command, ISqlInstruction sql, ISqlDialect dialect)
        {
            var sqlStr = sql.ToString(dialect);
            var parameters = sql.GetParameters();
            command.Parameters.Clear();

            var sqlParts = sqlStr.Split('?');
            var sqlBuilder = new StringBuilder();
            for (var i = 0; i < sqlParts.Length - 1; i++)
            {
                var name = "p" + i;
                command.AddParameter(name, parameters[i]);
                sqlBuilder.AppendFormat("{0}@{1}", sqlParts[i], name);
            }
            sqlBuilder.Append(sqlParts[sqlParts.Length - 1]);
            command.CommandText = sqlBuilder.ToString();
        }


        public static T Get<T>(this IDataRecord r, int i)
        {
            if (r.IsDBNull(i))
            {
                return default(T);
            }

            var value = r.GetValue(i);
            if (typeof(T) == typeof(Guid))
            {
                value = r.GetGuid(i);
            }
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static T Get<T>(this IDataRecord r, string name)
        {
            return Get<T>(r, r.GetOrdinal(name));
        }

        private static bool IsAnonymousType(Type type)
        {
            return type.IsGenericType
                   && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic
                   && (type.Name.StartsWith("<>", StringComparison.OrdinalIgnoreCase) || type.Name.StartsWith("VB$", StringComparison.OrdinalIgnoreCase))
                   && (type.Name.Contains("AnonymousType") || type.Name.Contains("AnonType"))
                   && Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false);
        }

        private static DbCommand PrepareCommand(this DbCommand command, string sql, params object[] parameters)
        {
            command.CommandText = sql;
            if (parameters != null)
            {
                command.Parameters.Clear();
                command.AddParameters(parameters);
            }
            return command;
        }
    }
}