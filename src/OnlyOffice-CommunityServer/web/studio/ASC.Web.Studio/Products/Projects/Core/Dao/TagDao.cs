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
using System.Linq;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Projects.Core.DataInterfaces;

namespace ASC.Projects.Data.DAO
{
    class TagDao : BaseDao, ITagDao
    {
        public TagDao(int tenantID) : base(tenantID)
        {
        }

        public string GetById(int id)
        {
            return Db.ExecuteScalar<string>(Query(TagsTable).Select("title").Where("id", id));
        }

        public KeyValuePair<int, string> Create(string data)
        {
            var tagId = Db.ExecuteScalar<int>(
                Insert(TagsTable)
                    .InColumnValue("id", 0)
                    .InColumnValue("title", data)
                    .InColumnValue("last_modified_by", DateTime.UtcNow)
                    .Identity(1, 0, true));

            return new KeyValuePair<int, string>(tagId, data);
        }

        public Dictionary<int, string> GetTags()
        {
            return Db.ExecuteList(GetTagQuery()).ToDictionary(r => Convert.ToInt32(r[0]), n => n[1].ToString());
        }

        public Dictionary<int, string> GetTags(string prefix)
        {
            var query = GetTagQuery().Where(Exp.Like("title", prefix, SqlLike.StartWith));

            return Db.ExecuteList(query).ToDictionary(r => Convert.ToInt32(r[0]), n => n[1].ToString());
        }

        public int[] GetTagProjects(string tagName)
        {
            var query = new SqlQuery(ProjectTagTable)
                .Select("project_id")
                .InnerJoin(TagsTable, Exp.EqColumns("id", "tag_id") & Exp.Eq("projects_tags.tenant_id", Tenant))
                .Where(Exp.Eq("lower(title)", tagName.ToLower()))
                .Where("tenant_id", Tenant);

            return Db.ExecuteList(query).ConvertAll(r => Convert.ToInt32(r[0])).ToArray();
        }

        public int[] GetTagProjects(int tagID)
        {
            var query = new SqlQuery(ProjectTagTable)
                .Select("project_id")
                .Where("tag_id", tagID);

            return Db.ExecuteList(query).ConvertAll(r => Convert.ToInt32(r[0])).ToArray();
        }

        public Dictionary<int, string> GetProjectTags(int projectId)
        {
            var query = GetTagQuery()
                .InnerJoin(ProjectTagTable, Exp.EqColumns("id", "tag_id"))
                .Where(Exp.Eq("project_id", projectId));

            return Db.ExecuteList(query).ToDictionary(r => Convert.ToInt32(r[0]), n => n[1].ToString());
        }

        public void SetProjectTags(int projectId, string[] tags)
        {
            using (var tx = Db.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                var tagsToDelete = Db.ExecuteList(
                        new SqlQuery(ProjectTagTable).Select("tag_id").Where("project_id", projectId),
                        r => (int)r[0]);

                Db.ExecuteNonQuery(new SqlDelete(ProjectTagTable).Where("project_id", projectId));

                foreach (var tag in tagsToDelete)
                {
                    if (Db.ExecuteScalar<int>(new SqlQuery(ProjectTagTable).Select("project_id").Where("tag_id", tag)) == 0)
                    {
                        Db.ExecuteNonQuery(Delete(TagsTable).Where("id", tag));
                    }
                }


                foreach (var tag in tags)
                {
                    var tagId = Db.ExecuteScalar<int>(Query(TagsTable)
                                                         .Select("id")
                                                         .Where("lower(title)", tag.ToLower()));
                    if (tagId == 0)
                    {
                        tagId = Db.ExecuteScalar<int>(
                            Insert(TagsTable)
                                .InColumnValue("id", 0)
                                .InColumnValue("title", tag)
                                .InColumnValue("last_modified_by", DateTime.UtcNow)
                                .Identity(1, 0, true));
                    }

                    Db.ExecuteNonQuery(new SqlInsert(ProjectTagTable, true).InColumnValue("tag_id", tagId).InColumnValue("project_id", projectId));
                }
                tx.Commit();
            }
        }

        public void SetProjectTags(int projectId, IEnumerable<int> tags)
        {
            using (var tx = Db.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                Db.ExecuteNonQuery(new SqlDelete(ProjectTagTable).Where("project_id", projectId));

                var query = new SqlQuery(TagsTable + " pt")
                    .Select("DISTINCT(pt.id)")
                    .LeftOuterJoin(ProjectTagTable + " ppt", Exp.EqColumns("ppt.tag_id", "pt.id"))
                    .Where("ppt.tag_id", null);

                var tagsToDelete = Db.ExecuteList(query, r => (int)r[0]);

                foreach (var tag in tagsToDelete.Except(tags))
                {
                    if (Db.ExecuteScalar<int>(new SqlQuery(ProjectTagTable).Select("project_id").Where("tag_id", tag)) == 0)
                    {
                        Db.ExecuteNonQuery(Delete(TagsTable).Where("id", tag));
                    }
                }

                if (tags.Any())
                {
                    var insert = new SqlInsert(ProjectTagTable, true).InColumns("project_id", "tag_id");

                    foreach (var t in tags)
                    {
                        insert.Values(projectId, t);
                    }

                    Db.ExecuteNonQuery(insert);
                }

                tx.Commit();
            }
        }

        private SqlQuery GetTagQuery()
        {
            return Query(TagsTable)
                .Select("id", "title")
                .OrderBy("title", true);
        }
    }
}
