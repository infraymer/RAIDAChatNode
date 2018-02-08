using System;
using System.Collections.Generic;
using System.Linq;
using RAIDAChatNode.Model.Entity;
using RAIDAChatNode.Model;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

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
            long dateNow = DateTimeOffset.Now.ToUnixTimeSeconds();

            using (var db = new RaidaContext())
            {
                if (db.Transactions.Any(it => it.transactionId == transactionId && it.owner == owner && it.rollbackTime > dateNow))
                {
                    List<Transactions> trans = db.Transactions.Where(it => it.transactionId == transactionId && it.owner == owner && it.rollbackTime > dateNow).ToList();
                    trans.ForEach(delegate (Transactions t)
                    {
                        ReflectionRemove(db, t.tableName.Trim(), t.tableRowId.Trim());
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

        /// <summary>
        /// Reflection remove data from database
        /// </summary>
        /// <param name="db">DataBase context</param>
        /// <param name="entity">Name of class</param>
        /// <param name="remId">Remove key</param>
        private static void  ReflectionRemove(RaidaContext db, String entity, String remId)
        {
            //Подключаем к бд нужную таблицу
            var asob = Assembly.GetAssembly(typeof(Members));
            var tEntity = asob.GetType($"RAIDAChatNode.Model.Entity.{entity}", false, true);
            var method = db.GetType().GetMember("Set").Cast<MethodInfo>().Where(x => x.IsGenericMethodDefinition).FirstOrDefault();
            var genericMethod = method.MakeGenericMethod(tEntity);
            var invokeSet = genericMethod.Invoke(db, null);

            //Достаём контекст
            var eContext = db.GetType().GetProperty(entity).GetValue(db);

            //Получаем первичный ключ TESTING FOR MEMBERSINGROUP
            var key = tEntity.GetProperties()
                            .Where(x => x.GetCustomAttributes<KeyAttribute>() != null)
                            .FirstOrDefault();

            dynamic Id;
            if (key.PropertyType == typeof(Guid))
            {
                Id = Guid.Parse(remId);
            }
            else
            {
                Id = int.Parse(remId);
            }

            //Находим элемент
            var mFind = eContext.GetType().GetMethod("Find");
            var item = mFind.Invoke(eContext, new object[] { new object[] { Id } });
            if (item != null)
            {
                //Удаляем его
                var mRemove = eContext.GetType().GetMethod("Remove");
                mRemove.Invoke(eContext, new object[] { item });
            }
        }
    }
}
