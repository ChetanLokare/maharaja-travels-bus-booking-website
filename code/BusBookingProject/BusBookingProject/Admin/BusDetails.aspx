<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="BusDetails.aspx.cs" Inherits="BusBookingProject.Admin.BusDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <div class="row">
            <div class="col-md-6 mx-auto">
                <div class="card p-3 p-md-4 shadow-sm">
                    <div class="border rounded p-4" id="Main-Container">
                        <div class="border rounded p-1" id="sub1">
                            <div class="text-center">
                            <div class="text-center border-bottom">
                                <h3>Add Bus Details</h3>
                            </div>
                                
                        </div>
                        </div>
                        <br />
                        <div class="">
                            <asp:ValidationSummary ID="vsRegister" runat="server" CssClass="alert alert-danger" ShowSummary="true" ValidationGroup="vgRegister" />
                            <div id="divMessage" runat="server"></div>
                            <div class="row">
                                <div class="col-12 col-sm-6 col-md-6">
                                    <div class="mb-3">
                                        <i class="fa-solid fa-bus"></i>
                                        <asp:Label ID="lblBus" runat="server" Text="Bus No" Font-Bold="true"></asp:Label>
                                        <asp:TextBox ID="txtBusNo" runat="server" class="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtBusNo" Display="None" ID="rfvFirstName" ValidationGroup="vgRegister"
                                            CssClass="text-danger" ErrorMessage="Bus No is required." /><br />
                                    </div>
                                    <div class="mb-3">
                                        <i class="fa-solid fa-chair"></i>
                                        <asp:Label ID="lblSeatRow" runat="server" Text="No Of Seats Rows" Font-Bold="true"></asp:Label>
                                        <asp:TextBox ID="txtSeatRows" runat="server" class="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtSeatRows" Display="None" ID="rfVMobileNo" ValidationGroup="vgRegister"
                                            CssClass="text-danger" ErrorMessage="Seats Row is required." /><br />
                                    </div>
                                    <div class="mb-3">
                                        <i class="fa-solid fa-location-arrow"></i>
                                        <asp:Label ID="Label1" runat="server" Text="Origin" Font-Bold="true"></asp:Label>
                                        <asp:TextBox ID="txtOrigin" runat="server" class="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtOrigin" Display="None" ID="RequiredFieldValidator1" ValidationGroup="vgRegister"
                                            CssClass="text-danger" ErrorMessage="Origin is Required" /><br />
                                    </div>
                                    <div class="mb-3">
                                        <i class="fa-solid fa-bus"></i>
                                        <asp:Label ID="Label3" runat="server" Text="BusName" Font-Bold="true"></asp:Label>
                                        <asp:TextBox ID="txtBusName" runat="server" class="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtBusName" Display="None" ID="RequiredFieldValidator3" ValidationGroup="vgRegister"
                                            CssClass="text-danger" ErrorMessage="Bus Name is Required" /><br />
                                    </div>
                                </div>
                                <div class="col-12 col-sm-6 col-md-6">
                                    <div class="mb-3">
                                        <i class="fa-solid fa-bus"></i>
                                        <asp:Label ID="lblLastName" runat="server" Text="Bus Type" Font-Bold="true"></asp:Label>
                                        <asp:DropDownList ID="ddlBusType" runat="server" class="form-select">
                                            <asp:ListItem Value="0" Text="Select Bus Type"></asp:ListItem>
                                            <asp:ListItem Value="1" Text="Normal"></asp:ListItem>
                                            <asp:ListItem Value="2" Text="AC"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlBusType" Display="None" ID="rfVLastName" ValidationGroup="vgRegister"
                                            CssClass="text-danger" ErrorMessage="Last Name is required." /><br />
                                    </div>
                                    <div class="mb-3">
                                        <i class="fa-solid fa-chair"></i>
                                        <asp:Label ID="lblPassword" runat="server" Text="Total Seats Column" Font-Bold="true"></asp:Label>
                                        <asp:TextBox ID="txtSeatColumn" runat="server" class="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtSeatColumn" Display="None" ID="rfvPassword" ValidationGroup="vgRegister"
                                            CssClass="text-danger" ErrorMessage="Seats Column is Require" /><br />
                                    </div>
                                    <div class="mb-3">
                                        <i class="fa-solid fa-location-arrow"></i>
                                        <asp:Label ID="Label2" runat="server" Text="Destination" Font-Bold="true"></asp:Label>
                                        <asp:TextBox ID="txtDetination" runat="server" class="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDetination" Display="None" ID="RequiredFieldValidator2" ValidationGroup="vgRegister"
                                            CssClass="text-danger" ErrorMessage="Destination is Required" /><br />
                                    </div>
                                </div>
                            </div>
                            <div class="col-12 col-sm-12 col-md-12">
                                <div class="mb-3">
                                    <asp:Button ID="btnSave" runat="server" Text="Add Bus Details" OnClick="btnSave_Click" class="btn btn-info" ValidationGroup="vgRegister" CausesValidation="True" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
