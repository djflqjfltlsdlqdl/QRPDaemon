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
    public class clsBL
    {
        /// <summary>
        /// 파일데이터 저장
        /// </summary>
        /// <param name="dsData">파일데이터
        /// FI : 파일정보
        /// FC : 파일컬럼정보
        /// H : 헤더정보
        /// D : 측정데이터
        /// </param>
        /// <returns></returns>
        public static string mfSaveBatchFile(DataSet dsData)
        {
            using (SqlConnection sqlCon = QRPDB.mfConnect())
            {
                using (SqlTransaction trans = sqlCon.BeginTransaction())
                {
                    try
                    {
                        COM.TransErrRtn ErrRtn = new COM.TransErrRtn();

                        DataTable dtParam;
                        string strErrRtn = string.Empty;
                        int intRowCount = dsData.Tables["FI"].Rows.Count;
                        Int64 intFileID = 0;
                        Int64 intBatchID = 0;
                        int intBatchIndex = 0;
                        for (int i = 0; i < intRowCount; i++)
                        {
                            dtParam = QRPDB.mfSetParamDataTable();
                            QRPDB.mfAddParamDataRow(dtParam, "@Rtn", ParameterDirection.ReturnValue, SqlDbType.VarChar, 100);
                            QRPDB.mfAddParamDataRow(dtParam, "@i_intFileID", ParameterDirection.Input, SqlDbType.BigInt, dsData.Tables["FI"].Rows[i]["FileID"]);
                            QRPDB.mfAddParamDataRow(dtParam, "@i_strFileName", ParameterDirection.Input, SqlDbType.NVarChar, dsData.Tables["FI"].Rows[i]["FileName"]);
                            QRPDB.mfAddParamDataRow(dtParam, "@i_strOriginFilePath", ParameterDirection.Input, SqlDbType.NVarChar, dsData.Tables["FI"].Rows[i]["OriginFilePath"]);
                            QRPDB.mfAddParamDataRow(dtParam, "@i_strBackupFilePath", ParameterDirection.Input, SqlDbType.NVarChar, dsData.Tables["FI"].Rows[i]["BackupFilePath"]);
                            QRPDB.mfAddParamDataRow(dtParam, "@o_intFileID", ParameterDirection.Output, SqlDbType.BigInt);
                            QRPDB.mfAddParamDataRow(dtParam, "@ErrorMessage", ParameterDirection.Output, SqlDbType.VarChar, 8000);

                            strErrRtn = QRPDB.mfExecTransStoredProc(sqlCon, trans, "up_Update_EXPFileInfo_01", dtParam);
                            ErrRtn = ErrRtn.mfDecodingErrMessage(strErrRtn);
                            if (ErrRtn.ErrNum.Equals(0))
                            {
                                intFileID = ErrRtn.mfGetReturnValue(0).ToInt64();

                                #region Column 정보 저장
                                //var vColumn = dsData.Tables["FC"].AsEnumerable().Where(w => w.Field<int>("FileIndex").Equals(intFileIndex));
                                foreach (var row in dsData.Tables["FC"].AsEnumerable())
                                {
                                    dtParam = QRPDB.mfSetParamDataTable();
                                    QRPDB.mfAddParamDataRow(dtParam, "@Rtn", ParameterDirection.ReturnValue, SqlDbType.VarChar, 100);
                                    QRPDB.mfAddParamDataRow(dtParam, "@i_intFileID", ParameterDirection.Input, SqlDbType.BigInt, intFileID);
                                    QRPDB.mfAddParamDataRow(dtParam, "@i_intColID", ParameterDirection.Input, SqlDbType.Int, row.Field<int>("ColID").ToString());
                                    QRPDB.mfAddParamDataRow(dtParam, "@i_strColumnName", ParameterDirection.Input, SqlDbType.NVarChar, row.Field<string>("ColumnName"));
                                    QRPDB.mfAddParamDataRow(dtParam, "@i_strUnitName", ParameterDirection.Input, SqlDbType.NVarChar, row.Field<string>("UnitName"));
                                    QRPDB.mfAddParamDataRow(dtParam, "@ErrorMessage", ParameterDirection.Output, SqlDbType.VarChar, 8000);
                                    strErrRtn = QRPDB.mfExecTransStoredProc(sqlCon, trans, "up_Update_EXPFileColumn_01", dtParam);
                                    ErrRtn = ErrRtn.mfDecodingErrMessage(strErrRtn);
                                    if (!ErrRtn.ErrNum.Equals(0))
                                    {
                                        trans.Rollback();
                                        return strErrRtn;
                                    }
                                }
                                #endregion Column 정보 저장

                                #region File Header 정보 저장
                                //var vHeader = dsData.Tables["H"].AsEnumerable().Where(w => w.Field<int>("FileIndex").Equals(intFileIndex));
                                foreach (var row in dsData.Tables["H"].AsEnumerable())
                                {
                                    dtParam = QRPDB.mfSetParamDataTable();
                                    QRPDB.mfAddParamDataRow(dtParam, "@Rtn", ParameterDirection.ReturnValue, SqlDbType.VarChar, 100);
                                    QRPDB.mfAddParamDataRow(dtParam, "@i_intBatchID", ParameterDirection.Input, SqlDbType.BigInt, dsData.Tables["H"].Rows[i]["BatchIndex"]);
                                    QRPDB.mfAddParamDataRow(dtParam, "@i_intFileID", ParameterDirection.Input, SqlDbType.BigInt, intFileID.ToString());
                                    QRPDB.mfAddParamDataRow(dtParam, "@i_strPlantCode", ParameterDirection.Input, SqlDbType.VarChar, row.Field<string>("PlantCode"));
                                    QRPDB.mfAddParamDataRow(dtParam, "@i_strProcessGroupCode", ParameterDirection.Input, SqlDbType.VarChar, row.Field<string>("ProcessGroupCode"));
                                    QRPDB.mfAddParamDataRow(dtParam, "@i_strInspectTypeCode", ParameterDirection.Input, SqlDbType.VarChar, row.Field<string>("InspectTypeCode"));
                                    QRPDB.mfAddParamDataRow(dtParam, "@i_strSampleName", ParameterDirection.Input, SqlDbType.NVarChar, row.Field<string>("SampleName"));
                                    QRPDB.mfAddParamDataRow(dtParam, "@i_dateSampleDate", ParameterDirection.Input, SqlDbType.DateTime, row.Field<DateTime>("SampleDate"));
                                    QRPDB.mfAddParamDataRow(dtParam, "@o_intBatchID", ParameterDirection.Output, SqlDbType.BigInt);
                                    QRPDB.mfAddParamDataRow(dtParam, "@ErrorMessage", ParameterDirection.Output, SqlDbType.VarChar, 8000);
                                    strErrRtn = QRPDB.mfExecTransStoredProc(sqlCon, trans, "up_Update_EXPFileDataH_01", dtParam);
                                    ErrRtn = ErrRtn.mfDecodingErrMessage(strErrRtn);
                                    if (ErrRtn.ErrNum.Equals(0))
                                    {
                                        intBatchIndex = row.Field<int>("BatchIndex");
                                        intBatchID = ErrRtn.mfGetReturnValue(0).ToInt64();
                                        if (intBatchID < 0)
                                            continue;

                                        #region File 측정데이터 저장
                                        var vResult = dsData.Tables["D"].AsEnumerable().Where(w => w.Field<int>("BatchIndex").Equals(intBatchIndex));
                                        foreach (var data in vResult)
                                        {
                                            dtParam = QRPDB.mfSetParamDataTable();
                                            QRPDB.mfAddParamDataRow(dtParam, "@Rtn", ParameterDirection.ReturnValue, SqlDbType.VarChar, 100);
                                            QRPDB.mfAddParamDataRow(dtParam, "@i_intBatchID", ParameterDirection.Input, SqlDbType.BigInt, intBatchID.ToString());
                                            QRPDB.mfAddParamDataRow(dtParam, "@i_intColID", ParameterDirection.Input, SqlDbType.Int, data.Field<int>("ColID").ToString());
                                            QRPDB.mfAddParamDataRow(dtParam, "@i_intRowIndex", ParameterDirection.Input, SqlDbType.Int, data.Field<int>("RowIndex").ToString());
                                            QRPDB.mfAddParamDataRow(dtParam, "@i_strInspectValue", ParameterDirection.Input, SqlDbType.VarChar, data.Field<string>("InspectValue"));
                                            QRPDB.mfAddParamDataRow(dtParam, "@ErrorMessage", ParameterDirection.Output, SqlDbType.VarChar, 8000);
                                            strErrRtn = QRPDB.mfExecTransStoredProc(sqlCon, trans, "up_Update_EXPFileDataD_01", dtParam);
                                            ErrRtn = ErrRtn.mfDecodingErrMessage(strErrRtn);
                                            if (!ErrRtn.ErrNum.Equals(0))
                                            {
                                                trans.Rollback();
                                                return strErrRtn;
                                            }
                                        }
                                        #endregion File 측정데이터 저장
                                    }
                                    else
                                    {
                                        trans.Rollback();
                                        return strErrRtn;
                                    }
                                }
                                #endregion File Header 정보 저장
                            }
                            else
                            {
                                trans.Rollback();
                                return strErrRtn;
                            }
                        }

                        ErrRtn = ErrRtn.mfDecodingErrMessage(strErrRtn);
                        if(ErrRtn.ErrNum.Equals(0))
                        {
                            trans.Commit();
                        }

                        return strErrRtn;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw new System.ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                    }
                    finally
                    {
                        sqlCon.Close();
                    }
                }
            }
        }
    }
}
