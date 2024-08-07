import React, { Component } from "react";
import "./App.css";
import AdminApp from "./adminApp"; 
import EmployeeApp from "./EmployeeApp";

class App extends Component {
  constructor(props) {
    super(props);
    this.state = {
      view: "admin",
    };
  }

  switchView(view) {
    this.setState({ view });
  }

  render() {
    const { view } = this.state;
    return (
      <div className="App">
        <button onClick={() => this.switchView("admin")}>Admin View</button>
        <button onClick={() => this.switchView("employee")}>Employee View</button>
        {view === "admin" ? <AdminApp /> : <EmployeeApp />}
      </div>
    );
  }
}

export default App;