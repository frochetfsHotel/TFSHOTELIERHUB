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
    [HotelierHubAuthorize]
    public class RoomController : Controller
    {
        #region Declaration 

        private RoomTypeRepository roomTypeRepository = new RoomTypeRepository();
        private FloorRepository floorRepository = new FloorRepository();
        private RoomRepository roomRepository = new RoomRepository();
        private RoomStatusRepository roomStatusRepository = new RoomStatusRepository();

        #endregion

        #region Room

        public ActionResult Index()
        {
            var roomTypeList = roomTypeRepository.GetRoomType(string.Empty);

            ViewBag.RoomTypeList = roomTypeList;

            return View();
        }

        public ActionResult ShowRoomList(Guid roomTypeId)
        {
            var roomType = roomTypeRepository.GetRoomTypeById(roomTypeId).FirstOrDefault();

            if (roomType != null)
            {
                var roomList = roomRepository.GetRoomByRoomTypeId(roomType.Id);                
                var floorList = floorRepository.GetFloors();

                ViewBag.RoomList = roomList;                
                ViewBag.FloorList = floorList;
                ViewBag.NoOfRooms = roomType.NoOfRooms;

                return PartialView("_RoomList");
            }

            return Json(new
            {
                IsSuccess = false,
                errorMessage = "Not found any rooms details."
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            try
            {
                string roomId = string.Empty;

                var roomDetail = roomRepository.GetRoomById(id).FirstOrDefault();

                roomId = roomRepository.DeleteRoom(id, LogInManager.LoggedInUserId);

                if (!string.IsNullOrWhiteSpace(roomId))
                {
                    #region Update Room Type (No. Of Rooms)

                    //Decrease the no. of rooms quantity from room type.
                    var roomType = roomTypeRepository.GetRoomTypeById(roomDetail.RoomTypeId).FirstOrDefault();

                    roomType.NoOfRooms = roomType.NoOfRooms > 0 ? (roomType.NoOfRooms - 1) : 0;

                    roomTypeRepository.UpdateRoomType(roomType);

                    #endregion

                    return Json(new
                    {
                        IsSuccess = true,
                        data = new
                        {
                            RoomId = id
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = "Room not deleted successfully."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }

        [HttpPost]
        public ActionResult UpdateRooms(List<RoomVM> models)
        {
            try
            {
                string roomId = string.Empty;

                if (models != null && models.Count > 0)
                {
                    foreach (var room in models)
                    {
                        room.UpdatedBy = LogInManager.LoggedInUserId;

                        #region Check Room No Available.

                        if (this.CheckRoomNoAvailable(room.Id, room.RoomNo) == false)
                        {
                            return Json(new
                            {
                                IsSuccess = false,
                                errorMessage = string.Format("Room No : {0} already exist.", room.RoomNo)
                            }, JsonRequestBehavior.AllowGet);
                        }

                        #endregion

                        room.StatusId = Guid.Parse(RoomStatusType.CLEAN);

                        roomId = roomRepository.UpdateRoom(room);
                    }

                    return Json(new
                    {
                        IsSuccess = true,
                        data = new
                        {
                            RoomTypeId = roomId
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = "Room details not updated successfully."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    IsSuccess = false,
                    errorMessage = e.Message
                });
            }

        }

        [HttpPost]
        public ActionResult SearchAdvanceRoom(SearchAdvanceRoomParametersVM model)
        {
            try
            {
                var rooms = roomRepository.SearchAdvanceRoom(model);

                return Json(new
                {
                    IsSuccess = true,
                    data = rooms
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, errorMessage = e.Message });
            }
        }

        public ActionResult ShowAddRoom(Guid? roomTypeId)
        {
            try
            {
                var roomTypeList = new SelectList(roomTypeRepository.GetRoomType(string.Empty), "Id", "RoomTypeCode");
                var floorList = new SelectList(floorRepository.GetFloors(), "Id", "Name");

                ViewBag.RoomTypeList = roomTypeList;
                ViewBag.FloorList = floorList;

                RoomVM model = new RoomVM();
                model.IsActive = true;

                if (roomTypeId.HasValue)
                {
                    model.RoomTypeId = roomTypeId.Value;
                }

                return PartialView("_AddRoom", model);
            }
            catch (Exception e)
            {
                return Json(new
                {
                    IsSuccess = false,
                    errorMessage = e.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoomVM model)
        {
            try
            {
                string roomId = string.Empty;
                model.CreatedBy = LogInManager.LoggedInUserId;

                #region Check Room No Available.

                if (this.CheckRoomNoAvailable(model.Id, model.RoomNo) == false)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = string.Format("Room No : {0} already exist.", model.RoomNo)
                    }, JsonRequestBehavior.AllowGet);
                }

                #endregion

                model.StatusId = Guid.Parse(RoomStatusType.CLEAN);
                roomId = roomRepository.AddRoom(model);

                if (!string.IsNullOrWhiteSpace(roomId))
                {
                    model.Id = Guid.Parse(roomId);

                    #region Update Room Type (No. Of Rooms)

                    //Increase the no. of rooms quantity in room type.
                    var roomType = roomTypeRepository.GetRoomTypeById(model.RoomTypeId).FirstOrDefault();

                    roomType.NoOfRooms = roomType.NoOfRooms > 0 ? (roomType.NoOfRooms + 1) : 0;

                    roomTypeRepository.UpdateRoomType(roomType);

                    #endregion

                    return Json(new
                    {
                        IsSuccess = true,
                        data = new
                        {
                            RoomId = roomId,
                            Room = model
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        errorMessage = "Room details not saved successfully."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    IsSuccess = false,
                    errorMessage = e.Message
                });
            }
        }

        public bool CheckRoomNoAvailable(Guid? roomId, string roomNo)
        {
            bool blnAvailable = true;

            var room = roomRepository.CheckRoomNoAvailable(roomId, roomNo);

            if (room.Any())
            {
                blnAvailable = false;
            }

            return blnAvailable;
        }

        #endregion
    }
}