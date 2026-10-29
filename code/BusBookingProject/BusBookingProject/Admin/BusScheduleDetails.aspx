<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="BusScheduleDetails.aspx.cs" Inherits="BusBookingProject.Admin.BusScheduleDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <div class="row">
            <div class="col-md-6 mx-auto">
                <div class="card p-3 p-md-4 shadow-sm">
                    <div class="border rounded p-4" id="Main-Container">
                        <div class="border rounded p-1">
                            <div class="text-center border-bottom">
                                <h3 class="h5">Add Bus Schedule</h3>
                            </div>
                        </div>
                        <br />
                        <div class="">
                            <asp:ValidationSummary ID="vsRegister" runat="server" CssClass="alert alert-danger" ShowSummary="true" ValidationGroup="vgRegister" />
                            <div id="divMessage" runat="server" />
                            <div class="row">
                                <div class="col-md-6">
                                    <div class="mb-3">
                                        <i class="fa-solid fa-calendar-days"></i>
                                        <asp:Label ID="lblFirstName" runat="server" Text="Available Date" Font-Bold="true"></asp:Label>
                                        <asp:TextBox ID="txtDate" runat="server" placeholder="DD/MM/YYYY Format" class="form-control" TextMode="Date" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDate" Display="None" ID="rfvFirstName" ValidationGroup="vgRegister"
                                            CssClass="text-danger" ErrorMessage="Date is required." />
                                    </div>
                                    <div class="mb-3">
                                        <i class="fa-solid fa-indian-rupee-sign"></i>
                                        <asp:Label ID="lblUserName" runat="server" Text="Fare" Font-Bold="true"></asp:Label>
                                        <asp:TextBox ID="txtFare" runat="server" class="form-control" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFare" Display="None" ID="rfVMobileNo" ValidationGroup="vgRegister"
                                            CssClass="text-danger" ErrorMessage="Fare is required." />
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="mb-3">
                                        <i class="fa-solid fa-clock"></i>
                                        <asp:Label ID="lblLastName" runat="server" Text="Total Travelling Time" Font-Bold="true"></asp:Label>
                                        <asp:TextBox ID="txtTravelTime" runat="server" placeholder="00hr00Mints Format" class="form-control" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTravelTime" Display="None" ID="rfVLastName" ValidationGroup="vgRegister"
                                            CssClass="text-danger" ErrorMessage="Total Travel Time is required." />
                                    </div>
                                    <div class="mb-3">
                                        <i class="fa-solid fa-clock"></i>
                                        <asp:Label ID="lblPassword" runat="server" Text="Arrival Time" Font-Bold="true"></asp:Label>
                                        <asp:TextBox ID="txtArrival" runat="server" placeholder="00:00 AM/PM Format" class="form-control" TextMode="Time" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtArrival" Display="None" ID="rfvPassword" ValidationGroup="vgRegister"
                                            CssClass="text-danger" ErrorMessage="Arrival Time is required." />
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="mb-3">
                                        <i class="fa-solid fa-clock"></i>
                                        <asp:Label ID="lblEmail" runat="server" Text="Departure Time" Font-Bold="true"></asp:Label>
                                        <asp:TextBox ID="txtDeparture" placeholder="00:00 AM/PM Format" runat="server" class="form-control" TextMode="Time" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDeparture" Display="None" ID="rfVEmail" ValidationGroup="vgRegister"
                                            CssClass="text-danger" ErrorMessage="Departure Time is required." />
                                    </div>
                                </div>
                                <div class="col-md-12">
                                    <div class="mb-3">
                                        <asp:Button ID="btnSaveSchedule" runat="server" Text="Save Bus Schedule" Style="margin-top: 20px;" OnClick="btnSaveSchedule_Click" class="btn btn-info" ValidationGroup="vgRegister" CausesValidation="True" ViewStateMode="Inherit" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
