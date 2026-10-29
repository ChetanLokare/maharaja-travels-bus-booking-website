<%@ Page Title="Admin sign-in" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="AdminLogin.aspx.cs" Inherits="BusBookingProject.Admin.AdminLogin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="auth-wrap">
        <div class="card auth-card p-4 p-md-5">
            <div class="text-center mb-4">
                <span class="brand-mark mb-2" style="width:52px;height:52px;font-size:1.4rem"><i class="fa-solid fa-user-shield"></i></span>
                <h1 class="h3 mb-1">Admin sign-in</h1>
                <p class="text-body-secondary mb-0">Authorised staff only</p>
            </div>
            <div class="form-floating mb-3">
                <asp:TextBox ID="txtUserId" runat="server" CssClass="form-control" placeholder="Admin ID" MaxLength="50" required="required" autocomplete="username"></asp:TextBox>
                <label for="<%= txtUserId.ClientID %>">Admin ID</label>
            </div>
            <div class="input-group mb-3">
                <div class="form-floating flex-grow-1">
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Password" MaxLength="128" required="required" autocomplete="current-password"></asp:TextBox>
                    <label for="<%= txtPassword.ClientID %>">Password</label>
                </div>
                <button class="btn btn-outline-secondary" type="button" data-toggle-password="<%= txtPassword.ClientID %>" aria-label="Show password"><i class="fa-solid fa-eye"></i></button>
            </div>
            <asp:Button ID="btnLogin" runat="server" Text="Sign in" CssClass="btn btn-primary btn-lg w-100" OnClick="btnLogin_Click" />
            <p class="text-center small mt-3 mb-0"><a href="../Login.aspx" class="text-body-secondary">Customer sign-in</a></p>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server"></asp:Content>
