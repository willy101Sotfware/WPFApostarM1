using System.Windows.Media.Imaging;

namespace WPFApostar.ViewModel
{
    internal class OperatorsViewModel
    {

        private string _Title;
        public string abreviatura
        {
            get { return this._Title; }
            set { this._Title = value; }
        }

        private BitmapImage _ImageData;
        public BitmapImage ImageData
        {
            get { return this._ImageData; }
            set { this._ImageData = value; }
        }

        private string _ImageTag;
        public string ImageTag
        {
            get { return this._ImageTag; }
            set { this._ImageTag = value; }
        }

        private string _Tag;
        public string Tag
        {
            get { return this._Tag; }
            set { this._Tag = value; }
        }

        public string IdOperator { get; set; }
        public string DesOperator { get; set; }

        public int cantidad { get; set; }  
         
        public bool ConsumeFacturacion { get; set; }

        public string idPaqueteOperador { get; set; }

        public int idPaqueteRecarga { get; set; }  
        
        public string nomPaquete { get; set; }

        public float PorcentajeComision { get; set; }

        public string tipoRecarga { get; set; }

        public string valorComercial { get; set; }

        public int Vigencia { get; set; }

        public string desPaquete { get; set; }

        private BitmapImage _ImageDataS;
        public BitmapImage ImageDataS
        {
            get { return this._ImageDataS; }
            set { this._ImageDataS = value; }
        }

        private BitmapImage _IsSelect;
        public BitmapImage IsSelect
        {
            get { return this._IsSelect; }
            set { this._IsSelect = value; }
        }

    }
}
