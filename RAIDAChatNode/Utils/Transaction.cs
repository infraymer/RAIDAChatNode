using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RAIDAChatNode.Model.Entity;
using RAIDAChatNode.Model;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace RAIDAChatNode.Utils
{
    internal static class Transaction
    {
        public static void saveTransaction(Guid transactionId, String tableName, String rowId, Members owner)
        {
            const double DiffRoll = 60; //How long seconds rollback access 
            Int64 rollback = DateTimeOffset.Now.AddSeconds(DiffRoll).ToUnixTimeSeconds();

            Transactions newTransaction = new Transactions {
                transactionId = transactionId,
                tableName = tableName,
                tableRowId = rowId,
                rollbackTime = rollback,
                ownerId = owner
            };

            using (var db = new RaidaContext())
            {
                db.Transactions.Add(newTransaction);
                db.SaveChanges();
            }
        }

        public static void rollbackTransaction(Guid transactionId, Members owner)
        {
            String sqlPattern = @"DELETE FROM [{0}] WHERE {1} = @Param"; //0 - TableName, 1 - id column, 2 - remove id
            long dateNow = DateTimeOffset.Now.ToUnixTimeSeconds();

            using (var db = new RaidaContext())
            {
                if (db.Transactions.Any(it => it.transactionId == transactionId && it.ownerId == owner && it.rollbackTime > dateNow))
                {
                    List<Transactions> trans = db.Transactions.Where(it => it.transactionId == transactionId && it.ownerId == owner && it.rollbackTime > dateNow).ToList();
                    trans.ForEach(delegate (Transactions t)
                    {
                        String tablName = t.tableName.Trim();
                        String sql = String.Format(sqlPattern, tablName, TABLE_NAME.ColumnId[tablName]);
                        db.Database.ExecuteSqlCommand(sql, new SqlParameter("Param", t.tableRowId.Trim()));
                    });
                    db.SaveChanges();
                }
            }
        }

        public class TABLE_NAME
        {
            public const string MEMBERS = "Members";
            public const string GROUPS = "Groups";
            public const string MEMBERS_IN_GROUP = "MemberInGroup";
            public const string ORGANIZATIONS = "Organizations";
            public const string SHARES = "Shares";

            private static readonly Dictionary<String, String> _ColumnId = new Dictionary<String, String>() {
                { MEMBERS, "private_id" },
                { GROUPS, "group_id" },
                { MEMBERS_IN_GROUP, "Id" },
                { ORGANIZATIONS, "private_id" },
                { SHARES, "id" },
            };
            public static Dictionary<String, String> ColumnId
            {
                get { return _ColumnId; }
            }
        }

        /// <summary>
        /// Remove all message above limit
        /// </summary>
        /// <param name="ownerPrivate">Owner message id</param>
        /// <param name="toPublic">Recipient id</param>
        /// <param name="group">Group on not group</param>
        public static void removeMessageAboveLimit(Guid ownerPrivate, Guid toPublic, String group)
        {
          /*  const int limit = 50;
            List<Shares> extra;

            using (var db = new RaidaContext())
            {
                if (Boolean.Parse(group))
                {
                    extra = db.Shares.Where(it => it.self_one_or_group.Equals(group) && it.to_public == toPublic).ToList();
                }
                else
                {
                    Guid ownerPublic = db.members.Where(it => it.private_id == ownerPrivate).FirstOrDefault().public_id;
                    Guid toPrivate = db.members.Where(it => it.public_id == toPublic).FirstOrDefault().private_id;

                    extra = db.shares.Where(it => (it.self_one_or_group.Equals(group) && (
                                                                                            (it.to_public == toPublic && it.owner_private == ownerPrivate) ||
                                                                                            (it.to_public == ownerPublic && it.owner_private == toPrivate)
                                                                                        ))).ToList();
                }

                extra.OrderByDescending(it => it.sending_date)
                    .Skip(limit)
                    .ToList()
                    .ForEach(delegate (shares s)
                    {
                        db.content_over_8000.Remove(db.content_over_8000.Where(it => it.shar_id == s.id).FirstOrDefault());
                        db.shares.Remove(s);
                    });

                db.SaveChanges();
            }*/
        }
    }
}
