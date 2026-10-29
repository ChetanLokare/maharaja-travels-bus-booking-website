<%@ Page Title="Choose a new password" Language="C#" MasterPageFile="~/BusBookingMaster.Master" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="BusBookingProject.ResetPassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container auth-wrap py-4">
        <div class="card auth-card p-4 p-md-5">
            <h1 class="h3 mb-3 text-center">Choose a new password</h1>
            <div class="mb-3">
                <label class="form-label small">New password</label>
                <div class="input-group">
                    <asp:TextBox ID="txtNew" runat="server" CssClass="form-control" TextMode="Password" MaxLength="128" required="required" autocomplete="new-password"></asp:TextBox>
                    <button class="btn btn-outline-secondary" type="button" data-toggle-password="<%= txtNew.ClientID %>" aria-label="Show password"><i class="fa-solid fa-eye"></i></button>
                </div>
                <div class="mt-2"><div class="pw-meter" data-pw-meter="<%= txtNew.ClientID %>"><span></span></div><div class="small text-body-secondary mt-1" data-pw-label>&nbsp;</div></div>
            </div>
            <div class="mb-3"><label class="form-label small">Confirm password</label><asp:TextBox ID="txtConfirm" runat="server" CssClass="form-control" TextMode="Password" MaxLength="128" required="required" autocomplete="new-password"></asp:TextBox></div>
            <asp:Button ID="btnSave" runat="server" Text="Update password" CssClass="btn btn-primary btn-lg w-100" OnClick="btnSave_Click" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server"></asp:Content>
