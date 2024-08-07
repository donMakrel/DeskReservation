import React, { Component } from "react";
import "./App.css";

class EmployeeApp extends Component {
  constructor(props) {
    super(props);
    this.state = {
      locations: [],
      desks: [],
      reservations: [],
      selectedLocation: null,
      selectedDesk: null,
      selectedReservation: null,
      userId: "user123", // Replace with dynamic user ID
      startDate: "",
      endDate: "",
    };
  }

  API_URL = "http://localhost:5213/";

  componentDidMount() {
    this.refreshLocations();
    this.refreshDesks();
    this.refreshReservations();
  }

  async refreshLocations() {
    fetch(this.API_URL + "api/LocationDesk/GetLocations")
      .then((response) => response.json())
      .then((data) => {
        this.setState({ locations: data });
      });
  }

  async refreshDesks() {
    fetch(this.API_URL + "api/LocationDesk/GetDesksWithLocations")
      .then((response) => response.json())
      .then((data) => {
        this.setState({ desks: data });
      });
  }

  async refreshReservations() {
    fetch(this.API_URL + "api/LocationDesk/GetDeskReservations?isAdmin=false")
      .then((response) => response.json())
      .then((data) => {
        this.setState({ reservations: data });
      });
  }

  async bookDesk() {
    const { selectedDesk, userId, startDate, endDate } = this.state;
    const data = new FormData();
    data.append("deskId", selectedDesk);
    data.append("userId", userId);
    data.append("startDate", startDate);
    data.append("endDate", endDate);
    fetch(this.API_URL + "api/LocationDesk/BookDesk", {
      method: "POST",
      body: data,
    })
      .then((res) => res.json())
      .then((result) => {
        alert(result);
        this.refreshReservations();
      });
  }

  async changeReservation() {
    const { selectedReservation, selectedDesk } = this.state;
    const data = new FormData();
    data.append("reservationId", selectedReservation);
    data.append("newDeskId", selectedDesk);
    fetch(this.API_URL + "api/LocationDesk/ChangeReservation", {
      method: "PUT",
      body: data,
    })
      .then((res) => res.json())
      .then((result) => {
        alert(result);
        this.refreshReservations();
        this.setState({ selectedReservation: null, selectedDesk: null });
      });
  }

  formatDate(dateString) {
    return new Date(dateString).toLocaleDateString();
  }

  render() {
    const { locations, desks, reservations, selectedLocation, selectedDesk, selectedReservation, startDate, endDate } = this.state;
    const unavailableDesks = desks.filter(desk => !desk.IsAvailable);

    return (
      <div className="App">
        <h2>Employee Desk Booking</h2>
        <div className="App-container">
          <select
            onChange={(e) => this.setState({ selectedLocation: parseInt(e.target.value), selectedDesk: null })}
          >
            <option value="">Select Location</option>
            {locations.map((location) => (
              <option key={location.Id} value={location.Id}>
                {location.Name}
              </option>
            ))}
          </select>
          <select
            onChange={(e) => this.setState({ selectedDesk: parseInt(e.target.value) })}
            disabled={!selectedLocation}
          >
            <option value="">Select Desk</option>
            {desks
              .filter((desk) => desk.LocationId === selectedLocation && desk.IsAvailable)
              .map((desk) => (
                <option key={desk.Id} value={desk.Id}>
                  {desk.Description}
                </option>
              ))}
          </select>
          <input
            type="date"
            value={startDate}
            onChange={(e) => this.setState({ startDate: e.target.value })}
          />
          <input
            type="date"
            value={endDate}
            onChange={(e) => this.setState({ endDate: e.target.value })}
          />
          <button className="button-add" onClick={() => this.bookDesk()} disabled={!selectedDesk || !startDate || !endDate}>
            Book Desk
          </button>
          <h3>Unavailable Desks:</h3>
          <div className="desks">
            {unavailableDesks.length > 0 ? (
              unavailableDesks.map((desk) => (
                <p key={desk.Id}>
                  Desk: {desk.Description} - Location: {desk.LocationName}
                </p>
              ))
            ) : (
              <p>All desks are available.</p>
            )}
          </div>
          <h3>Reservations:</h3>
          <div className="reservations">
            {reservations.map((reservation) => (
              <div key={reservation.Id}>
                <p>
                  Desk: {reservation.Description} - Location: {reservation.LocationName} - From: {this.formatDate(reservation.StartDate)} - To: {this.formatDate(reservation.EndDate)}
                </p>
                <button className="button-change" onClick={() => this.setState({ selectedReservation: reservation.Id })}>Change Reservation</button>
                {selectedReservation === reservation.Id && (
                  <div>
                    <select
                      onChange={(e) => this.setState({ selectedDesk: parseInt(e.target.value) })}
                    >
                      <option value="">Select New Desk</option>
                      {desks
                        .filter((desk) => desk.IsAvailable)
                        .map((desk) => (
                          <option key={desk.Id} value={desk.Id}>
                            {desk.Description}
                          </option>
                        ))}
                    </select>
                    <button className="button-change" onClick={() => this.changeReservation()} disabled={!selectedDesk}>
                      Confirm Change
                    </button>
                  </div>
                )}
              </div>
            ))}
          </div>
        </div>
      </div>
    );
  }
}

export default EmployeeApp;
