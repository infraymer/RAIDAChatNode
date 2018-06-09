using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RAIDAChatNode.DTO;
using RAIDAChatNode.Utils;
using RAIDAChatNode.Model;
using RAIDAChatNode.Model.Entity;

namespace RAIDAChatNode.Reflections
{
    public class RollbackTransaction : IReflectionActions
    {
        public OutputSocketMessageWithUsers Execute(object val, string myLogin)
        {
            #region Тестовые данные
            /*
            {
                "execFun": "rollbacktransaction",
                "data": {
                    "transactionId": "80f7efc032dd4a7c97f69fca51ad3001"
                }
            }
            */
            #endregion

            OutputSocketMessage output = new OutputSocketMessage("rollbackTransaction", true, "", new { });
            OutputSocketMessageWithUsers rez = new OutputSocketMessageWithUsers();

            TransactionInfo info = DeserializeObject.ParseJSON<TransactionInfo>(val, output, out rez);
            if (info == null)
            {
                return rez;
            }
            Guid transId = info.transactionId;

            output.data = new { transactionId = transId };
            //long dateNow = DateTimeOffset.Now.ToUnixTimeSeconds();
            long dateNow = SystemClock.GetInstance().CurrentTime;

            using (var db = new RaidaContext())
            {
                Members owner = db.Members.FirstOrDefault(it => it.login == myLogin);

                if (!db.Transactions.Any(it => it.transactionId == transId))
                {
                    output.success = false;
                    output.msgError = "This transaction is not found";
                }
                else if (!(db.Transactions.Any(it => it.transactionId == transId && it.owner == owner)))
                {
                    output.success = false;
                    output.msgError = "You not owner for this transaction";
                }
                else if (!(db.Transactions.Any(it => it.transactionId == transId && it.owner == owner && it.rollbackTime > dateNow)))
                {
                    output.success = false;
                    output.msgError = "This transaction is blocked";
                }
                else
                {
                    Transaction.rollbackTransaction(transId, owner);
                }
            }

            rez.msgForOwner = output;
            return rez;
        }
    }
}
