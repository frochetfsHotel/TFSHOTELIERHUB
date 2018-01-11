﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using SuccessHotelierHub.Models;

namespace SuccessHotelierHub.Repository
{
    public class ReservationChargeRepository
    {
        #region Reservation Charge

        public List<ReservationChargeVM> GetReservationCharges(Guid? reservationId, Guid? additionalChargeId)
        {
            SqlParameter[] parameters =
               {
                    new SqlParameter { ParameterName = "@ReservationId", Value = reservationId },
                    new SqlParameter { ParameterName = "@AdditionalChargeId", Value = additionalChargeId }
                };

            var dt = DALHelper.GetDataTableWithExtendedTimeOut("GetReservationCharges", parameters);

            var charges = new List<ReservationChargeVM>();
            charges = DALHelper.CreateListFromTable<ReservationChargeVM>(dt);

            return charges;
        }

        public List<ReservationChargeVM> GetReservationChargesById(Guid id)
        {
            SqlParameter[] parameters =
               {
                    new SqlParameter { ParameterName = "@Id", Value = id}
                };

            var dt = DALHelper.GetDataTableWithExtendedTimeOut("GetReservationChargesById", parameters);


            var charge = new List<ReservationChargeVM>();
            charge = DALHelper.CreateListFromTable<ReservationChargeVM>(dt);

            return charge;
        }
        

        public string AddReservationCharges(ReservationChargeVM reservationCharge)
        {
            string chargeId = string.Empty;

            SqlParameter[] parameters =
                {
                    new SqlParameter { ParameterName = "@ReservationId", Value = reservationCharge.ReservationId },
                    new SqlParameter { ParameterName = "@AdditionalChargeId", Value = reservationCharge.AdditionalChargeId },
                    new SqlParameter { ParameterName = "@Code", Value = reservationCharge.Code },
                    new SqlParameter { ParameterName = "@Description", Value = reservationCharge.Description },
                    new SqlParameter { ParameterName = "@TransactionDate", Value = reservationCharge.TransactionDate },
                    new SqlParameter { ParameterName = "@Amount", Value = reservationCharge.Amount },
                    new SqlParameter { ParameterName = "@Qty", Value = reservationCharge.Qty },
                    new SqlParameter { ParameterName = "@Supplement", Value = reservationCharge.Supplement },
                    new SqlParameter { ParameterName = "@Reference", Value = reservationCharge.Reference },
                    new SqlParameter { ParameterName = "@IsActive", Value = reservationCharge.IsActive },
                    new SqlParameter { ParameterName = "@CreatedBy", Value = reservationCharge.CreatedBy }
                };

            chargeId = Convert.ToString(DALHelper.ExecuteScalar("AddReservationCharges", parameters));

            return chargeId;
        }

        public string UpdateReservationCharges(ReservationChargeVM reservationCharge)
        {
            string chargeId = string.Empty;

            SqlParameter[] parameters =
                {
                    new SqlParameter { ParameterName = "@Id", Value = reservationCharge.Id },
                    new SqlParameter { ParameterName = "@ReservationId", Value = reservationCharge.ReservationId },
                    new SqlParameter { ParameterName = "@AdditionalChargeId", Value = reservationCharge.AdditionalChargeId },
                    new SqlParameter { ParameterName = "@Code", Value = reservationCharge.Code },
                    new SqlParameter { ParameterName = "@Description", Value = reservationCharge.Description },
                    new SqlParameter { ParameterName = "@TransactionDate", Value = reservationCharge.TransactionDate },
                    new SqlParameter { ParameterName = "@Amount", Value = reservationCharge.Amount },
                    new SqlParameter { ParameterName = "@Qty", Value = reservationCharge.Qty },
                    new SqlParameter { ParameterName = "@Supplement", Value = reservationCharge.Supplement },
                    new SqlParameter { ParameterName = "@Reference", Value = reservationCharge.Reference },
                    new SqlParameter { ParameterName = "@IsActive", Value = reservationCharge.IsActive },
                    new SqlParameter { ParameterName = "@UpdatedBy", Value = reservationCharge.UpdatedBy }
                };

            chargeId = Convert.ToString(DALHelper.ExecuteScalar("UpdateReservationCharges", parameters));

            return chargeId;
        }

        public string DeleteReservationCharges(Guid id, int updatedBy)
        {
            string chargeId = string.Empty;

            SqlParameter[] parameters =
                {
                    new SqlParameter { ParameterName = "@Id", Value = id },
                    new SqlParameter { ParameterName = "@UpdatedBy", Value = updatedBy }
                };

            chargeId = Convert.ToString(DALHelper.ExecuteScalar("DeleteReservationCharges", parameters));

            return chargeId;
        }

        #endregion Reservation Charge
    }
}