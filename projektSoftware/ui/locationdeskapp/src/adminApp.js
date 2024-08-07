import React, { Component } from "react";
import "./App.css";

class AdminApp extends Component {
  constructor(props) {
    super(props);
    this.state = {
      locations: [],
      desksWithLocations: [],
      reservations: [],
      selectedLocation: null,
    };
  }

  API_URL = "http://localhost:5213/";

  componentDidMount() {
    this.refreshLocations();
    this.refreshDesksWithLocations();
    this.refreshReservations();
  }

  async refreshLocations() {
    fetch(this.API_URL + "api/LocationDesk/GetLocations")
      .then((response) => response.json())
      .then((data) => {
        this.setState({ locations: data });
      });
  }

  async refreshDesksWithLocations() {
    fetch(this.API_URL + "api/LocationDesk/GetDesksWithLocations")
      .then((response) => response.json())
      .then((data) => {
        this.setState({ desksWithLocations: data });
      });
  }

  async refreshReservations() {
    fetch(this.API_URL + "api/LocationDesk/GetDeskReservations?isAdmin=true")
      .then((response) => response.json())
      .then((data) => {
        this.setState({ reservations: data });
      });
  }

  async addLocation() {
    var newLocation = document.getElementById("newLocation").value;
    const data = new FormData();
    data.append("newLocation", newLocation);
    fetch(this.API_URL + "api/LocationDesk/AddLocation", {
      method: "POST",
      body: data,
    })
      .then((res) => res.json())
      .then((result) => {
        alert(result);
        this.refreshLocations();
      });
  }

  async deleteLocation(id) {
    fetch(this.API_URL + "api/LocationDesk/DeleteLocation?id=" + id, {
      method: "DELETE",
    })
      .then((res) => res.json())
      .then((result) => {
        alert(result);
        this.refreshLocations();
      });
  }

  async addDesk() {
    var locationId = this.state.selectedLocation;
    var newDesk = document.getElementById("newDesk").value;
    const data = new FormData();
    data.append("locationId", locationId);
    data.append("newDesk", newDesk);
    fetch(this.API_URL + "api/LocationDesk/AddDesk", {
      method: "POST",
      body: data,
    })
      .then((res) => res.json())
      .then((result) => {
        alert(result);
        this.refreshDesksWithLocations();
      });
  }

  async deleteDesk(id) {
    fetch(this.API_URL + "api/LocationDesk/DeleteDesk?id=" + id, {
      method: "DELETE",
    })
      .then((res) => res.json())
      .then((result) => {
        if (result === "Cannot delete desk with active reservations") {
          alert(result);
        } else {
          alert("Desk deleted successfully");
          this.refreshDesksWithLocations();
        }
      });
  }

  async setDeskAvailability(id, isAvailable) {
    fetch(this.API_URL + `api/LocationDesk/SetDeskAvailability?id=${id}&isAvailable=${isAvailable}`, {
      method: "PUT",
    })
      .then((res) => res.json())
      .then((result) => {
        alert(result);
        this.refreshDesksWithLocations();
      });
  }

  formatDate(dateString) {
    return new Date(dateString).toLocaleDateString();
  }

  render() {
    const { locations, desksWithLocations, reservations, selectedLocation } = this.state;
    return (
      <div className="App">
        <h2>Admin Desk Management</h2>
        <div className="App-container">
          <input id="newLocation" placeholder="New Location" />
          &nbsp;
          <button className="button-add" onClick={() => this.addLocation()}>Add Location</button>
          <h3>Locations:</h3>
          <div className="locations">
            {locations.map((location) => (
              <p key={location.Id}>
                <b>{location.Name}</b>
                <span>
                  <button className="button-delete" onClick={() => this.deleteLocation(location.Id)}>Delete Location</button>
                </span>
              </p>
            ))}
          </div>
          <h3>Desks:</h3>
          <select
            onChange={(e) => this.setState({ selectedLocation: parseInt(e.target.value) })}
          >
            <option value="">Select Location</option>
            {locations.map((location) => (
              <option key={location.Id} value={location.Id}>
                {location.Name}
              </option>
            ))}
          </select>
          <input id="newDesk" placeholder="New Desk" />
          &nbsp;
          <button className="button-add" onClick={() => this.addDesk()}>Add Desk</button>
          <div className="desks">
            {desksWithLocations.map((desk) => (
              <p key={desk.Id}>
                <b>{desk.Description}</b> - {desk.LocationName} - {desk.IsAvailable ? "Available" : "Unavailable"}
                <span>
                  <button className="button-delete" onClick={() => this.deleteDesk(desk.Id)}>Delete Desk</button>
                  <button className="button-change" onClick={() => this.setDeskAvailability(desk.Id, !desk.IsAvailable)}>
                    {desk.IsAvailable ? "Mark as Unavailable" : "Mark as Available"}
                  </button>
                </span>
              </p>
            ))}
          </div>
          <h3>Reservations:</h3>
          <div className="reservations">
            {reservations.map((reservation) => (
              <p key={reservation.Id}>
                Desk: {reservation.Description} - Location: {reservation.LocationName} - User: {reservation.UserId} - From: {this.formatDate(reservation.StartDate)} - To: {this.formatDate(reservation.EndDate)}
              </p>
            ))}
          </div>
        </div>
      </div>
    );
  }
}

export default AdminApp;
