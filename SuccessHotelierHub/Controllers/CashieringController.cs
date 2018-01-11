﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SuccessHotelierHub.Models;
using SuccessHotelierHub.Utility;
using SuccessHotelierHub.Repository;

namespace SuccessHotelierHub.Controllers
{
    public class CashieringController : Controller
    {
        #region Declaration

        private ProfileRepository profileRepository = new ProfileRepository();
        private RoomTypeRepository roomTypeRepository = new RoomTypeRepository();
        private RateTypeRepository rateTypeRepository = new RateTypeRepository();
        private CheckInCheckOutRepository checkInCheckOutRepository = new CheckInCheckOutRepository();
        private RoomRepository roomRepository = new RoomRepository();
        private ReservationRepository reservationRepository = new ReservationRepository();
        private AdditionalChargeRepository additionalChargeRepository = new AdditionalChargeRepository();
        private ReservationChargeRepository reservationChargeRepository = new ReservationChargeRepository();

        #endregion

        // GET: Cashiering
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SearchGuest()
        {
            var charges = additionalChargeRepository.GetAdditionalCharges();            
            ViewBag.AdditionalChargeList = charges;
            return View();
        }

        [HttpPost]
        public ActionResult SearchGuest(SearchGuestParametersVM model)
        {
            try
            {
                object sortColumn = "";
                object sortDirection = "";

                if (model.order.Count == 0)
                {
                    sortColumn = "CreatedOn";
                    sortDirection = "desc";
                }
                else
                {
                    sortColumn = model.columns[Convert.ToInt32(model.order[0].column)].data ?? (object)DBNull.Value;
                    sortDirection = model.order[0].dir ?? (object)DBNull.Value;
                }

                model.PageSize = Constants.PAGESIZE;
                var reservations = checkInCheckOutRepository.SearchGuest(model, Convert.ToString(sortColumn), Convert.ToString(sortDirection));

                int totalRecords = 0;
                var dbRecords = reservations.Select(m => m.TotalCount).FirstOrDefault();

                if (dbRecords != 0)
                    totalRecords = Convert.ToInt32(dbRecords);

                return Json(new
                {
                    IsSuccess = true,
                    CurrentPage = model.PageNum,
                    PageSize = Constants.PAGESIZE,
                    TotalRecords = totalRecords,
                    data = reservations
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }
        

        [HttpPost]
        public ActionResult GetBillingInfo(Guid reservationId, string source = "")
        {
            try
            {
                var reservation = reservationRepository.GetReservationById(reservationId).FirstOrDefault();

                #region Room Mapping

                //Get Room Mapping
                var selectedRooms = roomRepository.GetReservationRoomMapping(reservationId, null);
                var roomIds = string.Empty;
                var roomNumbers = string.Empty;

                if (selectedRooms != null && selectedRooms.Count > 0)
                {
                    foreach (var room in selectedRooms)
                    {
                        roomIds += string.Format("{0},", room.RoomId);
                        roomNumbers += string.Format("{0}, ", room.RoomNo);
                    }

                    if (!string.IsNullOrWhiteSpace(roomIds))
                    {
                        //Remove Last Comma.
                        roomIds = Utility.Utility.RemoveLastCharcter(roomIds, ',');
                    }

                    if (!string.IsNullOrWhiteSpace(roomNumbers))
                    {
                        //Remove Last Comma.
                        roomNumbers = Utility.Utility.RemoveLastCharcter(roomNumbers, ',');
                    }
                }
                #endregion

                #region Reservation Charges

                var transactions = reservationChargeRepository.GetReservationCharges(reservation.Id, null);

                #endregion

                #region Rate Type

                var rateType = new RateTypeVM();
                if(reservation.RateCodeId.HasValue)
                    rateType = rateTypeRepository.GetRateTypeById(reservation.RateCodeId.Value).FirstOrDefault();

                #endregion

                #region Room Type

                var roomType = new RoomTypeVM();
                if (reservation.RoomTypeId.HasValue)
                    roomType = roomTypeRepository.GetRoomTypeById(reservation.RoomTypeId.Value).FirstOrDefault();

                #endregion

                BillingInfoVM model = new BillingInfoVM();

                model.ReservationId = reservation.Id;
                model.ProfileId = reservation.ProfileId;
                model.Name = (reservation.LastName + " " + reservation.FirstName).Trim();

                model.Balance = reservation.TotalBalance;
                model.ArrivalDate = reservation.ArrivalDate;
                model.DepartureDate = reservation.DepartureDate;

                model.CompanyId = reservation.CompanyId;
                model.Company = string.Empty;
                model.GroupId = reservation.GroupId;
                model.Group = string.Empty;
                
                model.RoomIds = roomIds;
                model.RoomNumbers = roomNumbers;

                model.RateCodeId = reservation.RateCodeId;
                model.RateCode = rateType.RateTypeCode;
                model.Rate= reservation.Rate;
                model.RoomRent = reservation.TotalPrice;

                model.RoomTypeId = reservation.RoomTypeId;
                model.RoomTypeCode = roomType.RoomTypeCode;
                model.NoOfRooms = reservation.NoOfRoom;

                if (reservation.IsCheckOut)
                {
                    model.IsCheckedOut = reservation.IsCheckOut;
                    model.Status = "CHECKED OUT";
                }
                else
                {
                    model.IsCheckedOut = false;
                    model.Status = "DUE OUT";
                }
                
                model.Transactions = transactions;


                return Json(new
                {
                    IsSuccess = true,
                    Source = source,
                    data = model
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }

        [HttpPost]
        public ActionResult ShowCheckOutPaymentMethod(Guid reservationId, string source = "")
        {
            try
            {
                var reservation = reservationRepository.GetReservationById(reservationId).FirstOrDefault();

                #region Room Mapping

                //Get Room Mapping
                var selectedRooms = roomRepository.GetReservationRoomMapping(reservationId, null);
                var roomIds = string.Empty;
                var roomNumbers = string.Empty;

                if (selectedRooms != null && selectedRooms.Count > 0)
                {
                    foreach (var room in selectedRooms)
                    {
                        roomIds += string.Format("{0},", room.RoomId);
                        roomNumbers += string.Format("{0}, ", room.RoomNo);
                    }

                    if (!string.IsNullOrWhiteSpace(roomIds))
                    {
                        //Remove Last Comma.
                        roomIds = Utility.Utility.RemoveLastCharcter(roomIds, ',');
                    }

                    if (!string.IsNullOrWhiteSpace(roomNumbers))
                    {
                        //Remove Last Comma.
                        roomNumbers = Utility.Utility.RemoveLastCharcter(roomNumbers, ',');
                    }
                }
                
                #endregion

                CheckOutPaymentMethodVM model = new CheckOutPaymentMethodVM();

                model.ReservationId = reservation.Id;
                model.ProfileId = reservation.ProfileId;

                model.CheckOutDate = reservation.DepartureDate;                

                model.NoOfRoom = reservation.NoOfRoom;
                model.Name = Convert.ToString(reservation.LastName + " " + reservation.FirstName).Trim();
                model.PaymentMethodId = reservation.PaymentMethodId;
                model.CreditCardNo = reservation.CreditCardNo;
                model.CardExpiryDate = reservation.CardExpiryDate;
                model.RoomNumbers = roomNumbers;
                model.RoomIds = roomIds;
                model.RoomTypeId = reservation.RoomTypeId;
                model.Amount = reservation.TotalBalance;

                ViewData["Source"] = source;

                return PartialView("_CheckOutPaymentMethod", model);
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }

        [HttpPost]
        public ActionResult CheckOut(CheckOutPaymentMethodVM model)
        {
            try
            {
                CheckInCheckOutVM checkOut = new CheckInCheckOutVM();

                var checkInDetails = checkInCheckOutRepository.GetCheckInDetails(model.ReservationId, model.ProfileId.Value).FirstOrDefault();

                if(checkInDetails != null)
                {
                    string CheckOutTimeText = model.CheckOutTimeText;
                    if (!string.IsNullOrWhiteSpace(CheckOutTimeText))
                    {
                        string todayDate = DateTime.Now.ToString("dd/MM/yyyy");
                        string date = (todayDate + " " + CheckOutTimeText);
                        DateTime time = Convert.ToDateTime(date);

                        checkOut.CheckOutTime = time.TimeOfDay;
                    }

                    //Get Reservation detail.
                    var reservation = reservationRepository.GetReservationById(model.ReservationId).FirstOrDefault();

                    if (reservation != null)
                    {
                        #region Add Entry for Minus All the Expenses

                        var roomRentCharge = additionalChargeRepository.GetAdditionalChargesByCode(AdditionalChargeCode.CHECK_OUT).FirstOrDefault();

                        ReservationChargeVM reservationCharge = new ReservationChargeVM();
                        reservationCharge.ReservationId = reservation.Id;
                        reservationCharge.AdditionalChargeId = roomRentCharge.Id;
                        reservationCharge.Code = roomRentCharge.Code;
                        reservationCharge.Description = model.PaymentMethod;
                        reservationCharge.TransactionDate = model.CheckOutDate.Value;
                        reservationCharge.Amount = -(model.Amount.Value);
                        reservationCharge.Qty = 1;
                        reservationCharge.IsActive = true;
                        reservationCharge.CreatedBy = LogInManager.LoggedInUserId;

                        reservationChargeRepository.AddReservationCharges(reservationCharge);

                        #endregion

                        #region Update Room Occupied Flag

                        var roomIds = model.RoomIds;
                        if (!string.IsNullOrWhiteSpace(roomIds))
                        {
                            var roomIdsArr = roomIds.Split(',');

                            if (roomIdsArr != null)
                            {
                                //Remove Duplication.
                                roomIdsArr = roomIdsArr.Distinct().ToArray();

                                foreach (var item in roomIdsArr)
                                {
                                    //Update Room Occupied Flag.
                                    roomRepository.UpdateRoomOccupiedFlag(Guid.Parse(item.Trim()), false, LogInManager.LoggedInUserId);
                                }
                            }
                        }

                        #endregion

                        #region Update Check Out Details

                        checkOut.Id = checkInDetails.Id;
                        checkOut.ReservationId = model.ReservationId;
                        checkOut.ProfileId = model.ProfileId;
                        checkOut.CheckOutDate = model.CheckOutDate.Value;
                        checkOut.CheckOutReference = model.Reference;
                        checkOut.IsActive = true;
                        checkOut.UpdatedBy = LogInManager.LoggedInUserId;

                        var checkOutId = checkInCheckOutRepository.UpdateCheckOutDetail(checkOut);

                        #endregion

                        #region Update Reservation

                        reservation.PaymentMethodId = model.PaymentMethodId;
                        reservation.CreditCardNo = model.CreditCardNo;
                        reservation.CardExpiryDate = model.CardExpiryDate;

                        //Replace Departure date with  check out date.
                        //reservation.DepartureDate = model.CheckOutDate.Value;

                        //Update Total Balance.
                        if (model.Amount > reservation.TotalBalance)
                        {
                            reservation.TotalBalance = 0;
                        }
                        else
                        {
                            reservation.TotalBalance -= model.Amount;
                        }

                        reservation.UpdatedBy = LogInManager.LoggedInUserId;
                        reservationRepository.UpdateReservation(reservation);

                        #endregion

                        #region Update Reservation Check Out Flag

                        reservationRepository.UpdateReservationCheckOutFlag(model.ReservationId, true, LogInManager.LoggedInUserId);

                        #endregion

                        return Json(new
                        {
                            IsSuccess = true,
                            data = new
                            {
                                CheckOutId = checkOutId,
                                Name = model.Name
                            }
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            IsSuccess = false,
                            errorMessage = "Check In details not exist for this guest."
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = "Check Out details not saved successfully."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }
    }
}