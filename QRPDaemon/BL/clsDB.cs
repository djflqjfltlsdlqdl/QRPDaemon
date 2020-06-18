using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QRPDaemon.COM;

namespace QRPDaemon.BL
{
    public static class QRPDB
    {
        #region Var
        /// <summary>
        /// CommandTimeOut
        /// </summary>
        private const int m_intCmdTimeOut = 30;

        private const string m_strConnectionstring = "data source={0};user id={1};password={2};Initial Catalog={3};persist security info=true";
        #endregion

        #region Function
        /// <summary>
        /// SQL Connection Open
        /// </summary>
        /// <returns></returns>
        public static System.Data.SqlClient.SqlConnection mfConnect()
        {
            try
            {
                System.Data.SqlClient.SqlConnection sqlCon = new System.Data.SqlClient.SqlConnection(string.Format(m_strConnectionstring
                    , Properties.Settings.Default.DBIP
                    , Properties.Settings.Default.DBID
                    , Properties.Settings.Default.DBPW
                    , Properties.Settings.Default.DBCatalog));
                sqlCon.Open();

                return sqlCon;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
        public static string mfExecTransStoredProc(System.Data.SqlClient.SqlConnection m_SqlCon, System.Data.SqlClient.SqlTransaction m_SqlTrans, string strSPName, System.Data.DataTable dtSPParameter, int intCommandTimeOut = m_intCmdTimeOut)
        {
            TransErrRtn Result = new TransErrRtn();
            string strErrRtn = null;
            if (m_SqlCon.State.Equals(System.Data.ConnectionState.Open))
            {
                using (System.Data.SqlClient.SqlCommand m_SqlCmd = new System.Data.SqlClient.SqlCommand())
                {
                    m_SqlCmd.CommandTimeout = intCommandTimeOut;
                    m_SqlCmd.Connection = m_SqlCon;
                    if (m_SqlTrans != null)
                        m_SqlCmd.Transaction = m_SqlTrans;
                    m_SqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    m_SqlCmd.CommandText = strSPName;
                    m_SqlCmd.CommandTimeout = intCommandTimeOut;
                    foreach (System.Data.DataRow dr in dtSPParameter.Rows)
                    {
                        System.Data.SqlClient.SqlParameter param = new System.Data.SqlClient.SqlParameter();
                        param.ParameterName = dr["ParamName"].ToString();
                        param.Direction = (System.Data.ParameterDirection)dr["ParamDirect"];
                        param.SqlDbType = (System.Data.SqlDbType)dr["DBType"];
                        param.IsNullable = true;
                        param.Value = dr["Value"];

                        if (!dr["Length"].ToString().Equals(string.Empty))
                            param.Size = dr["Length"].ToInt();

                        if (!dr["Scale"].ToString().Equals(string.Empty))
                        {
                            byte outbyte = 0;
                            byte.TryParse(dr["Scale"].ToString(), out outbyte);
                            param.Scale = outbyte;
                        }

                        m_SqlCmd.Parameters.Add(param);
                    }

                    try
                    {
                        m_SqlCmd.ExecuteNonQuery();

                        //처리 결과를 구조체 변수에 저장시킴
                        Result.ErrNum = m_SqlCmd.Parameters["@Rtn"].Value.ToInt();
                        Result.ErrMessage = m_SqlCmd.Parameters["@ErrorMessage"].Value.ToString();

                        //Output Param이 있는 경우 ArrayList에 저장시킴.
                        Result.mfInitReturnValue();
                        foreach (System.Data.DataRow dr in dtSPParameter.Rows)
                        {
                            if (!dr["ParamName"].ToString().Equals("@Rtn") &&
                                !dr["ParamName"].ToString().Equals("@ErrorMessage") &&
                                (((System.Data.ParameterDirection)dr["ParamDirect"]).Equals(System.Data.ParameterDirection.Output) || ((System.Data.ParameterDirection)dr["ParamDirect"]).Equals(System.Data.ParameterDirection.InputOutput)))
                            {
                                Result.mfAddReturnValue(m_SqlCmd.Parameters[dr["ParamName"].ToString()].Value.ToString());
                            }
                        }
                        strErrRtn = Result.mfEncodingErrMessage(Result);
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        throw new ApplicationException("SQL Exception", ex);
                    }
                    catch (System.Exception ex)
                    {
                        throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                    }
                    return strErrRtn;
                }
            }
            else
            {
                Result.ErrNum = 99;
                Result.ErrMessage = "DataBase 연결되지 않았습니다.";
                strErrRtn = Result.mfEncodingErrMessage(Result);
                return strErrRtn;
            }
        }
        public static string mfExecTransStoredProc(System.Data.SqlClient.SqlConnection m_SqlCon, string strSPName, System.Data.DataTable dtSPParameter)
        {
            return mfExecTransStoredProc(m_SqlCon, null, strSPName, dtSPParameter);
        }
        public static System.Data.DataTable mfExecTransStoredProcWithSelect(System.Data.SqlClient.SqlConnection m_SqlCon, System.Data.SqlClient.SqlTransaction m_SqlTrans, string strSPName, System.Data.DataTable dtSPParameter, int intCommandTimeOut = m_intCmdTimeOut)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            if (m_SqlCon.State.Equals(System.Data.ConnectionState.Open))
            {
                using (System.Data.SqlClient.SqlCommand m_SqlCmd = new System.Data.SqlClient.SqlCommand())
                {
                    m_SqlCmd.CommandTimeout = intCommandTimeOut;
                    m_SqlCmd.Connection = m_SqlCon;
                    m_SqlCmd.Transaction = m_SqlTrans;
                    m_SqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    m_SqlCmd.CommandText = strSPName;
                    foreach (System.Data.DataRow dr in dtSPParameter.Rows)
                    {
                        System.Data.SqlClient.SqlParameter param = new System.Data.SqlClient.SqlParameter();
                        param.ParameterName = dr["ParamName"].ToString();
                        param.Direction = (System.Data.ParameterDirection)dr["ParamDirect"];
                        param.SqlDbType = (System.Data.SqlDbType)dr["DBType"];
                        param.IsNullable = true;
                        param.Value = dr["Value"];

                        if (!dr["Length"].ToString().Equals(string.Empty))
                        {
                            param.Size = dr["Length"].ToInt();
                        }

                        m_SqlCmd.Parameters.Add(param);
                    }

                    try
                    {
                        System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(m_SqlCmd);
                        da.Fill(dt);

                        dt.Columns.Add("ERRNUM", typeof(int));
                        dt.Columns.Add("ERRORMESSAGE", typeof(string));

                        //처리 결과를 구조체 변수에 저장시킴
                        if (dt.Rows.Count.Equals(0))
                            dt.Rows.Add(dt.NewRow());
                        dt.Rows[0]["ERRNUM"] = Convert.ToInt32(m_SqlCmd.Parameters["@Rtn"].Value);
                        dt.Rows[0]["ERRORMESSAGE"] = m_SqlCmd.Parameters["@ErrorMessage"].Value.ToString();

                        foreach (System.Data.DataRow dr in dtSPParameter.Rows)
                        {
                            if (!dr["ParamName"].ToString().Equals("@Rtn") &&
                                !dr["ParamName"].ToString().Equals("@ErrorMessage") &&
                                (((System.Data.ParameterDirection)dr["ParamDirect"]).Equals(System.Data.ParameterDirection.Output) || ((System.Data.ParameterDirection)dr["ParamDirect"]).Equals(System.Data.ParameterDirection.InputOutput)))
                            {
                                string strName = dr["ParamName"].ToString().Replace("@", string.Empty);
                                dt.Columns.Add(strName, typeof(object));
                                dt.Rows[0][strName] = m_SqlCmd.Parameters[dr["ParamName"].ToString()].Value;
                            }
                        }
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        throw new ApplicationException("SQL Exception", ex);
                    }
                    catch (System.Exception ex)
                    {
                        throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                    }
                    return dt;
                }
            }
            else
            {
                dt.Columns.Add("ErrNum", typeof(int));
                dt.Columns.Add("ErrorMessage", typeof(string));
                dt.Rows.Add(99, "DataBase 연결되지 않았습니다.");
                return dt;
            }
        }
        public static System.Data.DataTable mfExecReadStoredProc(System.Data.SqlClient.SqlConnection m_SqlCon, string strSPName, System.Data.DataTable dtSPParameter, int intCommandTimeOut = m_intCmdTimeOut)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            try
            {
                if (m_SqlCon.State == System.Data.ConnectionState.Open)
                {
                    using (System.Data.SqlClient.SqlCommand m_SqlCmd = new System.Data.SqlClient.SqlCommand())
                    {
                        m_SqlCmd.Connection = m_SqlCon;
                        m_SqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        m_SqlCmd.CommandText = strSPName;
                        m_SqlCmd.CommandTimeout = intCommandTimeOut;
                        foreach (System.Data.DataRow dr in dtSPParameter.Rows)
                        {
                            System.Data.SqlClient.SqlParameter param = new System.Data.SqlClient.SqlParameter();
                            param.ParameterName = dr["ParamName"].ToString();
                            param.Direction = (System.Data.ParameterDirection)dr["ParamDirect"];
                            param.SqlDbType = (System.Data.SqlDbType)dr["DBType"];
                            param.Value = dr["Value"].ToString();
                            if (dr["Length"].ToString() != "")
                            {
                                if (Convert.ToInt32(dr["Length"].ToString()) > 0)
                                    param.Size = Convert.ToInt32(dr["Length"].ToString());
                            }
                            m_SqlCmd.Parameters.Add(param);
                        }

                        System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(m_SqlCmd);
                        da.Fill(dt);
                    }
                }
                return dt;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new ApplicationException("SQL Exception", ex);
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
        public static System.Data.DataSet mfExecReadStoredProc_DS(System.Data.SqlClient.SqlConnection m_SqlCon, string strSPName, System.Data.DataTable dtSPParameter, int intCommandTimeOut = m_intCmdTimeOut)
        {
            System.Data.DataSet ds = new System.Data.DataSet();
            try
            {
                if (m_SqlCon.State == System.Data.ConnectionState.Open)
                {
                    using (System.Data.SqlClient.SqlCommand m_SqlCmd = new System.Data.SqlClient.SqlCommand())
                    {
                        m_SqlCmd.Connection = m_SqlCon;
                        m_SqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        m_SqlCmd.CommandText = strSPName;
                        m_SqlCmd.CommandTimeout = intCommandTimeOut;
                        foreach (System.Data.DataRow dr in dtSPParameter.Rows)
                        {
                            System.Data.SqlClient.SqlParameter param = new System.Data.SqlClient.SqlParameter();
                            param.ParameterName = dr["ParamName"].ToString();
                            param.Direction = (System.Data.ParameterDirection)dr["ParamDirect"];
                            param.SqlDbType = (System.Data.SqlDbType)dr["DBType"];
                            param.Value = dr["Value"].ToString();
                            if (dr["Length"].ToString() != "")
                            {
                                if (Convert.ToInt32(dr["Length"].ToString()) > 0)
                                    param.Size = Convert.ToInt32(dr["Length"].ToString());
                            }
                            m_SqlCmd.Parameters.Add(param);
                        }

                        System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(m_SqlCmd);
                        da.Fill(ds);
                    }
                }
                return ds;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new ApplicationException("SQL Exception", ex);
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
        public static System.Data.DataTable mfSetParamDataTable()
        {
            System.Data.DataTable dt = null;
            try
            {
                dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[] {
                    new System.Data.DataColumn("ParamName", typeof(string))
                    , new System.Data.DataColumn("ParamDirect", typeof(System.Data.ParameterDirection))
                    , new System.Data.DataColumn("DBType", typeof(System.Data.SqlDbType ))
                    , new System.Data.DataColumn("Value", typeof(object))
                    , new System.Data.DataColumn("Length", typeof(int))
                    , new System.Data.DataColumn("Scale", typeof(int))
                });
                return dt;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
        public static void mfAddParamDataRow(System.Data.DataTable dt, string strName, System.Data.ParameterDirection Direction, System.Data.SqlDbType DBType, string strValue, int intSize)
        {
            mfAddParamDataRow(dt, strName, Direction, DBType, (object)strValue, intSize);
        }
        public static void mfAddParamDataRow(System.Data.DataTable dt, string strName, System.Data.ParameterDirection Direction, System.Data.SqlDbType DBType, string strValue)
        {
            mfAddParamDataRow(dt, strName, Direction, DBType, (object)strValue, int.MinValue);
        }
        public static void mfAddParamDataRow(System.Data.DataTable dt, string strName, System.Data.ParameterDirection Direction, System.Data.SqlDbType DBType)
        {
            mfAddParamDataRow(dt, strName, Direction, DBType, (object)null, int.MinValue);
        }
        public static void mfAddParamDataRow(System.Data.DataTable dt, string strName, System.Data.ParameterDirection Direction, System.Data.SqlDbType DBType, int intSize)
        {
            mfAddParamDataRow(dt, strName, Direction, DBType, (object)null, intSize);
        }
        public static void mfAddParamDataRow(System.Data.DataTable dt, string strName, System.Data.ParameterDirection Direction, System.Data.SqlDbType DBType, object objValue)
        {
            mfAddParamDataRow(dt, strName, Direction, DBType, objValue, int.MinValue);
        }
        public static void mfAddParamDataRow(System.Data.DataTable dt, string strName, System.Data.ParameterDirection Direction, System.Data.SqlDbType DBType, object objValue, int intSize)
        {
            mfAddParamDataRow(dt, strName, Direction, DBType, objValue, intSize, int.MinValue);
        }
        public static void mfAddParamDataRow(System.Data.DataTable dt, string strName, System.Data.ParameterDirection Direction, System.Data.SqlDbType DBType, object objValue, int intSize, int intScale)
        {
            try
            {
                System.Data.DataRow dr = dt.NewRow();
                dr["ParamName"] = strName;
                dr["ParamDirect"] = Direction;
                dr["DBType"] = DBType;
                dr["Value"] = objValue;
                if (!intSize.Equals(int.MinValue))
                    dr["Length"] = intSize;
                if (!intScale.Equals(int.MinValue))
                    dr["Scale"] = intScale;
                dt.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
        #endregion
    }
}
