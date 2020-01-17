using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HNFactoryModbusServer.Data
{
	#region 类基础对象

	public class EntityBase
	{
		public int Id { get; set; }

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
			catch (Exception ex) { }
			return strJson;
		}
		#endregion
	}

	public class ListBase<T> : List<T>
	{
		/// <summary>
		/// List集合转换成对象
		/// </summary>
		/// <param name="list"></param>
		public void ListToConvert(List<T> list)
		{
			if (list != null)
			{
				//先移除对象里面的集合数据
				this.Clear();
				foreach (T t in list)
				{
					this.Add(t);
				}
			}
		}

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
			catch (Exception ex) { }
			return strJson;
		}
		#endregion
	}
	#endregion

	#region 实体对象

	public class ModbusMessageLog : EntityBase
	{
		#region 属性
		public DateTime LogTime { get; set; }
		public string LogMessage { get; set; }
		public string FunctionCode { get; set; }
		public string SlaveAddress { get; set; }
		public byte[] MessageFrame { get; set; }
		public byte[] ProtocolDataUnit { get; set; }
		public string TransactionId { get; set; }

		#endregion


	}

	#endregion

	#region 传感器扩展对象

	/// <summary>
	/// PLC内传感器参数对象
	/// </summary>
	public class PLCAddressSet:EntityBase
	{
		#region 属性
		/// <summary>
		/// 工厂编号
		/// </summary>
		public string FactoryId { get; set; }
		/// <summary>
		/// 所在PLC机柜编号
		/// </summary>
		public string PLC_Id { get; set; }
		/// <summary>
		/// 传感器在PLC机柜里面的地址编码
		/// </summary>
		public string PLC_Address { get; set; }
		/// <summary>
		/// 传感器数值在PLC里面的长度（默认2）
		/// </summary>
		public int ValueLength { get; set; }
		/// <summary>
		/// 传感器控制器编号
		/// </summary>
		public string SensorId { get; set; }
		/// <summary>
		/// 设备的读写性质
		/// </summary>
		public string RWType { get; set; }
		/// <summary>
		/// 设备数值与实际数值的缩放比例
		/// </summary>
		public decimal Scale { get; set; }
		/// <summary>
		/// 数值对应的状态类型
		/// </summary>
		public string ValueType { get; set; }
		#endregion

		#region 构造函数

		#endregion
	}

	public class PLCAddressSetCollection : ListBase<PLCAddressSet>
	{

	}


	#endregion

	#region 用来传递给后台处理的对象

	public class SensorData : EntityBase
	{
		#region 属性
		/// <summary>
		/// 工厂编号
		/// </summary>
		public string FactoryId { get; set; }
		/// <summary>
		/// 所在PLC机柜编号
		/// </summary>
		public string PLCId { get; set; }
		/// <summary>
		/// 传感器在PLC机柜里面的地址编码
		/// </summary>
		public string PLCAddress { get; set; }
		/// <summary>
		/// 传感器控制器编号
		/// </summary>
		public string SensorId { get; set; }
		/// <summary>
		/// 数值对应的状态类型
		/// </summary>
		public string ValueType { get; set; }
		/// <summary>
		/// 传感器的数值
		/// </summary>
		public decimal SensorValue { get; set; }
		/// <summary>
		/// 读取传感器的时间
		/// </summary>
		public DateTime ReceiveTime { get; set; }
		#endregion
	}

    public class SensorDataCollection : ListBase<SensorData>
	{

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

		/// <summary>
		/// 通过工厂和PLC机柜编号获取对应的传感器控制器的地址参数信息
		/// </summary>
		/// <param name="strFactoryId"></param>
		/// <param name="strPLCId"></param>
		/// <returns></returns>
		public static PLCAddressSetCollection GetPLCAddressSets(string strFactoryId,string strPLCId)
		{
			try
			{
				PLCAddressSetCollection addressSets = new PLCAddressSetCollection();
				string strSQL = "select * from p_plcaddressset where FactoryId=?FactoryId and PLC_Id=?PLC_Id";

				List<MySqlParameter> sqlParameters = new List<MySqlParameter>
				{
					new MySqlParameter("?FactoryId", strFactoryId),
					new MySqlParameter("?PLC_Id", strPLCId)
				};
				PLCAddressSet info = new PLCAddressSet();

				CreateDataList<PLCAddressSet>(addressSets, strSQL, sqlParameters, info, null);

				return addressSets;
			}
			catch(Exception ex)
			{
				return new PLCAddressSetCollection();
			}
		}

		#region 内部处理方法

		private static void CreateDataList<T>(ListBase<T> infos, string strSQL, List<MySqlParameter> sqlParameters, T info, Dictionary<string, Type> dEnum)
		{

			//提取数据
			DataSet dataSet = MySqlHelper.GetDataSet(CommandType.Text, strSQL, sqlParameters.ToArray());
			if (dataSet.Tables.Count > 0)
			{
				DataTable table = dataSet.Tables[0];
				infos.ListToConvert(TableToObject<T>(table, info, dEnum));
			}
		}


		private static List<TResult> TableToObject<TResult>(DataTable dt, TResult ob, Dictionary<string, Type> dEnum) //泛型方法,此处TResult为类型参数
		{
			List<PropertyInfo> prlist = new List<PropertyInfo>();//创建一个属性列表集合

			Type t = typeof(TResult); //获取实体对象的元数据Type类型

			PropertyInfo[] prArr = t.GetProperties(); //取得实体对象的所有属性到属性集合中

			foreach (PropertyInfo pr in prArr) //循环遍历属性集合到List集合
				prlist.Add(pr);
			//通过匿名方法自定义筛选条件  => 检查datatable中是否存在存在此列, 
			Predicate<PropertyInfo> prPredicate = delegate (PropertyInfo pr) { if (dt.Columns.IndexOf(pr.Name) != -1) return true; return false; };
			//从指定的条件中
			List<PropertyInfo> templist = prlist.FindAll(prPredicate);
			//创建一个实体集合
			List<TResult> oblist = new List<TResult>();
			//遍历DataTable每一行
			foreach (DataRow row in dt.Rows)
			{
				ob = (TResult)Activator.CreateInstance(t);  //通过Type类型创建对象,并强制转换成实体类型

				Action<PropertyInfo> prAction = delegate (PropertyInfo pr)
				{

					if (dEnum != null)
					{
						string tempName = pr.Name;
						object value = row[tempName];

						var queryResult = from n in dEnum
										  where n.Key == tempName
										  select n;
						//枚举集合中包含与当前属性名相同的项
						if (queryResult.Count() > 0)
						{
							//将字符串转换为枚举对象
							pr.SetValue(ob, Enum.Parse(queryResult.FirstOrDefault().Value, value.ToString()), null);

						}
						else
						{
							if (row[pr.Name] != DBNull.Value)
							{
								pr.SetValue(ob, row[pr.Name], null);
							}
						}
					}
					else
					{
						if (row[pr.Name] != DBNull.Value)
						{
							pr.SetValue(ob, row[pr.Name], null);
						}
					}
				};
				//把选择出来的属性集合的每一个属性设置成上面创建的对象的属性
				templist.ForEach(prAction);

				oblist.Add(ob); //把属性添加到实体集合
			}
			return oblist;
		}
		#endregion
	}

	#endregion
}
