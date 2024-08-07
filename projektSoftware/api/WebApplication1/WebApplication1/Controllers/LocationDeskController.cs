using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationDeskController : ControllerBase
    {
        private IConfiguration _configuration;

        public LocationDeskController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetLocations")]
        public JsonResult GetLocations()
        {
            string query = "SELECT * FROM dbo.Locations";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("locationDeskDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpPost]
        [Route("AddLocation")]
        public JsonResult AddLocation([FromForm] string newLocation)
        {
            string query = "INSERT INTO dbo.Locations (Name) VALUES (@newLocation)";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("locationDeskDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@newLocation", newLocation);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("added successfully");
        }

        [HttpDelete]
        [Route("DeleteLocation")]
        public JsonResult DeleteLocation(int id)
        {
            string checkQuery = "SELECT COUNT(*) FROM dbo.Desks WHERE LocationId = @id";
            string deleteQuery = "DELETE FROM dbo.Locations WHERE Id = @id";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("locationDeskDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(checkQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    int deskCount = (int)myCommand.ExecuteScalar();
                    if (deskCount > 0)
                    {
                        return new JsonResult("Cannot delete location with desks assigned");
                    }
                }
                using (SqlCommand myCommand = new SqlCommand(deleteQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("deleted successfully");
        }

        [HttpGet]
        [Route("GetDesks")]
        public JsonResult GetDesks()
        {
            string query = "SELECT * FROM dbo.Desks";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("locationDeskDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpPost]
        [Route("AddDesk")]
        public JsonResult AddDesk([FromForm] int locationId, [FromForm] string newDesk)
        {
            string query = "INSERT INTO dbo.Desks (LocationId, Description) VALUES (@locationId, @newDesk)";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("locationDeskDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@locationId", locationId);
                    myCommand.Parameters.AddWithValue("@newDesk", newDesk);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("added successfully");
        }

        [HttpDelete]
        [Route("DeleteDesk")]
        public JsonResult DeleteDesk(int id)
        {
            string checkQuery = "SELECT COUNT(*) FROM dbo.Reservations WHERE DeskId = @id";
            string deleteQuery = "DELETE FROM dbo.Desks WHERE Id = @id";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("locationDeskDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(checkQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    int reservationCount = (int)myCommand.ExecuteScalar();
                    if (reservationCount > 0)
                    {
                        return new JsonResult("Cannot delete desk with active reservations");
                    }
                }
                using (SqlCommand myCommand = new SqlCommand(deleteQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("deleted successfully");
        }

        [HttpPut]
        [Route("SetDeskAvailability")]
        public JsonResult SetDeskAvailability(int id, bool isAvailable)
        {
            string query = "UPDATE dbo.Desks SET IsAvailable = @isAvailable WHERE Id = @id";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("locationDeskDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    myCommand.Parameters.AddWithValue("@isAvailable", isAvailable);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Desk availability updated successfully");
        }

        [HttpGet]
        [Route("GetDesksWithLocations")]
        public JsonResult GetDesksWithLocations()
        {
            string query = @"
        SELECT d.Id, d.Description, d.LocationId, d.IsAvailable, l.Name AS LocationName
        FROM dbo.Desks d
        JOIN dbo.Locations l ON d.LocationId = l.Id";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("locationDeskDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }
        [HttpPost]
        [Route("BookDesk")]
        public JsonResult BookDesk([FromForm] int deskId, [FromForm] string userId, [FromForm] DateTime startDate, [FromForm] DateTime endDate)
        {
            // Check if reservation is not longer than 7 days
            if ((endDate - startDate).TotalDays > 7)
            {
                return new JsonResult("Reservation cannot be longer than 7 days");
            }

            // Check if the user has a reservation for the same day
            string checkQuery = "SELECT COUNT(*) FROM dbo.Reservations WHERE UserId = @userId AND ((StartDate <= @endDate AND EndDate >= @startDate))";
            string insertQuery = "INSERT INTO dbo.Reservations (DeskId, UserId, StartDate, EndDate) VALUES (@deskId, @userId, @startDate, @endDate)";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("locationDeskDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(checkQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@deskId", deskId);
                    myCommand.Parameters.AddWithValue("@userId", userId);
                    myCommand.Parameters.AddWithValue("@startDate", startDate);
                    myCommand.Parameters.AddWithValue("@endDate", endDate);
                    int count = (int)myCommand.ExecuteScalar();
                    if (count > 0)
                    {
                        return new JsonResult("You already have a reservation for these dates");
                    }
                }

                using (SqlCommand myCommand = new SqlCommand(insertQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@deskId", deskId);
                    myCommand.Parameters.AddWithValue("@userId", userId);
                    myCommand.Parameters.AddWithValue("@startDate", startDate);
                    myCommand.Parameters.AddWithValue("@endDate", endDate);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("booked successfully");
        }

        [HttpPut]
        [Route("ChangeReservation")]
        public JsonResult ChangeReservation([FromForm] int reservationId, [FromForm] int newDeskId)
        {
            // Check if the reservation is within 24 hours
            string checkQuery = "SELECT StartDate FROM dbo.Reservations WHERE Id = @reservationId";
            string updateQuery = "UPDATE dbo.Reservations SET DeskId = @newDeskId WHERE Id = @reservationId";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("locationDeskDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                DateTime startDate;
                using (SqlCommand myCommand = new SqlCommand(checkQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@reservationId", reservationId);
                    myReader = myCommand.ExecuteReader();
                    if (myReader.Read())
                    {
                        startDate = (DateTime)myReader["StartDate"];
                        myReader.Close();
                    }
                    else
                    {
                        return new JsonResult("Reservation not found");
                    }
                }

                // Check if the reservation can be changed (at least 24 hours before the start date)
                if ((startDate - DateTime.Now).TotalHours < 24)
                {
                    return new JsonResult("You cannot change the reservation within 24 hours of the start date");
                }

                using (SqlCommand myCommand = new SqlCommand(updateQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@reservationId", reservationId);
                    myCommand.Parameters.AddWithValue("@newDeskId", newDeskId);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Reservation changed successfully");
        }

        [HttpGet]
        [Route("GetDeskReservations")]
        public JsonResult GetDeskReservations([FromQuery] bool isAdmin)
        {
            string query = isAdmin
                ? @"
            SELECT r.Id, r.DeskId, r.UserId, r.StartDate, r.EndDate, d.Description, l.Name AS LocationName
            FROM dbo.Reservations r
            JOIN dbo.Desks d ON r.DeskId = d.Id
            JOIN dbo.Locations l ON d.LocationId = l.Id"
                : @"
            SELECT r.Id, r.DeskId, r.StartDate, r.EndDate, d.Description, l.Name AS LocationName
            FROM dbo.Reservations r
            JOIN dbo.Desks d ON r.DeskId = d.Id
            JOIN dbo.Locations l ON d.LocationId = l.Id";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("locationDeskDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }
    }
}
