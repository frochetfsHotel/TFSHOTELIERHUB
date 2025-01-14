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
    public class PaymentMethodController : Controller
    {
        #region Declaration

        private PaymentMethodRepository paymentMethodRepository = new PaymentMethodRepository();

        #endregion

        // GET: PaymentMethod
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
        public ActionResult Create(PaymentMethodVM model)
        {
            try
            {
                string paymentMethodId = string.Empty;
                model.CreatedBy = LogInManager.LoggedInUserId;

                paymentMethodId = paymentMethodRepository.AddPaymentMethod(model);

                if (!string.IsNullOrWhiteSpace(paymentMethodId))
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        data = new
                        {
                            PaymentMethodId = paymentMethodId
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false, 
                        errorMessage = "Payment Method details not saved successfully."
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
            var paymentMethod = paymentMethodRepository.GetPaymentMethodById(id);

            PaymentMethodVM model = new PaymentMethodVM();

            if (paymentMethod != null && paymentMethod.Count > 0)
            {
                model = paymentMethod[0];

                return View(model);
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PaymentMethodVM model)
        {
            try
            {
                string paymentMethodId = string.Empty;
                model.UpdatedBy = LogInManager.LoggedInUserId;
                model.Code = !string.IsNullOrWhiteSpace(model.Code) ? model.Code.ToUpper() : model.Code;
                paymentMethodId = paymentMethodRepository.UpdatePaymentMethod(model);

                if (!string.IsNullOrWhiteSpace(paymentMethodId))
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        data = new
                        {
                            PaymentMethodId = paymentMethodId
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = "Payment Method details not updated successfully."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Utility.Utility.LogError(e, "Edit");
                return Json(new
                {
                    IsSuccess = false,
                    errorMessage = e.Message
                });
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            try
            {
                string paymentMethodId = string.Empty;

                paymentMethodId = paymentMethodRepository.DeletePaymentMethod(id, LogInManager.LoggedInUserId);

                if (!string.IsNullOrWhiteSpace(paymentMethodId))
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        data = new
                        {
                            PaymentMethodId = paymentMethodId
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = "Payment Method details not deleted successfully."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Utility.Utility.LogError(e, "Delete");
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(SearchPaymentMethodParametersVM model)
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
                var paymentMethods = paymentMethodRepository.SearchPaymentMethod(model, Convert.ToString(sortColumn), Convert.ToString(sortDirection));

                int totalRecords = 0;
                var dbRecords = paymentMethods.Select(m => m.TotalCount).FirstOrDefault();

                if (dbRecords != 0)
                    totalRecords = Convert.ToInt32(dbRecords);

                return Json(new
                {
                    IsSuccess = true,
                    CurrentPage = model.PageNum,
                    PageSize = Constants.PAGESIZE,
                    TotalRecords = totalRecords,
                    data = paymentMethods
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                Utility.Utility.LogError(e, "Search");
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }
    }
}