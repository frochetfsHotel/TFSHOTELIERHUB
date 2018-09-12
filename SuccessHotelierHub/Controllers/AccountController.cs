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
    public class AccountController : Controller
    {
        #region Declaration

        private UserRepository userRepository = new UserRepository();        

        #endregion

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult SignOut()
        {
            RecordActivityLog.RecordActivity(Pages.LOGOUT, "Loggedout successfully.");            
                        
            System.Web.HttpContext.Current.Session.Clear();
            System.Web.HttpContext.Current.Session.RemoveAll();
            System.Web.HttpContext.Current.Session.Abandon();

            //Here system going to call Session_End event from Global.asax file.

            //Clear Session cookie from browser. So it will generate new session id after session expire.
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = "";
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies["SFToken"] != null)
            {
                Response.Cookies["SFToken"].Value = "";
                Response.Cookies["SFToken"].Expires = DateTime.Now.AddMonths(-20);
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var result = LogInManager.Login(model.Email, Utility.Utility.Encrypt(model.Password, Utility.Utility.EncryptionKey));

                switch (result)
                {
                    case LoginStatus.Success:
                        RecordActivityLog.RecordActivity(Pages.LOGIN, "Loggedin successfully.");

                        if (LogInManager.HasRights("STUDENT"))
                        {
                            //Create Dummy Reservation.
                            TempReservation.CreateDummyReservation();
                        }

                        return Json(new
                        {
                            IsSuccess = true,
                            data = new
                            {
                                UserId = LogInManager.LoggedInUserId
                            }
                        }, JsonRequestBehavior.AllowGet);
                    case LoginStatus.AlreadyLoggedIn:
                        return Json(new
                        {
                            IsSuccess = false,
                            errorMessage = "User already logged in!"
                        }, JsonRequestBehavior.AllowGet);
                    case LoginStatus.Failure:                        
                    default:
                        RecordActivityLog.RecordActivity(Pages.LOGIN, "Login fail.");
                        return Json(new
                        {
                            IsSuccess = false,
                            errorMessage = "Invalid Email and Password."
                        }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Utility.Utility.LogError(e, "Login POST");

                return Json(new
                {
                    IsSuccess = false,
                    errorMessage = e.Message
                });
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return RedirectToAction("Login");
            //return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserVM model)
        {
            try
            {
                if (model.Password != model.ConfirmPassword)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = "Confirm password and password must be same."
                    }, JsonRequestBehavior.AllowGet);
                }

                //Check Email Exist.
                #region Check User Email Exist

                if (this.CheckUserEmailExist(model.Id, model.Email) == true)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = string.Format("Email : {0} already registered.", model.Email)
                    }, JsonRequestBehavior.AllowGet);
                }

                #endregion

                string userId = string.Empty;
                model.CreatedBy = LogInManager.LoggedInUserId;
                model.IsActive = true;

                model.Password = Utility.Utility.Encrypt(model.Password, Utility.Utility.EncryptionKey);

                userId = userRepository.AddUserDetail(model);

                if (!string.IsNullOrWhiteSpace(userId))
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        data = new
                        {
                            UserId = userId
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = "User not registered successfully."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Utility.Utility.LogError(e, "Register POST");

                return Json(new
                {
                    IsSuccess = false,
                    errorMessage = e.Message
                });
            }
        }

        public bool CheckUserEmailExist(Guid? id, string email)
        {
            bool blnExist = false;

            var user = userRepository.CheckUserEmailExist(id, email);

            if (user.Any())
            {
                blnExist = true;
            }

            return blnExist;
        }
    }
}