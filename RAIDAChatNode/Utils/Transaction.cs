using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RAIDAChatNode.Model.Entity;
using RAIDAChatNode.Model;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Text;
using SQLitePCL;
using Microsoft.Data.Sqlite;

namespace RAIDAChatNode.Utils
{
    internal static class Transaction
    {
        public static void saveTransaction(RaidaContext db, Guid transactionId, String tableName, String rowId, Members owner)
        {
            const double DiffRoll = 60; //How long seconds rollback access 
            Int64 rollback = DateTimeOffset.Now.AddSeconds(DiffRoll).ToUnixTimeSeconds();

            Transactions newTransaction = new Transactions {
                transactionId = transactionId,
                tableName = tableName,
                tableRowId = rowId,
                rollbackTime = rollback,
                owner = owner
            };
            db.Transactions.Add(newTransaction);
        }

        public static void rollbackTransaction(Guid transactionId, Members owner)
        {
            String sqlPattern = @"DELETE FROM {0} WHERE {1} = @Param"; //0 - TableName, 1 - id column, 2 - remove id
            long dateNow = DateTimeOffset.Now.ToUnixTimeSeconds();

            using (var db = new RaidaContext())
            {
                if (db.Transactions.Any(it => it.transactionId == transactionId && it.owner == owner && it.rollbackTime > dateNow))
                {
                    List<Transactions> trans = db.Transactions.Where(it => it.transactionId == transactionId && it.owner == owner && it.rollbackTime > dateNow).ToList();
                    trans.ForEach(delegate (Transactions t)
                    {
                        String tablName = t.tableName.Trim();
                        String sql = String.Format(sqlPattern, tablName, TABLE_NAME.ColumnId[tablName]);
                        try
                        {
                            db.Database.ExecuteSqlCommand(sql, new SqlParameter("Param", Guid.Parse(t.tableRowId.Trim()))); //НЕ РАБОТАЕТ
                        }catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
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
        /// <param name="group">Check dialog</param>
        public static void removeMessageAboveLimit(Groups group)
        {
            const int limit = 100;
            List<Shares> extra = group.Shares.ToList();

            using (var db = new RaidaContext())
            { 
                extra.OrderByDescending(it => it.sending_date)
                    .Skip(limit)
                    .ToList()
                    .ForEach(delegate (Shares s)
                    {
                        db.Shares.Remove(s);
                    });

                db.SaveChanges();
            }
        }
    }
}
