<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="BusScheduleReport.aspx.cs" Inherits="BusBookingProject.Admin.BusScheduleReport" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Button runat="server" Text="Download Report PDF" ID="BusReport" CssClass="btn w-100 btn-sm btn-primary justify-content-start m-lg-auto" OnClick="BusReport_Click" Width="20%" />
    
        <br />
    <div class="table-responsive"><asp:GridView ID="gdBusSchedule" runat="server" AutoGenerateColumns="False" CssClass="table table-hover table-bordered" OnRowDataBound="gdBusSchedule_RowDataBound">
        <Columns>
            <asp:TemplateField HeaderText="Sr.No">
                <ItemTemplate>
                    <asp:Label ID="lblSrNo" runat="server"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="BusNo" HeaderText="Bus No" />
            <asp:BoundField DataField="BusName" HeaderText="Bus Name" />
            <asp:BoundField DataField="BustType" HeaderText="Bus Type" />
            <asp:BoundField DataField="Origin" HeaderText="Origin" />
            <asp:BoundField DataField="Destination" HeaderText="Destination" />
            <asp:BoundField DataField="Date" HeaderText="Travel Date" />
            <asp:BoundField DataField="TotalSeat" HeaderText="Total Seats" />
            <asp:BoundField DataField="AvailableSeats" HeaderText="Available Seats" />
            <asp:BoundField DataField="BookedSeats" HeaderText="Booked Seats" />
            <asp:BoundField DataField="Fare" HeaderText="Fare" />
            <asp:BoundField DataField="EstimatedTime" HeaderText="Estimated Time" />
            <asp:BoundField DataField="ArivalTime" HeaderText="Arrival Time" />
            <asp:BoundField DataField="DepartureTime" HeaderText="Departure Time" />
        </Columns>
    </asp:GridView></div>
</asp:Content>