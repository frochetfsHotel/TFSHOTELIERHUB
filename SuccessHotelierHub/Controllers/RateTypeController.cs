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
    [HotelierHubAuthorize(Roles = "ADMIN")]
    public class RateTypeController : Controller
    {
        #region Declaration

        private RateTypeRepository rateTypeRepository = new RateTypeRepository();
        private RateTypeCategoryRepository rateTypeCategoryRepository = new RateTypeCategoryRepository();

        #endregion

        // GET: RateType
        public ActionResult Create()
        {
            var rateTypeCategoryList = new SelectList(rateTypeCategoryRepository.GetRateTypeCategory(), "Id", "Name");
            ViewBag.RateTypeCategoryList = rateTypeCategoryList;
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RateTypeVM model)
        {
            try
            {
                string rateTypeId = string.Empty;
                model.CreatedBy = LogInManager.LoggedInUserId;
                model.Amount = 100; //Default Price

                #region Check Rate Type Code Available.

                if (this.CheckRateTypeCodeAvailable(model.Id, model.RateTypeCode) == false)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = string.Format("Rate Type Code : {0} already exist.", model.RateTypeCode)
                    }, JsonRequestBehavior.AllowGet);
                }

                #endregion

                model.IsLeisRateType = false;

                rateTypeId = rateTypeRepository.AddRateType(model);

                if (!string.IsNullOrWhiteSpace(rateTypeId))
                {
                    #region  Check Source Parameters
                    if (Request.Form["Source"] != null && !string.IsNullOrWhiteSpace(Convert.ToString(Request.Form["Source"])))
                    {
                        string source = string.Empty;
                        string url = string.Empty;
                        string qid = string.Empty;

                        source = Convert.ToString(Request.Form["Source"]);

                        if (source == "WeekEndPrice")
                        {
                            TempData["TabName"] = "WeekEndPrice";
                            url = Url.Action("ManagePrice", "Rate");
                        }

                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            return Json(new
                            {
                                IsSuccess = true,
                                IsExternalUrl = true,
                                data = url
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    #endregion

                    return Json(new
                    {
                        IsSuccess = true,
                        data = new
                        {
                            RateTypeId = model.Id
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = "Rate Type details not saved successfully."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Utility.Utility.LogError(e, "Create");
                return Json(new
                {
                    IsSuccess = false,
                    errorMessage = e.Message
                });
            }
        }

        public ActionResult Edit(Guid id)
        {
            var rateType = rateTypeRepository.GetRateTypeById(id);

            RateTypeVM model = new RateTypeVM();

            if (rateType != null && rateType.Count > 0)
            {
                model = rateType[0];

                var rateTypeCategoryList = new SelectList(rateTypeCategoryRepository.GetRateTypeCategory(), "Id", "Name");
                ViewBag.RateTypeCategoryList = rateTypeCategoryList;

                return View(model);
            }

            return RedirectToAction("List");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RateTypeVM model)
        {
            try
            {
                string rateTypeId = string.Empty;
                model.Amount = 100; //Default Price
                model.UpdatedBy = LogInManager.LoggedInUserId;

                #region Check Rate Type Code Available.

                if (this.CheckRateTypeCodeAvailable(model.Id, model.RateTypeCode) == false)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = string.Format("Rate Type Code : {0} already exist.", model.RateTypeCode)
                    }, JsonRequestBehavior.AllowGet);
                }

                #endregion

                model.IsLeisRateType = false;

                rateTypeId = rateTypeRepository.UpdateRateType(model);

                if (!string.IsNullOrWhiteSpace(rateTypeId))
                {
                    #region  Check Source Parameters
                    if (Request.Form["Source"] != null && !string.IsNullOrWhiteSpace(Convert.ToString(Request.Form["Source"])))
                    {
                        string source = string.Empty;
                        string url = string.Empty;
                        string qid = string.Empty;

                        source = Convert.ToString(Request.Form["Source"]);

                        if (source == "WeekEndPrice")
                        {
                            TempData["TabName"] = "WeekEndPrice";
                            url = Url.Action("ManagePrice", "Rate");
                        }

                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            return Json(new
                            {
                                IsSuccess = true,
                                IsExternalUrl = true,
                                data = url
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    #endregion

                    return Json(new
                    {
                        IsSuccess = true,
                        data = new
                        {
                            RateTypeId = model.Id
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = "Rate Type details not updated successfully."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Utility.Utility.LogError(e, "Edit");
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            try
            {
                string rateTypeId = string.Empty;

                rateTypeId = rateTypeRepository.DeleteRateType(id, LogInManager.LoggedInUserId);

                if (!string.IsNullOrWhiteSpace(rateTypeId))
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        data = new
                        {
                            RateTypeId = id
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = "Rate Type not deleted successfully."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Utility.Utility.LogError(e, "Delete");
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }


        [HttpPost]
        public ActionResult DeleteSelected(List<Guid> ids)
        {
            try
            {
                var isDelete = false;

                if (ids != null)
                {
                    foreach (var id in ids)
                    {
                        rateTypeRepository.DeleteRateType(id, LogInManager.LoggedInUserId);
                        isDelete = true;
                    }
                }

                if (isDelete)
                {
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
                        errorMessage = "Rate Types not deleted successfully."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Utility.Utility.LogError(e, "DeleteSelected");
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }

        public ActionResult List()
        {
            var rateTypeCategoryList = new SelectList(rateTypeCategoryRepository.GetRateTypeCategory(), "Id", "Name");
            ViewBag.RateTypeCategoryList = rateTypeCategoryList;

            return View();
        }

        [HttpPost]
        public ActionResult Search(SearchRateTypeParametersVM model)
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
                var rateTypes = rateTypeRepository.SearchRateType(model, Convert.ToString(sortColumn), Convert.ToString(sortDirection));

                int totalRecords = 0;
                var dbRecords = rateTypes.Select(m => m.TotalCount).FirstOrDefault();

                if (dbRecords != 0)
                    totalRecords = Convert.ToInt32(dbRecords);

                return Json(new
                {
                    IsSuccess = true,
                    CurrentPage = model.PageNum,
                    PageSize = Constants.PAGESIZE,
                    TotalRecords = totalRecords,
                    data = rateTypes
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                Utility.Utility.LogError(e, "Search");
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }

        public ActionResult GetRateTypeDetailsByRoomType(Guid roomTypeId, bool isWeekEndPrice = false)
        {
            try
            {
                var rateTypes = rateTypeRepository.GetRateTypeDetailsByRoomType(roomTypeId, isWeekEndPrice);

                return Json(new
                {
                    IsSuccess = true,                    
                    data = rateTypes
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                Utility.Utility.LogError(e, "GetRateTypeDetailsByRoomType");
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }

        public bool CheckRateTypeCodeAvailable(Guid? rateTypeId, string rateTypeCode)
        {
            bool blnAvailable = true;

            var rateType = rateTypeRepository.CheckRateTypeCodeAvailable(rateTypeId, rateTypeCode);

            if (rateType.Any())
            {
                blnAvailable = false;
            }

            return blnAvailable;
        }
    }
}