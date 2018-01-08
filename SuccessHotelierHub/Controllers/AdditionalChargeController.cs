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
    public class AdditionalChargeController : Controller
    {
        #region Declaration 

        private AdditionalChargeRepository additionalChargeRepository = new AdditionalChargeRepository();

        #endregion

        // GET: AdditionalCharge
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdditionalChargeVM model)
        {
            try
            {
                string chargeId = string.Empty;

                #region Check Additional Charge Code Available.
                if (this.CheckAdditionalChargeCodeAvailable(model.Id, model.Code) == false)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = string.Format("Additional Charge Code : {0} already exist.", model.Code)
                    }, JsonRequestBehavior.AllowGet);
                }
                #endregion

                model.CreatedBy = LogInManager.LoggedInUserId;
                chargeId = additionalChargeRepository.AddAdditionalCharges(model);

                if (!string.IsNullOrWhiteSpace(chargeId))
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        data = new
                        {
                            AdditionalChargeId = model.Id
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = "Additional Charges not saved successfully."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }

        public ActionResult Edit(Guid id)
        {
            var charge = additionalChargeRepository.GetAdditionalChargesById(id);

            AdditionalChargeVM model = new AdditionalChargeVM();

            if (charge != null && charge.Count > 0)
            {
                model = charge[0];
                
                return View(model);
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdditionalChargeVM model)
        {
            try
            {
                string chargeId = string.Empty;

                #region Check Additional Charge Code Available.
                if (this.CheckAdditionalChargeCodeAvailable(model.Id, model.Code) == false)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = string.Format("Additional Charge Code : {0} already exist.", model.Code)
                    }, JsonRequestBehavior.AllowGet);
                }
                #endregion

                model.UpdatedBy = LogInManager.LoggedInUserId;
                chargeId = additionalChargeRepository.UpdateAdditionalCharges(model);

                if (!string.IsNullOrWhiteSpace(chargeId))
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        data = new
                        {
                            AdditionalChargeId = model.Id
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = "Additional Charges not updated successfully."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            try
            {
                string chargeId = string.Empty;

                chargeId = additionalChargeRepository.DeleteAdditionalCharges(id, LogInManager.LoggedInUserId);

                if (!string.IsNullOrWhiteSpace(chargeId))
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        data = new
                        {
                            AdditionalChargeId = id
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = "Additional Charges not deleted successfully."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchAdditionalCharges(SearchAdditionalChargeParametersVM model)
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
                var charges = additionalChargeRepository.SearchAdditionalCharges(model, Convert.ToString(sortColumn), Convert.ToString(sortDirection));

                int totalRecords = 0;
                var dbRecords = charges.Select(m => m.TotalCount).FirstOrDefault();

                if (dbRecords != 0)
                    totalRecords = Convert.ToInt32(dbRecords);

                return Json(new
                {
                    IsSuccess = true,
                    CurrentPage = model.PageNum,
                    PageSize = Constants.PAGESIZE,
                    TotalRecords = totalRecords,
                    data = charges
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }

        public bool CheckAdditionalChargeCodeAvailable(Guid? id, string code)
        {
            bool blnAvailable = true;

            var charge = additionalChargeRepository.CheckAdditionalChargeCodeAvailable(id, code);

            if (charge.Any())
            {
                blnAvailable = false;
            }

            return blnAvailable;
        }
    }
}