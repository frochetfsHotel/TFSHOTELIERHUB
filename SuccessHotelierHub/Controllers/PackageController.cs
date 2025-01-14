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
    
    public class PackageController : Controller
    {
        #region Declaration

        private PackageRepository packageRepository = new PackageRepository();

        #endregion

        // GET: Package
        public ActionResult Index()
        {
            return View();
        }

        [HotelierHubAuthorize(Roles = "ADMIN")]
        public ActionResult Create()
        {
            var calculationRatioList = new SelectList(packageRepository.GetPackageCalculationRatio(), "Id", "Name").ToList();

            ViewBag.CalculationRatioList = calculationRatioList;
            return View();
        }

        [HotelierHubAuthorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PackageVM model)
        {
            try
            {
                string packageId = string.Empty;
                model.CreatedBy = LogInManager.LoggedInUserId;

                packageId = packageRepository.AddPackage(model);

                if (!string.IsNullOrWhiteSpace(packageId))
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        data = new
                        {
                            PackageId = packageId
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = "Package details not saved successfully."
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

        [HotelierHubAuthorize(Roles = "ADMIN")]
        public ActionResult Edit(Guid id)
        {
            var package = packageRepository.GetPackageById(id);

            PackageVM model = new PackageVM();

            if (package != null && package.Count > 0)
            {
                model = package[0];

                var calculationRatioList = new SelectList(packageRepository.GetPackageCalculationRatio(), "Id", "Name").ToList();

                ViewBag.CalculationRatioList = calculationRatioList;

                return View(model);
            }

            return RedirectToAction("List");
        }

        [HotelierHubAuthorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PackageVM model)
        {
            try
            {
                string packageId = string.Empty;
                model.UpdatedBy = LogInManager.LoggedInUserId;
                packageId = packageRepository.UpdatePackage(model);

                if (!string.IsNullOrWhiteSpace(packageId))
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        data = new
                        {
                            PackageId = packageId
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = "Package details not updated successfully."
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

        [HotelierHubAuthorize(Roles = "ADMIN")]
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            try
            {
                string packageId = string.Empty;

                packageId = packageRepository.DeletePackage(id, LogInManager.LoggedInUserId);

                if (!string.IsNullOrWhiteSpace(packageId))
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        data = new
                        {
                            PackageId = packageId
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = "Package details not deleted successfully."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Utility.Utility.LogError(e, "Delete");
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }

        [HotelierHubAuthorize(Roles = "ADMIN")]
        public ActionResult List()
        {
            var calculationRatioList = new SelectList(packageRepository.GetPackageCalculationRatio(), "Id", "Name").ToList();

            ViewBag.CalculationRatioList = calculationRatioList;

            return View();
        }

        [HotelierHubAuthorize(Roles = "ADMIN")]
        [HttpPost]
        public ActionResult Search(SearchPackageParametersVM model)
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
                var packages = packageRepository.SearchPackage(model, Convert.ToString(sortColumn), Convert.ToString(sortDirection));

                int totalRecords = 0;
                var dbRecords = packages.Select(m => m.TotalCount).FirstOrDefault();

                if (dbRecords != 0)
                    totalRecords = Convert.ToInt32(dbRecords);

                return Json(new
                {
                    IsSuccess = true,
                    CurrentPage = model.PageNum,
                    PageSize = Constants.PAGESIZE,
                    TotalRecords = totalRecords,
                    data = packages
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                Utility.Utility.LogError(e, "Search");
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }

        [HttpPost]
        [HotelierHubAuthorize(Roles = "ADMIN,STUDENT")]
        public ActionResult SearchAdvancePackage(SearchAdvancePackageParametersVM model)
        {
            try
            {
                var packages = packageRepository.SearchAdvancePackage(model);

                return Json(new
                {
                    IsSuccess = true,
                    data = packages
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                Utility.Utility.LogError(e, "SearchAdvancePackage");
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }
    }
}