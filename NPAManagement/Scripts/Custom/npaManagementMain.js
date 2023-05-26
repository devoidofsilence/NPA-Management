$.validator.setDefaults({ ignore: [] });
$(document).ready(function () {
    $("#formNPA").on("submit", function () {
        document.getElementById("overlay").style.display = "block";
        var validated = $('#formNPA').validate().form();
        if (validated == false) {
            document.getElementById("overlay").style.display = "none";
            return false;
        }

        //make JSON object ready from the Borrower Individual/Legal Details
        var borrowerIndLegDetailObject = {};
        $("#collapseBorrowerDetails .form-control[data-model]").each(function () {
            var modelNameToMap = $(this).data("model");
            var modelValueToMap = $(this).val();
            borrowerIndLegDetailObject[modelNameToMap] = modelValueToMap;
        });
        var stringifiedBorrowerIndLegDetailObject = JSON.stringify(borrowerIndLegDetailObject);
        $("#StringifiedBorrowerTypeIndividualLegalDetail").val(stringifiedBorrowerIndLegDetailObject);
        //make JSON object ready from the Borrower Individual/Legal Details

        //make JSON object ready from the loan partial views
        var listOfLoanAccountDetailObjects = [];
        var loanAccountDetailObject = {};
        $("#dvLoanDetails .individualLoanBlock").each(function (index) {
            //Here for all detail in Loan Detail except inner partial views i.e. Primary Collaterals, Secondary Collaterals
            loanAccountDetailObject = {};
            $(this).find(".form-control[data-model]").not(".collapseSecurityDetailsAndValuation .form-control[data-model]").each(function () {
                var modelNameToMap = $(this).data("model");
                var modelValueToMap = $(this).val();
                loanAccountDetailObject[modelNameToMap] = modelValueToMap;
            });
            //Here for all detail in Loan Detail except inner partial views i.e. Primary Collaterals, Secondary Collaterals

            listOfLoanAccountDetailObjects.push(loanAccountDetailObject);
        });
        var stringifiedListOfLoanAccountDetailObjects = JSON.stringify(listOfLoanAccountDetailObjects);
        $("#StringifiedListOfLoanAccountDetailObjects").val(stringifiedListOfLoanAccountDetailObjects);
        //make JSON object ready from the loan partial views

        //Here for all primary collateral details
        var listOfPrimaryCollateralDetailObjects = [];
        var primaryCollateralDetailObject = {};
        $('#collapseSecurityDetailsAndValuation').find(".primaryCollaterals .collateralDetail").each(function () {
            primaryCollateralDetailObject = {};
            $(this).find(".form-control[data-model]").each(function () {
                var modelNameToMap = $(this).data("model");
                var modelValueToMap = $(this).val();
                primaryCollateralDetailObject[modelNameToMap] = modelValueToMap;
            }
            );
            listOfPrimaryCollateralDetailObjects.push(primaryCollateralDetailObject);
        });
        var stringifiedListOfPrimaryCollateralDetailObjects = JSON.stringify(listOfPrimaryCollateralDetailObjects);
        $("#StringifiedListOfPrimaryCollateralDetailObjects").val(stringifiedListOfPrimaryCollateralDetailObjects);
        //Here for all primary collateral details

        //Here for all secondary collateral details
        var listOfSecondaryCollateralDetailObjects = [];
        var secondaryCollateralDetailObject = {};
        $("#collapseSecurityDetailsAndValuation").find(".secondaryCollaterals .collateralDetail").each(function () {
            secondaryCollateralDetailObject = {};
            $(this).find(".form-control[data-model]").each(function () {
                var modelNameToMap = $(this).data("model");
                var modelValueToMap = $(this).val();
                secondaryCollateralDetailObject[modelNameToMap] = modelValueToMap;
            }
            );
            listOfSecondaryCollateralDetailObjects.push(secondaryCollateralDetailObject);
        });
        var stringifiedListOfSecondaryCollateralDetailObjects = JSON.stringify(listOfSecondaryCollateralDetailObjects);
        $("#StringifiedListOfSecondaryCollateralDetailObjects").val(stringifiedListOfSecondaryCollateralDetailObjects);
        //Here for all secondary collateral details

        //make JSON object ready from the branch follow ups
        var listOfBranchFollowUpDetailObjects = [];
        var branchFollowUpDetailObject = {};
        $("#collapseBranchFollowUp #dvBranchFollowUps .collapseFollowUpDetail").each(function (index) {
            //Here for all detail in Branch Follow Up
            branchFollowUpDetailObject = {};
            $(this).find(".form-control[data-model]").each(function () {
                var modelNameToMap = $(this).data("model");
                var modelValueToMap = $(this).val();
                branchFollowUpDetailObject[modelNameToMap] = modelValueToMap;
            });

            $(this).find(".form-control-file[data-model]").each(function () {
                var modelNameToMap = $(this).data("model");
                var modelValueToMap = $(this).attr("bytearr");
                branchFollowUpDetailObject[modelNameToMap] = modelValueToMap;
            });

            $(this).find(".scanCopyOfActionTakenFileType[data-model]").each(function () {
                var modelNameToMap = $(this).data("model");
                var modelValueToMap = $(this).val();
                branchFollowUpDetailObject[modelNameToMap] = modelValueToMap;
            });
            //Here for all detail in Branch Follow Up
            listOfBranchFollowUpDetailObjects.push(branchFollowUpDetailObject);
        });

        var stringifiedListOfBranchFollowUpDetailObjects = JSON.stringify(listOfBranchFollowUpDetailObjects);
        $("#StringifiedListOfBranchFollowUpDetailObjects").val(stringifiedListOfBranchFollowUpDetailObjects);
        //make JSON object ready from the branch follow ups

        //make JSON object ready from the SAC follow ups
        var listOfSACFollowUpDetailObjects = [];
        var sacFollowUpDetailObject = {};
        $("#collapseSACFollowUp #dvSACFollowUps .collapseFollowUpDetail").each(function (index) {
            //Here for all detail in SAC Follow Up
            sacFollowUpDetailObject = {};
            $(this).find(".form-control[data-model]").each(function () {
                var modelNameToMap = $(this).data("model");
                var modelValueToMap = $(this).val();
                sacFollowUpDetailObject[modelNameToMap] = modelValueToMap;
            });

            $(this).find(".form-control-file[data-model]").each(function () {
                var modelNameToMap = $(this).data("model");
                var modelValueToMap = $(this).attr("bytearr");
                sacFollowUpDetailObject[modelNameToMap] = modelValueToMap;
            });

            $(this).find(".scanCopyOfActionTakenFileType[data-model]").each(function () {
                var modelNameToMap = $(this).data("model");
                var modelValueToMap = $(this).val();
                sacFollowUpDetailObject[modelNameToMap] = modelValueToMap;
            });
            //Here for all detail in SAC Follow Up
            listOfSACFollowUpDetailObjects.push(sacFollowUpDetailObject);
        });

        var stringifiedListOfSACFollowUpDetailObjects = JSON.stringify(listOfSACFollowUpDetailObjects);
        $("#StringifiedListOfSACFollowUpDetailObjects").val(stringifiedListOfSACFollowUpDetailObjects);
        //make JSON object ready from the SAC follow ups
        //document.getElementById("overlay").style.display = "none";
        return true;
    });

    $(".date").datepicker({
        dateFormat: 'yy/mm/dd',
        changeMonth: true,
        changeYear: true
    });

    $(".select2Employees").select2();

    $("#btnGetDataFromCIF").click(function () {
        document.getElementById("overlay").style.display = "block";
        var urlValue = "";
        urlValue = "/NPAManagement/GetGeneralDetailDataFromCIF?CIFNo=" + $("#CIFNo").val();
        if (urlValue !== "") {
            $.ajax({
                url: urlValue,
                contentType: 'application/html; charset=utf-8',
                type: 'GET',
                dataType: 'html',
                success: function (result) {
                    //populate fields here
                    var jsonifiedMainBody = JSON.parse(result);
                    $("#BorrowerName").val(jsonifiedMainBody.BorrowerName);
                    $("#GroupName").val(jsonifiedMainBody.GroupName);
                    $("#BorrowerContactNumber").val(jsonifiedMainBody.BorrowerContactNumber);
                    $("#BorrowerEmailAddress").val(jsonifiedMainBody.BorrowerEmailAddress);
                    if (jsonifiedMainBody.StringifiedBorrowerTypeIndividualLegalDetail !== undefined) {
                        var jsonifiedBorrowerDetail = JSON.parse(jsonifiedMainBody.StringifiedBorrowerTypeIndividualLegalDetail);
                        if (jsonifiedBorrowerDetail !== null) {
                            if (jsonifiedBorrowerDetail.BorrowerTypeId == 1) {
                                var urlValue = "";
                                urlValue = "/NPAManagement/GetIndividualBorrowerDetail";
                                if (urlValue !== "") {
                                    $.ajax({
                                        url: urlValue,
                                        contentType: 'application/html; charset=utf-8',
                                        type: 'GET',
                                        dataType: 'html',
                                        success: function (result) {
                                            $('#BorrowerDetailSection').html(result.replace(/&amp;/g, '&'));
                                            var form = $("#formNPA")
                                                .removeData("validator") /* added by the raw jquery.validate plugin */
                                                .removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin*/

                                            $.validator.unobtrusive.parse("#formNPA");

                                            var jsonifiedBorrowerDetail = JSON.parse(jsonifiedMainBody.StringifiedBorrowerTypeIndividualLegalDetail);
                                            $('#BorrowerTypeId').val(jsonifiedBorrowerDetail.BorrowerTypeId);
                                            $('#BorrowerFatherName').val(jsonifiedBorrowerDetail.BorrowerFatherName);
                                            $('#BorrowerGrandfatherName').val(jsonifiedBorrowerDetail.BorrowerGrandfatherName);
                                            $('#BorrowerCitizenshipNumber').val(jsonifiedBorrowerDetail.BorrowerCitizenshipNumber);
                                            $('#BorrowerSpouseName').val(jsonifiedBorrowerDetail.BorrowerSpouseName);
                                            $('#BorrowerSonName').val(jsonifiedBorrowerDetail.BorrowerSonName);
                                            $('#BorrowerDaughterName').val(jsonifiedBorrowerDetail.BorrowerDaughterName);
                                        },
                                        error: function (xhr, status) { alert(status); }
                                    });
                                }
                                else {
                                    $('#BorrowerDetailSection').html('');
                                }
                            }
                            else if (jsonifiedBorrowerDetail.BorrowerTypeId == 2) {
                                var urlValue = "";
                                urlValue = "/NPAManagement/GetLegalEntityBorrowerDetail";
                                if (urlValue !== "") {
                                    $.ajax({
                                        url: urlValue,
                                        contentType: 'application/html; charset=utf-8',
                                        type: 'GET',
                                        dataType: 'html',
                                        success: function (result) {
                                            $('#BorrowerDetailSection').html(result.replace(/&amp;/g, '&'));
                                            var form = $("#formNPA")
                                                .removeData("validator") /* added by the raw jquery.validate plugin */
                                                .removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin*/

                                            $.validator.unobtrusive.parse("#formNPA");
                                            var jsonifiedBorrowerDetail = JSON.parse(jsonifiedMainBody.StringifiedBorrowerTypeIndividualLegalDetail);
                                            $('#BorrowerTypeId').val(jsonifiedBorrowerDetail.BorrowerTypeId);
                                            $('#LegalEntityRegistrationNumber').val(jsonifiedBorrowerDetail.LegalEntityRegistrationNumber);
                                            $('#LegalEntityRegisteredOffice').val(jsonifiedBorrowerDetail.LegalEntityRegisteredOffice);
                                            $('#LegalEntityRegistrationDate').val(jsonifiedBorrowerDetail.LegalEntityRegistrationDate.trim());
                                            //if (jsonifiedBorrowerDetail.LegalEntityRegistrationDate.trim() != '') {
                                            //    var date = jsonifiedBorrowerDetail.LegalEntityRegistrationDate.split("-");
                                            //    var months = ['JAN', 'FEB', 'MAR', 'APR', 'MAY', 'JUN', 'JULY', 'AUG', 'SEP', 'OCT', 'NOV', 'DEC'];
                                            //    for (var j = 0; j < months.length; j++) {
                                            //        if (date[1] == months[j]) {
                                            //            date[1] = months.indexOf(months[j]) + 1;
                                            //        }
                                            //    }
                                            //    if (date[1] < 10) {
                                            //        date[1] = '0' + date[1];
                                            //    }
                                            //    var formattedDate = date[2] + '/' + date[1] + '/' + date[0];
                                            //    $('#LegalEntityRegistrationDate').val(formattedDate);
                                            //}
                                        },
                                        error: function (xhr, status) { alert(status); }
                                    });
                                }
                                else {
                                    $('#BorrowerDetailSection').html('');
                                }
                            }
                        }
                    }
                    document.getElementById("overlay").style.display = "none";
                },
                error: function (xhr, status) { alert(status); document.getElementById("overlay").style.display = "none"; }
            });
        }
    });

    $("#collapseLoanDetails").on("click", ".btnGetDataFromCIFAndLoan", function (e) {
        var thisCtrl = e.currentTarget;
        if ($(this).val().trim() !== "" && $("#CIFNo").val().trim() !== "") {
            document.getElementById("overlay").style.display = "block";
            let loanAccNo = $(thisCtrl).closest(".individualLoanBlock").find("#LoanAccountNo");
            var urlValue = "";
            urlValue = "/NPAManagement/GetLoanDetailDataFromCIF?CIFNo=" + $("#CIFNo").val() + "&LoanAccountNo=" + $(loanAccNo).val();
            if (urlValue !== "") {
                $.ajax({
                    url: urlValue,
                    contentType: 'application/html; charset=utf-8',
                    type: 'GET',
                    dataType: 'html',
                    success: function (result) {
                        //populate fields here
                        var jsonifiedMainBody = JSON.parse(result);
                        if (jsonifiedMainBody.LoanAccountNo !== null) {
                            let productCodeDDL = $(thisCtrl).closest(".individualLoanBlock").find("#ProductCode");
                            $(productCodeDDL).val(jsonifiedMainBody.ProductCode);
                            $(productCodeDDL).trigger("change");

                            let nomineeAccountNo = $(thisCtrl).closest(".individualLoanBlock").find("#NomineeAccountNo");
                            $(nomineeAccountNo).val(jsonifiedMainBody.NomineeAccountNo);

                            let loanTypeId = $(thisCtrl).closest(".individualLoanBlock").find("#LoanTypeId");
                            $(loanTypeId).val(jsonifiedMainBody.LoanTypeId);

                            let loanCurrentStatusId = $(thisCtrl).closest(".individualLoanBlock").find("#LoanCurrentStatusId");
                            $(loanCurrentStatusId).val(jsonifiedMainBody.LoanCurrentStatusId);

                            let recoveredAmtPrincipal = $(thisCtrl).closest(".individualLoanBlock").find("#RecoveredAmtPrincipal");
                            $(recoveredAmtPrincipal).val(jsonifiedMainBody.RecoveredAmtPrincipal);

                            let recoveredAmtInterest = $(thisCtrl).closest(".individualLoanBlock").find("#RecoveredAmtInterest");
                            $(recoveredAmtInterest).val(jsonifiedMainBody.RecoveredAmtInterest);

                            let recoveredAmtInterestOnInterest = $(thisCtrl).closest(".individualLoanBlock").find("#RecoveredAmtInterestOnInterest");
                            $(recoveredAmtInterestOnInterest).val(jsonifiedMainBody.RecoveredAmtInterestOnInterest);

                            let recoveredAmtPenalCharges = $(thisCtrl).closest(".individualLoanBlock").find("#RecoveredAmtPenalCharges");
                            $(recoveredAmtPenalCharges).val(jsonifiedMainBody.RecoveredAmtPenalCharges);

                            let recoveredAmtTotalAmount = $(thisCtrl).closest(".individualLoanBlock").find("#RecoveredAmtTotalAmount");
                            $(recoveredAmtTotalAmount).val(jsonifiedMainBody.RecoveredAmtTotalAmount);

                            let accruedAmtPrincipal = $(thisCtrl).closest(".individualLoanBlock").find("#AccruedAmtPrincipal");
                            $(accruedAmtPrincipal).val(jsonifiedMainBody.AccruedAmtPrincipal);

                            let accruedAmtInterest = $(thisCtrl).closest(".individualLoanBlock").find("#AccruedAmtInterest");
                            $(accruedAmtInterest).val(jsonifiedMainBody.AccruedAmtInterest);

                            let accruedAmtInterestOnInterest = $(thisCtrl).closest(".individualLoanBlock").find("#AccruedAmtInterestOnInterest");
                            $(accruedAmtInterestOnInterest).val(jsonifiedMainBody.AccruedAmtInterestOnInterest);

                            let accruedAmtPenalCharges = $(thisCtrl).closest(".individualLoanBlock").find("#AccruedAmtPenalCharges");
                            $(accruedAmtPenalCharges).val(jsonifiedMainBody.AccruedAmtPenalCharges);

                            let accruedAmtTotalAmount = $(thisCtrl).closest(".individualLoanBlock").find("#AccruedAmtTotalAmount");
                            $(accruedAmtTotalAmount).val(jsonifiedMainBody.AccruedAmtTotalAmount);
                            // second service call
                            var urlValueForOutSt = "";
                            urlValueForOutSt = "/NPAManagement/GetOutStDetailDataFromCIF?CIFNo=" + $("#CIFNo").val() + "&LoanAccountNo=" + $(loanAccNo).val();
                            if (urlValueForOutSt !== "") {
                                $.ajax({
                                    url: urlValueForOutSt,
                                    contentType: 'application/html; charset=utf-8',
                                    type: 'GET',
                                    dataType: 'html',
                                    success: function (result2) {
                                        //populate fields here
                                        var jsonifiedMainBody = JSON.parse(result2);
                                        let outStDate = $(thisCtrl).closest(".individualLoanBlock").find("#OutStDate");
                                        $(outStDate).val(jsonifiedMainBody.OutStDate);

                                        let outStPrincipal = $(thisCtrl).closest(".individualLoanBlock").find("#OutStPrincipal");
                                        $(outStPrincipal).val(jsonifiedMainBody.OutStPrincipal);

                                        let outStInterest = $(thisCtrl).closest(".individualLoanBlock").find("#OutStInterest");
                                        $(outStInterest).val(jsonifiedMainBody.OutStInterest);

                                        let outStAdhocCharges = $(thisCtrl).closest(".individualLoanBlock").find("#OutStAdhocCharges");
                                        $(outStAdhocCharges).val(jsonifiedMainBody.OutStAdhocCharges);

                                        let outStInterestOnInterest = $(thisCtrl).closest(".individualLoanBlock").find("#OutStInterestOnInterest");
                                        $(outStInterestOnInterest).val(jsonifiedMainBody.OutStInterestOnInterest);

                                        let outStPenalCharges = $(thisCtrl).closest(".individualLoanBlock").find("#OutStPenalCharges");
                                        $(outStPenalCharges).val(jsonifiedMainBody.OutStPenalCharges);

                                        let outStTotalAmount = $(thisCtrl).closest(".individualLoanBlock").find("#OutStTotalAmount");
                                        $(outStTotalAmount).val(jsonifiedMainBody.OutStTotalAmount);

                                        document.getElementById("overlay").style.display = "none";
                                    },
                                    error: function (xhr, status) { alert(status); document.getElementById("overlay").style.display = "none"; }
                                });
                            }
                        }
                        else {
                            let loanAccountNo = $(thisCtrl).closest(".individualLoanBlock").find("#LoanAccountNo").val('');
                        }
                        document.getElementById("overlay").style.display = "none";
                    },
                    error: function (xhr, status) { alert(status); document.getElementById("overlay").style.display = "none"; }
                });
            }
        }
    });

    $("#collapseLoanDetails").on("change", ".loanStatusDDL", function (e) {
        var thisCtrl = e.currentTarget;
        if ($(thisCtrl).val() == "2") {
            $(thisCtrl).closest("#dvLoanDetails").find(".showHideAccToCurrentStatusOfLoan").show();
        }
        else {
            $(thisCtrl).closest("#dvLoanDetails").find(".showHideAccToCurrentStatusOfLoan").hide();
            $(thisCtrl).closest("#dvLoanDetails").find(".settlementDate").val("");
        }
    });

    $("#BorrowerTypeId").change(function () {
        document.getElementById("overlay").style.display = "block";
        var urlValue = "";
        if ($(this).val() === "1") {
            urlValue = "/NPAManagement/GetIndividualBorrowerDetail";
        }
        else if ($(this).val() === "2") {
            urlValue = "/NPAManagement/GetLegalEntityBorrowerDetail";
        }
        if (urlValue !== "") {
            $.ajax({
                url: urlValue,
                contentType: 'application/html; charset=utf-8',
                type: 'GET',
                dataType: 'html',
                success: function (result) {
                    $('#BorrowerDetailSection').html(result.replace(/&amp;/g, '&'));
                    var form = $("#formNPA")
                        .removeData("validator") /* added by the raw jquery.validate plugin */
                        .removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin*/

                    $.validator.unobtrusive.parse("#formNPA");
                    document.getElementById("overlay").style.display = "none";
                },
                error: function (xhr, status) { alert(status); }
            });
        }
        else {
            $('#BorrowerDetailSection').html('');
            document.getElementById("overlay").style.display = "none";
        }
    });

    $("#btnAddNewRowLoanAccountDetail").on("click", function () {
        document.getElementById("overlay").style.display = "block";
        var target = $(this).data("target");
        var urlValue = "/NPAManagement/GetLoanAccountDetail";
        if (urlValue !== "") {
            $.ajax({
                url: urlValue,
                contentType: 'application/html; charset=utf-8',
                type: 'GET',
                dataType: 'html',
                success: function (result) {
                    $('#' + target).append(result);
                    var form = $("#formNPA")
                        .removeData("validator") /* added by the raw jquery.validate plugin */
                        .removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin*/

                    $.validator.unobtrusive.parse("#formNPA");
                    document.getElementById("overlay").style.display = "none";
                },
                error: function (xhr, status) { alert(status); document.getElementById("overlay").style.display = "none"; }
            });
        }
        else {
            $('#' + target).html('');
            document.getElementById("overlay").style.display = "none";
        }
    });

    $("#btnAddNewRowBranchFollowUp").on("click", function () {
        document.getElementById("overlay").style.display = "block";
        var typeId = $(this).data("type-id");
        var urlValue = "/NPAManagement/GetFollowUpPanel?followUpById=" + typeId;
        if (urlValue !== "") {
            $.ajax({
                url: urlValue,
                contentType: 'application/html; charset=utf-8',
                type: 'GET',
                dataType: 'html',
                success: function (result) {
                    $('#dvBranchFollowUps').append(result);
                    var form = $("#formNPA")
                        .removeData("validator") /* added by the raw jquery.validate plugin */
                        .removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin*/

                    $.validator.unobtrusive.parse("#formNPA");
                    document.getElementById("overlay").style.display = "none";
                },
                error: function (xhr, status) { alert(status); document.getElementById("overlay").style.display = "none"; }
            });
        }
        else {
            $('#dvBranchFollowUps').html('');
            document.getElementById("overlay").style.display = "none";
        }
    });

    $("#btnAddNewRowSACFollowUp").on("click", function () {
        document.getElementById("overlay").style.display = "block";
        var typeId = $(this).data("type-id");
        var urlValue = "/NPAManagement/GetFollowUpPanel?followUpById=" + typeId;
        if (urlValue !== "") {
            $.ajax({
                url: urlValue,
                contentType: 'application/html; charset=utf-8',
                type: 'GET',
                dataType: 'html',
                success: function (result) {
                    $('#dvSACFollowUps').append(result);
                    var form = $("#formNPA")
                        .removeData("validator") /* added by the raw jquery.validate plugin */
                        .removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin*/

                    $.validator.unobtrusive.parse("#formNPA");
                    document.getElementById("overlay").style.display = "none";
                },
                error: function (xhr, status) { alert(status); document.getElementById("overlay").style.display = "none"; }
            });
        }
        else {
            $('#dvSACFollowUps').html('');
            document.getElementById("overlay").style.display = "none";
        }
    });

    //$("#AuctionFollowUpResultId").change(function () {
    //    var urlValue = "";
    //    if ($(this).val() === "1") {
    //        urlValue = "/NPAManagement/GetLoanSettlementDetail";
    //    }
    //    else {
    //        urlValue = "";
    //    }
    //    if (urlValue !== "") {
    //        $.ajax({
    //            url: urlValue,
    //            contentType: 'application/html; charset=utf-8',
    //            type: 'GET',
    //            dataType: 'html',
    //            success: function (result) { $('#dvAuctionLoanSettlement').html(result); },
    //            error: function (xhr, status) { alert(status); }
    //        });
    //    }
    //    else {
    //        $('#dvAuctionLoanSettlement').html('');
    //    }
    //});

    $("#TempAddSameAsPerAdd").change(function () {
        if ($(this).is(":checked")) {
            $("#BorrowerTemporaryAddressProvinceId").html($("#BorrowerPermanentAddressProvinceId").html());
            $("#BorrowerTemporaryAddressProvinceId").val($("#BorrowerPermanentAddressProvinceId").val());
            $("#txtTemporaryAddressProvinceCode").val($("#BorrowerPermanentAddressProvinceId").val());
            $("#BorrowerTemporaryAddressZoneId").html($("#BorrowerPermanentAddressZoneId").html());
            $("#BorrowerTemporaryAddressZoneId").val($("#BorrowerPermanentAddressZoneId").val());
            $("#txtTemporaryAddressZoneCode").val($("#BorrowerPermanentAddressZoneId").val());
            $("#BorrowerTemporaryAddressDistrictId").html($("#BorrowerPermanentAddressDistrictId").html());
            $("#BorrowerTemporaryAddressDistrictId").val($("#BorrowerPermanentAddressDistrictId").val());
            $("#txtTemporaryAddressDistrictCode").val($("#BorrowerPermanentAddressDistrictId").val());
            $("#txtTemporaryAddressAreaMunicipality").val($("#txtPermanentAddressAreaMunicipality").val());
            $("#BorrowerTemporaryToleStreetName").val($("#BorrowerPermanentToleStreetName").val());
            $("#BorrowerTemporaryHouseNumber").val($("#BorrowerPermanentHouseNumber").val());
        }
    });

    $("#body").on("change", ".ddlHandler", function () {
        var copyTo = $(this).data("copy-to");
        $(this).closest('.form-row').find('#' + copyTo).val($(this).val());
    });

    $("#body").on("change", ".ddlCascader", function () {
        var parentTable = $(this).data("type");
        var parentCodeValue = $("#" + $(this).data("copy-to")).val();
        var childDdl = $(this).data("parent-of");
        var urlValue = "/NPAManagement/GetDdlKeyValuePairsCascade?parentTable=" + parentTable + "&parentCodeValue=" + parentCodeValue;
        if (urlValue !== "") {
            $.ajax({
                url: urlValue,
                contentType: 'application/html; charset=utf-8',
                type: 'GET',
                dataType: 'html',
                success: function (result) {
                    $("#" + childDdl + " option:not(:first)").remove();
                    $("#" + childDdl).trigger("change");
                    var parsedColl = JSON.parse(result);
                    $.each(parsedColl, function (i, e) {
                        $("#" + childDdl).append($("<option>", {
                            value: e.Value,
                            text: e.Text
                        }));
                    });
                    //$('#BorrowerDetailSection').html(result);
                },
                error: function (xhr, status) { alert(status); }
            });
        }
        else {
            //$('#BorrowerDetailSection').html('');
        }
    });
});