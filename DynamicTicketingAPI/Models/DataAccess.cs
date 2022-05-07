using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace DynamicTicketingAPI.Models
{
    public class DataAccess : IDisposable
    {
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public DataSet ExecuteDataSet(string strSpName, List<CommandParameter> objLstParameter = null, List<string> tableNames = null)
		{
			Database objDatabase = (Database)DatabaseFactory.CreateDatabase();

			DbCommand objDbComm = null;
			DataSet objDS = null;
			try
			{
				objDatabase = (Database)DatabaseFactory.CreateDatabase();

				//initilized the DBcommand 
				objDbComm = objDatabase.GetStoredProcCommand(strSpName);

				objDbComm.CommandTimeout = 600;

				// check if list of Parameter have some elements or not
				if (objLstParameter != null && objLstParameter.Count > 0)
				{
					//executing for loop for creating the parameter object for DBCommand and initiling the value 
					for (int iCount = 0; iCount < objLstParameter.Count; iCount++)
					{
						if (objLstParameter[iCount].Direction == PrmDirection.Input)
						{
							//set the value for input parameter
							objDatabase.AddInParameter(objDbComm, objLstParameter[iCount].Name, ReturnDBType(objLstParameter[iCount].pDbType), objLstParameter[iCount].Value);
							//objDatabase.AddInParameter(objDbComm,"",DbType.Int32
						}
						else if (objLstParameter[iCount].Direction == PrmDirection.Output)
						{
							//set the value for output parameter
							objDatabase.AddOutParameter(objDbComm, objLstParameter[iCount].Name, ReturnDBType(objLstParameter[iCount].pDbType), objLstParameter[iCount].Size);
						}
					}
				}

				//claa the Execute DataSet method returns DataSet
				objDS = objDatabase.ExecuteDataSet(objDbComm);

				if (tableNames != null)
				{
					for (int cnt = 0; cnt < objDS.Tables.Count; cnt++)
					{
						if (cnt > tableNames.Count)
						{
							break;
						}
						objDS.Tables[cnt].TableName = tableNames[cnt];
					}
				}
				return objDS;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.ToString());
			}
		}
		private DbType ReturnDBType(PrmType pType)
		{
			switch (pType)
			{
				case PrmType.AnsiString:
					return DbType.AnsiString;
				case PrmType.AnsiStringFixedLength:
					return DbType.AnsiStringFixedLength;
				case PrmType.Binary:
					return DbType.Binary;
				case PrmType.Boolean:
					return DbType.Boolean;
				case PrmType.Byte:
					return DbType.Byte;
				case PrmType.Currency:
					return DbType.Currency;
				case PrmType.Date:
					return DbType.Date;
				case PrmType.DateTime:
					return DbType.DateTime;
				case PrmType.DateTime2:
					return DbType.DateTime2;
				case PrmType.DateTimeOffset:
					return DbType.DateTimeOffset;
				case PrmType.Decimal:
					return DbType.Decimal;
				case PrmType.Double:
					return DbType.Double;
				case PrmType.Guid:
					return DbType.Guid;
				case PrmType.Int16:
					return DbType.Int16;
				case PrmType.Int32:
					return DbType.Int32;
				case PrmType.Int64:
					return DbType.Int64;
				case PrmType.Object:
					return DbType.Object;
				case PrmType.SByte:
					return DbType.SByte;
				case PrmType.Single:
					return DbType.Single;
				case PrmType.String:
					return DbType.String;
				case PrmType.StringFixedLength:
					return DbType.StringFixedLength;
				case PrmType.Time:
					return DbType.Time;
				case PrmType.UInt16:
					return DbType.UInt16;
				case PrmType.UInt32:
					return DbType.UInt32;
				case PrmType.UInt64:
					return DbType.UInt64;
				case PrmType.VarNumeric:
					return DbType.VarNumeric;
				case PrmType.Xml:
					return DbType.Xml;
				default:
					return DbType.Object;
			}
		}
	}
}