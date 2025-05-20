using System.Windows.Media.Imaging;

namespace WPFApostar.ViewModel
{
    public class RecaudosViewModel
    {

        public int Codigo { get; set; }

        public int CodigoSubProducto { get; set; }

        public float Iva { get;set; }

        public string Nombre { get; set; }

        public string NombreLv { get; set; }

        private BitmapImage _ImageData;
        public BitmapImage ImageData
        {
            get { return this._ImageData; }
            set { this._ImageData = value; }
        }


    }
}
