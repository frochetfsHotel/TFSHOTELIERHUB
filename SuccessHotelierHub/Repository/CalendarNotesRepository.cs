﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using SuccessHotelierHub.Models;

namespace SuccessHotelierHub.Repository
{
    public class CalendarNotesRepository
    {
        #region Calendar Notes

        public List<CalendarNotesVM> GetNotesById(Guid notesId, int userId)
        {
            SqlParameter[] parameters =
               {
                    new SqlParameter { ParameterName = "@Id", Value = notesId },
                    new SqlParameter { ParameterName = "@UserId", Value = userId }
                };

            var dt = DALHelper.GetDataTableWithExtendedTimeOut("GetNotesById", parameters);


            var calendarNotes = new List<CalendarNotesVM>();
            calendarNotes = DALHelper.CreateListFromTable<CalendarNotesVM>(dt);

            return calendarNotes;
        }

        public string AddCalendarNotes(CalendarNotesVM notes)
        {
            string notesId = string.Empty;

            SqlParameter[] parameters =
                {
                    new SqlParameter { ParameterName = "@Date", Value = notes.Date },
                    new SqlParameter { ParameterName = "@Notes", Value = notes.Notes },
                    new SqlParameter { ParameterName = "@IsActive", Value = notes.IsActive },
                    new SqlParameter { ParameterName = "@CreatedBy", Value = notes.CreatedBy }
                };

            notesId = Convert.ToString(DALHelper.ExecuteScalar("AddCalendarNotes", parameters));

            return notesId;
        }

        public string UpdateCalendarNotes(CalendarNotesVM notes)
        {
            string notesId = string.Empty;

            SqlParameter[] parameters =
                {
                    new SqlParameter { ParameterName = "@Id", Value = notes.Id },
                    new SqlParameter { ParameterName = "@Date", Value = notes.Date },
                    new SqlParameter { ParameterName = "@Notes", Value = notes.Notes },
                    new SqlParameter { ParameterName = "@IsActive", Value = notes.IsActive },
                    new SqlParameter { ParameterName = "@UpdatedBy", Value = notes.UpdatedBy }
                };

            notesId = Convert.ToString(DALHelper.ExecuteScalar("UpdateCalendarNotes", parameters));

            return notesId;
        }
      
        public List<CalendarNotesVM> GetCalendarNotesOfCurrentMonth(int month, int year, int userId)
        {
            SqlParameter[] parameters =
               {
                    new SqlParameter { ParameterName = "@Month" , Value = month },
                    new SqlParameter { ParameterName = "@Year"  , Value = year },
                    new SqlParameter { ParameterName = "@UserId", Value = userId }
                };

            var dt = DALHelper.GetDataTableWithExtendedTimeOut("GetCalendarNotesOfCurrentMonth", parameters);


            var notes = new List<CalendarNotesVM>();
            notes = DALHelper.CreateListFromTable<CalendarNotesVM>(dt);

            return notes;
        }

        public string DeleteCalendarNotes(Guid id, int updatedBy)
        {
            string notesId = string.Empty;

            SqlParameter[] parameters =
                {
                    new SqlParameter { ParameterName = "@Id", Value = id },
                    new SqlParameter { ParameterName = "@UpdatedBy", Value = updatedBy }
                };

            notesId = Convert.ToString(DALHelper.ExecuteScalar("DeleteCalendarNotes", parameters));

            return notesId;
        }

        #endregion
    }
}