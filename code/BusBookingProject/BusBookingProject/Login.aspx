<%@ Page Title="Sign in" Language="C#" MasterPageFile="~/BusBookingMaster.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="BusBookingProject.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container auth-wrap py-4">
        <div class="card auth-card p-4 p-md-5">
            <div class="text-center mb-4">
                <span class="brand-mark mb-2" style="width:48px;height:48px;font-size:1.3rem"><i class="fa-solid fa-bus"></i></span>
                <h1 class="h3 mb-1">Welcome back</h1>
                <p class="text-body-secondary mb-0">Sign in to book and manage your trips</p>
            </div>

            <% if (GoogleEnabled || MicrosoftEnabled) { %>
            <div class="d-grid gap-2 mb-3">
                <% if (GoogleEnabled) { %><a class="btn social-btn py-2" href="OAuthStart.ashx?provider=google&amp;ReturnUrl=<%= ReturnUrlEncoded %>" data-no-overlay="1"><i class="fa-brands fa-google" style="color:#ea4335"></i>Continue with Google</a><% } %>
                <% if (MicrosoftEnabled) { %><a class="btn social-btn py-2" href="OAuthStart.ashx?provider=microsoft&amp;ReturnUrl=<%= ReturnUrlEncoded %>" data-no-overlay="1"><i class="fa-brands fa-microsoft" style="color:#00a4ef"></i>Continue with Microsoft</a><% } %>
            </div>
            <div class="divider-text mb-3">or</div>
            <% } %>

            <asp:HiddenField ID="hdnTab" runat="server" Value="pw" />
            <ul class="nav nav-pills nav-fill mb-3" role="tablist">
                <li class="nav-item" role="presentation"><button class="nav-link active" id="tab-pw" type="button" data-bs-toggle="pill" data-bs-target="#pane-pw" role="tab" data-tab="pw">Password</button></li>
                <li class="nav-item" role="presentation"><button class="nav-link" id="tab-otp" type="button" data-bs-toggle="pill" data-bs-target="#pane-otp" role="tab" data-tab="otp">One-time code</button></li>
            </ul>

            <div class="tab-content">
                <div class="tab-pane fade show active" id="pane-pw" role="tabpanel">
                    <div class="form-floating mb-3">
                        <asp:TextBox ID="txtUserId" runat="server" CssClass="form-control" placeholder="E-mail or mobile" MaxLength="150" autocomplete="username"></asp:TextBox>
                        <label for="<%= txtUserId.ClientID %>">E-mail or mobile number</label>
                    </div>
                    <div class="input-group mb-2">
                        <div class="form-floating flex-grow-1">
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Password" MaxLength="128" autocomplete="current-password"></asp:TextBox>
                            <label for="<%= txtPassword.ClientID %>">Password</label>
                        </div>
                        <button class="btn btn-outline-secondary" type="button" data-toggle-password="<%= txtPassword.ClientID %>" aria-label="Show password"><i class="fa-solid fa-eye"></i></button>
                    </div>
                    <div class="text-end mb-3"><a href="ForgotPassword.aspx" class="small">Forgot password?</a></div>
                    <asp:Button ID="btnLogin" runat="server" Text="Sign in" CssClass="btn btn-primary btn-lg w-100" OnClick="btnLogin_Click" />
                </div>

                <div class="tab-pane fade" id="pane-otp" role="tabpanel">
                    <p class="small text-body-secondary">Enter the e-mail or mobile number on your account and we will send a 6-digit code. No password needed.</p>
                    <div class="form-floating mb-3">
                        <asp:TextBox ID="txtOtpId" runat="server" CssClass="form-control" placeholder="E-mail or mobile" MaxLength="150" autocomplete="username"></asp:TextBox>
                        <label for="<%= txtOtpId.ClientID %>">E-mail or mobile number</label>
                    </div>
                    <asp:Button ID="btnSendOtp" runat="server" Text="Send code" CssClass="btn btn-primary btn-lg w-100" OnClick="btnSendOtp_Click" CausesValidation="false" UseSubmitBehavior="false" />
                </div>
            </div>

            <p class="text-center mt-4 mb-0">New here? <a href="UserRegistration.aspx" class="fw-semibold">Create an account</a></p>
            <p class="text-center small mt-3 mb-0"><a href="Admin/AdminLogin.aspx" class="text-body-secondary">Admin sign-in</a></p>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        (function () {
            var f = document.getElementById('<%= hdnTab.ClientID %>');
            document.querySelectorAll('[data-tab]').forEach(function (b) {
                b.addEventListener('shown.bs.tab', function () { f.value = b.getAttribute('data-tab'); });
            });
            if (f.value === 'otp') { var t = document.getElementById('tab-otp'); if (t && window.bootstrap) bootstrap.Tab.getOrCreateInstance(t).show(); }
        })();
    </script>
</asp:Content>
