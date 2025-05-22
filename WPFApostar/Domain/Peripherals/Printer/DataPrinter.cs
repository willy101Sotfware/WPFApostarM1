using System.Drawing;

namespace WPFApostar.Domain.Peripherals.Printer
{
    public class DataPrinter
    {
        public string key { get; set; }

        public string value { get; set; }

        public string image { get; set; }

        public int x { get; set; }

        public int y { get; set; }

        public Font font { get; set; }

        public SolidBrush brush { get; set; }

        public Rectangle rectangle { get; set; }

        public PointF point { get; set; }

        public StringFormat direction { get; set; }
    }
}
