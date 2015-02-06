using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terminator
{
    class SystemMessage
    {
        static void DeleteRecord(string FileName, string MsgId, SqlConnection conn)
        {
            string sql = "DELETE FROM SystemMessage WHERE FileName = '" + FileName + "' AND MsgId = '" + MsgId + "'";
            SqlCommand command = new SqlCommand(sql, conn);
            command.ExecuteNonQuery();
        }

        static void ModifyRecord(string FileName, string MsgId, string Message, SqlConnection conn)
        {
            DeleteRecord(FileName, MsgId, conn);
            string sql = "INSERT INTO SystemMessage (MsgId, MessageDesc, MessageText, FileName, CreatedBy) " +
                "VALUES ('" + MsgId + "', '" + Message + "', '" + Message + "', '" + FileName + "', 'Terminator' )";
            //command.CommandText = sql;
            //command.ExecuteNonQuery();
        }
    }

}
