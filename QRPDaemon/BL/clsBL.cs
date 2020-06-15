using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using QRPDaemon.COM;

namespace QRPDaemon.BL
{
    /// <summary>
    /// BL 클래스
    /// </summary>
    public static class clsBL
    {
        ///// <summary>
        ///// 파일데이터 저장
        ///// </summary>
        ///// <param name="dsData">파일데이터
        ///// FI : 파일정보
        ///// FC : 파일컬럼정보
        ///// H : 헤더정보
        ///// D : 측정데이터
        ///// </param>
        ///// <returns></returns>
        //public string mfSaveBatchFile(DataSet dsData)
        //{
        //    using (SqlConnection sqlCon = QRPDB.mfConnect())
        //    {
        //        using (SqlTransaction trans = sqlCon.BeginTransaction())
        //        {
        //            try
        //            {
        //                COM.TransErrRtn ErrRtn = new COM.TransErrRtn();

        //                DataTable dtParam;
        //                string strErrRtn;
        //                int intRowCount = dsData.Tables["H"].Rows.Count;
        //                Int64 intBatchID = 0;
        //                int intOriginBatchID = 0;
        //                for (int i = 0; i < intRowCount; i++)
        //                {
        //                    dtParam = QRPDB.mfSetParamDataTable();
        //                    QRPDB.mfAddParamDataRow(dtParam, "@Rtn", ParameterDirection.ReturnValue, SqlDbType.VarChar, 100);
        //                    QRPDB.mfAddParamDataRow(dtParam, "@i_intBatchID", ParameterDirection.Input, SqlDbType.BigInt, dsData.Tables["H"].Rows[i]["BatchID"]);
        //                    QRPDB.mfAddParamDataRow(dtParam, "@i_strPlantCode", ParameterDirection.Input, SqlDbType.VarChar, dsData.Tables["H"].Rows[i]["PlantCode"]);
        //                    QRPDB.mfAddParamDataRow(dtParam, "@i_strProcessGroupCode", ParameterDirection.Input, SqlDbType.VarChar, dsData.Tables["H"].Rows[i]["ProcessGroupCode"]);
        //                    QRPDB.mfAddParamDataRow(dtParam, "@i_strInspectTypeCode", ParameterDirection.Input, SqlDbType.VarChar, dsData.Tables["H"].Rows[i]["InspectTypeCode"]);
        //                    QRPDB.mfAddParamDataRow(dtParam, "@i_strSampleName", ParameterDirection.Input, SqlDbType.NVarChar, dsData.Tables["H"].Rows[i]["SampleName"]);
        //                    QRPDB.mfAddParamDataRow(dtParam, "@i_dateSampleDate", ParameterDirection.Input, SqlDbType.DateTime, dsData.Tables["H"].Rows[i]["SampleDate"]);
        //                    QRPDB.mfAddParamDataRow(dtParam, "@i_strFileName", ParameterDirection.Input, SqlDbType.NVarChar, dsData.Tables["H"].Rows[i]["FileName"]);
        //                    QRPDB.mfAddParamDataRow(dtParam, "@i_strOriginFilePath", ParameterDirection.Input, SqlDbType.NVarChar, dsData.Tables["H"].Rows[i]["OriginFilePath"]);
        //                    QRPDB.mfAddParamDataRow(dtParam, "@i_strBackupFilePath", ParameterDirection.Input, SqlDbType.NVarChar, dsData.Tables["H"].Rows[i]["BackupFilePath"]);
        //                    //QRPDB.mfAddParamDataRow(dtParam, "@i_intResultCode", ParameterDirection.Input, SqlDbType.Int, dsData.Tables["H"].Rows[i]["ResultCode"]);
        //                    //QRPDB.mfAddParamDataRow(dtParam, "@i_strReusltDesc", ParameterDirection.Input, SqlDbType.NVarChar, dsData.Tables["H"].Rows[i]["ResultDesc"]);
        //                    QRPDB.mfAddParamDataRow(dtParam, "@o_intBatchID", ParameterDirection.Output, SqlDbType.BigInt);
        //                    QRPDB.mfAddParamDataRow(dtParam, "@ErrorMessage", ParameterDirection.Output, SqlDbType.VarChar, 8000);

        //                    strErrRtn = QRPDB.mfExecTransStoredProc(sqlCon, trans, "up_Update_EXPFileDataH_01", dtParam);
        //                    ErrRtn = ErrRtn.mfDecodingErrMessage(strErrRtn);
        //                    if (ErrRtn.ErrNum.Equals(0))
        //                    {
        //                        intOriginBatchID = dsData.Tables["H"].Rows[i]["BatchID"].ToInt();
        //                        intBatchID = ErrRtn.mfGetReturnValue(0).ToInt64();

        //                        #region Column 정보 저장
                                
        //                        #endregion Column 정보 저장
        //                    }
        //                    else
        //                    {
        //                        return strErrRtn;
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new System.ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
        //            }
        //            finally
        //            {
        //                sqlCon.Close();
        //            }
        //        }
        //    }
        //}
    }
}
