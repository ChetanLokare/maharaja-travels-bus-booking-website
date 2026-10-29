<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="BoardingDetails.aspx.cs" Inherits="BusBookingProject.Admin.BoardingDetails" %>
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
                                <h3 class="h5">Add Boarding Details</h3>
                            </div>
                        </div>
                        <br />
                        <div class="">
                            <asp:ValidationSummary ID="vsRegister" runat="server" CssClass="alert alert-danger" ShowSummary="true" ValidationGroup="vgRegister" />
                            <div id="divMessage" runat="server" />
                            <div class="col-12 col-sm-12 col-md-12">
                                <div class="mb-3">
                                    <asp:Label ID="lblFirstName" runat="server" Text="Boarding Place Name" Font-Bold="true"></asp:Label>
                                    <asp:TextBox ID="txtPlace" runat="server" class="form-control" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPlace" Display="None" ID="rfvFirstName" ValidationGroup="vgRegister"
                                        CssClass="text-danger" ErrorMessage="Boarding Place Name is required." /><br />
                                </div>
                                <div class="mb-3">
                                    <asp:Label ID="lblUserName" runat="server" Text="Boarding Place Expected Time Of Arrival" Font-Bold="true"></asp:Label>
                                    <asp:TextBox ID="txtArrival" runat="server" placeholder="00:00 AM/PM" class="form-control" TextMode="Time" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtArrival" Display="None" ID="rfVMobileNo" ValidationGroup="vgRegister"
                                        CssClass="text-danger" ErrorMessage="Expected Arrival Time is required." /><br />
                                </div>
                            </div>
                            <div class="col-12 col-sm-12 col-md-12">
                                <div class="mb-3">
                                    <asp:Button ID="btnAddBoardingDetails" runat="server" Text="Add Boarding Details" OnClick="btnAddBoardingDetails_Click" class="btn btn-info" ValidationGroup="vgRegister" CausesValidation="True" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
