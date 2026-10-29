<%@ Page Title="Feedback" Language="C#" MasterPageFile="~/BusBookingMaster.Master" AutoEventWireup="true" CodeBehind="Feedback.aspx.cs" Inherits="BusBookingProject.Feedback" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container py-4">
        <div class="row justify-content-center">
            <div class="col-lg-9">
                <div class="text-center mb-4"><h1 class="h2">We would love your feedback</h1><p class="text-body-secondary mb-0">Tell us what went well, or what we should fix.</p></div>
                <div class="card p-4 p-md-5" data-reveal>
                    <div class="row g-3">
                        <div class="col-md-6"><label class="form-label small">Your name</label><asp:TextBox ID="txtUserName" runat="server" CssClass="form-control" MaxLength="100" required="required"></asp:TextBox></div>
                        <div class="col-md-6"><label class="form-label small">E-mail</label><asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" MaxLength="100" required="required"></asp:TextBox></div>
                        <div class="col-md-6"><label class="form-label small">Contact number</label><asp:TextBox ID="txtContactNo" runat="server" CssClass="form-control" TextMode="Phone" MaxLength="20"></asp:TextBox></div>
                        <div class="col-md-6"><label class="form-label small">How was your experience?</label>
                            <asp:DropDownList ID="ddlRating" runat="server" CssClass="form-select" required="required">
                                <asp:ListItem Value="" Text="Select a rating"></asp:ListItem>
                                <asp:ListItem Value="5" Text="5 - Excellent"></asp:ListItem>
                                <asp:ListItem Value="4" Text="4 - Good"></asp:ListItem>
                                <asp:ListItem Value="3" Text="3 - Okay"></asp:ListItem>
                                <asp:ListItem Value="2" Text="2 - Bad"></asp:ListItem>
                                <asp:ListItem Value="1" Text="1 - Very bad"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-6"><label class="form-label small">Feedback</label><asp:TextBox ID="txtFeedback" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" MaxLength="2000"></asp:TextBox></div>
                        <div class="col-md-6"><label class="form-label small">Complaint or suggestion</label><asp:TextBox ID="txtComplain" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" MaxLength="2000"></asp:TextBox></div>
                        <div class="col-12 d-grid d-sm-block"><asp:Button ID="btnSubmit" runat="server" Text="Send feedback" CssClass="btn btn-primary btn-lg px-5" OnClick="btnSubmit_Click" /></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server"></asp:Content>
