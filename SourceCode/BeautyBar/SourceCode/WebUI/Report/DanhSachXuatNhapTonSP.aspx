<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DanhSachXuatNhapTonSP.aspx.cs" Inherits="WebUI.Report.DanhSachXuatNhapTonSP" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        
    </div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" BackColor="White" BorderColor="White" AsyncRendering="False" ZoomMode="PageWidth" Width="600px" Height="1600px" ZoomPercent="100" ShowPrintButton="True" SizeToReportContent="False">
        </rsweb:ReportViewer>
       
    </form>
</body>
</html>
