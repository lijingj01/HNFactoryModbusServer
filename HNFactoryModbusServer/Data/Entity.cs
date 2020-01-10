using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNFactoryModbusServer.Data
{
	#region 实体对象

	public class ModbusMessageLog
	{
		#region 属性
		public int Id { get; set; }
		public DateTime LogTime { get; set; }
		public string LogMessage { get; set; }
		public string FunctionCode { get; set; }
		public string SlaveAddress { get; set; }
		public byte[] MessageFrame { get; set; }
		public byte[] ProtocolDataUnit { get; set; }
		public string TransactionId { get; set; }

		#endregion

		#region 处理方法
		/// <summary>
		/// 将对象转换成Json字符串
		/// </summary>
		/// <returns></returns>
		public string ToJsonString()
		{
			string strJson = string.Empty;
			try
			{
				IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
				//这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式  'HH':'mm':'ss"     
				timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd HH':'mm':'ss";
				strJson = JsonConvert.SerializeObject(this, Formatting.Indented, timeConverter);
				

			}
			catch(Exception ex) { }
			return strJson;
		}
		#endregion
	}

    #endregion

    #region 数据库操作

    public static class DataLogHelper
	{
		/// <summary>
		/// 添加PLC设备参数日志
		/// </summary>
		/// <param name="item"></param>
		public static void AddModbusMessageLog(ModbusMessageLog item)
		{
			try
			{
				string strSQL = @"INSERT INTO l_modbusmessage(LogTime,LogMessage,FunctionCode,SlaveAddress,MessageFrame,ProtocolDataUnit,TransactionId)VALUES(?LogTime,?LogMessage,?FunctionCode,?SlaveAddress,?MessageFrame,?ProtocolDataUnit,?TransactionId)";
				List<MySqlParameter> sqlParameters = new List<MySqlParameter>
				{
					new MySqlParameter("?LogTime", item.LogTime)
					,new MySqlParameter("?LogMessage",item.LogMessage)
					,new MySqlParameter("?FunctionCode",item.FunctionCode)
					,new MySqlParameter("?SlaveAddress",item.SlaveAddress)
					,new MySqlParameter("?MessageFrame",item.MessageFrame)
					,new MySqlParameter("?ProtocolDataUnit",item.ProtocolDataUnit)
					,new MySqlParameter("?TransactionId",item.TransactionId)
				};

				MySqlHelper.ExecuteNonQuery(CommandType.Text, strSQL, sqlParameters.ToArray());

			}
			catch(Exception ex)
			{

			}
		}
	}

	#endregion
}
