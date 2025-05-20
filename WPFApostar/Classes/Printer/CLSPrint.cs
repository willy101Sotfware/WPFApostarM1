using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;


namespace WPFApostar.Classes.Printer
{
    public class CLSPrint
    {
        private Utilities utilities;
        private SolidBrush sb;
        private Font fTitles;
        private Font fGIBTitles;
        private Font fContent;
        private string im1;



        public CLSPrint()
        {
            utilities = new Utilities();
            sb = new SolidBrush(Color.Black);
            fTitles = new Font("Arial", 8, FontStyle.Bold);
            fGIBTitles = new Font("Arial", 12, FontStyle.Bold);
            fContent = new Font("Arial", 8, FontStyle.Regular);

            string Boucher = AdminPayPlus.DataPayPlus.PayPadConfiguration.imageS_PATH;
            im1 = Path.Combine(Boucher, "Others", "logoEPM1.png");
        }

        //______________BetPL_____________________//
        public string Cedula { get; set; }
        public string Monto { get; set; }

        public void GuardarComprobante()
        {
            try
            {
                PrintController printcc = new StandardPrintController();
                PrintDocument pd = new PrintDocument();
                pd.PrintController = printcc;
                PaperSize ps = new PaperSize("Recibo Pago", 475, 470);

                pd.PrintPage += new PrintPageEventHandler(PrintBetPlay);
                       
                pd.Print();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "CLSPrint", ex, ex.ToString());
            }
        }

        public void ImprimirComprobante()
        {
            try
            {
                PrintController printcc = new StandardPrintController();
                PrintDocument pd = new PrintDocument();
                pd.PrintController = printcc;
                PaperSize ps = new PaperSize("Recibo Pago", 475, 470);
                pd.PrinterSettings.PrintToFile = true;
                pd.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                pd.PrinterSettings.PrintFileName = String.Concat(@"C:\Facturas\Datos" + DateTime.Now + ".pdf");
                //pd.PrintPage += new PrintPageEventHandler(PrintPage);
                   
                 pd.PrintPage += new PrintPageEventHandler(PrintBetPlay);
                 GuardarComprobante();                 

                pd.Print();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "CLSPrint", ex, ex.ToString());
            }
        }

        private void PrintBetPlay(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                int y = 0;
                int sum = 30;
                int x = 150;

                g.DrawImage(Image.FromFile(im1), y += sum + 20, 0);

                g.DrawString("COMPROBANTE DE VENTA", fGIBTitles, sb, 25, y += sum);
                g.DrawString("Numero de Cedula:" + Cedula, fContent, sb, 65, y += sum);
                g.DrawString("Valor Pagado:" + Monto, fContent, sb, 50, y += sum - 10);
                g.DrawString("========================================", fContent, sb, 10, y += sum);
                g.DrawString("Toda Transaccion esta sujeta", fTitles, sb, 10, y += sum + 10);
                g.DrawString("a verificacion y aprobacion", fTitles, sb, 10, y += 20);

            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "CLSPrint", ex, ex.ToString());
            }
        }   

    }
}
