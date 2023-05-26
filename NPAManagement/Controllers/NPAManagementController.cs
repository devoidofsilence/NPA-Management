using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPAManagement.Models;
using NPAManagement.Services;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace NPAManagement.Controllers
{
    [Authorize]
    public class NPAManagementController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        readonly CommonService cs = null;
        readonly NPAManagementService nms = null;
        public NPAManagementController()
        {
            cs = new CommonService();
            nms = new NPAManagementService();
        }
        // GET: NPAManagement
        public ActionResult NPAManagementList()
        {
            LoggedInUser user = LoginCheck.CheckIfUserIsRegisteredInSystem(User.Identity.Name);
            if (user != null && user.UserActiveFlag)
            {
                if (TempData["SuccessMessage"] != null)
                {
                    ViewBag.SuccessMessage = TempData["SuccessMessage"];
                }
                if (TempData["ErrorMessage"] != null)
                {
                    ViewBag.ErrorMessage = TempData["ErrorMessage"];
                }
                List<NPAManagementMain> lstNPAs;
                lstNPAs = nms.GetSavedNPAList();
                return View(lstNPAs);
            }
            if (User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
            }
            return RedirectToAction("Login", "Account");
        }

        // GET: NPAManagement/Create
        public ActionResult NPAManagementCreate()
        {
            NPAManagementMain nmmModel = new NPAManagementMain();
            LoggedInUser user = LoginCheck.CheckIfUserIsRegisteredInSystem(User.Identity.Name);
            if (user != null && user.UserActiveFlag)
            {
                ViewData["SBLEmployees"] = cs.GetEmployeesFromAD();
                ViewData["Provinces"] = cs.GetDdlKeyValuePairs("Province", "ProvinceCode", "ProvinceName");
                ViewData["Branches"] = cs.GetDdlKeyValuePairs("Branches", "LocationCode", "BranchName");
                ViewData["BorrowerTypes"] = cs.GetDdlKeyValuePairs("BorrowerType", "BorrowerTypeId", "BorrowerTypeName");
                ViewData["CollateralTypes"] = cs.GetDdlKeyValuePairs("CollateralType", "CollateralTypeId", "CollateralTypeName");
                ViewData["ProvisionStatuses"] = cs.GetDdlKeyValuePairs("ProvisionStatus", "ProvisionStatusId", "ProvisionStatusName");
                ViewData["AuctionFollowUpResults"] = cs.GetDdlKeyValuePairs("AuctionFollowUpResult", "AuctionFollowUpResultId", "AuctionFollowUpResultName");
                ViewData["RequestStatuses"] = cs.GetDdlKeyValuePairs("RequestStatus", "RequestStatusId", "RequestStatusName");
                nmmModel.LoggedInUserType = user.UserRole;
                //nmmModel.ViewOnlyUser = user.ViewOnlyUser;
                return View(nmmModel);
            }
            if (User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
            }
            return RedirectToAction("Login", "Account");
        }

        // POST: NPAManagement/Create
        [HttpPost]
        public ActionResult NPAManagementCreate(NPAManagementMain nmm)
        {
            logger.Info("Creating NPA");
            try
            {
                nmm.EnteredBy = User.Identity.Name;
                nmm.EnteredOn = DateTime.Now.ToString();

                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    if (!CheckCIFDuplication("I", nmm.CIFNo.Trim()))
                    {
                        int operatedInt = nms.SaveData(nmm);

                        if (operatedInt > 0)
                        {
                            TempData["SuccessMessage"] = "User created successfully";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Some error occured";
                        }
                    }
                    else
                    {
                        ViewData["Provinces"] = cs.GetDdlKeyValuePairs("Province", "ProvinceCode", "ProvinceName");
                        ViewData["BorrowerTypes"] = cs.GetDdlKeyValuePairs("BorrowerType", "BorrowerTypeId", "BorrowerTypeName");
                        ViewData["ProvisionStatuses"] = cs.GetDdlKeyValuePairs("ProvisionStatus", "ProvisionStatusId", "ProvisionStatusName");
                        ViewData["AuctionFollowUpResults"] = cs.GetDdlKeyValuePairs("AuctionFollowUpResult", "AuctionFollowUpResultId", "AuctionFollowUpResultName");
                        ViewData["RequestStatuses"] = cs.GetDdlKeyValuePairs("RequestStatus", "RequestStatusId", "RequestStatusName");

                        ViewData["SBLEmployees"] = cs.GetEmployeesFromAD();
                        ViewData["Branches"] = cs.GetDdlKeyValuePairs("Branches", "LocationCode", "BranchName");
                        ViewData["CollateralTypes"] = cs.GetDdlKeyValuePairs("CollateralType", "CollateralTypeId", "CollateralTypeName");
                        ViewBag.ErrorMessage = "CIF already entered in system";
                        return View(nmm);
                    }
                }
                else
                {
                    ViewData["Provinces"] = cs.GetDdlKeyValuePairs("Province", "ProvinceCode", "ProvinceName");
                    ViewData["BorrowerTypes"] = cs.GetDdlKeyValuePairs("BorrowerType", "BorrowerTypeId", "BorrowerTypeName");
                    ViewData["ProvisionStatuses"] = cs.GetDdlKeyValuePairs("ProvisionStatus", "ProvisionStatusId", "ProvisionStatusName");
                    ViewData["AuctionFollowUpResults"] = cs.GetDdlKeyValuePairs("AuctionFollowUpResult", "AuctionFollowUpResultId", "AuctionFollowUpResultName");
                    ViewData["RequestStatuses"] = cs.GetDdlKeyValuePairs("RequestStatus", "RequestStatusId", "RequestStatusName");
                    ViewData["SBLEmployees"] = cs.GetEmployeesFromAD();
                    ViewData["Branches"] = cs.GetDdlKeyValuePairs("Branches", "LocationCode", "BranchName");
                    ViewData["CollateralTypes"] = cs.GetDdlKeyValuePairs("CollateralType", "CollateralTypeId", "CollateralTypeName");
                    return View(nmm);
                }
                return RedirectToAction("NPAManagementList");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult NPAManagementEdit(int npaManagementId)
        {
            logger.Info("Editing NPA");
            LoggedInUser user = LoginCheck.CheckIfUserIsRegisteredInSystem(User.Identity.Name);
            if (user != null && user.UserActiveFlag)
            {
                NPAManagementMain nmmModel = nms.GetSavedNPADetail(npaManagementId);
                ViewData["SBLEmployees"] = cs.GetEmployeesFromAD();
                ViewData["Provinces"] = cs.GetDdlKeyValuePairs("Province", "ProvinceCode", "ProvinceName");
                ViewData["Branches"] = cs.GetDdlKeyValuePairs("Branches", "LocationCode", "BranchName");
                ViewData["CollateralTypes"] = cs.GetDdlKeyValuePairs("CollateralType", "CollateralTypeId", "CollateralTypeName");
                ViewData["PermanentAddressProvinces"] = cs.GetDdlKeyValuePairs("Province", "ProvinceCode", "ProvinceName", nmmModel.BorrowerPermanentAddressProvinceId.ToString());
                ViewData["PermanentAddressZones"] = cs.GetDdlKeyValuePairsCascade("Zone", "ZoneCode", "ZoneName", "ProvinceCode", nmmModel.BorrowerPermanentAddressProvinceId.ToString(), nmmModel.BorrowerPermanentAddressZoneId.ToString());
                ViewData["PermanentAddressDistricts"] = cs.GetDdlKeyValuePairsCascade("District", "DistrictCode", "DistrictName", "ZoneCode", nmmModel.BorrowerPermanentAddressZoneId.ToString(), nmmModel.BorrowerPermanentAddressDistrictId.ToString());
                //ViewData["PermanentAddressVDCMuns"] = cs.GetDdlKeyValuePairsCascade("AreaMunicipality", "AreaCode", "AreaName", "DistrictCode", nmmModel.BorrowerPermanentAddressDistrictId.ToString(), nmmModel.BorrowerPermanentAddressVDCMunId.ToString());

                ViewData["TemporaryAddressProvinces"] = cs.GetDdlKeyValuePairs("Province", "ProvinceCode", "ProvinceName", nmmModel.BorrowerTemporaryAddressProvinceId.ToString());
                ViewData["TemporaryAddressZones"] = cs.GetDdlKeyValuePairsCascade("Zone", "ZoneCode", "ZoneName", "ProvinceCode", nmmModel.BorrowerTemporaryAddressProvinceId.ToString(), nmmModel.BorrowerTemporaryAddressZoneId.ToString());
                ViewData["TemporaryAddressDistricts"] = cs.GetDdlKeyValuePairsCascade("District", "DistrictCode", "DistrictName", "ZoneCode", nmmModel.BorrowerTemporaryAddressZoneId.ToString(), nmmModel.BorrowerTemporaryAddressDistrictId.ToString());
                //ViewData["TemporaryAddressVDCMuns"] = cs.GetDdlKeyValuePairsCascade("AreaMunicipality", "AreaCode", "AreaName", "DistrictCode", nmmModel.BorrowerTemporaryAddressDistrictId.ToString(), nmmModel.BorrowerTemporaryAddressVDCMunId.ToString());

                ViewData["BorrowerTypes"] = cs.GetDdlKeyValuePairs("BorrowerType", "BorrowerTypeId", "BorrowerTypeName");
                ViewData["ProvisionStatuses"] = cs.GetDdlKeyValuePairs("ProvisionStatus", "ProvisionStatusId", "ProvisionStatusName");
                ViewData["AuctionFollowUpResults"] = cs.GetDdlKeyValuePairs("AuctionFollowUpResult", "AuctionFollowUpResultId", "AuctionFollowUpResultName");
                ViewData["RequestStatuses"] = cs.GetDdlKeyValuePairs("RequestStatus", "RequestStatusId", "RequestStatusName");
                nmmModel.LoggedInUserType = user.UserRole;
                nmmModel.ViewOnlyUser = (user.Username.ToLower().Trim() != nmmModel.CurrentROName.ToLower().Trim()) && user.UserRole != "S";
                return View(nmmModel);
            }
            if (User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public ActionResult NPAManagementEdit(NPAManagementMain nmm)
        {
            try
            {
                nmm.UpdatedBy = User.Identity.Name;
                nmm.UpdatedOn = DateTime.Now.ToString();

                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    if (!CheckCIFDuplication("U", nmm.CIFNo.Trim(), (int)nmm.NPAManagementId))
                    {
                        int operatedInt = nms.UpdateData(nmm);
                        if (operatedInt > 0)
                        {
                            TempData["SuccessMessage"] = "User updated successfully";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Some error occured";
                        }
                    }
                    else
                    {
                        ViewData["Provinces"] = cs.GetDdlKeyValuePairs("Province", "ProvinceCode", "ProvinceName");

                        ViewData["PermanentAddressProvinces"] = cs.GetDdlKeyValuePairs("Province", "ProvinceCode", "ProvinceName", nmm.BorrowerPermanentAddressProvinceId.ToString());
                        ViewData["PermanentAddressZones"] = cs.GetDdlKeyValuePairsCascade("Zone", "ZoneCode", "ZoneName", "ProvinceCode", nmm.BorrowerPermanentAddressProvinceId.ToString(), nmm.BorrowerPermanentAddressZoneId.ToString());
                        ViewData["PermanentAddressDistricts"] = cs.GetDdlKeyValuePairsCascade("District", "DistrictCode", "DistrictName", "ZoneCode", nmm.BorrowerPermanentAddressZoneId.ToString(), nmm.BorrowerPermanentAddressDistrictId.ToString());
                        //ViewData["PermanentAddressVDCMuns"] = cs.GetDdlKeyValuePairsCascade("AreaMunicipality", "AreaCode", "AreaName", "DistrictCode", nmm.BorrowerPermanentAddressDistrictId.ToString(), nmm.BorrowerPermanentAddressVDCMunId.ToString());

                        ViewData["TemporaryAddressProvinces"] = cs.GetDdlKeyValuePairs("Province", "ProvinceCode", "ProvinceName", nmm.BorrowerTemporaryAddressProvinceId.ToString());
                        ViewData["TemporaryAddressZones"] = cs.GetDdlKeyValuePairsCascade("Zone", "ZoneCode", "ZoneName", "ProvinceCode", nmm.BorrowerTemporaryAddressProvinceId.ToString(), nmm.BorrowerTemporaryAddressZoneId.ToString());
                        ViewData["TemporaryAddressDistricts"] = cs.GetDdlKeyValuePairsCascade("District", "DistrictCode", "DistrictName", "ZoneCode", nmm.BorrowerTemporaryAddressZoneId.ToString(), nmm.BorrowerTemporaryAddressDistrictId.ToString());
                        //ViewData["TemporaryAddressVDCMuns"] = cs.GetDdlKeyValuePairsCascade("AreaMunicipality", "AreaCode", "AreaName", "DistrictCode", nmm.BorrowerTemporaryAddressDistrictId.ToString(), nmm.BorrowerTemporaryAddressVDCMunId.ToString());

                        ViewData["BorrowerTypes"] = cs.GetDdlKeyValuePairs("BorrowerType", "BorrowerTypeId", "BorrowerTypeName");
                        ViewData["ProvisionStatuses"] = cs.GetDdlKeyValuePairs("ProvisionStatus", "ProvisionStatusId", "ProvisionStatusName");
                        ViewData["AuctionFollowUpResults"] = cs.GetDdlKeyValuePairs("AuctionFollowUpResult", "AuctionFollowUpResultId", "AuctionFollowUpResultName");
                        ViewData["RequestStatuses"] = cs.GetDdlKeyValuePairs("RequestStatus", "RequestStatusId", "RequestStatusName");

                        ViewData["SBLEmployees"] = cs.GetEmployeesFromAD();
                        ViewData["Branches"] = cs.GetDdlKeyValuePairs("Branches", "LocationCode", "BranchName");
                        ViewData["CollateralTypes"] = cs.GetDdlKeyValuePairs("CollateralType", "CollateralTypeId", "CollateralTypeName");
                        ViewBag.ErrorMessage = "CIF already entered in system";
                        return View(nmm);
                    }
                }
                else
                {
                    ViewData["Provinces"] = cs.GetDdlKeyValuePairs("Province", "ProvinceCode", "ProvinceName");

                    ViewData["PermanentAddressProvinces"] = cs.GetDdlKeyValuePairs("Province", "ProvinceCode", "ProvinceName", nmm.BorrowerPermanentAddressProvinceId.ToString());
                    ViewData["PermanentAddressZones"] = cs.GetDdlKeyValuePairsCascade("Zone", "ZoneCode", "ZoneName", "ProvinceCode", nmm.BorrowerPermanentAddressProvinceId.ToString(), nmm.BorrowerPermanentAddressZoneId.ToString());
                    ViewData["PermanentAddressDistricts"] = cs.GetDdlKeyValuePairsCascade("District", "DistrictCode", "DistrictName", "ZoneCode", nmm.BorrowerPermanentAddressZoneId.ToString(), nmm.BorrowerPermanentAddressDistrictId.ToString());
                    //ViewData["PermanentAddressVDCMuns"] = cs.GetDdlKeyValuePairsCascade("AreaMunicipality", "AreaCode", "AreaName", "DistrictCode", nmm.BorrowerPermanentAddressDistrictId.ToString(), nmm.BorrowerPermanentAddressVDCMunId.ToString());

                    ViewData["TemporaryAddressProvinces"] = cs.GetDdlKeyValuePairs("Province", "ProvinceCode", "ProvinceName", nmm.BorrowerTemporaryAddressProvinceId.ToString());
                    ViewData["TemporaryAddressZones"] = cs.GetDdlKeyValuePairsCascade("Zone", "ZoneCode", "ZoneName", "ProvinceCode", nmm.BorrowerTemporaryAddressProvinceId.ToString(), nmm.BorrowerTemporaryAddressZoneId.ToString());
                    ViewData["TemporaryAddressDistricts"] = cs.GetDdlKeyValuePairsCascade("District", "DistrictCode", "DistrictName", "ZoneCode", nmm.BorrowerTemporaryAddressZoneId.ToString(), nmm.BorrowerTemporaryAddressDistrictId.ToString());
                    //ViewData["TemporaryAddressVDCMuns"] = cs.GetDdlKeyValuePairsCascade("AreaMunicipality", "AreaCode", "AreaName", "DistrictCode", nmm.BorrowerTemporaryAddressDistrictId.ToString(), nmm.BorrowerTemporaryAddressVDCMunId.ToString());

                    ViewData["BorrowerTypes"] = cs.GetDdlKeyValuePairs("BorrowerType", "BorrowerTypeId", "BorrowerTypeName");
                    ViewData["ProvisionStatuses"] = cs.GetDdlKeyValuePairs("ProvisionStatus", "ProvisionStatusId", "ProvisionStatusName");
                    ViewData["AuctionFollowUpResults"] = cs.GetDdlKeyValuePairs("AuctionFollowUpResult", "AuctionFollowUpResultId", "AuctionFollowUpResultName");
                    ViewData["RequestStatuses"] = cs.GetDdlKeyValuePairs("RequestStatus", "RequestStatusId", "RequestStatusName");

                    ViewData["SBLEmployees"] = cs.GetEmployeesFromAD();
                    ViewData["Branches"] = cs.GetDdlKeyValuePairs("Branches", "LocationCode", "BranchName");
                    ViewData["CollateralTypes"] = cs.GetDdlKeyValuePairs("CollateralType", "CollateralTypeId", "CollateralTypeName");
                    return View(nmm);
                }
                return RedirectToAction("NPAManagementList");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PartialViewResult GetIndividualBorrowerDetail()
        {
            return PartialView("_BorrowerIndividualDetail", new BorrowerTypeIndividualDetail());
        }

        public PartialViewResult GetIndividualBorrowerDetailForEdit(string unparsedDetail, bool viewOnlyUser)
        {
            BorrowerTypeIndividualDetail parsedIndividualBorrowerDetailArray = new BorrowerTypeIndividualDetail();
            if (unparsedDetail != null && unparsedDetail != "" && unparsedDetail != "[]" && unparsedDetail != "[{}]")
            {
                parsedIndividualBorrowerDetailArray = JsonConvert.DeserializeObject<BorrowerTypeIndividualDetail>(unparsedDetail);

            }
            ViewBag.ViewOnlyUser = viewOnlyUser;
            return PartialView("_BorrowerIndividualDetail", parsedIndividualBorrowerDetailArray);
        }

        public PartialViewResult GetLegalEntityBorrowerDetail()
        {
            return PartialView("_BorrowerLegalEntityDetail", new BorrowerTypeLegalEntitiesDetail());
        }

        public PartialViewResult GetLegalEntityBorrowerDetailForEdit(string unparsedDetail, bool viewOnlyUser)
        {
            BorrowerTypeLegalEntitiesDetail parsedLegalBorrowerDetailArray = new BorrowerTypeLegalEntitiesDetail();
            if (unparsedDetail != null && unparsedDetail != "" && unparsedDetail != "[]" && unparsedDetail != "[{}]")
            {
                parsedLegalBorrowerDetailArray = JsonConvert.DeserializeObject<BorrowerTypeLegalEntitiesDetail>(unparsedDetail);

            }
            ViewBag.ViewOnlyUser = viewOnlyUser;
            return PartialView("_BorrowerLegalEntityDetail", parsedLegalBorrowerDetailArray);
        }

        public PartialViewResult GetLoanAccountDetail()
        {
            ViewData["Departments"] = cs.GetDdlKeyValuePairs("Departments", "DepartmentId", "DepartmentName");
            ViewData["Provinces"] = cs.GetDdlKeyValuePairs("Province", "ProvinceCode", "ProvinceName");
            ViewData["LoanTypes"] = cs.GetDdlKeyValuePairs("LoanType", "LoanTypeId", "LoanTypeName");
            ViewData["LoanNatures"] = cs.GetDdlKeyValuePairs("LoanNature", "LoanNatureId", "LoanNatureName");
            ViewData["LoanProducts"] = cs.GetDdlKeyValuePairs("LoanProduct", "LoanProductCode", "LoanProductName");
            ViewData["CurrentStatusOfLoan"] = cs.GetDdlKeyValuePairs("LoanCurrentStatus", "LoanCurrentStatusId", "LoanCurrentStatusName");
            ViewData["LoanStatuses"] = cs.GetDdlKeyValuePairs("LoanStatus", "LoanStatusId", "LoanStatusName");
            ViewData["ProvisionPercentages"] = cs.GetDdlKeyValuePairs("ProvisionPercentage", "ProvisionPercentageId", "ProvisionPercentageName");
            return PartialView("_LoanAccountDetail", new LoanAccountDetail() { });
        }

        public PartialViewResult GetLoanAccountDetailForEdit(string unparsedDetail, bool viewOnlyUser)
        {
            LoanAccountDetail parsedLoanAccountDetailArray = new LoanAccountDetail();
            if (unparsedDetail != null && unparsedDetail != "" && unparsedDetail != "[]" && unparsedDetail != "[{}]")
            {
                parsedLoanAccountDetailArray = JsonConvert.DeserializeObject<LoanAccountDetail>(unparsedDetail);
            }
            ViewData["Departments"] = cs.GetDdlKeyValuePairs("Departments", "DepartmentId", "DepartmentName");
            ViewData["Provinces"] = cs.GetDdlKeyValuePairs("Province", "ProvinceCode", "ProvinceName");
            ViewData["LoanTypes"] = cs.GetDdlKeyValuePairs("LoanType", "LoanTypeId", "LoanTypeName");
            ViewData["LoanNatures"] = cs.GetDdlKeyValuePairs("LoanNature", "LoanNatureId", "LoanNatureName");
            ViewData["LoanProducts"] = cs.GetDdlKeyValuePairs("LoanProduct", "LoanProductCode", "LoanProductName");
            ViewData["CurrentStatusOfLoan"] = cs.GetDdlKeyValuePairs("LoanCurrentStatus", "LoanCurrentStatusId", "LoanCurrentStatusName");
            ViewData["LoanStatuses"] = cs.GetDdlKeyValuePairs("LoanStatus", "LoanStatusId", "LoanStatusName");
            ViewData["ProvisionPercentages"] = cs.GetDdlKeyValuePairs("ProvisionPercentage", "ProvisionPercentageId", "ProvisionPercentageName");
            ViewBag.ViewOnlyUser = viewOnlyUser;
            return PartialView("_LoanAccountDetailForEdit", parsedLoanAccountDetailArray);
        }

        public PartialViewResult GetFollowUpPanel(int followUpById)
        {
            ViewData["FollowUpTypes"] = cs.GetDdlKeyValuePairs("FollowUpType", "FollowUpTypeId", "FollowUpTypeName");
            return PartialView("_FollowUpPanel", new FollowUpMain() { FollowUpById = followUpById });
        }

        public PartialViewResult GetFollowUpPanelForEditForBranchFollowUp(string unparsedDetail, string userRole, bool viewOnlyUser)
        {
            ViewData["FollowUpTypes"] = cs.GetDdlKeyValuePairs("FollowUpType", "FollowUpTypeId", "FollowUpTypeName");
            FollowUpMain fum = new FollowUpMain();
            if (unparsedDetail != null && unparsedDetail != "" && unparsedDetail != "[]" && unparsedDetail != "[{}]" && unparsedDetail != "{\"FollowUpTypeId\":\"\"}")
            {
                fum = JsonConvert.DeserializeObject<FollowUpMain>(unparsedDetail);
                fum.FollowUpById = 1;
                fum.StringifiedFollowUpDetail = unparsedDetail;
                fum.LoggedInUserType = userRole;
                ViewBag.ViewOnlyUser = viewOnlyUser;
                fum.EnableEditingOnThisPanel = (userRole == "B" && fum.FollowUpById == 1 && !viewOnlyUser);
            }
            return PartialView("_FollowUpPanelWithDetail", fum);
        }

        public PartialViewResult GetFollowUpPanelForEditForSACFollowUp(string unparsedDetail, string userRole, bool viewOnlyUser)
        {
            ViewData["FollowUpTypes"] = cs.GetDdlKeyValuePairs("FollowUpType", "FollowUpTypeId", "FollowUpTypeName");
            FollowUpMain fum = new FollowUpMain();
            if (unparsedDetail != null && unparsedDetail != "" && unparsedDetail != "[]" && unparsedDetail != "[{}]" && unparsedDetail != "{\"FollowUpTypeId\":\"\"}")
            {
                fum = JsonConvert.DeserializeObject<FollowUpMain>(unparsedDetail);
                fum.FollowUpById = 2;
                fum.StringifiedFollowUpDetail = unparsedDetail;
                fum.LoggedInUserType = userRole;
                ViewBag.ViewOnlyUser = viewOnlyUser;
                fum.EnableEditingOnThisPanel = (userRole == "S" && fum.FollowUpById == 2 && !viewOnlyUser);
            }
            return PartialView("_FollowUpPanelWithDetail", fum);
        }

        public PartialViewResult GetFollowUpDetailCardAccordingToFollowUpType(string followUpTypeId)
        {
            string detailPartialView = string.Empty;
            dynamic model = null;
            ViewData["FollowUpResults"] = cs.GetDdlKeyValuePairs("FollowUpResult", "FollowUpResultId", "FollowUpResultName");
            switch (followUpTypeId)
            {
                case "1":
                    detailPartialView = "_FollowUpDetailTelephone";
                    model = new FollowUpDetailByTelephone() { EnableControls = true };
                    break;
                case "2":
                    detailPartialView = "_FollowUpDetailInPersonVisit";
                    model = new FollowUpDetailByInPersonVisit() { EnableControls = true };
                    break;
                case "3":
                    detailPartialView = "_FollowUpDetail15DaysLetter";
                    model = new FollowUpDetailFor15DaysLetter() { EnableControls = true };
                    break;
                case "4":
                    detailPartialView = "_FollowUpDetailFirst7DaysLetter";
                    model = new FollowUpDetailForFirst7DaysLetter() { EnableControls = true };
                    break;
                case "5":
                    detailPartialView = "_FollowUpDetailSecond7DaysLetter";
                    model = new FollowUpDetailForSecond7DaysLetter() { EnableControls = true };
                    break;
                case "6":
                    detailPartialView = "_FollowUpDetail35DaysCallNotice";
                    model = new FollowUpDetailFor35DaysCallNotice() { EnableControls = true };
                    break;
                case "7":
                    detailPartialView = "_FollowUpDetailAuctionNotice";
                    ViewData["AuctionNoticeTypes"] = cs.GetDdlKeyValuePairs("AuctionNoticeType", "AuctionNoticeTypeId", "AuctionNoticeTypeName");
                    model = new FollowUpDetailForAuctionNotice() { EnableControls = true };
                    break;
                case "8":
                    detailPartialView = "_FollowUpDetailOthers";
                    model = new FollowUpDetailForOthers() { EnableControls = true };
                    break;
                default:
                    break;
            }
            return PartialView(detailPartialView, model);
        }

        public PartialViewResult GetFollowUpDetailCardAccordingToFollowUpTypeForEdit(string followUpTypeId, string unparsedFollowUpDetail, bool enableControls)
        {
            string detailPartialView = string.Empty;
            dynamic model = null;
            ViewData["FollowUpResults"] = cs.GetDdlKeyValuePairs("FollowUpResult", "FollowUpResultId", "FollowUpResultName");
            switch (followUpTypeId)
            {
                case "1":
                    detailPartialView = "_FollowUpDetailTelephone";
                    model = new FollowUpDetailByTelephone() { };
                    if (unparsedFollowUpDetail != null && unparsedFollowUpDetail != "" && unparsedFollowUpDetail != "[]" && unparsedFollowUpDetail != "[{}]")
                    {
                        model = JsonConvert.DeserializeObject<FollowUpDetailByTelephone>(unparsedFollowUpDetail);
                        model.EnableControls = enableControls;
                    }
                    break;
                case "2":
                    detailPartialView = "_FollowUpDetailInPersonVisit";
                    model = new FollowUpDetailByInPersonVisit() { };
                    if (unparsedFollowUpDetail != null && unparsedFollowUpDetail != "" && unparsedFollowUpDetail != "[]" && unparsedFollowUpDetail != "[{}]")
                    {
                        model = JsonConvert.DeserializeObject<FollowUpDetailByInPersonVisit>(unparsedFollowUpDetail);
                        model.EnableControls = enableControls;
                    }
                    break;
                case "3":
                    detailPartialView = "_FollowUpDetail15DaysLetter";
                    model = new FollowUpDetailFor15DaysLetter() { };
                    if (unparsedFollowUpDetail != null && unparsedFollowUpDetail != "" && unparsedFollowUpDetail != "[]" && unparsedFollowUpDetail != "[{}]")
                    {
                        model = JsonConvert.DeserializeObject<FollowUpDetailFor15DaysLetter>(unparsedFollowUpDetail);
                        model.EnableControls = enableControls;
                    }
                    break;
                case "4":
                    detailPartialView = "_FollowUpDetailFirst7DaysLetter";
                    model = new FollowUpDetailForFirst7DaysLetter() { };
                    if (unparsedFollowUpDetail != null && unparsedFollowUpDetail != "" && unparsedFollowUpDetail != "[]" && unparsedFollowUpDetail != "[{}]")
                    {
                        model = JsonConvert.DeserializeObject<FollowUpDetailForFirst7DaysLetter>(unparsedFollowUpDetail);
                        model.EnableControls = enableControls;
                    }
                    break;
                case "5":
                    detailPartialView = "_FollowUpDetailSecond7DaysLetter";
                    model = new FollowUpDetailForSecond7DaysLetter() { };
                    if (unparsedFollowUpDetail != null && unparsedFollowUpDetail != "" && unparsedFollowUpDetail != "[]" && unparsedFollowUpDetail != "[{}]")
                    {
                        model = JsonConvert.DeserializeObject<FollowUpDetailForSecond7DaysLetter>(unparsedFollowUpDetail);
                        model.EnableControls = enableControls;
                    }
                    break;
                case "6":
                    detailPartialView = "_FollowUpDetail35DaysCallNotice";
                    model = new FollowUpDetailFor35DaysCallNotice() { };
                    if (unparsedFollowUpDetail != null && unparsedFollowUpDetail != "" && unparsedFollowUpDetail != "[]" && unparsedFollowUpDetail != "[{}]")
                    {
                        model = JsonConvert.DeserializeObject<FollowUpDetailFor35DaysCallNotice>(unparsedFollowUpDetail);
                        model.EnableControls = enableControls;
                    }
                    break;
                case "7":
                    detailPartialView = "_FollowUpDetailAuctionNotice";
                    ViewData["AuctionNoticeTypes"] = cs.GetDdlKeyValuePairs("AuctionNoticeType", "AuctionNoticeTypeId", "AuctionNoticeTypeName");
                    model = new FollowUpDetailForAuctionNotice() { };
                    if (unparsedFollowUpDetail != null && unparsedFollowUpDetail != "" && unparsedFollowUpDetail != "[]" && unparsedFollowUpDetail != "[{}]")
                    {
                        model = JsonConvert.DeserializeObject<FollowUpDetailForAuctionNotice>(unparsedFollowUpDetail);
                        model.EnableControls = enableControls;
                    }
                    break;
                case "8":
                    detailPartialView = "_FollowUpDetailOthers";
                    model = new FollowUpDetailForOthers() { };
                    if (unparsedFollowUpDetail != null && unparsedFollowUpDetail != "" && unparsedFollowUpDetail != "[]" && unparsedFollowUpDetail != "[{}]")
                    {
                        model = JsonConvert.DeserializeObject<FollowUpDetailForOthers>(unparsedFollowUpDetail);
                        model.EnableControls = enableControls;
                    }
                    break;
                default:
                    break;
            }
            return PartialView(detailPartialView, model);
        }

        public PartialViewResult GetCollateralAddPanel(string collateralClass)
        {
            ViewData["CollateralTypes"] = cs.GetDdlKeyValuePairs("CollateralType", "CollateralTypeId", "CollateralTypeName");
            return PartialView("_CollateralDetailPanel", new CollateralDetailMain() { CollateralClass = collateralClass });
        }

        public PartialViewResult GetPrimaryCollateralPanelForEdit(string unparsedDetail, bool viewOnlyUser)
        {
            ViewData["CollateralTypes"] = cs.GetDdlKeyValuePairs("CollateralType", "CollateralTypeId", "CollateralTypeName");
            CollateralDetailMain cdm = new CollateralDetailMain();
            if (unparsedDetail != null && unparsedDetail != "" && unparsedDetail != "[]" && unparsedDetail != "[{}]")
            {
                cdm = JsonConvert.DeserializeObject<CollateralDetailMain>(unparsedDetail);
                cdm.CollateralClass = "1";
                cdm.StringifiedCollateralDetail = unparsedDetail;
            }
            ViewBag.ViewOnlyUser = viewOnlyUser;
            return PartialView("_CollateralDetailPanelWithDetail", cdm);
        }

        public PartialViewResult GetSecondaryCollateralPanelForEdit(string unparsedDetail, bool viewOnlyUser)
        {
            ViewData["CollateralTypes"] = cs.GetDdlKeyValuePairs("CollateralType", "CollateralTypeId", "CollateralTypeName");
            CollateralDetailMain cdm = new CollateralDetailMain();
            if (unparsedDetail != null && unparsedDetail != "" && unparsedDetail != "[]" && unparsedDetail != "[{}]")
            {
                cdm = JsonConvert.DeserializeObject<CollateralDetailMain>(unparsedDetail);
                cdm.CollateralClass = "2";
                cdm.StringifiedCollateralDetail = unparsedDetail;
            }
            ViewBag.ViewOnlyUser = viewOnlyUser;
            return PartialView("_CollateralDetailPanelWithDetail", cdm);
        }

        public PartialViewResult GetCollateralDetailCardAccordingToCollateralType(int collateralTypeId, string collateralClass)
        {
            string detailPartialView = string.Empty;
            dynamic model = null;
            SelectListItem firstItem = new SelectListItem() { Value = "", Text = "-- Please Select --" };
            switch (collateralTypeId)
            {
                case 1:
                    detailPartialView = "_CollateralDetailLand";
                    ViewData["Provinces"] = cs.GetDdlKeyValuePairs("Province", "ProvinceCode", "ProvinceName");
                    ViewData["Zones"] = new List<SelectListItem>() { firstItem };
                    ViewData["Districts"] = new List<SelectListItem>() { firstItem };
                    //ViewData["VDCMuns"] = new List<SelectListItem>() { firstItem };
                    ViewData["AreaTypes"] = cs.GetDdlKeyValuePairs("AreaType", "AreaTypeId", "AreaTypeName");
                    model = new CollateralDetailInCaseOfLand() { CollateralDetailMainId = 1 };
                    break;
                case 2:
                    detailPartialView = "_CollateralDetailBuilding";
                    ViewData["Provinces"] = cs.GetDdlKeyValuePairs("Province", "ProvinceCode", "ProvinceName");
                    ViewData["Zones"] = new List<SelectListItem>() { firstItem };
                    ViewData["Districts"] = new List<SelectListItem>() { firstItem };
                    //ViewData["VDCMuns"] = new List<SelectListItem>() { firstItem };
                    ViewData["AreaTypes"] = cs.GetDdlKeyValuePairs("AreaType", "AreaTypeId", "AreaTypeName");
                    model = new CollateralDetailInCaseOfBuilding() { };
                    break;
                case 3:
                    detailPartialView = "_CollateralDetailVehicle";
                    ViewData["VehicleTypes"] = cs.GetDdlKeyValuePairs("VehicleType", "VehicleTypeId", "VehicleTypeName");
                    model = new CollateralDetailInCaseOfVehicle() { };
                    break;
                case 4:
                    detailPartialView = "_CollateralDetailPlantAndMachinery";
                    model = new CollateralDetailInCaseOfPlantAndMachinery() { };
                    break;
                case 5:
                    detailPartialView = "_CollateralDetailCurrentAsset";
                    model = new CollateralDetailInCaseOfCurrentAsset() { };
                    break;
                case 6:
                    detailPartialView = "_CollateralDetailPersonalGuarantee";
                    model = new CollateralDetailInCaseOfPersonalGuarantee() { };
                    break;
                case 7:
                    detailPartialView = "_CollateralDetailCorporateGuarantee";
                    model = new CollateralDetailInCaseOfCorporateGuarantee() { };
                    break;
                case 8:
                    detailPartialView = "_CollateralDetailStock";
                    ViewData["ShareType"] = cs.GetDdlKeyValuePairs("ShareType", "ShareTypeId", "ShareTypeName");
                    ViewData["ListedInNepse"] = cs.GetDdlKeyValuePairs("NepseListed", "NepseListedBoolId", "NepseListedBoolName");
                    ViewData["ShareUnitType"] = cs.GetDdlKeyValuePairs("ShareUnitType", "ShareUnitTypeId", "ShareUnitTypeName");
                    model = new CollateralDetailInCaseOfStock() { CollateralDetailMainId = 1 };
                    break;
                case 9:
                    detailPartialView = "_CollateralDetailGold";
                    ViewData["MeasurementUnit"] = cs.GetDdlKeyValuePairs("MeasurementUnit", "MeasurementUnitId", "MeasurementUnitName");
                    ViewData["GoldForm"] = cs.GetDdlKeyValuePairs("GoldForm", "GoldFormId", "GoldFormName");
                    model = new CollateralDetailInCaseOfGold() { CollateralDetailMainId = 1 };
                    break;
                default:
                    break;
            }
            return PartialView(detailPartialView, model);
        }

        public PartialViewResult GetCollateralDetailCardAccordingToCollateralTypeForEdit(int collateralTypeId, string unparsedCollateralDetail, bool viewOnlyUser)
        {
            string detailPartialView = string.Empty;
            dynamic model = null;

            SelectListItem firstItem = new SelectListItem() { Value = "", Text = "-- Please Select --" };

            switch (collateralTypeId)
            {
                case 1:
                    detailPartialView = "_CollateralDetailLand";
                    model = new CollateralDetailInCaseOfLand() { };
                    ViewData["Provinces"] = cs.GetDdlKeyValuePairs("Province", "ProvinceCode", "ProvinceName");
                    ViewData["Zones"] = new List<SelectListItem>() { firstItem };
                    ViewData["Districts"] = new List<SelectListItem>() { firstItem };
                    //ViewData["VDCMuns"] = new List<SelectListItem>() { firstItem };
                    ViewData["AreaTypes"] = cs.GetDdlKeyValuePairs("AreaType", "AreaTypeId", "AreaTypeName");
                    if (unparsedCollateralDetail != null && unparsedCollateralDetail != "" && unparsedCollateralDetail != "[]" && unparsedCollateralDetail != "[{}]")
                    {
                        model = JsonConvert.DeserializeObject<CollateralDetailInCaseOfLand>(unparsedCollateralDetail);
                        if (model != null && !string.IsNullOrEmpty(model.ProvinceId))
                        {
                            ViewData["Provinces"] = cs.GetDdlKeyValuePairs("Province", "ProvinceCode", "ProvinceName", model.ProvinceId.ToString());
                            if (!string.IsNullOrEmpty(model.ZoneId))
                            {
                                List<SelectListItem> Zones = cs.GetDdlKeyValuePairsCascade("Zone", "ZoneCode", "ZoneName", "ProvinceCode", model.ProvinceId.ToString(), model.ZoneId.ToString());
                                Zones.Insert(0, firstItem);
                                ViewData["Zones"] = Zones;
                                if (!string.IsNullOrEmpty(model.DistrictId))
                                {
                                    List<SelectListItem> Districts = cs.GetDdlKeyValuePairsCascade("District", "DistrictCode", "DistrictName", "ZoneCode", model.ZoneId.ToString(), model.DistrictId.ToString());
                                    Districts.Insert(0, firstItem);
                                    ViewData["Districts"] = Districts;
                                    //if (!string.IsNullOrEmpty(model.DistrictId))
                                    //{
                                    //    List<SelectListItem> VDCMuns = cs.GetDdlKeyValuePairsCascade("AreaMunicipality", "AreaCode", "AreaName", "DistrictCode", model.DistrictId.ToString(), model.VDCMunId.ToString());
                                    //    VDCMuns.Insert(0, firstItem);
                                    //    ViewData["VDCMuns"] = VDCMuns;
                                    //}
                                }
                            }
                        }
                    }
                    break;
                case 2:
                    detailPartialView = "_CollateralDetailBuilding";
                    model = new CollateralDetailInCaseOfBuilding() { };
                    ViewData["Provinces"] = cs.GetDdlKeyValuePairs("Province", "ProvinceCode", "ProvinceName");
                    ViewData["Zones"] = new List<SelectListItem>() { firstItem };
                    ViewData["Districts"] = new List<SelectListItem>() { firstItem };
                    //ViewData["VDCMuns"] = new List<SelectListItem>() { firstItem };
                    ViewData["AreaTypes"] = cs.GetDdlKeyValuePairs("AreaType", "AreaTypeId", "AreaTypeName");
                    if (unparsedCollateralDetail != null && unparsedCollateralDetail != "" && unparsedCollateralDetail != "[]" && unparsedCollateralDetail != "[{}]")
                    {
                        model = JsonConvert.DeserializeObject<CollateralDetailInCaseOfBuilding>(unparsedCollateralDetail);
                        if (model != null && !string.IsNullOrEmpty(model.ProvinceId))
                        {
                            ViewData["Provinces"] = cs.GetDdlKeyValuePairs("Province", "ProvinceCode", "ProvinceName", model.ProvinceId.ToString());
                            if (!string.IsNullOrEmpty(model.ZoneId))
                            {
                                List<SelectListItem> Zones = cs.GetDdlKeyValuePairsCascade("Zone", "ZoneCode", "ZoneName", "ProvinceCode", model.ProvinceId.ToString(), model.ZoneId.ToString());
                                Zones.Insert(0, firstItem);
                                ViewData["Zones"] = Zones;
                                if (!string.IsNullOrEmpty(model.DistrictId))
                                {
                                    List<SelectListItem> Districts = cs.GetDdlKeyValuePairsCascade("District", "DistrictCode", "DistrictName", "ZoneCode", model.ZoneId.ToString(), model.DistrictId.ToString());
                                    Districts.Insert(0, firstItem);
                                    ViewData["Districts"] = Districts;
                                    //if (!string.IsNullOrEmpty(model.DistrictId))
                                    //{
                                    //    List<SelectListItem> VDCMuns = cs.GetDdlKeyValuePairsCascade("AreaMunicipality", "AreaCode", "AreaName", "DistrictCode", model.DistrictId.ToString(), model.VDCMunId.ToString());
                                    //    VDCMuns.Insert(0, firstItem);
                                    //    ViewData["VDCMuns"] = VDCMuns;
                                    //}
                                }
                            }
                        }
                    }
                    break;
                case 3:
                    detailPartialView = "_CollateralDetailVehicle";
                    ViewData["VehicleTypes"] = cs.GetDdlKeyValuePairs("VehicleType", "VehicleTypeId", "VehicleTypeName");
                    model = new CollateralDetailInCaseOfVehicle() { };
                    if (unparsedCollateralDetail != null && unparsedCollateralDetail != "" && unparsedCollateralDetail != "[]" && unparsedCollateralDetail != "[{}]")
                    {
                        model = JsonConvert.DeserializeObject<CollateralDetailInCaseOfVehicle>(unparsedCollateralDetail);
                    }
                    break;
                case 4:
                    detailPartialView = "_CollateralDetailPlantAndMachinery";
                    model = new CollateralDetailInCaseOfPlantAndMachinery() { };
                    if (unparsedCollateralDetail != null && unparsedCollateralDetail != "" && unparsedCollateralDetail != "[]" && unparsedCollateralDetail != "[{}]")
                    {
                        model = JsonConvert.DeserializeObject<CollateralDetailInCaseOfPlantAndMachinery>(unparsedCollateralDetail);
                    }
                    break;
                case 5:
                    detailPartialView = "_CollateralDetailCurrentAsset";
                    model = new CollateralDetailInCaseOfCurrentAsset() { };
                    if (unparsedCollateralDetail != null && unparsedCollateralDetail != "" && unparsedCollateralDetail != "[]" && unparsedCollateralDetail != "[{}]")
                    {
                        model = JsonConvert.DeserializeObject<CollateralDetailInCaseOfCurrentAsset>(unparsedCollateralDetail);
                    }
                    break;
                case 6:
                    detailPartialView = "_CollateralDetailPersonalGuarantee";
                    model = new CollateralDetailInCaseOfPersonalGuarantee() { };
                    if (unparsedCollateralDetail != null && unparsedCollateralDetail != "" && unparsedCollateralDetail != "[]" && unparsedCollateralDetail != "[{}]")
                    {
                        model = JsonConvert.DeserializeObject<CollateralDetailInCaseOfPersonalGuarantee>(unparsedCollateralDetail);
                    }
                    break;
                case 7:
                    detailPartialView = "_CollateralDetailCorporateGuarantee";
                    model = new CollateralDetailInCaseOfCorporateGuarantee() { };
                    if (unparsedCollateralDetail != null && unparsedCollateralDetail != "" && unparsedCollateralDetail != "[]" && unparsedCollateralDetail != "[{}]")
                    {
                        model = JsonConvert.DeserializeObject<CollateralDetailInCaseOfCorporateGuarantee>(unparsedCollateralDetail);
                    }
                    break;
                case 8:
                    detailPartialView = "_CollateralDetailStock";
                    model = new CollateralDetailInCaseOfStock() { };
                    ViewData["ShareType"] = cs.GetDdlKeyValuePairs("ShareType", "ShareTypeId", "ShareTypeName");
                    ViewData["ListedInNepse"] = cs.GetDdlKeyValuePairs("NepseListed", "NepseListedBoolId", "NepseListedBoolName");
                    ViewData["ShareUnitType"] = cs.GetDdlKeyValuePairs("ShareUnitType", "ShareUnitTypeId", "ShareUnitTypeName");
                    if (unparsedCollateralDetail != null && unparsedCollateralDetail != "" && unparsedCollateralDetail != "[]" && unparsedCollateralDetail != "[{}]")
                    {
                        model = JsonConvert.DeserializeObject<CollateralDetailInCaseOfStock>(unparsedCollateralDetail);
                    }
                    break;
                case 9:
                    detailPartialView = "_CollateralDetailGold";
                    model = new CollateralDetailInCaseOfGold() { };
                    ViewData["MeasurementUnit"] = cs.GetDdlKeyValuePairs("MeasurementUnit", "MeasurementUnitId", "MeasurementUnitName");
                    ViewData["GoldForm"] = cs.GetDdlKeyValuePairs("GoldForm", "GoldFormId", "GoldFormName");
                    if (unparsedCollateralDetail != null && unparsedCollateralDetail != "" && unparsedCollateralDetail != "[]" && unparsedCollateralDetail != "[{}]")
                    {
                        model = JsonConvert.DeserializeObject<CollateralDetailInCaseOfGold>(unparsedCollateralDetail);
                    }
                    break;
                default:
                    break;
            }
            ViewBag.ViewOnlyUser = viewOnlyUser;
            return PartialView(detailPartialView, model);
        }

        public PartialViewResult GetLoanSettlementDetail()
        {
            return PartialView("_AuctionLoanSettlementCase");
        }

        public JsonResult GetDdlKeyValuePairsCascade(string parentTable, string parentCodeValue)
        {
            CascadingDDLParameters cddlp = new CascadingDDLParameters();
            switch (parentTable.ToUpper())
            {
                case "PROVINCE":
                    cddlp = new CascadingDDLParameters() { TableName = "Zone", KeyTitle = "ZoneCode", ValueTitle = "ZoneName", ParentCodeTitle = "ProvinceCode", ParentCodeValue = parentCodeValue };
                    break;
                case "ZONE":
                    cddlp = new CascadingDDLParameters() { TableName = "District", KeyTitle = "DistrictCode", ValueTitle = "DistrictName", ParentCodeTitle = "ZoneCode", ParentCodeValue = parentCodeValue };
                    break;
                case "DISTRICT":
                    cddlp = new CascadingDDLParameters() { TableName = "AreaMunicipality", KeyTitle = "AreaCode", ValueTitle = "AreaName", ParentCodeTitle = "DistrictCode", ParentCodeValue = parentCodeValue };
                    break;
                default:
                    break;
            }
            List<SelectListItem> sli = cs.GetDdlKeyValuePairsCascade(cddlp.TableName, cddlp.KeyTitle, cddlp.ValueTitle, cddlp.ParentCodeTitle, cddlp.ParentCodeValue);
            return new JsonResult() { Data = sli, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult CreateTables()
        {
            var dbFactory = new OrmLiteConnectionFactory(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString, SqlServerDialect.Provider);
            using (var db = dbFactory.Open())
            {
                //db.CreateTableIfNotExists<PersonalGuarantorDetail>();
            }
            return RedirectToAction("NPAManagementList");
        }

        public JsonResult GetGeneralDetailDataFromCIF(string CIFNo)
        {
            NPAManagementMain generalDetail = new NPAManagementMain();
            if (!string.IsNullOrEmpty(CIFNo) && CIFNo.Trim() != string.Empty)
            {
                generalDetail = nms.GetGeneralDetailDataFromCIF(CIFNo);
            }
            return Json(generalDetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLoanAccountsFromCIF(string CIFNo)
        {
            List<string> loanAccounts;
            loanAccounts = nms.GetLoanAccountsFromCIF(CIFNo);
            return Json(loanAccounts, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLoanDetailDataFromCIF(string CIFNo, string LoanAccountNo)
        {
            LoanAccountDetail generalDetail = new LoanAccountDetail();
            if (!string.IsNullOrEmpty(CIFNo) && CIFNo.Trim() != string.Empty && !string.IsNullOrEmpty(LoanAccountNo) && LoanAccountNo.Trim() != string.Empty)
            {
                generalDetail = nms.GetLoanDetailDataFromCIF(CIFNo, LoanAccountNo);
            }
            return Json(generalDetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOutStDetailDataFromCIF(string CIFNo, string LoanAccountNo)
        {
            LoanAccountDetail generalDetail = new LoanAccountDetail();
            if (!string.IsNullOrEmpty(CIFNo) && CIFNo.Trim() != string.Empty && !string.IsNullOrEmpty(LoanAccountNo) && LoanAccountNo.Trim() != string.Empty)
            {
                generalDetail = nms.GetOutStDetailDataFromCIF(CIFNo, LoanAccountNo);
            }
            return Json(generalDetail, JsonRequestBehavior.AllowGet);
        }

        public bool CheckCIFDuplication(string forOperationMode, string CIFNo, int npaMainId = 0)
        {
            bool cifIsDuplicate;

            cifIsDuplicate = cs.CheckCIFDuplication(forOperationMode, CIFNo, npaMainId);

            return cifIsDuplicate;
        }
    }

    public class CascadingDDLParameters
    {
        public string TableName { get; set; }
        public string KeyTitle { get; set; }
        public string ValueTitle { get; set; }
        public string ParentCodeTitle { get; set; }
        public string ParentCodeValue { get; set; }
    }

    public class AddressParams
    {
        public string ProvinceId { get; set; }
        public string ZoneId { get; set; }
        public string DistrictId { get; set; }
        public string VDCMunId { get; set; }
        public string ToleStreetName { get; set; }
        public string HouseNumber { get; set; }
    }
}
