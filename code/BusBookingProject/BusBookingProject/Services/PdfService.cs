using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace BusBookingProject.Services
{
    /// <summary>E-ticket and payment receipt PDFs, built with the iTextSharp library already shipped in /bin.</summary>
    public static class PdfService
    {
        private static readonly BaseColor Brand = new BaseColor(67, 56, 202);
        private static readonly BaseColor Soft = new BaseColor(243, 244, 251);
        private static readonly BaseColor Muted = new BaseColor(107, 114, 128);
        private static readonly BaseColor Dark = new BaseColor(17, 24, 39);

        private static string Money(decimal v) { return "\u20B9" + v.ToString("N2"); }

        private static BaseFont LoadFont(bool bold, out bool hasRupee)
        {
            hasRupee = true;
            try
            {
                string dir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
                string file = Path.Combine(dir, bold ? "segoeuib.ttf" : "segoeui.ttf");
                if (File.Exists(file)) return BaseFont.CreateFont(file, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            }
            catch { }
            hasRupee = false;
            return BaseFont.CreateFont(bold ? BaseFont.HELVETICA_BOLD : BaseFont.HELVETICA, BaseFont.WINANSI, false);
        }

        private class Fonts
        {
            public Font Title, H, Body, Small, Big, White, WhiteBig;
            public bool Rupee;
        }

        private static Fonts MakeFonts()
        {
            bool r1, r2;
            BaseFont reg = LoadFont(false, out r1);
            BaseFont bold = LoadFont(true, out r2);
            return new Fonts
            {
                Rupee = r1 && r2,
                Title = new Font(bold, 20, Font.NORMAL, BaseColor.WHITE),
                H = new Font(bold, 11, Font.NORMAL, Brand),
                Body = new Font(reg, 10, Font.NORMAL, Dark),
                Small = new Font(reg, 8.5f, Font.NORMAL, Muted),
                Big = new Font(bold, 16, Font.NORMAL, Dark),
                White = new Font(reg, 10, Font.NORMAL, BaseColor.WHITE),
                WhiteBig = new Font(bold, 24, Font.NORMAL, BaseColor.WHITE)
            };
        }

        private static string Amt(Fonts f, decimal v) { return f.Rupee ? Money(v) : "Rs. " + v.ToString("N2"); }

        private static PdfPCell Cell(string text, Font font, BaseColor bg = null, int align = Element.ALIGN_LEFT, float pad = 6)
        {
            var c = new PdfPCell(new Phrase(text ?? "", font)) { Border = iTextSharp.text.Rectangle.NO_BORDER, Padding = pad, HorizontalAlignment = align };
            if (bg != null) c.BackgroundColor = bg;
            return c;
        }

        private static void Header(Document doc, Fonts f, string title, string sub)
        {
            var t = new PdfPTable(1) { WidthPercentage = 100 };
            var c = new PdfPCell { Border = iTextSharp.text.Rectangle.NO_BORDER, BackgroundColor = Brand, Padding = 16 };
            c.AddElement(new Paragraph("Maharaja Travel's", f.Title));
            c.AddElement(new Paragraph(title + (string.IsNullOrEmpty(sub) ? "" : "   |   " + sub), f.White));
            t.AddCell(c);
            doc.Add(t);
            doc.Add(new Paragraph(" ", f.Small));
        }

        private static PdfPTable KeyValue(Fonts f, params string[] kv)
        {
            var t = new PdfPTable(2) { WidthPercentage = 100 };
            t.SetWidths(new float[] { 32, 68 });
            for (int i = 0; i + 1 < kv.Length; i += 2)
            {
                t.AddCell(Cell(kv[i], f.Small, null, Element.ALIGN_LEFT, 4));
                t.AddCell(Cell(kv[i + 1], f.Body, null, Element.ALIGN_LEFT, 4));
            }
            return t;
        }

        // ------------------------------------------------------------------ ticket
        public static byte[] BuildTicket(BookingDetail b)
        {
            Fonts f = MakeFonts();
            using (var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A4, 36, 36, 36, 36);
                PdfWriter.GetInstance(doc, ms);
                doc.AddTitle("E-Ticket " + b.Pnr);
                doc.AddAuthor("Maharaja Travel's");
                doc.Open();

                Header(doc, f, "E-TICKET", "PNR " + b.Pnr);

                // route band
                var route = new PdfPTable(3) { WidthPercentage = 100 };
                route.SetWidths(new float[] { 42, 16, 42 });
                route.AddCell(Cell(b.Origin, f.Big, Soft, Element.ALIGN_LEFT, 12));
                route.AddCell(Cell("\u2192", f.Big, Soft, Element.ALIGN_CENTER, 12));
                route.AddCell(Cell(b.Destination, f.Big, Soft, Element.ALIGN_RIGHT, 12));
                doc.Add(route);
                doc.Add(new Paragraph(" ", f.Small));

                // details + QR
                var top = new PdfPTable(2) { WidthPercentage = 100 };
                top.SetWidths(new float[] { 74, 26 });
                var left = new PdfPCell { Border = iTextSharp.text.Rectangle.NO_BORDER };
                left.AddElement(KeyValue(f,
                    "Travel date", b.TravelDate,
                    "Departure", b.DepartureTime,
                    "Arrival (est.)", b.ArrivalTime,
                    "Bus", b.BusName + " (" + b.BusNo + ")  " + b.BusType,
                    "Boarding point", b.BoardingPlace + " at " + b.BoardingTime,
                    "Status", b.Status.ToUpperInvariant()));
                top.AddCell(left);

                var qrCell = new PdfPCell { Border = iTextSharp.text.Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
                var qr = new BarcodeQRCode("MBT|" + b.Pnr + "|" + b.Origin + "-" + b.Destination + "|" + b.TravelDate, 140, 140, null).GetImage();
                qr.ScaleAbsolute(100, 100);
                qrCell.AddElement(qr);
                qrCell.AddElement(new Paragraph("Show at boarding", f.Small) { Alignment = Element.ALIGN_CENTER });
                top.AddCell(qrCell);
                doc.Add(top);

                // passengers
                doc.Add(new Paragraph("Passengers", f.H) { SpacingBefore = 10, SpacingAfter = 4 });
                var pax = new PdfPTable(4) { WidthPercentage = 100 };
                pax.SetWidths(new float[] { 8, 52, 20, 20 });
                foreach (string h in new[] { "#", "Name", "Age / Gender", "Seat" })
                    pax.AddCell(Cell(h, f.White, Brand, Element.ALIGN_LEFT, 6));
                int n = 1;
                foreach (var p in b.Passengers)
                {
                    BaseColor bg = n % 2 == 0 ? Soft : null;
                    pax.AddCell(Cell(n.ToString(), f.Body, bg));
                    pax.AddCell(Cell(p.FullName, f.Body, bg));
                    pax.AddCell(Cell((p.Age.HasValue ? p.Age.ToString() : "-") + " / " + (string.IsNullOrEmpty(p.Gender) ? "-" : p.Gender), f.Body, bg));
                    pax.AddCell(Cell(p.SeatNo, f.Body, bg));
                    n++;
                }
                doc.Add(pax);

                doc.Add(new Paragraph("Total paid: " + Amt(f, b.TotalAmount), f.Big) { SpacingBefore = 12, Alignment = Element.ALIGN_RIGHT });

                doc.Add(new Paragraph("Important", f.H) { SpacingBefore = 14, SpacingAfter = 3 });
                foreach (string line in new[]
                {
                    "Carry a valid government photo ID for every passenger.",
                    "Reach the boarding point at least 15 minutes before departure.",
                    "This e-ticket (printed or on your phone) is valid together with a photo ID."
                })
                    doc.Add(new Paragraph("\u2022  " + line, f.Small));

                doc.Close();
                return ms.ToArray();
            }
        }

        // ------------------------------------------------------------------ receipt
        public static byte[] BuildReceipt(BookingDetail b)
        {
            Fonts f = MakeFonts();
            using (var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A4, 36, 36, 36, 36);
                PdfWriter.GetInstance(doc, ms);
                doc.AddTitle("Payment receipt " + b.Pnr);
                doc.AddAuthor("Maharaja Travel's");
                doc.Open();

                Header(doc, f, "PAYMENT RECEIPT", "Ref " + b.Pnr);

                var paid = new PdfPTable(1) { WidthPercentage = 100 };
                var pc = new PdfPCell { Border = iTextSharp.text.Rectangle.NO_BORDER, BackgroundColor = new BaseColor(22, 163, 74), Padding = 14 };
                pc.AddElement(new Paragraph("PAID", f.White));
                pc.AddElement(new Paragraph(Amt(f, b.TotalAmount), f.WhiteBig));
                paid.AddCell(pc);
                doc.Add(paid);

                doc.Add(new Paragraph("Payment details", f.H) { SpacingBefore = 14, SpacingAfter = 4 });
                doc.Add(KeyValue(f,
                    "Receipt for PNR", b.Pnr,
                    "Payment date", (b.PaidAt ?? b.ConfirmedAt ?? DateTime.Now).ToString("dd MMM yyyy, hh:mm tt"),
                    "Payment method", string.IsNullOrEmpty(b.PaymentMethod) ? "Online" : b.PaymentMethod.ToUpperInvariant(),
                    "Gateway", "Razorpay",
                    "Transaction ID", string.IsNullOrEmpty(b.PaymentId) ? "-" : b.PaymentId,
                    "Order ID", string.IsNullOrEmpty(b.OrderId) ? "-" : b.OrderId,
                    "Billed to", b.ContactEmail));

                doc.Add(new Paragraph("Journey", f.H) { SpacingBefore = 14, SpacingAfter = 4 });
                doc.Add(KeyValue(f,
                    "Route", b.Origin + " to " + b.Destination,
                    "Travel date", b.TravelDate + ", departs " + b.DepartureTime,
                    "Bus", b.BusName + " (" + b.BusNo + ")"));

                doc.Add(new Paragraph("Fare breakup", f.H) { SpacingBefore = 14, SpacingAfter = 4 });
                var t = new PdfPTable(3) { WidthPercentage = 100 };
                t.SetWidths(new float[] { 56, 14, 30 });
                foreach (string h in new[] { "Passenger / seat", "Qty", "Amount" })
                    t.AddCell(Cell(h, f.White, Brand));
                foreach (var p in b.Passengers)
                {
                    t.AddCell(Cell(p.FullName + "  (seat " + p.SeatNo + ")", f.Body));
                    t.AddCell(Cell("1", f.Body));
                    t.AddCell(Cell(Amt(f, p.Fare), f.Body, null, Element.ALIGN_RIGHT));
                }
                t.AddCell(Cell("Total", f.H, Soft));
                t.AddCell(Cell(b.TotalTickets.ToString(), f.H, Soft));
                t.AddCell(Cell(Amt(f, b.TotalAmount), f.H, Soft, Element.ALIGN_RIGHT));
                doc.Add(t);

                doc.Add(new Paragraph("This is a computer-generated receipt and does not require a signature.", f.Small) { SpacingBefore = 18 });
                doc.Close();
                return ms.ToArray();
            }
        }
    }
}
