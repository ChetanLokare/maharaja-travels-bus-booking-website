<%@ Page Title="Change password" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="AdminChangePassword.aspx.cs" Inherits="BusBookingProject.Admin.AdminChangePassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row justify-content-center">
        <div class="col-lg-6 col-xl-5">
            <asp:Panel ID="pnlForced" runat="server" Visible="false" CssClass="alert alert-warning"><i class="fa-solid fa-triangle-exclamation me-1"></i>Please choose a new password before you continue.</asp:Panel>
            <div class="card p-4">
                <h1 class="h4 mb-3">Change admin password</h1>
                <div class="mb-3"><label class="form-label small">Current password</label><asp:TextBox ID="txtCurrent" runat="server" CssClass="form-control" TextMode="Password" MaxLength="128" required="required" autocomplete="current-password"></asp:TextBox></div>
                <div class="mb-3">
                    <label class="form-label small">New password</label>
                    <asp:TextBox ID="txtNew" runat="server" CssClass="form-control" TextMode="Password" MaxLength="128" required="required" autocomplete="new-password"></asp:TextBox>
                    <div class="mt-2"><div class="pw-meter" data-pw-meter="<%= txtNew.ClientID %>"><span></span></div><div class="small text-body-secondary mt-1" data-pw-label>&nbsp;</div></div>
                </div>
                <div class="mb-3"><label class="form-label small">Confirm new password</label><asp:TextBox ID="txtConfirm" runat="server" CssClass="form-control" TextMode="Password" MaxLength="128" required="required" autocomplete="new-password"></asp:TextBox></div>
                <asp:Button ID="btnSave" runat="server" Text="Update password" CssClass="btn btn-primary" OnClick="btnSave_Click" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server"></asp:Content>
