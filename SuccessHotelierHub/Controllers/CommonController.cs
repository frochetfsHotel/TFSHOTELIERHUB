﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SuccessHotelierHub.Models;
using SuccessHotelierHub.Repository;
using SuccessHotelierHub.Utility;

namespace SuccessHotelierHub.Controllers
{

    public class CommonController : Controller
    {
        #region Declaration

        private CountryRepository countryRepository = new CountryRepository();
        private StateRepository stateRepository = new StateRepository();
        private CityRepository cityRepository = new CityRepository();
        private ProfileRepository profileRepository = new ProfileRepository();
        private RateTypeRepository rateTypeRepository = new RateTypeRepository();
        private RoomTypeRepository roomTypeRepository = new RoomTypeRepository();
        private RoomFeatureRepository roomFeatureRepository = new RoomFeatureRepository();

        private ReservationTypeRepository reservationTypeRepository = new ReservationTypeRepository();
        private MarketRepository marketRepository = new MarketRepository();
        private ReservationSourceRepository reservationSourceRepository = new ReservationSourceRepository();
        private PaymentMethodRepository paymentMethodRepository = new PaymentMethodRepository();
        private ReservationRepository reservationRepository = new ReservationRepository();
        private RateRepository rateRepository = new RateRepository();
        private RTCRepository rtcRepository = new RTCRepository();
        private RoomRepository roomRepository = new RoomRepository();

        #endregion

        [HotelierHubAuthorize(Roles = "ADMIN,STUDENT")]
        public ActionResult GetCountry()
        {
            try
            {
                var countryList = new SelectList(countryRepository.GetCountries(), "Id", "Name").ToList();

                if (countryList != null && countryList.Count() > 0)
                    return Json(new { IsSuccess = true, data = countryList.ToList() }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { IsSuccess = false, data = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Utility.Utility.LogError(e, "GetCountry");
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }

        }

        [HotelierHubAuthorize(Roles = "ADMIN,STUDENT")]
        public ActionResult GetState(int countryId)
        {
            try
            {
                var stateList = new SelectList(stateRepository.GetStates(countryId), "Id", "Name").ToList();

                if (stateList != null && stateList.Count() > 0)
                    return Json(new { IsSuccess = true, data = stateList.ToList() }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { IsSuccess = false, data = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Utility.Utility.LogError(e, "GetState");
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }

        [HotelierHubAuthorize(Roles = "ADMIN,STUDENT")]
        public ActionResult GetCity(int? countryId, int? stateId)
        {
            try
            {
                var cityList = new SelectList(cityRepository.GetCities(countryId, stateId), "Id", "Name").ToList();

                if (cityList != null && cityList.Count() > 0)
                    return Json(new { IsSuccess = true, data = cityList.ToList() }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { IsSuccess = false, data = "" },
                        JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Utility.Utility.LogError(e, "GetCity");
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }

        [HotelierHubAuthorize(Roles = "ADMIN")]
        public ActionResult BulkReservation()
        {
            //return RedirectToAction("Index", "Home");

            var roomTypeList = new SelectList(roomTypeRepository.GetRoomType(string.Empty), "Id", "RoomTypeCode").ToList();
            var rateTypeList = new SelectList(rateTypeRepository.GetRateType(string.Empty)
                                       .Select(
                                           m => new SelectListItem()
                                           {
                                               Value = m.Id.ToString(),
                                               Text = m.RateTypeCode
                                           }
                                       ), "Value", "Text").ToList();


            ViewBag.RoomTypeList = roomTypeList;
            ViewBag.RateTypeList = rateTypeList;


            return View();
        }

        [HotelierHubAuthorize(Roles = "ADMIN")]
        [HttpPost]
        public ActionResult BulkReservation(List<TempBulkReservationVM> models)
        {
            try
            {
                //return RedirectToAction("Index", "Home");

                if (models != null && models.Count > 0)
                {
                    foreach (var model in models)
                    {
                        //Get temp bulk reservation by id.
                        var tempReservation = reservationRepository.GetTempBulkReservationById(model.Id, LogInManager.LoggedInUserId);

                        if (tempReservation != null)
                        {
                            tempReservation.ProfileId = model.ProfileId;
                            tempReservation.LastName = model.LastName;
                            tempReservation.FirstName = model.FirstName;
                            tempReservation.TitleId = model.TitleId;
                            tempReservation.CountryId = model.CountryId;
                            tempReservation.ArrivalDate = model.ArrivalDate;
                            tempReservation.DepartureDate = model.DepartureDate;
                            tempReservation.NoOfNight = model.NoOfNight;
                            tempReservation.RoomNo = model.RoomNo;
                            tempReservation.RoomId = model.RoomId;
                            tempReservation.RoomTypeId = model.RoomTypeId;
                            tempReservation.IsWeekEndPrice = model.IsWeekEndPrice;
                            tempReservation.RateCodeId = model.RateCodeId;
                            tempReservation.Rate = model.Rate;
                            tempReservation.IsActive = model.IsActive;
                            tempReservation.UpdatedBy = LogInManager.LoggedInUserId;

                            //Update Temp Bulk Reservation.
                            reservationRepository.UpdateTempBulkReservation(tempReservation);
                        }
                    }

                    return Json(new
                    {
                        IsSuccess = true
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = "Please select at lease one profile to create temp reservation."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Utility.Utility.LogError(e, "BulkReservation");
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }

        [HotelierHubAuthorize(Roles = "ADMIN")]
        public ActionResult SearchBulkReservationList(string lastName, string firstName)
        {
            try
            {
                //return RedirectToAction("Index", "Home");

                // var profiles = profileRepository.GetIndividualProfiles(lastName, firstName, LogInManager.LoggedInUserId);

                var reservations = reservationRepository.GetTempBulkReservation(LogInManager.LoggedInUserId);

                ViewData["Source"] = "BulkReservation";
                //ViewData["ProfileList"] = profiles;
                ViewData["TempBulkReservationList"] = reservations;

                return PartialView("_ShowBulkReservationList");
            }
            catch (Exception e)
            {
                Utility.Utility.LogError(e, "SearchBulkReservationList");
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }


        [HttpPost]
        public ActionResult SearchBulkReservation(SearchBulkReservationParametersVM model)
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
                model.CreatedBy = LogInManager.LoggedInUserId;

                var reservations = reservationRepository.SearchBulkReservation(model, Convert.ToString(sortColumn), Convert.ToString(sortDirection));

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
                Utility.Utility.LogError(e, "SearchBulkReservation");
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }


        public ActionResult EditBulkReservation(Guid id)
        {
            TempBulkReservationMasterVM model = new TempBulkReservationMasterVM();
            var reservation = reservationRepository.GetTempBulkReservationMassterById(id, LogInManager.LoggedInUserId);

            if (reservation != null)
            {
                model = reservation;

                if (model.ArrivalDate.HasValue)
                {
                    if (model.ArrivalDate.Value.DayOfWeek == DayOfWeek.Friday || model.ArrivalDate.Value.DayOfWeek == DayOfWeek.Saturday)
                    {
                        model.IsWeekEndPrice = true;
                    }
                }

                if (!string.IsNullOrWhiteSpace(model.CreditCardNo))
                {
                    model.CreditCardNo = Utility.Utility.MaskCreditCardNo(model.CreditCardNo);
                }


                #region Room Type
                //Get Room Type Details.
                if (model.RoomTypeId.HasValue)
                {
                    var roomType = roomTypeRepository.GetRoomTypeById(model.RoomTypeId.Value).FirstOrDefault();

                    if (roomType != null)
                    {
                        model.RoomTypeCode = roomType.RoomTypeCode;
                    }
                }
                #endregion

                var roomTypeList = new SelectList(roomTypeRepository.GetRoomType(string.Empty), "Id", "RoomTypeCode").ToList();
                var rateTypeList = new SelectList(rateTypeRepository.GetRateType(string.Empty)
                                        .Select(
                                            m => new SelectListItem()
                                            {
                                                Value = m.Id.ToString(),
                                                Text = m.RateTypeCode
                                            }
                                        ), "Value", "Text").ToList();
                var reservationTypeList = new SelectList(reservationTypeRepository.GetReservationTypes(), "Id", "Name").ToList();

                var marketList = new SelectList(
                              marketRepository.GetMarkets()
                              .Select(
                                  m => new SelectListItem()
                                  {
                                      Value = m.Id.ToString(),
                                      Text = !string.IsNullOrWhiteSpace(m.Description) ? m.Description : m.Name
                                  }
                      ), "Value", "Text").ToList();


                var reservationSourceList = new SelectList(
                              reservationSourceRepository.GetReservationSources()
                              .Select(
                                  m => new SelectListItem()
                                  {
                                      Value = m.Id.ToString(),
                                      Text = !string.IsNullOrWhiteSpace(m.Description) ? m.Description : m.Name
                                  }
                      ), "Value", "Text").ToList();


                var paymentMethodList = new SelectList(
                    paymentMethodRepository.GetPaymentMethods()
                    .Select(
                        m => new SelectListItem()
                        {
                            Value = m.Id.ToString(),
                            Text = (m.Code + " - " + m.Name)
                        }
                    ), "Value", "Text").ToList();
                var roomFeaturesList = roomFeatureRepository.GetRoomFeatures();


                var rtcList = new SelectList(rtcRepository.GetRTC(), "Id", "Code").ToList();

                var roomList = new List<SelectListItem>();
                if (model.RoomTypeId.HasValue)
                {
                    roomList = new SelectList(roomRepository.GetRoomByRoomTypeId(model.RoomTypeId.Value),"Id","RoomNo").ToList();
                }


                ViewBag.RateTypeList = rateTypeList;
                ViewBag.RoomTypeList = roomTypeList;
                ViewBag.ReservationTypeList = reservationTypeList;

                ViewBag.MarketList = marketList;
                ViewBag.ReservationSourceList = reservationSourceList;
                ViewBag.PaymentMethodList = paymentMethodList;
                ViewBag.RoomFeaturesList = roomFeaturesList;
                ViewBag.RtcList = rtcList;
                ViewBag.RoomList = roomList;

                double? dblWeekEndPrice = model.Rate;
                if (model.RoomTypeId.HasValue && model.RateCodeId.HasValue)
                {
                    var weekEndPrice = rateRepository.GetWeekEndPrice(model.RoomTypeId.Value, model.RateCodeId.Value).FirstOrDefault();

                    if (weekEndPrice != null)
                    {
                        dblWeekEndPrice = weekEndPrice.Amount;
                    }
                }

                ViewBag.WeekEndPrice = dblWeekEndPrice;

                return View(model);
            }

            return RedirectToAction("BulkReservation");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBulkReservation(TempBulkReservationMasterVM model)
        {
            try
            {
                string reservationId = string.Empty;

                model.UpdatedBy = LogInManager.LoggedInUserId;
                model.IsActive = true;

                string ETAText = model.ETAText;
                if (!string.IsNullOrWhiteSpace(ETAText))
                {
                    string todayDate = DateTime.Now.ToString("dd/MM/yyyy");
                    string date = (todayDate + " " + ETAText);
                    DateTime time = Convert.ToDateTime(date);

                    model.ETA = time.TimeOfDay;
                }


                double totalBalance = 0, totalPrice = 0;

                totalPrice = Utility.Utility.CalculateRoomRentCharges(model.NoOfNight, (model.Rate.HasValue ? model.Rate.Value : 0), model.NoOfChildren, model.DiscountAmount, model.DiscountPercentage, (model.DiscountPercentage.HasValue ? true : false));

                totalBalance = totalPrice;

                //model.GuestBalance = totalBalance;                
                model.TotalPrice = totalPrice;

                //Credit Card No.
                model.CreditCardNo = Utility.Utility.ExtractCreditCardNoLastFourDigits(model.CreditCardNo);                

                reservationId = reservationRepository.UpdateTempBulkReservationMaster(model);

                return Json(new
                {
                    IsSuccess = true,
                    errorMessage = "Bulk Reservation updated successfully."
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                Utility.Utility.LogError(e, "EditBulkReservation");
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }
    }
}