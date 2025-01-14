﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using SuccessHotelierHub.Models;

namespace SuccessHotelierHub.Repository
{
    public class AdditionalChargeRepository
    {
        #region Additional Charge

        public List<AdditionalChargeVM> GetAdditionalCharges()
        {
            var dt = DALHelper.GetDataTableWithExtendedTimeOut("GetAdditionalCharges");

            var charges = new List<AdditionalChargeVM>();
            charges = DALHelper.CreateListFromTable<AdditionalChargeVM>(dt);

            return charges;
        }

        public List<AdditionalChargeVM> GetAdditionalChargesById(Guid id)
        {
            SqlParameter[] parameters =
               {
                    new SqlParameter { ParameterName = "@Id", Value = id }
                };

            var dt = DALHelper.GetDataTableWithExtendedTimeOut("GetAdditionalChargesById", parameters);


            var charge = new List<AdditionalChargeVM>();
            charge = DALHelper.CreateListFromTable<AdditionalChargeVM>(dt);

            return charge;
        }

        public List<AdditionalChargeVM> GetAdditionalChargesByCode(string code)
        {
            SqlParameter[] parameters =
               {
                    new SqlParameter { ParameterName = "@Code", Value = code }
                };

            var dt = DALHelper.GetDataTableWithExtendedTimeOut("GetAdditionalChargesByCode", parameters);


            var charge = new List<AdditionalChargeVM>();
            charge = DALHelper.CreateListFromTable<AdditionalChargeVM>(dt);

            return charge;
        }

        public string AddAdditionalCharges(AdditionalChargeVM additionalCharge)
        {
            string chargeId = string.Empty;

            SqlParameter[] parameters =
                {
                    new SqlParameter { ParameterName = "@Code", Value = additionalCharge.Code },
                    new SqlParameter { ParameterName = "@Description", Value = additionalCharge.Description },
                    new SqlParameter { ParameterName = "@Price", Value = additionalCharge.Price },
                    new SqlParameter { ParameterName = "@IsActive", Value = additionalCharge.IsActive },
                    new SqlParameter { ParameterName = "@CreatedBy", Value = additionalCharge.CreatedBy },
                    new SqlParameter { ParameterName = "@CategoryId", Value = additionalCharge.CategoryId }
                };

            chargeId = Convert.ToString(DALHelper.ExecuteScalar("AddAdditionalCharges", parameters));

            return chargeId;
        }

        public string UpdateAdditionalCharges(AdditionalChargeVM additionalCharge)
        {
            string chargeId = string.Empty;

            SqlParameter[] parameters =
                {
                    new SqlParameter { ParameterName = "@Id", Value = additionalCharge.Id },
                    new SqlParameter { ParameterName = "@Code", Value = additionalCharge.Code },
                    new SqlParameter { ParameterName = "@Description", Value = additionalCharge.Description },
                    new SqlParameter { ParameterName = "@Price", Value = additionalCharge.Price },
                    new SqlParameter { ParameterName = "@IsActive", Value = additionalCharge.IsActive },
                    new SqlParameter { ParameterName = "@UpdatedBy", Value = additionalCharge.UpdatedBy },
                    new SqlParameter { ParameterName = "@CategoryId", Value = additionalCharge.CategoryId }
                };

            chargeId = Convert.ToString(DALHelper.ExecuteScalar("UpdateAdditionalCharges", parameters));

            return chargeId;
        }

        public string DeleteAdditionalCharges(Guid id, int updatedBy)
        {
            string chargeId = string.Empty;

            SqlParameter[] parameters =
                {
                    new SqlParameter { ParameterName = "@Id", Value = id },
                    new SqlParameter { ParameterName = "@UpdatedBy", Value = updatedBy }
                };

            chargeId = Convert.ToString(DALHelper.ExecuteScalar("DeleteAdditionalCharges", parameters));

            return chargeId;
        }

        public List<AdditionalChargeVM> CheckAdditionalChargeCodeAvailable(Guid? id, string code)
        {
            SqlParameter[] parameters =
               {
                    new SqlParameter { ParameterName = "@Id", Value = id },
                    new SqlParameter { ParameterName = "@Code", Value = code }
                };

            var dt = DALHelper.GetDataTableWithExtendedTimeOut("CheckAdditionalChargeCodeAvailable", parameters);

            var charge = new List<AdditionalChargeVM>();
            charge = DALHelper.CreateListFromTable<AdditionalChargeVM>(dt);

            return charge;
        }

        public List<SearchAdditionalChargeResultVM> SearchAdditionalCharges(SearchAdditionalChargeParametersVM model, string sortColumn, string sortDirection)
        {
            SqlParameter[] parameters =
                {
                    new SqlParameter { ParameterName = "@Code", Value = model.Code },
                    new SqlParameter { ParameterName = "@Description", Value = model.Description },
                    new SqlParameter { ParameterName = "@Price", Value = model.Price },
                    new SqlParameter { ParameterName = "@PageNum", Value = model.PageNum },
                    new SqlParameter { ParameterName = "@PageSize", Value = model.PageSize },
                    new SqlParameter { ParameterName = "@SortColumn", Value = sortColumn },
                    new SqlParameter { ParameterName = "@SortDirection", Value = sortDirection }
                };

            var dt = DALHelper.GetDataTableWithExtendedTimeOut("SearchAdditionalCharges", parameters);

            var charges = new List<SearchAdditionalChargeResultVM>();
            charges = DALHelper.CreateListFromTable<SearchAdditionalChargeResultVM>(dt);

            return charges;
        }

        public List<SearchAdvanceAdditionalChargeResultVM> SearchAdvanceAdditionalCharge(SearchAdvanceAdditionalChargeParametersVM model, int userId)
        {
            SqlParameter[] parameters =
                {
                    new SqlParameter { ParameterName = "@Code", Value = model.Code },
                    new SqlParameter { ParameterName = "@UserId", Value = userId }
                };

            var dt = DALHelper.GetDataTableWithExtendedTimeOut("SearchAdvanceAdditionalCharge", parameters);

            var charges = new List<SearchAdvanceAdditionalChargeResultVM>();
            charges = DALHelper.CreateListFromTable<SearchAdvanceAdditionalChargeResultVM>(dt);

            return charges;
        }
        #endregion Additional Charge

        #region "Addtional Charges Category"

        public List<AdditionalChargeCategoryVM> GetAdditionalChargeCategory()
        {
            var dt = DALHelper.GetDataTableWithExtendedTimeOut("GetAdditionalChargeCategories");

            var category = new List<AdditionalChargeCategoryVM>();
            category = DALHelper.CreateListFromTable<AdditionalChargeCategoryVM>(dt);

            return category;
        }


        public List<AdditionalChargeCategoryVM> IsBrekFastCharges(Guid addionalChargeId)
        {
            SqlParameter[] parameters =
               {
                    new SqlParameter { ParameterName = "@AddionalChargeId", Value = addionalChargeId }
                };

            var dt = DALHelper.GetDataTableWithExtendedTimeOut("IsBrekFastCharges", parameters);

            var category = new List<AdditionalChargeCategoryVM>();
            category = DALHelper.CreateListFromTable<AdditionalChargeCategoryVM>(dt);

            return category;
        }

        #endregion
    }
}