<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountReports.aspx.cs" Inherits="Skutta.AccountReporting.Web.sitecore.admin.CompanyAnalyticsReports" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Account Reports</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>Account Reports</h1>  

            <asp:Button runat="server" ID="RunAll" Text="Run All" OnClick="RunAll_Click" />
        </div>
    </form>
</body>
</html>
