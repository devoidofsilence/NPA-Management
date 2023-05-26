using Newtonsoft.Json;
using NPAManagement.Helpers;
using NPAManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System.IO;
using System.Diagnostics;

namespace NPAManagement.Services
{
    public class NPAManagementService
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        readonly string connectionString = string.Empty;
        readonly string connectionStringCBS = string.Empty;
        readonly string connectionStringSBLINTRA = string.Empty;
        public NPAManagementService()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            connectionStringCBS = ConfigurationManager.ConnectionStrings["CBSConnection"].ConnectionString;
            connectionStringSBLINTRA = ConfigurationManager.ConnectionStrings["SBLINTRAConnection"].ConnectionString;
        }

        public List<NPAManagementMain> GetSavedNPAList()
        {
            List<NPAManagementMain> lstSavedNPAs = new List<NPAManagementMain>();
            string commandText = @"SELECT NPAMANAGEMENTMAIN.*,BRANCHES.BRANCHNAME FROM NPAMANAGEMENTMAIN with(nolock) LEFT OUTER JOIN BRANCHES on NPAMANAGEMENTMAIN.BRANCHLOCATIONCODE=BRANCHES.LOCATIONCODE";
            //if (!isHRAdmin)
            //{

            //    commandText += " WHERE RTRIM(LTRIM(LOWER(ADDEDBY)))=RTRIM(LTRIM(LOWER('" + username + "')))";
            //}
            using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, commandText,
                CommandType.Text))
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        try
                        {
                            NPAManagementMain npaObject = new NPAManagementMain()
                            {
                                NPAManagementId = (int)reader["NPAManagementId"],
                                CIFNo = reader["CIFNo"].ToString(),
                                BorrowerName = reader["BorrowerName"].ToString(),
                                BorrowerContactNumber = reader["BorrowerContactNumber"].ToString(),
                                BorrowerEmailAddress = reader["BorrowerEmailAddress"].ToString(),
                                CurrentROName = reader["CurrentROName"].ToString(),
                                BranchLocationCode = reader["BranchName"].ToString()
                            };
                            lstSavedNPAs.Add(npaObject);
                        }
                        catch (Exception ex)
                        {
                            // Get stack trace for the exception with source file information
                            var st = new StackTrace(ex, true);
                            // Get the top stack frame
                            var frame = st.GetFrame(0);
                            // Get the line number from the stack frame
                            var line = frame.GetFileLineNumber();
                            logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                            throw ex;
                        }
                    }
                }
            }
            return lstSavedNPAs;
        }

        public int SaveData(NPAManagementMain npaModel)
        {
            SqlConnection db = new SqlConnection(connectionString);
            SqlCommand comForNPAMain = new SqlCommand();
            SqlCommand comForBorrowerDetail = new SqlCommand();
            SqlCommand comForLoanDetailMain = new SqlCommand();
            SqlCommand comForBranchFollowUpMain = new SqlCommand();
            SqlCommand comForIndividualBranchFollowUpDetail = new SqlCommand();
            SqlCommand comForSACFollowUpMain = new SqlCommand();
            SqlCommand comForIndividualSACFollowUpDetail = new SqlCommand();
            SqlCommand comForPrimaryCollateralMain = new SqlCommand();
            SqlCommand comForIndividualPrimaryCollateralDetail = new SqlCommand();
            SqlCommand comForSecondaryCollateralMain = new SqlCommand();
            SqlCommand comForIndividualSecondaryCollateralDetail = new SqlCommand();
            SqlTransaction tran;
            db.Open();
            tran = db.BeginTransaction();
            int i = 0;
            if (npaModel != null)
            {
                try
                {
                    //exclude Files and save it


                    #region All inserts start

                    #region Insert into main NPA Table
                    comForNPAMain.CommandType = CommandType.Text;
                    comForNPAMain.Connection = db;
                    comForNPAMain.Transaction = tran;
                    comForNPAMain.CommandText = @"INSERT INTO [dbo].[NPAManagementMain]
                                           ([CIFNo]
                                           ,[GroupName]
                                           ,[BorrowerName]
                                           ,[BorrowerContactNumber]
                                           ,[BorrowerEmailAddress]
                                           ,[BorrowerPermanentAddressProvinceId]
                                           ,[BorrowerPermanentAddressZoneId]
                                           ,[BorrowerPermanentAddressDistrictId]
                                           ,[BorrowerPermanentAddressVDCMun]
                                           ,[BorrowerPermanentToleStreetName]
                                           ,[BorrowerPermanentHouseNumber]
                                           ,[TempAddSameAsPerAdd]
                                           ,[BorrowerTemporaryAddressProvinceId]
                                           ,[BorrowerTemporaryAddressZoneId]
                                           ,[BorrowerTemporaryAddressDistrictId]
                                           ,[BorrowerTemporaryAddressVDCMun]
                                           ,[BorrowerTemporaryToleStreetName]
                                           ,[BorrowerTemporaryHouseNumber]
                                           ,[BorrowerTypeId]
                                           ,[CurrentROName]
                                           ,[BranchLocationCode]
                                           ,[SACROName]
                                           ,[FileTransferDateToSAC]
                                           ,[ProvisionStatusIdOnTheDateOfTransfer]
                                           ,[DateOfRequestForBlacklisting]
                                           ,[BlacklistStatusOfRequest]
                                           ,[BlacklistDate]
                                           ,[BlacklistNumber]
                                           ,[DateOfInitiationForBookingNBA]
                                           ,[NBAStatusOfRequestId]
                                           ,[NBADateOfCompletion]
                                           ,[NBAAmount]
                                           ,[NBARemarks]
                                           ,[DateOfFileHandedOverToLegalForDRT]
                                           ,[DateOfFilingACaseInDRT]
                                           ,[DRTStatusOfRequestId]
                                           ,[DRTDateOfCompletion]
                                           ,[DRTAmount]
                                           ,[DRTRemarks]
                                           ,[DateOfInitiationForWriteOff]
                                           ,[WriteOffStatusOfRequestId]
                                           ,[WriteOffDateOfCompletion]
                                           ,[PrincipalOnWriteOffDate]
                                           ,[InterestOnWriteOffDate]
                                           ,[PrincipalRecoveredFromStartDateToLoanWriteOffDate]
                                           ,[InterestRecoveredFromStartDateToLoanWriteOffDate]
                                           ,[InterestOnInterestRecoveredFromStartDateToLoanWriteOffDate]
                                           ,[PenalAmountRecoveredFromStartDateToLoanWriteOffDate]
                                           ,[WriteOffNextActionToBeTaken]
                                           ,[WriteOffDueDateOfNextAction]
                                           ,[WriteOffRemarks]
                                           ,[EnteredBy]
                                           ,[EnteredOn]) OUTPUT INSERTED.NPAManagementId
                                     VALUES
                                           (@CIFNo
                                           ,@GroupName
                                           ,@BorrowerName
                                           ,@BorrowerContactNumber
                                           ,@BorrowerEmailAddress
                                           ,@BorrowerPermanentAddressProvinceId
                                           ,@BorrowerPermanentAddressZoneId
                                           ,@BorrowerPermanentAddressDistrictId
                                           ,@BorrowerPermanentAddressVDCMun
                                           ,@BorrowerPermanentToleStreetName
                                           ,@BorrowerPermanentHouseNumber
                                           ,@TempAddSameAsPerAdd
                                           ,@BorrowerTemporaryAddressProvinceId
                                           ,@BorrowerTemporaryAddressZoneId
                                           ,@BorrowerTemporaryAddressDistrictId
                                           ,@BorrowerTemporaryAddressVDCMun
                                           ,@BorrowerTemporaryToleStreetName
                                           ,@BorrowerTemporaryHouseNumber
                                           ,@BorrowerTypeId
                                           ,@CurrentROName
                                           ,@BranchLocationCode
                                           ,@SACROName
                                           ,@FileTransferDateToSAC
                                           ,@ProvisionStatusIdOnTheDateOfTransfer
                                           ,@DateOfRequestForBlacklisting
                                           ,@BlacklistStatusOfRequest
                                           ,@BlacklistDate
                                           ,@BlacklistNumber
                                           ,@DateOfInitiationForBookingNBA
                                           ,@NBAStatusOfRequestId
                                           ,@NBADateOfCompletion
                                           ,@NBAAmount
                                           ,@NBARemarks
                                           ,@DateOfFileHandedOverToLegalForDRT
                                           ,@DateOfFilingACaseInDRT
                                           ,@DRTStatusOfRequestId
                                           ,@DRTDateOfCompletion
                                           ,@DRTAmount
                                           ,@DRTRemarks
                                           ,@DateOfInitiationForWriteOff
                                           ,@WriteOffStatusOfRequestId
                                           ,@WriteOffDateOfCompletion
                                           ,@PrincipalOnWriteOffDate
                                           ,@InterestOnWriteOffDate
                                           ,@PrincipalRecoveredFromStartDateToLoanWriteOffDate
                                           ,@InterestRecoveredFromStartDateToLoanWriteOffDate
                                           ,@InterestOnInterestRecoveredFromStartDateToLoanWriteOffDate
                                           ,@PenalAmountRecoveredFromStartDateToLoanWriteOffDate
                                           ,@WriteOffNextActionToBeTaken
                                           ,@WriteOffDueDateOfNextAction
                                           ,@WriteOffRemarks
                                           ,@EnteredBy
                                           ,@EnteredOn)";
                    //removed fields
                    //,[BorrowerTypeIndividualLegalDetail]
                    //,[PrimaryCollateralsList]
                    //,[SecondaryCollateralsList]
                    //,[LoanAccountDetailsList]
                    //,[BranchFollowUpDetail]
                    //,[SACFollowUpDetail]
                    //,@BorrowerTypeIndividualLegalDetail
                    //,@PrimaryCollateralsList
                    //,@SecondaryCollateralsList
                    //,@LoanAccountDetailsList
                    //,@BranchFollowUpDetail
                    //,@SACFollowUpDetail
                    comForNPAMain.Parameters.AddWithValue("@CIFNo", ((object)npaModel.CIFNo) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@GroupName", ((object)npaModel.GroupName) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@BorrowerName", ((object)npaModel.BorrowerName) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@BorrowerContactNumber", ((object)npaModel.BorrowerContactNumber) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@BorrowerEmailAddress", ((object)npaModel.BorrowerEmailAddress) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@BorrowerPermanentAddressProvinceId", ((object)npaModel.BorrowerPermanentAddressProvinceId) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@BorrowerPermanentAddressZoneId", ((object)npaModel.BorrowerPermanentAddressZoneId) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@BorrowerPermanentAddressDistrictId", ((object)npaModel.BorrowerPermanentAddressDistrictId) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@BorrowerPermanentAddressVDCMun", ((object)npaModel.BorrowerPermanentAddressVDCMun) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@BorrowerPermanentToleStreetName", ((object)npaModel.BorrowerPermanentToleStreetName) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@BorrowerPermanentHouseNumber", ((object)npaModel.BorrowerPermanentHouseNumber) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@TempAddSameAsPerAdd", ((object)npaModel.TempAddSameAsPerAdd) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@BorrowerTemporaryAddressProvinceId", ((object)npaModel.BorrowerTemporaryAddressProvinceId) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@BorrowerTemporaryAddressZoneId", ((object)npaModel.BorrowerTemporaryAddressZoneId) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@BorrowerTemporaryAddressDistrictId", ((object)npaModel.BorrowerTemporaryAddressDistrictId) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@BorrowerTemporaryAddressVDCMun", ((object)npaModel.BorrowerTemporaryAddressVDCMun) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@BorrowerTemporaryToleStreetName", ((object)npaModel.BorrowerTemporaryToleStreetName) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@BorrowerTemporaryHouseNumber", ((object)npaModel.BorrowerTemporaryHouseNumber) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@BorrowerTypeId", ((object)npaModel.BorrowerTypeId) ?? DBNull.Value);
                    //comForNPAMain.Parameters.AddWithValue("@BorrowerTypeIndividualLegalDetail", ((object)npaModel.StringifiedBorrowerTypeIndividualLegalDetail) ?? DBNull.Value);
                    //comForNPAMain.Parameters.AddWithValue("@PrimaryCollateralsList", ((object)npaModel.StringifiedListOfPrimaryCollateralDetailObjects) ?? DBNull.Value);
                    //comForNPAMain.Parameters.AddWithValue("@SecondaryCollateralsList", ((object)npaModel.StringifiedListOfSecondaryCollateralDetailObjects) ?? DBNull.Value);
                    //comForNPAMain.Parameters.AddWithValue("@LoanAccountDetailsList", ((object)npaModel.StringifiedListOfLoanAccountDetailObjects) ?? DBNull.Value);
                    //comForNPAMain.Parameters.AddWithValue("@BranchFollowUpDetail", ((object)npaModel.StringifiedListOfBranchFollowUpDetailObjects) ?? DBNull.Value);
                    //comForNPAMain.Parameters.AddWithValue("@SACFollowUpDetail", ((object)npaModel.StringifiedListOfSACFollowUpDetailObjects) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@CurrentROName", ((object)npaModel.CurrentROName) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@BranchLocationCode", ((object)npaModel.BranchLocationCode) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@SACROName", ((object)npaModel.SACROName) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@FileTransferDateToSAC", ((object)npaModel.FileTransferDateToSAC) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@ProvisionStatusIdOnTheDateOfTransfer", ((object)npaModel.ProvisionStatusIdOnTheDateOfTransfer) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@DateOfRequestForBlacklisting", ((object)npaModel.DateOfRequestForBlacklisting) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@BlacklistStatusOfRequest", ((object)npaModel.BlacklistStatusOfRequest) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@BlacklistDate", ((object)npaModel.BlacklistDate) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@BlacklistNumber", ((object)npaModel.BlacklistNumber) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@DateOfInitiationForBookingNBA", ((object)npaModel.DateOfInitiationForBookingNBA) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@NBAStatusOfRequestId", ((object)npaModel.NBAStatusOfRequestId) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@NBADateOfCompletion", ((object)npaModel.NBADateOfCompletion) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@NBAAmount", npaModel.NBAAmount == null ? DBNull.Value : (((object)npaModel.NBAAmount.ToString().Replace(",", "")) ?? DBNull.Value));
                    comForNPAMain.Parameters.AddWithValue("@NBARemarks", ((object)npaModel.NBARemarks) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@DateOfFileHandedOverToLegalForDRT", ((object)npaModel.DateOfFileHandedOverToLegalForDRT) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@DateOfFilingACaseInDRT", ((object)npaModel.DateOfFilingACaseInDRT) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@DRTStatusOfRequestId", ((object)npaModel.DRTStatusOfRequestId) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@DRTDateOfCompletion", ((object)npaModel.DRTDateOfCompletion) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@DRTAmount", npaModel.DRTAmount == null ? DBNull.Value : (((object)npaModel.DRTAmount.ToString().Replace(",", "")) ?? DBNull.Value));
                    comForNPAMain.Parameters.AddWithValue("@DRTRemarks", ((object)npaModel.DRTRemarks) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@DateOfInitiationForWriteOff", ((object)npaModel.DateOfInitiationForWriteOff) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@WriteOffStatusOfRequestId", ((object)npaModel.WriteOffStatusOfRequestId) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@WriteOffDateOfCompletion", ((object)npaModel.WriteOffDateOfCompletion) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@PrincipalOnWriteOffDate", npaModel.PrincipalOnWriteOffDate == null ? DBNull.Value : (((object)npaModel.PrincipalOnWriteOffDate.ToString().Replace(",", "")) ?? DBNull.Value));
                    comForNPAMain.Parameters.AddWithValue("@InterestOnWriteOffDate", npaModel.InterestOnWriteOffDate == null ? DBNull.Value : (((object)npaModel.InterestOnWriteOffDate.ToString().Replace(",", "")) ?? DBNull.Value));
                    comForNPAMain.Parameters.AddWithValue("@PrincipalRecoveredFromStartDateToLoanWriteOffDate", npaModel.PrincipalRecoveredFromStartDateToLoanWriteOffDate == null ? DBNull.Value : (((object)npaModel.PrincipalRecoveredFromStartDateToLoanWriteOffDate.ToString().Replace(",", "")) ?? DBNull.Value));
                    comForNPAMain.Parameters.AddWithValue("@InterestRecoveredFromStartDateToLoanWriteOffDate", npaModel.InterestRecoveredFromStartDateToLoanWriteOffDate == null ? DBNull.Value : (((object)npaModel.InterestRecoveredFromStartDateToLoanWriteOffDate.ToString().Replace(",", "")) ?? DBNull.Value));
                    comForNPAMain.Parameters.AddWithValue("@InterestOnInterestRecoveredFromStartDateToLoanWriteOffDate", npaModel.InterestOnInterestRecoveredFromStartDateToLoanWriteOffDate == null ? DBNull.Value : (((object)npaModel.InterestOnInterestRecoveredFromStartDateToLoanWriteOffDate.ToString().Replace(",", "")) ?? DBNull.Value));
                    comForNPAMain.Parameters.AddWithValue("@PenalAmountRecoveredFromStartDateToLoanWriteOffDate", npaModel.PenalAmountRecoveredFromStartDateToLoanWriteOffDate == null ? DBNull.Value : (((object)npaModel.PenalAmountRecoveredFromStartDateToLoanWriteOffDate.ToString().Replace(",", "")) ?? DBNull.Value));
                    comForNPAMain.Parameters.AddWithValue("@WriteOffNextActionToBeTaken", ((object)npaModel.WriteOffNextActionToBeTaken) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@WriteOffDueDateOfNextAction", ((object)npaModel.WriteOffDueDateOfNextAction) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@WriteOffRemarks", ((object)npaModel.WriteOffRemarks) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@EnteredBy", ((object)npaModel.EnteredBy) ?? DBNull.Value);
                    comForNPAMain.Parameters.AddWithValue("@EnteredOn", ((object)npaModel.EnteredOn) ?? DBNull.Value);
                    int insertedNPAMgmtId = (int)comForNPAMain.ExecuteScalar();
                    #endregion

                    if (insertedNPAMgmtId > 0)
                    {
                        #region Inserts in child tables

                        #region Insert start for Borrower Detail table(Individual or Legal)
                        int updatedNPAMgmtIdFromBorrowerDetail = 0;
                        if (!string.IsNullOrEmpty(npaModel.StringifiedBorrowerTypeIndividualLegalDetail) && npaModel.StringifiedBorrowerTypeIndividualLegalDetail != "[]" && npaModel.StringifiedBorrowerTypeIndividualLegalDetail != "{}")
                        {

                            #region Insert case for Individual Borrower
                            if (npaModel.BorrowerTypeId == 1)
                            {
                                #region Parsing the StringifiedBorrowerTypeIndividualDetail and operating on it
                                BorrowerTypeIndividualDetail borrowerTypeIndividualDetail = JsonConvert.DeserializeObject<BorrowerTypeIndividualDetail>(npaModel.StringifiedBorrowerTypeIndividualLegalDetail);
                                if (borrowerTypeIndividualDetail != null)
                                {
                                    #region Inserting into Individual Detail
                                    comForBorrowerDetail.CommandType = CommandType.Text;
                                    comForBorrowerDetail.Connection = db;
                                    comForBorrowerDetail.Transaction = tran;
                                    comForBorrowerDetail.CommandText = @"INSERT INTO [dbo].[BorrowerTypeIndividualDetail]
                                                       ([NPAManagementId]
                                                       ,[BorrowerTypeId]
                                                       ,[BorrowerFatherName]
                                                       ,[BorrowerGrandfatherName]
                                                       ,[BorrowerCitizenshipNumber]
                                                       ,[BorrowerSpouseName]
                                                       ,[BorrowerSonName]
                                                       ,[BorrowerDaughterName]) OUTPUT INSERTED.NPAManagementId
                                                 VALUES
                                                       (@NPAManagementId,
                                                        @BorrowerTypeId,
                                                        @BorrowerFatherName,
                                                        @BorrowerGrandfatherName,
                                                        @BorrowerCitizenshipNumber,
                                                        @BorrowerSpouseName,
                                                        @BorrowerSonName,
                                                        @BorrowerDaughterName)";
                                    comForBorrowerDetail.Parameters.AddWithValue("@NPAManagementId", insertedNPAMgmtId);
                                    comForBorrowerDetail.Parameters.AddWithValue("@BorrowerTypeId", ((object)borrowerTypeIndividualDetail.BorrowerTypeId) ?? DBNull.Value);
                                    comForBorrowerDetail.Parameters.AddWithValue("@BorrowerFatherName", ((object)borrowerTypeIndividualDetail.BorrowerFatherName) ?? DBNull.Value);
                                    comForBorrowerDetail.Parameters.AddWithValue("@BorrowerGrandfatherName", ((object)borrowerTypeIndividualDetail.BorrowerGrandfatherName) ?? DBNull.Value);
                                    comForBorrowerDetail.Parameters.AddWithValue("@BorrowerCitizenshipNumber", ((object)borrowerTypeIndividualDetail.BorrowerCitizenshipNumber) ?? DBNull.Value);
                                    comForBorrowerDetail.Parameters.AddWithValue("@BorrowerSpouseName", ((object)borrowerTypeIndividualDetail.BorrowerSpouseName) ?? DBNull.Value);
                                    comForBorrowerDetail.Parameters.AddWithValue("@BorrowerSonName", ((object)borrowerTypeIndividualDetail.BorrowerSonName) ?? DBNull.Value);
                                    comForBorrowerDetail.Parameters.AddWithValue("@BorrowerDaughterName", ((object)borrowerTypeIndividualDetail.BorrowerDaughterName) ?? DBNull.Value);
                                    updatedNPAMgmtIdFromBorrowerDetail = (int)comForBorrowerDetail.ExecuteScalar();
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion

                            #region Insert case for Legal Borrower
                            else if (npaModel.BorrowerTypeId == 2)
                            {
                                #region Parsing the StringifiedBorrowerTypeLegalDetail and operating on it
                                BorrowerTypeLegalEntitiesDetail borrowerTypeIndividualDetail = JsonConvert.DeserializeObject<BorrowerTypeLegalEntitiesDetail>(npaModel.StringifiedBorrowerTypeIndividualLegalDetail);
                                if (borrowerTypeIndividualDetail != null)
                                {
                                    #region Inserting into Legal Detail
                                    comForBorrowerDetail.CommandType = CommandType.Text;
                                    comForBorrowerDetail.Connection = db;
                                    comForBorrowerDetail.Transaction = tran;
                                    comForBorrowerDetail.CommandText = @"INSERT INTO [dbo].[BorrowerTypeLegalEntitiesDetail]
                                                                        ([NPAManagementId]
                                                                        ,[BorrowerTypeId]
                                                                        ,[LegalEntityRegistrationNumber]
                                                                        ,[LegalEntityRegistrationDate]
                                                                        ,[LegalEntityRegisteredOffice]) OUTPUT INSERTED.NPAManagementId
                                                                    VALUES
                                                                        (@NPAManagementId,
                                                                         @BorrowerTypeId,
                                                                         @LegalEntityRegistrationNumber,
                                                                         @LegalEntityRegistrationDate,
                                                                         @LegalEntityRegisteredOffice)";
                                    comForBorrowerDetail.Parameters.AddWithValue("@NPAManagementId", insertedNPAMgmtId);
                                    comForBorrowerDetail.Parameters.AddWithValue("@BorrowerTypeId", ((object)borrowerTypeIndividualDetail.BorrowerTypeId) ?? DBNull.Value);
                                    comForBorrowerDetail.Parameters.AddWithValue("@LegalEntityRegistrationNumber", ((object)borrowerTypeIndividualDetail.LegalEntityRegistrationNumber) ?? DBNull.Value);
                                    comForBorrowerDetail.Parameters.AddWithValue("@LegalEntityRegistrationDate", ((object)borrowerTypeIndividualDetail.LegalEntityRegistrationDate) ?? DBNull.Value);
                                    comForBorrowerDetail.Parameters.AddWithValue("@LegalEntityRegisteredOffice", ((object)borrowerTypeIndividualDetail.LegalEntityRegisteredOffice) ?? DBNull.Value);
                                    updatedNPAMgmtIdFromBorrowerDetail = (int)comForBorrowerDetail.ExecuteScalar();
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion

                        }
                        #endregion

                        #region Proceed if insertion to Borrower Detail succeeds
                        if (updatedNPAMgmtIdFromBorrowerDetail > 0)
                        {
                            #region Insert into details of Primary Collaterals
                            if (!string.IsNullOrEmpty(npaModel.StringifiedListOfPrimaryCollateralDetailObjects) && npaModel.StringifiedListOfPrimaryCollateralDetailObjects != "[]" && npaModel.StringifiedListOfPrimaryCollateralDetailObjects != "{}")
                            {

                                #region Insert cases for Primary Collateral Details

                                #region Parsing the stringifiedListOfPrimaryCollateralDetailObjects and operating on it

                                string strListOfPrimaryCollaterals = $"{{\"primaryCollaterals\":{npaModel.StringifiedListOfPrimaryCollateralDetailObjects}}}";
                                JObject joListOfPrimaryCollaterals = JObject.Parse(strListOfPrimaryCollaterals);
                                JArray jaListOfPrimaryCollaterals = (JArray)joListOfPrimaryCollaterals["primaryCollaterals"];
                                int insertedMainIdFromPrimaryCollateralMain = 0;
                                int insertedMainIdFromPrimaryCollateralDetail = 0;
                                foreach (var itemDetail in jaListOfPrimaryCollaterals)
                                {
                                    if (itemDetail["CollateralTypeId"].ToString() != "")
                                    {
                                        insertedMainIdFromPrimaryCollateralDetail = 0;
                                        #region Insert to Collateral Main table Primary Collaterals
                                        comForPrimaryCollateralMain.CommandType = CommandType.Text;
                                        comForPrimaryCollateralMain.Connection = db;
                                        comForPrimaryCollateralMain.Transaction = tran;
                                        comForPrimaryCollateralMain.CommandText = @"INSERT INTO [dbo].[CollateralDetailMain]
                                                                                           ([NPAManagementId]
                                                                                           ,[CollateralTypeId]
                                                                                           ,[CollateralClass]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                            VALUES
                                                                                           (@NPAManagementId,
                                                                                            @CollateralTypeId,
                                                                                            @CollateralClass)";
                                        comForPrimaryCollateralMain.Parameters.AddWithValue("@NPAManagementId", insertedNPAMgmtId);
                                        comForPrimaryCollateralMain.Parameters.AddWithValue("@CollateralTypeId", ((object)itemDetail["CollateralTypeId"].ToString()) ?? DBNull.Value);
                                        comForPrimaryCollateralMain.Parameters.AddWithValue("@CollateralClass", "1");
                                        insertedMainIdFromPrimaryCollateralMain = (int)comForPrimaryCollateralMain.ExecuteScalar();
                                        comForPrimaryCollateralMain.Parameters.Clear();
                                        #endregion
                                        if (insertedMainIdFromPrimaryCollateralMain > 0)
                                        {
                                            dynamic individualPrimaryCollateral = null;
                                            switch (itemDetail["CollateralTypeId"].ToString())
                                            {
                                                case "1":
                                                    individualPrimaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfLand>();
                                                    if (individualPrimaryCollateral != null)
                                                    {
                                                        #region Inserting into Primary Collateral table(Collateral Land)
                                                        comForIndividualPrimaryCollateralDetail.CommandType = CommandType.Text;
                                                        comForIndividualPrimaryCollateralDetail.Connection = db;
                                                        comForIndividualPrimaryCollateralDetail.Transaction = tran;
                                                        comForIndividualPrimaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfLand]
                                                                                                                       ([CollateralDetailMainId]
                                                                                                                       ,[ProvinceId]
                                                                                                                       ,[ZoneId]
                                                                                                                       ,[DistrictId]
                                                                                                                       ,[VDCMun]
                                                                                                                       ,[Street]
                                                                                                                       ,[WardNumber]
                                                                                                                       ,[PlotNumber]
                                                                                                                       ,[AreaTypeId]
                                                                                                                       ,[Area]
                                                                                                                       ,[PropertyOwner]
                                                                                                                       ,[RelationWithBorrower]
                                                                                                                       ,[FirstValuatorName]
                                                                                                                       ,[FirstValuationDate]
                                                                                                                       ,[FirstFMVOfProperty]
                                                                                                                       ,[FirstValuatorExistsInSBLCurrentlyYN]
                                                                                                                       ,[LatestValuatorName]
                                                                                                                       ,[LatestValuationDate]
                                                                                                                       ,[LatestValuatorExistsInSBLCurrentlyYN]
                                                                                                                       ,[FMVOfPropertyAsPerLatestValuationReport]
                                                                                                                       ,[InsuranceCoverageType]
                                                                                                                       ,[InsuranceExpiryDate]
                                                                                                                       ,[InsuranceAmount]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                 VALUES
                                                                                                                       (@CollateralDetailMainId,
                                                                                                                        @ProvinceId,
                                                                                                                        @ZoneId,
                                                                                                                        @DistrictId,
                                                                                                                        @VDCMun,
                                                                                                                        @Street,
                                                                                                                        @WardNumber,
                                                                                                                        @PlotNumber,
                                                                                                                        @AreaTypeId,
                                                                                                                        @Area,
                                                                                                                        @PropertyOwner,
                                                                                                                        @RelationWithBorrower,
                                                                                                                        @FirstValuatorName,
                                                                                                                        @FirstValuationDate,
                                                                                                                        @FirstFMVOfProperty,
                                                                                                                        @FirstValuatorExistsInSBLCurrentlyYN,
                                                                                                                        @LatestValuatorName,
                                                                                                                        @LatestValuationDate,
                                                                                                                        @LatestValuatorExistsInSBLCurrentlyYN,
                                                                                                                        @FMVOfPropertyAsPerLatestValuationReport,
                                                                                                                        @InsuranceCoverageType,
                                                                                                                        @InsuranceExpiryDate,
                                                                                                                        @InsuranceAmount)";
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromPrimaryCollateralMain);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ProvinceId", ((object)itemDetail["ProvinceId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ZoneId", ((object)itemDetail["ZoneId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@DistrictId", ((object)itemDetail["DistrictId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@VDCMun", ((object)itemDetail["VDCMun"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@Street", ((object)itemDetail["Street"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@WardNumber", ((object)itemDetail["WardNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@PlotNumber", ((object)itemDetail["PlotNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@AreaTypeId", !string.IsNullOrEmpty(itemDetail["AreaTypeId"].ToString()) ? (object)itemDetail["AreaTypeId"].ToString() : DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@Area", ((object)itemDetail["Area"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@PropertyOwner", ((object)itemDetail["PropertyOwner"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@RelationWithBorrower", ((object)itemDetail["RelationWithBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorName", ((object)itemDetail["FirstValuatorName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuationDate", itemDetail["FirstValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["FirstValuationDate"].ToString()));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstFMVOfProperty", string.IsNullOrEmpty(itemDetail["FirstFMVOfProperty"].ToString()) ? DBNull.Value : (((object)itemDetail["FirstFMVOfProperty"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorName", ((object)itemDetail["LatestValuatorName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuationDate", itemDetail["LatestValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["LatestValuationDate"].ToString()));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FMVOfPropertyAsPerLatestValuationReport", string.IsNullOrEmpty(itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString()) ? DBNull.Value : (((object)itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceCoverageType", ((object)itemDetail["InsuranceCoverageType"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceExpiryDate", itemDetail["InsuranceExpiryDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["InsuranceExpiryDate"].ToString()));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceAmount", string.IsNullOrEmpty(itemDetail["InsuranceAmount"].ToString()) ? DBNull.Value : (((object)itemDetail["InsuranceAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        insertedMainIdFromPrimaryCollateralDetail = (int)comForIndividualPrimaryCollateralDetail.ExecuteScalar();
                                                        comForIndividualPrimaryCollateralDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                case "2":
                                                    individualPrimaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfBuilding>();
                                                    if (individualPrimaryCollateral != null)
                                                    {
                                                        #region Inserting into Primary Collateral table(Collateral Land and Building)
                                                        comForIndividualPrimaryCollateralDetail.CommandType = CommandType.Text;
                                                        comForIndividualPrimaryCollateralDetail.Connection = db;
                                                        comForIndividualPrimaryCollateralDetail.Transaction = tran;
                                                        comForIndividualPrimaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfBuilding]
                                                                                                                        ([CollateralDetailMainId]
                                                                                                                        ,[ProvinceId]
                                                                                                                        ,[ZoneId]
                                                                                                                        ,[DistrictId]
                                                                                                                        ,[VDCMun]
                                                                                                                        ,[Street]
                                                                                                                        ,[WardNumber]
                                                                                                                        ,[PlotNumber]
                                                                                                                        ,[AreaTypeId]
                                                                                                                        ,[AreaOfLand]
                                                                                                                        ,[AreaOfBuilding]
                                                                                                                        ,[PropertyOwner]
                                                                                                                        ,[RelationWithBorrower]
                                                                                                                        ,[ConstructionCompletionDate]
                                                                                                                        ,[FirstValuatorName]
                                                                                                                        ,[FirstValuationDate]
                                                                                                                        ,[FirstFMVOfProperty]
                                                                                                                        ,[FirstValuatorExistsInSBLCurrentlyYN]
                                                                                                                        ,[LatestValuatorName]
                                                                                                                        ,[LatestValuationDate]
                                                                                                                        ,[LatestValuatorExistsInSBLCurrentlyYN]
                                                                                                                        ,[FMVOfPropertyAsPerLatestValuationReport]
                                                                                                                        ,[InsuranceCoverageType]
                                                                                                                        ,[InsuranceExpiryDate]
                                                                                                                        ,[InsuranceAmount]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                    VALUES
                                                                                                                        (@CollateralDetailMainId,
                                                                                                                         @ProvinceId,
                                                                                                                         @ZoneId,
                                                                                                                         @DistrictId,
                                                                                                                         @VDCMun,
                                                                                                                         @Street,
                                                                                                                         @WardNumber,
                                                                                                                         @PlotNumber,
                                                                                                                         @AreaTypeId,
                                                                                                                         @AreaOfLand,
                                                                                                                         @AreaOfBuilding,
                                                                                                                         @PropertyOwner,
                                                                                                                         @RelationWithBorrower,
                                                                                                                         @ConstructionCompletionDate,
                                                                                                                         @FirstValuatorName,
                                                                                                                         @FirstValuationDate,
                                                                                                                         @FirstFMVOfProperty,
                                                                                                                         @FirstValuatorExistsInSBLCurrentlyYN,
                                                                                                                         @LatestValuatorName,
                                                                                                                         @LatestValuationDate,
                                                                                                                         @LatestValuatorExistsInSBLCurrentlyYN,
                                                                                                                         @FMVOfPropertyAsPerLatestValuationReport,
                                                                                                                         @InsuranceCoverageType,
                                                                                                                         @InsuranceExpiryDate,
                                                                                                                         @InsuranceAmount)";
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromPrimaryCollateralMain);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ProvinceId", ((object)itemDetail["ProvinceId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ZoneId", ((object)itemDetail["ZoneId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@DistrictId", ((object)itemDetail["DistrictId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@VDCMun", ((object)itemDetail["VDCMun"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@Street", ((object)itemDetail["Street"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@WardNumber", ((object)itemDetail["WardNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@PlotNumber", ((object)itemDetail["PlotNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@AreaTypeId", !string.IsNullOrEmpty(itemDetail["AreaTypeId"].ToString()) ? (object)itemDetail["AreaTypeId"].ToString() : DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@AreaOfLand", ((object)itemDetail["AreaOfLand"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@AreaOfBuilding", ((object)itemDetail["AreaOfBuilding"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@PropertyOwner", ((object)itemDetail["PropertyOwner"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@RelationWithBorrower", ((object)itemDetail["RelationWithBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ConstructionCompletionDate", itemDetail["ConstructionCompletionDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["ConstructionCompletionDate"].ToString()));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorName", ((object)itemDetail["FirstValuatorName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuationDate", itemDetail["FirstValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["FirstValuationDate"].ToString()));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstFMVOfProperty", string.IsNullOrEmpty(itemDetail["FirstFMVOfProperty"].ToString()) ? DBNull.Value : (((object)itemDetail["FirstFMVOfProperty"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorName", ((object)itemDetail["LatestValuatorName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuationDate", itemDetail["LatestValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["LatestValuationDate"].ToString()));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FMVOfPropertyAsPerLatestValuationReport", string.IsNullOrEmpty(itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString()) ? DBNull.Value : (((object)itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceCoverageType", ((object)itemDetail["InsuranceCoverageType"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceExpiryDate", itemDetail["InsuranceExpiryDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["InsuranceExpiryDate"].ToString()));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceAmount", string.IsNullOrEmpty(itemDetail["InsuranceAmount"].ToString()) ? DBNull.Value : (((object)itemDetail["InsuranceAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        insertedMainIdFromPrimaryCollateralDetail = (int)comForIndividualPrimaryCollateralDetail.ExecuteScalar();
                                                        comForIndividualPrimaryCollateralDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                case "3":
                                                    individualPrimaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfVehicle>();
                                                    if (individualPrimaryCollateral != null)
                                                    {
                                                        #region Inserting into Primary Collateral table(Collateral Vehicle)
                                                        comForIndividualPrimaryCollateralDetail.CommandType = CommandType.Text;
                                                        comForIndividualPrimaryCollateralDetail.Connection = db;
                                                        comForIndividualPrimaryCollateralDetail.Transaction = tran;
                                                        comForIndividualPrimaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfVehicle]
                                                                                                                       ([CollateralDetailMainId]
                                                                                                                       ,[VehicleTypeId]
                                                                                                                       ,[VehicleModel]
                                                                                                                       ,[EngineNumber]
                                                                                                                       ,[ChassisNumber]
                                                                                                                       ,[VehicleRegistrationNumber]
                                                                                                                       ,[VehicleRegistrationDate]
                                                                                                                       ,[VehicleRegisteredOffice]
                                                                                                                       ,[VehicleMakeYear]
                                                                                                                       ,[FirstValuatorName]
                                                                                                                       ,[FirstValuationDate]
                                                                                                                       ,[FirstFMVOfProperty]
                                                                                                                       ,[FirstValuatorExistsInSBLCurrentlyYN]
                                                                                                                       ,[LatestValuatorName]
                                                                                                                       ,[LatestValuationDate]
                                                                                                                       ,[LatestValuatorExistsInSBLCurrentlyYN]
                                                                                                                       ,[ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice]
                                                                                                                       ,[InsuranceCoverageType]
                                                                                                                       ,[InsuranceExpiryDate]
                                                                                                                       ,[InsuranceAmount]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                 VALUES
                                                                                                                       (@CollateralDetailMainId,
                                                                                                                        @VehicleTypeId,
                                                                                                                        @VehicleModel,
                                                                                                                        @EngineNumber,
                                                                                                                        @ChassisNumber,
                                                                                                                        @VehicleRegistrationNumber,
                                                                                                                        @VehicleRegistrationDate,
                                                                                                                        @VehicleRegisteredOffice,
                                                                                                                        @VehicleMakeYear,
                                                                                                                        @FirstValuatorName,
                                                                                                                        @FirstValuationDate,
                                                                                                                        @FirstFMVOfProperty,
                                                                                                                        @FirstValuatorExistsInSBLCurrentlyYN,
                                                                                                                        @LatestValuatorName,
                                                                                                                        @LatestValuationDate,
                                                                                                                        @LatestValuatorExistsInSBLCurrentlyYN,
                                                                                                                        @ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice,
                                                                                                                        @InsuranceCoverageType,
                                                                                                                        @InsuranceExpiryDate,
                                                                                                                        @InsuranceAmount)";
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromPrimaryCollateralMain);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@VehicleTypeId", !string.IsNullOrEmpty(itemDetail["VehicleTypeId"].ToString()) ? (object)itemDetail["VehicleTypeId"].ToString() : DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@VehicleModel", ((object)itemDetail["VehicleModel"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@EngineNumber", ((object)itemDetail["EngineNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ChassisNumber", ((object)itemDetail["ChassisNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@VehicleRegistrationNumber", ((object)itemDetail["VehicleRegistrationNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@VehicleRegistrationDate", itemDetail["VehicleRegistrationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["VehicleRegistrationDate"].ToString()));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@VehicleRegisteredOffice", ((object)itemDetail["VehicleRegisteredOffice"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@VehicleMakeYear", ((object)itemDetail["VehicleMakeYear"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorName", ((object)itemDetail["FirstValuatorName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuationDate", itemDetail["FirstValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["FirstValuationDate"].ToString()));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstFMVOfProperty", string.IsNullOrEmpty(itemDetail["FirstFMVOfProperty"].ToString()) ? DBNull.Value : (((object)itemDetail["FirstFMVOfProperty"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorName", ((object)itemDetail["LatestValuatorName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuationDate", itemDetail["LatestValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["LatestValuationDate"].ToString()));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice", string.IsNullOrEmpty(itemDetail["ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice"].ToString()) ? DBNull.Value : (((object)itemDetail["ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceCoverageType", ((object)itemDetail["InsuranceCoverageType"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceExpiryDate", itemDetail["InsuranceExpiryDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["InsuranceExpiryDate"].ToString()));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceAmount", string.IsNullOrEmpty(itemDetail["InsuranceAmount"].ToString()) ? DBNull.Value : (((object)itemDetail["InsuranceAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        insertedMainIdFromPrimaryCollateralDetail = (int)comForIndividualPrimaryCollateralDetail.ExecuteScalar();
                                                        comForIndividualPrimaryCollateralDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                case "4":
                                                    individualPrimaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfPlantAndMachinery>();
                                                    if (individualPrimaryCollateral != null)
                                                    {
                                                        #region Inserting into Primary Collateral table(Collateral Plant and Machinery)
                                                        comForIndividualPrimaryCollateralDetail.CommandType = CommandType.Text;
                                                        comForIndividualPrimaryCollateralDetail.Connection = db;
                                                        comForIndividualPrimaryCollateralDetail.Transaction = tran;
                                                        comForIndividualPrimaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfPlantAndMachinery]
                                                                                                                        ([CollateralDetailMainId]
                                                                                                                        ,[PlantAndMachineryModel]
                                                                                                                        ,[EngineNumber]
                                                                                                                        ,[ChassisNumber]
                                                                                                                        ,[RegistrationNumber]
                                                                                                                        ,[FirstValuatorName]
                                                                                                                        ,[FirstValuationDate]
                                                                                                                        ,[FirstFMVOfProperty]
                                                                                                                        ,[FirstValuatorExistsInSBLCurrentlyYN]
                                                                                                                        ,[LatestValuatorName]
                                                                                                                        ,[LatestValuationDate]
                                                                                                                        ,[LatestValuatorExistsInSBLCurrentlyYN]
                                                                                                                        ,[FMVOfPropertyAsPerLatestValuationReport]
                                                                                                                        ,[InsuranceCoverageType]
                                                                                                                        ,[InsuranceExpiryDate]
                                                                                                                        ,[InsuranceAmount]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                    VALUES
                                                                                                                        (@CollateralDetailMainId,
                                                                                                                         @PlantAndMachineryModel,
                                                                                                                         @EngineNumber,
                                                                                                                         @ChassisNumber,
                                                                                                                         @RegistrationNumber,
                                                                                                                         @FirstValuatorName,
                                                                                                                         @FirstValuationDate,
                                                                                                                         @FirstFMVOfProperty,
                                                                                                                         @FirstValuatorExistsInSBLCurrentlyYN,
                                                                                                                         @LatestValuatorName,
                                                                                                                         @LatestValuationDate,
                                                                                                                         @LatestValuatorExistsInSBLCurrentlyYN,
                                                                                                                         @FMVOfPropertyAsPerLatestValuationReport,
                                                                                                                         @InsuranceCoverageType,
                                                                                                                         @InsuranceExpiryDate,
                                                                                                                         @InsuranceAmount)";
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromPrimaryCollateralMain);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@PlantAndMachineryModel", ((object)itemDetail["PlantAndMachineryModel"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@EngineNumber", ((object)itemDetail["EngineNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ChassisNumber", ((object)itemDetail["ChassisNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@RegistrationNumber", ((object)itemDetail["RegistrationNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorName", ((object)itemDetail["FirstValuatorName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuationDate", itemDetail["FirstValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["FirstValuationDate"].ToString()));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstFMVOfProperty", string.IsNullOrEmpty(itemDetail["FirstFMVOfProperty"].ToString()) ? DBNull.Value : (((object)itemDetail["FirstFMVOfProperty"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorName", ((object)itemDetail["LatestValuatorName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuationDate", itemDetail["LatestValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["LatestValuationDate"].ToString()));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FMVOfPropertyAsPerLatestValuationReport", string.IsNullOrEmpty(itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString()) ? DBNull.Value : (((object)itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceCoverageType", ((object)itemDetail["InsuranceCoverageType"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceExpiryDate", itemDetail["InsuranceExpiryDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["InsuranceExpiryDate"].ToString()));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceAmount", string.IsNullOrEmpty(itemDetail["InsuranceAmount"].ToString()) ? DBNull.Value : (((object)itemDetail["InsuranceAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        insertedMainIdFromPrimaryCollateralDetail = (int)comForIndividualPrimaryCollateralDetail.ExecuteScalar();
                                                        comForIndividualPrimaryCollateralDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                case "5":
                                                    individualPrimaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfCurrentAsset>();
                                                    if (individualPrimaryCollateral != null)
                                                    {
                                                        #region Inserting into Primary Collateral table(Collateral Current Asset)
                                                        comForIndividualPrimaryCollateralDetail.CommandType = CommandType.Text;
                                                        comForIndividualPrimaryCollateralDetail.Connection = db;
                                                        comForIndividualPrimaryCollateralDetail.Transaction = tran;
                                                        comForIndividualPrimaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfCurrentAsset]
                                                                                                                        ([CollateralDetailMainId]
                                                                                                                       ,[DateOfLatestStockInspectionReportCollectedByBranch]
                                                                                                                       ,[WorkingCapitalAsPerStockReport]
                                                                                                                       ,[WorkingCapitalAsPerStockInspector]
                                                                                                                       ,[DrawingPower]
                                                                                                                       ,[InsuranceCoverageType]
                                                                                                                       ,[InsuranceExpiryDate]
                                                                                                                       ,[InsuranceAmount]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                 VALUES
                                                                                                                       (@CollateralDetailMainId,
                                                                                                                        @DateOfLatestStockInspectionReportCollectedByBranch,
                                                                                                                        @WorkingCapitalAsPerStockReport,
                                                                                                                        @WorkingCapitalAsPerStockInspector,
                                                                                                                        @DrawingPower,
                                                                                                                        @InsuranceCoverageType,
                                                                                                                        @InsuranceExpiryDate,
                                                                                                                        @InsuranceAmount)";
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromPrimaryCollateralMain);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@DateOfLatestStockInspectionReportCollectedByBranch", itemDetail["DateOfLatestStockInspectionReportCollectedByBranch"].ToString() == "" ? DBNull.Value : ((object)itemDetail["DateOfLatestStockInspectionReportCollectedByBranch"].ToString()));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@WorkingCapitalAsPerStockReport", string.IsNullOrEmpty(itemDetail["WorkingCapitalAsPerStockReport"].ToString()) ? DBNull.Value : (((object)itemDetail["WorkingCapitalAsPerStockReport"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@WorkingCapitalAsPerStockInspector", string.IsNullOrEmpty(itemDetail["WorkingCapitalAsPerStockInspector"].ToString()) ? DBNull.Value : (((object)itemDetail["WorkingCapitalAsPerStockInspector"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@DrawingPower", ((object)itemDetail["DrawingPower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceCoverageType", ((object)itemDetail["InsuranceCoverageType"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceExpiryDate", itemDetail["InsuranceExpiryDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["InsuranceExpiryDate"].ToString()));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceAmount", string.IsNullOrEmpty(itemDetail["InsuranceAmount"].ToString()) ? DBNull.Value : (((object)itemDetail["InsuranceAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        insertedMainIdFromPrimaryCollateralDetail = (int)comForIndividualPrimaryCollateralDetail.ExecuteScalar();
                                                        comForIndividualPrimaryCollateralDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                case "6":
                                                    individualPrimaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfPersonalGuarantee>();
                                                    if (individualPrimaryCollateral != null)
                                                    {
                                                        #region Inserting into Primary Collateral table(Collateral Personal Guarantee)
                                                        comForIndividualPrimaryCollateralDetail.CommandType = CommandType.Text;
                                                        comForIndividualPrimaryCollateralDetail.Connection = db;
                                                        comForIndividualPrimaryCollateralDetail.Transaction = tran;
                                                        comForIndividualPrimaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfPersonalGuarantee]
                                                                                                                       ([CollateralDetailMainId]
                                                                                                                       ,[NameOfGuarantor]
                                                                                                                       ,[CitizenshipNumber]
                                                                                                                       ,[FatherName]
                                                                                                                       ,[GrandfatherName]
                                                                                                                       ,[PermanentAddress]
                                                                                                                       ,[CurrentAddress]
                                                                                                                       ,[NetworthOfGuarantor]
                                                                                                                       ,[RelationWithBorrower]
                                                                                                                       ,[ContactNumber]
                                                                                                                       ,[Profession]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                 VALUES
                                                                                                                       (@CollateralDetailMainId,
                                                                                                                        @NameOfGuarantor,
                                                                                                                        @CitizenshipNumber,
                                                                                                                        @FatherName,
                                                                                                                        @GrandfatherName,
                                                                                                                        @PermanentAddress,
                                                                                                                        @CurrentAddress,
                                                                                                                        @NetworthOfGuarantor,
                                                                                                                        @RelationWithBorrower,
                                                                                                                        @ContactNumber,
                                                                                                                        @Profession)";
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromPrimaryCollateralMain);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@NameOfGuarantor", ((object)itemDetail["NameOfGuarantor"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CitizenshipNumber", ((object)itemDetail["CitizenshipNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FatherName", ((object)itemDetail["FatherName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@GrandfatherName", ((object)itemDetail["GrandfatherName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@PermanentAddress", ((object)itemDetail["PermanentAddress"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CurrentAddress", ((object)itemDetail["CurrentAddress"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@NetworthOfGuarantor", string.IsNullOrEmpty(itemDetail["NetworthOfGuarantor"].ToString()) ? DBNull.Value : (((object)itemDetail["NetworthOfGuarantor"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@RelationWithBorrower", ((object)itemDetail["RelationWithBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ContactNumber", ((object)itemDetail["ContactNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@Profession", ((object)itemDetail["Profession"].ToString()) ?? DBNull.Value);
                                                        insertedMainIdFromPrimaryCollateralDetail = (int)comForIndividualPrimaryCollateralDetail.ExecuteScalar();
                                                        comForIndividualPrimaryCollateralDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                case "7":
                                                    individualPrimaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfCorporateGuarantee>();
                                                    if (individualPrimaryCollateral != null)
                                                    {
                                                        #region Inserting into Primary Collateral table(Collateral Corporate Guarantee)
                                                        comForIndividualPrimaryCollateralDetail.CommandType = CommandType.Text;
                                                        comForIndividualPrimaryCollateralDetail.Connection = db;
                                                        comForIndividualPrimaryCollateralDetail.Transaction = tran;
                                                        comForIndividualPrimaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfCorporateGuarantee]
                                                                                                                       ([CollateralDetailMainId]
                                                                                                                       ,[NameOfFirm]
                                                                                                                       ,[RegistrationNumber]
                                                                                                                       ,[FirmAddress]
                                                                                                                       ,[NetworthOfTheFirm]
                                                                                                                       ,[ContactPerson]
                                                                                                                       ,[ContactNumber]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                 VALUES
                                                                                                                       (@CollateralDetailMainId,
                                                                                                                        @NameOfFirm,
                                                                                                                        @RegistrationNumber,
                                                                                                                        @FirmAddress,
                                                                                                                        @NetworthOfTheFirm,
                                                                                                                        @ContactPerson,
                                                                                                                        @ContactNumber)";
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromPrimaryCollateralMain);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@NameOfFirm", ((object)itemDetail["NameOfFirm"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@RegistrationNumber", ((object)itemDetail["RegistrationNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirmAddress", ((object)itemDetail["FirmAddress"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@NetworthOfTheFirm", string.IsNullOrEmpty(itemDetail["NetworthOfTheFirm"].ToString()) ? DBNull.Value : (((object)itemDetail["NetworthOfTheFirm"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ContactPerson", ((object)itemDetail["ContactPerson"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ContactNumber", ((object)itemDetail["ContactNumber"].ToString()) ?? DBNull.Value);
                                                        insertedMainIdFromPrimaryCollateralDetail = (int)comForIndividualPrimaryCollateralDetail.ExecuteScalar();
                                                        comForIndividualPrimaryCollateralDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                case "8":
                                                    individualPrimaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfStock>();
                                                    if (individualPrimaryCollateral != null)
                                                    {
                                                        #region Inserting into Primary Collateral table(Collateral Stock)
                                                        comForIndividualPrimaryCollateralDetail.CommandType = CommandType.Text;
                                                        comForIndividualPrimaryCollateralDetail.Connection = db;
                                                        comForIndividualPrimaryCollateralDetail.Transaction = tran;
                                                        comForIndividualPrimaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfStock]
                                                                                                                                       ([CollateralDetailMainId]
                                                                                                                                       ,[ShareTypeId]
                                                                                                                                       ,[CompanyName]
                                                                                                                                       ,[ListedInNepseBoolId]
                                                                                                                                       ,[PledgedUnits]
                                                                                                                                       ,[UnitTypeId]
                                                                                                                                       ,[ValueOfShare]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                                 VALUES
                                                                                                                                       (@CollateralDetailMainId,
			                                                                                                                            @ShareTypeId,
			                                                                                                                            @CompanyName,
			                                                                                                                            @ListedInNepseBoolId,
			                                                                                                                            @PledgedUnits,
			                                                                                                                            @UnitTypeId,
			                                                                                                                            @ValueOfShare)";
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromPrimaryCollateralMain);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ShareTypeId", ((object)itemDetail["ShareTypeId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CompanyName", ((object)itemDetail["CompanyName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ListedInNepseBoolId", ((object)itemDetail["ListedInNepseBoolId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@PledgedUnits", string.IsNullOrEmpty(itemDetail["PledgedUnits"].ToString()) ? DBNull.Value : (((object)itemDetail["PledgedUnits"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@UnitTypeId", ((object)itemDetail["ShareUnitTypeId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ValueOfShare", string.IsNullOrEmpty(itemDetail["ValueOfShare"].ToString()) ? DBNull.Value : (((object)itemDetail["ValueOfShare"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        insertedMainIdFromPrimaryCollateralDetail = (int)comForIndividualPrimaryCollateralDetail.ExecuteScalar();
                                                        comForIndividualPrimaryCollateralDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                case "9":
                                                    individualPrimaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfGold>();
                                                    if (individualPrimaryCollateral != null)
                                                    {
                                                        #region Inserting into Primary Collateral table(Collateral Gold)
                                                        comForIndividualPrimaryCollateralDetail.CommandType = CommandType.Text;
                                                        comForIndividualPrimaryCollateralDetail.Connection = db;
                                                        comForIndividualPrimaryCollateralDetail.Transaction = tran;
                                                        comForIndividualPrimaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfGold]
                                                                                                                                   ([CollateralDetailMainId]
                                                                                                                                   ,[Quantity]
                                                                                                                                   ,[MeasurementUnitId]
                                                                                                                                   ,[GoldTypeId]
                                                                                                                                   ,[ValueOfGold]
                                                                                                                                   ,[NameOfGoldTester]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                             VALUES
                                                                                                                                   (@CollateralDetailMainId,
                                                                                                                                    @Quantity,
                                                                                                                                    @MeasurementUnitId,
                                                                                                                                    @GoldTypeId,
                                                                                                                                    @ValueOfGold,
                                                                                                                                    @NameOfGoldTester)";
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromPrimaryCollateralMain);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@Quantity", ((object)itemDetail["Quantity"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@MeasurementUnitId", ((object)itemDetail["MeasurementUnitId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@GoldTypeId", ((object)itemDetail["GoldTypeId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ValueOfGold", string.IsNullOrEmpty(itemDetail["ValueOfGold"].ToString()) ? DBNull.Value : (((object)itemDetail["ValueOfGold"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@NameOfGoldTester", ((object)itemDetail["NameOfGoldTester"].ToString()) ?? DBNull.Value);
                                                        insertedMainIdFromPrimaryCollateralDetail = (int)comForIndividualPrimaryCollateralDetail.ExecuteScalar();
                                                        comForIndividualPrimaryCollateralDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #endregion

                            }
                            #endregion

                            #region Insert into details of Secondary Collaterals
                            if (!string.IsNullOrEmpty(npaModel.StringifiedListOfSecondaryCollateralDetailObjects) && npaModel.StringifiedListOfSecondaryCollateralDetailObjects != "[]" && npaModel.StringifiedListOfSecondaryCollateralDetailObjects != "{}")
                            {

                                #region Insert cases for Secondary Collateral Details

                                #region Parsing the stringifiedListOfSecondaryCollateralDetailObjects and operating on it

                                string strListOfSecondaryCollaterals = $"{{\"secondaryCollaterals\":{npaModel.StringifiedListOfSecondaryCollateralDetailObjects}}}";
                                JObject joListOfSecondaryCollaterals = JObject.Parse(strListOfSecondaryCollaterals);
                                JArray jaListOfSecondaryCollaterals = (JArray)joListOfSecondaryCollaterals["secondaryCollaterals"];
                                int insertedMainIdFromSecondaryCollateralMain = 0;
                                int insertedMainIdFromSecondaryCollateralDetail = 0;
                                foreach (var itemDetail in jaListOfSecondaryCollaterals)
                                {
                                    if (itemDetail["CollateralTypeId"].ToString() != "")
                                    {
                                        insertedMainIdFromSecondaryCollateralDetail = 0;
                                        #region Insert to Collateral Main table Secondary Collaterals
                                        comForSecondaryCollateralMain.CommandType = CommandType.Text;
                                        comForSecondaryCollateralMain.Connection = db;
                                        comForSecondaryCollateralMain.Transaction = tran;
                                        comForSecondaryCollateralMain.CommandText = @"INSERT INTO [dbo].[CollateralDetailMain]
                                                                                           ([NPAManagementId]
                                                                                           ,[CollateralTypeId]
                                                                                           ,[CollateralClass]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                            VALUES
                                                                                           (@NPAManagementId,
                                                                                            @CollateralTypeId,
                                                                                            @CollateralClass)";
                                        comForSecondaryCollateralMain.Parameters.AddWithValue("@NPAManagementId", insertedNPAMgmtId);
                                        comForSecondaryCollateralMain.Parameters.AddWithValue("@CollateralTypeId", ((object)itemDetail["CollateralTypeId"].ToString()) ?? DBNull.Value);
                                        comForSecondaryCollateralMain.Parameters.AddWithValue("@CollateralClass", "2");
                                        insertedMainIdFromSecondaryCollateralMain = (int)comForSecondaryCollateralMain.ExecuteScalar();
                                        comForSecondaryCollateralMain.Parameters.Clear();
                                        #endregion
                                        if (insertedMainIdFromSecondaryCollateralMain > 0)
                                        {
                                            dynamic individualSecondaryCollateral = null;
                                            switch (itemDetail["CollateralTypeId"].ToString())
                                            {
                                                case "1":
                                                    individualSecondaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfLand>();
                                                    if (individualSecondaryCollateral != null)
                                                    {
                                                        #region Inserting into Secondary Collateral table(Collateral Land)
                                                        comForIndividualSecondaryCollateralDetail.CommandType = CommandType.Text;
                                                        comForIndividualSecondaryCollateralDetail.Connection = db;
                                                        comForIndividualSecondaryCollateralDetail.Transaction = tran;
                                                        comForIndividualSecondaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfLand]
                                                                                                                       ([CollateralDetailMainId]
                                                                                                                       ,[ProvinceId]
                                                                                                                       ,[ZoneId]
                                                                                                                       ,[DistrictId]
                                                                                                                       ,[VDCMun]
                                                                                                                       ,[Street]
                                                                                                                       ,[WardNumber]
                                                                                                                       ,[PlotNumber]
                                                                                                                       ,[AreaTypeId]
                                                                                                                       ,[Area]
                                                                                                                       ,[PropertyOwner]
                                                                                                                       ,[RelationWithBorrower]
                                                                                                                       ,[FirstValuatorName]
                                                                                                                       ,[FirstValuationDate]
                                                                                                                       ,[FirstFMVOfProperty]
                                                                                                                       ,[FirstValuatorExistsInSBLCurrentlyYN]
                                                                                                                       ,[LatestValuatorName]
                                                                                                                       ,[LatestValuationDate]
                                                                                                                       ,[LatestValuatorExistsInSBLCurrentlyYN]
                                                                                                                       ,[FMVOfPropertyAsPerLatestValuationReport]
                                                                                                                       ,[InsuranceCoverageType]
                                                                                                                       ,[InsuranceExpiryDate]
                                                                                                                       ,[InsuranceAmount]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                 VALUES
                                                                                                                       (@CollateralDetailMainId,
                                                                                                                        @ProvinceId,
                                                                                                                        @ZoneId,
                                                                                                                        @DistrictId,
                                                                                                                        @VDCMun,
                                                                                                                        @Street,
                                                                                                                        @WardNumber,
                                                                                                                        @PlotNumber,
                                                                                                                        @AreaTypeId,
                                                                                                                        @Area,
                                                                                                                        @PropertyOwner,
                                                                                                                        @RelationWithBorrower,
                                                                                                                        @FirstValuatorName,
                                                                                                                        @FirstValuationDate,
                                                                                                                        @FirstFMVOfProperty,
                                                                                                                        @FirstValuatorExistsInSBLCurrentlyYN,
                                                                                                                        @LatestValuatorName,
                                                                                                                        @LatestValuationDate,
                                                                                                                        @LatestValuatorExistsInSBLCurrentlyYN,
                                                                                                                        @FMVOfPropertyAsPerLatestValuationReport,
                                                                                                                        @InsuranceCoverageType,
                                                                                                                        @InsuranceExpiryDate,
                                                                                                                        @InsuranceAmount)";
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromSecondaryCollateralMain);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ProvinceId", ((object)itemDetail["ProvinceId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ZoneId", ((object)itemDetail["ZoneId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@DistrictId", ((object)itemDetail["DistrictId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@VDCMun", ((object)itemDetail["VDCMun"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@Street", ((object)itemDetail["Street"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@WardNumber", ((object)itemDetail["WardNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@PlotNumber", ((object)itemDetail["PlotNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@AreaTypeId", !string.IsNullOrEmpty(itemDetail["AreaTypeId"].ToString()) ? (object)itemDetail["AreaTypeId"].ToString() : DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@Area", ((object)itemDetail["Area"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@PropertyOwner", ((object)itemDetail["PropertyOwner"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@RelationWithBorrower", ((object)itemDetail["RelationWithBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorName", ((object)itemDetail["FirstValuatorName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuationDate", itemDetail["FirstValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["FirstValuationDate"].ToString()));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstFMVOfProperty", string.IsNullOrEmpty(itemDetail["FirstFMVOfProperty"].ToString()) ? DBNull.Value : (((object)itemDetail["FirstFMVOfProperty"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorName", ((object)itemDetail["LatestValuatorName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuationDate", itemDetail["LatestValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["LatestValuationDate"].ToString()));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FMVOfPropertyAsPerLatestValuationReport", string.IsNullOrEmpty(itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString()) ? DBNull.Value : (((object)itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceCoverageType", ((object)itemDetail["InsuranceCoverageType"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceExpiryDate", itemDetail["InsuranceExpiryDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["InsuranceExpiryDate"].ToString()));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceAmount", string.IsNullOrEmpty(itemDetail["InsuranceAmount"].ToString()) ? DBNull.Value : (((object)itemDetail["InsuranceAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        insertedMainIdFromSecondaryCollateralDetail = (int)comForIndividualSecondaryCollateralDetail.ExecuteScalar();
                                                        comForIndividualSecondaryCollateralDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                case "2":
                                                    individualSecondaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfBuilding>();
                                                    if (individualSecondaryCollateral != null)
                                                    {
                                                        #region Inserting into Secondary Collateral table(Collateral Land and Building)
                                                        comForIndividualSecondaryCollateralDetail.CommandType = CommandType.Text;
                                                        comForIndividualSecondaryCollateralDetail.Connection = db;
                                                        comForIndividualSecondaryCollateralDetail.Transaction = tran;
                                                        comForIndividualSecondaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfBuilding]
                                                                                                                        ([CollateralDetailMainId]
                                                                                                                        ,[ProvinceId]
                                                                                                                        ,[ZoneId]
                                                                                                                        ,[DistrictId]
                                                                                                                        ,[VDCMun]
                                                                                                                        ,[Street]
                                                                                                                        ,[WardNumber]
                                                                                                                        ,[PlotNumber]
                                                                                                                        ,[AreaTypeId]
                                                                                                                        ,[AreaOfLand]
                                                                                                                        ,[AreaOfBuilding]
                                                                                                                        ,[PropertyOwner]
                                                                                                                        ,[RelationWithBorrower]
                                                                                                                        ,[ConstructionCompletionDate]
                                                                                                                        ,[FirstValuatorName]
                                                                                                                        ,[FirstValuationDate]
                                                                                                                        ,[FirstFMVOfProperty]
                                                                                                                        ,[FirstValuatorExistsInSBLCurrentlyYN]
                                                                                                                        ,[LatestValuatorName]
                                                                                                                        ,[LatestValuationDate]
                                                                                                                        ,[LatestValuatorExistsInSBLCurrentlyYN]
                                                                                                                        ,[FMVOfPropertyAsPerLatestValuationReport]
                                                                                                                        ,[InsuranceCoverageType]
                                                                                                                        ,[InsuranceExpiryDate]
                                                                                                                        ,[InsuranceAmount]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                    VALUES
                                                                                                                        (@CollateralDetailMainId,
                                                                                                                         @ProvinceId,
                                                                                                                         @ZoneId,
                                                                                                                         @DistrictId,
                                                                                                                         @VDCMun,
                                                                                                                         @Street,
                                                                                                                         @WardNumber,
                                                                                                                         @PlotNumber,
                                                                                                                         @AreaTypeId,
                                                                                                                         @AreaOfLand,
                                                                                                                         @AreaOfBuilding,
                                                                                                                         @PropertyOwner,
                                                                                                                         @RelationWithBorrower,
                                                                                                                         @ConstructionCompletionDate,
                                                                                                                         @FirstValuatorName,
                                                                                                                         @FirstValuationDate,
                                                                                                                         @FirstFMVOfProperty,
                                                                                                                         @FirstValuatorExistsInSBLCurrentlyYN,
                                                                                                                         @LatestValuatorName,
                                                                                                                         @LatestValuationDate,
                                                                                                                         @LatestValuatorExistsInSBLCurrentlyYN,
                                                                                                                         @FMVOfPropertyAsPerLatestValuationReport,
                                                                                                                         @InsuranceCoverageType,
                                                                                                                         @InsuranceExpiryDate,
                                                                                                                         @InsuranceAmount)";
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromSecondaryCollateralMain);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ProvinceId", ((object)itemDetail["ProvinceId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ZoneId", ((object)itemDetail["ZoneId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@DistrictId", ((object)itemDetail["DistrictId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@VDCMun", ((object)itemDetail["VDCMun"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@Street", ((object)itemDetail["Street"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@WardNumber", ((object)itemDetail["WardNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@PlotNumber", ((object)itemDetail["PlotNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@AreaTypeId", !string.IsNullOrEmpty(itemDetail["AreaTypeId"].ToString()) ? (object)itemDetail["AreaTypeId"].ToString() : DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@AreaOfLand", ((object)itemDetail["AreaOfLand"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@AreaOfBuilding", ((object)itemDetail["AreaOfBuilding"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@PropertyOwner", ((object)itemDetail["PropertyOwner"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@RelationWithBorrower", ((object)itemDetail["RelationWithBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ConstructionCompletionDate", itemDetail["ConstructionCompletionDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["ConstructionCompletionDate"].ToString()));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorName", ((object)itemDetail["FirstValuatorName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuationDate", itemDetail["FirstValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["FirstValuationDate"].ToString()));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstFMVOfProperty", string.IsNullOrEmpty(itemDetail["FirstFMVOfProperty"].ToString()) ? DBNull.Value : (((object)itemDetail["FirstFMVOfProperty"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorName", ((object)itemDetail["LatestValuatorName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuationDate", itemDetail["LatestValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["LatestValuationDate"].ToString()));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FMVOfPropertyAsPerLatestValuationReport", string.IsNullOrEmpty(itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString()) ? DBNull.Value : (((object)itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceCoverageType", ((object)itemDetail["InsuranceCoverageType"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceExpiryDate", itemDetail["InsuranceExpiryDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["InsuranceExpiryDate"].ToString()));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceAmount", string.IsNullOrEmpty(itemDetail["InsuranceAmount"].ToString()) ? DBNull.Value : (((object)itemDetail["InsuranceAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        insertedMainIdFromSecondaryCollateralDetail = (int)comForIndividualSecondaryCollateralDetail.ExecuteScalar();
                                                        comForIndividualSecondaryCollateralDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                case "3":
                                                    individualSecondaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfVehicle>();
                                                    if (individualSecondaryCollateral != null)
                                                    {
                                                        #region Inserting into Secondary Collateral table(Collateral Vehicle)
                                                        comForIndividualSecondaryCollateralDetail.CommandType = CommandType.Text;
                                                        comForIndividualSecondaryCollateralDetail.Connection = db;
                                                        comForIndividualSecondaryCollateralDetail.Transaction = tran;
                                                        comForIndividualSecondaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfVehicle]
                                                                                                                       ([CollateralDetailMainId]
                                                                                                                       ,[VehicleTypeId]
                                                                                                                       ,[VehicleModel]
                                                                                                                       ,[EngineNumber]
                                                                                                                       ,[ChassisNumber]
                                                                                                                       ,[VehicleRegistrationNumber]
                                                                                                                       ,[VehicleRegistrationDate]
                                                                                                                       ,[VehicleRegisteredOffice]
                                                                                                                       ,[VehicleMakeYear]
                                                                                                                       ,[FirstValuatorName]
                                                                                                                       ,[FirstValuationDate]
                                                                                                                       ,[FirstFMVOfProperty]
                                                                                                                       ,[FirstValuatorExistsInSBLCurrentlyYN]
                                                                                                                       ,[LatestValuatorName]
                                                                                                                       ,[LatestValuationDate]
                                                                                                                       ,[LatestValuatorExistsInSBLCurrentlyYN]
                                                                                                                       ,[ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice]
                                                                                                                       ,[InsuranceCoverageType]
                                                                                                                       ,[InsuranceExpiryDate]
                                                                                                                       ,[InsuranceAmount]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                 VALUES
                                                                                                                       (@CollateralDetailMainId,
                                                                                                                        @VehicleTypeId,
                                                                                                                        @VehicleModel,
                                                                                                                        @EngineNumber,
                                                                                                                        @ChassisNumber,
                                                                                                                        @VehicleRegistrationNumber,
                                                                                                                        @VehicleRegistrationDate,
                                                                                                                        @VehicleRegisteredOffice,
                                                                                                                        @VehicleMakeYear,
                                                                                                                        @FirstValuatorName,
                                                                                                                        @FirstValuationDate,
                                                                                                                        @FirstFMVOfProperty,
                                                                                                                        @FirstValuatorExistsInSBLCurrentlyYN,
                                                                                                                        @LatestValuatorName,
                                                                                                                        @LatestValuationDate,
                                                                                                                        @LatestValuatorExistsInSBLCurrentlyYN,
                                                                                                                        @ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice,
                                                                                                                        @InsuranceCoverageType,
                                                                                                                        @InsuranceExpiryDate,
                                                                                                                        @InsuranceAmount)";
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromSecondaryCollateralMain);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@VehicleTypeId", !string.IsNullOrEmpty(itemDetail["VehicleTypeId"].ToString()) ? (object)itemDetail["VehicleTypeId"].ToString() : DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@VehicleModel", ((object)itemDetail["VehicleModel"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@EngineNumber", ((object)itemDetail["EngineNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ChassisNumber", ((object)itemDetail["ChassisNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@VehicleRegistrationNumber", ((object)itemDetail["VehicleRegistrationNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@VehicleRegistrationDate", itemDetail["VehicleRegistrationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["VehicleRegistrationDate"].ToString()));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@VehicleRegisteredOffice", ((object)itemDetail["VehicleRegisteredOffice"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@VehicleMakeYear", ((object)itemDetail["VehicleMakeYear"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorName", ((object)itemDetail["FirstValuatorName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuationDate", itemDetail["FirstValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["FirstValuationDate"].ToString()));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstFMVOfProperty", string.IsNullOrEmpty(itemDetail["FirstFMVOfProperty"].ToString()) ? DBNull.Value : (((object)itemDetail["FirstFMVOfProperty"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorName", ((object)itemDetail["LatestValuatorName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuationDate", itemDetail["LatestValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["LatestValuationDate"].ToString()));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice", string.IsNullOrEmpty(itemDetail["ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice"].ToString()) ? DBNull.Value : (((object)itemDetail["ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceCoverageType", ((object)itemDetail["InsuranceCoverageType"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceExpiryDate", itemDetail["InsuranceExpiryDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["InsuranceExpiryDate"].ToString()));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceAmount", string.IsNullOrEmpty(itemDetail["InsuranceAmount"].ToString()) ? DBNull.Value : (((object)itemDetail["InsuranceAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        insertedMainIdFromSecondaryCollateralDetail = (int)comForIndividualSecondaryCollateralDetail.ExecuteScalar();
                                                        comForIndividualSecondaryCollateralDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                case "4":
                                                    individualSecondaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfPlantAndMachinery>();
                                                    if (individualSecondaryCollateral != null)
                                                    {
                                                        #region Inserting into Secondary Collateral table(Collateral Plant and Machinery)
                                                        comForIndividualSecondaryCollateralDetail.CommandType = CommandType.Text;
                                                        comForIndividualSecondaryCollateralDetail.Connection = db;
                                                        comForIndividualSecondaryCollateralDetail.Transaction = tran;
                                                        comForIndividualSecondaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfPlantAndMachinery]
                                                                                                                        ([CollateralDetailMainId]
                                                                                                                        ,[PlantAndMachineryModel]
                                                                                                                        ,[EngineNumber]
                                                                                                                        ,[ChassisNumber]
                                                                                                                        ,[RegistrationNumber]
                                                                                                                        ,[FirstValuatorName]
                                                                                                                        ,[FirstValuationDate]
                                                                                                                        ,[FirstFMVOfProperty]
                                                                                                                        ,[FirstValuatorExistsInSBLCurrentlyYN]
                                                                                                                        ,[LatestValuatorName]
                                                                                                                        ,[LatestValuationDate]
                                                                                                                        ,[LatestValuatorExistsInSBLCurrentlyYN]
                                                                                                                        ,[FMVOfPropertyAsPerLatestValuationReport]
                                                                                                                        ,[InsuranceCoverageType]
                                                                                                                        ,[InsuranceExpiryDate]
                                                                                                                        ,[InsuranceAmount]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                    VALUES
                                                                                                                        (@CollateralDetailMainId,
                                                                                                                         @PlantAndMachineryModel,
                                                                                                                         @EngineNumber,
                                                                                                                         @ChassisNumber,
                                                                                                                         @RegistrationNumber,
                                                                                                                         @FirstValuatorName,
                                                                                                                         @FirstValuationDate,
                                                                                                                         @FirstFMVOfProperty,
                                                                                                                         @FirstValuatorExistsInSBLCurrentlyYN,
                                                                                                                         @LatestValuatorName,
                                                                                                                         @LatestValuationDate,
                                                                                                                         @LatestValuatorExistsInSBLCurrentlyYN,
                                                                                                                         @FMVOfPropertyAsPerLatestValuationReport,
                                                                                                                         @InsuranceCoverageType,
                                                                                                                         @InsuranceExpiryDate,
                                                                                                                         @InsuranceAmount)";
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromSecondaryCollateralMain);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@PlantAndMachineryModel", ((object)itemDetail["PlantAndMachineryModel"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@EngineNumber", ((object)itemDetail["EngineNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ChassisNumber", ((object)itemDetail["ChassisNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@RegistrationNumber", ((object)itemDetail["RegistrationNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorName", ((object)itemDetail["FirstValuatorName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuationDate", itemDetail["FirstValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["FirstValuationDate"].ToString()));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstFMVOfProperty", string.IsNullOrEmpty(itemDetail["FirstFMVOfProperty"].ToString()) ? DBNull.Value : (((object)itemDetail["FirstFMVOfProperty"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorName", ((object)itemDetail["LatestValuatorName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuationDate", itemDetail["LatestValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["LatestValuationDate"].ToString()));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FMVOfPropertyAsPerLatestValuationReport", string.IsNullOrEmpty(itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString()) ? DBNull.Value : (((object)itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceCoverageType", ((object)itemDetail["InsuranceCoverageType"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceExpiryDate", itemDetail["InsuranceExpiryDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["InsuranceExpiryDate"].ToString()));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceAmount", string.IsNullOrEmpty(itemDetail["InsuranceAmount"].ToString()) ? DBNull.Value : (((object)itemDetail["InsuranceAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        insertedMainIdFromSecondaryCollateralDetail = (int)comForIndividualSecondaryCollateralDetail.ExecuteScalar();
                                                        comForIndividualSecondaryCollateralDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                case "5":
                                                    individualSecondaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfCurrentAsset>();
                                                    if (individualSecondaryCollateral != null)
                                                    {
                                                        #region Inserting into Secondary Collateral table(Collateral Current Asset)
                                                        comForIndividualSecondaryCollateralDetail.CommandType = CommandType.Text;
                                                        comForIndividualSecondaryCollateralDetail.Connection = db;
                                                        comForIndividualSecondaryCollateralDetail.Transaction = tran;
                                                        comForIndividualSecondaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfCurrentAsset]
                                                                                                                        ([CollateralDetailMainId]
                                                                                                                       ,[DateOfLatestStockInspectionReportCollectedByBranch]
                                                                                                                       ,[WorkingCapitalAsPerStockReport]
                                                                                                                       ,[WorkingCapitalAsPerStockInspector]
                                                                                                                       ,[DrawingPower]
                                                                                                                       ,[InsuranceCoverageType]
                                                                                                                       ,[InsuranceExpiryDate]
                                                                                                                       ,[InsuranceAmount]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                 VALUES
                                                                                                                       (@CollateralDetailMainId,
                                                                                                                        @DateOfLatestStockInspectionReportCollectedByBranch,
                                                                                                                        @WorkingCapitalAsPerStockReport,
                                                                                                                        @WorkingCapitalAsPerStockInspector,
                                                                                                                        @DrawingPower,
                                                                                                                        @InsuranceCoverageType,
                                                                                                                        @InsuranceExpiryDate,
                                                                                                                        @InsuranceAmount)";
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromSecondaryCollateralMain);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@DateOfLatestStockInspectionReportCollectedByBranch", itemDetail["DateOfLatestStockInspectionReportCollectedByBranch"].ToString() == "" ? DBNull.Value : ((object)itemDetail["DateOfLatestStockInspectionReportCollectedByBranch"].ToString()));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@WorkingCapitalAsPerStockReport", string.IsNullOrEmpty(itemDetail["WorkingCapitalAsPerStockReport"].ToString()) ? DBNull.Value : (((object)itemDetail["WorkingCapitalAsPerStockReport"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@WorkingCapitalAsPerStockInspector", string.IsNullOrEmpty(itemDetail["WorkingCapitalAsPerStockInspector"].ToString()) ? DBNull.Value : (((object)itemDetail["WorkingCapitalAsPerStockInspector"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@DrawingPower", ((object)itemDetail["DrawingPower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceCoverageType", ((object)itemDetail["InsuranceCoverageType"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceExpiryDate", itemDetail["InsuranceExpiryDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["InsuranceExpiryDate"].ToString()));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceAmount", string.IsNullOrEmpty(itemDetail["InsuranceAmount"].ToString()) ? DBNull.Value : (((object)itemDetail["InsuranceAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        insertedMainIdFromSecondaryCollateralDetail = (int)comForIndividualSecondaryCollateralDetail.ExecuteScalar();
                                                        comForIndividualSecondaryCollateralDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                case "6":
                                                    individualSecondaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfPersonalGuarantee>();
                                                    if (individualSecondaryCollateral != null)
                                                    {
                                                        #region Inserting into Secondary Collateral table(Collateral Personal Guarantee)
                                                        comForIndividualSecondaryCollateralDetail.CommandType = CommandType.Text;
                                                        comForIndividualSecondaryCollateralDetail.Connection = db;
                                                        comForIndividualSecondaryCollateralDetail.Transaction = tran;
                                                        comForIndividualSecondaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfPersonalGuarantee]
                                                                                                                       ([CollateralDetailMainId]
                                                                                                                       ,[NameOfGuarantor]
                                                                                                                       ,[CitizenshipNumber]
                                                                                                                       ,[FatherName]
                                                                                                                       ,[GrandfatherName]
                                                                                                                       ,[PermanentAddress]
                                                                                                                       ,[CurrentAddress]
                                                                                                                       ,[NetworthOfGuarantor]
                                                                                                                       ,[RelationWithBorrower]
                                                                                                                       ,[ContactNumber]
                                                                                                                       ,[Profession]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                 VALUES
                                                                                                                       (@CollateralDetailMainId,
                                                                                                                        @NameOfGuarantor,
                                                                                                                        @CitizenshipNumber,
                                                                                                                        @FatherName,
                                                                                                                        @GrandfatherName,
                                                                                                                        @PermanentAddress,
                                                                                                                        @CurrentAddress,
                                                                                                                        @NetworthOfGuarantor,
                                                                                                                        @RelationWithBorrower,
                                                                                                                        @ContactNumber,
                                                                                                                        @Profession)";
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromSecondaryCollateralMain);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@NameOfGuarantor", ((object)itemDetail["NameOfGuarantor"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CitizenshipNumber", ((object)itemDetail["CitizenshipNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FatherName", ((object)itemDetail["FatherName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@GrandfatherName", ((object)itemDetail["GrandfatherName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@PermanentAddress", ((object)itemDetail["PermanentAddress"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CurrentAddress", ((object)itemDetail["CurrentAddress"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@NetworthOfGuarantor", string.IsNullOrEmpty(itemDetail["NetworthOfGuarantor"].ToString()) ? DBNull.Value : (((object)itemDetail["NetworthOfGuarantor"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@RelationWithBorrower", ((object)itemDetail["RelationWithBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ContactNumber", ((object)itemDetail["ContactNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@Profession", ((object)itemDetail["Profession"].ToString()) ?? DBNull.Value);
                                                        insertedMainIdFromSecondaryCollateralDetail = (int)comForIndividualSecondaryCollateralDetail.ExecuteScalar();
                                                        comForIndividualSecondaryCollateralDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                case "7":
                                                    individualSecondaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfCorporateGuarantee>();
                                                    if (individualSecondaryCollateral != null)
                                                    {
                                                        #region Inserting into Secondary Collateral table(Collateral Corporate Guarantee)
                                                        comForIndividualSecondaryCollateralDetail.CommandType = CommandType.Text;
                                                        comForIndividualSecondaryCollateralDetail.Connection = db;
                                                        comForIndividualSecondaryCollateralDetail.Transaction = tran;
                                                        comForIndividualSecondaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfCorporateGuarantee]
                                                                                                                       ([CollateralDetailMainId]
                                                                                                                       ,[NameOfFirm]
                                                                                                                       ,[RegistrationNumber]
                                                                                                                       ,[FirmAddress]
                                                                                                                       ,[NetworthOfTheFirm]
                                                                                                                       ,[ContactPerson]
                                                                                                                       ,[ContactNumber]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                 VALUES
                                                                                                                       (@CollateralDetailMainId,
                                                                                                                        @NameOfFirm,
                                                                                                                        @RegistrationNumber,
                                                                                                                        @FirmAddress,
                                                                                                                        @NetworthOfTheFirm,
                                                                                                                        @ContactPerson,
                                                                                                                        @ContactNumber)";
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromSecondaryCollateralMain);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@NameOfFirm", ((object)itemDetail["NameOfFirm"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@RegistrationNumber", ((object)itemDetail["RegistrationNumber"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirmAddress", ((object)itemDetail["FirmAddress"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@NetworthOfTheFirm", string.IsNullOrEmpty(itemDetail["NetworthOfTheFirm"].ToString()) ? DBNull.Value : (((object)itemDetail["NetworthOfTheFirm"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ContactPerson", ((object)itemDetail["ContactPerson"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ContactNumber", ((object)itemDetail["ContactNumber"].ToString()) ?? DBNull.Value);
                                                        insertedMainIdFromSecondaryCollateralDetail = (int)comForIndividualSecondaryCollateralDetail.ExecuteScalar();
                                                        comForIndividualSecondaryCollateralDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                case "8":
                                                    individualSecondaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfStock>();
                                                    if (individualSecondaryCollateral != null)
                                                    {
                                                        #region Inserting into Secondary Collateral table(Collateral Stock)
                                                        comForIndividualSecondaryCollateralDetail.CommandType = CommandType.Text;
                                                        comForIndividualSecondaryCollateralDetail.Connection = db;
                                                        comForIndividualSecondaryCollateralDetail.Transaction = tran;
                                                        comForIndividualSecondaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfStock]
                                                                                                                                       ([CollateralDetailMainId]
                                                                                                                                       ,[ShareTypeId]
                                                                                                                                       ,[CompanyName]
                                                                                                                                       ,[ListedInNepseBoolId]
                                                                                                                                       ,[PledgedUnits]
                                                                                                                                       ,[UnitTypeId]
                                                                                                                                       ,[ValueOfShare]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                                 VALUES
                                                                                                                                       (@CollateralDetailMainId,
			                                                                                                                            @ShareTypeId,
			                                                                                                                            @CompanyName,
			                                                                                                                            @ListedInNepseBoolId,
			                                                                                                                            @PledgedUnits,
			                                                                                                                            @UnitTypeId,
			                                                                                                                            @ValueOfShare)";
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromSecondaryCollateralMain);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ShareTypeId", ((object)itemDetail["ShareTypeId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CompanyName", ((object)itemDetail["CompanyName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ListedInNepseBoolId", ((object)itemDetail["ListedInNepseBoolId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@PledgedUnits", string.IsNullOrEmpty(itemDetail["PledgedUnits"].ToString()) ? DBNull.Value : (((object)itemDetail["PledgedUnits"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@UnitTypeId", ((object)itemDetail["ShareUnitTypeId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ValueOfShare", string.IsNullOrEmpty(itemDetail["ValueOfShare"].ToString()) ? DBNull.Value : (((object)itemDetail["ValueOfShare"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        insertedMainIdFromSecondaryCollateralDetail = (int)comForIndividualSecondaryCollateralDetail.ExecuteScalar();
                                                        comForIndividualSecondaryCollateralDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                case "9":
                                                    individualSecondaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfGold>();
                                                    if (individualSecondaryCollateral != null)
                                                    {
                                                        #region Inserting into Secondary Collateral table(Collateral Gold)
                                                        comForIndividualSecondaryCollateralDetail.CommandType = CommandType.Text;
                                                        comForIndividualSecondaryCollateralDetail.Connection = db;
                                                        comForIndividualSecondaryCollateralDetail.Transaction = tran;
                                                        comForIndividualSecondaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfGold]
                                                                                                                                   ([CollateralDetailMainId]
                                                                                                                                   ,[Quantity]
                                                                                                                                   ,[MeasurementUnitId]
                                                                                                                                   ,[GoldTypeId]
                                                                                                                                   ,[ValueOfGold]
                                                                                                                                   ,[NameOfGoldTester]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                             VALUES
                                                                                                                                   (@CollateralDetailMainId,
                                                                                                                                    @Quantity,
                                                                                                                                    @MeasurementUnitId,
                                                                                                                                    @GoldTypeId,
                                                                                                                                    @ValueOfGold,
                                                                                                                                    @NameOfGoldTester)";
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromSecondaryCollateralMain);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@Quantity", ((object)itemDetail["Quantity"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@MeasurementUnitId", ((object)itemDetail["MeasurementUnitId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@GoldTypeId", ((object)itemDetail["GoldTypeId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ValueOfGold", string.IsNullOrEmpty(itemDetail["ValueOfGold"].ToString()) ? DBNull.Value : (((object)itemDetail["ValueOfGold"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@NameOfGoldTester", ((object)itemDetail["NameOfGoldTester"].ToString()) ?? DBNull.Value);
                                                        insertedMainIdFromSecondaryCollateralDetail = (int)comForIndividualSecondaryCollateralDetail.ExecuteScalar();
                                                        comForIndividualSecondaryCollateralDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #endregion

                            }
                            #endregion

                            #region Insert start for Loan Details
                            if (!string.IsNullOrEmpty(npaModel.StringifiedListOfLoanAccountDetailObjects) && npaModel.StringifiedListOfLoanAccountDetailObjects != "[]" && npaModel.StringifiedListOfLoanAccountDetailObjects != "{}")
                            {
                                string strListOfLoanDetails = $"{{\"loanDetails\":{npaModel.StringifiedListOfLoanAccountDetailObjects}}}";
                                JObject joListOfLoanDetails = JObject.Parse(strListOfLoanDetails);
                                JArray jaListOfLoanDetails = (JArray)joListOfLoanDetails["loanDetails"];
                                int insertedLoanDetailMainId = 0;
                                //
                                foreach (var item in jaListOfLoanDetails)
                                {
                                    #region Insert to Loan Detail Main table
                                    comForLoanDetailMain.CommandType = CommandType.Text;
                                    comForLoanDetailMain.Connection = db;
                                    comForLoanDetailMain.Transaction = tran;
                                    comForLoanDetailMain.CommandText = @"INSERT INTO [dbo].[LoanAccountDetail]
                                                                        ([NPAManagementId]
                                                                        ,[LoanAccountNo]
                                                                        ,[ProductCode]
                                                                        ,[LoanTypeId]
                                                                        ,[LoanNatureId]
                                                                        ,[FirstLimitInitiatedBy]
                                                                        ,[FirstRecommendedBy]
                                                                        ,[FirstApprovedBy]
                                                                        ,[DepartmentId]
                                                                        ,[BranchProvince]
                                                                        ,[ReportingSBO]
                                                                        ,[NomineeAccountNo]
                                                                        ,[InitialApprovedLimit]
                                                                        ,[FirstLimitPlacementDate]
                                                                        ,[LatestApprovedLimit]
                                                                        ,[LimitExpiryDate]
                                                                        ,[LoanCurrentStatusId]
                                                                        ,[SettlementDate]
                                                                        ,[LoanStatusId]
                                                                        ,[OutStDate]
                                                                        ,[OutStPrincipal]
                                                                        ,[OutStInterest]
                                                                        ,[OutStAdhocCharges]
                                                                        ,[OutStInterestOnInterest]
                                                                        ,[OutStPenalCharges]
                                                                        ,[OutStTotalAmount]
                                                                        ,[AccruedAmtFromDate]
                                                                        ,[AccruedAmtToDate]
                                                                        ,[AccruedAmtPrincipal]
                                                                        ,[AccruedAmtInterest]
                                                                        ,[AccruedAmtAdhocCharges]
                                                                        ,[AccruedAmtInterestOnInterest]
                                                                        ,[AccruedAmtPenalCharges]
                                                                        ,[AccruedAmtTotalAmount]
                                                                        ,[RecoveredAmtFromDate]
                                                                        ,[RecoveredAmtToDate]
                                                                        ,[RecoveredAmtPrincipal]
                                                                        ,[RecoveredAmtInterest]
                                                                        ,[RecoveredAmtAdhocCharges]
                                                                        ,[RecoveredAmtInterestOnInterest]
                                                                        ,[RecoveredAmtPenalCharges]
                                                                        ,[RecoveredAmtTotalAmount]
                                                                        ,[ProvisionPercentageId]
                                                                        ,[ProvisionDate]
                                                                        ,[ProvisionAmount]) OUTPUT INSERTED.LoanMainId
                                                                    VALUES
                                                                        (@NPAManagementId,
                                                                         @LoanAccountNo,
                                                                         @ProductCode,
                                                                         @LoanTypeId,
                                                                         @LoanNatureId,
                                                                         @FirstLimitInitiatedBy,
                                                                         @FirstRecommendedBy,
                                                                         @FirstApprovedBy,
                                                                         @DepartmentId,
                                                                         @BranchProvince,
                                                                         @ReportingSBO,
                                                                         @NomineeAccountNo,
                                                                         @InitialApprovedLimit,
                                                                         @FirstLimitPlacementDate,
                                                                         @LatestApprovedLimit,
                                                                         @LimitExpiryDate,
                                                                         @LoanCurrentStatusId,
                                                                         @SettlementDate,
                                                                         @LoanStatusId,
                                                                         @OutStDate,
                                                                         @OutStPrincipal,
                                                                         @OutStInterest,
                                                                         @OutStAdhocCharges,
                                                                         @OutStInterestOnInterest,
                                                                         @OutStPenalCharges,
                                                                         @OutStTotalAmount,
                                                                         @AccruedAmtFromDate,
                                                                         @AccruedAmtToDate,
                                                                         @AccruedAmtPrincipal,
                                                                         @AccruedAmtInterest,
                                                                         @AccruedAmtAdhocCharges,
                                                                         @AccruedAmtInterestOnInterest,
                                                                         @AccruedAmtPenalCharges,
                                                                         @AccruedAmtTotalAmount,
                                                                         @RecoveredAmtFromDate,
                                                                         @RecoveredAmtToDate,
                                                                         @RecoveredAmtPrincipal,
                                                                         @RecoveredAmtInterest,
                                                                         @RecoveredAmtAdhocCharges,
                                                                         @RecoveredAmtInterestOnInterest,
                                                                         @RecoveredAmtPenalCharges,
                                                                         @RecoveredAmtTotalAmount,
                                                                         @ProvisionPercentageId,
                                                                         @ProvisionDate,
                                                                         @ProvisionAmount)";
                                    comForLoanDetailMain.Parameters.AddWithValue("@NPAManagementId", insertedNPAMgmtId);
                                    comForLoanDetailMain.Parameters.AddWithValue("@LoanAccountNo", ((object)item["LoanAccountNo"].ToString()) ?? DBNull.Value);
                                    comForLoanDetailMain.Parameters.AddWithValue("@ProductCode", ((object)item["ProductCode"].ToString()) ?? DBNull.Value);
                                    comForLoanDetailMain.Parameters.AddWithValue("@LoanTypeId", !string.IsNullOrEmpty(item["LoanTypeId"].ToString()) ? (object)item["LoanTypeId"].ToString() : DBNull.Value);
                                    comForLoanDetailMain.Parameters.AddWithValue("@LoanNatureId", !string.IsNullOrEmpty(item["LoanNatureId"].ToString()) ? (object)item["LoanNatureId"].ToString() : DBNull.Value);
                                    comForLoanDetailMain.Parameters.AddWithValue("@FirstLimitInitiatedBy", ((object)item["FirstLimitInitiatedBy"].ToString()) ?? DBNull.Value);
                                    comForLoanDetailMain.Parameters.AddWithValue("@FirstRecommendedBy", ((object)item["FirstRecommendedBy"].ToString()) ?? DBNull.Value);
                                    comForLoanDetailMain.Parameters.AddWithValue("@FirstApprovedBy", ((object)item["FirstApprovedBy"].ToString()) ?? DBNull.Value);
                                    comForLoanDetailMain.Parameters.AddWithValue("@DepartmentId", !string.IsNullOrEmpty(item["DepartmentId"].ToString()) ? (object)item["DepartmentId"].ToString() : DBNull.Value);
                                    comForLoanDetailMain.Parameters.AddWithValue("@BranchProvince", ((object)item["BranchProvince"].ToString()) ?? DBNull.Value);
                                    comForLoanDetailMain.Parameters.AddWithValue("@ReportingSBO", ((object)item["ReportingSBO"].ToString()) ?? DBNull.Value);
                                    comForLoanDetailMain.Parameters.AddWithValue("@NomineeAccountNo", ((object)item["NomineeAccountNo"].ToString()) ?? DBNull.Value);
                                    comForLoanDetailMain.Parameters.AddWithValue("@InitialApprovedLimit", string.IsNullOrEmpty(item["InitialApprovedLimit"].ToString()) ? DBNull.Value : (((object)item["InitialApprovedLimit"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    comForLoanDetailMain.Parameters.AddWithValue("@FirstLimitPlacementDate", item["FirstLimitPlacementDate"].ToString() == "" ? DBNull.Value : ((object)item["FirstLimitPlacementDate"].ToString()));
                                    comForLoanDetailMain.Parameters.AddWithValue("@LatestApprovedLimit", string.IsNullOrEmpty(item["LatestApprovedLimit"].ToString()) ? DBNull.Value : (((object)item["LatestApprovedLimit"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    comForLoanDetailMain.Parameters.AddWithValue("@LimitExpiryDate", item["LimitExpiryDate"].ToString() == "" ? DBNull.Value : ((object)item["LimitExpiryDate"].ToString()));
                                    comForLoanDetailMain.Parameters.AddWithValue("@LoanCurrentStatusId", !string.IsNullOrEmpty(item["LoanCurrentStatusId"].ToString()) ? (object)item["LoanCurrentStatusId"].ToString() : DBNull.Value);
                                    comForLoanDetailMain.Parameters.AddWithValue("@SettlementDate", item["SettlementDate"].ToString() == "" ? DBNull.Value : ((object)item["SettlementDate"].ToString()));
                                    comForLoanDetailMain.Parameters.AddWithValue("@LoanStatusId", !string.IsNullOrEmpty(item["LoanStatusId"].ToString()) ? (object)item["LoanStatusId"].ToString() : DBNull.Value);
                                    comForLoanDetailMain.Parameters.AddWithValue("@OutStDate", item["OutStDate"].ToString() == "" ? DBNull.Value : ((object)item["OutStDate"].ToString()));
                                    comForLoanDetailMain.Parameters.AddWithValue("@OutStPrincipal", string.IsNullOrEmpty(item["OutStPrincipal"].ToString()) ? DBNull.Value : (((object)item["OutStPrincipal"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    comForLoanDetailMain.Parameters.AddWithValue("@OutStInterest", string.IsNullOrEmpty(item["OutStInterest"].ToString()) ? DBNull.Value : (((object)item["OutStInterest"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    comForLoanDetailMain.Parameters.AddWithValue("@OutStAdhocCharges", string.IsNullOrEmpty(item["OutStAdhocCharges"].ToString()) ? DBNull.Value : (((object)item["OutStAdhocCharges"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    comForLoanDetailMain.Parameters.AddWithValue("@OutStInterestOnInterest", string.IsNullOrEmpty(item["OutStInterestOnInterest"].ToString()) ? DBNull.Value : (((object)item["OutStInterestOnInterest"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    comForLoanDetailMain.Parameters.AddWithValue("@OutStPenalCharges", string.IsNullOrEmpty(item["OutStPenalCharges"].ToString()) ? DBNull.Value : (((object)item["OutStPenalCharges"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    comForLoanDetailMain.Parameters.AddWithValue("@OutStTotalAmount", string.IsNullOrEmpty(item["OutStTotalAmount"].ToString()) ? DBNull.Value : (((object)item["OutStTotalAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    comForLoanDetailMain.Parameters.AddWithValue("@AccruedAmtFromDate", item["AccruedAmtFromDate"].ToString() == "" ? DBNull.Value : ((object)item["AccruedAmtFromDate"].ToString()));
                                    comForLoanDetailMain.Parameters.AddWithValue("@AccruedAmtToDate", item["AccruedAmtToDate"].ToString() == "" ? DBNull.Value : ((object)item["AccruedAmtToDate"].ToString()));
                                    comForLoanDetailMain.Parameters.AddWithValue("@AccruedAmtPrincipal", string.IsNullOrEmpty(item["AccruedAmtPrincipal"].ToString()) ? DBNull.Value : (((object)item["AccruedAmtPrincipal"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    comForLoanDetailMain.Parameters.AddWithValue("@AccruedAmtInterest", string.IsNullOrEmpty(item["AccruedAmtInterest"].ToString()) ? DBNull.Value : (((object)item["AccruedAmtInterest"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    comForLoanDetailMain.Parameters.AddWithValue("@AccruedAmtAdhocCharges", string.IsNullOrEmpty(item["AccruedAmtAdhocCharges"].ToString()) ? DBNull.Value : (((object)item["AccruedAmtAdhocCharges"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    comForLoanDetailMain.Parameters.AddWithValue("@AccruedAmtInterestOnInterest", string.IsNullOrEmpty(item["AccruedAmtInterestOnInterest"].ToString()) ? DBNull.Value : (((object)item["AccruedAmtInterestOnInterest"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    comForLoanDetailMain.Parameters.AddWithValue("@AccruedAmtPenalCharges", string.IsNullOrEmpty(item["AccruedAmtPenalCharges"].ToString()) ? DBNull.Value : (((object)item["AccruedAmtPenalCharges"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    comForLoanDetailMain.Parameters.AddWithValue("@AccruedAmtTotalAmount", string.IsNullOrEmpty(item["AccruedAmtTotalAmount"].ToString()) ? DBNull.Value : (((object)item["AccruedAmtTotalAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    comForLoanDetailMain.Parameters.AddWithValue("@RecoveredAmtFromDate", item["RecoveredAmtFromDate"].ToString() == "" ? DBNull.Value : ((object)item["RecoveredAmtFromDate"].ToString()));
                                    comForLoanDetailMain.Parameters.AddWithValue("@RecoveredAmtToDate", item["RecoveredAmtToDate"].ToString() == "" ? DBNull.Value : ((object)item["RecoveredAmtToDate"].ToString()));
                                    comForLoanDetailMain.Parameters.AddWithValue("@RecoveredAmtPrincipal", string.IsNullOrEmpty(item["RecoveredAmtPrincipal"].ToString()) ? DBNull.Value : (((object)item["RecoveredAmtPrincipal"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    comForLoanDetailMain.Parameters.AddWithValue("@RecoveredAmtInterest", string.IsNullOrEmpty(item["RecoveredAmtInterest"].ToString()) ? DBNull.Value : (((object)item["RecoveredAmtInterest"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    comForLoanDetailMain.Parameters.AddWithValue("@RecoveredAmtAdhocCharges", string.IsNullOrEmpty(item["RecoveredAmtAdhocCharges"].ToString()) ? DBNull.Value : (((object)item["RecoveredAmtAdhocCharges"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    comForLoanDetailMain.Parameters.AddWithValue("@RecoveredAmtInterestOnInterest", string.IsNullOrEmpty(item["RecoveredAmtInterestOnInterest"].ToString()) ? DBNull.Value : (((object)item["RecoveredAmtInterestOnInterest"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    comForLoanDetailMain.Parameters.AddWithValue("@RecoveredAmtPenalCharges", string.IsNullOrEmpty(item["RecoveredAmtPenalCharges"].ToString()) ? DBNull.Value : (((object)item["RecoveredAmtPenalCharges"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    comForLoanDetailMain.Parameters.AddWithValue("@RecoveredAmtTotalAmount", string.IsNullOrEmpty(item["RecoveredAmtTotalAmount"].ToString()) ? DBNull.Value : (((object)item["RecoveredAmtTotalAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    comForLoanDetailMain.Parameters.AddWithValue("@ProvisionPercentageId", !string.IsNullOrEmpty(item["ProvisionPercentageId"].ToString()) ? (object)item["ProvisionPercentageId"].ToString() : DBNull.Value);
                                    comForLoanDetailMain.Parameters.AddWithValue("@ProvisionDate", item["ProvisionDate"].ToString() == "" ? DBNull.Value : ((object)item["ProvisionDate"].ToString()));
                                    comForLoanDetailMain.Parameters.AddWithValue("@ProvisionAmount", string.IsNullOrEmpty(item["ProvisionAmount"].ToString()) ? DBNull.Value : (((object)item["ProvisionAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                    insertedLoanDetailMainId = (int)comForLoanDetailMain.ExecuteScalar();
                                    comForLoanDetailMain.Parameters.Clear();
                                    #endregion
                                }
                            }
                            #endregion

                            #region Insert start for FollowUp Details(Branch and SAC)

                            #region Branch
                            if (!string.IsNullOrEmpty(npaModel.StringifiedListOfBranchFollowUpDetailObjects) && npaModel.StringifiedListOfBranchFollowUpDetailObjects != "[]" && npaModel.StringifiedListOfBranchFollowUpDetailObjects != "{}")
                            {

                                #region Insert cases for Branch FollowUps

                                #region Parsing the StringifiedListOfBranchFollowUpDetailObjects and operating on it

                                string strListOfBranchFollowUps = $"{{\"branchFollowUps\":{npaModel.StringifiedListOfBranchFollowUpDetailObjects}}}";
                                JObject joListOfBranchFollowUps = JObject.Parse(strListOfBranchFollowUps);
                                JArray jaListOfBranchFollowUps = (JArray)joListOfBranchFollowUps["branchFollowUps"];
                                int insertedFollowUpMainIdFromBranchFollowUpMain = 0;
                                int insertedFollowUpMainIdFromBranchFollowUpDetail = 0;
                                foreach (var item in jaListOfBranchFollowUps)
                                {
                                    if (item["FollowUpTypeId"].ToString() != "")
                                    {
                                        insertedFollowUpMainIdFromBranchFollowUpDetail = 0;
                                        #region Insert to FollowUp Main table Branch FollowUps
                                        comForBranchFollowUpMain.CommandType = CommandType.Text;
                                        comForBranchFollowUpMain.Connection = db;
                                        comForBranchFollowUpMain.Transaction = tran;
                                        comForBranchFollowUpMain.CommandText = @"INSERT INTO [dbo].[FollowUpMain]
                                                                            ([NPAManagementId]
                                                                            ,[FollowUpTypeId]
                                                                            ,[FollowUpById]
                                                                            ,[EnteredBy]
                                                                            ,[EnteredDate]
                                                                            ,[ModifiedBy]
                                                                            ,[ModifiedDate]) OUTPUT INSERTED.FollowUpMainId
                                                                        VALUES
                                                                            (@NPAManagementId,
                                                                             @FollowUpTypeId,
                                                                             @FollowUpById,
                                                                             @EnteredBy,
                                                                             @EnteredDate,
                                                                             @ModifiedBy,
                                                                             @ModifiedDate)";
                                        comForBranchFollowUpMain.Parameters.AddWithValue("@NPAManagementId", insertedNPAMgmtId);
                                        comForBranchFollowUpMain.Parameters.AddWithValue("@FollowUpTypeId", ((object)item["FollowUpTypeId"].ToString()) ?? DBNull.Value);
                                        comForBranchFollowUpMain.Parameters.AddWithValue("@FollowUpById", "1");
                                        comForBranchFollowUpMain.Parameters.AddWithValue("@EnteredBy", ((object)npaModel.EnteredBy) ?? DBNull.Value);
                                        comForBranchFollowUpMain.Parameters.AddWithValue("@EnteredDate", ((object)npaModel.EnteredOn) ?? DBNull.Value);
                                        comForBranchFollowUpMain.Parameters.AddWithValue("@ModifiedBy", ((object)npaModel.EnteredBy) ?? DBNull.Value);
                                        comForBranchFollowUpMain.Parameters.AddWithValue("@ModifiedDate", ((object)npaModel.EnteredOn) ?? DBNull.Value);
                                        insertedFollowUpMainIdFromBranchFollowUpMain = (int)comForBranchFollowUpMain.ExecuteScalar();
                                        comForBranchFollowUpMain.Parameters.Clear();
                                        #endregion
                                        if (insertedFollowUpMainIdFromBranchFollowUpMain > 0)
                                        {
                                            dynamic individualBranchFollowUp = null;
                                            switch (item["FollowUpTypeId"].ToString())
                                            {
                                                case "1":
                                                    individualBranchFollowUp = item.ToObject<FollowUpDetailByTelephone>();
                                                    if (individualBranchFollowUp != null)
                                                    {
                                                        #region Inserting into Branch FollowUp table(FollowUp by Telephone)
                                                        comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                        comForIndividualBranchFollowUpDetail.Connection = db;
                                                        comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                        comForIndividualBranchFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailByTelephone]
                                                                                       ([FollowUpMainId]
                                                                                       ,[ContactDate]
                                                                                       ,[ResponseOfBorrower]
                                                                                       ,[NextActionToBeTaken]
                                                                                       ,[DueDateOfNextAction]) OUTPUT INSERTED.FollowUpMainId
                                                                                 VALUES
                                                                                       (@FollowUpMainId,
                                                                                        @ContactDate,
                                                                                        @ResponseOfBorrower,
                                                                                        @NextActionToBeTaken,
                                                                                        @DueDateOfNextAction)";
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromBranchFollowUpMain);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ContactDate", item["ContactDate"].ToString() == "" ? DBNull.Value : ((object)item["ContactDate"].ToString()));
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                        insertedFollowUpMainIdFromBranchFollowUpDetail = (int)comForIndividualBranchFollowUpDetail.ExecuteScalar();
                                                        comForIndividualBranchFollowUpDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                case "2":
                                                    individualBranchFollowUp = item.ToObject<FollowUpDetailByInPersonVisit>();
                                                    if (individualBranchFollowUp != null)
                                                    {
                                                        #region Inserting into Branch FollowUp table(FollowUp by In-Person Visit)
                                                        comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                        comForIndividualBranchFollowUpDetail.Connection = db;
                                                        comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                        comForIndividualBranchFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailByInPersonVisit]
                                                                                                        ([FollowUpMainId]
                                                                                                        ,[VisitDate]
                                                                                                        ,[ResponseOfBorrower]
                                                                                                        ,[NextActionToBeTaken]
                                                                                                        ,[DueDateOfNextAction]) OUTPUT INSERTED.FollowUpMainId
                                                                                                    VALUES
                                                                                                        (@FollowUpMainId,
                                                                                                         @VisitDate,
                                                                                                         @ResponseOfBorrower,
                                                                                                         @NextActionToBeTaken,
                                                                                                         @DueDateOfNextAction)";
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromBranchFollowUpMain);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@VisitDate", item["VisitDate"].ToString() == "" ? DBNull.Value : ((object)item["VisitDate"].ToString()));
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                        insertedFollowUpMainIdFromBranchFollowUpDetail = (int)comForIndividualBranchFollowUpDetail.ExecuteScalar();
                                                        comForIndividualBranchFollowUpDetail.Parameters.Clear();

                                                        #region Come Save Uploaded Files Here
                                                        if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                        {
                                                            //File saving part here
                                                            if (!string.IsNullOrEmpty(individualBranchFollowUp.ScanCopyOfActionTakenFile) && individualBranchFollowUp.ScanCopyOfActionTakenFile != "[]" && individualBranchFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                            {
                                                                //Convert Base64 Encoded string to Byte Array.
                                                                byte[] fileBytes = Convert.FromBase64String(individualBranchFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}")))
                                                                {
                                                                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}"));
                                                                }
                                                                File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}/File.{individualBranchFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                            }
                                                        }
                                                        #endregion

                                                        #endregion
                                                    }
                                                    break;
                                                case "3":
                                                    individualBranchFollowUp = item.ToObject<FollowUpDetailFor15DaysLetter>();
                                                    if (individualBranchFollowUp != null)
                                                    {
                                                        #region Inserting into Branch FollowUp table(FollowUp by 15 Days Letter)
                                                        comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                        comForIndividualBranchFollowUpDetail.Connection = db;
                                                        comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                        comForIndividualBranchFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailFor15DaysLetter]
                                                                                                       ([FollowUpMainId]
                                                                                                       ,[DateOfIssuanceOfFollowUpLetter]
                                                                                                       ,[DateOfReceiptOfLetter]
                                                                                                       ,[ResponseOfBorrower]
                                                                                                       ,[NextActionToBeTaken]
                                                                                                       ,[DueDateOfNextAction]) OUTPUT INSERTED.FollowUpMainId
                                                                                                 VALUES
                                                                                                       (@FollowUpMainId,
                                                                                                        @DateOfIssuanceOfFollowUpLetter,
                                                                                                        @DateOfReceiptOfLetter,
                                                                                                        @ResponseOfBorrower,
                                                                                                        @NextActionToBeTaken,
                                                                                                        @DueDateOfNextAction)";
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromBranchFollowUpMain);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfIssuanceOfFollowUpLetter", item["DateOfIssuanceOfFollowUpLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfIssuanceOfFollowUpLetter"].ToString()));
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfReceiptOfLetter", item["DateOfReceiptOfLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfReceiptOfLetter"].ToString()));
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                        insertedFollowUpMainIdFromBranchFollowUpDetail = (int)comForIndividualBranchFollowUpDetail.ExecuteScalar();
                                                        comForIndividualBranchFollowUpDetail.Parameters.Clear();

                                                        #region Come Save Uploaded Files Here
                                                        if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                        {
                                                            //File saving part here
                                                            if (!string.IsNullOrEmpty(individualBranchFollowUp.ScanCopyOfActionTakenFile) && individualBranchFollowUp.ScanCopyOfActionTakenFile != "[]" && individualBranchFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                            {
                                                                //Convert Base64 Encoded string to Byte Array.
                                                                byte[] fileBytes = Convert.FromBase64String(individualBranchFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}")))
                                                                {
                                                                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}"));
                                                                }
                                                                File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}/File.{individualBranchFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                            }
                                                        }
                                                        #endregion

                                                        #endregion
                                                    }
                                                    break;
                                                case "4":
                                                    individualBranchFollowUp = item.ToObject<FollowUpDetailForFirst7DaysLetter>();
                                                    if (individualBranchFollowUp != null)
                                                    {
                                                        #region Inserting into Branch FollowUp table(FollowUp by First 7 Days Letter)
                                                        comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                        comForIndividualBranchFollowUpDetail.Connection = db;
                                                        comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                        comForIndividualBranchFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailForFirst7DaysLetter]
                                                                                                        ([FollowUpMainId]
                                                                                                        ,[DateOfIssuanceOfFollowUpLetter]
                                                                                                        ,[DateOfReceiptOfLetter]
                                                                                                        ,[ResponseOfBorrower]
                                                                                                        ,[NextActionToBeTaken]
                                                                                                        ,[DueDateOfNextAction]) OUTPUT INSERTED.FollowUpMainId
                                                                                                    VALUES
                                                                                                        (@FollowUpMainId,
                                                                                                         @DateOfIssuanceOfFollowUpLetter,
                                                                                                         @DateOfReceiptOfLetter,
                                                                                                         @ResponseOfBorrower,
                                                                                                         @NextActionToBeTaken,
                                                                                                         @DueDateOfNextAction)";
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromBranchFollowUpMain);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfIssuanceOfFollowUpLetter", item["DateOfIssuanceOfFollowUpLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfIssuanceOfFollowUpLetter"].ToString()));
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfReceiptOfLetter", item["DateOfReceiptOfLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfReceiptOfLetter"].ToString()));
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                        insertedFollowUpMainIdFromBranchFollowUpDetail = (int)comForIndividualBranchFollowUpDetail.ExecuteScalar();
                                                        comForIndividualBranchFollowUpDetail.Parameters.Clear();

                                                        #region Come Save Uploaded Files Here
                                                        if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                        {
                                                            //File saving part here
                                                            if (!string.IsNullOrEmpty(individualBranchFollowUp.ScanCopyOfActionTakenFile) && individualBranchFollowUp.ScanCopyOfActionTakenFile != "[]" && individualBranchFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                            {
                                                                //Convert Base64 Encoded string to Byte Array.
                                                                byte[] fileBytes = Convert.FromBase64String(individualBranchFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}")))
                                                                {
                                                                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}"));
                                                                }
                                                                File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}/File.{individualBranchFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                            }
                                                        }
                                                        #endregion

                                                        #endregion
                                                    }
                                                    break;
                                                case "5":
                                                    individualBranchFollowUp = item.ToObject<FollowUpDetailForSecond7DaysLetter>();
                                                    if (individualBranchFollowUp != null)
                                                    {
                                                        #region Inserting into Branch FollowUp table(FollowUp by Second 7 Days Letter)
                                                        comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                        comForIndividualBranchFollowUpDetail.Connection = db;
                                                        comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                        comForIndividualBranchFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailForSecond7DaysLetter]
                                                                                                       ([FollowUpMainId]
                                                                                                       ,[DateOfIssuanceOfFollowUpLetter]
                                                                                                       ,[DateOfReceiptOfLetter]
                                                                                                       ,[ResponseOfBorrower]
                                                                                                       ,[NextActionToBeTaken]
                                                                                                       ,[DueDateOfNextAction]) OUTPUT INSERTED.FollowUpMainId
                                                                                                 VALUES
                                                                                                       (@FollowUpMainId,
                                                                                                        @DateOfIssuanceOfFollowUpLetter,
                                                                                                        @DateOfReceiptOfLetter,
                                                                                                        @ResponseOfBorrower,
                                                                                                        @NextActionToBeTaken,
                                                                                                        @DueDateOfNextAction)";
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromBranchFollowUpMain);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfIssuanceOfFollowUpLetter", item["DateOfIssuanceOfFollowUpLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfIssuanceOfFollowUpLetter"].ToString()));
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfReceiptOfLetter", item["DateOfReceiptOfLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfReceiptOfLetter"].ToString()));
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                        insertedFollowUpMainIdFromBranchFollowUpDetail = (int)comForIndividualBranchFollowUpDetail.ExecuteScalar();
                                                        comForIndividualBranchFollowUpDetail.Parameters.Clear();

                                                        #region Come Save Uploaded Files Here
                                                        if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                        {
                                                            //File saving part here
                                                            if (!string.IsNullOrEmpty(individualBranchFollowUp.ScanCopyOfActionTakenFile) && individualBranchFollowUp.ScanCopyOfActionTakenFile != "[]" && individualBranchFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                            {
                                                                //Convert Base64 Encoded string to Byte Array.
                                                                byte[] fileBytes = Convert.FromBase64String(individualBranchFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}")))
                                                                {
                                                                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}"));
                                                                }
                                                                File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}/File.{individualBranchFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                            }
                                                        }
                                                        #endregion

                                                        #endregion
                                                    }
                                                    break;
                                                case "6":
                                                    individualBranchFollowUp = item.ToObject<FollowUpDetailFor35DaysCallNotice>();
                                                    if (individualBranchFollowUp != null)
                                                    {
                                                        #region Inserting into Branch FollowUp table(FollowUp by 35 Days Call Notice)
                                                        comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                        comForIndividualBranchFollowUpDetail.Connection = db;
                                                        comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                        comForIndividualBranchFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailFor35DaysCallNotice]
                                                                                                       ([FollowUpMainId]
                                                                                                       ,[DateOfNotice]
                                                                                                       ,[NewspaperName]
                                                                                                       ,[ResponseOfBorrower]
                                                                                                       ,[NextActionToBeTaken]
                                                                                                       ,[DueDateOfNextAction]
                                                                                                       ,[FollowUpResultId]
                                                                                                       ,[Remarks]) OUTPUT INSERTED.FollowUpMainId
                                                                                                 VALUES
                                                                                                       (@FollowUpMainId,
                                                                                                        @DateOfNotice,
                                                                                                        @NewspaperName,
                                                                                                        @ResponseOfBorrower,
                                                                                                        @NextActionToBeTaken,
                                                                                                        @DueDateOfNextAction,
                                                                                                        @FollowUpResultId,
                                                                                                        @Remarks)";
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromBranchFollowUpMain);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfNotice", item["DateOfNotice"].ToString() == "" ? DBNull.Value : ((object)item["DateOfNotice"].ToString()));
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NewspaperName", ((object)item["NewspaperName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpResultId", ((object)item["FollowUpResultId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@Remarks", ((object)item["Remarks"].ToString()) ?? DBNull.Value);
                                                        insertedFollowUpMainIdFromBranchFollowUpDetail = (int)comForIndividualBranchFollowUpDetail.ExecuteScalar();
                                                        comForIndividualBranchFollowUpDetail.Parameters.Clear();

                                                        #region Come Save Uploaded Files Here
                                                        if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                        {
                                                            //File saving part here
                                                            if (!string.IsNullOrEmpty(individualBranchFollowUp.ScanCopyOfActionTakenFile) && individualBranchFollowUp.ScanCopyOfActionTakenFile != "[]" && individualBranchFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                            {
                                                                //Convert Base64 Encoded string to Byte Array.
                                                                byte[] fileBytes = Convert.FromBase64String(individualBranchFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}")))
                                                                {
                                                                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}"));
                                                                }
                                                                File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}/File.{individualBranchFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                            }
                                                        }
                                                        #endregion

                                                        #endregion
                                                    }
                                                    break;
                                                case "7":
                                                    individualBranchFollowUp = item.ToObject<FollowUpDetailForAuctionNotice>();
                                                    if (individualBranchFollowUp != null)
                                                    {
                                                        #region Inserting into Branch FollowUp table(FollowUp by Auction Notice)
                                                        comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                        comForIndividualBranchFollowUpDetail.Connection = db;
                                                        comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                        comForIndividualBranchFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailForAuctionNotice]
                                                                                                       ([FollowUpMainId]
                                                                                                       ,[AuctionNoticeTypeId]
                                                                                                       ,[DateOfAuctionNotice]
                                                                                                       ,[AuctionNewspaperName]
                                                                                                       ,[AuctionResponseOfBorrower]
                                                                                                       ,[AuctionNextActionToBeTaken]
                                                                                                       ,[AuctionDueDateOfNextAction]
                                                                                                       ,[AuctionRemarks]
                                                                                                       ,[DateOfRevaluationForAuction]
                                                                                                       ,[RevaluatedFMVOfCollateralForAuction]) OUTPUT INSERTED.FollowUpMainId
                                                                                                 VALUES
                                                                                                       (@FollowUpMainId,
                                                                                                        @AuctionNoticeTypeId,
                                                                                                        @DateOfAuctionNotice,
                                                                                                        @AuctionNewspaperName,
                                                                                                        @AuctionResponseOfBorrower,
                                                                                                        @AuctionNextActionToBeTaken,
                                                                                                        @AuctionDueDateOfNextAction,
                                                                                                        @AuctionRemarks,
                                                                                                        @DateOfRevaluationForAuction,
                                                                                                        @RevaluatedFMVOfCollateralForAuction)";
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromBranchFollowUpMain);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@AuctionNoticeTypeId", ((object)item["AuctionNoticeTypeId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfAuctionNotice", item["DateOfAuctionNotice"].ToString() == "" ? DBNull.Value : ((object)item["DateOfAuctionNotice"].ToString()));
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@AuctionNewspaperName", ((object)item["AuctionNewspaperName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@AuctionResponseOfBorrower", ((object)item["AuctionResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@AuctionNextActionToBeTaken", ((object)item["AuctionNextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@AuctionDueDateOfNextAction", item["AuctionDueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["AuctionDueDateOfNextAction"].ToString()));
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@AuctionRemarks", ((object)item["AuctionRemarks"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfRevaluationForAuction", item["DateOfRevaluationForAuction"].ToString() == "" ? DBNull.Value : ((object)item["DateOfRevaluationForAuction"].ToString()));
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@RevaluatedFMVOfCollateralForAuction", string.IsNullOrEmpty(item["RevaluatedFMVOfCollateralForAuction"].ToString()) ? DBNull.Value : (((object)item["RevaluatedFMVOfCollateralForAuction"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        insertedFollowUpMainIdFromBranchFollowUpDetail = (int)comForIndividualBranchFollowUpDetail.ExecuteScalar();
                                                        comForIndividualBranchFollowUpDetail.Parameters.Clear();

                                                        #region Come Save Uploaded Files Here
                                                        if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                        {
                                                            //File saving part here
                                                            if (!string.IsNullOrEmpty(individualBranchFollowUp.ScanCopyOfActionTakenFile) && individualBranchFollowUp.ScanCopyOfActionTakenFile != "[]" && individualBranchFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                            {
                                                                //Convert Base64 Encoded string to Byte Array.
                                                                byte[] fileBytes = Convert.FromBase64String(individualBranchFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}")))
                                                                {
                                                                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}"));
                                                                }
                                                                File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}/File.{individualBranchFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                            }
                                                        }
                                                        #endregion

                                                        #endregion
                                                    }
                                                    break;
                                                case "8":
                                                    individualBranchFollowUp = item.ToObject<FollowUpDetailForOthers>();
                                                    if (individualBranchFollowUp != null)
                                                    {
                                                        #region Inserting into Branch FollowUp table(FollowUp for Others)
                                                        comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                        comForIndividualBranchFollowUpDetail.Connection = db;
                                                        comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                        comForIndividualBranchFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailForOthers]
                                                                                       ([FollowUpMainId]
                                                                                       ,[ContactDate]
                                                                                       ,[ResponseOfBorrower]
                                                                                       ,[NextActionToBeTaken]
                                                                                       ,[DueDateOfNextAction]
                                                                                       ,[Remarks]) OUTPUT INSERTED.FollowUpMainId
                                                                                 VALUES
                                                                                       (@FollowUpMainId,
                                                                                        @ContactDate,
                                                                                        @ResponseOfBorrower,
                                                                                        @NextActionToBeTaken,
                                                                                        @DueDateOfNextAction,
                                                                                        @Remarks)";
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromBranchFollowUpMain);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ContactDate", item["ContactDate"].ToString() == "" ? DBNull.Value : ((object)item["ContactDate"].ToString()));
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                        comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@Remarks", ((object)item["Remarks"].ToString()) ?? DBNull.Value);
                                                        insertedFollowUpMainIdFromBranchFollowUpDetail = (int)comForIndividualBranchFollowUpDetail.ExecuteScalar();
                                                        comForIndividualBranchFollowUpDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #endregion

                            }
                            #endregion

                            #region SAC
                            if (!string.IsNullOrEmpty(npaModel.StringifiedListOfSACFollowUpDetailObjects) && npaModel.StringifiedListOfSACFollowUpDetailObjects != "[]" && npaModel.StringifiedListOfSACFollowUpDetailObjects != "{}")
                            {

                                #region Insert cases for SAC FollowUps

                                #region Parsing the StringifiedListOfSACFollowUpDetailObjects and operating on it

                                string strListOfSACFollowUps = $"{{\"sacFollowUps\":{npaModel.StringifiedListOfSACFollowUpDetailObjects}}}";
                                JObject joListOfSACFollowUps = JObject.Parse(strListOfSACFollowUps);
                                JArray jaListOfSACFollowUps = (JArray)joListOfSACFollowUps["sacFollowUps"];
                                int insertedFollowUpMainIdFromSACFollowUpMain = 0;
                                int insertedFollowUpMainIdFromSACFollowUpDetail = 0;
                                foreach (var item in jaListOfSACFollowUps)
                                {
                                    if (item["FollowUpTypeId"].ToString() != "")
                                    {
                                        insertedFollowUpMainIdFromSACFollowUpDetail = 0;
                                        #region Insert to FollowUp Main table SAC FollowUps
                                        comForSACFollowUpMain.CommandType = CommandType.Text;
                                        comForSACFollowUpMain.Connection = db;
                                        comForSACFollowUpMain.Transaction = tran;
                                        comForSACFollowUpMain.CommandText = @"INSERT INTO [dbo].[FollowUpMain]
                                                                            ([NPAManagementId]
                                                                            ,[FollowUpTypeId]
                                                                            ,[FollowUpById]
                                                                            ,[EnteredBy]
                                                                            ,[EnteredDate]
                                                                            ,[ModifiedBy]
                                                                            ,[ModifiedDate]) OUTPUT INSERTED.FollowUpMainId
                                                                        VALUES
                                                                            (@NPAManagementId,
                                                                             @FollowUpTypeId,
                                                                             @FollowUpById,
                                                                             @EnteredBy,
                                                                             @EnteredDate,
                                                                             @ModifiedBy,
                                                                             @ModifiedDate)";
                                        comForSACFollowUpMain.Parameters.AddWithValue("@NPAManagementId", insertedNPAMgmtId);
                                        comForSACFollowUpMain.Parameters.AddWithValue("@FollowUpTypeId", ((object)item["FollowUpTypeId"].ToString()) ?? DBNull.Value);
                                        comForSACFollowUpMain.Parameters.AddWithValue("@FollowUpById", "2");
                                        comForSACFollowUpMain.Parameters.AddWithValue("@EnteredBy", ((object)npaModel.EnteredBy) ?? DBNull.Value);
                                        comForSACFollowUpMain.Parameters.AddWithValue("@EnteredDate", ((object)npaModel.EnteredOn) ?? DBNull.Value);
                                        comForSACFollowUpMain.Parameters.AddWithValue("@ModifiedBy", ((object)npaModel.EnteredBy) ?? DBNull.Value);
                                        comForSACFollowUpMain.Parameters.AddWithValue("@ModifiedDate", ((object)npaModel.EnteredOn) ?? DBNull.Value);
                                        insertedFollowUpMainIdFromSACFollowUpMain = (int)comForSACFollowUpMain.ExecuteScalar();
                                        comForSACFollowUpMain.Parameters.Clear();
                                        #endregion
                                        if (insertedFollowUpMainIdFromSACFollowUpMain > 0)
                                        {
                                            dynamic individualSACFollowUp = null;
                                            switch (item["FollowUpTypeId"].ToString())
                                            {
                                                case "1":
                                                    individualSACFollowUp = item.ToObject<FollowUpDetailByTelephone>();
                                                    if (individualSACFollowUp != null)
                                                    {
                                                        #region Inserting into SAC FollowUp table(FollowUp by Telephone)
                                                        comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                        comForIndividualSACFollowUpDetail.Connection = db;
                                                        comForIndividualSACFollowUpDetail.Transaction = tran;
                                                        comForIndividualSACFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailByTelephone]
                                                                                       ([FollowUpMainId]
                                                                                       ,[ContactDate]
                                                                                       ,[ResponseOfBorrower]
                                                                                       ,[NextActionToBeTaken]
                                                                                       ,[DueDateOfNextAction]) OUTPUT INSERTED.FollowUpMainId
                                                                                 VALUES
                                                                                       (@FollowUpMainId,
                                                                                        @ContactDate,
                                                                                        @ResponseOfBorrower,
                                                                                        @NextActionToBeTaken,
                                                                                        @DueDateOfNextAction)";
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromSACFollowUpMain);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ContactDate", item["ContactDate"].ToString() == "" ? DBNull.Value : ((object)item["ContactDate"].ToString()));
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                        insertedFollowUpMainIdFromSACFollowUpDetail = (int)comForIndividualSACFollowUpDetail.ExecuteScalar();
                                                        comForIndividualSACFollowUpDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                case "2":
                                                    individualSACFollowUp = item.ToObject<FollowUpDetailByInPersonVisit>();
                                                    if (individualSACFollowUp != null)
                                                    {
                                                        #region Inserting into SAC FollowUp table(FollowUp by In-Person Visit)
                                                        comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                        comForIndividualSACFollowUpDetail.Connection = db;
                                                        comForIndividualSACFollowUpDetail.Transaction = tran;
                                                        comForIndividualSACFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailByInPersonVisit]
                                                                                                        ([FollowUpMainId]
                                                                                                        ,[VisitDate]
                                                                                                        ,[ResponseOfBorrower]
                                                                                                        ,[NextActionToBeTaken]
                                                                                                        ,[DueDateOfNextAction]) OUTPUT INSERTED.FollowUpMainId
                                                                                                    VALUES
                                                                                                        (@FollowUpMainId,
                                                                                                         @VisitDate,
                                                                                                         @ResponseOfBorrower,
                                                                                                         @NextActionToBeTaken,
                                                                                                         @DueDateOfNextAction)";
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromSACFollowUpMain);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@VisitDate", item["VisitDate"].ToString() == "" ? DBNull.Value : ((object)item["VisitDate"].ToString()));
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                        insertedFollowUpMainIdFromSACFollowUpDetail = (int)comForIndividualSACFollowUpDetail.ExecuteScalar();
                                                        comForIndividualSACFollowUpDetail.Parameters.Clear();

                                                        #region Come Save Uploaded Files Here
                                                        if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                        {
                                                            //File saving part here
                                                            if (!string.IsNullOrEmpty(individualSACFollowUp.ScanCopyOfActionTakenFile) && individualSACFollowUp.ScanCopyOfActionTakenFile != "[]" && individualSACFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                            {
                                                                //Convert Base64 Encoded string to Byte Array.
                                                                byte[] fileBytes = Convert.FromBase64String(individualSACFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}")))
                                                                {
                                                                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}"));
                                                                }
                                                                File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}/File.{individualSACFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                            }
                                                        }
                                                        #endregion

                                                        #endregion
                                                    }
                                                    break;
                                                case "3":
                                                    individualSACFollowUp = item.ToObject<FollowUpDetailFor15DaysLetter>();
                                                    if (individualSACFollowUp != null)
                                                    {
                                                        #region Inserting into SAC FollowUp table(FollowUp by 15 Days Letter)
                                                        comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                        comForIndividualSACFollowUpDetail.Connection = db;
                                                        comForIndividualSACFollowUpDetail.Transaction = tran;
                                                        comForIndividualSACFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailFor15DaysLetter]
                                                                                                       ([FollowUpMainId]
                                                                                                       ,[DateOfIssuanceOfFollowUpLetter]
                                                                                                       ,[DateOfReceiptOfLetter]
                                                                                                       ,[ResponseOfBorrower]
                                                                                                       ,[NextActionToBeTaken]
                                                                                                       ,[DueDateOfNextAction]) OUTPUT INSERTED.FollowUpMainId
                                                                                                 VALUES
                                                                                                       (@FollowUpMainId,
                                                                                                        @DateOfIssuanceOfFollowUpLetter,
                                                                                                        @DateOfReceiptOfLetter,
                                                                                                        @ResponseOfBorrower,
                                                                                                        @NextActionToBeTaken,
                                                                                                        @DueDateOfNextAction)";
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromSACFollowUpMain);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfIssuanceOfFollowUpLetter", item["DateOfIssuanceOfFollowUpLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfIssuanceOfFollowUpLetter"].ToString()));
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfReceiptOfLetter", item["DateOfReceiptOfLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfReceiptOfLetter"].ToString()));
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                        insertedFollowUpMainIdFromSACFollowUpDetail = (int)comForIndividualSACFollowUpDetail.ExecuteScalar();
                                                        comForIndividualSACFollowUpDetail.Parameters.Clear();

                                                        #region Come Save Uploaded Files Here
                                                        if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                        {
                                                            //File saving part here
                                                            if (!string.IsNullOrEmpty(individualSACFollowUp.ScanCopyOfActionTakenFile) && individualSACFollowUp.ScanCopyOfActionTakenFile != "[]" && individualSACFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                            {
                                                                //Convert Base64 Encoded string to Byte Array.
                                                                byte[] fileBytes = Convert.FromBase64String(individualSACFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}")))
                                                                {
                                                                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}"));
                                                                }
                                                                File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}/File.{individualSACFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                            }
                                                        }
                                                        #endregion

                                                        #endregion
                                                    }
                                                    break;
                                                case "4":
                                                    individualSACFollowUp = item.ToObject<FollowUpDetailForFirst7DaysLetter>();
                                                    if (individualSACFollowUp != null)
                                                    {
                                                        #region Inserting into SAC FollowUp table(FollowUp by First 7 Days Letter)
                                                        comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                        comForIndividualSACFollowUpDetail.Connection = db;
                                                        comForIndividualSACFollowUpDetail.Transaction = tran;
                                                        comForIndividualSACFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailForFirst7DaysLetter]
                                                                                                        ([FollowUpMainId]
                                                                                                        ,[DateOfIssuanceOfFollowUpLetter]
                                                                                                        ,[DateOfReceiptOfLetter]
                                                                                                        ,[ResponseOfBorrower]
                                                                                                        ,[NextActionToBeTaken]
                                                                                                        ,[DueDateOfNextAction]) OUTPUT INSERTED.FollowUpMainId
                                                                                                    VALUES
                                                                                                        (@FollowUpMainId,
                                                                                                         @DateOfIssuanceOfFollowUpLetter,
                                                                                                         @DateOfReceiptOfLetter,
                                                                                                         @ResponseOfBorrower,
                                                                                                         @NextActionToBeTaken,
                                                                                                         @DueDateOfNextAction)";
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromSACFollowUpMain);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfIssuanceOfFollowUpLetter", item["DateOfIssuanceOfFollowUpLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfIssuanceOfFollowUpLetter"].ToString()));
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfReceiptOfLetter", item["DateOfReceiptOfLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfReceiptOfLetter"].ToString()));
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                        insertedFollowUpMainIdFromSACFollowUpDetail = (int)comForIndividualSACFollowUpDetail.ExecuteScalar();
                                                        comForIndividualSACFollowUpDetail.Parameters.Clear();

                                                        #region Come Save Uploaded Files Here
                                                        if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                        {
                                                            //File saving part here
                                                            if (!string.IsNullOrEmpty(individualSACFollowUp.ScanCopyOfActionTakenFile) && individualSACFollowUp.ScanCopyOfActionTakenFile != "[]" && individualSACFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                            {
                                                                //Convert Base64 Encoded string to Byte Array.
                                                                byte[] fileBytes = Convert.FromBase64String(individualSACFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}")))
                                                                {
                                                                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}"));
                                                                }
                                                                File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}/File.{individualSACFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                            }
                                                        }
                                                        #endregion

                                                        #endregion
                                                    }
                                                    break;
                                                case "5":
                                                    individualSACFollowUp = item.ToObject<FollowUpDetailForSecond7DaysLetter>();
                                                    if (individualSACFollowUp != null)
                                                    {
                                                        #region Inserting into SAC FollowUp table(FollowUp by Second 7 Days Letter)
                                                        comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                        comForIndividualSACFollowUpDetail.Connection = db;
                                                        comForIndividualSACFollowUpDetail.Transaction = tran;
                                                        comForIndividualSACFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailForSecond7DaysLetter]
                                                                                                       ([FollowUpMainId]
                                                                                                       ,[DateOfIssuanceOfFollowUpLetter]
                                                                                                       ,[DateOfReceiptOfLetter]
                                                                                                       ,[ResponseOfBorrower]
                                                                                                       ,[NextActionToBeTaken]
                                                                                                       ,[DueDateOfNextAction]) OUTPUT INSERTED.FollowUpMainId
                                                                                                 VALUES
                                                                                                       (@FollowUpMainId,
                                                                                                        @DateOfIssuanceOfFollowUpLetter,
                                                                                                        @DateOfReceiptOfLetter,
                                                                                                        @ResponseOfBorrower,
                                                                                                        @NextActionToBeTaken,
                                                                                                        @DueDateOfNextAction)";
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromSACFollowUpMain);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfIssuanceOfFollowUpLetter", item["DateOfIssuanceOfFollowUpLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfIssuanceOfFollowUpLetter"].ToString()));
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfReceiptOfLetter", item["DateOfReceiptOfLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfReceiptOfLetter"].ToString()));
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                        insertedFollowUpMainIdFromSACFollowUpDetail = (int)comForIndividualSACFollowUpDetail.ExecuteScalar();
                                                        comForIndividualSACFollowUpDetail.Parameters.Clear();

                                                        #region Come Save Uploaded Files Here
                                                        if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                        {
                                                            //File saving part here
                                                            if (!string.IsNullOrEmpty(individualSACFollowUp.ScanCopyOfActionTakenFile) && individualSACFollowUp.ScanCopyOfActionTakenFile != "[]" && individualSACFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                            {
                                                                //Convert Base64 Encoded string to Byte Array.
                                                                byte[] fileBytes = Convert.FromBase64String(individualSACFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}")))
                                                                {
                                                                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}"));
                                                                }
                                                                File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}/File.{individualSACFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                            }
                                                        }
                                                        #endregion

                                                        #endregion
                                                    }
                                                    break;
                                                case "6":
                                                    individualSACFollowUp = item.ToObject<FollowUpDetailFor35DaysCallNotice>();
                                                    if (individualSACFollowUp != null)
                                                    {
                                                        #region Inserting into SAC FollowUp table(FollowUp by 35 Days Call Notice)
                                                        comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                        comForIndividualSACFollowUpDetail.Connection = db;
                                                        comForIndividualSACFollowUpDetail.Transaction = tran;
                                                        comForIndividualSACFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailFor35DaysCallNotice]
                                                                                                       ([FollowUpMainId]
                                                                                                       ,[DateOfNotice]
                                                                                                       ,[NewspaperName]
                                                                                                       ,[ResponseOfBorrower]
                                                                                                       ,[NextActionToBeTaken]
                                                                                                       ,[DueDateOfNextAction]
                                                                                                       ,[FollowUpResultId]
                                                                                                       ,[Remarks]) OUTPUT INSERTED.FollowUpMainId
                                                                                                 VALUES
                                                                                                       (@FollowUpMainId,
                                                                                                        @DateOfNotice,
                                                                                                        @NewspaperName,
                                                                                                        @ResponseOfBorrower,
                                                                                                        @NextActionToBeTaken,
                                                                                                        @DueDateOfNextAction,
                                                                                                        @FollowUpResultId,
                                                                                                        @Remarks)";
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromSACFollowUpMain);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfNotice", item["DateOfNotice"].ToString() == "" ? DBNull.Value : ((object)item["DateOfNotice"].ToString()));
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NewspaperName", ((object)item["NewspaperName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpResultId", ((object)item["FollowUpResultId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@Remarks", ((object)item["Remarks"].ToString()) ?? DBNull.Value);
                                                        insertedFollowUpMainIdFromSACFollowUpDetail = (int)comForIndividualSACFollowUpDetail.ExecuteScalar();
                                                        comForIndividualSACFollowUpDetail.Parameters.Clear();

                                                        #region Come Save Uploaded Files Here
                                                        if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                        {
                                                            //File saving part here
                                                            if (!string.IsNullOrEmpty(individualSACFollowUp.ScanCopyOfActionTakenFile) && individualSACFollowUp.ScanCopyOfActionTakenFile != "[]" && individualSACFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                            {
                                                                //Convert Base64 Encoded string to Byte Array.
                                                                byte[] fileBytes = Convert.FromBase64String(individualSACFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}")))
                                                                {
                                                                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}"));
                                                                }
                                                                File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}/File.{individualSACFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                            }
                                                        }
                                                        #endregion

                                                        #endregion
                                                    }
                                                    break;
                                                case "7":
                                                    individualSACFollowUp = item.ToObject<FollowUpDetailForAuctionNotice>();
                                                    if (individualSACFollowUp != null)
                                                    {
                                                        #region Inserting into SAC FollowUp table(FollowUp by Auction Notice)
                                                        comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                        comForIndividualSACFollowUpDetail.Connection = db;
                                                        comForIndividualSACFollowUpDetail.Transaction = tran;
                                                        comForIndividualSACFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailForAuctionNotice]
                                                                                                       ([FollowUpMainId]
                                                                                                       ,[AuctionNoticeTypeId]
                                                                                                       ,[DateOfAuctionNotice]
                                                                                                       ,[AuctionNewspaperName]
                                                                                                       ,[AuctionResponseOfBorrower]
                                                                                                       ,[AuctionNextActionToBeTaken]
                                                                                                       ,[AuctionDueDateOfNextAction]
                                                                                                       ,[AuctionRemarks]
                                                                                                       ,[DateOfRevaluationForAuction]
                                                                                                       ,[RevaluatedFMVOfCollateralForAuction]) OUTPUT INSERTED.FollowUpMainId
                                                                                                 VALUES
                                                                                                       (@FollowUpMainId,
                                                                                                        @AuctionNoticeTypeId,
                                                                                                        @DateOfAuctionNotice,
                                                                                                        @AuctionNewspaperName,
                                                                                                        @AuctionResponseOfBorrower,
                                                                                                        @AuctionNextActionToBeTaken,
                                                                                                        @AuctionDueDateOfNextAction,
                                                                                                        @AuctionRemarks,
                                                                                                        @DateOfRevaluationForAuction,
                                                                                                        @RevaluatedFMVOfCollateralForAuction)";
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromSACFollowUpMain);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@AuctionNoticeTypeId", ((object)item["AuctionNoticeTypeId"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfAuctionNotice", item["DateOfAuctionNotice"].ToString() == "" ? DBNull.Value : ((object)item["DateOfAuctionNotice"].ToString()));
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@AuctionNewspaperName", ((object)item["AuctionNewspaperName"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@AuctionResponseOfBorrower", ((object)item["AuctionResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@AuctionNextActionToBeTaken", ((object)item["AuctionNextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@AuctionDueDateOfNextAction", item["AuctionDueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["AuctionDueDateOfNextAction"].ToString()));
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@AuctionRemarks", ((object)item["AuctionRemarks"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfRevaluationForAuction", item["DateOfRevaluationForAuction"].ToString() == "" ? DBNull.Value : ((object)item["DateOfRevaluationForAuction"].ToString()));
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@RevaluatedFMVOfCollateralForAuction", string.IsNullOrEmpty(item["RevaluatedFMVOfCollateralForAuction"].ToString()) ? DBNull.Value : (((object)item["RevaluatedFMVOfCollateralForAuction"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                        insertedFollowUpMainIdFromSACFollowUpDetail = (int)comForIndividualSACFollowUpDetail.ExecuteScalar();
                                                        comForIndividualSACFollowUpDetail.Parameters.Clear();

                                                        #region Come Save Uploaded Files Here
                                                        if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                        {
                                                            //File saving part here
                                                            if (!string.IsNullOrEmpty(individualSACFollowUp.ScanCopyOfActionTakenFile) && individualSACFollowUp.ScanCopyOfActionTakenFile != "[]" && individualSACFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                            {
                                                                //Convert Base64 Encoded string to Byte Array.
                                                                byte[] fileBytes = Convert.FromBase64String(individualSACFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}")))
                                                                {
                                                                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}"));
                                                                }
                                                                File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{insertedNPAMgmtId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}/File.{individualSACFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                            }
                                                        }
                                                        #endregion

                                                        #endregion
                                                    }
                                                    break;
                                                case "8":
                                                    individualSACFollowUp = item.ToObject<FollowUpDetailForOthers>();
                                                    if (individualSACFollowUp != null)
                                                    {
                                                        #region Inserting into SAC FollowUp table(FollowUp for Others)
                                                        comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                        comForIndividualSACFollowUpDetail.Connection = db;
                                                        comForIndividualSACFollowUpDetail.Transaction = tran;
                                                        comForIndividualSACFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailForOthers]
                                                                                       ([FollowUpMainId]
                                                                                       ,[ContactDate]
                                                                                       ,[ResponseOfBorrower]
                                                                                       ,[NextActionToBeTaken]
                                                                                       ,[DueDateOfNextAction]
                                                                                       ,[Remarks]) OUTPUT INSERTED.FollowUpMainId
                                                                                 VALUES
                                                                                       (@FollowUpMainId,
                                                                                        @ContactDate,
                                                                                        @ResponseOfBorrower,
                                                                                        @NextActionToBeTaken,
                                                                                        @DueDateOfNextAction,
                                                                                        @Remarks)";
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromSACFollowUpMain);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ContactDate", item["ContactDate"].ToString() == "" ? DBNull.Value : ((object)item["ContactDate"].ToString()));
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                        comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@Remarks", ((object)item["Remarks"].ToString()) ?? DBNull.Value);
                                                        insertedFollowUpMainIdFromSACFollowUpDetail = (int)comForIndividualSACFollowUpDetail.ExecuteScalar();
                                                        comForIndividualSACFollowUpDetail.Parameters.Clear();
                                                        #endregion
                                                    }
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #endregion

                            }
                            #endregion

                            #endregion

                            i = insertedNPAMgmtId;
                            tran.Commit();
                            //tran.Rollback();//For now in testing
                        }
                        #endregion

                        #endregion
                    }

                    #endregion
                }
                catch (SqlException sqlex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(sqlex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    logger.Error($"{sqlex.Message} :: Line Number: {line.ToString()}");
                    tran.Rollback();
                }
                catch (Exception ex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                    tran.Rollback();
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                    {
                        db.Close();
                    }
                }
            }
            return i;
        }

        public int UpdateData(NPAManagementMain npaModel)
        {
            SqlConnection db = new SqlConnection(connectionString);
            SqlCommand com = new SqlCommand();
            SqlCommand com2 = new SqlCommand();
            SqlCommand comForBorrowerDetail = new SqlCommand();
            SqlCommand comForLoanDetailMain = new SqlCommand();
            SqlCommand comForBranchFollowUpMain = new SqlCommand();
            SqlCommand comForIndividualBranchFollowUpDetail = new SqlCommand();
            SqlCommand comForSACFollowUpMain = new SqlCommand();
            SqlCommand comForIndividualSACFollowUpDetail = new SqlCommand();
            SqlCommand comForPrimaryCollateralMain = new SqlCommand();
            SqlCommand comForIndividualPrimaryCollateralDetail = new SqlCommand();
            SqlCommand comForSecondaryCollateralMain = new SqlCommand();
            SqlCommand comForIndividualSecondaryCollateralDetail = new SqlCommand();
            SqlTransaction tran;
            db.Open();
            tran = db.BeginTransaction();
            int i = 0;
            if (npaModel != null && npaModel.NPAManagementId != null)
            {
                try
                {
                    //Run all your insert statements here here
                    com.CommandType = CommandType.Text;
                    com.Connection = db;
                    com.Transaction = tran;
                    com.CommandText = @"INSERT INTO DBO.HistNPAManagementMain
		                                SELECT [NPAManagementId]
                                           ,[CIFNo]
                                           ,[GroupName]
                                           ,[BorrowerName]
                                           ,[BorrowerContactNumber]
                                           ,[BorrowerEmailAddress]
                                           ,[BorrowerPermanentAddressProvinceId]
                                           ,[BorrowerPermanentAddressZoneId]
                                           ,[BorrowerPermanentAddressDistrictId]
                                           ,[BorrowerPermanentAddressVDCMun]
                                           ,[BorrowerPermanentToleStreetName]
                                           ,[BorrowerPermanentHouseNumber]
                                           ,[TempAddSameAsPerAdd]
                                           ,[BorrowerTemporaryAddressProvinceId]
                                           ,[BorrowerTemporaryAddressZoneId]
                                           ,[BorrowerTemporaryAddressDistrictId]
                                           ,[BorrowerTemporaryAddressVDCMun]
                                           ,[BorrowerTemporaryToleStreetName]
                                           ,[BorrowerTemporaryHouseNumber]
                                           ,[BorrowerTypeId]
                                           ,[CurrentROName]
                                           ,[BranchLocationCode]
                                           ,[SACROName]
                                           ,[FileTransferDateToSAC]
                                           ,[ProvisionStatusIdOnTheDateOfTransfer]
                                           ,[DateOfRequestForBlacklisting]
                                           ,[BlacklistStatusOfRequest]
                                           ,[BlacklistDate]
                                           ,[BlacklistNumber]
                                           ,[DateOfInitiationForBookingNBA]
                                           ,[NBAStatusOfRequestId]
                                           ,[NBADateOfCompletion]
                                           ,[NBAAmount]
                                           ,[NBARemarks]
                                           ,[DateOfFileHandedOverToLegalForDRT]
                                           ,[DateOfFilingACaseInDRT]
                                           ,[DRTStatusOfRequestId]
                                           ,[DRTDateOfCompletion]
                                           ,[DRTAmount]
                                           ,[DRTRemarks]
                                           ,[DateOfInitiationForWriteOff]
                                           ,[WriteOffStatusOfRequestId]
                                           ,[WriteOffDateOfCompletion]
                                           ,[PrincipalOnWriteOffDate]
                                           ,[InterestOnWriteOffDate]
                                           ,[PrincipalRecoveredFromStartDateToLoanWriteOffDate]
                                           ,[InterestRecoveredFromStartDateToLoanWriteOffDate]
                                           ,[InterestOnInterestRecoveredFromStartDateToLoanWriteOffDate]
                                           ,[PenalAmountRecoveredFromStartDateToLoanWriteOffDate]
                                           ,[WriteOffNextActionToBeTaken]
                                           ,[WriteOffDueDateOfNextAction]
                                           ,[WriteOffRemarks]
                                           ,[EnteredBy]
                                           ,[EnteredOn],@UpdatedBy, @UpdatedOn, @UpdatedFor FROM NPAManagementMain WHERE NPAManagementId=@NPAManagementId";
                    //removed fields
                    //,[BorrowerTypeIndividualLegalDetail]
                    //,[PrimaryCollateralsList]
                    //,[SecondaryCollateralsList]
                    //,[LoanAccountDetailsList]
                    //,[BranchFollowUpDetail]
                    //,[SACFollowUpDetail]
                    com.Parameters.AddWithValue("@NPAManagementId", ((object)npaModel.NPAManagementId) ?? DBNull.Value);
                    com.Parameters.AddWithValue("@UpdatedBy", ((object)npaModel.UpdatedBy) ?? DBNull.Value);
                    com.Parameters.AddWithValue("@UpdatedOn", ((object)npaModel.UpdatedOn) ?? DBNull.Value);
                    com.Parameters.AddWithValue("@UpdatedFor", "Backup for Update");
                    int insertedHistNPAMgmtId = com.ExecuteNonQuery();
                    if (insertedHistNPAMgmtId > 0)
                    {
                        com2.CommandType = CommandType.Text;
                        com2.Connection = db;
                        com2.Transaction = tran;
                        com2.CommandText = @"UPDATE [dbo].[NPAManagementMain] SET
                                           [CIFNo] = @CIFNo
                                           ,[GroupName] = @GroupName
                                           ,[BorrowerName] = @BorrowerName
                                           ,[BorrowerContactNumber] = @BorrowerContactNumber
                                           ,[BorrowerEmailAddress] = @BorrowerEmailAddress
                                           ,[BorrowerPermanentAddressProvinceId] = @BorrowerPermanentAddressProvinceId
                                           ,[BorrowerPermanentAddressZoneId] = @BorrowerPermanentAddressZoneId
                                           ,[BorrowerPermanentAddressDistrictId] = @BorrowerPermanentAddressDistrictId
                                           ,[BorrowerPermanentAddressVDCMun] = @BorrowerPermanentAddressVDCMun
                                           ,[BorrowerPermanentToleStreetName] = @BorrowerPermanentToleStreetName
                                           ,[BorrowerPermanentHouseNumber] = @BorrowerPermanentHouseNumber
                                           ,[TempAddSameAsPerAdd] = @TempAddSameAsPerAdd
                                           ,[BorrowerTemporaryAddressProvinceId] = @BorrowerTemporaryAddressProvinceId
                                           ,[BorrowerTemporaryAddressZoneId] = @BorrowerTemporaryAddressZoneId
                                           ,[BorrowerTemporaryAddressDistrictId] = @BorrowerTemporaryAddressDistrictId
                                           ,[BorrowerTemporaryAddressVDCMun] = @BorrowerTemporaryAddressVDCMun
                                           ,[BorrowerTemporaryToleStreetName] = @BorrowerTemporaryToleStreetName
                                           ,[BorrowerTemporaryHouseNumber] = @BorrowerTemporaryHouseNumber
                                           ,[BorrowerTypeId] = @BorrowerTypeId
                                           ,[CurrentROName] = @CurrentROName
                                           ,[BranchLocationCode] = @BranchLocationCode
                                           ,[SACROName] = @SACROName
                                           ,[FileTransferDateToSAC] = @FileTransferDateToSAC
                                           ,[ProvisionStatusIdOnTheDateOfTransfer] = @ProvisionStatusIdOnTheDateOfTransfer
                                           ,[DateOfRequestForBlacklisting] = @DateOfRequestForBlacklisting
                                           ,[BlacklistStatusOfRequest] = @BlacklistStatusOfRequest
                                           ,[BlacklistDate] = @BlacklistDate
                                           ,[BlacklistNumber] = @BlacklistNumber
                                           ,[DateOfInitiationForBookingNBA] = @DateOfInitiationForBookingNBA
                                           ,[NBAStatusOfRequestId] = @NBAStatusOfRequestId
                                           ,[NBADateOfCompletion] = @NBADateOfCompletion
                                           ,[NBAAmount] = @NBAAmount
                                           ,[NBARemarks] = @NBARemarks
                                           ,[DateOfFileHandedOverToLegalForDRT] = @DateOfFileHandedOverToLegalForDRT
                                           ,[DateOfFilingACaseInDRT] = @DateOfFilingACaseInDRT
                                           ,[DRTStatusOfRequestId] = @DRTStatusOfRequestId
                                           ,[DRTDateOfCompletion] = @DRTDateOfCompletion
                                           ,[DRTAmount] = @DRTAmount
                                           ,[DRTRemarks] = @DRTRemarks
                                           ,[DateOfInitiationForWriteOff] = @DateOfInitiationForWriteOff
                                           ,[WriteOffStatusOfRequestId] = @WriteOffStatusOfRequestId
                                           ,[WriteOffDateOfCompletion] = @WriteOffDateOfCompletion
                                           ,[PrincipalOnWriteOffDate] = @PrincipalOnWriteOffDate
                                           ,[InterestOnWriteOffDate] = @InterestOnWriteOffDate
                                           ,[PrincipalRecoveredFromStartDateToLoanWriteOffDate] = @PrincipalRecoveredFromStartDateToLoanWriteOffDate
                                           ,[InterestRecoveredFromStartDateToLoanWriteOffDate] = @InterestRecoveredFromStartDateToLoanWriteOffDate
                                           ,[InterestOnInterestRecoveredFromStartDateToLoanWriteOffDate] = @InterestOnInterestRecoveredFromStartDateToLoanWriteOffDate
                                           ,[PenalAmountRecoveredFromStartDateToLoanWriteOffDate] = @PenalAmountRecoveredFromStartDateToLoanWriteOffDate
                                           ,[WriteOffNextActionToBeTaken] = @WriteOffNextActionToBeTaken
                                           ,[WriteOffDueDateOfNextAction] = @WriteOffDueDateOfNextAction
                                           ,[WriteOffRemarks] = @WriteOffRemarks
                                           ,[EnteredBy] = @EnteredBy
                                           ,[EnteredOn] = @EnteredOn
                                           ,[UpdatedBy] = @UpdatedBy
                                           ,[UpdatedOn] = @UpdatedOn WHERE NPAManagementId=@NPAManagementId";
                        //removed fields
                        //,[BorrowerTypeIndividualLegalDetail] = @BorrowerTypeIndividualLegalDetail
                        //,[PrimaryCollateralsList] = @PrimaryCollateralsList
                        //,[SecondaryCollateralsList] = @SecondaryCollateralsList
                        //,[LoanAccountDetailsList] = @LoanAccountDetailsList
                        //,[BranchFollowUpDetail] = @BranchFollowUpDetail
                        //,[SACFollowUpDetail] = @SACFollowUpDetail
                        com2.Parameters.AddWithValue("@NPAManagementId", ((object)npaModel.NPAManagementId) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@CIFNo", ((object)npaModel.CIFNo) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@GroupName", ((object)npaModel.GroupName) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@BorrowerName", ((object)npaModel.BorrowerName) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@BorrowerContactNumber", ((object)npaModel.BorrowerContactNumber) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@BorrowerEmailAddress", ((object)npaModel.BorrowerEmailAddress) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@BorrowerPermanentAddressProvinceId", ((object)npaModel.BorrowerPermanentAddressProvinceId) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@BorrowerPermanentAddressZoneId", ((object)npaModel.BorrowerPermanentAddressZoneId) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@BorrowerPermanentAddressDistrictId", ((object)npaModel.BorrowerPermanentAddressDistrictId) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@BorrowerPermanentAddressVDCMun", ((object)npaModel.BorrowerPermanentAddressVDCMun) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@BorrowerPermanentToleStreetName", ((object)npaModel.BorrowerPermanentToleStreetName) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@BorrowerPermanentHouseNumber", ((object)npaModel.BorrowerPermanentHouseNumber) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@TempAddSameAsPerAdd", ((object)npaModel.TempAddSameAsPerAdd) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@BorrowerTemporaryAddressProvinceId", ((object)npaModel.BorrowerTemporaryAddressProvinceId) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@BorrowerTemporaryAddressZoneId", ((object)npaModel.BorrowerTemporaryAddressZoneId) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@BorrowerTemporaryAddressDistrictId", ((object)npaModel.BorrowerTemporaryAddressDistrictId) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@BorrowerTemporaryAddressVDCMun", ((object)npaModel.BorrowerTemporaryAddressVDCMun) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@BorrowerTemporaryToleStreetName", ((object)npaModel.BorrowerTemporaryToleStreetName) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@BorrowerTemporaryHouseNumber", ((object)npaModel.BorrowerTemporaryHouseNumber) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@BorrowerTypeId", ((object)npaModel.BorrowerTypeId) ?? DBNull.Value);
                        //com2.Parameters.AddWithValue("@BorrowerTypeIndividualLegalDetail", ((object)npaModel.StringifiedBorrowerTypeIndividualLegalDetail) ?? DBNull.Value);
                        //com2.Parameters.AddWithValue("@PrimaryCollateralsList", ((object)npaModel.StringifiedListOfPrimaryCollateralDetailObjects) ?? DBNull.Value);
                        //com2.Parameters.AddWithValue("@SecondaryCollateralsList", ((object)npaModel.StringifiedListOfSecondaryCollateralDetailObjects) ?? DBNull.Value);
                        //com2.Parameters.AddWithValue("@LoanAccountDetailsList", ((object)npaModel.StringifiedListOfLoanAccountDetailObjects) ?? DBNull.Value);
                        //com2.Parameters.AddWithValue("@BranchFollowUpDetail", ((object)npaModel.StringifiedListOfBranchFollowUpDetailObjects) ?? DBNull.Value);
                        //com2.Parameters.AddWithValue("@SACFollowUpDetail", ((object)npaModel.StringifiedListOfSACFollowUpDetailObjects) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@CurrentROName", ((object)npaModel.CurrentROName) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@BranchLocationCode", ((object)npaModel.BranchLocationCode) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@SACROName", ((object)npaModel.SACROName) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@FileTransferDateToSAC", ((object)npaModel.FileTransferDateToSAC) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@ProvisionStatusIdOnTheDateOfTransfer", ((object)npaModel.ProvisionStatusIdOnTheDateOfTransfer) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@DateOfRequestForBlacklisting", ((object)npaModel.DateOfRequestForBlacklisting) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@BlacklistStatusOfRequest", ((object)npaModel.BlacklistStatusOfRequest) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@BlacklistDate", ((object)npaModel.BlacklistDate) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@BlacklistNumber", ((object)npaModel.BlacklistNumber) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@DateOfInitiationForBookingNBA", ((object)npaModel.DateOfInitiationForBookingNBA) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@NBAStatusOfRequestId", ((object)npaModel.NBAStatusOfRequestId) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@NBADateOfCompletion", ((object)npaModel.NBADateOfCompletion) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@NBAAmount", npaModel.NBAAmount == null ? DBNull.Value : (((object)npaModel.NBAAmount.ToString().Replace(",", "")) ?? DBNull.Value));
                        com2.Parameters.AddWithValue("@NBARemarks", ((object)npaModel.NBARemarks) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@DateOfFileHandedOverToLegalForDRT", ((object)npaModel.DateOfFileHandedOverToLegalForDRT) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@DateOfFilingACaseInDRT", ((object)npaModel.DateOfFilingACaseInDRT) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@DRTStatusOfRequestId", ((object)npaModel.DRTStatusOfRequestId) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@DRTDateOfCompletion", ((object)npaModel.DRTDateOfCompletion) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@DRTAmount", npaModel.DRTAmount == null ? DBNull.Value : (((object)npaModel.DRTAmount.ToString().Replace(",", "")) ?? DBNull.Value));
                        com2.Parameters.AddWithValue("@DRTRemarks", ((object)npaModel.DRTRemarks) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@DateOfInitiationForWriteOff", ((object)npaModel.DateOfInitiationForWriteOff) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@WriteOffStatusOfRequestId", ((object)npaModel.WriteOffStatusOfRequestId) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@WriteOffDateOfCompletion", ((object)npaModel.WriteOffDateOfCompletion) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@PrincipalOnWriteOffDate", npaModel.PrincipalOnWriteOffDate == null ? DBNull.Value : (((object)npaModel.PrincipalOnWriteOffDate.ToString().Replace(",", "")) ?? DBNull.Value));
                        com2.Parameters.AddWithValue("@InterestOnWriteOffDate", npaModel.InterestOnWriteOffDate == null ? DBNull.Value : (((object)npaModel.InterestOnWriteOffDate.ToString().Replace(",", "")) ?? DBNull.Value));
                        com2.Parameters.AddWithValue("@PrincipalRecoveredFromStartDateToLoanWriteOffDate", npaModel.PrincipalRecoveredFromStartDateToLoanWriteOffDate == null ? DBNull.Value : (((object)npaModel.PrincipalRecoveredFromStartDateToLoanWriteOffDate.ToString().Replace(",", "")) ?? DBNull.Value));
                        com2.Parameters.AddWithValue("@InterestRecoveredFromStartDateToLoanWriteOffDate", npaModel.InterestRecoveredFromStartDateToLoanWriteOffDate == null ? DBNull.Value : (((object)npaModel.InterestRecoveredFromStartDateToLoanWriteOffDate.ToString().Replace(",", "")) ?? DBNull.Value));
                        com2.Parameters.AddWithValue("@InterestOnInterestRecoveredFromStartDateToLoanWriteOffDate", npaModel.InterestOnInterestRecoveredFromStartDateToLoanWriteOffDate == null ? DBNull.Value : (((object)npaModel.InterestOnInterestRecoveredFromStartDateToLoanWriteOffDate.ToString().Replace(",", "")) ?? DBNull.Value));
                        com2.Parameters.AddWithValue("@PenalAmountRecoveredFromStartDateToLoanWriteOffDate", npaModel.PenalAmountRecoveredFromStartDateToLoanWriteOffDate == null ? DBNull.Value : (((object)npaModel.PenalAmountRecoveredFromStartDateToLoanWriteOffDate.ToString().Replace(",", "")) ?? DBNull.Value));
                        com2.Parameters.AddWithValue("@WriteOffNextActionToBeTaken", ((object)npaModel.WriteOffNextActionToBeTaken) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@WriteOffDueDateOfNextAction", ((object)npaModel.WriteOffDueDateOfNextAction) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@WriteOffRemarks", ((object)npaModel.WriteOffRemarks) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@EnteredBy", ((object)npaModel.EnteredBy) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@EnteredOn", ((object)npaModel.EnteredOn) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@UpdatedBy", ((object)npaModel.UpdatedBy) ?? DBNull.Value);
                        com2.Parameters.AddWithValue("@UpdatedOn", ((object)npaModel.UpdatedOn) ?? DBNull.Value);
                        int updatedNPAMgmtId = com2.ExecuteNonQuery();
                        if (updatedNPAMgmtId > 0)
                        {
                            DeleteDataFromTableWithWhere("BorrowerTypeIndividualDetail", "NPAManagementId", npaModel.NPAManagementId.ToString(), db, tran);
                            DeleteDataFromTableWithWhere("BorrowerTypeLegalEntitiesDetail", "NPAManagementId", npaModel.NPAManagementId.ToString(), db, tran);
                            DeleteDataFromTableWithWhere("LoanAccountDetail", "NPAManagementId", npaModel.NPAManagementId.ToString(), db, tran);
                            DeleteDataFromTableWithWhere("CollateralDetailMain", "NPAManagementId", npaModel.NPAManagementId.ToString(), db, tran);
                            //DeleteDataFromTableWithWhere("FollowUpMain", "NPAManagementId", npaModel.NPAManagementId.ToString(), db, tran);
                            #region Inserts in child tables

                            #region Insert start for Borrower Detail table(Individual or Legal)
                            int updatedNPAMgmtIdFromBorrowerDetail = 0;
                            if (!string.IsNullOrEmpty(npaModel.StringifiedBorrowerTypeIndividualLegalDetail) && npaModel.StringifiedBorrowerTypeIndividualLegalDetail != "[]" && npaModel.StringifiedBorrowerTypeIndividualLegalDetail != "{}")
                            {

                                #region Insert case for Individual Borrower
                                if (npaModel.BorrowerTypeId == 1)
                                {
                                    #region Parsing the StringifiedBorrowerTypeIndividualDetail and operating on it
                                    BorrowerTypeIndividualDetail borrowerTypeIndividualDetail = JsonConvert.DeserializeObject<BorrowerTypeIndividualDetail>(npaModel.StringifiedBorrowerTypeIndividualLegalDetail);
                                    if (borrowerTypeIndividualDetail != null)
                                    {
                                        #region Inserting into Individual Detail
                                        comForBorrowerDetail.CommandType = CommandType.Text;
                                        comForBorrowerDetail.Connection = db;
                                        comForBorrowerDetail.Transaction = tran;
                                        comForBorrowerDetail.CommandText = @"INSERT INTO [dbo].[BorrowerTypeIndividualDetail]
                                                       ([NPAManagementId]
                                                       ,[BorrowerTypeId]
                                                       ,[BorrowerFatherName]
                                                       ,[BorrowerGrandfatherName]
                                                       ,[BorrowerCitizenshipNumber]
                                                       ,[BorrowerSpouseName]
                                                       ,[BorrowerSonName]
                                                       ,[BorrowerDaughterName]) OUTPUT INSERTED.NPAManagementId
                                                 VALUES
                                                       (@NPAManagementId,
                                                        @BorrowerTypeId,
                                                        @BorrowerFatherName,
                                                        @BorrowerGrandfatherName,
                                                        @BorrowerCitizenshipNumber,
                                                        @BorrowerSpouseName,
                                                        @BorrowerSonName,
                                                        @BorrowerDaughterName)";
                                        comForBorrowerDetail.Parameters.AddWithValue("@NPAManagementId", npaModel.NPAManagementId);
                                        comForBorrowerDetail.Parameters.AddWithValue("@BorrowerTypeId", ((object)borrowerTypeIndividualDetail.BorrowerTypeId) ?? DBNull.Value);
                                        comForBorrowerDetail.Parameters.AddWithValue("@BorrowerFatherName", ((object)borrowerTypeIndividualDetail.BorrowerFatherName) ?? DBNull.Value);
                                        comForBorrowerDetail.Parameters.AddWithValue("@BorrowerGrandfatherName", ((object)borrowerTypeIndividualDetail.BorrowerGrandfatherName) ?? DBNull.Value);
                                        comForBorrowerDetail.Parameters.AddWithValue("@BorrowerCitizenshipNumber", ((object)borrowerTypeIndividualDetail.BorrowerCitizenshipNumber) ?? DBNull.Value);
                                        comForBorrowerDetail.Parameters.AddWithValue("@BorrowerSpouseName", ((object)borrowerTypeIndividualDetail.BorrowerSpouseName) ?? DBNull.Value);
                                        comForBorrowerDetail.Parameters.AddWithValue("@BorrowerSonName", ((object)borrowerTypeIndividualDetail.BorrowerSonName) ?? DBNull.Value);
                                        comForBorrowerDetail.Parameters.AddWithValue("@BorrowerDaughterName", ((object)borrowerTypeIndividualDetail.BorrowerDaughterName) ?? DBNull.Value);
                                        updatedNPAMgmtIdFromBorrowerDetail = (int)comForBorrowerDetail.ExecuteScalar();
                                        #endregion
                                    }
                                    #endregion
                                }
                                #endregion

                                #region Insert case for Legal Borrower
                                else if (npaModel.BorrowerTypeId == 2)
                                {
                                    #region Parsing the StringifiedBorrowerTypeLegalDetail and operating on it
                                    BorrowerTypeLegalEntitiesDetail borrowerTypeIndividualDetail = JsonConvert.DeserializeObject<BorrowerTypeLegalEntitiesDetail>(npaModel.StringifiedBorrowerTypeIndividualLegalDetail);
                                    if (borrowerTypeIndividualDetail != null)
                                    {
                                        #region Inserting into Legal Detail
                                        comForBorrowerDetail.CommandType = CommandType.Text;
                                        comForBorrowerDetail.Connection = db;
                                        comForBorrowerDetail.Transaction = tran;
                                        comForBorrowerDetail.CommandText = @"INSERT INTO [dbo].[BorrowerTypeLegalEntitiesDetail]
                                                                        ([NPAManagementId]
                                                                        ,[BorrowerTypeId]
                                                                        ,[LegalEntityRegistrationNumber]
                                                                        ,[LegalEntityRegistrationDate]
                                                                        ,[LegalEntityRegisteredOffice]) OUTPUT INSERTED.NPAManagementId
                                                                    VALUES
                                                                        (@NPAManagementId,
                                                                         @BorrowerTypeId,
                                                                         @LegalEntityRegistrationNumber,
                                                                         @LegalEntityRegistrationDate,
                                                                         @LegalEntityRegisteredOffice)";
                                        comForBorrowerDetail.Parameters.AddWithValue("@NPAManagementId", npaModel.NPAManagementId);
                                        comForBorrowerDetail.Parameters.AddWithValue("@BorrowerTypeId", ((object)borrowerTypeIndividualDetail.BorrowerTypeId) ?? DBNull.Value);
                                        comForBorrowerDetail.Parameters.AddWithValue("@LegalEntityRegistrationNumber", ((object)borrowerTypeIndividualDetail.LegalEntityRegistrationNumber) ?? DBNull.Value);
                                        comForBorrowerDetail.Parameters.AddWithValue("@LegalEntityRegistrationDate", ((object)borrowerTypeIndividualDetail.LegalEntityRegistrationDate) ?? DBNull.Value);
                                        comForBorrowerDetail.Parameters.AddWithValue("@LegalEntityRegisteredOffice", ((object)borrowerTypeIndividualDetail.LegalEntityRegisteredOffice) ?? DBNull.Value);
                                        updatedNPAMgmtIdFromBorrowerDetail = (int)comForBorrowerDetail.ExecuteScalar();
                                        #endregion
                                    }
                                    #endregion
                                }
                                #endregion

                            }
                            #endregion

                            #region Proceed if insertion to Borrower Detail succeeds
                            if (updatedNPAMgmtIdFromBorrowerDetail > 0)
                            {
                                #region Insert into details of Primary Collaterals
                                if (!string.IsNullOrEmpty(npaModel.StringifiedListOfPrimaryCollateralDetailObjects) && npaModel.StringifiedListOfPrimaryCollateralDetailObjects != "[]" && npaModel.StringifiedListOfPrimaryCollateralDetailObjects != "{}")
                                {

                                    #region Insert cases for Primary Collateral Details

                                    #region Parsing the stringifiedListOfPrimaryCollateralDetailObjects and operating on it

                                    string strListOfPrimaryCollaterals = $"{{\"primaryCollaterals\":{npaModel.StringifiedListOfPrimaryCollateralDetailObjects}}}";
                                    JObject joListOfPrimaryCollaterals = JObject.Parse(strListOfPrimaryCollaterals);
                                    JArray jaListOfPrimaryCollaterals = (JArray)joListOfPrimaryCollaterals["primaryCollaterals"];
                                    int insertedMainIdFromPrimaryCollateralMain = 0;
                                    int insertedMainIdFromPrimaryCollateralDetail = 0;
                                    foreach (var itemDetail in jaListOfPrimaryCollaterals)
                                    {
                                        if (itemDetail["CollateralTypeId"].ToString() != "")
                                        {
                                            insertedMainIdFromPrimaryCollateralDetail = 0;
                                            #region Insert to Collateral Main table Primary Collaterals
                                            comForPrimaryCollateralMain.CommandType = CommandType.Text;
                                            comForPrimaryCollateralMain.Connection = db;
                                            comForPrimaryCollateralMain.Transaction = tran;
                                            comForPrimaryCollateralMain.CommandText = @"INSERT INTO [dbo].[CollateralDetailMain]
                                                                                           ([NPAManagementId]
                                                                                           ,[CollateralTypeId]
                                                                                           ,[CollateralClass]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                            VALUES
                                                                                           (@NPAManagementId,
                                                                                            @CollateralTypeId,
                                                                                            @CollateralClass)";
                                            comForPrimaryCollateralMain.Parameters.AddWithValue("@NPAManagementId", npaModel.NPAManagementId);
                                            comForPrimaryCollateralMain.Parameters.AddWithValue("@CollateralTypeId", ((object)itemDetail["CollateralTypeId"].ToString()) ?? DBNull.Value);
                                            comForPrimaryCollateralMain.Parameters.AddWithValue("@CollateralClass", "1");
                                            insertedMainIdFromPrimaryCollateralMain = (int)comForPrimaryCollateralMain.ExecuteScalar();
                                            comForPrimaryCollateralMain.Parameters.Clear();
                                            #endregion
                                            if (insertedMainIdFromPrimaryCollateralMain > 0)
                                            {
                                                dynamic individualPrimaryCollateral = null;
                                                switch (itemDetail["CollateralTypeId"].ToString())
                                                {
                                                    case "1":
                                                        individualPrimaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfLand>();
                                                        if (individualPrimaryCollateral != null)
                                                        {
                                                            #region Inserting into Primary Collateral table(Collateral Land)
                                                            comForIndividualPrimaryCollateralDetail.CommandType = CommandType.Text;
                                                            comForIndividualPrimaryCollateralDetail.Connection = db;
                                                            comForIndividualPrimaryCollateralDetail.Transaction = tran;
                                                            comForIndividualPrimaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfLand]
                                                                                                                       ([CollateralDetailMainId]
                                                                                                                       ,[ProvinceId]
                                                                                                                       ,[ZoneId]
                                                                                                                       ,[DistrictId]
                                                                                                                       ,[VDCMun]
                                                                                                                       ,[Street]
                                                                                                                       ,[WardNumber]
                                                                                                                       ,[PlotNumber]
                                                                                                                       ,[AreaTypeId]
                                                                                                                       ,[Area]
                                                                                                                       ,[PropertyOwner]
                                                                                                                       ,[RelationWithBorrower]
                                                                                                                       ,[FirstValuatorName]
                                                                                                                       ,[FirstValuationDate]
                                                                                                                       ,[FirstFMVOfProperty]
                                                                                                                       ,[FirstValuatorExistsInSBLCurrentlyYN]
                                                                                                                       ,[LatestValuatorName]
                                                                                                                       ,[LatestValuationDate]
                                                                                                                       ,[LatestValuatorExistsInSBLCurrentlyYN]
                                                                                                                       ,[FMVOfPropertyAsPerLatestValuationReport]
                                                                                                                       ,[InsuranceCoverageType]
                                                                                                                       ,[InsuranceExpiryDate]
                                                                                                                       ,[InsuranceAmount]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                 VALUES
                                                                                                                       (@CollateralDetailMainId,
                                                                                                                        @ProvinceId,
                                                                                                                        @ZoneId,
                                                                                                                        @DistrictId,
                                                                                                                        @VDCMun,
                                                                                                                        @Street,
                                                                                                                        @WardNumber,
                                                                                                                        @PlotNumber,
                                                                                                                        @AreaTypeId,
                                                                                                                        @Area,
                                                                                                                        @PropertyOwner,
                                                                                                                        @RelationWithBorrower,
                                                                                                                        @FirstValuatorName,
                                                                                                                        @FirstValuationDate,
                                                                                                                        @FirstFMVOfProperty,
                                                                                                                        @FirstValuatorExistsInSBLCurrentlyYN,
                                                                                                                        @LatestValuatorName,
                                                                                                                        @LatestValuationDate,
                                                                                                                        @LatestValuatorExistsInSBLCurrentlyYN,
                                                                                                                        @FMVOfPropertyAsPerLatestValuationReport,
                                                                                                                        @InsuranceCoverageType,
                                                                                                                        @InsuranceExpiryDate,
                                                                                                                        @InsuranceAmount)";
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromPrimaryCollateralMain);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ProvinceId", ((object)itemDetail["ProvinceId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ZoneId", ((object)itemDetail["ZoneId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@DistrictId", ((object)itemDetail["DistrictId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@VDCMun", ((object)itemDetail["VDCMun"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@Street", ((object)itemDetail["Street"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@WardNumber", ((object)itemDetail["WardNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@PlotNumber", ((object)itemDetail["PlotNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@AreaTypeId", !string.IsNullOrEmpty(itemDetail["AreaTypeId"].ToString()) ? (object)itemDetail["AreaTypeId"].ToString() : DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@Area", ((object)itemDetail["Area"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@PropertyOwner", ((object)itemDetail["PropertyOwner"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@RelationWithBorrower", ((object)itemDetail["RelationWithBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorName", ((object)itemDetail["FirstValuatorName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuationDate", itemDetail["FirstValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["FirstValuationDate"].ToString()));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstFMVOfProperty", string.IsNullOrEmpty(itemDetail["FirstFMVOfProperty"].ToString()) ? DBNull.Value : (((object)itemDetail["FirstFMVOfProperty"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorName", ((object)itemDetail["LatestValuatorName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuationDate", itemDetail["LatestValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["LatestValuationDate"].ToString()));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FMVOfPropertyAsPerLatestValuationReport", string.IsNullOrEmpty(itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString()) ? DBNull.Value : (((object)itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceCoverageType", ((object)itemDetail["InsuranceCoverageType"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceExpiryDate", itemDetail["InsuranceExpiryDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["InsuranceExpiryDate"].ToString()));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceAmount", string.IsNullOrEmpty(itemDetail["InsuranceAmount"].ToString()) ? DBNull.Value : (((object)itemDetail["InsuranceAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            insertedMainIdFromPrimaryCollateralDetail = (int)comForIndividualPrimaryCollateralDetail.ExecuteScalar();
                                                            comForIndividualPrimaryCollateralDetail.Parameters.Clear();
                                                            #endregion
                                                        }
                                                        break;
                                                    case "2":
                                                        individualPrimaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfBuilding>();
                                                        if (individualPrimaryCollateral != null)
                                                        {
                                                            #region Inserting into Primary Collateral table(Collateral Land and Building)
                                                            comForIndividualPrimaryCollateralDetail.CommandType = CommandType.Text;
                                                            comForIndividualPrimaryCollateralDetail.Connection = db;
                                                            comForIndividualPrimaryCollateralDetail.Transaction = tran;
                                                            comForIndividualPrimaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfBuilding]
                                                                                                                        ([CollateralDetailMainId]
                                                                                                                        ,[ProvinceId]
                                                                                                                        ,[ZoneId]
                                                                                                                        ,[DistrictId]
                                                                                                                        ,[VDCMun]
                                                                                                                        ,[Street]
                                                                                                                        ,[WardNumber]
                                                                                                                        ,[PlotNumber]
                                                                                                                        ,[AreaTypeId]
                                                                                                                        ,[AreaOfLand]
                                                                                                                        ,[AreaOfBuilding]
                                                                                                                        ,[PropertyOwner]
                                                                                                                        ,[RelationWithBorrower]
                                                                                                                        ,[ConstructionCompletionDate]
                                                                                                                        ,[FirstValuatorName]
                                                                                                                        ,[FirstValuationDate]
                                                                                                                        ,[FirstFMVOfProperty]
                                                                                                                        ,[FirstValuatorExistsInSBLCurrentlyYN]
                                                                                                                        ,[LatestValuatorName]
                                                                                                                        ,[LatestValuationDate]
                                                                                                                        ,[LatestValuatorExistsInSBLCurrentlyYN]
                                                                                                                        ,[FMVOfPropertyAsPerLatestValuationReport]
                                                                                                                        ,[InsuranceCoverageType]
                                                                                                                        ,[InsuranceExpiryDate]
                                                                                                                        ,[InsuranceAmount]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                    VALUES
                                                                                                                        (@CollateralDetailMainId,
                                                                                                                         @ProvinceId,
                                                                                                                         @ZoneId,
                                                                                                                         @DistrictId,
                                                                                                                         @VDCMun,
                                                                                                                         @Street,
                                                                                                                         @WardNumber,
                                                                                                                         @PlotNumber,
                                                                                                                         @AreaTypeId,
                                                                                                                         @AreaOfLand,
                                                                                                                         @AreaOfBuilding,
                                                                                                                         @PropertyOwner,
                                                                                                                         @RelationWithBorrower,
                                                                                                                         @ConstructionCompletionDate,
                                                                                                                         @FirstValuatorName,
                                                                                                                         @FirstValuationDate,
                                                                                                                         @FirstFMVOfProperty,
                                                                                                                         @FirstValuatorExistsInSBLCurrentlyYN,
                                                                                                                         @LatestValuatorName,
                                                                                                                         @LatestValuationDate,
                                                                                                                         @LatestValuatorExistsInSBLCurrentlyYN,
                                                                                                                         @FMVOfPropertyAsPerLatestValuationReport,
                                                                                                                         @InsuranceCoverageType,
                                                                                                                         @InsuranceExpiryDate,
                                                                                                                         @InsuranceAmount)";
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromPrimaryCollateralMain);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ProvinceId", ((object)itemDetail["ProvinceId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ZoneId", ((object)itemDetail["ZoneId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@DistrictId", ((object)itemDetail["DistrictId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@VDCMun", ((object)itemDetail["VDCMun"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@Street", ((object)itemDetail["Street"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@WardNumber", ((object)itemDetail["WardNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@PlotNumber", ((object)itemDetail["PlotNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@AreaTypeId", !string.IsNullOrEmpty(itemDetail["AreaTypeId"].ToString()) ? (object)itemDetail["AreaTypeId"].ToString() : DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@AreaOfLand", ((object)itemDetail["AreaOfLand"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@AreaOfBuilding", ((object)itemDetail["AreaOfBuilding"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@PropertyOwner", ((object)itemDetail["PropertyOwner"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@RelationWithBorrower", ((object)itemDetail["RelationWithBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ConstructionCompletionDate", itemDetail["ConstructionCompletionDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["ConstructionCompletionDate"].ToString()));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorName", ((object)itemDetail["FirstValuatorName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuationDate", itemDetail["FirstValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["FirstValuationDate"].ToString()));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstFMVOfProperty", string.IsNullOrEmpty(itemDetail["FirstFMVOfProperty"].ToString()) ? DBNull.Value : (((object)itemDetail["FirstFMVOfProperty"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorName", ((object)itemDetail["LatestValuatorName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuationDate", itemDetail["LatestValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["LatestValuationDate"].ToString()));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FMVOfPropertyAsPerLatestValuationReport", string.IsNullOrEmpty(itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString()) ? DBNull.Value : (((object)itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceCoverageType", ((object)itemDetail["InsuranceCoverageType"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceExpiryDate", itemDetail["InsuranceExpiryDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["InsuranceExpiryDate"].ToString()));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceAmount", string.IsNullOrEmpty(itemDetail["InsuranceAmount"].ToString()) ? DBNull.Value : (((object)itemDetail["InsuranceAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            insertedMainIdFromPrimaryCollateralDetail = (int)comForIndividualPrimaryCollateralDetail.ExecuteScalar();
                                                            comForIndividualPrimaryCollateralDetail.Parameters.Clear();
                                                            #endregion
                                                        }
                                                        break;
                                                    case "3":
                                                        individualPrimaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfVehicle>();
                                                        if (individualPrimaryCollateral != null)
                                                        {
                                                            #region Inserting into Primary Collateral table(Collateral Vehicle)
                                                            comForIndividualPrimaryCollateralDetail.CommandType = CommandType.Text;
                                                            comForIndividualPrimaryCollateralDetail.Connection = db;
                                                            comForIndividualPrimaryCollateralDetail.Transaction = tran;
                                                            comForIndividualPrimaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfVehicle]
                                                                                                                       ([CollateralDetailMainId]
                                                                                                                       ,[VehicleTypeId]
                                                                                                                       ,[VehicleModel]
                                                                                                                       ,[EngineNumber]
                                                                                                                       ,[ChassisNumber]
                                                                                                                       ,[VehicleRegistrationNumber]
                                                                                                                       ,[VehicleRegistrationDate]
                                                                                                                       ,[VehicleRegisteredOffice]
                                                                                                                       ,[VehicleMakeYear]
                                                                                                                       ,[FirstValuatorName]
                                                                                                                       ,[FirstValuationDate]
                                                                                                                       ,[FirstFMVOfProperty]
                                                                                                                       ,[FirstValuatorExistsInSBLCurrentlyYN]
                                                                                                                       ,[LatestValuatorName]
                                                                                                                       ,[LatestValuationDate]
                                                                                                                       ,[LatestValuatorExistsInSBLCurrentlyYN]
                                                                                                                       ,[ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice]
                                                                                                                       ,[InsuranceCoverageType]
                                                                                                                       ,[InsuranceExpiryDate]
                                                                                                                       ,[InsuranceAmount]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                 VALUES
                                                                                                                       (@CollateralDetailMainId,
                                                                                                                        @VehicleTypeId,
                                                                                                                        @VehicleModel,
                                                                                                                        @EngineNumber,
                                                                                                                        @ChassisNumber,
                                                                                                                        @VehicleRegistrationNumber,
                                                                                                                        @VehicleRegistrationDate,
                                                                                                                        @VehicleRegisteredOffice,
                                                                                                                        @VehicleMakeYear,
                                                                                                                        @FirstValuatorName,
                                                                                                                        @FirstValuationDate,
                                                                                                                        @FirstFMVOfProperty,
                                                                                                                        @FirstValuatorExistsInSBLCurrentlyYN,
                                                                                                                        @LatestValuatorName,
                                                                                                                        @LatestValuationDate,
                                                                                                                        @LatestValuatorExistsInSBLCurrentlyYN,
                                                                                                                        @ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice,
                                                                                                                        @InsuranceCoverageType,
                                                                                                                        @InsuranceExpiryDate,
                                                                                                                        @InsuranceAmount)";
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromPrimaryCollateralMain);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@VehicleTypeId", !string.IsNullOrEmpty(itemDetail["VehicleTypeId"].ToString()) ? (object)itemDetail["VehicleTypeId"].ToString() : DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@VehicleModel", ((object)itemDetail["VehicleModel"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@EngineNumber", ((object)itemDetail["EngineNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ChassisNumber", ((object)itemDetail["ChassisNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@VehicleRegistrationNumber", ((object)itemDetail["VehicleRegistrationNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@VehicleRegistrationDate", itemDetail["VehicleRegistrationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["VehicleRegistrationDate"].ToString()));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@VehicleRegisteredOffice", ((object)itemDetail["VehicleRegisteredOffice"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@VehicleMakeYear", ((object)itemDetail["VehicleMakeYear"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorName", ((object)itemDetail["FirstValuatorName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuationDate", itemDetail["FirstValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["FirstValuationDate"].ToString()));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstFMVOfProperty", string.IsNullOrEmpty(itemDetail["FirstFMVOfProperty"].ToString()) ? DBNull.Value : (((object)itemDetail["FirstFMVOfProperty"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorName", ((object)itemDetail["LatestValuatorName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuationDate", itemDetail["LatestValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["LatestValuationDate"].ToString()));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice", string.IsNullOrEmpty(itemDetail["ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice"].ToString()) ? DBNull.Value : (((object)itemDetail["ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceCoverageType", ((object)itemDetail["InsuranceCoverageType"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceExpiryDate", itemDetail["InsuranceExpiryDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["InsuranceExpiryDate"].ToString()));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceAmount", string.IsNullOrEmpty(itemDetail["InsuranceAmount"].ToString()) ? DBNull.Value : (((object)itemDetail["InsuranceAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            insertedMainIdFromPrimaryCollateralDetail = (int)comForIndividualPrimaryCollateralDetail.ExecuteScalar();
                                                            comForIndividualPrimaryCollateralDetail.Parameters.Clear();
                                                            #endregion
                                                        }
                                                        break;
                                                    case "4":
                                                        individualPrimaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfPlantAndMachinery>();
                                                        if (individualPrimaryCollateral != null)
                                                        {
                                                            #region Inserting into Primary Collateral table(Collateral Plant and Machinery)
                                                            comForIndividualPrimaryCollateralDetail.CommandType = CommandType.Text;
                                                            comForIndividualPrimaryCollateralDetail.Connection = db;
                                                            comForIndividualPrimaryCollateralDetail.Transaction = tran;
                                                            comForIndividualPrimaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfPlantAndMachinery]
                                                                                                                        ([CollateralDetailMainId]
                                                                                                                        ,[PlantAndMachineryModel]
                                                                                                                        ,[EngineNumber]
                                                                                                                        ,[ChassisNumber]
                                                                                                                        ,[RegistrationNumber]
                                                                                                                        ,[FirstValuatorName]
                                                                                                                        ,[FirstValuationDate]
                                                                                                                        ,[FirstFMVOfProperty]
                                                                                                                        ,[FirstValuatorExistsInSBLCurrentlyYN]
                                                                                                                        ,[LatestValuatorName]
                                                                                                                        ,[LatestValuationDate]
                                                                                                                        ,[LatestValuatorExistsInSBLCurrentlyYN]
                                                                                                                        ,[FMVOfPropertyAsPerLatestValuationReport]
                                                                                                                        ,[InsuranceCoverageType]
                                                                                                                        ,[InsuranceExpiryDate]
                                                                                                                        ,[InsuranceAmount]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                    VALUES
                                                                                                                        (@CollateralDetailMainId,
                                                                                                                         @PlantAndMachineryModel,
                                                                                                                         @EngineNumber,
                                                                                                                         @ChassisNumber,
                                                                                                                         @RegistrationNumber,
                                                                                                                         @FirstValuatorName,
                                                                                                                         @FirstValuationDate,
                                                                                                                         @FirstFMVOfProperty,
                                                                                                                         @FirstValuatorExistsInSBLCurrentlyYN,
                                                                                                                         @LatestValuatorName,
                                                                                                                         @LatestValuationDate,
                                                                                                                         @LatestValuatorExistsInSBLCurrentlyYN,
                                                                                                                         @FMVOfPropertyAsPerLatestValuationReport,
                                                                                                                         @InsuranceCoverageType,
                                                                                                                         @InsuranceExpiryDate,
                                                                                                                         @InsuranceAmount)";
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromPrimaryCollateralMain);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@PlantAndMachineryModel", ((object)itemDetail["PlantAndMachineryModel"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@EngineNumber", ((object)itemDetail["EngineNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ChassisNumber", ((object)itemDetail["ChassisNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@RegistrationNumber", ((object)itemDetail["RegistrationNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorName", ((object)itemDetail["FirstValuatorName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuationDate", itemDetail["FirstValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["FirstValuationDate"].ToString()));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstFMVOfProperty", string.IsNullOrEmpty(itemDetail["FirstFMVOfProperty"].ToString()) ? DBNull.Value : (((object)itemDetail["FirstFMVOfProperty"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorName", ((object)itemDetail["LatestValuatorName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuationDate", itemDetail["LatestValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["LatestValuationDate"].ToString()));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FMVOfPropertyAsPerLatestValuationReport", string.IsNullOrEmpty(itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString()) ? DBNull.Value : (((object)itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceCoverageType", ((object)itemDetail["InsuranceCoverageType"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceExpiryDate", itemDetail["InsuranceExpiryDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["InsuranceExpiryDate"].ToString()));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceAmount", string.IsNullOrEmpty(itemDetail["InsuranceAmount"].ToString()) ? DBNull.Value : (((object)itemDetail["InsuranceAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            insertedMainIdFromPrimaryCollateralDetail = (int)comForIndividualPrimaryCollateralDetail.ExecuteScalar();
                                                            comForIndividualPrimaryCollateralDetail.Parameters.Clear();
                                                            #endregion
                                                        }
                                                        break;
                                                    case "5":
                                                        individualPrimaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfCurrentAsset>();
                                                        if (individualPrimaryCollateral != null)
                                                        {
                                                            #region Inserting into Primary Collateral table(Collateral Current Asset)
                                                            comForIndividualPrimaryCollateralDetail.CommandType = CommandType.Text;
                                                            comForIndividualPrimaryCollateralDetail.Connection = db;
                                                            comForIndividualPrimaryCollateralDetail.Transaction = tran;
                                                            comForIndividualPrimaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfCurrentAsset]
                                                                                                                        ([CollateralDetailMainId]
                                                                                                                       ,[DateOfLatestStockInspectionReportCollectedByBranch]
                                                                                                                       ,[WorkingCapitalAsPerStockReport]
                                                                                                                       ,[WorkingCapitalAsPerStockInspector]
                                                                                                                       ,[DrawingPower]
                                                                                                                       ,[InsuranceCoverageType]
                                                                                                                       ,[InsuranceExpiryDate]
                                                                                                                       ,[InsuranceAmount]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                 VALUES
                                                                                                                       (@CollateralDetailMainId,
                                                                                                                        @DateOfLatestStockInspectionReportCollectedByBranch,
                                                                                                                        @WorkingCapitalAsPerStockReport,
                                                                                                                        @WorkingCapitalAsPerStockInspector,
                                                                                                                        @DrawingPower,
                                                                                                                        @InsuranceCoverageType,
                                                                                                                        @InsuranceExpiryDate,
                                                                                                                        @InsuranceAmount)";
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromPrimaryCollateralMain);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@DateOfLatestStockInspectionReportCollectedByBranch", itemDetail["DateOfLatestStockInspectionReportCollectedByBranch"].ToString() == "" ? DBNull.Value : ((object)itemDetail["DateOfLatestStockInspectionReportCollectedByBranch"].ToString()));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@WorkingCapitalAsPerStockReport", string.IsNullOrEmpty(itemDetail["WorkingCapitalAsPerStockReport"].ToString()) ? DBNull.Value : (((object)itemDetail["WorkingCapitalAsPerStockReport"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@WorkingCapitalAsPerStockInspector", string.IsNullOrEmpty(itemDetail["WorkingCapitalAsPerStockInspector"].ToString()) ? DBNull.Value : (((object)itemDetail["WorkingCapitalAsPerStockInspector"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@DrawingPower", ((object)itemDetail["DrawingPower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceCoverageType", ((object)itemDetail["InsuranceCoverageType"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceExpiryDate", itemDetail["InsuranceExpiryDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["InsuranceExpiryDate"].ToString()));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@InsuranceAmount", string.IsNullOrEmpty(itemDetail["InsuranceAmount"].ToString()) ? DBNull.Value : (((object)itemDetail["InsuranceAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            insertedMainIdFromPrimaryCollateralDetail = (int)comForIndividualPrimaryCollateralDetail.ExecuteScalar();
                                                            comForIndividualPrimaryCollateralDetail.Parameters.Clear();
                                                            #endregion
                                                        }
                                                        break;
                                                    case "6":
                                                        individualPrimaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfPersonalGuarantee>();
                                                        if (individualPrimaryCollateral != null)
                                                        {
                                                            #region Inserting into Primary Collateral table(Collateral Personal Guarantee)
                                                            comForIndividualPrimaryCollateralDetail.CommandType = CommandType.Text;
                                                            comForIndividualPrimaryCollateralDetail.Connection = db;
                                                            comForIndividualPrimaryCollateralDetail.Transaction = tran;
                                                            comForIndividualPrimaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfPersonalGuarantee]
                                                                                                                       ([CollateralDetailMainId]
                                                                                                                       ,[NameOfGuarantor]
                                                                                                                       ,[CitizenshipNumber]
                                                                                                                       ,[FatherName]
                                                                                                                       ,[GrandfatherName]
                                                                                                                       ,[PermanentAddress]
                                                                                                                       ,[CurrentAddress]
                                                                                                                       ,[NetworthOfGuarantor]
                                                                                                                       ,[RelationWithBorrower]
                                                                                                                       ,[ContactNumber]
                                                                                                                       ,[Profession]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                 VALUES
                                                                                                                       (@CollateralDetailMainId,
                                                                                                                        @NameOfGuarantor,
                                                                                                                        @CitizenshipNumber,
                                                                                                                        @FatherName,
                                                                                                                        @GrandfatherName,
                                                                                                                        @PermanentAddress,
                                                                                                                        @CurrentAddress,
                                                                                                                        @NetworthOfGuarantor,
                                                                                                                        @RelationWithBorrower,
                                                                                                                        @ContactNumber,
                                                                                                                        @Profession)";
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromPrimaryCollateralMain);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@NameOfGuarantor", ((object)itemDetail["NameOfGuarantor"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CitizenshipNumber", ((object)itemDetail["CitizenshipNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FatherName", ((object)itemDetail["FatherName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@GrandfatherName", ((object)itemDetail["GrandfatherName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@PermanentAddress", ((object)itemDetail["PermanentAddress"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CurrentAddress", ((object)itemDetail["CurrentAddress"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@NetworthOfGuarantor", string.IsNullOrEmpty(itemDetail["NetworthOfGuarantor"].ToString()) ? DBNull.Value : (((object)itemDetail["NetworthOfGuarantor"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@RelationWithBorrower", ((object)itemDetail["RelationWithBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ContactNumber", ((object)itemDetail["ContactNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@Profession", ((object)itemDetail["Profession"].ToString()) ?? DBNull.Value);
                                                            insertedMainIdFromPrimaryCollateralDetail = (int)comForIndividualPrimaryCollateralDetail.ExecuteScalar();
                                                            comForIndividualPrimaryCollateralDetail.Parameters.Clear();
                                                            #endregion
                                                        }
                                                        break;
                                                    case "7":
                                                        individualPrimaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfCorporateGuarantee>();
                                                        if (individualPrimaryCollateral != null)
                                                        {
                                                            #region Inserting into Primary Collateral table(Collateral Corporate Guarantee)
                                                            comForIndividualPrimaryCollateralDetail.CommandType = CommandType.Text;
                                                            comForIndividualPrimaryCollateralDetail.Connection = db;
                                                            comForIndividualPrimaryCollateralDetail.Transaction = tran;
                                                            comForIndividualPrimaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfCorporateGuarantee]
                                                                                                                       ([CollateralDetailMainId]
                                                                                                                       ,[NameOfFirm]
                                                                                                                       ,[RegistrationNumber]
                                                                                                                       ,[FirmAddress]
                                                                                                                       ,[NetworthOfTheFirm]
                                                                                                                       ,[ContactPerson]
                                                                                                                       ,[ContactNumber]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                 VALUES
                                                                                                                       (@CollateralDetailMainId,
                                                                                                                        @NameOfFirm,
                                                                                                                        @RegistrationNumber,
                                                                                                                        @FirmAddress,
                                                                                                                        @NetworthOfTheFirm,
                                                                                                                        @ContactPerson,
                                                                                                                        @ContactNumber)";
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromPrimaryCollateralMain);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@NameOfFirm", ((object)itemDetail["NameOfFirm"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@RegistrationNumber", ((object)itemDetail["RegistrationNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@FirmAddress", ((object)itemDetail["FirmAddress"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@NetworthOfTheFirm", string.IsNullOrEmpty(itemDetail["NetworthOfTheFirm"].ToString()) ? DBNull.Value : (((object)itemDetail["NetworthOfTheFirm"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ContactPerson", ((object)itemDetail["ContactPerson"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ContactNumber", ((object)itemDetail["ContactNumber"].ToString()) ?? DBNull.Value);
                                                            insertedMainIdFromPrimaryCollateralDetail = (int)comForIndividualPrimaryCollateralDetail.ExecuteScalar();
                                                            comForIndividualPrimaryCollateralDetail.Parameters.Clear();
                                                            #endregion
                                                        }
                                                        break;
                                                    case "8":
                                                        individualPrimaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfStock>();
                                                        if (individualPrimaryCollateral != null)
                                                        {
                                                            #region Inserting into Primary Collateral table(Collateral Stock)
                                                            comForIndividualPrimaryCollateralDetail.CommandType = CommandType.Text;
                                                            comForIndividualPrimaryCollateralDetail.Connection = db;
                                                            comForIndividualPrimaryCollateralDetail.Transaction = tran;
                                                            comForIndividualPrimaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfStock]
                                                                                                                                       ([CollateralDetailMainId]
                                                                                                                                       ,[ShareTypeId]
                                                                                                                                       ,[CompanyName]
                                                                                                                                       ,[ListedInNepseBoolId]
                                                                                                                                       ,[PledgedUnits]
                                                                                                                                       ,[UnitTypeId]
                                                                                                                                       ,[ValueOfShare]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                                 VALUES
                                                                                                                                       (@CollateralDetailMainId,
			                                                                                                                            @ShareTypeId,
			                                                                                                                            @CompanyName,
			                                                                                                                            @ListedInNepseBoolId,
			                                                                                                                            @PledgedUnits,
			                                                                                                                            @UnitTypeId,
			                                                                                                                            @ValueOfShare)";
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromPrimaryCollateralMain);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ShareTypeId", ((object)itemDetail["ShareTypeId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CompanyName", ((object)itemDetail["CompanyName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ListedInNepseBoolId", ((object)itemDetail["ListedInNepseBoolId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@PledgedUnits", string.IsNullOrEmpty(itemDetail["PledgedUnits"].ToString()) ? DBNull.Value : (((object)itemDetail["PledgedUnits"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@UnitTypeId", ((object)itemDetail["ShareUnitTypeId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ValueOfShare", string.IsNullOrEmpty(itemDetail["ValueOfShare"].ToString()) ? DBNull.Value : (((object)itemDetail["ValueOfShare"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            insertedMainIdFromPrimaryCollateralDetail = (int)comForIndividualPrimaryCollateralDetail.ExecuteScalar();
                                                            comForIndividualPrimaryCollateralDetail.Parameters.Clear();
                                                            #endregion
                                                        }
                                                        break;
                                                    case "9":
                                                        individualPrimaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfGold>();
                                                        if (individualPrimaryCollateral != null)
                                                        {
                                                            #region Inserting into Primary Collateral table(Collateral Gold)
                                                            comForIndividualPrimaryCollateralDetail.CommandType = CommandType.Text;
                                                            comForIndividualPrimaryCollateralDetail.Connection = db;
                                                            comForIndividualPrimaryCollateralDetail.Transaction = tran;
                                                            comForIndividualPrimaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfGold]
                                                                                                                                   ([CollateralDetailMainId]
                                                                                                                                   ,[Quantity]
                                                                                                                                   ,[MeasurementUnitId]
                                                                                                                                   ,[GoldTypeId]
                                                                                                                                   ,[ValueOfGold]
                                                                                                                                   ,[NameOfGoldTester]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                             VALUES
                                                                                                                                   (@CollateralDetailMainId,
                                                                                                                                    @Quantity,
                                                                                                                                    @MeasurementUnitId,
                                                                                                                                    @GoldTypeId,
                                                                                                                                    @ValueOfGold,
                                                                                                                                    @NameOfGoldTester)";
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromPrimaryCollateralMain);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@Quantity", ((object)itemDetail["Quantity"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@MeasurementUnitId", ((object)itemDetail["MeasurementUnitId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@GoldTypeId", ((object)itemDetail["GoldTypeId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@ValueOfGold", string.IsNullOrEmpty(itemDetail["ValueOfGold"].ToString()) ? DBNull.Value : (((object)itemDetail["ValueOfGold"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualPrimaryCollateralDetail.Parameters.AddWithValue("@NameOfGoldTester", ((object)itemDetail["NameOfGoldTester"].ToString()) ?? DBNull.Value);
                                                            insertedMainIdFromPrimaryCollateralDetail = (int)comForIndividualPrimaryCollateralDetail.ExecuteScalar();
                                                            comForIndividualPrimaryCollateralDetail.Parameters.Clear();
                                                            #endregion
                                                        }
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #endregion

                                }
                                #endregion

                                #region Insert into details of Secondary Collaterals
                                if (!string.IsNullOrEmpty(npaModel.StringifiedListOfSecondaryCollateralDetailObjects) && npaModel.StringifiedListOfSecondaryCollateralDetailObjects != "[]" && npaModel.StringifiedListOfSecondaryCollateralDetailObjects != "{}")
                                {

                                    #region Insert cases for Secondary Collateral Details

                                    #region Parsing the stringifiedListOfSecondaryCollateralDetailObjects and operating on it

                                    string strListOfSecondaryCollaterals = $"{{\"secondaryCollaterals\":{npaModel.StringifiedListOfSecondaryCollateralDetailObjects}}}";
                                    JObject joListOfSecondaryCollaterals = JObject.Parse(strListOfSecondaryCollaterals);
                                    JArray jaListOfSecondaryCollaterals = (JArray)joListOfSecondaryCollaterals["secondaryCollaterals"];
                                    int insertedMainIdFromSecondaryCollateralMain = 0;
                                    int insertedMainIdFromSecondaryCollateralDetail = 0;
                                    foreach (var itemDetail in jaListOfSecondaryCollaterals)
                                    {
                                        if (itemDetail["CollateralTypeId"].ToString() != "")
                                        {
                                            insertedMainIdFromSecondaryCollateralDetail = 0;
                                            #region Insert to Collateral Main table Secondary Collaterals
                                            comForSecondaryCollateralMain.CommandType = CommandType.Text;
                                            comForSecondaryCollateralMain.Connection = db;
                                            comForSecondaryCollateralMain.Transaction = tran;
                                            comForSecondaryCollateralMain.CommandText = @"INSERT INTO [dbo].[CollateralDetailMain]
                                                                                           ([NPAManagementId]
                                                                                           ,[CollateralTypeId]
                                                                                           ,[CollateralClass]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                            VALUES
                                                                                           (@NPAManagementId,
                                                                                            @CollateralTypeId,
                                                                                            @CollateralClass)";
                                            comForSecondaryCollateralMain.Parameters.AddWithValue("@NPAManagementId", npaModel.NPAManagementId);
                                            comForSecondaryCollateralMain.Parameters.AddWithValue("@CollateralTypeId", ((object)itemDetail["CollateralTypeId"].ToString()) ?? DBNull.Value);
                                            comForSecondaryCollateralMain.Parameters.AddWithValue("@CollateralClass", "2");
                                            insertedMainIdFromSecondaryCollateralMain = (int)comForSecondaryCollateralMain.ExecuteScalar();
                                            comForSecondaryCollateralMain.Parameters.Clear();
                                            #endregion
                                            if (insertedMainIdFromSecondaryCollateralMain > 0)
                                            {
                                                dynamic individualSecondaryCollateral = null;
                                                switch (itemDetail["CollateralTypeId"].ToString())
                                                {
                                                    case "1":
                                                        individualSecondaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfLand>();
                                                        if (individualSecondaryCollateral != null)
                                                        {
                                                            #region Inserting into Secondary Collateral table(Collateral Land)
                                                            comForIndividualSecondaryCollateralDetail.CommandType = CommandType.Text;
                                                            comForIndividualSecondaryCollateralDetail.Connection = db;
                                                            comForIndividualSecondaryCollateralDetail.Transaction = tran;
                                                            comForIndividualSecondaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfLand]
                                                                                                                       ([CollateralDetailMainId]
                                                                                                                       ,[ProvinceId]
                                                                                                                       ,[ZoneId]
                                                                                                                       ,[DistrictId]
                                                                                                                       ,[VDCMun]
                                                                                                                       ,[Street]
                                                                                                                       ,[WardNumber]
                                                                                                                       ,[PlotNumber]
                                                                                                                       ,[AreaTypeId]
                                                                                                                       ,[Area]
                                                                                                                       ,[PropertyOwner]
                                                                                                                       ,[RelationWithBorrower]
                                                                                                                       ,[FirstValuatorName]
                                                                                                                       ,[FirstValuationDate]
                                                                                                                       ,[FirstFMVOfProperty]
                                                                                                                       ,[FirstValuatorExistsInSBLCurrentlyYN]
                                                                                                                       ,[LatestValuatorName]
                                                                                                                       ,[LatestValuationDate]
                                                                                                                       ,[LatestValuatorExistsInSBLCurrentlyYN]
                                                                                                                       ,[FMVOfPropertyAsPerLatestValuationReport]
                                                                                                                       ,[InsuranceCoverageType]
                                                                                                                       ,[InsuranceExpiryDate]
                                                                                                                       ,[InsuranceAmount]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                 VALUES
                                                                                                                       (@CollateralDetailMainId,
                                                                                                                        @ProvinceId,
                                                                                                                        @ZoneId,
                                                                                                                        @DistrictId,
                                                                                                                        @VDCMun,
                                                                                                                        @Street,
                                                                                                                        @WardNumber,
                                                                                                                        @PlotNumber,
                                                                                                                        @AreaTypeId,
                                                                                                                        @Area,
                                                                                                                        @PropertyOwner,
                                                                                                                        @RelationWithBorrower,
                                                                                                                        @FirstValuatorName,
                                                                                                                        @FirstValuationDate,
                                                                                                                        @FirstFMVOfProperty,
                                                                                                                        @FirstValuatorExistsInSBLCurrentlyYN,
                                                                                                                        @LatestValuatorName,
                                                                                                                        @LatestValuationDate,
                                                                                                                        @LatestValuatorExistsInSBLCurrentlyYN,
                                                                                                                        @FMVOfPropertyAsPerLatestValuationReport,
                                                                                                                        @InsuranceCoverageType,
                                                                                                                        @InsuranceExpiryDate,
                                                                                                                        @InsuranceAmount)";
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromSecondaryCollateralMain);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ProvinceId", ((object)itemDetail["ProvinceId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ZoneId", ((object)itemDetail["ZoneId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@DistrictId", ((object)itemDetail["DistrictId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@VDCMun", ((object)itemDetail["VDCMun"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@Street", ((object)itemDetail["Street"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@WardNumber", ((object)itemDetail["WardNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@PlotNumber", ((object)itemDetail["PlotNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@AreaTypeId", !string.IsNullOrEmpty(itemDetail["AreaTypeId"].ToString()) ? (object)itemDetail["AreaTypeId"].ToString() : DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@Area", ((object)itemDetail["Area"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@PropertyOwner", ((object)itemDetail["PropertyOwner"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@RelationWithBorrower", ((object)itemDetail["RelationWithBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorName", ((object)itemDetail["FirstValuatorName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuationDate", itemDetail["FirstValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["FirstValuationDate"].ToString()));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstFMVOfProperty", string.IsNullOrEmpty(itemDetail["FirstFMVOfProperty"].ToString()) ? DBNull.Value : (((object)itemDetail["FirstFMVOfProperty"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorName", ((object)itemDetail["LatestValuatorName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuationDate", itemDetail["LatestValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["LatestValuationDate"].ToString()));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FMVOfPropertyAsPerLatestValuationReport", string.IsNullOrEmpty(itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString()) ? DBNull.Value : (((object)itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceCoverageType", ((object)itemDetail["InsuranceCoverageType"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceExpiryDate", itemDetail["InsuranceExpiryDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["InsuranceExpiryDate"].ToString()));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceAmount", string.IsNullOrEmpty(itemDetail["InsuranceAmount"].ToString()) ? DBNull.Value : (((object)itemDetail["InsuranceAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            insertedMainIdFromSecondaryCollateralDetail = (int)comForIndividualSecondaryCollateralDetail.ExecuteScalar();
                                                            comForIndividualSecondaryCollateralDetail.Parameters.Clear();
                                                            #endregion
                                                        }
                                                        break;
                                                    case "2":
                                                        individualSecondaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfBuilding>();
                                                        if (individualSecondaryCollateral != null)
                                                        {
                                                            #region Inserting into Secondary Collateral table(Collateral Land and Building)
                                                            comForIndividualSecondaryCollateralDetail.CommandType = CommandType.Text;
                                                            comForIndividualSecondaryCollateralDetail.Connection = db;
                                                            comForIndividualSecondaryCollateralDetail.Transaction = tran;
                                                            comForIndividualSecondaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfBuilding]
                                                                                                                        ([CollateralDetailMainId]
                                                                                                                        ,[ProvinceId]
                                                                                                                        ,[ZoneId]
                                                                                                                        ,[DistrictId]
                                                                                                                        ,[VDCMun]
                                                                                                                        ,[Street]
                                                                                                                        ,[WardNumber]
                                                                                                                        ,[PlotNumber]
                                                                                                                        ,[AreaTypeId]
                                                                                                                        ,[AreaOfLand]
                                                                                                                        ,[AreaOfBuilding]
                                                                                                                        ,[PropertyOwner]
                                                                                                                        ,[RelationWithBorrower]
                                                                                                                        ,[ConstructionCompletionDate]
                                                                                                                        ,[FirstValuatorName]
                                                                                                                        ,[FirstValuationDate]
                                                                                                                        ,[FirstFMVOfProperty]
                                                                                                                        ,[FirstValuatorExistsInSBLCurrentlyYN]
                                                                                                                        ,[LatestValuatorName]
                                                                                                                        ,[LatestValuationDate]
                                                                                                                        ,[LatestValuatorExistsInSBLCurrentlyYN]
                                                                                                                        ,[FMVOfPropertyAsPerLatestValuationReport]
                                                                                                                        ,[InsuranceCoverageType]
                                                                                                                        ,[InsuranceExpiryDate]
                                                                                                                        ,[InsuranceAmount]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                    VALUES
                                                                                                                        (@CollateralDetailMainId,
                                                                                                                         @ProvinceId,
                                                                                                                         @ZoneId,
                                                                                                                         @DistrictId,
                                                                                                                         @VDCMun,
                                                                                                                         @Street,
                                                                                                                         @WardNumber,
                                                                                                                         @PlotNumber,
                                                                                                                         @AreaTypeId,
                                                                                                                         @AreaOfLand,
                                                                                                                         @AreaOfBuilding,
                                                                                                                         @PropertyOwner,
                                                                                                                         @RelationWithBorrower,
                                                                                                                         @ConstructionCompletionDate,
                                                                                                                         @FirstValuatorName,
                                                                                                                         @FirstValuationDate,
                                                                                                                         @FirstFMVOfProperty,
                                                                                                                         @FirstValuatorExistsInSBLCurrentlyYN,
                                                                                                                         @LatestValuatorName,
                                                                                                                         @LatestValuationDate,
                                                                                                                         @LatestValuatorExistsInSBLCurrentlyYN,
                                                                                                                         @FMVOfPropertyAsPerLatestValuationReport,
                                                                                                                         @InsuranceCoverageType,
                                                                                                                         @InsuranceExpiryDate,
                                                                                                                         @InsuranceAmount)";
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromSecondaryCollateralMain);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ProvinceId", ((object)itemDetail["ProvinceId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ZoneId", ((object)itemDetail["ZoneId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@DistrictId", ((object)itemDetail["DistrictId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@VDCMun", ((object)itemDetail["VDCMun"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@Street", ((object)itemDetail["Street"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@WardNumber", ((object)itemDetail["WardNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@PlotNumber", ((object)itemDetail["PlotNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@AreaTypeId", !string.IsNullOrEmpty(itemDetail["AreaTypeId"].ToString()) ? (object)itemDetail["AreaTypeId"].ToString() : DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@AreaOfLand", ((object)itemDetail["AreaOfLand"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@AreaOfBuilding", ((object)itemDetail["AreaOfBuilding"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@PropertyOwner", ((object)itemDetail["PropertyOwner"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@RelationWithBorrower", ((object)itemDetail["RelationWithBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ConstructionCompletionDate", itemDetail["ConstructionCompletionDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["ConstructionCompletionDate"].ToString()));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorName", ((object)itemDetail["FirstValuatorName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuationDate", itemDetail["FirstValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["FirstValuationDate"].ToString()));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstFMVOfProperty", string.IsNullOrEmpty(itemDetail["FirstFMVOfProperty"].ToString()) ? DBNull.Value : (((object)itemDetail["FirstFMVOfProperty"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorName", ((object)itemDetail["LatestValuatorName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuationDate", itemDetail["LatestValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["LatestValuationDate"].ToString()));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FMVOfPropertyAsPerLatestValuationReport", string.IsNullOrEmpty(itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString()) ? DBNull.Value : (((object)itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceCoverageType", ((object)itemDetail["InsuranceCoverageType"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceExpiryDate", itemDetail["InsuranceExpiryDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["InsuranceExpiryDate"].ToString()));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceAmount", string.IsNullOrEmpty(itemDetail["InsuranceAmount"].ToString()) ? DBNull.Value : (((object)itemDetail["InsuranceAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            insertedMainIdFromSecondaryCollateralDetail = (int)comForIndividualSecondaryCollateralDetail.ExecuteScalar();
                                                            comForIndividualSecondaryCollateralDetail.Parameters.Clear();
                                                            #endregion
                                                        }
                                                        break;
                                                    case "3":
                                                        individualSecondaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfVehicle>();
                                                        if (individualSecondaryCollateral != null)
                                                        {
                                                            #region Inserting into Secondary Collateral table(Collateral Vehicle)
                                                            comForIndividualSecondaryCollateralDetail.CommandType = CommandType.Text;
                                                            comForIndividualSecondaryCollateralDetail.Connection = db;
                                                            comForIndividualSecondaryCollateralDetail.Transaction = tran;
                                                            comForIndividualSecondaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfVehicle]
                                                                                                                       ([CollateralDetailMainId]
                                                                                                                       ,[VehicleTypeId]
                                                                                                                       ,[VehicleModel]
                                                                                                                       ,[EngineNumber]
                                                                                                                       ,[ChassisNumber]
                                                                                                                       ,[VehicleRegistrationNumber]
                                                                                                                       ,[VehicleRegistrationDate]
                                                                                                                       ,[VehicleRegisteredOffice]
                                                                                                                       ,[VehicleMakeYear]
                                                                                                                       ,[FirstValuatorName]
                                                                                                                       ,[FirstValuationDate]
                                                                                                                       ,[FirstFMVOfProperty]
                                                                                                                       ,[FirstValuatorExistsInSBLCurrentlyYN]
                                                                                                                       ,[LatestValuatorName]
                                                                                                                       ,[LatestValuationDate]
                                                                                                                       ,[LatestValuatorExistsInSBLCurrentlyYN]
                                                                                                                       ,[ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice]
                                                                                                                       ,[InsuranceCoverageType]
                                                                                                                       ,[InsuranceExpiryDate]
                                                                                                                       ,[InsuranceAmount]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                 VALUES
                                                                                                                       (@CollateralDetailMainId,
                                                                                                                        @VehicleTypeId,
                                                                                                                        @VehicleModel,
                                                                                                                        @EngineNumber,
                                                                                                                        @ChassisNumber,
                                                                                                                        @VehicleRegistrationNumber,
                                                                                                                        @VehicleRegistrationDate,
                                                                                                                        @VehicleRegisteredOffice,
                                                                                                                        @VehicleMakeYear,
                                                                                                                        @FirstValuatorName,
                                                                                                                        @FirstValuationDate,
                                                                                                                        @FirstFMVOfProperty,
                                                                                                                        @FirstValuatorExistsInSBLCurrentlyYN,
                                                                                                                        @LatestValuatorName,
                                                                                                                        @LatestValuationDate,
                                                                                                                        @LatestValuatorExistsInSBLCurrentlyYN,
                                                                                                                        @ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice,
                                                                                                                        @InsuranceCoverageType,
                                                                                                                        @InsuranceExpiryDate,
                                                                                                                        @InsuranceAmount)";
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromSecondaryCollateralMain);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@VehicleTypeId", !string.IsNullOrEmpty(itemDetail["VehicleTypeId"].ToString()) ? (object)itemDetail["VehicleTypeId"].ToString() : DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@VehicleModel", ((object)itemDetail["VehicleModel"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@EngineNumber", ((object)itemDetail["EngineNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ChassisNumber", ((object)itemDetail["ChassisNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@VehicleRegistrationNumber", ((object)itemDetail["VehicleRegistrationNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@VehicleRegistrationDate", itemDetail["VehicleRegistrationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["VehicleRegistrationDate"].ToString()));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@VehicleRegisteredOffice", ((object)itemDetail["VehicleRegisteredOffice"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@VehicleMakeYear", ((object)itemDetail["VehicleMakeYear"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorName", ((object)itemDetail["FirstValuatorName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuationDate", itemDetail["FirstValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["FirstValuationDate"].ToString()));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstFMVOfProperty", string.IsNullOrEmpty(itemDetail["FirstFMVOfProperty"].ToString()) ? DBNull.Value : (((object)itemDetail["FirstFMVOfProperty"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorName", ((object)itemDetail["LatestValuatorName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuationDate", itemDetail["LatestValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["LatestValuationDate"].ToString()));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice", string.IsNullOrEmpty(itemDetail["ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice"].ToString()) ? DBNull.Value : (((object)itemDetail["ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceCoverageType", ((object)itemDetail["InsuranceCoverageType"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceExpiryDate", itemDetail["InsuranceExpiryDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["InsuranceExpiryDate"].ToString()));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceAmount", string.IsNullOrEmpty(itemDetail["InsuranceAmount"].ToString()) ? DBNull.Value : (((object)itemDetail["InsuranceAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            insertedMainIdFromSecondaryCollateralDetail = (int)comForIndividualSecondaryCollateralDetail.ExecuteScalar();
                                                            comForIndividualSecondaryCollateralDetail.Parameters.Clear();
                                                            #endregion
                                                        }
                                                        break;
                                                    case "4":
                                                        individualSecondaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfPlantAndMachinery>();
                                                        if (individualSecondaryCollateral != null)
                                                        {
                                                            #region Inserting into Secondary Collateral table(Collateral Plant and Machinery)
                                                            comForIndividualSecondaryCollateralDetail.CommandType = CommandType.Text;
                                                            comForIndividualSecondaryCollateralDetail.Connection = db;
                                                            comForIndividualSecondaryCollateralDetail.Transaction = tran;
                                                            comForIndividualSecondaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfPlantAndMachinery]
                                                                                                                        ([CollateralDetailMainId]
                                                                                                                        ,[PlantAndMachineryModel]
                                                                                                                        ,[EngineNumber]
                                                                                                                        ,[ChassisNumber]
                                                                                                                        ,[RegistrationNumber]
                                                                                                                        ,[FirstValuatorName]
                                                                                                                        ,[FirstValuationDate]
                                                                                                                        ,[FirstFMVOfProperty]
                                                                                                                        ,[FirstValuatorExistsInSBLCurrentlyYN]
                                                                                                                        ,[LatestValuatorName]
                                                                                                                        ,[LatestValuationDate]
                                                                                                                        ,[LatestValuatorExistsInSBLCurrentlyYN]
                                                                                                                        ,[FMVOfPropertyAsPerLatestValuationReport]
                                                                                                                        ,[InsuranceCoverageType]
                                                                                                                        ,[InsuranceExpiryDate]
                                                                                                                        ,[InsuranceAmount]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                    VALUES
                                                                                                                        (@CollateralDetailMainId,
                                                                                                                         @PlantAndMachineryModel,
                                                                                                                         @EngineNumber,
                                                                                                                         @ChassisNumber,
                                                                                                                         @RegistrationNumber,
                                                                                                                         @FirstValuatorName,
                                                                                                                         @FirstValuationDate,
                                                                                                                         @FirstFMVOfProperty,
                                                                                                                         @FirstValuatorExistsInSBLCurrentlyYN,
                                                                                                                         @LatestValuatorName,
                                                                                                                         @LatestValuationDate,
                                                                                                                         @LatestValuatorExistsInSBLCurrentlyYN,
                                                                                                                         @FMVOfPropertyAsPerLatestValuationReport,
                                                                                                                         @InsuranceCoverageType,
                                                                                                                         @InsuranceExpiryDate,
                                                                                                                         @InsuranceAmount)";
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromSecondaryCollateralMain);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@PlantAndMachineryModel", ((object)itemDetail["PlantAndMachineryModel"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@EngineNumber", ((object)itemDetail["EngineNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ChassisNumber", ((object)itemDetail["ChassisNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@RegistrationNumber", ((object)itemDetail["RegistrationNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorName", ((object)itemDetail["FirstValuatorName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuationDate", itemDetail["FirstValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["FirstValuationDate"].ToString()));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstFMVOfProperty", string.IsNullOrEmpty(itemDetail["FirstFMVOfProperty"].ToString()) ? DBNull.Value : (((object)itemDetail["FirstFMVOfProperty"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirstValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["FirstValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorName", ((object)itemDetail["LatestValuatorName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuationDate", itemDetail["LatestValuationDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["LatestValuationDate"].ToString()));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FMVOfPropertyAsPerLatestValuationReport", string.IsNullOrEmpty(itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString()) ? DBNull.Value : (((object)itemDetail["FMVOfPropertyAsPerLatestValuationReport"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@LatestValuatorExistsInSBLCurrentlyYN", !string.IsNullOrEmpty(itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString()) ? (object)itemDetail["LatestValuatorExistsInSBLCurrentlyYN"].ToString() : "0");
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceCoverageType", ((object)itemDetail["InsuranceCoverageType"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceExpiryDate", itemDetail["InsuranceExpiryDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["InsuranceExpiryDate"].ToString()));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceAmount", string.IsNullOrEmpty(itemDetail["InsuranceAmount"].ToString()) ? DBNull.Value : (((object)itemDetail["InsuranceAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            insertedMainIdFromSecondaryCollateralDetail = (int)comForIndividualSecondaryCollateralDetail.ExecuteScalar();
                                                            comForIndividualSecondaryCollateralDetail.Parameters.Clear();
                                                            #endregion
                                                        }
                                                        break;
                                                    case "5":
                                                        individualSecondaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfCurrentAsset>();
                                                        if (individualSecondaryCollateral != null)
                                                        {
                                                            #region Inserting into Secondary Collateral table(Collateral Current Asset)
                                                            comForIndividualSecondaryCollateralDetail.CommandType = CommandType.Text;
                                                            comForIndividualSecondaryCollateralDetail.Connection = db;
                                                            comForIndividualSecondaryCollateralDetail.Transaction = tran;
                                                            comForIndividualSecondaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfCurrentAsset]
                                                                                                                        ([CollateralDetailMainId]
                                                                                                                       ,[DateOfLatestStockInspectionReportCollectedByBranch]
                                                                                                                       ,[WorkingCapitalAsPerStockReport]
                                                                                                                       ,[WorkingCapitalAsPerStockInspector]
                                                                                                                       ,[DrawingPower]
                                                                                                                       ,[InsuranceCoverageType]
                                                                                                                       ,[InsuranceExpiryDate]
                                                                                                                       ,[InsuranceAmount]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                 VALUES
                                                                                                                       (@CollateralDetailMainId,
                                                                                                                        @DateOfLatestStockInspectionReportCollectedByBranch,
                                                                                                                        @WorkingCapitalAsPerStockReport,
                                                                                                                        @WorkingCapitalAsPerStockInspector,
                                                                                                                        @DrawingPower,
                                                                                                                        @InsuranceCoverageType,
                                                                                                                        @InsuranceExpiryDate,
                                                                                                                        @InsuranceAmount)";
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromSecondaryCollateralMain);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@DateOfLatestStockInspectionReportCollectedByBranch", itemDetail["DateOfLatestStockInspectionReportCollectedByBranch"].ToString() == "" ? DBNull.Value : ((object)itemDetail["DateOfLatestStockInspectionReportCollectedByBranch"].ToString()));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@WorkingCapitalAsPerStockReport", string.IsNullOrEmpty(itemDetail["WorkingCapitalAsPerStockReport"].ToString()) ? DBNull.Value : (((object)itemDetail["WorkingCapitalAsPerStockReport"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@WorkingCapitalAsPerStockInspector", string.IsNullOrEmpty(itemDetail["WorkingCapitalAsPerStockInspector"].ToString()) ? DBNull.Value : (((object)itemDetail["WorkingCapitalAsPerStockInspector"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@DrawingPower", ((object)itemDetail["DrawingPower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceCoverageType", ((object)itemDetail["InsuranceCoverageType"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceExpiryDate", itemDetail["InsuranceExpiryDate"].ToString() == "" ? DBNull.Value : ((object)itemDetail["InsuranceExpiryDate"].ToString()));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@InsuranceAmount", string.IsNullOrEmpty(itemDetail["InsuranceAmount"].ToString()) ? DBNull.Value : (((object)itemDetail["InsuranceAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            insertedMainIdFromSecondaryCollateralDetail = (int)comForIndividualSecondaryCollateralDetail.ExecuteScalar();
                                                            comForIndividualSecondaryCollateralDetail.Parameters.Clear();
                                                            #endregion
                                                        }
                                                        break;
                                                    case "6":
                                                        individualSecondaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfPersonalGuarantee>();
                                                        if (individualSecondaryCollateral != null)
                                                        {
                                                            #region Inserting into Secondary Collateral table(Collateral Personal Guarantee)
                                                            comForIndividualSecondaryCollateralDetail.CommandType = CommandType.Text;
                                                            comForIndividualSecondaryCollateralDetail.Connection = db;
                                                            comForIndividualSecondaryCollateralDetail.Transaction = tran;
                                                            comForIndividualSecondaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfPersonalGuarantee]
                                                                                                                       ([CollateralDetailMainId]
                                                                                                                       ,[NameOfGuarantor]
                                                                                                                       ,[CitizenshipNumber]
                                                                                                                       ,[FatherName]
                                                                                                                       ,[GrandfatherName]
                                                                                                                       ,[PermanentAddress]
                                                                                                                       ,[CurrentAddress]
                                                                                                                       ,[NetworthOfGuarantor]
                                                                                                                       ,[RelationWithBorrower]
                                                                                                                       ,[ContactNumber]
                                                                                                                       ,[Profession]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                 VALUES
                                                                                                                       (@CollateralDetailMainId,
                                                                                                                        @NameOfGuarantor,
                                                                                                                        @CitizenshipNumber,
                                                                                                                        @FatherName,
                                                                                                                        @GrandfatherName,
                                                                                                                        @PermanentAddress,
                                                                                                                        @CurrentAddress,
                                                                                                                        @NetworthOfGuarantor,
                                                                                                                        @RelationWithBorrower,
                                                                                                                        @ContactNumber,
                                                                                                                        @Profession)";
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromSecondaryCollateralMain);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@NameOfGuarantor", ((object)itemDetail["NameOfGuarantor"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CitizenshipNumber", ((object)itemDetail["CitizenshipNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FatherName", ((object)itemDetail["FatherName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@GrandfatherName", ((object)itemDetail["GrandfatherName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@PermanentAddress", ((object)itemDetail["PermanentAddress"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CurrentAddress", ((object)itemDetail["CurrentAddress"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@NetworthOfGuarantor", string.IsNullOrEmpty(itemDetail["NetworthOfGuarantor"].ToString()) ? DBNull.Value : (((object)itemDetail["NetworthOfGuarantor"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@RelationWithBorrower", ((object)itemDetail["RelationWithBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ContactNumber", ((object)itemDetail["ContactNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@Profession", ((object)itemDetail["Profession"].ToString()) ?? DBNull.Value);
                                                            insertedMainIdFromSecondaryCollateralDetail = (int)comForIndividualSecondaryCollateralDetail.ExecuteScalar();
                                                            comForIndividualSecondaryCollateralDetail.Parameters.Clear();
                                                            #endregion
                                                        }
                                                        break;
                                                    case "7":
                                                        individualSecondaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfCorporateGuarantee>();
                                                        if (individualSecondaryCollateral != null)
                                                        {
                                                            #region Inserting into Secondary Collateral table(Collateral Corporate Guarantee)
                                                            comForIndividualSecondaryCollateralDetail.CommandType = CommandType.Text;
                                                            comForIndividualSecondaryCollateralDetail.Connection = db;
                                                            comForIndividualSecondaryCollateralDetail.Transaction = tran;
                                                            comForIndividualSecondaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfCorporateGuarantee]
                                                                                                                       ([CollateralDetailMainId]
                                                                                                                       ,[NameOfFirm]
                                                                                                                       ,[RegistrationNumber]
                                                                                                                       ,[FirmAddress]
                                                                                                                       ,[NetworthOfTheFirm]
                                                                                                                       ,[ContactPerson]
                                                                                                                       ,[ContactNumber]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                 VALUES
                                                                                                                       (@CollateralDetailMainId,
                                                                                                                        @NameOfFirm,
                                                                                                                        @RegistrationNumber,
                                                                                                                        @FirmAddress,
                                                                                                                        @NetworthOfTheFirm,
                                                                                                                        @ContactPerson,
                                                                                                                        @ContactNumber)";
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromSecondaryCollateralMain);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@NameOfFirm", ((object)itemDetail["NameOfFirm"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@RegistrationNumber", ((object)itemDetail["RegistrationNumber"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@FirmAddress", ((object)itemDetail["FirmAddress"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@NetworthOfTheFirm", !string.IsNullOrEmpty(itemDetail["NetworthOfTheFirm"].ToString()) ? (object)itemDetail["NetworthOfTheFirm"].ToString() : DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ContactPerson", ((object)itemDetail["ContactPerson"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ContactNumber", ((object)itemDetail["ContactNumber"].ToString()) ?? DBNull.Value);
                                                            insertedMainIdFromSecondaryCollateralDetail = (int)comForIndividualSecondaryCollateralDetail.ExecuteScalar();
                                                            comForIndividualSecondaryCollateralDetail.Parameters.Clear();
                                                            #endregion
                                                        }
                                                        break;
                                                    case "8":
                                                        individualSecondaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfStock>();
                                                        if (individualSecondaryCollateral != null)
                                                        {
                                                            #region Inserting into Secondary Collateral table(Collateral Stock)
                                                            comForIndividualSecondaryCollateralDetail.CommandType = CommandType.Text;
                                                            comForIndividualSecondaryCollateralDetail.Connection = db;
                                                            comForIndividualSecondaryCollateralDetail.Transaction = tran;
                                                            comForIndividualSecondaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfStock]
                                                                                                                                       ([CollateralDetailMainId]
                                                                                                                                       ,[ShareTypeId]
                                                                                                                                       ,[CompanyName]
                                                                                                                                       ,[ListedInNepseBoolId]
                                                                                                                                       ,[PledgedUnits]
                                                                                                                                       ,[UnitTypeId]
                                                                                                                                       ,[ValueOfShare]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                                 VALUES
                                                                                                                                       (@CollateralDetailMainId,
			                                                                                                                            @ShareTypeId,
			                                                                                                                            @CompanyName,
			                                                                                                                            @ListedInNepseBoolId,
			                                                                                                                            @PledgedUnits,
			                                                                                                                            @UnitTypeId,
			                                                                                                                            @ValueOfShare)";
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromSecondaryCollateralMain);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ShareTypeId", ((object)itemDetail["ShareTypeId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CompanyName", ((object)itemDetail["CompanyName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ListedInNepseBoolId", ((object)itemDetail["ListedInNepseBoolId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@PledgedUnits", string.IsNullOrEmpty(itemDetail["PledgedUnits"].ToString()) ? DBNull.Value : (((object)itemDetail["PledgedUnits"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@UnitTypeId", ((object)itemDetail["ShareUnitTypeId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ValueOfShare", string.IsNullOrEmpty(itemDetail["ValueOfShare"].ToString()) ? DBNull.Value : (((object)itemDetail["ValueOfShare"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            insertedMainIdFromSecondaryCollateralDetail = (int)comForIndividualSecondaryCollateralDetail.ExecuteScalar();
                                                            comForIndividualSecondaryCollateralDetail.Parameters.Clear();
                                                            #endregion
                                                        }
                                                        break;
                                                    case "9":
                                                        individualSecondaryCollateral = itemDetail.ToObject<CollateralDetailInCaseOfGold>();
                                                        if (individualSecondaryCollateral != null)
                                                        {
                                                            #region Inserting into Secondary Collateral table(Collateral Gold)
                                                            comForIndividualSecondaryCollateralDetail.CommandType = CommandType.Text;
                                                            comForIndividualSecondaryCollateralDetail.Connection = db;
                                                            comForIndividualSecondaryCollateralDetail.Transaction = tran;
                                                            comForIndividualSecondaryCollateralDetail.CommandText = @"INSERT INTO [dbo].[CollateralDetailInCaseOfGold]
                                                                                                                                   ([CollateralDetailMainId]
                                                                                                                                   ,[Quantity]
                                                                                                                                   ,[MeasurementUnitId]
                                                                                                                                   ,[GoldTypeId]
                                                                                                                                   ,[ValueOfGold]
                                                                                                                                   ,[NameOfGoldTester]) OUTPUT INSERTED.CollateralDetailMainId
                                                                                                                             VALUES
                                                                                                                                   (@CollateralDetailMainId,
                                                                                                                                    @Quantity,
                                                                                                                                    @MeasurementUnitId,
                                                                                                                                    @GoldTypeId,
                                                                                                                                    @ValueOfGold,
                                                                                                                                    @NameOfGoldTester)";
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@CollateralDetailMainId", insertedMainIdFromSecondaryCollateralMain);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@Quantity", ((object)itemDetail["Quantity"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@MeasurementUnitId", ((object)itemDetail["MeasurementUnitId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@GoldTypeId", ((object)itemDetail["GoldTypeId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@ValueOfGold", string.IsNullOrEmpty(itemDetail["ValueOfGold"].ToString()) ? DBNull.Value : (((object)itemDetail["ValueOfGold"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            comForIndividualSecondaryCollateralDetail.Parameters.AddWithValue("@NameOfGoldTester", ((object)itemDetail["NameOfGoldTester"].ToString()) ?? DBNull.Value);
                                                            insertedMainIdFromSecondaryCollateralDetail = (int)comForIndividualSecondaryCollateralDetail.ExecuteScalar();
                                                            comForIndividualSecondaryCollateralDetail.Parameters.Clear();
                                                            #endregion
                                                        }
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #endregion

                                }
                                #endregion

                                #region Insert start for Loan Details
                                if (!string.IsNullOrEmpty(npaModel.StringifiedListOfLoanAccountDetailObjects) && npaModel.StringifiedListOfLoanAccountDetailObjects != "[]" && npaModel.StringifiedListOfLoanAccountDetailObjects != "{}")
                                {
                                    string strListOfLoanDetails = $"{{\"loanDetails\":{npaModel.StringifiedListOfLoanAccountDetailObjects}}}";
                                    JObject joListOfLoanDetails = JObject.Parse(strListOfLoanDetails);
                                    JArray jaListOfLoanDetails = (JArray)joListOfLoanDetails["loanDetails"];
                                    int insertedLoanDetailMainId = 0;
                                    //
                                    foreach (var item in jaListOfLoanDetails)
                                    {
                                        #region Insert to Loan Detail Main table
                                        comForLoanDetailMain.CommandType = CommandType.Text;
                                        comForLoanDetailMain.Connection = db;
                                        comForLoanDetailMain.Transaction = tran;
                                        comForLoanDetailMain.CommandText = @"INSERT INTO [dbo].[LoanAccountDetail]
                                                                        ([NPAManagementId]
                                                                        ,[LoanAccountNo]
                                                                        ,[ProductCode]
                                                                        ,[LoanTypeId]
                                                                        ,[LoanNatureId]
                                                                        ,[FirstLimitInitiatedBy]
                                                                        ,[FirstRecommendedBy]
                                                                        ,[FirstApprovedBy]
                                                                        ,[DepartmentId]
                                                                        ,[BranchProvince]
                                                                        ,[ReportingSBO]
                                                                        ,[NomineeAccountNo]
                                                                        ,[InitialApprovedLimit]
                                                                        ,[FirstLimitPlacementDate]
                                                                        ,[LatestApprovedLimit]
                                                                        ,[LimitExpiryDate]
                                                                        ,[LoanCurrentStatusId]
                                                                        ,[SettlementDate]
                                                                        ,[LoanStatusId]
                                                                        ,[OutStDate]
                                                                        ,[OutStPrincipal]
                                                                        ,[OutStInterest]
                                                                        ,[OutStAdhocCharges]
                                                                        ,[OutStInterestOnInterest]
                                                                        ,[OutStPenalCharges]
                                                                        ,[OutStTotalAmount]
                                                                        ,[AccruedAmtFromDate]
                                                                        ,[AccruedAmtToDate]
                                                                        ,[AccruedAmtPrincipal]
                                                                        ,[AccruedAmtInterest]
                                                                        ,[AccruedAmtAdhocCharges]
                                                                        ,[AccruedAmtInterestOnInterest]
                                                                        ,[AccruedAmtPenalCharges]
                                                                        ,[AccruedAmtTotalAmount]
                                                                        ,[RecoveredAmtFromDate]
                                                                        ,[RecoveredAmtToDate]
                                                                        ,[RecoveredAmtPrincipal]
                                                                        ,[RecoveredAmtInterest]
                                                                        ,[RecoveredAmtAdhocCharges]
                                                                        ,[RecoveredAmtInterestOnInterest]
                                                                        ,[RecoveredAmtPenalCharges]
                                                                        ,[RecoveredAmtTotalAmount]
                                                                        ,[ProvisionPercentageId]
                                                                        ,[ProvisionDate]
                                                                        ,[ProvisionAmount]) OUTPUT INSERTED.LoanMainId
                                                                    VALUES
                                                                        (@NPAManagementId,
                                                                         @LoanAccountNo,
                                                                         @ProductCode,
                                                                         @LoanTypeId,
                                                                         @LoanNatureId,
                                                                         @FirstLimitInitiatedBy,
                                                                         @FirstRecommendedBy,
                                                                         @FirstApprovedBy,
                                                                         @DepartmentId,
                                                                         @BranchProvince,
                                                                         @ReportingSBO,
                                                                         @NomineeAccountNo,
                                                                         @InitialApprovedLimit,
                                                                         @FirstLimitPlacementDate,
                                                                         @LatestApprovedLimit,
                                                                         @LimitExpiryDate,
                                                                         @LoanCurrentStatusId,
                                                                         @SettlementDate,
                                                                         @LoanStatusId,
                                                                         @OutStDate,
                                                                         @OutStPrincipal,
                                                                         @OutStInterest,
                                                                         @OutStAdhocCharges,
                                                                         @OutStInterestOnInterest,
                                                                         @OutStPenalCharges,
                                                                         @OutStTotalAmount,
                                                                         @AccruedAmtFromDate,
                                                                         @AccruedAmtToDate,
                                                                         @AccruedAmtPrincipal,
                                                                         @AccruedAmtInterest,
                                                                         @AccruedAmtAdhocCharges,
                                                                         @AccruedAmtInterestOnInterest,
                                                                         @AccruedAmtPenalCharges,
                                                                         @AccruedAmtTotalAmount,
                                                                         @RecoveredAmtFromDate,
                                                                         @RecoveredAmtToDate,
                                                                         @RecoveredAmtPrincipal,
                                                                         @RecoveredAmtInterest,
                                                                         @RecoveredAmtAdhocCharges,
                                                                         @RecoveredAmtInterestOnInterest,
                                                                         @RecoveredAmtPenalCharges,
                                                                         @RecoveredAmtTotalAmount,
                                                                         @ProvisionPercentageId,
                                                                         @ProvisionDate,
                                                                         @ProvisionAmount)";
                                        comForLoanDetailMain.Parameters.AddWithValue("@NPAManagementId", npaModel.NPAManagementId);
                                        comForLoanDetailMain.Parameters.AddWithValue("@LoanAccountNo", ((object)item["LoanAccountNo"].ToString()) ?? DBNull.Value);
                                        comForLoanDetailMain.Parameters.AddWithValue("@ProductCode", ((object)item["ProductCode"].ToString()) ?? DBNull.Value);
                                        comForLoanDetailMain.Parameters.AddWithValue("@LoanTypeId", !string.IsNullOrEmpty(item["LoanTypeId"].ToString()) ? (object)item["LoanTypeId"].ToString() : DBNull.Value);
                                        comForLoanDetailMain.Parameters.AddWithValue("@LoanNatureId", !string.IsNullOrEmpty(item["LoanNatureId"].ToString()) ? (object)item["LoanNatureId"].ToString() : DBNull.Value);
                                        comForLoanDetailMain.Parameters.AddWithValue("@FirstLimitInitiatedBy", ((object)item["FirstLimitInitiatedBy"].ToString()) ?? DBNull.Value);
                                        comForLoanDetailMain.Parameters.AddWithValue("@FirstRecommendedBy", ((object)item["FirstRecommendedBy"].ToString()) ?? DBNull.Value);
                                        comForLoanDetailMain.Parameters.AddWithValue("@FirstApprovedBy", ((object)item["FirstApprovedBy"].ToString()) ?? DBNull.Value);
                                        comForLoanDetailMain.Parameters.AddWithValue("@DepartmentId", !string.IsNullOrEmpty(item["DepartmentId"].ToString()) ? (object)item["DepartmentId"].ToString() : DBNull.Value);
                                        comForLoanDetailMain.Parameters.AddWithValue("@BranchProvince", ((object)item["BranchProvince"].ToString()) ?? DBNull.Value);
                                        comForLoanDetailMain.Parameters.AddWithValue("@ReportingSBO", ((object)item["ReportingSBO"].ToString()) ?? DBNull.Value);
                                        comForLoanDetailMain.Parameters.AddWithValue("@NomineeAccountNo", ((object)item["NomineeAccountNo"].ToString()) ?? DBNull.Value);
                                        comForLoanDetailMain.Parameters.AddWithValue("@InitialApprovedLimit", string.IsNullOrEmpty(item["InitialApprovedLimit"].ToString()) ? DBNull.Value : (((object)item["InitialApprovedLimit"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        comForLoanDetailMain.Parameters.AddWithValue("@FirstLimitPlacementDate", item["FirstLimitPlacementDate"].ToString() == "" ? DBNull.Value : ((object)item["FirstLimitPlacementDate"].ToString()));
                                        comForLoanDetailMain.Parameters.AddWithValue("@LatestApprovedLimit", string.IsNullOrEmpty(item["LatestApprovedLimit"].ToString()) ? DBNull.Value : (((object)item["LatestApprovedLimit"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        comForLoanDetailMain.Parameters.AddWithValue("@LimitExpiryDate", item["LimitExpiryDate"].ToString() == "" ? DBNull.Value : ((object)item["LimitExpiryDate"].ToString()));
                                        comForLoanDetailMain.Parameters.AddWithValue("@LoanCurrentStatusId", !string.IsNullOrEmpty(item["LoanCurrentStatusId"].ToString()) ? (object)item["LoanCurrentStatusId"].ToString() : DBNull.Value);
                                        comForLoanDetailMain.Parameters.AddWithValue("@SettlementDate", item["SettlementDate"].ToString() == "" ? DBNull.Value : ((object)item["SettlementDate"].ToString()));
                                        comForLoanDetailMain.Parameters.AddWithValue("@LoanStatusId", !string.IsNullOrEmpty(item["LoanStatusId"].ToString()) ? (object)item["LoanStatusId"].ToString() : DBNull.Value);
                                        comForLoanDetailMain.Parameters.AddWithValue("@OutStDate", item["OutStDate"].ToString() == "" ? DBNull.Value : ((object)item["OutStDate"].ToString()));
                                        comForLoanDetailMain.Parameters.AddWithValue("@OutStPrincipal", string.IsNullOrEmpty(item["OutStPrincipal"].ToString()) ? DBNull.Value : (((object)item["OutStPrincipal"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        comForLoanDetailMain.Parameters.AddWithValue("@OutStInterest", string.IsNullOrEmpty(item["OutStInterest"].ToString()) ? DBNull.Value : (((object)item["OutStInterest"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        comForLoanDetailMain.Parameters.AddWithValue("@OutStAdhocCharges", string.IsNullOrEmpty(item["OutStAdhocCharges"].ToString()) ? DBNull.Value : (((object)item["OutStAdhocCharges"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        comForLoanDetailMain.Parameters.AddWithValue("@OutStInterestOnInterest", string.IsNullOrEmpty(item["OutStInterestOnInterest"].ToString()) ? DBNull.Value : (((object)item["OutStInterestOnInterest"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        comForLoanDetailMain.Parameters.AddWithValue("@OutStPenalCharges", string.IsNullOrEmpty(item["OutStPenalCharges"].ToString()) ? DBNull.Value : (((object)item["OutStPenalCharges"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        comForLoanDetailMain.Parameters.AddWithValue("@OutStTotalAmount", string.IsNullOrEmpty(item["OutStTotalAmount"].ToString()) ? DBNull.Value : (((object)item["OutStTotalAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        comForLoanDetailMain.Parameters.AddWithValue("@AccruedAmtFromDate", item["AccruedAmtFromDate"].ToString() == "" ? DBNull.Value : ((object)item["AccruedAmtFromDate"].ToString()));
                                        comForLoanDetailMain.Parameters.AddWithValue("@AccruedAmtToDate", item["AccruedAmtToDate"].ToString() == "" ? DBNull.Value : ((object)item["AccruedAmtToDate"].ToString()));
                                        comForLoanDetailMain.Parameters.AddWithValue("@AccruedAmtPrincipal", string.IsNullOrEmpty(item["AccruedAmtPrincipal"].ToString()) ? DBNull.Value : (((object)item["AccruedAmtPrincipal"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        comForLoanDetailMain.Parameters.AddWithValue("@AccruedAmtInterest", string.IsNullOrEmpty(item["AccruedAmtInterest"].ToString()) ? DBNull.Value : (((object)item["AccruedAmtInterest"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        comForLoanDetailMain.Parameters.AddWithValue("@AccruedAmtAdhocCharges", string.IsNullOrEmpty(item["AccruedAmtAdhocCharges"].ToString()) ? DBNull.Value : (((object)item["AccruedAmtAdhocCharges"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        comForLoanDetailMain.Parameters.AddWithValue("@AccruedAmtInterestOnInterest", string.IsNullOrEmpty(item["AccruedAmtInterestOnInterest"].ToString()) ? DBNull.Value : (((object)item["AccruedAmtInterestOnInterest"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        comForLoanDetailMain.Parameters.AddWithValue("@AccruedAmtPenalCharges", string.IsNullOrEmpty(item["AccruedAmtPenalCharges"].ToString()) ? DBNull.Value : (((object)item["AccruedAmtPenalCharges"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        comForLoanDetailMain.Parameters.AddWithValue("@AccruedAmtTotalAmount", string.IsNullOrEmpty(item["AccruedAmtTotalAmount"].ToString()) ? DBNull.Value : (((object)item["AccruedAmtTotalAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        comForLoanDetailMain.Parameters.AddWithValue("@RecoveredAmtFromDate", item["RecoveredAmtFromDate"].ToString() == "" ? DBNull.Value : ((object)item["RecoveredAmtFromDate"].ToString()));
                                        comForLoanDetailMain.Parameters.AddWithValue("@RecoveredAmtToDate", item["RecoveredAmtToDate"].ToString() == "" ? DBNull.Value : ((object)item["RecoveredAmtToDate"].ToString()));
                                        comForLoanDetailMain.Parameters.AddWithValue("@RecoveredAmtPrincipal", string.IsNullOrEmpty(item["RecoveredAmtPrincipal"].ToString()) ? DBNull.Value : (((object)item["RecoveredAmtPrincipal"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        comForLoanDetailMain.Parameters.AddWithValue("@RecoveredAmtInterest", string.IsNullOrEmpty(item["RecoveredAmtInterest"].ToString()) ? DBNull.Value : (((object)item["RecoveredAmtInterest"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        comForLoanDetailMain.Parameters.AddWithValue("@RecoveredAmtAdhocCharges", string.IsNullOrEmpty(item["RecoveredAmtAdhocCharges"].ToString()) ? DBNull.Value : (((object)item["RecoveredAmtAdhocCharges"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        comForLoanDetailMain.Parameters.AddWithValue("@RecoveredAmtInterestOnInterest", string.IsNullOrEmpty(item["RecoveredAmtInterestOnInterest"].ToString()) ? DBNull.Value : (((object)item["RecoveredAmtInterestOnInterest"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        comForLoanDetailMain.Parameters.AddWithValue("@RecoveredAmtPenalCharges", string.IsNullOrEmpty(item["RecoveredAmtPenalCharges"].ToString()) ? DBNull.Value : (((object)item["RecoveredAmtPenalCharges"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        comForLoanDetailMain.Parameters.AddWithValue("@RecoveredAmtTotalAmount", string.IsNullOrEmpty(item["RecoveredAmtTotalAmount"].ToString()) ? DBNull.Value : (((object)item["RecoveredAmtTotalAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        comForLoanDetailMain.Parameters.AddWithValue("@ProvisionPercentageId", !string.IsNullOrEmpty(item["ProvisionPercentageId"].ToString()) ? (object)item["ProvisionPercentageId"].ToString() : DBNull.Value);
                                        comForLoanDetailMain.Parameters.AddWithValue("@ProvisionDate", item["ProvisionDate"].ToString() == "" ? DBNull.Value : ((object)item["ProvisionDate"].ToString()));
                                        comForLoanDetailMain.Parameters.AddWithValue("@ProvisionAmount", string.IsNullOrEmpty(item["ProvisionAmount"].ToString()) ? DBNull.Value : (((object)item["ProvisionAmount"].ToString().Replace(",", "")) ?? DBNull.Value));
                                        insertedLoanDetailMainId = (int)comForLoanDetailMain.ExecuteScalar();
                                        comForLoanDetailMain.Parameters.Clear();
                                        #endregion
                                    }
                                }
                                #endregion

                                #region Insert start for FollowUp Details(Branch and SAC)

                                #region Branch
                                if (!string.IsNullOrEmpty(npaModel.StringifiedListOfBranchFollowUpDetailObjects) && npaModel.StringifiedListOfBranchFollowUpDetailObjects != "[]" && npaModel.StringifiedListOfBranchFollowUpDetailObjects != "{}")
                                {

                                    #region Insert/Update cases for Branch FollowUps

                                    #region Parsing the StringifiedListOfBranchFollowUpDetailObjects and operating on it

                                    string strListOfBranchFollowUps = $"{{\"branchFollowUps\":{npaModel.StringifiedListOfBranchFollowUpDetailObjects}}}";
                                    JObject joListOfBranchFollowUps = JObject.Parse(strListOfBranchFollowUps);
                                    JArray jaListOfBranchFollowUps = (JArray)joListOfBranchFollowUps["branchFollowUps"];
                                    int insertedFollowUpMainIdFromBranchFollowUpMain = 0;
                                    int insertedFollowUpMainIdFromBranchFollowUpDetail = 0;
                                    List<string> lstOfValidBranchDetailIds = new List<string>();
                                    foreach (var item in jaListOfBranchFollowUps)
                                    {
                                        if (item["FollowUpTypeId"].ToString() != "" && item["FollowUpMainId"].ToString() != "0")
                                        {
                                            insertedFollowUpMainIdFromBranchFollowUpDetail = 0;
                                            int updatedFollowUpMainIdFromBranchFollowUpDetail = 0;
                                            #region Update FollowUp Main table Branch FollowUps for Modified Date
                                            comForBranchFollowUpMain.CommandType = CommandType.Text;
                                            comForBranchFollowUpMain.Connection = db;
                                            comForBranchFollowUpMain.Transaction = tran;
                                            comForBranchFollowUpMain.CommandText = @"UPDATE [dbo].[FollowUpMain]
                                                                            SET [ModifiedBy] = @ModifiedBy,
                                                                            [ModifiedDate] = @ModifiedDate WHERE FollowUpMainId = @FollowUpMainId AND NPAManagementId = @NPAManagementId AND FollowUpTypeId = @FollowUpTypeId AND FollowUpById = @FollowUpById";
                                            comForBranchFollowUpMain.Parameters.AddWithValue("@NPAManagementId", npaModel.NPAManagementId);
                                            comForBranchFollowUpMain.Parameters.AddWithValue("@FollowUpMainId", item["FollowUpMainId"].ToString());
                                            comForBranchFollowUpMain.Parameters.AddWithValue("@FollowUpTypeId", ((object)item["FollowUpTypeId"].ToString()) ?? DBNull.Value);
                                            comForBranchFollowUpMain.Parameters.AddWithValue("@FollowUpById", "1");
                                            comForBranchFollowUpMain.Parameters.AddWithValue("@ModifiedBy", ((object)npaModel.UpdatedBy) ?? DBNull.Value);
                                            comForBranchFollowUpMain.Parameters.AddWithValue("@ModifiedDate", ((object)npaModel.UpdatedOn) ?? DBNull.Value);
                                            updatedFollowUpMainIdFromBranchFollowUpDetail = comForBranchFollowUpMain.ExecuteNonQuery();
                                            comForBranchFollowUpMain.Parameters.Clear();
                                            #endregion
                                            if (updatedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                            {
                                                dynamic individualBranchFollowUp = null;
                                                switch (item["FollowUpTypeId"].ToString())
                                                {
                                                    case "1":
                                                        individualBranchFollowUp = item.ToObject<FollowUpDetailByTelephone>();
                                                        if (individualBranchFollowUp != null)
                                                        {
                                                            #region Updating Branch FollowUp table(FollowUp by Telephone)
                                                            comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualBranchFollowUpDetail.Connection = db;
                                                            comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                            comForIndividualBranchFollowUpDetail.CommandText = @"UPDATE [dbo].[FollowUpDetailByTelephone]
                                                                                        SET [ContactDate] = @ContactDate,
                                                                                        [ResponseOfBorrower] = @ResponseOfBorrower,
                                                                                        [NextActionToBeTaken] = @NextActionToBeTaken,
                                                                                        [DueDateOfNextAction] = @DueDateOfNextAction WHERE [FollowUpMainId] = @FollowUpMainId";
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", item["FollowUpMainId"].ToString());
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ContactDate", item["ContactDate"].ToString() == "" ? DBNull.Value : ((object)item["ContactDate"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            insertedFollowUpMainIdFromBranchFollowUpDetail = comForIndividualBranchFollowUpDetail.ExecuteNonQuery();
                                                            comForIndividualBranchFollowUpDetail.Parameters.Clear();
                                                            if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                            {
                                                                lstOfValidBranchDetailIds.Add(item["FollowUpMainId"].ToString());
                                                            }
                                                            #endregion
                                                        }
                                                        break;
                                                    case "2":
                                                        individualBranchFollowUp = item.ToObject<FollowUpDetailByInPersonVisit>();
                                                        if (individualBranchFollowUp != null)
                                                        {
                                                            #region Updating Branch FollowUp table(FollowUp by In-Person Visit)
                                                            comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualBranchFollowUpDetail.Connection = db;
                                                            comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                            comForIndividualBranchFollowUpDetail.CommandText = @"UPDATE [dbo].[FollowUpDetailByInPersonVisit]
                                                                                                         SET
                                                                                                         [VisitDate] = @VisitDate
                                                                                                        ,[ResponseOfBorrower] = @ResponseOfBorrower
                                                                                                        ,[NextActionToBeTaken] = @NextActionToBeTaken
                                                                                                        ,[DueDateOfNextAction] = @DueDateOfNextAction WHERE [FollowUpMainId] = @FollowUpMainId";
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", item["FollowUpMainId"].ToString());
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@VisitDate", item["VisitDate"].ToString() == "" ? DBNull.Value : ((object)item["VisitDate"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            insertedFollowUpMainIdFromBranchFollowUpDetail = comForIndividualBranchFollowUpDetail.ExecuteNonQuery();
                                                            comForIndividualBranchFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualBranchFollowUp.ScanCopyOfActionTakenFile) && individualBranchFollowUp.ScanCopyOfActionTakenFile != "[]" && individualBranchFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualBranchFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}"));
                                                                    }
                                                                    if (Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}"));
                                                                        foreach (FileInfo fi in dir.GetFiles())
                                                                        {
                                                                            fi.Delete();
                                                                        }
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}/File.{individualBranchFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidBranchDetailIds.Add(item["FollowUpMainId"].ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "3":
                                                        individualBranchFollowUp = item.ToObject<FollowUpDetailFor15DaysLetter>();
                                                        if (individualBranchFollowUp != null)
                                                        {
                                                            #region Updating Branch FollowUp table(FollowUp by 15 Days Letter)
                                                            comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualBranchFollowUpDetail.Connection = db;
                                                            comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                            comForIndividualBranchFollowUpDetail.CommandText = @"UPDATE [dbo].[FollowUpDetailFor15DaysLetter]
                                                                                                        SET
                                                                                                        [DateOfIssuanceOfFollowUpLetter] = @DateOfIssuanceOfFollowUpLetter
                                                                                                       ,[DateOfReceiptOfLetter] = @DateOfReceiptOfLetter
                                                                                                       ,[ResponseOfBorrower] = @ResponseOfBorrower
                                                                                                       ,[NextActionToBeTaken] = @NextActionToBeTaken
                                                                                                       ,[DueDateOfNextAction] = @DueDateOfNextAction WHERE [FollowUpMainId] = @FollowUpMainId";
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", item["FollowUpMainId"].ToString());
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfIssuanceOfFollowUpLetter", item["DateOfIssuanceOfFollowUpLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfIssuanceOfFollowUpLetter"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfReceiptOfLetter", item["DateOfReceiptOfLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfReceiptOfLetter"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            insertedFollowUpMainIdFromBranchFollowUpDetail = comForIndividualBranchFollowUpDetail.ExecuteNonQuery();
                                                            comForIndividualBranchFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualBranchFollowUp.ScanCopyOfActionTakenFile) && individualBranchFollowUp.ScanCopyOfActionTakenFile != "[]" && individualBranchFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualBranchFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}"));
                                                                    }
                                                                    if (Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}"));
                                                                        foreach (FileInfo fi in dir.GetFiles())
                                                                        {
                                                                            fi.Delete();
                                                                        }
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}/File.{individualBranchFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidBranchDetailIds.Add(item["FollowUpMainId"].ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "4":
                                                        individualBranchFollowUp = item.ToObject<FollowUpDetailForFirst7DaysLetter>();
                                                        if (individualBranchFollowUp != null)
                                                        {
                                                            #region Updating Branch FollowUp table(FollowUp by First 7 Days Letter)
                                                            comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualBranchFollowUpDetail.Connection = db;
                                                            comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                            comForIndividualBranchFollowUpDetail.CommandText = @"UPDATE [dbo].[FollowUpDetailForFirst7DaysLetter]
                                                                                                         SET
                                                                                                         [DateOfIssuanceOfFollowUpLetter] = @DateOfIssuanceOfFollowUpLetter
                                                                                                        ,[DateOfReceiptOfLetter] = @DateOfReceiptOfLetter
                                                                                                        ,[ResponseOfBorrower] = @ResponseOfBorrower
                                                                                                        ,[NextActionToBeTaken] = @NextActionToBeTaken
                                                                                                        ,[DueDateOfNextAction] = @DueDateOfNextAction WHERE [FollowUpMainId] = @FollowUpMainId";
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", item["FollowUpMainId"].ToString());
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfIssuanceOfFollowUpLetter", item["DateOfIssuanceOfFollowUpLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfIssuanceOfFollowUpLetter"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfReceiptOfLetter", item["DateOfReceiptOfLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfReceiptOfLetter"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            insertedFollowUpMainIdFromBranchFollowUpDetail = comForIndividualBranchFollowUpDetail.ExecuteNonQuery();
                                                            comForIndividualBranchFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualBranchFollowUp.ScanCopyOfActionTakenFile) && individualBranchFollowUp.ScanCopyOfActionTakenFile != "[]" && individualBranchFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualBranchFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}"));
                                                                    }
                                                                    if (Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}"));
                                                                        foreach (FileInfo fi in dir.GetFiles())
                                                                        {
                                                                            fi.Delete();
                                                                        }
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}/File.{individualBranchFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidBranchDetailIds.Add(item["FollowUpMainId"].ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "5":
                                                        individualBranchFollowUp = item.ToObject<FollowUpDetailForSecond7DaysLetter>();
                                                        if (individualBranchFollowUp != null)
                                                        {
                                                            #region Updating Branch FollowUp table(FollowUp by Second 7 Days Letter)
                                                            comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualBranchFollowUpDetail.Connection = db;
                                                            comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                            comForIndividualBranchFollowUpDetail.CommandText = @"UPDATE [dbo].[FollowUpDetailForSecond7DaysLetter]
                                                                                                        SET
                                                                                                        [DateOfIssuanceOfFollowUpLetter] = @DateOfIssuanceOfFollowUpLetter
                                                                                                       ,[DateOfReceiptOfLetter] = @DateOfReceiptOfLetter
                                                                                                       ,[ResponseOfBorrower] = @ResponseOfBorrower
                                                                                                       ,[NextActionToBeTaken] = @NextActionToBeTaken
                                                                                                       ,[DueDateOfNextAction] = @DueDateOfNextAction WHERE [FollowUpMainId] = @FollowUpMainId";
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", item["FollowUpMainId"].ToString());
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfIssuanceOfFollowUpLetter", item["DateOfIssuanceOfFollowUpLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfIssuanceOfFollowUpLetter"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfReceiptOfLetter", item["DateOfReceiptOfLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfReceiptOfLetter"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            insertedFollowUpMainIdFromBranchFollowUpDetail = comForIndividualBranchFollowUpDetail.ExecuteNonQuery();
                                                            comForIndividualBranchFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualBranchFollowUp.ScanCopyOfActionTakenFile) && individualBranchFollowUp.ScanCopyOfActionTakenFile != "[]" && individualBranchFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualBranchFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}"));
                                                                    }
                                                                    if (Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}"));
                                                                        foreach (FileInfo fi in dir.GetFiles())
                                                                        {
                                                                            fi.Delete();
                                                                        }
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}/File.{individualBranchFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidBranchDetailIds.Add(item["FollowUpMainId"].ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "6":
                                                        individualBranchFollowUp = item.ToObject<FollowUpDetailFor35DaysCallNotice>();
                                                        if (individualBranchFollowUp != null)
                                                        {
                                                            #region Updating Branch FollowUp table(FollowUp by 35 Days Call Notice)
                                                            comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualBranchFollowUpDetail.Connection = db;
                                                            comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                            comForIndividualBranchFollowUpDetail.CommandText = @"UPDATE [dbo].[FollowUpDetailFor35DaysCallNotice]
                                                                                                        SET
                                                                                                        [DateOfNotice] = @DateOfNotice
                                                                                                       ,[NewspaperName] = @NewspaperName
                                                                                                       ,[ResponseOfBorrower] = @ResponseOfBorrower
                                                                                                       ,[NextActionToBeTaken] = @NextActionToBeTaken
                                                                                                       ,[DueDateOfNextAction] = @DueDateOfNextAction
                                                                                                       ,[FollowUpResultId] = @FollowUpResultId
                                                                                                       ,[Remarks] = @Remarks WHERE [FollowUpMainId] = @FollowUpMainId";
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", item["FollowUpMainId"].ToString());
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfNotice", item["DateOfNotice"].ToString() == "" ? DBNull.Value : ((object)item["DateOfNotice"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NewspaperName", ((object)item["NewspaperName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpResultId", ((object)item["FollowUpResultId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@Remarks", ((object)item["Remarks"].ToString()) ?? DBNull.Value);
                                                            insertedFollowUpMainIdFromBranchFollowUpDetail = comForIndividualBranchFollowUpDetail.ExecuteNonQuery();
                                                            comForIndividualBranchFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualBranchFollowUp.ScanCopyOfActionTakenFile) && individualBranchFollowUp.ScanCopyOfActionTakenFile != "[]" && individualBranchFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualBranchFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}"));
                                                                    }
                                                                    if (Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}"));
                                                                        foreach (FileInfo fi in dir.GetFiles())
                                                                        {
                                                                            fi.Delete();
                                                                        }
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}/File.{individualBranchFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidBranchDetailIds.Add(item["FollowUpMainId"].ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "7":
                                                        individualBranchFollowUp = item.ToObject<FollowUpDetailForAuctionNotice>();
                                                        if (individualBranchFollowUp != null)
                                                        {
                                                            #region Updating Branch FollowUp table(FollowUp by Auction Notice)
                                                            comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualBranchFollowUpDetail.Connection = db;
                                                            comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                            comForIndividualBranchFollowUpDetail.CommandText = @"UPDATE [dbo].[FollowUpDetailForAuctionNotice]
                                                                                                        SET
                                                                                                        [AuctionNoticeTypeId] = @AuctionNoticeTypeId
                                                                                                       ,[DateOfAuctionNotice] = @DateOfAuctionNotice
                                                                                                       ,[AuctionNewspaperName] = @AuctionNewspaperName
                                                                                                       ,[AuctionResponseOfBorrower] = @AuctionResponseOfBorrower
                                                                                                       ,[AuctionNextActionToBeTaken] = @AuctionNextActionToBeTaken
                                                                                                       ,[AuctionDueDateOfNextAction] = @AuctionDueDateOfNextAction
                                                                                                       ,[AuctionRemarks] = @AuctionRemarks
                                                                                                       ,[DateOfRevaluationForAuction] = @DateOfRevaluationForAuction
                                                                                                       ,[RevaluatedFMVOfCollateralForAuction] = @RevaluatedFMVOfCollateralForAuction WHERE [FollowUpMainId] = @FollowUpMainId";
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", item["FollowUpMainId"].ToString());
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@AuctionNoticeTypeId", ((object)item["AuctionNoticeTypeId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfAuctionNotice", item["DateOfAuctionNotice"].ToString() == "" ? DBNull.Value : ((object)item["DateOfAuctionNotice"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@AuctionNewspaperName", ((object)item["AuctionNewspaperName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@AuctionResponseOfBorrower", ((object)item["AuctionResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@AuctionNextActionToBeTaken", ((object)item["AuctionNextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@AuctionDueDateOfNextAction", item["AuctionDueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["AuctionDueDateOfNextAction"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@AuctionRemarks", ((object)item["AuctionRemarks"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfRevaluationForAuction", item["DateOfRevaluationForAuction"].ToString() == "" ? DBNull.Value : ((object)item["DateOfRevaluationForAuction"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@RevaluatedFMVOfCollateralForAuction", string.IsNullOrEmpty(item["RevaluatedFMVOfCollateralForAuction"].ToString()) ? DBNull.Value : (((object)item["RevaluatedFMVOfCollateralForAuction"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            insertedFollowUpMainIdFromBranchFollowUpDetail = comForIndividualBranchFollowUpDetail.ExecuteNonQuery();
                                                            comForIndividualBranchFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualBranchFollowUp.ScanCopyOfActionTakenFile) && individualBranchFollowUp.ScanCopyOfActionTakenFile != "[]" && individualBranchFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualBranchFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}"));
                                                                    }
                                                                    if (Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}"));
                                                                        foreach (FileInfo fi in dir.GetFiles())
                                                                        {
                                                                            fi.Delete();
                                                                        }
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{item["FollowUpMainId"].ToString()}/File.{individualBranchFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidBranchDetailIds.Add(item["FollowUpMainId"].ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "8":
                                                        individualBranchFollowUp = item.ToObject<FollowUpDetailForOthers>();
                                                        if (individualBranchFollowUp != null)
                                                        {
                                                            #region Updating Branch FollowUp table(FollowUp for Others)
                                                            comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualBranchFollowUpDetail.Connection = db;
                                                            comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                            comForIndividualBranchFollowUpDetail.CommandText = @"UPDATE [dbo].[FollowUpDetailForOthers]
                                                                                        SET [ContactDate] = @ContactDate,
                                                                                        [ResponseOfBorrower] = @ResponseOfBorrower,
                                                                                        [NextActionToBeTaken] = @NextActionToBeTaken,
                                                                                        [DueDateOfNextAction] = @DueDateOfNextAction,
                                                                                        [Remarks] = @Remarks WHERE [FollowUpMainId] = @FollowUpMainId";
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", item["FollowUpMainId"].ToString());
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ContactDate", item["ContactDate"].ToString() == "" ? DBNull.Value : ((object)item["ContactDate"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@Remarks", ((object)item["Remarks"].ToString()) ?? DBNull.Value);
                                                            insertedFollowUpMainIdFromBranchFollowUpDetail = comForIndividualBranchFollowUpDetail.ExecuteNonQuery();
                                                            comForIndividualBranchFollowUpDetail.Parameters.Clear();
                                                            if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                            {
                                                                lstOfValidBranchDetailIds.Add(item["FollowUpMainId"].ToString());
                                                            }
                                                            #endregion
                                                        }
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                        else if (item["FollowUpTypeId"].ToString() != "" && item["FollowUpMainId"].ToString() == "0")
                                        {
                                            insertedFollowUpMainIdFromBranchFollowUpDetail = 0;
                                            #region Insert to FollowUp Main table Branch FollowUps
                                            comForBranchFollowUpMain.CommandType = CommandType.Text;
                                            comForBranchFollowUpMain.Connection = db;
                                            comForBranchFollowUpMain.Transaction = tran;
                                            comForBranchFollowUpMain.CommandText = @"INSERT INTO [dbo].[FollowUpMain]
                                                                            ([NPAManagementId]
                                                                            ,[FollowUpTypeId]
                                                                            ,[FollowUpById]
                                                                            ,[EnteredBy]
                                                                            ,[EnteredDate]
                                                                            ,[ModifiedBy]
                                                                            ,[ModifiedDate]) OUTPUT INSERTED.FollowUpMainId
                                                                        VALUES
                                                                            (@NPAManagementId,
                                                                             @FollowUpTypeId,
                                                                             @FollowUpById,
                                                                             @EnteredBy,
                                                                             @EnteredDate,
                                                                             @ModifiedBy,
                                                                             @ModifiedDate)";
                                            comForBranchFollowUpMain.Parameters.AddWithValue("@NPAManagementId", npaModel.NPAManagementId);
                                            comForBranchFollowUpMain.Parameters.AddWithValue("@FollowUpTypeId", ((object)item["FollowUpTypeId"].ToString()) ?? DBNull.Value);
                                            comForBranchFollowUpMain.Parameters.AddWithValue("@FollowUpById", "1");
                                            comForBranchFollowUpMain.Parameters.AddWithValue("@EnteredBy", ((object)npaModel.EnteredBy) ?? DBNull.Value);
                                            comForBranchFollowUpMain.Parameters.AddWithValue("@EnteredDate", ((object)npaModel.EnteredOn) ?? DBNull.Value);
                                            comForBranchFollowUpMain.Parameters.AddWithValue("@ModifiedBy", ((object)npaModel.EnteredBy) ?? DBNull.Value);
                                            comForBranchFollowUpMain.Parameters.AddWithValue("@ModifiedDate", ((object)npaModel.EnteredOn) ?? DBNull.Value);
                                            insertedFollowUpMainIdFromBranchFollowUpMain = (int)comForBranchFollowUpMain.ExecuteScalar();
                                            comForBranchFollowUpMain.Parameters.Clear();
                                            #endregion
                                            if (insertedFollowUpMainIdFromBranchFollowUpMain > 0)
                                            {
                                                dynamic individualBranchFollowUp = null;
                                                switch (item["FollowUpTypeId"].ToString())
                                                {
                                                    case "1":
                                                        individualBranchFollowUp = item.ToObject<FollowUpDetailByTelephone>();
                                                        if (individualBranchFollowUp != null)
                                                        {
                                                            #region Inserting into Branch FollowUp table(FollowUp by Telephone)
                                                            comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualBranchFollowUpDetail.Connection = db;
                                                            comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                            comForIndividualBranchFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailByTelephone]
                                                                                       ([FollowUpMainId]
                                                                                       ,[ContactDate]
                                                                                       ,[ResponseOfBorrower]
                                                                                       ,[NextActionToBeTaken]
                                                                                       ,[DueDateOfNextAction]) OUTPUT INSERTED.FollowUpMainId
                                                                                 VALUES
                                                                                       (@FollowUpMainId,
                                                                                        @ContactDate,
                                                                                        @ResponseOfBorrower,
                                                                                        @NextActionToBeTaken,
                                                                                        @DueDateOfNextAction)";
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromBranchFollowUpMain);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ContactDate", item["ContactDate"].ToString() == "" ? DBNull.Value : ((object)item["ContactDate"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            insertedFollowUpMainIdFromBranchFollowUpDetail = (int)comForIndividualBranchFollowUpDetail.ExecuteScalar();
                                                            comForIndividualBranchFollowUpDetail.Parameters.Clear();
                                                            if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                            {
                                                                lstOfValidBranchDetailIds.Add(insertedFollowUpMainIdFromBranchFollowUpMain.ToString());
                                                            }
                                                            #endregion
                                                        }
                                                        break;
                                                    case "2":
                                                        individualBranchFollowUp = item.ToObject<FollowUpDetailByInPersonVisit>();
                                                        if (individualBranchFollowUp != null)
                                                        {
                                                            #region Inserting into Branch FollowUp table(FollowUp by In-Person Visit)
                                                            comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualBranchFollowUpDetail.Connection = db;
                                                            comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                            comForIndividualBranchFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailByInPersonVisit]
                                                                                                        ([FollowUpMainId]
                                                                                                        ,[VisitDate]
                                                                                                        ,[ResponseOfBorrower]
                                                                                                        ,[NextActionToBeTaken]
                                                                                                        ,[DueDateOfNextAction]) OUTPUT INSERTED.FollowUpMainId
                                                                                                    VALUES
                                                                                                        (@FollowUpMainId,
                                                                                                         @VisitDate,
                                                                                                         @ResponseOfBorrower,
                                                                                                         @NextActionToBeTaken,
                                                                                                         @DueDateOfNextAction)";
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromBranchFollowUpMain);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@VisitDate", item["VisitDate"].ToString() == "" ? DBNull.Value : ((object)item["VisitDate"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            insertedFollowUpMainIdFromBranchFollowUpDetail = (int)comForIndividualBranchFollowUpDetail.ExecuteScalar();
                                                            comForIndividualBranchFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualBranchFollowUp.ScanCopyOfActionTakenFile) && individualBranchFollowUp.ScanCopyOfActionTakenFile != "[]" && individualBranchFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualBranchFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}"));
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}/File.{individualBranchFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidBranchDetailIds.Add(insertedFollowUpMainIdFromBranchFollowUpDetail.ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "3":
                                                        individualBranchFollowUp = item.ToObject<FollowUpDetailFor15DaysLetter>();
                                                        if (individualBranchFollowUp != null)
                                                        {
                                                            #region Inserting into Branch FollowUp table(FollowUp by 15 Days Letter)
                                                            comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualBranchFollowUpDetail.Connection = db;
                                                            comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                            comForIndividualBranchFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailFor15DaysLetter]
                                                                                                       ([FollowUpMainId]
                                                                                                       ,[DateOfIssuanceOfFollowUpLetter]
                                                                                                       ,[DateOfReceiptOfLetter]
                                                                                                       ,[ResponseOfBorrower]
                                                                                                       ,[NextActionToBeTaken]
                                                                                                       ,[DueDateOfNextAction]) OUTPUT INSERTED.FollowUpMainId
                                                                                                 VALUES
                                                                                                       (@FollowUpMainId,
                                                                                                        @DateOfIssuanceOfFollowUpLetter,
                                                                                                        @DateOfReceiptOfLetter,
                                                                                                        @ResponseOfBorrower,
                                                                                                        @NextActionToBeTaken,
                                                                                                        @DueDateOfNextAction)";
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromBranchFollowUpMain);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfIssuanceOfFollowUpLetter", item["DateOfIssuanceOfFollowUpLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfIssuanceOfFollowUpLetter"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfReceiptOfLetter", item["DateOfReceiptOfLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfReceiptOfLetter"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            insertedFollowUpMainIdFromBranchFollowUpDetail = (int)comForIndividualBranchFollowUpDetail.ExecuteScalar();
                                                            comForIndividualBranchFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualBranchFollowUp.ScanCopyOfActionTakenFile) && individualBranchFollowUp.ScanCopyOfActionTakenFile != "[]" && individualBranchFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualBranchFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}"));
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}/File.{individualBranchFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidBranchDetailIds.Add(insertedFollowUpMainIdFromBranchFollowUpDetail.ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "4":
                                                        individualBranchFollowUp = item.ToObject<FollowUpDetailForFirst7DaysLetter>();
                                                        if (individualBranchFollowUp != null)
                                                        {
                                                            #region Inserting into Branch FollowUp table(FollowUp by First 7 Days Letter)
                                                            comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualBranchFollowUpDetail.Connection = db;
                                                            comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                            comForIndividualBranchFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailForFirst7DaysLetter]
                                                                                                        ([FollowUpMainId]
                                                                                                        ,[DateOfIssuanceOfFollowUpLetter]
                                                                                                        ,[DateOfReceiptOfLetter]
                                                                                                        ,[ResponseOfBorrower]
                                                                                                        ,[NextActionToBeTaken]
                                                                                                        ,[DueDateOfNextAction]) OUTPUT INSERTED.FollowUpMainId
                                                                                                    VALUES
                                                                                                        (@FollowUpMainId,
                                                                                                         @DateOfIssuanceOfFollowUpLetter,
                                                                                                         @DateOfReceiptOfLetter,
                                                                                                         @ResponseOfBorrower,
                                                                                                         @NextActionToBeTaken,
                                                                                                         @DueDateOfNextAction)";
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromBranchFollowUpMain);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfIssuanceOfFollowUpLetter", item["DateOfIssuanceOfFollowUpLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfIssuanceOfFollowUpLetter"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfReceiptOfLetter", item["DateOfReceiptOfLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfReceiptOfLetter"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            insertedFollowUpMainIdFromBranchFollowUpDetail = (int)comForIndividualBranchFollowUpDetail.ExecuteScalar();
                                                            comForIndividualBranchFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualBranchFollowUp.ScanCopyOfActionTakenFile) && individualBranchFollowUp.ScanCopyOfActionTakenFile != "[]" && individualBranchFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualBranchFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}"));
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}/File.{individualBranchFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidBranchDetailIds.Add(insertedFollowUpMainIdFromBranchFollowUpDetail.ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "5":
                                                        individualBranchFollowUp = item.ToObject<FollowUpDetailForSecond7DaysLetter>();
                                                        if (individualBranchFollowUp != null)
                                                        {
                                                            #region Inserting into Branch FollowUp table(FollowUp by Second 7 Days Letter)
                                                            comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualBranchFollowUpDetail.Connection = db;
                                                            comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                            comForIndividualBranchFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailForSecond7DaysLetter]
                                                                                                       ([FollowUpMainId]
                                                                                                       ,[DateOfIssuanceOfFollowUpLetter]
                                                                                                       ,[DateOfReceiptOfLetter]
                                                                                                       ,[ResponseOfBorrower]
                                                                                                       ,[NextActionToBeTaken]
                                                                                                       ,[DueDateOfNextAction]) OUTPUT INSERTED.FollowUpMainId
                                                                                                 VALUES
                                                                                                       (@FollowUpMainId,
                                                                                                        @DateOfIssuanceOfFollowUpLetter,
                                                                                                        @DateOfReceiptOfLetter,
                                                                                                        @ResponseOfBorrower,
                                                                                                        @NextActionToBeTaken,
                                                                                                        @DueDateOfNextAction)";
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromBranchFollowUpMain);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfIssuanceOfFollowUpLetter", item["DateOfIssuanceOfFollowUpLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfIssuanceOfFollowUpLetter"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfReceiptOfLetter", item["DateOfReceiptOfLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfReceiptOfLetter"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            insertedFollowUpMainIdFromBranchFollowUpDetail = (int)comForIndividualBranchFollowUpDetail.ExecuteScalar();
                                                            comForIndividualBranchFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualBranchFollowUp.ScanCopyOfActionTakenFile) && individualBranchFollowUp.ScanCopyOfActionTakenFile != "[]" && individualBranchFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualBranchFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}"));
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}/File.{individualBranchFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidBranchDetailIds.Add(insertedFollowUpMainIdFromBranchFollowUpDetail.ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "6":
                                                        individualBranchFollowUp = item.ToObject<FollowUpDetailFor35DaysCallNotice>();
                                                        if (individualBranchFollowUp != null)
                                                        {
                                                            #region Inserting into Branch FollowUp table(FollowUp by 35 Days Call Notice)
                                                            comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualBranchFollowUpDetail.Connection = db;
                                                            comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                            comForIndividualBranchFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailFor35DaysCallNotice]
                                                                                                       ([FollowUpMainId]
                                                                                                       ,[DateOfNotice]
                                                                                                       ,[NewspaperName]
                                                                                                       ,[ResponseOfBorrower]
                                                                                                       ,[NextActionToBeTaken]
                                                                                                       ,[DueDateOfNextAction]
                                                                                                       ,[FollowUpResultId]
                                                                                                       ,[Remarks]) OUTPUT INSERTED.FollowUpMainId
                                                                                                 VALUES
                                                                                                       (@FollowUpMainId,
                                                                                                        @DateOfNotice,
                                                                                                        @NewspaperName,
                                                                                                        @ResponseOfBorrower,
                                                                                                        @NextActionToBeTaken,
                                                                                                        @DueDateOfNextAction,
                                                                                                        @FollowUpResultId,
                                                                                                        @Remarks)";
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromBranchFollowUpMain);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfNotice", item["DateOfNotice"].ToString() == "" ? DBNull.Value : ((object)item["DateOfNotice"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NewspaperName", ((object)item["NewspaperName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpResultId", ((object)item["FollowUpResultId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@Remarks", ((object)item["Remarks"].ToString()) ?? DBNull.Value);
                                                            insertedFollowUpMainIdFromBranchFollowUpDetail = (int)comForIndividualBranchFollowUpDetail.ExecuteScalar();
                                                            comForIndividualBranchFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualBranchFollowUp.ScanCopyOfActionTakenFile) && individualBranchFollowUp.ScanCopyOfActionTakenFile != "[]" && individualBranchFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualBranchFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}"));
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}/File.{individualBranchFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidBranchDetailIds.Add(insertedFollowUpMainIdFromBranchFollowUpDetail.ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "7":
                                                        individualBranchFollowUp = item.ToObject<FollowUpDetailForAuctionNotice>();
                                                        if (individualBranchFollowUp != null)
                                                        {
                                                            #region Inserting into Branch FollowUp table(FollowUp by Auction Notice)
                                                            comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualBranchFollowUpDetail.Connection = db;
                                                            comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                            comForIndividualBranchFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailForAuctionNotice]
                                                                                                       ([FollowUpMainId]
                                                                                                       ,[AuctionNoticeTypeId]
                                                                                                       ,[DateOfAuctionNotice]
                                                                                                       ,[AuctionNewspaperName]
                                                                                                       ,[AuctionResponseOfBorrower]
                                                                                                       ,[AuctionNextActionToBeTaken]
                                                                                                       ,[AuctionDueDateOfNextAction]
                                                                                                       ,[AuctionRemarks]
                                                                                                       ,[DateOfRevaluationForAuction]
                                                                                                       ,[RevaluatedFMVOfCollateralForAuction]) OUTPUT INSERTED.FollowUpMainId
                                                                                                 VALUES
                                                                                                       (@FollowUpMainId,
                                                                                                        @AuctionNoticeTypeId,
                                                                                                        @DateOfAuctionNotice,
                                                                                                        @AuctionNewspaperName,
                                                                                                        @AuctionResponseOfBorrower,
                                                                                                        @AuctionNextActionToBeTaken,
                                                                                                        @AuctionDueDateOfNextAction,
                                                                                                        @AuctionRemarks,
                                                                                                        @DateOfRevaluationForAuction,
                                                                                                        @RevaluatedFMVOfCollateralForAuction)";
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromBranchFollowUpMain);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@AuctionNoticeTypeId", ((object)item["AuctionNoticeTypeId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfAuctionNotice", item["DateOfAuctionNotice"].ToString() == "" ? DBNull.Value : ((object)item["DateOfAuctionNotice"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@AuctionNewspaperName", ((object)item["AuctionNewspaperName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@AuctionResponseOfBorrower", ((object)item["AuctionResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@AuctionNextActionToBeTaken", ((object)item["AuctionNextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@AuctionDueDateOfNextAction", item["AuctionDueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["AuctionDueDateOfNextAction"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@AuctionRemarks", ((object)item["AuctionRemarks"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DateOfRevaluationForAuction", item["DateOfRevaluationForAuction"].ToString() == "" ? DBNull.Value : ((object)item["DateOfRevaluationForAuction"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@RevaluatedFMVOfCollateralForAuction", string.IsNullOrEmpty(item["RevaluatedFMVOfCollateralForAuction"].ToString()) ? DBNull.Value : (((object)item["RevaluatedFMVOfCollateralForAuction"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            insertedFollowUpMainIdFromBranchFollowUpDetail = (int)comForIndividualBranchFollowUpDetail.ExecuteScalar();
                                                            comForIndividualBranchFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualBranchFollowUp.ScanCopyOfActionTakenFile) && individualBranchFollowUp.ScanCopyOfActionTakenFile != "[]" && individualBranchFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualBranchFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}"));
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/{insertedFollowUpMainIdFromBranchFollowUpDetail}/File.{individualBranchFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidBranchDetailIds.Add(insertedFollowUpMainIdFromBranchFollowUpDetail.ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "8":
                                                        individualBranchFollowUp = item.ToObject<FollowUpDetailForOthers>();
                                                        if (individualBranchFollowUp != null)
                                                        {
                                                            #region Inserting into Branch FollowUp table(FollowUp for Others)
                                                            comForIndividualBranchFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualBranchFollowUpDetail.Connection = db;
                                                            comForIndividualBranchFollowUpDetail.Transaction = tran;
                                                            comForIndividualBranchFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailForOthers]
                                                                                       ([FollowUpMainId]
                                                                                       ,[ContactDate]
                                                                                       ,[ResponseOfBorrower]
                                                                                       ,[NextActionToBeTaken]
                                                                                       ,[DueDateOfNextAction]
                                                                                       ,[Remarks]) OUTPUT INSERTED.FollowUpMainId
                                                                                 VALUES
                                                                                       (@FollowUpMainId,
                                                                                        @ContactDate,
                                                                                        @ResponseOfBorrower,
                                                                                        @NextActionToBeTaken,
                                                                                        @DueDateOfNextAction,
                                                                                        @Remarks)";
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromBranchFollowUpMain);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ContactDate", item["ContactDate"].ToString() == "" ? DBNull.Value : ((object)item["ContactDate"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            comForIndividualBranchFollowUpDetail.Parameters.AddWithValue("@Remarks", ((object)item["Remarks"].ToString()) ?? DBNull.Value);
                                                            insertedFollowUpMainIdFromBranchFollowUpDetail = (int)comForIndividualBranchFollowUpDetail.ExecuteScalar();
                                                            comForIndividualBranchFollowUpDetail.Parameters.Clear();
                                                            if (insertedFollowUpMainIdFromBranchFollowUpDetail > 0)
                                                            {
                                                                lstOfValidBranchDetailIds.Add(insertedFollowUpMainIdFromBranchFollowUpMain.ToString());
                                                            }
                                                            #endregion
                                                        }
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                    }

                                    #region Delete removed followups from previously saved list
                                    DeleteRemovedFollowUps("1", npaModel.NPAManagementId, lstOfValidBranchDetailIds, db, tran);
                                    DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/"));
                                    if (Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-1/")))
                                    {
                                        foreach (DirectoryInfo dir in di.GetDirectories())
                                        {
                                            if (!lstOfValidBranchDetailIds.Contains(dir.Name))
                                            {
                                                dir.Delete(true);
                                            }
                                        }
                                    }
                                    #endregion
                                    #endregion

                                    #endregion

                                }
                                #endregion

                                #region SAC
                                if (!string.IsNullOrEmpty(npaModel.StringifiedListOfSACFollowUpDetailObjects) && npaModel.StringifiedListOfSACFollowUpDetailObjects != "[]" && npaModel.StringifiedListOfSACFollowUpDetailObjects != "{}")
                                {

                                    #region Insert cases for SAC FollowUps

                                    #region Parsing the StringifiedListOfSACFollowUpDetailObjects and operating on it

                                    string strListOfSACFollowUps = $"{{\"sacFollowUps\":{npaModel.StringifiedListOfSACFollowUpDetailObjects}}}";
                                    JObject joListOfSACFollowUps = JObject.Parse(strListOfSACFollowUps);
                                    JArray jaListOfSACFollowUps = (JArray)joListOfSACFollowUps["sacFollowUps"];
                                    int insertedFollowUpMainIdFromSACFollowUpMain = 0;
                                    int insertedFollowUpMainIdFromSACFollowUpDetail = 0;
                                    List<string> lstOfValidSACDetailIds = new List<string>();
                                    foreach (var item in jaListOfSACFollowUps)
                                    {
                                        if (item["FollowUpTypeId"].ToString() != "" && item["FollowUpMainId"].ToString() != "0")
                                        {
                                            insertedFollowUpMainIdFromSACFollowUpDetail = 0;
                                            int updatedFollowUpMainIdFromSACFollowUpDetail = 0;
                                            #region Update FollowUp Main table SAC FollowUps for Modified Date
                                            comForSACFollowUpMain.CommandType = CommandType.Text;
                                            comForSACFollowUpMain.Connection = db;
                                            comForSACFollowUpMain.Transaction = tran;
                                            comForSACFollowUpMain.CommandText = @"UPDATE [dbo].[FollowUpMain]
                                                                            SET [ModifiedBy] = @ModifiedBy,
                                                                            [ModifiedDate] = @ModifiedDate WHERE FollowUpMainId = @FollowUpMainId AND NPAManagementId = @NPAManagementId AND FollowUpTypeId = @FollowUpTypeId AND FollowUpById = @FollowUpById";
                                            comForSACFollowUpMain.Parameters.AddWithValue("@NPAManagementId", npaModel.NPAManagementId);
                                            comForSACFollowUpMain.Parameters.AddWithValue("@FollowUpMainId", item["FollowUpMainId"].ToString());
                                            comForSACFollowUpMain.Parameters.AddWithValue("@FollowUpTypeId", ((object)item["FollowUpTypeId"].ToString()) ?? DBNull.Value);
                                            comForSACFollowUpMain.Parameters.AddWithValue("@FollowUpById", "2");
                                            comForSACFollowUpMain.Parameters.AddWithValue("@ModifiedBy", ((object)npaModel.UpdatedBy) ?? DBNull.Value);
                                            comForSACFollowUpMain.Parameters.AddWithValue("@ModifiedDate", ((object)npaModel.UpdatedOn) ?? DBNull.Value);
                                            updatedFollowUpMainIdFromSACFollowUpDetail = comForSACFollowUpMain.ExecuteNonQuery();
                                            comForSACFollowUpMain.Parameters.Clear();
                                            #endregion
                                            if (updatedFollowUpMainIdFromSACFollowUpDetail > 0)
                                            {
                                                dynamic individualSACFollowUp = null;
                                                switch (item["FollowUpTypeId"].ToString())
                                                {
                                                    case "1":
                                                        individualSACFollowUp = item.ToObject<FollowUpDetailByTelephone>();
                                                        if (individualSACFollowUp != null)
                                                        {
                                                            #region Updating SAC FollowUp table(FollowUp by Telephone)
                                                            comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualSACFollowUpDetail.Connection = db;
                                                            comForIndividualSACFollowUpDetail.Transaction = tran;
                                                            comForIndividualSACFollowUpDetail.CommandText = @"UPDATE [dbo].[FollowUpDetailByTelephone]
                                                                                        SET [ContactDate] = @ContactDate,
                                                                                        [ResponseOfBorrower] = @ResponseOfBorrower,
                                                                                        [NextActionToBeTaken] = @NextActionToBeTaken,
                                                                                        [DueDateOfNextAction] = @DueDateOfNextAction WHERE [FollowUpMainId] = @FollowUpMainId";
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", item["FollowUpMainId"].ToString());
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ContactDate", item["ContactDate"].ToString() == "" ? DBNull.Value : ((object)item["ContactDate"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            insertedFollowUpMainIdFromSACFollowUpDetail = comForIndividualSACFollowUpDetail.ExecuteNonQuery();
                                                            comForIndividualSACFollowUpDetail.Parameters.Clear();
                                                            if (insertedFollowUpMainIdFromSACFollowUpDetail>0)
                                                            {
                                                                lstOfValidSACDetailIds.Add(item["FollowUpMainId"].ToString());
                                                            }
                                                            #endregion
                                                        }
                                                        break;
                                                    case "2":
                                                        individualSACFollowUp = item.ToObject<FollowUpDetailByInPersonVisit>();
                                                        if (individualSACFollowUp != null)
                                                        {
                                                            #region Updating SAC FollowUp table(FollowUp by In-Person Visit)
                                                            comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualSACFollowUpDetail.Connection = db;
                                                            comForIndividualSACFollowUpDetail.Transaction = tran;
                                                            comForIndividualSACFollowUpDetail.CommandText = @"UPDATE [dbo].[FollowUpDetailByInPersonVisit]
                                                                                                         SET
                                                                                                         [VisitDate] = @VisitDate
                                                                                                        ,[ResponseOfBorrower] = @ResponseOfBorrower
                                                                                                        ,[NextActionToBeTaken] = @NextActionToBeTaken
                                                                                                        ,[DueDateOfNextAction] = @DueDateOfNextAction WHERE [FollowUpMainId] = @FollowUpMainId";
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", item["FollowUpMainId"].ToString());
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@VisitDate", item["VisitDate"].ToString() == "" ? DBNull.Value : ((object)item["VisitDate"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            insertedFollowUpMainIdFromSACFollowUpDetail = comForIndividualSACFollowUpDetail.ExecuteNonQuery();
                                                            comForIndividualSACFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualSACFollowUp.ScanCopyOfActionTakenFile) && individualSACFollowUp.ScanCopyOfActionTakenFile != "[]" && individualSACFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualSACFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}"));
                                                                    }
                                                                    if (Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}"));
                                                                        foreach (FileInfo fi in dir.GetFiles())
                                                                        {
                                                                            fi.Delete();
                                                                        }
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}/File.{individualSACFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidSACDetailIds.Add(item["FollowUpMainId"].ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "3":
                                                        individualSACFollowUp = item.ToObject<FollowUpDetailFor15DaysLetter>();
                                                        if (individualSACFollowUp != null)
                                                        {
                                                            #region Updating SAC FollowUp table(FollowUp by 15 Days Letter)
                                                            comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualSACFollowUpDetail.Connection = db;
                                                            comForIndividualSACFollowUpDetail.Transaction = tran;
                                                            comForIndividualSACFollowUpDetail.CommandText = @"UPDATE [dbo].[FollowUpDetailFor15DaysLetter]
                                                                                                        SET
                                                                                                        [DateOfIssuanceOfFollowUpLetter] = @DateOfIssuanceOfFollowUpLetter
                                                                                                       ,[DateOfReceiptOfLetter] = @DateOfReceiptOfLetter
                                                                                                       ,[ResponseOfBorrower] = @ResponseOfBorrower
                                                                                                       ,[NextActionToBeTaken] = @NextActionToBeTaken
                                                                                                       ,[DueDateOfNextAction] = @DueDateOfNextAction WHERE [FollowUpMainId] = @FollowUpMainId";
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", item["FollowUpMainId"].ToString());
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfIssuanceOfFollowUpLetter", item["DateOfIssuanceOfFollowUpLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfIssuanceOfFollowUpLetter"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfReceiptOfLetter", item["DateOfReceiptOfLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfReceiptOfLetter"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            insertedFollowUpMainIdFromSACFollowUpDetail = comForIndividualSACFollowUpDetail.ExecuteNonQuery();
                                                            comForIndividualSACFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualSACFollowUp.ScanCopyOfActionTakenFile) && individualSACFollowUp.ScanCopyOfActionTakenFile != "[]" && individualSACFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualSACFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}"));
                                                                    }
                                                                    if (Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}"));
                                                                        foreach (FileInfo fi in dir.GetFiles())
                                                                        {
                                                                            fi.Delete();
                                                                        }
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}/File.{individualSACFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidSACDetailIds.Add(item["FollowUpMainId"].ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "4":
                                                        individualSACFollowUp = item.ToObject<FollowUpDetailForFirst7DaysLetter>();
                                                        if (individualSACFollowUp != null)
                                                        {
                                                            #region Updating SAC FollowUp table(FollowUp by First 7 Days Letter)
                                                            comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualSACFollowUpDetail.Connection = db;
                                                            comForIndividualSACFollowUpDetail.Transaction = tran;
                                                            comForIndividualSACFollowUpDetail.CommandText = @"UPDATE [dbo].[FollowUpDetailForFirst7DaysLetter]
                                                                                                         SET
                                                                                                         [DateOfIssuanceOfFollowUpLetter] = @DateOfIssuanceOfFollowUpLetter
                                                                                                        ,[DateOfReceiptOfLetter] = @DateOfReceiptOfLetter
                                                                                                        ,[ResponseOfBorrower] = @ResponseOfBorrower
                                                                                                        ,[NextActionToBeTaken] = @NextActionToBeTaken
                                                                                                        ,[DueDateOfNextAction] = @DueDateOfNextAction WHERE [FollowUpMainId] = @FollowUpMainId";
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", item["FollowUpMainId"].ToString());
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfIssuanceOfFollowUpLetter", item["DateOfIssuanceOfFollowUpLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfIssuanceOfFollowUpLetter"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfReceiptOfLetter", item["DateOfReceiptOfLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfReceiptOfLetter"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            insertedFollowUpMainIdFromSACFollowUpDetail = comForIndividualSACFollowUpDetail.ExecuteNonQuery();
                                                            comForIndividualSACFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualSACFollowUp.ScanCopyOfActionTakenFile) && individualSACFollowUp.ScanCopyOfActionTakenFile != "[]" && individualSACFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualSACFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}"));
                                                                    }
                                                                    if (Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}"));
                                                                        foreach (FileInfo fi in dir.GetFiles())
                                                                        {
                                                                            fi.Delete();
                                                                        }
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}/File.{individualSACFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidSACDetailIds.Add(item["FollowUpMainId"].ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "5":
                                                        individualSACFollowUp = item.ToObject<FollowUpDetailForSecond7DaysLetter>();
                                                        if (individualSACFollowUp != null)
                                                        {
                                                            #region Updating SAC FollowUp table(FollowUp by Second 7 Days Letter)
                                                            comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualSACFollowUpDetail.Connection = db;
                                                            comForIndividualSACFollowUpDetail.Transaction = tran;
                                                            comForIndividualSACFollowUpDetail.CommandText = @"UPDATE [dbo].[FollowUpDetailForSecond7DaysLetter]
                                                                                                        SET
                                                                                                        [DateOfIssuanceOfFollowUpLetter] = @DateOfIssuanceOfFollowUpLetter
                                                                                                       ,[DateOfReceiptOfLetter] = @DateOfReceiptOfLetter
                                                                                                       ,[ResponseOfBorrower] = @ResponseOfBorrower
                                                                                                       ,[NextActionToBeTaken] = @NextActionToBeTaken
                                                                                                       ,[DueDateOfNextAction] = @DueDateOfNextAction WHERE [FollowUpMainId] = @FollowUpMainId";
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", item["FollowUpMainId"].ToString());
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfIssuanceOfFollowUpLetter", item["DateOfIssuanceOfFollowUpLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfIssuanceOfFollowUpLetter"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfReceiptOfLetter", item["DateOfReceiptOfLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfReceiptOfLetter"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            insertedFollowUpMainIdFromSACFollowUpDetail = comForIndividualSACFollowUpDetail.ExecuteNonQuery();
                                                            comForIndividualSACFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualSACFollowUp.ScanCopyOfActionTakenFile) && individualSACFollowUp.ScanCopyOfActionTakenFile != "[]" && individualSACFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualSACFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}"));
                                                                    }
                                                                    if (Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}"));
                                                                        foreach (FileInfo fi in dir.GetFiles())
                                                                        {
                                                                            fi.Delete();
                                                                        }
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}/File.{individualSACFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidSACDetailIds.Add(item["FollowUpMainId"].ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "6":
                                                        individualSACFollowUp = item.ToObject<FollowUpDetailFor35DaysCallNotice>();
                                                        if (individualSACFollowUp != null)
                                                        {
                                                            #region Updating SAC FollowUp table(FollowUp by 35 Days Call Notice)
                                                            comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualSACFollowUpDetail.Connection = db;
                                                            comForIndividualSACFollowUpDetail.Transaction = tran;
                                                            comForIndividualSACFollowUpDetail.CommandText = @"UPDATE [dbo].[FollowUpDetailFor35DaysCallNotice]
                                                                                                        SET
                                                                                                        [DateOfNotice] = @DateOfNotice
                                                                                                       ,[NewspaperName] = @NewspaperName
                                                                                                       ,[ResponseOfBorrower] = @ResponseOfBorrower
                                                                                                       ,[NextActionToBeTaken] = @NextActionToBeTaken
                                                                                                       ,[DueDateOfNextAction] = @DueDateOfNextAction
                                                                                                       ,[FollowUpResultId] = @FollowUpResultId
                                                                                                       ,[Remarks] = @Remarks WHERE [FollowUpMainId] = @FollowUpMainId";
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", item["FollowUpMainId"].ToString());
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfNotice", item["DateOfNotice"].ToString() == "" ? DBNull.Value : ((object)item["DateOfNotice"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NewspaperName", ((object)item["NewspaperName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpResultId", ((object)item["FollowUpResultId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@Remarks", ((object)item["Remarks"].ToString()) ?? DBNull.Value);
                                                            insertedFollowUpMainIdFromSACFollowUpDetail = comForIndividualSACFollowUpDetail.ExecuteNonQuery();
                                                            comForIndividualSACFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualSACFollowUp.ScanCopyOfActionTakenFile) && individualSACFollowUp.ScanCopyOfActionTakenFile != "[]" && individualSACFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualSACFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}"));
                                                                    }
                                                                    if (Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}"));
                                                                        foreach (FileInfo fi in dir.GetFiles())
                                                                        {
                                                                            fi.Delete();
                                                                        }
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}/File.{individualSACFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidSACDetailIds.Add(item["FollowUpMainId"].ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "7":
                                                        individualSACFollowUp = item.ToObject<FollowUpDetailForAuctionNotice>();
                                                        if (individualSACFollowUp != null)
                                                        {
                                                            #region Updating SAC FollowUp table(FollowUp by Auction Notice)
                                                            comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualSACFollowUpDetail.Connection = db;
                                                            comForIndividualSACFollowUpDetail.Transaction = tran;
                                                            comForIndividualSACFollowUpDetail.CommandText = @"UPDATE [dbo].[FollowUpDetailForAuctionNotice]
                                                                                                        SET
                                                                                                        [AuctionNoticeTypeId] = @AuctionNoticeTypeId
                                                                                                       ,[DateOfAuctionNotice] = @DateOfAuctionNotice
                                                                                                       ,[AuctionNewspaperName] = @AuctionNewspaperName
                                                                                                       ,[AuctionResponseOfBorrower] = @AuctionResponseOfBorrower
                                                                                                       ,[AuctionNextActionToBeTaken] = @AuctionNextActionToBeTaken
                                                                                                       ,[AuctionDueDateOfNextAction] = @AuctionDueDateOfNextAction
                                                                                                       ,[AuctionRemarks] = @AuctionRemarks
                                                                                                       ,[DateOfRevaluationForAuction] = @DateOfRevaluationForAuction
                                                                                                       ,[RevaluatedFMVOfCollateralForAuction] = @RevaluatedFMVOfCollateralForAuction WHERE [FollowUpMainId] = @FollowUpMainId";
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", item["FollowUpMainId"].ToString());
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@AuctionNoticeTypeId", ((object)item["AuctionNoticeTypeId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfAuctionNotice", item["DateOfAuctionNotice"].ToString() == "" ? DBNull.Value : ((object)item["DateOfAuctionNotice"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@AuctionNewspaperName", ((object)item["AuctionNewspaperName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@AuctionResponseOfBorrower", ((object)item["AuctionResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@AuctionNextActionToBeTaken", ((object)item["AuctionNextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@AuctionDueDateOfNextAction", item["AuctionDueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["AuctionDueDateOfNextAction"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@AuctionRemarks", ((object)item["AuctionRemarks"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfRevaluationForAuction", item["DateOfRevaluationForAuction"].ToString() == "" ? DBNull.Value : ((object)item["DateOfRevaluationForAuction"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@RevaluatedFMVOfCollateralForAuction", string.IsNullOrEmpty(item["RevaluatedFMVOfCollateralForAuction"].ToString()) ? DBNull.Value : (((object)item["RevaluatedFMVOfCollateralForAuction"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            insertedFollowUpMainIdFromSACFollowUpDetail = comForIndividualSACFollowUpDetail.ExecuteNonQuery();
                                                            comForIndividualSACFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualSACFollowUp.ScanCopyOfActionTakenFile) && individualSACFollowUp.ScanCopyOfActionTakenFile != "[]" && individualSACFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualSACFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}"));
                                                                    }
                                                                    if (Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}")))
                                                                    {
                                                                        DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}"));
                                                                        foreach (FileInfo fi in dir.GetFiles())
                                                                        {
                                                                            fi.Delete();
                                                                        }
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{item["FollowUpMainId"].ToString()}/File.{individualSACFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidSACDetailIds.Add(item["FollowUpMainId"].ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "8":
                                                        individualSACFollowUp = item.ToObject<FollowUpDetailForOthers>();
                                                        if (individualSACFollowUp != null)
                                                        {
                                                            #region Updating SAC FollowUp table(FollowUp for Others)
                                                            comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualSACFollowUpDetail.Connection = db;
                                                            comForIndividualSACFollowUpDetail.Transaction = tran;
                                                            comForIndividualSACFollowUpDetail.CommandText = @"UPDATE [dbo].[FollowUpDetailForOthers]
                                                                                        SET [ContactDate] = @ContactDate,
                                                                                        [ResponseOfBorrower] = @ResponseOfBorrower,
                                                                                        [NextActionToBeTaken] = @NextActionToBeTaken,
                                                                                        [DueDateOfNextAction] = @DueDateOfNextAction,
                                                                                        [Remarks] = @Remarks WHERE [FollowUpMainId] = @FollowUpMainId";
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", item["FollowUpMainId"].ToString());
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ContactDate", item["ContactDate"].ToString() == "" ? DBNull.Value : ((object)item["ContactDate"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@Remarks", ((object)item["Remarks"].ToString()) ?? DBNull.Value);
                                                            insertedFollowUpMainIdFromSACFollowUpDetail = comForIndividualSACFollowUpDetail.ExecuteNonQuery();
                                                            comForIndividualSACFollowUpDetail.Parameters.Clear();
                                                            if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                            {
                                                                lstOfValidSACDetailIds.Add(item["FollowUpMainId"].ToString());
                                                            }
                                                            #endregion
                                                        }
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                        else if (item["FollowUpTypeId"].ToString() != "" && item["FollowUpMainId"].ToString() == "0")
                                        {
                                            insertedFollowUpMainIdFromSACFollowUpDetail = 0;
                                            #region Insert to FollowUp Main table SAC FollowUps
                                            comForSACFollowUpMain.CommandType = CommandType.Text;
                                            comForSACFollowUpMain.Connection = db;
                                            comForSACFollowUpMain.Transaction = tran;
                                            comForSACFollowUpMain.CommandText = @"INSERT INTO [dbo].[FollowUpMain]
                                                                            ([NPAManagementId]
                                                                            ,[FollowUpTypeId]
                                                                            ,[FollowUpById]
                                                                            ,[EnteredBy]
                                                                            ,[EnteredDate]
                                                                            ,[ModifiedBy]
                                                                            ,[ModifiedDate]) OUTPUT INSERTED.FollowUpMainId
                                                                        VALUES
                                                                            (@NPAManagementId,
                                                                             @FollowUpTypeId,
                                                                             @FollowUpById,
                                                                             @EnteredBy,
                                                                             @EnteredDate,
                                                                             @ModifiedBy,
                                                                             @ModifiedDate)";
                                            comForSACFollowUpMain.Parameters.AddWithValue("@NPAManagementId", npaModel.NPAManagementId);
                                            comForSACFollowUpMain.Parameters.AddWithValue("@FollowUpTypeId", ((object)item["FollowUpTypeId"].ToString()) ?? DBNull.Value);
                                            comForSACFollowUpMain.Parameters.AddWithValue("@FollowUpById", "2");
                                            comForSACFollowUpMain.Parameters.AddWithValue("@EnteredBy", ((object)npaModel.EnteredBy) ?? DBNull.Value);
                                            comForSACFollowUpMain.Parameters.AddWithValue("@EnteredDate", ((object)npaModel.EnteredOn) ?? DBNull.Value);
                                            comForSACFollowUpMain.Parameters.AddWithValue("@ModifiedBy", ((object)npaModel.EnteredBy) ?? DBNull.Value);
                                            comForSACFollowUpMain.Parameters.AddWithValue("@ModifiedDate", ((object)npaModel.EnteredOn) ?? DBNull.Value);
                                            insertedFollowUpMainIdFromSACFollowUpMain = (int)comForSACFollowUpMain.ExecuteScalar();
                                            comForSACFollowUpMain.Parameters.Clear();
                                            #endregion
                                            if (insertedFollowUpMainIdFromSACFollowUpMain > 0)
                                            {
                                                dynamic individualSACFollowUp = null;
                                                switch (item["FollowUpTypeId"].ToString())
                                                {
                                                    case "1":
                                                        individualSACFollowUp = item.ToObject<FollowUpDetailByTelephone>();
                                                        if (individualSACFollowUp != null)
                                                        {
                                                            #region Inserting into SAC FollowUp table(FollowUp by Telephone)
                                                            comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualSACFollowUpDetail.Connection = db;
                                                            comForIndividualSACFollowUpDetail.Transaction = tran;
                                                            comForIndividualSACFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailByTelephone]
                                                                                       ([FollowUpMainId]
                                                                                       ,[ContactDate]
                                                                                       ,[ResponseOfBorrower]
                                                                                       ,[NextActionToBeTaken]
                                                                                       ,[DueDateOfNextAction]) OUTPUT INSERTED.FollowUpMainId
                                                                                 VALUES
                                                                                       (@FollowUpMainId,
                                                                                        @ContactDate,
                                                                                        @ResponseOfBorrower,
                                                                                        @NextActionToBeTaken,
                                                                                        @DueDateOfNextAction)";
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromSACFollowUpMain);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ContactDate", item["ContactDate"].ToString() == "" ? DBNull.Value : ((object)item["ContactDate"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            insertedFollowUpMainIdFromSACFollowUpDetail = (int)comForIndividualSACFollowUpDetail.ExecuteScalar();
                                                            comForIndividualSACFollowUpDetail.Parameters.Clear();
                                                            if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                            {
                                                                lstOfValidSACDetailIds.Add(insertedFollowUpMainIdFromSACFollowUpDetail.ToString());
                                                            }
                                                            #endregion
                                                        }
                                                        break;
                                                    case "2":
                                                        individualSACFollowUp = item.ToObject<FollowUpDetailByInPersonVisit>();
                                                        if (individualSACFollowUp != null)
                                                        {
                                                            #region Inserting into SAC FollowUp table(FollowUp by In-Person Visit)
                                                            comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualSACFollowUpDetail.Connection = db;
                                                            comForIndividualSACFollowUpDetail.Transaction = tran;
                                                            comForIndividualSACFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailByInPersonVisit]
                                                                                                        ([FollowUpMainId]
                                                                                                        ,[VisitDate]
                                                                                                        ,[ResponseOfBorrower]
                                                                                                        ,[NextActionToBeTaken]
                                                                                                        ,[DueDateOfNextAction]) OUTPUT INSERTED.FollowUpMainId
                                                                                                    VALUES
                                                                                                        (@FollowUpMainId,
                                                                                                         @VisitDate,
                                                                                                         @ResponseOfBorrower,
                                                                                                         @NextActionToBeTaken,
                                                                                                         @DueDateOfNextAction)";
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromSACFollowUpMain);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@VisitDate", item["VisitDate"].ToString() == "" ? DBNull.Value : ((object)item["VisitDate"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            insertedFollowUpMainIdFromSACFollowUpDetail = (int)comForIndividualSACFollowUpDetail.ExecuteScalar();
                                                            comForIndividualSACFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualSACFollowUp.ScanCopyOfActionTakenFile) && individualSACFollowUp.ScanCopyOfActionTakenFile != "[]" && individualSACFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualSACFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}"));
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}/File.{individualSACFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidSACDetailIds.Add(insertedFollowUpMainIdFromSACFollowUpDetail.ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "3":
                                                        individualSACFollowUp = item.ToObject<FollowUpDetailFor15DaysLetter>();
                                                        if (individualSACFollowUp != null)
                                                        {
                                                            #region Inserting into SAC FollowUp table(FollowUp by 15 Days Letter)
                                                            comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualSACFollowUpDetail.Connection = db;
                                                            comForIndividualSACFollowUpDetail.Transaction = tran;
                                                            comForIndividualSACFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailFor15DaysLetter]
                                                                                                       ([FollowUpMainId]
                                                                                                       ,[DateOfIssuanceOfFollowUpLetter]
                                                                                                       ,[DateOfReceiptOfLetter]
                                                                                                       ,[ResponseOfBorrower]
                                                                                                       ,[NextActionToBeTaken]
                                                                                                       ,[DueDateOfNextAction]) OUTPUT INSERTED.FollowUpMainId
                                                                                                 VALUES
                                                                                                       (@FollowUpMainId,
                                                                                                        @DateOfIssuanceOfFollowUpLetter,
                                                                                                        @DateOfReceiptOfLetter,
                                                                                                        @ResponseOfBorrower,
                                                                                                        @NextActionToBeTaken,
                                                                                                        @DueDateOfNextAction)";
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromSACFollowUpMain);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfIssuanceOfFollowUpLetter", item["DateOfIssuanceOfFollowUpLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfIssuanceOfFollowUpLetter"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfReceiptOfLetter", item["DateOfReceiptOfLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfReceiptOfLetter"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            insertedFollowUpMainIdFromSACFollowUpDetail = (int)comForIndividualSACFollowUpDetail.ExecuteScalar();
                                                            comForIndividualSACFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualSACFollowUp.ScanCopyOfActionTakenFile) && individualSACFollowUp.ScanCopyOfActionTakenFile != "[]" && individualSACFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualSACFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}"));
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}/File.{individualSACFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidSACDetailIds.Add(insertedFollowUpMainIdFromSACFollowUpDetail.ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "4":
                                                        individualSACFollowUp = item.ToObject<FollowUpDetailForFirst7DaysLetter>();
                                                        if (individualSACFollowUp != null)
                                                        {
                                                            #region Inserting into SAC FollowUp table(FollowUp by First 7 Days Letter)
                                                            comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualSACFollowUpDetail.Connection = db;
                                                            comForIndividualSACFollowUpDetail.Transaction = tran;
                                                            comForIndividualSACFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailForFirst7DaysLetter]
                                                                                                        ([FollowUpMainId]
                                                                                                        ,[DateOfIssuanceOfFollowUpLetter]
                                                                                                        ,[DateOfReceiptOfLetter]
                                                                                                        ,[ResponseOfBorrower]
                                                                                                        ,[NextActionToBeTaken]
                                                                                                        ,[DueDateOfNextAction]) OUTPUT INSERTED.FollowUpMainId
                                                                                                    VALUES
                                                                                                        (@FollowUpMainId,
                                                                                                         @DateOfIssuanceOfFollowUpLetter,
                                                                                                         @DateOfReceiptOfLetter,
                                                                                                         @ResponseOfBorrower,
                                                                                                         @NextActionToBeTaken,
                                                                                                         @DueDateOfNextAction)";
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromSACFollowUpMain);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfIssuanceOfFollowUpLetter", item["DateOfIssuanceOfFollowUpLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfIssuanceOfFollowUpLetter"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfReceiptOfLetter", item["DateOfReceiptOfLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfReceiptOfLetter"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            insertedFollowUpMainIdFromSACFollowUpDetail = (int)comForIndividualSACFollowUpDetail.ExecuteScalar();
                                                            comForIndividualSACFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualSACFollowUp.ScanCopyOfActionTakenFile) && individualSACFollowUp.ScanCopyOfActionTakenFile != "[]" && individualSACFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualSACFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}"));
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}/File.{individualSACFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidSACDetailIds.Add(insertedFollowUpMainIdFromSACFollowUpDetail.ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "5":
                                                        individualSACFollowUp = item.ToObject<FollowUpDetailForSecond7DaysLetter>();
                                                        if (individualSACFollowUp != null)
                                                        {
                                                            #region Inserting into SAC FollowUp table(FollowUp by Second 7 Days Letter)
                                                            comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualSACFollowUpDetail.Connection = db;
                                                            comForIndividualSACFollowUpDetail.Transaction = tran;
                                                            comForIndividualSACFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailForSecond7DaysLetter]
                                                                                                       ([FollowUpMainId]
                                                                                                       ,[DateOfIssuanceOfFollowUpLetter]
                                                                                                       ,[DateOfReceiptOfLetter]
                                                                                                       ,[ResponseOfBorrower]
                                                                                                       ,[NextActionToBeTaken]
                                                                                                       ,[DueDateOfNextAction]) OUTPUT INSERTED.FollowUpMainId
                                                                                                 VALUES
                                                                                                       (@FollowUpMainId,
                                                                                                        @DateOfIssuanceOfFollowUpLetter,
                                                                                                        @DateOfReceiptOfLetter,
                                                                                                        @ResponseOfBorrower,
                                                                                                        @NextActionToBeTaken,
                                                                                                        @DueDateOfNextAction)";
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromSACFollowUpMain);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfIssuanceOfFollowUpLetter", item["DateOfIssuanceOfFollowUpLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfIssuanceOfFollowUpLetter"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfReceiptOfLetter", item["DateOfReceiptOfLetter"].ToString() == "" ? DBNull.Value : ((object)item["DateOfReceiptOfLetter"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            insertedFollowUpMainIdFromSACFollowUpDetail = (int)comForIndividualSACFollowUpDetail.ExecuteScalar();
                                                            comForIndividualSACFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualSACFollowUp.ScanCopyOfActionTakenFile) && individualSACFollowUp.ScanCopyOfActionTakenFile != "[]" && individualSACFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualSACFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}"));
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}/File.{individualSACFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidSACDetailIds.Add(insertedFollowUpMainIdFromSACFollowUpDetail.ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "6":
                                                        individualSACFollowUp = item.ToObject<FollowUpDetailFor35DaysCallNotice>();
                                                        if (individualSACFollowUp != null)
                                                        {
                                                            #region Inserting into SAC FollowUp table(FollowUp by 35 Days Call Notice)
                                                            comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualSACFollowUpDetail.Connection = db;
                                                            comForIndividualSACFollowUpDetail.Transaction = tran;
                                                            comForIndividualSACFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailFor35DaysCallNotice]
                                                                                                       ([FollowUpMainId]
                                                                                                       ,[DateOfNotice]
                                                                                                       ,[NewspaperName]
                                                                                                       ,[ResponseOfBorrower]
                                                                                                       ,[NextActionToBeTaken]
                                                                                                       ,[DueDateOfNextAction]
                                                                                                       ,[FollowUpResultId]
                                                                                                       ,[Remarks]) OUTPUT INSERTED.FollowUpMainId
                                                                                                 VALUES
                                                                                                       (@FollowUpMainId,
                                                                                                        @DateOfNotice,
                                                                                                        @NewspaperName,
                                                                                                        @ResponseOfBorrower,
                                                                                                        @NextActionToBeTaken,
                                                                                                        @DueDateOfNextAction,
                                                                                                        @FollowUpResultId,
                                                                                                        @Remarks)";
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromSACFollowUpMain);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfNotice", item["DateOfNotice"].ToString() == "" ? DBNull.Value : ((object)item["DateOfNotice"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NewspaperName", ((object)item["NewspaperName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpResultId", ((object)item["FollowUpResultId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@Remarks", ((object)item["Remarks"].ToString()) ?? DBNull.Value);
                                                            insertedFollowUpMainIdFromSACFollowUpDetail = (int)comForIndividualSACFollowUpDetail.ExecuteScalar();
                                                            comForIndividualSACFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualSACFollowUp.ScanCopyOfActionTakenFile) && individualSACFollowUp.ScanCopyOfActionTakenFile != "[]" && individualSACFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualSACFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}"));
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}/File.{individualSACFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidSACDetailIds.Add(insertedFollowUpMainIdFromSACFollowUpDetail.ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "7":
                                                        individualSACFollowUp = item.ToObject<FollowUpDetailForAuctionNotice>();
                                                        if (individualSACFollowUp != null)
                                                        {
                                                            #region Inserting into SAC FollowUp table(FollowUp by Auction Notice)
                                                            comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualSACFollowUpDetail.Connection = db;
                                                            comForIndividualSACFollowUpDetail.Transaction = tran;
                                                            comForIndividualSACFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailForAuctionNotice]
                                                                                                       ([FollowUpMainId]
                                                                                                       ,[AuctionNoticeTypeId]
                                                                                                       ,[DateOfAuctionNotice]
                                                                                                       ,[AuctionNewspaperName]
                                                                                                       ,[AuctionResponseOfBorrower]
                                                                                                       ,[AuctionNextActionToBeTaken]
                                                                                                       ,[AuctionDueDateOfNextAction]
                                                                                                       ,[AuctionRemarks]
                                                                                                       ,[DateOfRevaluationForAuction]
                                                                                                       ,[RevaluatedFMVOfCollateralForAuction]) OUTPUT INSERTED.FollowUpMainId
                                                                                                 VALUES
                                                                                                       (@FollowUpMainId,
                                                                                                        @AuctionNoticeTypeId,
                                                                                                        @DateOfAuctionNotice,
                                                                                                        @AuctionNewspaperName,
                                                                                                        @AuctionResponseOfBorrower,
                                                                                                        @AuctionNextActionToBeTaken,
                                                                                                        @AuctionDueDateOfNextAction,
                                                                                                        @AuctionRemarks,
                                                                                                        @DateOfRevaluationForAuction,
                                                                                                        @RevaluatedFMVOfCollateralForAuction)";
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromSACFollowUpMain);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@AuctionNoticeTypeId", ((object)item["AuctionNoticeTypeId"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfAuctionNotice", item["DateOfAuctionNotice"].ToString() == "" ? DBNull.Value : ((object)item["DateOfAuctionNotice"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@AuctionNewspaperName", ((object)item["AuctionNewspaperName"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@AuctionResponseOfBorrower", ((object)item["AuctionResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@AuctionNextActionToBeTaken", ((object)item["AuctionNextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@AuctionDueDateOfNextAction", item["AuctionDueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["AuctionDueDateOfNextAction"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@AuctionRemarks", ((object)item["AuctionRemarks"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DateOfRevaluationForAuction", item["DateOfRevaluationForAuction"].ToString() == "" ? DBNull.Value : ((object)item["DateOfRevaluationForAuction"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@RevaluatedFMVOfCollateralForAuction", string.IsNullOrEmpty(item["RevaluatedFMVOfCollateralForAuction"].ToString()) ? DBNull.Value : (((object)item["RevaluatedFMVOfCollateralForAuction"].ToString().Replace(",", "")) ?? DBNull.Value));
                                                            insertedFollowUpMainIdFromSACFollowUpDetail = (int)comForIndividualSACFollowUpDetail.ExecuteScalar();
                                                            comForIndividualSACFollowUpDetail.Parameters.Clear();

                                                            #region Come Save Uploaded Files Here
                                                            if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                            {
                                                                //File saving part here
                                                                if (!string.IsNullOrEmpty(individualSACFollowUp.ScanCopyOfActionTakenFile) && individualSACFollowUp.ScanCopyOfActionTakenFile != "[]" && individualSACFollowUp.ScanCopyOfActionTakenFile != "{}")
                                                                {
                                                                    //Convert Base64 Encoded string to Byte Array.
                                                                    byte[] fileBytes = Convert.FromBase64String(individualSACFollowUp.ScanCopyOfActionTakenFile);//Save File
                                                                    if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}")))
                                                                    {
                                                                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}"));
                                                                    }
                                                                    File.WriteAllBytes(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/{insertedFollowUpMainIdFromSACFollowUpDetail}/File.{individualSACFollowUp.ScanCopyOfActionTakenFileType}"), fileBytes);
                                                                }
                                                                lstOfValidSACDetailIds.Add(insertedFollowUpMainIdFromSACFollowUpDetail.ToString());
                                                            }
                                                            #endregion

                                                            #endregion
                                                        }
                                                        break;
                                                    case "8":
                                                        individualSACFollowUp = item.ToObject<FollowUpDetailForOthers>();
                                                        if (individualSACFollowUp != null)
                                                        {
                                                            #region Inserting into SAC FollowUp table(FollowUp for Others)
                                                            comForIndividualSACFollowUpDetail.CommandType = CommandType.Text;
                                                            comForIndividualSACFollowUpDetail.Connection = db;
                                                            comForIndividualSACFollowUpDetail.Transaction = tran;
                                                            comForIndividualSACFollowUpDetail.CommandText = @"INSERT INTO [dbo].[FollowUpDetailForOthers]
                                                                                       ([FollowUpMainId]
                                                                                       ,[ContactDate]
                                                                                       ,[ResponseOfBorrower]
                                                                                       ,[NextActionToBeTaken]
                                                                                       ,[DueDateOfNextAction]
                                                                                       ,[Remarks]) OUTPUT INSERTED.FollowUpMainId
                                                                                 VALUES
                                                                                       (@FollowUpMainId,
                                                                                        @ContactDate,
                                                                                        @ResponseOfBorrower,
                                                                                        @NextActionToBeTaken,
                                                                                        @DueDateOfNextAction,
                                                                                        @Remarks)";
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@FollowUpMainId", insertedFollowUpMainIdFromSACFollowUpMain);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ContactDate", item["ContactDate"].ToString() == "" ? DBNull.Value : ((object)item["ContactDate"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@ResponseOfBorrower", ((object)item["ResponseOfBorrower"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@NextActionToBeTaken", ((object)item["NextActionToBeTaken"].ToString()) ?? DBNull.Value);
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@DueDateOfNextAction", item["DueDateOfNextAction"].ToString() == "" ? DBNull.Value : ((object)item["DueDateOfNextAction"].ToString()));
                                                            comForIndividualSACFollowUpDetail.Parameters.AddWithValue("@Remarks", ((object)item["Remarks"].ToString()) ?? DBNull.Value);
                                                            insertedFollowUpMainIdFromSACFollowUpDetail = (int)comForIndividualSACFollowUpDetail.ExecuteScalar();
                                                            comForIndividualSACFollowUpDetail.Parameters.Clear();
                                                            if (insertedFollowUpMainIdFromSACFollowUpDetail > 0)
                                                            {
                                                                lstOfValidSACDetailIds.Add(insertedFollowUpMainIdFromSACFollowUpDetail.ToString());
                                                            }
                                                            #endregion
                                                        }
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                    #region Delete removed followups from previously saved list
                                    DeleteRemovedFollowUps("2", npaModel.NPAManagementId, lstOfValidSACDetailIds, db, tran);
                                    DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/"));
                                    if (Directory.Exists(HttpContext.Current.Server.MapPath($"~/UploadedFiles/NPAID-{npaModel.NPAManagementId}/FLWTYPE-2/")))
                                    {
                                        foreach (DirectoryInfo dir in di.GetDirectories())
                                        {
                                            if (!lstOfValidSACDetailIds.Contains(dir.Name))
                                            {
                                                dir.Delete(true);
                                            }
                                        }
                                    }
                                    #endregion

                                    #endregion

                                    #endregion

                                }
                                #endregion

                                #endregion

                                i = updatedNPAMgmtId;
                                tran.Commit();
                                //tran.Rollback();//For now in testing
                            }
                            #endregion

                            #endregion
                        }
                    }
                }
                catch (SqlException sqlex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(sqlex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    logger.Error($"{sqlex.Message} :: Line Number: {line.ToString()}");
                    tran.Rollback();
                }
                catch (Exception ex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                    tran.Rollback();
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                    {
                        db.Close();
                    }
                }
            }
            return i;
        }

        public int DeleteRemovedFollowUps(string followUpById, int? npaMgmtId, List<string> validFollowUpMainIds, SqlConnection db, SqlTransaction tran)
        {
            //SqlConnection db = new SqlConnection(connectionString);
            SqlCommand com = new SqlCommand();
            //SqlTransaction tran;

            //tran = db.BeginTransaction();
            int i = 0;
            if (validFollowUpMainIds.Count() > 0)
            {
                try
                {
                    //Run all your insert statements here here
                    com.CommandType = CommandType.Text;
                    com.Connection = db;
                    com.Transaction = tran;
                    com.CommandText = $@"DELETE FROM [dbo].[FollowUpMain]
                                        WHERE NPAManagementId={npaMgmtId} and FollowUpById={followUpById} and FollowUpMainId not in (" + String.Join(",", validFollowUpMainIds) + ")";
                    int deleted = com.ExecuteNonQuery();
                    if (deleted > 0)
                    {
                        i = deleted;
                        //tran.Commit();
                    }
                }
                catch (SqlException sqlex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(sqlex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    logger.Error($"{sqlex.Message} :: Line Number: {line.ToString()}");
                    //tran.Rollback();
                }
                catch (Exception ex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                    //tran.Rollback();
                }
                finally
                {

                }
            }

            return i;
        }

        public int DeleteDataFromTableWithWhere(string tableNameToDeleteFrom, string idTitleToDelete, string idValueToDelete, SqlConnection db, SqlTransaction tran)
        {
            //SqlConnection db = new SqlConnection(connectionString);
            SqlCommand com = new SqlCommand();
            //SqlTransaction tran;

            //tran = db.BeginTransaction();
            int i = 0;
            if (!string.IsNullOrEmpty(idValueToDelete))
            {
                try
                {
                    //Run all your insert statements here here
                    com.CommandType = CommandType.Text;
                    com.Connection = db;
                    com.Transaction = tran;
                    com.CommandText = $@"DELETE FROM [dbo].[{tableNameToDeleteFrom}]
                                        WHERE {idTitleToDelete}={idValueToDelete.Trim()}";
                    int deleted = com.ExecuteNonQuery();
                    if (deleted > 0)
                    {
                        i = deleted;
                        //tran.Commit();
                    }
                }
                catch (SqlException sqlex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(sqlex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    logger.Error($"{sqlex.Message} :: Line Number: {line.ToString()}");
                    //tran.Rollback();
                }
                catch (Exception ex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                    //tran.Rollback();
                }
                finally
                {

                }
            }
            return i;
        }

        public NPAManagementMain GetSavedNPADetail(int npaManagementId)
        {
            NPAManagementMain savedNPA = new NPAManagementMain();
            string commandText = $@"SELECT * FROM NPAMANAGEMENTMAIN with(nolock) where NPAManagementId = {npaManagementId}";
            //if (!isHRAdmin)
            //{

            //    commandText += " WHERE RTRIM(LTRIM(LOWER(ADDEDBY)))=RTRIM(LTRIM(LOWER('" + username + "')))";
            //}
            using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, commandText,
                CommandType.Text))
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        try
                        {
                            savedNPA = new NPAManagementMain()
                            {
                                NPAManagementId = (int)reader["NPAManagementId"],
                                CIFNo = reader["CIFNo"].ToString(),
                                GroupName = reader["GroupName"].ToString(),
                                BorrowerName = reader["BorrowerName"].ToString(),
                                BorrowerContactNumber = reader["BorrowerContactNumber"].ToString(),
                                BorrowerEmailAddress = reader["BorrowerEmailAddress"].ToString(),
                                BorrowerPermanentAddressProvinceId = reader["BorrowerPermanentAddressProvinceId"].ToString(),
                                BorrowerPermanentAddressZoneId = reader["BorrowerPermanentAddressZoneId"].ToString(),
                                BorrowerPermanentAddressDistrictId = reader["BorrowerPermanentAddressDistrictId"].ToString(),
                                BorrowerPermanentAddressVDCMun = reader["BorrowerPermanentAddressVDCMun"].ToString(),
                                BorrowerPermanentToleStreetName = reader["BorrowerPermanentToleStreetName"].ToString(),
                                BorrowerPermanentHouseNumber = reader["BorrowerPermanentHouseNumber"].ToString(),
                                TempAddSameAsPerAdd = Convert.ToBoolean(reader["TempAddSameAsPerAdd"]),
                                BorrowerTemporaryAddressProvinceId = reader["BorrowerTemporaryAddressProvinceId"].ToString(),
                                BorrowerTemporaryAddressZoneId = reader["BorrowerTemporaryAddressZoneId"].ToString(),
                                BorrowerTemporaryAddressDistrictId = reader["BorrowerTemporaryAddressDistrictId"].ToString(),
                                BorrowerTemporaryAddressVDCMun = reader["BorrowerTemporaryAddressVDCMun"].ToString(),
                                BorrowerTemporaryToleStreetName = reader["BorrowerTemporaryToleStreetName"].ToString(),
                                BorrowerTemporaryHouseNumber = reader["BorrowerTemporaryHouseNumber"].ToString(),
                                BorrowerTypeId = (int)reader["BorrowerTypeId"],
                                StringifiedBorrowerTypeIndividualLegalDetail = GetSavedIndividualLegalDetail((int)reader["NPAManagementId"], (int)reader["BorrowerTypeId"]),
                                StringifiedListOfPrimaryCollateralDetailObjects = GetSavedPrimaryCollateralDetail((int)reader["NPAManagementId"]),
                                StringifiedListOfSecondaryCollateralDetailObjects = GetSavedSecondaryCollateralDetail((int)reader["NPAManagementId"]),
                                StringifiedListOfLoanAccountDetailObjects = GetSavedLoanAccountDetail((int)reader["NPAManagementId"]),
                                StringifiedListOfBranchFollowUpDetailObjects = GetSavedBranchFollowUpDetail((int)reader["NPAManagementId"]),
                                StringifiedListOfSACFollowUpDetailObjects = GetSavedSACFollowUpDetail((int)reader["NPAManagementId"]),
                                CurrentROName = reader["CurrentROName"].ToString(),
                                BranchLocationCode = reader["BranchLocationCode"].ToString(),
                                SACROName = reader["SACROName"].ToString(),
                                FileTransferDateToSAC = reader["FileTransferDateToSAC"] != DBNull.Value ? Convert.ToDateTime(reader["FileTransferDateToSAC"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                ProvisionStatusIdOnTheDateOfTransfer = reader["ProvisionStatusIdOnTheDateOfTransfer"] != DBNull.Value ? (int)reader["ProvisionStatusIdOnTheDateOfTransfer"] : (int?)null,
                                DateOfRequestForBlacklisting = reader["DateOfRequestForBlacklisting"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfRequestForBlacklisting"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                BlacklistStatusOfRequest = reader["BlacklistStatusOfRequest"].ToString(),
                                BlacklistDate = reader["BlacklistDate"] != DBNull.Value ? Convert.ToDateTime(reader["BlacklistDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                BlacklistNumber = reader["BlacklistNumber"].ToString(),
                                DateOfInitiationForBookingNBA = reader["DateOfInitiationForBookingNBA"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfInitiationForBookingNBA"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                NBAStatusOfRequestId = reader["NBAStatusOfRequestId"] != DBNull.Value ? Convert.ToInt32(reader["NBAStatusOfRequestId"]) : (Nullable<int>)null,
                                NBADateOfCompletion = reader["NBADateOfCompletion"] != DBNull.Value ? Convert.ToDateTime(reader["NBADateOfCompletion"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                NBAAmount = reader["NBAAmount"] != DBNull.Value ? Convert.ToDecimal(reader["NBAAmount"]) : (Nullable<decimal>)null,
                                NBARemarks = reader["NBARemarks"].ToString(),
                                DateOfFileHandedOverToLegalForDRT = reader["DateOfFileHandedOverToLegalForDRT"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfFileHandedOverToLegalForDRT"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                DateOfFilingACaseInDRT = reader["DateOfFilingACaseInDRT"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfFilingACaseInDRT"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                DRTStatusOfRequestId = reader["DRTStatusOfRequestId"] != DBNull.Value ? Convert.ToInt32(reader["DRTStatusOfRequestId"]) : (Nullable<int>)null,
                                DRTDateOfCompletion = reader["DRTDateOfCompletion"] != DBNull.Value ? Convert.ToDateTime(reader["DRTDateOfCompletion"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                DRTAmount = reader["DRTAmount"] != DBNull.Value ? Convert.ToDecimal(reader["DRTAmount"]) : (Nullable<decimal>)null,
                                DRTRemarks = reader["DRTRemarks"].ToString(),
                                DateOfInitiationForWriteOff = reader["DateOfInitiationForWriteOff"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfInitiationForWriteOff"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                WriteOffStatusOfRequestId = reader["WriteOffStatusOfRequestId"] != DBNull.Value ? Convert.ToInt32(reader["WriteOffStatusOfRequestId"]) : (Nullable<int>)null,
                                WriteOffDateOfCompletion = reader["WriteOffDateOfCompletion"] != DBNull.Value ? Convert.ToDateTime(reader["WriteOffDateOfCompletion"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                PrincipalOnWriteOffDate = reader["PrincipalOnWriteOffDate"] != DBNull.Value ? Convert.ToDecimal(reader["PrincipalOnWriteOffDate"]) : (Nullable<decimal>)null,
                                InterestOnWriteOffDate = reader["InterestOnWriteOffDate"] != DBNull.Value ? Convert.ToDecimal(reader["InterestOnWriteOffDate"]) : (Nullable<decimal>)null,
                                PrincipalRecoveredFromStartDateToLoanWriteOffDate = reader["PrincipalRecoveredFromStartDateToLoanWriteOffDate"] != DBNull.Value ? Convert.ToDecimal(reader["PrincipalRecoveredFromStartDateToLoanWriteOffDate"]) : (Nullable<decimal>)null,
                                InterestRecoveredFromStartDateToLoanWriteOffDate = reader["InterestRecoveredFromStartDateToLoanWriteOffDate"] != DBNull.Value ? Convert.ToDecimal(reader["InterestRecoveredFromStartDateToLoanWriteOffDate"]) : (Nullable<decimal>)null,
                                InterestOnInterestRecoveredFromStartDateToLoanWriteOffDate = reader["InterestOnInterestRecoveredFromStartDateToLoanWriteOffDate"] != DBNull.Value ? Convert.ToDecimal(reader["InterestOnInterestRecoveredFromStartDateToLoanWriteOffDate"]) : (Nullable<decimal>)null,
                                PenalAmountRecoveredFromStartDateToLoanWriteOffDate = reader["PenalAmountRecoveredFromStartDateToLoanWriteOffDate"] != DBNull.Value ? Convert.ToDecimal(reader["PenalAmountRecoveredFromStartDateToLoanWriteOffDate"]) : (Nullable<decimal>)null,
                                WriteOffNextActionToBeTaken = reader["WriteOffNextActionToBeTaken"].ToString(),
                                WriteOffDueDateOfNextAction = reader["WriteOffDueDateOfNextAction"] != DBNull.Value ? Convert.ToDateTime(reader["WriteOffDueDateOfNextAction"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                WriteOffRemarks = reader["WriteOffRemarks"].ToString(),
                                EnteredBy = reader["EnteredBy"].ToString(),
                                EnteredOn = reader["EnteredOn"].ToString()
                            };
                        }
                        catch (Exception ex)
                        {
                            // Get stack trace for the exception with source file information
                            var st = new StackTrace(ex, true);
                            // Get the top stack frame
                            var frame = st.GetFrame(0);
                            // Get the line number from the stack frame
                            var line = frame.GetFileLineNumber();
                            logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                            throw ex;
                        }
                    }
                }
            }
            return savedNPA;
        }

        private string GetSavedIndividualLegalDetail(int npaManagementId, int borrowerId)
        {
            string borrowerDetail = string.Empty;
            string borrowerDetailTableName = borrowerId == 1 ? "BorrowerTypeIndividualDetail" : (borrowerId == 2 ? "BorrowerTypeLegalEntitiesDetail" : string.Empty);
            string commandText = $@"SELECT * FROM {borrowerDetailTableName} with(nolock) where NPAManagementId = {npaManagementId}";
            if (!string.IsNullOrEmpty(borrowerDetailTableName))
            {
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, commandText,
                CommandType.Text))
                {
                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            try
                            {
                                if (borrowerId == 1)
                                {
                                    borrowerDetail = JsonConvert.SerializeObject(new BorrowerTypeIndividualDetail()
                                    {
                                        NPAManagementId = (int)reader["NPAManagementId"],
                                        BorrowerTypeId = (int)reader["BorrowerTypeId"],
                                        BorrowerFatherName = reader["BorrowerFatherName"].ToString(),
                                        BorrowerGrandfatherName = reader["BorrowerGrandfatherName"].ToString(),
                                        BorrowerCitizenshipNumber = reader["BorrowerCitizenshipNumber"].ToString(),
                                        BorrowerSpouseName = reader["BorrowerSpouseName"].ToString(),
                                        BorrowerSonName = reader["BorrowerSonName"].ToString(),
                                        BorrowerDaughterName = reader["BorrowerDaughterName"].ToString()
                                    });
                                }
                                else if (borrowerId == 2)
                                {
                                    borrowerDetail = JsonConvert.SerializeObject(new BorrowerTypeLegalEntitiesDetail()
                                    {
                                        NPAManagementId = (int)reader["NPAManagementId"],
                                        BorrowerTypeId = (int)reader["BorrowerTypeId"],
                                        LegalEntityRegistrationNumber = reader["LegalEntityRegistrationNumber"].ToString(),
                                        LegalEntityRegistrationDate = reader["LegalEntityRegistrationDate"].ToString(),// != DBNull.Value ? Convert.ToDateTime(reader["LegalEntityRegistrationDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                        LegalEntityRegisteredOffice = reader["LegalEntityRegisteredOffice"].ToString()
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                // Get stack trace for the exception with source file information
                                var st = new StackTrace(ex, true);
                                // Get the top stack frame
                                var frame = st.GetFrame(0);
                                // Get the line number from the stack frame
                                var line = frame.GetFileLineNumber();
                                logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                throw ex;
                            }
                        }
                    }
                }
            }
            return borrowerDetail == string.Empty ? "{}" : borrowerDetail;
        }

        private string GetSavedPrimaryCollateralDetail(int npaManagementId)
        {
            //Get CollateralDetailMainId and CollateralTypeId from NPAMgmtMainId
            string primaryCollateralsDetail = string.Empty;
            int collateralDetailMainId = 0;
            int collateralTypeId = 0;
            string commandText = $@"SELECT * FROM CollateralDetailMain with(nolock) where NPAManagementId = {npaManagementId} and CollateralClass=1";
            using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, commandText,
            CommandType.Text))
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        try
                        {
                            collateralDetailMainId = (int)reader["CollateralDetailMainId"];
                            collateralTypeId = (int)reader["CollateralTypeId"];
                            if (collateralDetailMainId != 0 && collateralTypeId != 0)
                            {
                                #region Get Collateral Details
                                string commandText2 = string.Empty;
                                switch (collateralTypeId)
                                {
                                    case 1:
                                        commandText2 = $@"SELECT * FROM CollateralDetailInCaseOfLand with(nolock) where CollateralDetailMainId = {collateralDetailMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        CollateralDetailInCaseOfLand collateralLand = new CollateralDetailInCaseOfLand()
                                                        {
                                                            CollateralTypeId = collateralTypeId,
                                                            ProvinceId = reader2["ProvinceId"].ToString(),
                                                            ZoneId = reader2["ZoneId"].ToString(),
                                                            DistrictId = reader2["DistrictId"].ToString(),
                                                            VDCMun = reader2["VDCMun"].ToString(),
                                                            Street = reader2["Street"].ToString(),
                                                            WardNumber = reader2["WardNumber"].ToString(),
                                                            PlotNumber = reader2["PlotNumber"].ToString(),
                                                            AreaTypeId = reader2["AreaTypeId"] == DBNull.Value ? 0 : (int?)reader2["AreaTypeId"],
                                                            Area = reader2["Area"].ToString(),
                                                            PropertyOwner = reader2["PropertyOwner"].ToString(),
                                                            RelationWithBorrower = reader2["RelationWithBorrower"].ToString(),
                                                            FirstValuatorName = reader2["FirstValuatorName"].ToString(),
                                                            FirstValuationDate = reader2["FirstValuationDate"] != DBNull.Value ? Convert.ToDateTime(reader2["FirstValuationDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            FirstFMVOfProperty = reader2["FirstFMVOfProperty"] == DBNull.Value ? 0 : (decimal?)reader2["FirstFMVOfProperty"],
                                                            FirstValuatorExistsInSBLCurrentlyYN = reader2["FirstValuatorExistsInSBLCurrentlyYN"].ToString(),
                                                            LatestValuatorName = reader2["LatestValuatorName"].ToString(),
                                                            LatestValuationDate = reader2["LatestValuationDate"] != DBNull.Value ? Convert.ToDateTime(reader2["LatestValuationDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            LatestValuatorExistsInSBLCurrentlyYN = reader2["LatestValuatorExistsInSBLCurrentlyYN"].ToString(),
                                                            FMVOfPropertyAsPerLatestValuationReport = reader2["FMVOfPropertyAsPerLatestValuationReport"] == DBNull.Value ? 0 : (decimal?)reader2["FMVOfPropertyAsPerLatestValuationReport"],
                                                            InsuranceCoverageType = reader2["InsuranceCoverageType"].ToString(),
                                                            InsuranceExpiryDate = reader2["InsuranceExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader2["InsuranceExpiryDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            InsuranceAmount = reader2["InsuranceAmount"] == DBNull.Value ? 0 : (decimal?)reader2["InsuranceAmount"]
                                                        };
                                                        primaryCollateralsDetail = primaryCollateralsDetail + (!string.IsNullOrEmpty(primaryCollateralsDetail) ? "," : "") + JsonConvert.SerializeObject(collateralLand);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 2:
                                        commandText2 = $@"SELECT * FROM CollateralDetailInCaseOfBuilding with(nolock) where CollateralDetailMainId = {collateralDetailMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        CollateralDetailInCaseOfBuilding collateralBuilding = new CollateralDetailInCaseOfBuilding()
                                                        {
                                                            CollateralTypeId = collateralTypeId,
                                                            ProvinceId = reader2["ProvinceId"].ToString(),
                                                            ZoneId = reader2["ZoneId"].ToString(),
                                                            DistrictId = reader2["DistrictId"].ToString(),
                                                            VDCMun = reader2["VDCMun"].ToString(),
                                                            Street = reader2["Street"].ToString(),
                                                            WardNumber = reader2["WardNumber"].ToString(),
                                                            PlotNumber = reader2["PlotNumber"].ToString(),
                                                            AreaTypeId = reader2["AreaTypeId"] == DBNull.Value ? 0 : (int?)reader2["AreaTypeId"],
                                                            AreaOfLand = reader2["AreaOfLand"].ToString(),
                                                            AreaOfBuilding = reader2["AreaOfBuilding"].ToString(),
                                                            PropertyOwner = reader2["PropertyOwner"].ToString(),
                                                            RelationWithBorrower = reader2["RelationWithBorrower"].ToString(),
                                                            ConstructionCompletionDate = reader2["ConstructionCompletionDate"] != DBNull.Value ? Convert.ToDateTime(reader2["ConstructionCompletionDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            FirstValuatorName = reader2["FirstValuatorName"].ToString(),
                                                            FirstValuationDate = reader2["FirstValuationDate"] != DBNull.Value ? Convert.ToDateTime(reader2["FirstValuationDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            FirstFMVOfProperty = reader2["FirstFMVOfProperty"] == DBNull.Value ? 0 : (decimal?)reader2["FirstFMVOfProperty"],
                                                            FirstValuatorExistsInSBLCurrentlyYN = reader2["FirstValuatorExistsInSBLCurrentlyYN"].ToString(),
                                                            LatestValuatorName = reader2["LatestValuatorName"].ToString(),
                                                            LatestValuationDate = reader2["LatestValuationDate"] != DBNull.Value ? Convert.ToDateTime(reader2["LatestValuationDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            LatestValuatorExistsInSBLCurrentlyYN = reader2["LatestValuatorExistsInSBLCurrentlyYN"].ToString(),
                                                            FMVOfPropertyAsPerLatestValuationReport = reader2["FMVOfPropertyAsPerLatestValuationReport"] == DBNull.Value ? 0 : (decimal?)reader2["FMVOfPropertyAsPerLatestValuationReport"],
                                                            InsuranceCoverageType = reader2["InsuranceCoverageType"].ToString(),
                                                            InsuranceExpiryDate = reader2["InsuranceExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader2["InsuranceExpiryDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            InsuranceAmount = reader2["InsuranceAmount"] == DBNull.Value ? 0 : (decimal?)reader2["InsuranceAmount"]
                                                        };
                                                        primaryCollateralsDetail = primaryCollateralsDetail + (!string.IsNullOrEmpty(primaryCollateralsDetail) ? "," : "") + JsonConvert.SerializeObject(collateralBuilding);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 3:
                                        commandText2 = $@"SELECT * FROM CollateralDetailInCaseOfVehicle with(nolock) where CollateralDetailMainId = {collateralDetailMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        CollateralDetailInCaseOfVehicle collateralVehicle = new CollateralDetailInCaseOfVehicle()
                                                        {
                                                            CollateralTypeId = collateralTypeId,
                                                            VehicleTypeId = (int?)reader2["VehicleTypeId"],
                                                            VehicleModel = reader2["VehicleModel"].ToString(),
                                                            EngineNumber = reader2["EngineNumber"].ToString(),
                                                            ChassisNumber = reader2["ChassisNumber"].ToString(),
                                                            VehicleRegistrationNumber = reader2["VehicleRegistrationNumber"].ToString(),
                                                            VehicleRegistrationDate = reader2["VehicleRegistrationDate"] != DBNull.Value ? Convert.ToDateTime(reader2["VehicleRegistrationDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            VehicleRegisteredOffice = reader2["VehicleRegisteredOffice"].ToString(),
                                                            VehicleMakeYear = reader2["VehicleMakeYear"].ToString(),
                                                            FirstValuatorName = reader2["FirstValuatorName"].ToString(),
                                                            FirstValuationDate = reader2["FirstValuationDate"] != DBNull.Value ? Convert.ToDateTime(reader2["FirstValuationDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            FirstFMVOfProperty = reader2["FirstFMVOfProperty"] == DBNull.Value ? 0 : (decimal?)reader2["FirstFMVOfProperty"],
                                                            FirstValuatorExistsInSBLCurrentlyYN = reader2["FirstValuatorExistsInSBLCurrentlyYN"].ToString(),
                                                            LatestValuatorName = reader2["LatestValuatorName"].ToString(),
                                                            LatestValuationDate = reader2["LatestValuationDate"] != DBNull.Value ? Convert.ToDateTime(reader2["LatestValuationDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            LatestValuatorExistsInSBLCurrentlyYN = reader2["LatestValuatorExistsInSBLCurrentlyYN"].ToString(),
                                                            ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice = reader2["ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice"] == DBNull.Value ? 0 : (decimal?)reader2["ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice"],
                                                            InsuranceCoverageType = reader2["InsuranceCoverageType"].ToString(),
                                                            InsuranceExpiryDate = reader2["InsuranceExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader2["InsuranceExpiryDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            InsuranceAmount = reader2["InsuranceAmount"] == DBNull.Value ? 0 : (decimal?)reader2["InsuranceAmount"]
                                                        };
                                                        primaryCollateralsDetail = primaryCollateralsDetail + (!string.IsNullOrEmpty(primaryCollateralsDetail) ? "," : "") + JsonConvert.SerializeObject(collateralVehicle);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 4:
                                        commandText2 = $@"SELECT * FROM CollateralDetailInCaseOfPlantAndMachinery with(nolock) where CollateralDetailMainId = {collateralDetailMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        CollateralDetailInCaseOfPlantAndMachinery collateralPAM = new CollateralDetailInCaseOfPlantAndMachinery()
                                                        {
                                                            CollateralTypeId = collateralTypeId,
                                                            PlantAndMachineryModel = reader2["PlantAndMachineryModel"].ToString(),
                                                            EngineNumber = reader2["EngineNumber"].ToString(),
                                                            ChassisNumber = reader2["ChassisNumber"].ToString(),
                                                            RegistrationNumber = reader2["RegistrationNumber"].ToString(),
                                                            FirstValuatorName = reader2["FirstValuatorName"].ToString(),
                                                            FirstValuationDate = reader2["FirstValuationDate"] != DBNull.Value ? Convert.ToDateTime(reader2["FirstValuationDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            FirstFMVOfProperty = reader2["FirstFMVOfProperty"] == DBNull.Value ? 0 : (decimal?)reader2["FirstFMVOfProperty"],
                                                            FirstValuatorExistsInSBLCurrentlyYN = reader2["FirstValuatorExistsInSBLCurrentlyYN"].ToString(),
                                                            LatestValuatorName = reader2["LatestValuatorName"].ToString(),
                                                            LatestValuationDate = reader2["LatestValuationDate"] != DBNull.Value ? Convert.ToDateTime(reader2["LatestValuationDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            LatestValuatorExistsInSBLCurrentlyYN = reader2["LatestValuatorExistsInSBLCurrentlyYN"].ToString(),
                                                            FMVOfPropertyAsPerLatestValuationReport = reader2["FMVOfPropertyAsPerLatestValuationReport"] == DBNull.Value ? 0 : (decimal?)reader2["FMVOfPropertyAsPerLatestValuationReport"],
                                                            InsuranceCoverageType = reader2["InsuranceCoverageType"].ToString(),
                                                            InsuranceExpiryDate = reader2["InsuranceExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader2["InsuranceExpiryDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            InsuranceAmount = reader2["InsuranceAmount"] == DBNull.Value ? 0 : (decimal?)reader2["InsuranceAmount"]
                                                        };
                                                        primaryCollateralsDetail = primaryCollateralsDetail + (!string.IsNullOrEmpty(primaryCollateralsDetail) ? "," : "") + JsonConvert.SerializeObject(collateralPAM);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 5:
                                        commandText2 = $@"SELECT * FROM CollateralDetailInCaseOfCurrentAsset with(nolock) where CollateralDetailMainId = {collateralDetailMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        CollateralDetailInCaseOfCurrentAsset collateralCurrentAsset = new CollateralDetailInCaseOfCurrentAsset()
                                                        {
                                                            CollateralTypeId = collateralTypeId,
                                                            DateOfLatestStockInspectionReportCollectedByBranch = reader2["DateOfLatestStockInspectionReportCollectedByBranch"] != DBNull.Value ? Convert.ToDateTime(reader2["DateOfLatestStockInspectionReportCollectedByBranch"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            WorkingCapitalAsPerStockReport = reader2["WorkingCapitalAsPerStockReport"] == DBNull.Value ? 0 : (decimal?)reader2["WorkingCapitalAsPerStockReport"],
                                                            WorkingCapitalAsPerStockInspector = reader2["WorkingCapitalAsPerStockInspector"] == DBNull.Value ? 0 : (decimal?)reader2["WorkingCapitalAsPerStockInspector"],
                                                            DrawingPower = reader2["DrawingPower"].ToString(),
                                                            InsuranceCoverageType = reader2["InsuranceCoverageType"].ToString(),
                                                            InsuranceExpiryDate = reader2["InsuranceExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader2["InsuranceExpiryDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            InsuranceAmount = reader2["InsuranceAmount"] == DBNull.Value ? 0 : (decimal?)reader2["InsuranceAmount"]
                                                        };
                                                        primaryCollateralsDetail = primaryCollateralsDetail + (!string.IsNullOrEmpty(primaryCollateralsDetail) ? "," : "") + JsonConvert.SerializeObject(collateralCurrentAsset);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 6:
                                        commandText2 = $@"SELECT * FROM CollateralDetailInCaseOfPersonalGuarantee with(nolock) where CollateralDetailMainId = {collateralDetailMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        CollateralDetailInCaseOfPersonalGuarantee collateralPG = new CollateralDetailInCaseOfPersonalGuarantee()
                                                        {
                                                            CollateralTypeId = collateralTypeId,
                                                            NameOfGuarantor = reader2["NameOfGuarantor"].ToString(),
                                                            CitizenshipNumber = reader2["CitizenshipNumber"].ToString(),
                                                            FatherName = reader2["FatherName"].ToString(),
                                                            GrandfatherName = reader2["GrandfatherName"].ToString(),
                                                            PermanentAddress = reader2["PermanentAddress"].ToString(),
                                                            CurrentAddress = reader2["CurrentAddress"].ToString(),
                                                            NetworthOfGuarantor = reader2["NetworthOfGuarantor"] == DBNull.Value ? 0 : (decimal?)reader2["NetworthOfGuarantor"],
                                                            RelationWithBorrower = reader2["RelationWithBorrower"].ToString(),
                                                            ContactNumber = reader2["ContactNumber"].ToString(),
                                                            Profession = reader2["Profession"].ToString()
                                                        };
                                                        primaryCollateralsDetail = primaryCollateralsDetail + (!string.IsNullOrEmpty(primaryCollateralsDetail) ? "," : "") + JsonConvert.SerializeObject(collateralPG);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 7:
                                        commandText2 = $@"SELECT * FROM CollateralDetailInCaseOfCorporateGuarantee with(nolock) where CollateralDetailMainId = {collateralDetailMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        CollateralDetailInCaseOfCorporateGuarantee collateralCG = new CollateralDetailInCaseOfCorporateGuarantee()
                                                        {
                                                            CollateralTypeId = collateralTypeId,
                                                            NameOfFirm = reader2["NameOfFirm"].ToString(),
                                                            RegistrationNumber = reader2["RegistrationNumber"].ToString(),
                                                            FirmAddress = reader2["FirmAddress"].ToString(),
                                                            NetworthOfTheFirm = reader2["NetworthOfTheFirm"] == DBNull.Value ? 0 : (decimal?)reader2["NetworthOfTheFirm"],
                                                            ContactPerson = reader2["ContactPerson"].ToString(),
                                                            ContactNumber = reader2["ContactNumber"].ToString()
                                                        };
                                                        primaryCollateralsDetail = primaryCollateralsDetail + (!string.IsNullOrEmpty(primaryCollateralsDetail) ? "," : "") + JsonConvert.SerializeObject(collateralCG);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 8:
                                        commandText2 = $@"SELECT * FROM CollateralDetailInCaseOfStock with(nolock) where CollateralDetailMainId = {collateralDetailMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        CollateralDetailInCaseOfStock collateralShare = new CollateralDetailInCaseOfStock()
                                                        {
                                                            CollateralTypeId = collateralTypeId,
                                                            ShareTypeId = reader2["ShareTypeId"].ToString(),
                                                            CompanyName = reader2["CompanyName"].ToString(),
                                                            ListedInNepseBoolId = reader2["ListedInNepseBoolId"].ToString(),
                                                            PledgedUnits = reader2["PledgedUnits"] == DBNull.Value ? 0 : (decimal?)reader2["PledgedUnits"],
                                                            ShareUnitTypeId = reader2["UnitTypeId"].ToString(),
                                                            ValueOfShare = reader2["ValueOfShare"] == DBNull.Value ? 0 : (decimal?)reader2["ValueOfShare"]
                                                        };
                                                        primaryCollateralsDetail = primaryCollateralsDetail + (!string.IsNullOrEmpty(primaryCollateralsDetail) ? "," : "") + JsonConvert.SerializeObject(collateralShare);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 9:
                                        commandText2 = $@"SELECT * FROM CollateralDetailInCaseOfGold with(nolock) where CollateralDetailMainId = {collateralDetailMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        CollateralDetailInCaseOfGold collateralGold = new CollateralDetailInCaseOfGold()
                                                        {
                                                            CollateralTypeId = collateralTypeId,
                                                            Quantity = reader2["Quantity"] == DBNull.Value ? 0 : (decimal?)reader2["Quantity"],
                                                            MeasurementUnitId = reader2["MeasurementUnitId"].ToString(),
                                                            GoldTypeId = reader2["GoldTypeId"].ToString(),
                                                            ValueOfGold = reader2["ValueOfGold"] == DBNull.Value ? 0 : (decimal?)reader2["ValueOfGold"],
                                                            NameOfGoldTester = reader2["NameOfGoldTester"].ToString()
                                                        };
                                                        primaryCollateralsDetail = primaryCollateralsDetail + (!string.IsNullOrEmpty(primaryCollateralsDetail) ? "," : "") + JsonConvert.SerializeObject(collateralGold);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            // Get stack trace for the exception with source file information
                            var st = new StackTrace(ex, true);
                            // Get the top stack frame
                            var frame = st.GetFrame(0);
                            // Get the line number from the stack frame
                            var line = frame.GetFileLineNumber();
                            logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                            throw ex;
                        }
                    }
                }
            }
            return primaryCollateralsDetail == string.Empty ? "[]" : "[" + primaryCollateralsDetail + "]";
        }

        private string GetSavedSecondaryCollateralDetail(int npaManagementId)
        {
            //Get CollateralDetailMainId and CollateralTypeId from NPAMgmtMainId
            string secondaryCollateralsDetail = string.Empty;
            int collateralDetailMainId = 0;
            int collateralTypeId = 0;
            string commandText = $@"SELECT * FROM CollateralDetailMain with(nolock) where NPAManagementId = {npaManagementId} and CollateralClass=2";
            using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, commandText,
            CommandType.Text))
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        try
                        {
                            collateralDetailMainId = (int)reader["CollateralDetailMainId"];
                            collateralTypeId = (int)reader["CollateralTypeId"];
                            if (collateralDetailMainId != 0 && collateralTypeId != 0)
                            {
                                #region Get Collateral Details
                                string commandText2 = string.Empty;
                                switch (collateralTypeId)
                                {
                                    case 1:
                                        commandText2 = $@"SELECT * FROM CollateralDetailInCaseOfLand with(nolock) where CollateralDetailMainId = {collateralDetailMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        CollateralDetailInCaseOfLand collateralLand = new CollateralDetailInCaseOfLand()
                                                        {
                                                            CollateralTypeId = collateralTypeId,
                                                            ProvinceId = reader2["ProvinceId"].ToString(),
                                                            ZoneId = reader2["ZoneId"].ToString(),
                                                            DistrictId = reader2["DistrictId"].ToString(),
                                                            VDCMun = reader2["VDCMun"].ToString(),
                                                            Street = reader2["Street"].ToString(),
                                                            WardNumber = reader2["WardNumber"].ToString(),
                                                            PlotNumber = reader2["PlotNumber"].ToString(),
                                                            AreaTypeId = reader2["AreaTypeId"] == DBNull.Value ? 0 : (int?)reader2["AreaTypeId"],
                                                            Area = reader2["Area"].ToString(),
                                                            PropertyOwner = reader2["PropertyOwner"].ToString(),
                                                            RelationWithBorrower = reader2["RelationWithBorrower"].ToString(),
                                                            FirstValuatorName = reader2["FirstValuatorName"].ToString(),
                                                            FirstValuationDate = reader2["FirstValuationDate"] != DBNull.Value ? Convert.ToDateTime(reader2["FirstValuationDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            FirstFMVOfProperty = reader2["FirstFMVOfProperty"] == DBNull.Value ? 0 : (decimal?)reader2["FirstFMVOfProperty"],
                                                            FirstValuatorExistsInSBLCurrentlyYN = reader2["FirstValuatorExistsInSBLCurrentlyYN"].ToString(),
                                                            LatestValuatorName = reader2["LatestValuatorName"].ToString(),
                                                            LatestValuationDate = reader2["LatestValuationDate"] != DBNull.Value ? Convert.ToDateTime(reader2["LatestValuationDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            LatestValuatorExistsInSBLCurrentlyYN = reader2["LatestValuatorExistsInSBLCurrentlyYN"].ToString(),
                                                            FMVOfPropertyAsPerLatestValuationReport = reader2["FMVOfPropertyAsPerLatestValuationReport"] == DBNull.Value ? 0 : (decimal?)reader2["FMVOfPropertyAsPerLatestValuationReport"],
                                                            InsuranceCoverageType = reader2["InsuranceCoverageType"].ToString(),
                                                            InsuranceExpiryDate = reader2["InsuranceExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader2["InsuranceExpiryDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            InsuranceAmount = reader2["InsuranceAmount"] == DBNull.Value ? 0 : (decimal?)reader2["InsuranceAmount"]
                                                        };
                                                        secondaryCollateralsDetail = secondaryCollateralsDetail + (!string.IsNullOrEmpty(secondaryCollateralsDetail) ? "," : "") + JsonConvert.SerializeObject(collateralLand);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 2:
                                        commandText2 = $@"SELECT * FROM CollateralDetailInCaseOfBuilding with(nolock) where CollateralDetailMainId = {collateralDetailMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        CollateralDetailInCaseOfBuilding collateralBuilding = new CollateralDetailInCaseOfBuilding()
                                                        {
                                                            CollateralTypeId = collateralTypeId,
                                                            ProvinceId = reader2["ProvinceId"].ToString(),
                                                            ZoneId = reader2["ZoneId"].ToString(),
                                                            DistrictId = reader2["DistrictId"].ToString(),
                                                            VDCMun = reader2["VDCMun"].ToString(),
                                                            Street = reader2["Street"].ToString(),
                                                            WardNumber = reader2["WardNumber"].ToString(),
                                                            PlotNumber = reader2["PlotNumber"].ToString(),
                                                            AreaTypeId = reader2["AreaTypeId"] == DBNull.Value ? 0 : (int?)reader2["AreaTypeId"],
                                                            AreaOfLand = reader2["AreaOfLand"].ToString(),
                                                            AreaOfBuilding = reader2["AreaOfBuilding"].ToString(),
                                                            PropertyOwner = reader2["PropertyOwner"].ToString(),
                                                            RelationWithBorrower = reader2["RelationWithBorrower"].ToString(),
                                                            ConstructionCompletionDate = reader2["ConstructionCompletionDate"] != DBNull.Value ? Convert.ToDateTime(reader2["ConstructionCompletionDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            FirstValuatorName = reader2["FirstValuatorName"].ToString(),
                                                            FirstValuationDate = reader2["FirstValuationDate"] != DBNull.Value ? Convert.ToDateTime(reader2["FirstValuationDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            FirstFMVOfProperty = reader2["FirstFMVOfProperty"] == DBNull.Value ? 0 : (decimal?)reader2["FirstFMVOfProperty"],
                                                            FirstValuatorExistsInSBLCurrentlyYN = reader2["FirstValuatorExistsInSBLCurrentlyYN"].ToString(),
                                                            LatestValuatorName = reader2["LatestValuatorName"].ToString(),
                                                            LatestValuationDate = reader2["LatestValuationDate"] != DBNull.Value ? Convert.ToDateTime(reader2["LatestValuationDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            LatestValuatorExistsInSBLCurrentlyYN = reader2["LatestValuatorExistsInSBLCurrentlyYN"].ToString(),
                                                            FMVOfPropertyAsPerLatestValuationReport = reader2["FMVOfPropertyAsPerLatestValuationReport"] == DBNull.Value ? 0 : (decimal?)reader2["FMVOfPropertyAsPerLatestValuationReport"],
                                                            InsuranceCoverageType = reader2["InsuranceCoverageType"].ToString(),
                                                            InsuranceExpiryDate = reader2["InsuranceExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader2["InsuranceExpiryDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            InsuranceAmount = reader2["InsuranceAmount"] == DBNull.Value ? 0 : (decimal?)reader2["InsuranceAmount"]
                                                        };
                                                        secondaryCollateralsDetail = secondaryCollateralsDetail + (!string.IsNullOrEmpty(secondaryCollateralsDetail) ? "," : "") + JsonConvert.SerializeObject(collateralBuilding);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 3:
                                        commandText2 = $@"SELECT * FROM CollateralDetailInCaseOfVehicle with(nolock) where CollateralDetailMainId = {collateralDetailMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        CollateralDetailInCaseOfVehicle collateralVehicle = new CollateralDetailInCaseOfVehicle()
                                                        {
                                                            CollateralTypeId = collateralTypeId,
                                                            VehicleTypeId = (int?)reader2["VehicleTypeId"],
                                                            VehicleModel = reader2["VehicleModel"].ToString(),
                                                            EngineNumber = reader2["EngineNumber"].ToString(),
                                                            ChassisNumber = reader2["ChassisNumber"].ToString(),
                                                            VehicleRegistrationNumber = reader2["VehicleRegistrationNumber"].ToString(),
                                                            VehicleRegistrationDate = reader2["VehicleRegistrationDate"] != DBNull.Value ? Convert.ToDateTime(reader2["VehicleRegistrationDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            VehicleRegisteredOffice = reader2["VehicleRegisteredOffice"].ToString(),
                                                            VehicleMakeYear = reader2["VehicleMakeYear"].ToString(),
                                                            FirstValuatorName = reader2["FirstValuatorName"].ToString(),
                                                            FirstValuationDate = reader2["FirstValuationDate"] != DBNull.Value ? Convert.ToDateTime(reader2["FirstValuationDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            FirstFMVOfProperty = reader2["FirstFMVOfProperty"] == DBNull.Value ? 0 : (decimal?)reader2["FirstFMVOfProperty"],
                                                            FirstValuatorExistsInSBLCurrentlyYN = reader2["FirstValuatorExistsInSBLCurrentlyYN"].ToString(),
                                                            LatestValuatorName = reader2["LatestValuatorName"].ToString(),
                                                            LatestValuationDate = reader2["LatestValuationDate"] != DBNull.Value ? Convert.ToDateTime(reader2["LatestValuationDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            LatestValuatorExistsInSBLCurrentlyYN = reader2["LatestValuatorExistsInSBLCurrentlyYN"].ToString(),
                                                            ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice = reader2["ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice"] == DBNull.Value ? 0 : (decimal?)reader2["ValueOfPropertyAsPerLatestValuationReportOrTaxInvoice"],
                                                            InsuranceCoverageType = reader2["InsuranceCoverageType"].ToString(),
                                                            InsuranceExpiryDate = reader2["InsuranceExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader2["InsuranceExpiryDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            InsuranceAmount = reader2["InsuranceAmount"] == DBNull.Value ? 0 : (decimal?)reader2["InsuranceAmount"]
                                                        };
                                                        secondaryCollateralsDetail = secondaryCollateralsDetail + (!string.IsNullOrEmpty(secondaryCollateralsDetail) ? "," : "") + JsonConvert.SerializeObject(collateralVehicle);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 4:
                                        commandText2 = $@"SELECT * FROM CollateralDetailInCaseOfPlantAndMachinery with(nolock) where CollateralDetailMainId = {collateralDetailMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        CollateralDetailInCaseOfPlantAndMachinery collateralPAM = new CollateralDetailInCaseOfPlantAndMachinery()
                                                        {
                                                            CollateralTypeId = collateralTypeId,
                                                            PlantAndMachineryModel = reader2["PlantAndMachineryModel"].ToString(),
                                                            EngineNumber = reader2["EngineNumber"].ToString(),
                                                            ChassisNumber = reader2["ChassisNumber"].ToString(),
                                                            RegistrationNumber = reader2["RegistrationNumber"].ToString(),
                                                            FirstValuatorName = reader2["FirstValuatorName"].ToString(),
                                                            FirstValuationDate = reader2["FirstValuationDate"] != DBNull.Value ? Convert.ToDateTime(reader2["FirstValuationDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            FirstFMVOfProperty = reader2["FirstFMVOfProperty"] == DBNull.Value ? 0 : (decimal?)reader2["FirstFMVOfProperty"],
                                                            FirstValuatorExistsInSBLCurrentlyYN = reader2["FirstValuatorExistsInSBLCurrentlyYN"].ToString(),
                                                            LatestValuatorName = reader2["LatestValuatorName"].ToString(),
                                                            LatestValuationDate = reader2["LatestValuationDate"] != DBNull.Value ? Convert.ToDateTime(reader2["LatestValuationDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            LatestValuatorExistsInSBLCurrentlyYN = reader2["LatestValuatorExistsInSBLCurrentlyYN"].ToString(),
                                                            FMVOfPropertyAsPerLatestValuationReport = reader2["FMVOfPropertyAsPerLatestValuationReport"] == DBNull.Value ? 0 : (decimal?)reader2["FMVOfPropertyAsPerLatestValuationReport"],
                                                            InsuranceCoverageType = reader2["InsuranceCoverageType"].ToString(),
                                                            InsuranceExpiryDate = reader2["InsuranceExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader2["InsuranceExpiryDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            InsuranceAmount = reader2["InsuranceAmount"] == DBNull.Value ? 0 : (decimal?)reader2["InsuranceAmount"]
                                                        };
                                                        secondaryCollateralsDetail = secondaryCollateralsDetail + (!string.IsNullOrEmpty(secondaryCollateralsDetail) ? "," : "") + JsonConvert.SerializeObject(collateralPAM);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 5:
                                        commandText2 = $@"SELECT * FROM CollateralDetailInCaseOfCurrentAsset with(nolock) where CollateralDetailMainId = {collateralDetailMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        CollateralDetailInCaseOfCurrentAsset collateralCurrentAsset = new CollateralDetailInCaseOfCurrentAsset()
                                                        {
                                                            CollateralTypeId = collateralTypeId,
                                                            DateOfLatestStockInspectionReportCollectedByBranch = reader2["DateOfLatestStockInspectionReportCollectedByBranch"] != DBNull.Value ? Convert.ToDateTime(reader2["DateOfLatestStockInspectionReportCollectedByBranch"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            WorkingCapitalAsPerStockReport = reader2["WorkingCapitalAsPerStockReport"] == DBNull.Value ? 0 : (decimal?)reader2["WorkingCapitalAsPerStockReport"],
                                                            WorkingCapitalAsPerStockInspector = reader2["WorkingCapitalAsPerStockInspector"] == DBNull.Value ? 0 : (decimal?)reader2["WorkingCapitalAsPerStockInspector"],
                                                            DrawingPower = reader2["DrawingPower"].ToString(),
                                                            InsuranceCoverageType = reader2["InsuranceCoverageType"].ToString(),
                                                            InsuranceExpiryDate = reader2["InsuranceExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader2["InsuranceExpiryDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            InsuranceAmount = reader2["InsuranceAmount"] == DBNull.Value ? 0 : (decimal?)reader2["InsuranceAmount"]
                                                        };
                                                        secondaryCollateralsDetail = secondaryCollateralsDetail + (!string.IsNullOrEmpty(secondaryCollateralsDetail) ? "," : "") + JsonConvert.SerializeObject(collateralCurrentAsset);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 6:
                                        commandText2 = $@"SELECT * FROM CollateralDetailInCaseOfPersonalGuarantee with(nolock) where CollateralDetailMainId = {collateralDetailMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        CollateralDetailInCaseOfPersonalGuarantee collateralPG = new CollateralDetailInCaseOfPersonalGuarantee()
                                                        {
                                                            CollateralTypeId = collateralTypeId,
                                                            NameOfGuarantor = reader2["NameOfGuarantor"].ToString(),
                                                            CitizenshipNumber = reader2["CitizenshipNumber"].ToString(),
                                                            FatherName = reader2["FatherName"].ToString(),
                                                            GrandfatherName = reader2["GrandfatherName"].ToString(),
                                                            PermanentAddress = reader2["PermanentAddress"].ToString(),
                                                            CurrentAddress = reader2["CurrentAddress"].ToString(),
                                                            NetworthOfGuarantor = reader2["NetworthOfGuarantor"] == DBNull.Value ? 0 : (decimal?)reader2["NetworthOfGuarantor"],
                                                            RelationWithBorrower = reader2["RelationWithBorrower"].ToString(),
                                                            ContactNumber = reader2["ContactNumber"].ToString(),
                                                            Profession = reader2["Profession"].ToString()
                                                        };
                                                        secondaryCollateralsDetail = secondaryCollateralsDetail + (!string.IsNullOrEmpty(secondaryCollateralsDetail) ? "," : "") + JsonConvert.SerializeObject(collateralPG);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 7:
                                        commandText2 = $@"SELECT * FROM CollateralDetailInCaseOfCorporateGuarantee with(nolock) where CollateralDetailMainId = {collateralDetailMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        CollateralDetailInCaseOfCorporateGuarantee collateralCG = new CollateralDetailInCaseOfCorporateGuarantee()
                                                        {
                                                            CollateralTypeId = collateralTypeId,
                                                            NameOfFirm = reader2["NameOfFirm"].ToString(),
                                                            RegistrationNumber = reader2["RegistrationNumber"].ToString(),
                                                            FirmAddress = reader2["FirmAddress"].ToString(),
                                                            NetworthOfTheFirm = reader2["NetworthOfTheFirm"] == DBNull.Value ? 0 : (decimal?)reader2["NetworthOfTheFirm"],
                                                            ContactPerson = reader2["ContactPerson"].ToString(),
                                                            ContactNumber = reader2["ContactNumber"].ToString()
                                                        };
                                                        secondaryCollateralsDetail = secondaryCollateralsDetail + (!string.IsNullOrEmpty(secondaryCollateralsDetail) ? "," : "") + JsonConvert.SerializeObject(collateralCG);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 8:
                                        commandText2 = $@"SELECT * FROM CollateralDetailInCaseOfStock with(nolock) where CollateralDetailMainId = {collateralDetailMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        CollateralDetailInCaseOfStock collateralShare = new CollateralDetailInCaseOfStock()
                                                        {
                                                            CollateralTypeId = collateralTypeId,
                                                            ShareTypeId = reader2["ShareTypeId"].ToString(),
                                                            CompanyName = reader2["CompanyName"].ToString(),
                                                            ListedInNepseBoolId = reader2["ListedInNepseBoolId"].ToString(),
                                                            PledgedUnits = reader2["PledgedUnits"] == DBNull.Value ? 0 : (decimal?)reader2["PledgedUnits"],
                                                            ShareUnitTypeId = reader2["UnitTypeId"].ToString(),
                                                            ValueOfShare = reader2["ValueOfShare"] == DBNull.Value ? 0 : (decimal?)reader2["ValueOfShare"]
                                                        };
                                                        secondaryCollateralsDetail = secondaryCollateralsDetail + (!string.IsNullOrEmpty(secondaryCollateralsDetail) ? "," : "") + JsonConvert.SerializeObject(collateralShare);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 9:
                                        commandText2 = $@"SELECT * FROM CollateralDetailInCaseOfGold with(nolock) where CollateralDetailMainId = {collateralDetailMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        CollateralDetailInCaseOfGold collateralGold = new CollateralDetailInCaseOfGold()
                                                        {
                                                            CollateralTypeId = collateralTypeId,
                                                            Quantity = reader2["Quantity"] == DBNull.Value ? 0 : (decimal?)reader2["Quantity"],
                                                            MeasurementUnitId = reader2["MeasurementUnitId"].ToString(),
                                                            GoldTypeId = reader2["GoldTypeId"].ToString(),
                                                            ValueOfGold = reader2["ValueOfGold"] == DBNull.Value ? 0 : (decimal?)reader2["ValueOfGold"],
                                                            NameOfGoldTester = reader2["NameOfGoldTester"].ToString()
                                                        };
                                                        secondaryCollateralsDetail = secondaryCollateralsDetail + (!string.IsNullOrEmpty(secondaryCollateralsDetail) ? "," : "") + JsonConvert.SerializeObject(collateralGold);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            // Get stack trace for the exception with source file information
                            var st = new StackTrace(ex, true);
                            // Get the top stack frame
                            var frame = st.GetFrame(0);
                            // Get the line number from the stack frame
                            var line = frame.GetFileLineNumber();
                            logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                            throw ex;
                        }
                    }
                }
            }
            return secondaryCollateralsDetail == string.Empty ? "[]" : "[" + secondaryCollateralsDetail + "]";
        }

        private string GetSavedLoanAccountDetail(int npaManagementId)
        {
            string loanAccountDetail = string.Empty;
            string commandText = $@"SELECT * FROM LoanAccountDetail with(nolock) where NPAManagementId = {npaManagementId}";
            using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, commandText,
            CommandType.Text))
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        try
                        {
                            LoanAccountDetail individualLoanDetail = new LoanAccountDetail()
                            {
                                LoanMainId = (int)reader["LoanMainId"],
                                NPAManagementId = (int)reader["NPAManagementId"],
                                LoanAccountNo = reader["LoanAccountNo"].ToString(),
                                ProductCode = reader["ProductCode"].ToString(),
                                LoanTypeId = reader["LoanTypeId"] == DBNull.Value ? 0 : (int?)reader["LoanTypeId"],
                                LoanNatureId = reader["LoanNatureId"] == DBNull.Value ? 0 : (int?)reader["LoanNatureId"],
                                FirstLimitInitiatedBy = reader["FirstLimitInitiatedBy"].ToString(),
                                FirstApprovedBy = reader["FirstApprovedBy"].ToString(),
                                FirstRecommendedBy = reader["FirstRecommendedBy"].ToString(),
                                DepartmentId = reader["DepartmentId"] == DBNull.Value ? 0 : (int?)reader["DepartmentId"],
                                BranchProvince = reader["BranchProvince"].ToString(),
                                ReportingSBO = reader["ReportingSBO"].ToString(),
                                NomineeAccountNo = reader["NomineeAccountNo"].ToString(),
                                InitialApprovedLimit = reader["InitialApprovedLimit"] == DBNull.Value ? 0 : (decimal?)reader["InitialApprovedLimit"],
                                FirstLimitPlacementDate = reader["FirstLimitPlacementDate"] != DBNull.Value ? Convert.ToDateTime(reader["FirstLimitPlacementDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                LatestApprovedLimit = reader["LatestApprovedLimit"] == DBNull.Value ? 0 : (decimal?)reader["LatestApprovedLimit"],
                                LimitExpiryDate = reader["LimitExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader["LimitExpiryDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                LoanCurrentStatusId = reader["LoanCurrentStatusId"] == DBNull.Value ? 0 : (int?)reader["LoanCurrentStatusId"],
                                SettlementDate = reader["SettlementDate"] != DBNull.Value ? Convert.ToDateTime(reader["SettlementDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                LoanStatusId = reader["LoanStatusId"] == DBNull.Value ? 0 : (int?)reader["LoanStatusId"],
                                OutStDate = reader["OutStDate"] != DBNull.Value ? Convert.ToDateTime(reader["OutStDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                OutStPrincipal = reader["OutStPrincipal"] == DBNull.Value ? 0 : (decimal?)reader["OutStPrincipal"],
                                OutStInterest = reader["OutStInterest"] == DBNull.Value ? 0 : (decimal?)reader["OutStInterest"],
                                OutStAdhocCharges = reader["OutStAdhocCharges"] == DBNull.Value ? 0 : (decimal?)reader["OutStAdhocCharges"],
                                OutStInterestOnInterest = reader["OutStInterestOnInterest"] == DBNull.Value ? 0 : (decimal?)reader["OutStInterestOnInterest"],
                                OutStPenalCharges = reader["OutStPenalCharges"] == DBNull.Value ? 0 : (decimal?)reader["OutStPenalCharges"],
                                OutStTotalAmount = reader["OutStTotalAmount"] == DBNull.Value ? 0 : (decimal?)reader["OutStTotalAmount"],
                                AccruedAmtFromDate = reader["AccruedAmtFromDate"] != DBNull.Value ? Convert.ToDateTime(reader["AccruedAmtFromDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                AccruedAmtToDate = reader["AccruedAmtToDate"] != DBNull.Value ? Convert.ToDateTime(reader["AccruedAmtToDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                AccruedAmtPrincipal = reader["AccruedAmtPrincipal"] == DBNull.Value ? 0 : (decimal?)reader["AccruedAmtPrincipal"],
                                AccruedAmtInterest = reader["AccruedAmtInterest"] == DBNull.Value ? 0 : (decimal?)reader["AccruedAmtInterest"],
                                AccruedAmtAdhocCharges = reader["AccruedAmtAdhocCharges"] == DBNull.Value ? 0 : (decimal?)reader["AccruedAmtAdhocCharges"],
                                AccruedAmtInterestOnInterest = reader["AccruedAmtInterestOnInterest"] == DBNull.Value ? 0 : (decimal?)reader["AccruedAmtInterestOnInterest"],
                                AccruedAmtPenalCharges = reader["AccruedAmtPenalCharges"] == DBNull.Value ? 0 : (decimal?)reader["AccruedAmtPenalCharges"],
                                AccruedAmtTotalAmount = reader["AccruedAmtTotalAmount"] == DBNull.Value ? 0 : (decimal?)reader["AccruedAmtTotalAmount"],
                                RecoveredAmtFromDate = reader["RecoveredAmtFromDate"] != DBNull.Value ? Convert.ToDateTime(reader["RecoveredAmtFromDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                RecoveredAmtToDate = reader["RecoveredAmtToDate"] != DBNull.Value ? Convert.ToDateTime(reader["RecoveredAmtToDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                RecoveredAmtPrincipal = reader["RecoveredAmtPrincipal"] == DBNull.Value ? 0 : (decimal?)reader["RecoveredAmtPrincipal"],
                                RecoveredAmtInterest = reader["RecoveredAmtInterest"] == DBNull.Value ? 0 : (decimal?)reader["RecoveredAmtInterest"],
                                RecoveredAmtAdhocCharges = reader["RecoveredAmtAdhocCharges"] == DBNull.Value ? 0 : (decimal?)reader["RecoveredAmtAdhocCharges"],
                                RecoveredAmtInterestOnInterest = reader["RecoveredAmtInterestOnInterest"] == DBNull.Value ? 0 : (decimal?)reader["RecoveredAmtInterestOnInterest"],
                                RecoveredAmtPenalCharges = reader["RecoveredAmtPenalCharges"] == DBNull.Value ? 0 : (decimal?)reader["RecoveredAmtPenalCharges"],
                                RecoveredAmtTotalAmount = reader["RecoveredAmtTotalAmount"] == DBNull.Value ? 0 : (decimal?)reader["RecoveredAmtTotalAmount"],
                                ProvisionPercentageId = reader["ProvisionPercentageId"] == DBNull.Value ? 0 : (int?)reader["ProvisionPercentageId"],
                                ProvisionDate = reader["ProvisionDate"] != DBNull.Value ? Convert.ToDateTime(reader["ProvisionDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                ProvisionAmount = reader["ProvisionAmount"] == DBNull.Value ? 0 : (decimal?)reader["ProvisionAmount"]
                            };
                            loanAccountDetail = loanAccountDetail + (!string.IsNullOrEmpty(loanAccountDetail) ? "," : "") + JsonConvert.SerializeObject(individualLoanDetail);
                        }
                        catch (Exception ex)
                        {
                            // Get stack trace for the exception with source file information
                            var st = new StackTrace(ex, true);
                            // Get the top stack frame
                            var frame = st.GetFrame(0);
                            // Get the line number from the stack frame
                            var line = frame.GetFileLineNumber();
                            logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                            throw ex;
                        }
                    }
                }
            }
            return loanAccountDetail == string.Empty ? "[]" : "[" + loanAccountDetail + "]";
        }

        private string GetSavedBranchFollowUpDetail(int npaManagementId)
        {
            //Get FollowUpMainId and FollowUpTypeId from NPAMgmtMainId
            string branchFollowUpsDetail = string.Empty;
            int followUpMainId = 0;
            int followUpTypeId = 0;
            int followUpById = 1;
            string commandText = $@"SELECT * FROM FollowUpMain with(nolock) where NPAManagementId = {npaManagementId} and FollowUpById=1";
            using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, commandText,
            CommandType.Text))
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        try
                        {
                            followUpMainId = (int)reader["FollowUpMainId"];
                            followUpTypeId = (int)reader["FollowUpTypeId"];
                            if (followUpMainId != 0 && followUpTypeId != 0)
                            {
                                #region Get Collateral Details
                                string commandText2 = string.Empty;
                                switch (followUpTypeId)
                                {
                                    case 1:
                                        commandText2 = $@"SELECT * FROM FollowUpDetailByTelephone with(nolock) where FollowUpMainId = {followUpMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        FollowUpDetailByTelephone followUpTelephone = new FollowUpDetailByTelephone()
                                                        {
                                                            FollowUpMainId = followUpMainId,
                                                            FollowUpTypeId = followUpTypeId,
                                                            FollowUpById = followUpById,
                                                            NPAManagementId = npaManagementId,
                                                            ContactDate = reader2["ContactDate"] != DBNull.Value ? Convert.ToDateTime(reader2["ContactDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            ResponseOfBorrower = reader2["ResponseOfBorrower"].ToString(),
                                                            NextActionToBeTaken = reader2["NextActionToBeTaken"].ToString(),
                                                            DueDateOfNextAction = reader2["DueDateOfNextAction"] != DBNull.Value ? Convert.ToDateTime(reader2["DueDateOfNextAction"].ToString().Trim()).ToString("yyyy/MM/dd") : ""
                                                        };
                                                        branchFollowUpsDetail = branchFollowUpsDetail + (!string.IsNullOrEmpty(branchFollowUpsDetail) ? "," : "") + JsonConvert.SerializeObject(followUpTelephone);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 2:
                                        commandText2 = $@"SELECT * FROM FollowUpDetailByInPersonVisit with(nolock) where FollowUpMainId = {followUpMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        FollowUpDetailByInPersonVisit followUpByInPersonVisit = new FollowUpDetailByInPersonVisit()
                                                        {
                                                            FollowUpMainId = followUpMainId,
                                                            FollowUpTypeId = followUpTypeId,
                                                            FollowUpById = followUpById,
                                                            NPAManagementId = npaManagementId,
                                                            VisitDate = reader2["VisitDate"] != DBNull.Value ? Convert.ToDateTime(reader2["VisitDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            ResponseOfBorrower = reader2["ResponseOfBorrower"].ToString(),
                                                            NextActionToBeTaken = reader2["NextActionToBeTaken"].ToString(),
                                                            DueDateOfNextAction = reader2["DueDateOfNextAction"] != DBNull.Value ? Convert.ToDateTime(reader2["DueDateOfNextAction"].ToString().Trim()).ToString("yyyy/MM/dd") : ""
                                                        };
                                                        branchFollowUpsDetail = branchFollowUpsDetail + (!string.IsNullOrEmpty(branchFollowUpsDetail) ? "," : "") + JsonConvert.SerializeObject(followUpByInPersonVisit);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 3:
                                        commandText2 = $@"SELECT * FROM FollowUpDetailFor15DaysLetter with(nolock) where FollowUpMainId = {followUpMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        FollowUpDetailFor15DaysLetter followUp15DaysLetter = new FollowUpDetailFor15DaysLetter()
                                                        {
                                                            FollowUpMainId = followUpMainId,
                                                            FollowUpTypeId = followUpTypeId,
                                                            FollowUpById = followUpById,
                                                            NPAManagementId = npaManagementId,
                                                            DateOfIssuanceOfFollowUpLetter = reader2["DateOfIssuanceOfFollowUpLetter"] != DBNull.Value ? Convert.ToDateTime(reader2["DateOfIssuanceOfFollowUpLetter"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            DateOfReceiptOfLetter = reader2["DateOfReceiptOfLetter"] != DBNull.Value ? Convert.ToDateTime(reader2["DateOfReceiptOfLetter"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            ResponseOfBorrower = reader2["ResponseOfBorrower"].ToString(),
                                                            NextActionToBeTaken = reader2["NextActionToBeTaken"].ToString(),
                                                            DueDateOfNextAction = reader2["DueDateOfNextAction"] != DBNull.Value ? Convert.ToDateTime(reader2["DueDateOfNextAction"].ToString().Trim()).ToString("yyyy/MM/dd") : ""
                                                        };
                                                        branchFollowUpsDetail = branchFollowUpsDetail + (!string.IsNullOrEmpty(branchFollowUpsDetail) ? "," : "") + JsonConvert.SerializeObject(followUp15DaysLetter);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 4:
                                        commandText2 = $@"SELECT * FROM FollowUpDetailForFirst7DaysLetter with(nolock) where FollowUpMainId = {followUpMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        FollowUpDetailForFirst7DaysLetter followUpFirst7DaysLetter = new FollowUpDetailForFirst7DaysLetter()
                                                        {
                                                            FollowUpMainId = followUpMainId,
                                                            FollowUpTypeId = followUpTypeId,
                                                            FollowUpById = followUpById,
                                                            NPAManagementId = npaManagementId,
                                                            DateOfIssuanceOfFollowUpLetter = reader2["DateOfIssuanceOfFollowUpLetter"] != DBNull.Value ? Convert.ToDateTime(reader2["DateOfIssuanceOfFollowUpLetter"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            DateOfReceiptOfLetter = reader2["DateOfReceiptOfLetter"] != DBNull.Value ? Convert.ToDateTime(reader2["DateOfReceiptOfLetter"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            ResponseOfBorrower = reader2["ResponseOfBorrower"].ToString(),
                                                            NextActionToBeTaken = reader2["NextActionToBeTaken"].ToString(),
                                                            DueDateOfNextAction = reader2["DueDateOfNextAction"] != DBNull.Value ? Convert.ToDateTime(reader2["DueDateOfNextAction"].ToString().Trim()).ToString("yyyy/MM/dd") : ""
                                                        };
                                                        branchFollowUpsDetail = branchFollowUpsDetail + (!string.IsNullOrEmpty(branchFollowUpsDetail) ? "," : "") + JsonConvert.SerializeObject(followUpFirst7DaysLetter);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 5:
                                        commandText2 = $@"SELECT * FROM FollowUpDetailForSecond7DaysLetter with(nolock) where FollowUpMainId = {followUpMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        FollowUpDetailForSecond7DaysLetter followUpSecond7DaysLetter = new FollowUpDetailForSecond7DaysLetter()
                                                        {
                                                            FollowUpMainId = followUpMainId,
                                                            FollowUpTypeId = followUpTypeId,
                                                            FollowUpById = followUpById,
                                                            NPAManagementId = npaManagementId,
                                                            DateOfIssuanceOfFollowUpLetter = reader2["DateOfIssuanceOfFollowUpLetter"] != DBNull.Value ? Convert.ToDateTime(reader2["DateOfIssuanceOfFollowUpLetter"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            DateOfReceiptOfLetter = reader2["DateOfReceiptOfLetter"] != DBNull.Value ? Convert.ToDateTime(reader2["DateOfReceiptOfLetter"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            ResponseOfBorrower = reader2["ResponseOfBorrower"].ToString(),
                                                            NextActionToBeTaken = reader2["NextActionToBeTaken"].ToString(),
                                                            DueDateOfNextAction = reader2["DueDateOfNextAction"] != DBNull.Value ? Convert.ToDateTime(reader2["DueDateOfNextAction"].ToString().Trim()).ToString("yyyy/MM/dd") : ""
                                                        };
                                                        branchFollowUpsDetail = branchFollowUpsDetail + (!string.IsNullOrEmpty(branchFollowUpsDetail) ? "," : "") + JsonConvert.SerializeObject(followUpSecond7DaysLetter);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 6:
                                        commandText2 = $@"SELECT * FROM FollowUpDetailFor35DaysCallNotice with(nolock) where FollowUpMainId = {followUpMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        FollowUpDetailFor35DaysCallNotice followUp35DaysCallNotice = new FollowUpDetailFor35DaysCallNotice()
                                                        {
                                                            FollowUpMainId = followUpMainId,
                                                            FollowUpTypeId = followUpTypeId,
                                                            FollowUpById = followUpById,
                                                            NPAManagementId = npaManagementId,
                                                            DateOfNotice = reader2["DateOfNotice"] != DBNull.Value ? Convert.ToDateTime(reader2["DateOfNotice"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            NewspaperName = reader2["NewspaperName"].ToString(),
                                                            ResponseOfBorrower = reader2["ResponseOfBorrower"].ToString(),
                                                            NextActionToBeTaken = reader2["NextActionToBeTaken"].ToString(),
                                                            DueDateOfNextAction = reader2["DueDateOfNextAction"] != DBNull.Value ? Convert.ToDateTime(reader2["DueDateOfNextAction"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            FollowUpResultId = reader2["FollowUpResultId"] == DBNull.Value ? 0 : (int)reader2["FollowUpResultId"],
                                                            Remarks = reader2["Remarks"].ToString(),
                                                        };
                                                        branchFollowUpsDetail = branchFollowUpsDetail + (!string.IsNullOrEmpty(branchFollowUpsDetail) ? "," : "") + JsonConvert.SerializeObject(followUp35DaysCallNotice);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 7:
                                        commandText2 = $@"SELECT * FROM FollowUpDetailForAuctionNotice with(nolock) where FollowUpMainId = {followUpMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        FollowUpDetailForAuctionNotice followUpAuctionNotice = new FollowUpDetailForAuctionNotice()
                                                        {
                                                            FollowUpMainId = followUpMainId,
                                                            FollowUpTypeId = followUpTypeId,
                                                            FollowUpById = followUpById,
                                                            NPAManagementId = npaManagementId,
                                                            AuctionNoticeTypeId = reader2["AuctionNoticeTypeId"] == DBNull.Value ? 0 : (int?)reader2["AuctionNoticeTypeId"],
                                                            DateOfAuctionNotice = reader2["DateOfAuctionNotice"] != DBNull.Value ? Convert.ToDateTime(reader2["DateOfAuctionNotice"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            AuctionNewspaperName = reader2["AuctionNewspaperName"].ToString(),
                                                            AuctionResponseOfBorrower = reader2["AuctionResponseOfBorrower"].ToString(),
                                                            AuctionNextActionToBeTaken = reader2["AuctionNextActionToBeTaken"].ToString(),
                                                            AuctionDueDateOfNextAction = reader2["AuctionDueDateOfNextAction"] != DBNull.Value ? Convert.ToDateTime(reader2["AuctionDueDateOfNextAction"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            AuctionRemarks = reader2["AuctionRemarks"].ToString(),
                                                            DateOfRevaluationForAuction = reader2["DateOfRevaluationForAuction"] != DBNull.Value ? Convert.ToDateTime(reader2["DateOfRevaluationForAuction"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            RevaluatedFMVOfCollateralForAuction = reader2["RevaluatedFMVOfCollateralForAuction"] == DBNull.Value ? 0 : (decimal?)reader2["RevaluatedFMVOfCollateralForAuction"],
                                                        };
                                                        branchFollowUpsDetail = branchFollowUpsDetail + (!string.IsNullOrEmpty(branchFollowUpsDetail) ? "," : "") + JsonConvert.SerializeObject(followUpAuctionNotice);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 8:
                                        commandText2 = $@"SELECT * FROM FollowUpDetailForOthers with(nolock) where FollowUpMainId = {followUpMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        FollowUpDetailForOthers followUpForOthers = new FollowUpDetailForOthers()
                                                        {
                                                            FollowUpMainId = followUpMainId,
                                                            FollowUpTypeId = followUpTypeId,
                                                            FollowUpById = followUpById,
                                                            NPAManagementId = npaManagementId,
                                                            ContactDate = reader2["ContactDate"] != DBNull.Value ? Convert.ToDateTime(reader2["ContactDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            ResponseOfBorrower = reader2["ResponseOfBorrower"].ToString(),
                                                            NextActionToBeTaken = reader2["NextActionToBeTaken"].ToString(),
                                                            DueDateOfNextAction = reader2["DueDateOfNextAction"] != DBNull.Value ? Convert.ToDateTime(reader2["DueDateOfNextAction"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            Remarks = reader2["Remarks"].ToString()
                                                        };
                                                        branchFollowUpsDetail = branchFollowUpsDetail + (!string.IsNullOrEmpty(branchFollowUpsDetail) ? "," : "") + JsonConvert.SerializeObject(followUpForOthers);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            // Get stack trace for the exception with source file information
                            var st = new StackTrace(ex, true);
                            // Get the top stack frame
                            var frame = st.GetFrame(0);
                            // Get the line number from the stack frame
                            var line = frame.GetFileLineNumber();
                            logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                            throw ex;
                        }
                    }
                }
            }
            return branchFollowUpsDetail == string.Empty ? "[]" : "[" + branchFollowUpsDetail + "]";
        }

        private string GetSavedSACFollowUpDetail(int npaManagementId)
        {
            //Get FollowUpMainId and FollowUpTypeId from NPAMgmtMainId
            string sacFollowUpsDetail = string.Empty;
            int followUpMainId = 0;
            int followUpTypeId = 0;
            int followUpById = 2;
            string commandText = $@"SELECT * FROM FollowUpMain with(nolock) where NPAManagementId = {npaManagementId} and FollowUpById=2";
            using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, commandText,
            CommandType.Text))
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        try
                        {
                            followUpMainId = (int)reader["FollowUpMainId"];
                            followUpTypeId = (int)reader["FollowUpTypeId"];
                            if (followUpMainId != 0 && followUpTypeId != 0)
                            {
                                #region Get Collateral Details
                                string commandText2 = string.Empty;
                                switch (followUpTypeId)
                                {
                                    case 1:
                                        commandText2 = $@"SELECT * FROM FollowUpDetailByTelephone with(nolock) where FollowUpMainId = {followUpMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        FollowUpDetailByTelephone followUpTelephone = new FollowUpDetailByTelephone()
                                                        {
                                                            FollowUpMainId = followUpMainId,
                                                            FollowUpTypeId = followUpTypeId,
                                                            FollowUpById = followUpById,
                                                            NPAManagementId = npaManagementId,
                                                            ContactDate = reader2["ContactDate"] != DBNull.Value ? Convert.ToDateTime(reader2["ContactDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            ResponseOfBorrower = reader2["ResponseOfBorrower"].ToString(),
                                                            NextActionToBeTaken = reader2["NextActionToBeTaken"].ToString(),
                                                            DueDateOfNextAction = reader2["DueDateOfNextAction"] != DBNull.Value ? Convert.ToDateTime(reader2["DueDateOfNextAction"].ToString().Trim()).ToString("yyyy/MM/dd") : ""
                                                        };
                                                        sacFollowUpsDetail = sacFollowUpsDetail + (!string.IsNullOrEmpty(sacFollowUpsDetail) ? "," : "") + JsonConvert.SerializeObject(followUpTelephone);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 2:
                                        commandText2 = $@"SELECT * FROM FollowUpDetailByInPersonVisit with(nolock) where FollowUpMainId = {followUpMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        FollowUpDetailByInPersonVisit followUpByInPersonVisit = new FollowUpDetailByInPersonVisit()
                                                        {
                                                            FollowUpMainId = followUpMainId,
                                                            FollowUpTypeId = followUpTypeId,
                                                            FollowUpById = followUpById,
                                                            NPAManagementId = npaManagementId,
                                                            VisitDate = reader2["VisitDate"] != DBNull.Value ? Convert.ToDateTime(reader2["VisitDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            ResponseOfBorrower = reader2["ResponseOfBorrower"].ToString(),
                                                            NextActionToBeTaken = reader2["NextActionToBeTaken"].ToString(),
                                                            DueDateOfNextAction = reader2["DueDateOfNextAction"] != DBNull.Value ? Convert.ToDateTime(reader2["DueDateOfNextAction"].ToString().Trim()).ToString("yyyy/MM/dd") : ""
                                                        };
                                                        sacFollowUpsDetail = sacFollowUpsDetail + (!string.IsNullOrEmpty(sacFollowUpsDetail) ? "," : "") + JsonConvert.SerializeObject(followUpByInPersonVisit);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 3:
                                        commandText2 = $@"SELECT * FROM FollowUpDetailFor15DaysLetter with(nolock) where FollowUpMainId = {followUpMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        FollowUpDetailFor15DaysLetter followUp15DaysLetter = new FollowUpDetailFor15DaysLetter()
                                                        {
                                                            FollowUpMainId = followUpMainId,
                                                            FollowUpTypeId = followUpTypeId,
                                                            FollowUpById = followUpById,
                                                            NPAManagementId = npaManagementId,
                                                            DateOfIssuanceOfFollowUpLetter = reader2["DateOfIssuanceOfFollowUpLetter"] != DBNull.Value ? Convert.ToDateTime(reader2["DateOfIssuanceOfFollowUpLetter"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            DateOfReceiptOfLetter = reader2["DateOfReceiptOfLetter"] != DBNull.Value ? Convert.ToDateTime(reader2["DateOfReceiptOfLetter"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            ResponseOfBorrower = reader2["ResponseOfBorrower"].ToString(),
                                                            NextActionToBeTaken = reader2["NextActionToBeTaken"].ToString(),
                                                            DueDateOfNextAction = reader2["DueDateOfNextAction"] != DBNull.Value ? Convert.ToDateTime(reader2["DueDateOfNextAction"].ToString().Trim()).ToString("yyyy/MM/dd") : ""
                                                        };
                                                        sacFollowUpsDetail = sacFollowUpsDetail + (!string.IsNullOrEmpty(sacFollowUpsDetail) ? "," : "") + JsonConvert.SerializeObject(followUp15DaysLetter);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 4:
                                        commandText2 = $@"SELECT * FROM FollowUpDetailForFirst7DaysLetter with(nolock) where FollowUpMainId = {followUpMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        FollowUpDetailForFirst7DaysLetter followUpFirst7DaysLetter = new FollowUpDetailForFirst7DaysLetter()
                                                        {
                                                            FollowUpMainId = followUpMainId,
                                                            FollowUpTypeId = followUpTypeId,
                                                            FollowUpById = followUpById,
                                                            NPAManagementId = npaManagementId,
                                                            DateOfIssuanceOfFollowUpLetter = reader2["DateOfIssuanceOfFollowUpLetter"] != DBNull.Value ? Convert.ToDateTime(reader2["DateOfIssuanceOfFollowUpLetter"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            DateOfReceiptOfLetter = reader2["DateOfReceiptOfLetter"] != DBNull.Value ? Convert.ToDateTime(reader2["DateOfReceiptOfLetter"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            ResponseOfBorrower = reader2["ResponseOfBorrower"].ToString(),
                                                            NextActionToBeTaken = reader2["NextActionToBeTaken"].ToString(),
                                                            DueDateOfNextAction = reader2["DueDateOfNextAction"] != DBNull.Value ? Convert.ToDateTime(reader2["DueDateOfNextAction"].ToString().Trim()).ToString("yyyy/MM/dd") : ""
                                                        };
                                                        sacFollowUpsDetail = sacFollowUpsDetail + (!string.IsNullOrEmpty(sacFollowUpsDetail) ? "," : "") + JsonConvert.SerializeObject(followUpFirst7DaysLetter);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 5:
                                        commandText2 = $@"SELECT * FROM FollowUpDetailForSecond7DaysLetter with(nolock) where FollowUpMainId = {followUpMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        FollowUpDetailForSecond7DaysLetter followUpSecond7DaysLetter = new FollowUpDetailForSecond7DaysLetter()
                                                        {
                                                            FollowUpMainId = followUpMainId,
                                                            FollowUpTypeId = followUpTypeId,
                                                            FollowUpById = followUpById,
                                                            NPAManagementId = npaManagementId,
                                                            DateOfIssuanceOfFollowUpLetter = reader2["DateOfIssuanceOfFollowUpLetter"] != DBNull.Value ? Convert.ToDateTime(reader2["DateOfIssuanceOfFollowUpLetter"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            DateOfReceiptOfLetter = reader2["DateOfReceiptOfLetter"] != DBNull.Value ? Convert.ToDateTime(reader2["DateOfReceiptOfLetter"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            ResponseOfBorrower = reader2["ResponseOfBorrower"].ToString(),
                                                            NextActionToBeTaken = reader2["NextActionToBeTaken"].ToString(),
                                                            DueDateOfNextAction = reader2["DueDateOfNextAction"] != DBNull.Value ? Convert.ToDateTime(reader2["DueDateOfNextAction"].ToString().Trim()).ToString("yyyy/MM/dd") : ""
                                                        };
                                                        sacFollowUpsDetail = sacFollowUpsDetail + (!string.IsNullOrEmpty(sacFollowUpsDetail) ? "," : "") + JsonConvert.SerializeObject(followUpSecond7DaysLetter);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 6:
                                        commandText2 = $@"SELECT * FROM FollowUpDetailFor35DaysCallNotice with(nolock) where FollowUpMainId = {followUpMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        FollowUpDetailFor35DaysCallNotice followUp35DaysCallNotice = new FollowUpDetailFor35DaysCallNotice()
                                                        {
                                                            FollowUpMainId = followUpMainId,
                                                            FollowUpTypeId = followUpTypeId,
                                                            FollowUpById = followUpById,
                                                            NPAManagementId = npaManagementId,
                                                            DateOfNotice = reader2["DateOfNotice"] != DBNull.Value ? Convert.ToDateTime(reader2["DateOfNotice"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            NewspaperName = reader2["NewspaperName"].ToString(),
                                                            ResponseOfBorrower = reader2["ResponseOfBorrower"].ToString(),
                                                            NextActionToBeTaken = reader2["NextActionToBeTaken"].ToString(),
                                                            DueDateOfNextAction = reader2["DueDateOfNextAction"] != DBNull.Value ? Convert.ToDateTime(reader2["DueDateOfNextAction"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            FollowUpResultId = reader2["FollowUpResultId"] == DBNull.Value ? 0 : (int)reader2["FollowUpResultId"],
                                                            Remarks = reader2["Remarks"].ToString(),
                                                        };
                                                        sacFollowUpsDetail = sacFollowUpsDetail + (!string.IsNullOrEmpty(sacFollowUpsDetail) ? "," : "") + JsonConvert.SerializeObject(followUp35DaysCallNotice);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 7:
                                        commandText2 = $@"SELECT * FROM FollowUpDetailForAuctionNotice with(nolock) where FollowUpMainId = {followUpMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        FollowUpDetailForAuctionNotice followUpAuctionNotice = new FollowUpDetailForAuctionNotice()
                                                        {
                                                            FollowUpMainId = followUpMainId,
                                                            FollowUpTypeId = followUpTypeId,
                                                            FollowUpById = followUpById,
                                                            NPAManagementId = npaManagementId,
                                                            AuctionNoticeTypeId = reader2["AuctionNoticeTypeId"] == DBNull.Value ? 0 : (int?)reader2["AuctionNoticeTypeId"],
                                                            DateOfAuctionNotice = reader2["DateOfAuctionNotice"] != DBNull.Value ? Convert.ToDateTime(reader2["DateOfAuctionNotice"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            AuctionNewspaperName = reader2["AuctionNewspaperName"].ToString(),
                                                            AuctionResponseOfBorrower = reader2["AuctionResponseOfBorrower"].ToString(),
                                                            AuctionNextActionToBeTaken = reader2["AuctionNextActionToBeTaken"].ToString(),
                                                            AuctionDueDateOfNextAction = reader2["AuctionDueDateOfNextAction"] != DBNull.Value ? Convert.ToDateTime(reader2["AuctionDueDateOfNextAction"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            AuctionRemarks = reader2["AuctionRemarks"].ToString(),
                                                            DateOfRevaluationForAuction = reader2["DateOfRevaluationForAuction"] != DBNull.Value ? Convert.ToDateTime(reader2["DateOfRevaluationForAuction"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            RevaluatedFMVOfCollateralForAuction = reader2["RevaluatedFMVOfCollateralForAuction"] == DBNull.Value ? 0 : (decimal?)reader2["RevaluatedFMVOfCollateralForAuction"],
                                                        };
                                                        sacFollowUpsDetail = sacFollowUpsDetail + (!string.IsNullOrEmpty(sacFollowUpsDetail) ? "," : "") + JsonConvert.SerializeObject(followUpAuctionNotice);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case 8:
                                        commandText2 = $@"SELECT * FROM FollowUpDetailForOthers with(nolock) where FollowUpMainId = {followUpMainId}";
                                        using (SqlDataReader reader2 = SqlHelper.ExecuteReader(connectionString, commandText2,
                                        CommandType.Text))
                                        {
                                            while (reader2.Read())
                                            {
                                                if (reader2.HasRows)
                                                {
                                                    try
                                                    {
                                                        FollowUpDetailForOthers followUpOthers = new FollowUpDetailForOthers()
                                                        {
                                                            FollowUpMainId = followUpMainId,
                                                            FollowUpTypeId = followUpTypeId,
                                                            FollowUpById = followUpById,
                                                            NPAManagementId = npaManagementId,
                                                            ContactDate = reader2["ContactDate"] != DBNull.Value ? Convert.ToDateTime(reader2["ContactDate"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            ResponseOfBorrower = reader2["ResponseOfBorrower"].ToString(),
                                                            NextActionToBeTaken = reader2["NextActionToBeTaken"].ToString(),
                                                            DueDateOfNextAction = reader2["DueDateOfNextAction"] != DBNull.Value ? Convert.ToDateTime(reader2["DueDateOfNextAction"].ToString().Trim()).ToString("yyyy/MM/dd") : "",
                                                            Remarks = reader2["Remarks"].ToString()
                                                        };
                                                        sacFollowUpsDetail = sacFollowUpsDetail + (!string.IsNullOrEmpty(sacFollowUpsDetail) ? "," : "") + JsonConvert.SerializeObject(followUpOthers);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        // Get stack trace for the exception with source file information
                                                        var st = new StackTrace(ex, true);
                                                        // Get the top stack frame
                                                        var frame = st.GetFrame(0);
                                                        // Get the line number from the stack frame
                                                        var line = frame.GetFileLineNumber();
                                                        logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                                                        throw ex;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            // Get stack trace for the exception with source file information
                            var st = new StackTrace(ex, true);
                            // Get the top stack frame
                            var frame = st.GetFrame(0);
                            // Get the line number from the stack frame
                            var line = frame.GetFileLineNumber();
                            logger.Error($"{ex.Message} :: Line Number: {line.ToString()}");
                            throw ex;
                        }
                    }
                }
            }
            return sacFollowUpsDetail == string.Empty ? "[]" : "[" + sacFollowUpsDetail + "]";
        }

        public NPAManagementMain GetGeneralDetailDataFromCIF(string CIFNo)
        {
            NPAManagementMain generalDetail = new NPAManagementMain();
            string commandText = $"SELECT * FROM VW_NPA_CIF_DETAIL WHERE CIF={CIFNo}";
            try
            {
                using (OracleDataReader reader = SqlHelper.ExecuteReaderOracle(connectionStringCBS, commandText, CommandType.Text))
                {
                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            generalDetail = new NPAManagementMain()
                            {
                                BorrowerName = reader["BORROWER_NAME"].ToString(),
                                BorrowerContactNumber = reader["TELEPHONE"].ToString(),
                                BorrowerEmailAddress = reader["E_MAIL"].ToString(),
                                GroupName = reader["GROUP_NAME"].ToString(),
                                BorrowerTypeId = reader["BORROWER_TYPE"].ToString() == "I" ? 1 : (reader["BORROWER_TYPE"].ToString() == "C" ? 2 : (Nullable<int>)null)
                            };
                            if (generalDetail.BorrowerTypeId == 1)
                            {
                                generalDetail.StringifiedBorrowerTypeIndividualLegalDetail =
                                    JsonConvert.SerializeObject(
                                        new BorrowerTypeIndividualDetail()
                                        {
                                            BorrowerTypeId = (int)generalDetail.BorrowerTypeId,
                                            BorrowerFatherName = reader["FATHER_NAME"].ToString(),
                                            BorrowerGrandfatherName = reader["GRANDFATHER_NAME"].ToString(),
                                            BorrowerCitizenshipNumber = reader["REDG_CTZ_NO"].ToString(),
                                            BorrowerSpouseName = reader["SPOUSE_NAME"].ToString()
                                        }); ;
                            }
                            else if (generalDetail.BorrowerTypeId == 2)
                            {
                                generalDetail.StringifiedBorrowerTypeIndividualLegalDetail =
                                    JsonConvert.SerializeObject(
                                        new BorrowerTypeLegalEntitiesDetail()
                                        {
                                            BorrowerTypeId = (int)generalDetail.BorrowerTypeId,
                                            LegalEntityRegistrationNumber = (reader["REDG_CTZ_NO"] != null && reader["REDG_CTZ_NO"] != DBNull.Value) ? reader["REDG_CTZ_NO"].ToString() : "",
                                            LegalEntityRegisteredOffice = reader["REGISTERED_OFFICE"].ToString(),
                                            LegalEntityRegistrationDate = reader["REGISTRATION_DATE"].ToString()
                                        });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return generalDetail;
        }

        public List<string> GetLoanAccountsFromCIF(string CIFNo)
        {
            List<string> loanAccounts = new List<string>();
            string commandText = $"SELECT ACCOUNT_NUMBER FROM VW_NPA_LOAN_DETAIL WHERE CIF={CIFNo}";
            using (OracleDataReader reader = SqlHelper.ExecuteReaderOracle(connectionStringCBS, commandText, CommandType.Text))
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        loanAccounts.Add(reader["ACCOUNT_NUMBER"].ToString());
                    }
                }
            }
            return loanAccounts;
        }

        public LoanAccountDetail GetLoanDetailDataFromCIF(string CIFNo, string LoanAccountNo)
        {
            LoanAccountDetail generalDetail = new LoanAccountDetail();
            string commandText = $"SELECT * FROM VW_NPA_LOAN_DETAIL WHERE CIF='{CIFNo}' AND ACCOUNT_NUMBER='{LoanAccountNo}'";
            using (OracleDataReader reader = SqlHelper.ExecuteReaderOracle(connectionStringCBS, commandText, CommandType.Text))
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        generalDetail = new LoanAccountDetail()
                        {
                            LoanAccountNo = LoanAccountNo,
                            ProductCode = reader["PRODUCT_CODE"].ToString(),
                            LoanTypeId = (reader["LOAN_TYPE"] != null && reader["LOAN_TYPE"].ToString().Trim() != "") ? (reader["LOAN_TYPE"].ToString().Trim().ToUpper() == "FUNDED" ? 1 : 2) : (int?)null,
                            NomineeAccountNo = reader["NOMINEE_ACCOUNT"].ToString(),
                            LoanCurrentStatusId = (reader["LOAN_STATUS"] != null && reader["LOAN_STATUS"].ToString().Trim() != "") ? (reader["LOAN_STATUS"].ToString().Trim().ToUpper() == "OPEN" ? 1 : 2) : (int?)null,
                            RecoveredAmtPrincipal = ((reader["PRINCIPAL_REC"] != null && reader["PRINCIPAL_REC"] != DBNull.Value) ? Convert.ToDecimal(reader["PRINCIPAL_REC"].ToString().Trim()) : 0),
                            RecoveredAmtInterest = ((reader["MAIN_INT_REC"] != null && reader["MAIN_INT_REC"] != DBNull.Value) ? Convert.ToDecimal(reader["MAIN_INT_REC"].ToString().Trim()) : 0),
                            //RecoveredAmtInterestOnInterest = ((reader["CHG_ON_INT_REC"] != null && reader["CHG_ON_INT_REC"] != DBNull.Value) ? Convert.ToDecimal(reader["CHG_ON_INT_REC"].ToString().Trim()) : 0),
                            RecoveredAmtPenalCharges = ((reader["PENAL_REC"] != null && reader["PENAL_REC"] != DBNull.Value) ? Convert.ToDecimal(reader["PENAL_REC"].ToString().Trim()) : 0),
                            RecoveredAmtTotalAmount = ((reader["TOTAL_RECOVERED"] != null && reader["TOTAL_RECOVERED"] != DBNull.Value) ? Convert.ToDecimal(reader["TOTAL_RECOVERED"].ToString().Trim()) : 0),
                            AccruedAmtPrincipal = ((reader["PRINCIPAL_ACCR"] != null && reader["PRINCIPAL_ACCR"] != DBNull.Value) ? Convert.ToDecimal(reader["PRINCIPAL_ACCR"].ToString().Trim()) : 0),
                            AccruedAmtInterest = ((reader["MAIN_INT_ACCR"] != null && reader["MAIN_INT_ACCR"] != DBNull.Value) ? Convert.ToDecimal(reader["MAIN_INT_ACCR"].ToString().Trim()) : 0),
                            //AccruedAmtInterestOnInterest = ((reader["CHG_ON_INT_ACCR"] != null && reader["CHG_ON_INT_ACCR"] != DBNull.Value) ? Convert.ToDecimal(reader["CHG_ON_INT_ACCR"].ToString().Trim()) : 0),
                            AccruedAmtPenalCharges = ((reader["PENAL_ACCR"] != null && reader["PENAL_ACCR"] != DBNull.Value) ? Convert.ToDecimal(reader["PENAL_ACCR"].ToString().Trim()) : 0),
                            AccruedAmtTotalAmount = ((reader["TOTAL_ACCRUED"] != null && reader["TOTAL_ACCRUED"] != DBNull.Value) ? Convert.ToDecimal(reader["TOTAL_ACCRUED"].ToString().Trim()) : 0)
                        };
                    }
                }
            }
            return generalDetail;
        }

        public LoanAccountDetail GetOutStDetailDataFromCIF(string CIFNo, string LoanAccountNo)
        {
            LoanAccountDetail generalDetail = new LoanAccountDetail();
            string commandText = $"SELECT * FROM VW_NPA_OUST_BAL WHERE CIF='{CIFNo}' AND ACCOUNT_NUMBER='{LoanAccountNo}'";
            using (OracleDataReader reader = SqlHelper.ExecuteReaderOracle(connectionStringCBS, commandText, CommandType.Text))
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        generalDetail = new LoanAccountDetail()
                        {
                            LoanAccountNo = LoanAccountNo,
                            OutStPrincipal = ((reader["PRINCIPAL"] != null && reader["PRINCIPAL"] != DBNull.Value) ? Convert.ToDecimal(reader["PRINCIPAL"].ToString().Trim()) : 0),
                            OutStInterest = ((reader["INTEREST"] != null && reader["INTEREST"] != DBNull.Value) ? Convert.ToDecimal(reader["INTEREST"].ToString().Trim()) : 0),
                            OutStAdhocCharges = ((reader["ADHOC"] != null && reader["ADHOC"] != DBNull.Value) ? Convert.ToDecimal(reader["ADHOC"].ToString().Trim()) : 0),
                            OutStPenalCharges = ((reader["PENAL"] != null && reader["PENAL"] != DBNull.Value) ? Convert.ToDecimal(reader["PENAL"].ToString().Trim()) : 0),
                            OutStInterestOnInterest = ((reader["INT_ON_INT"] != null && reader["INT_ON_INT"] != DBNull.Value) ? Convert.ToDecimal(reader["INT_ON_INT"].ToString().Trim()) : 0),
                            OutStTotalAmount = ((reader["TOTAL_OUST_AMOUNT"] != null && reader["TOTAL_OUST_AMOUNT"] != DBNull.Value) ? Convert.ToDecimal(reader["TOTAL_OUST_AMOUNT"].ToString().Trim()) : 0),
                            OutStDate = ((reader["BALANCE_DATE"] != null && reader["BALANCE_DATE"].ToString().Trim() != string.Empty) ? Convert.ToDateTime(reader["BALANCE_DATE"].ToString().Trim()).ToString("yyyy/MM/dd") : null),
                        };
                    }
                }
            }
            return generalDetail;
        }
    }
}