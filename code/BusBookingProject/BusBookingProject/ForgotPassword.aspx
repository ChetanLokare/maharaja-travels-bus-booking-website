<%@ Page Title="Forgot password" Language="C#" MasterPageFile="~/BusBookingMaster.Master" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="BusBookingProject.ForgotPassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container auth-wrap py-4">
        <div class="card auth-card p-4 p-md-5">
            <div class="text-center mb-4">
                <span class="brand-mark mx-auto mb-3" style="width:56px;height:56px;font-size:1.5rem"><i class="fa-solid fa-key"></i></span>
                <h1 class="h3 mb-1">Reset your password</h1>
                <p class="text-body-secondary mb-0">Enter your e-mail or mobile number. We will send a code to the e-mail on the account.</p>
            </div>
            <div class="form-floating mb-3">
                <asp:TextBox ID="txtId" runat="server" CssClass="form-control" placeholder="E-mail or mobile" MaxLength="150" required="required" autocomplete="username"></asp:TextBox>
                <label for="<%= txtId.ClientID %>">E-mail or mobile number</label>
            </div>
            <asp:Button ID="btnSend" runat="server" Text="Send code" CssClass="btn btn-primary btn-lg w-100" OnClick="btnSend_Click" />
            <p class="text-center small mt-3 mb-0"><a href="Login.aspx">Back to sign in</a></p>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server"></asp:Content>
