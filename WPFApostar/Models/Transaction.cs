﻿using System.ComponentModel;
using System.Windows.Media.Imaging;
using WPFApostar.Classes;
using WPFApostar.DataModel;
using WPFApostar.Services.ObjectIntegration;
using WPFApostar.ViewModel;

namespace WPFApostar.Models
{

    public class Transaction : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        public string tramite { get; set; }

        public ETypeTramites eTypeTramites { get; set; }
        public string Document { get; set; }

        public string TypeDocument { get; set; }

        public string Fecha { get; set; }

        public bool DevueltaCorrecta { get; set; }

        public string Name { get; set; }    

        public int ValorGanar { get; set; }

        public float IVA { get; set; }

        public float Valor { get; set; }

        public double TransaccionChance { get; set; }

        public int ExtraMum { get; set; }

        public ResponseGetLotteries LotteryList { get; set; }

        public bool StateReturnMoney { get; set; }

        public int consecutivo { get; set; } = 800700625;
        public string  TransaccionRecaudo { get; set; }

        public string valor { get; set; }

        public int StateNotification { get; set; }

        public ProductSelect productSelect { get; set; }

        public bool statePaySuccess { get; set; }

        public string Observation { get; set; }

        public DateTime DateTransaction { get; set; }

        public string reference { get; set; }
        public ETransactionType Tipo { get; set; }

        public ETypeTramites Type { get; set; }

        public string StatePay { get; set; }

        public string NumeroLoteria { get; set; }

        public string NumeroDerecho { get; set; }

        public string NumeroCombinado { get; set; }

        public string nit { get; set; }

        public string calificacion { get; set; }

        public string SerieTerminal { get; set; }

        public int CodigoVendedor { get; set; }

        public int codigoPuntoVenta { get; set; }

        public PaymentViewModel Payment { get; set; }

       public List<LoteriaLiquidar> LoteriaLiquidar { get; set; } = new List<LoteriaLiquidar>();

        public List<NumeroChance> Chances { get; set; } = new List<NumeroChance>();
        public List<LoteriaChance> LoteriaChance { get; set; } = new List<LoteriaChance>();

        public List<Listadotipochancefield> TypeChance { get; set; }

        public List<ApuestaValidate> ApuestaValidate { get; set; }


        public ResponseConsultSubproductosPaquetes responseConsultSubproductosPaquetes { get; set; }
        public ResponseConsultPaquetes responseConsultPaquetes { get; set; }

        public ResponseGuardarPaquetes responseGuardarPaquetes { get; set; }

        public OperatorSelected SelectOperator { get; set; }

        public string Description { get; set; }

        public string NumOperator { get; set; }

        public string Operador { get; set; }

        public string Token { get; set; }

        public string IdProduct { get; set; }

        public PAYER payer { get; set; }

        public ETransactionState State { get; set; }

        public string Amount { get; set; }
        public decimal RealAmount { get; set; }

        //Data Chance

        public SubProductoGeneral ProductSelected { get; set; }
        public List<LotteriesViewModel> ListaLoteriasSeleccionadas { get; set; }
        public List<Chance> ListaChances { get; set; }

        public ResponseNotifyChance ResponseNotifyC { get; set; }
        public bool Editar { get; set; } = false;
        public int IndexChanceToEdit { get; set; }

        public int IdUser { get; set; }

        //Data BetPlay

        public ResponseTokenBetplay ResponseTokenBetplay { get; set; }
        public ulong Transacciondistribuidorid { get; set; }
        public ResponseNotifyBetPlay ResponseNotifyBetPlay { get; set; }

        //Data Recaudo
        public List<Listadorecaudosfield> listadorecaudosField { get; set; }

        public DataCompany Company { get; set; }

        public int IdServicioRecaudo { get; set; }

        public long CodigoSeguridad { get; set; }

        public List<Listadocamposfield> ParametersCompany { get; set; }

        public RecaudofieldData DataReference { get; set; }

        public List<infoNotifyOrder> infoNotify { get; set; }

        public ResponseNotifyPayment ResponseNotifyPayment { get;set; }

        //End Data Recaudo


        private int _totalChance;

        public int TotalChance
        {
            get
            {
                return _totalChance;
            }
            set
            {
                _totalChance = value;
                OnPropertyRaised("TotalChance");
            }
        }

        private int _transactionId { get; set; }

        public int TransactionId
        {
            get
            {
                return _transactionId;
            }
            set
            {
                _transactionId = value;
                OnPropertyRaised("TransactionId");
            }
        }

        private int _consecutivoId { get; set; }

        public int ConsecutivoId
        {
            get
            {
                return _consecutivoId;
            }
            set
            {
                _consecutivoId = value;
                OnPropertyRaised("ConsecutivoId");
            }
        }

        private int _idTransactionAPi { get; set; }

        public int IdTransactionAPi
        {
            get
            {
                return _idTransactionAPi;
            }
            set
            {
                _idTransactionAPi = value;
                OnPropertyRaised("IdTransactionAPi");
            }
        }


    }

    //Chance

    public class Chance
    {
        public List<LotteriesViewModel> Loterias { get; set; }
        public DateTime FechaJuego { get; set; }

        // Valores de la apuesta
        public int Directo { get; set; }
        public int Combinado { get; set; }
        public int Pata { get; set; }
        public int Una { get; set; }

        public int TipoChance { get; set; }

        //Número jugado
        public string Numero { get; set; }

        //Total
        private int _Total { get; set; }

        public int GetTotalChance()
        {
            _Total = (Directo + Combinado + Pata + Una) * Loterias.Count;
            return _Total;
        }
    }

    public class LotteriesViewModel
    {

        private string _Title;
        public string Title
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

        private string _CodigoCodesa;
        public string CodigoCodesa
        {
            get { return this._CodigoCodesa; }
            set { this._CodigoCodesa = value; }
        }

        public string IdCodesa { get; set; }
        public string DescripcionLoteria { get; set; }
        public string Nombre { get; set; }
        public string NombreCorto { get; set; }




        private BitmapImage _ImageS;
        public BitmapImage ImageS
        {
            get { return this._ImageS; }
            set { this._ImageS = value; }
        }

        private BitmapImage _Image;
        public BitmapImage Image
        {
            get { return this._Image; }
            set { this._Image = value; }
        }

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return this._IsSelected; }
            set
            {
                this._IsSelected = value;
                if (_IsSelected) this.ImageData = ImageS;
                else this.ImageData = Image;
            }
        }
    }

    //BetPlay Rec


    public class ProductSelect
    {
        public int codigoField { get; set; }
        public string nombreField { get; set; }
        public int idField { get; set; }

        public int CodigoServicio { get; set; }
    }


    //Recaudos

    public class infoNotifyOrder
    {
        public int id { get; set; }

        public string Nombre { get; set; }

        public string valor { get; set; }

    }
    //Recargas

    public class OperatorSelected
    {
        public float porcentajeComision { get; set; }
        public string idOperador { get; set; }
        public string desOperador { get; set; }
        public string tipoRecarga { get; set; }
        public int idPaqueteRecarga { get; set; }
        public string idPaqueteOperador { get; set; }
        public string nomPaquete { get; set; }
        public string desPaquete { get; set; }
        public string nomCorto { get; set; }

        public double valorComercial { get; set; }
        public int vigencia { get; set; }
        public int cantidad { get; set; }
        public bool consumeFacturacion { get; set; }
    }

    //Otros

    public class ResponseNotifyT
    {
        public bool ok { get; set; }
        public string msgError { get; set; }
        public int errorCode { get; set; }
        public string token { get; set; }
        public ModelNotify model { get; set; }
    }

    public class ModelNotify
    {
        public long cedulaApostador { get; set; }
        public long montoRecargado { get; set; }
        public long saldoCuenta { get; set; }
        public string numeroSeguridad { get; set; }

    }

    public class ResponseFechaT
    {
        public bool ok { get; set; }
        public string token { get; set; }
        public ModelFecha model { get; set; }
    }

    public class ModelFecha
    {
        public ListFecha[] list { get; set; }
    }

    public class ListFecha
    {
        public int idLoteria { get; set; }
        public int sorteo { get; set; }
        public string abreviatura { get; set; }
        public string desLoteria { get; set; }
        public string horaSorteo { get; set; }
        public int numCifras { get; set; }
        public string esExcluyente { get; set; }
        public DateTime fecSorteo { get; set; }
        public string nacional { get; set; }
    }

    //public class NumeroLiquidarT
    //{
    //    public string derecho { get; set; }
    //}

    //public class LoteriaLiquidarT
    //{
    //    public string idLoteria { get; set; }
    //    public string sorteo { get; set; } 
    //}


    //SuperChance

    public class TransactionChance : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

    }

    //Recaudo

    public class DataCompany
    {
        public int Codigo { get; set; }

        public int CodigoSubProducto { get; set; }

        public float Iva { get; set; }

        public string Nombre { get; set; }

        private BitmapImage _ImageData;
        public BitmapImage ImageData
        {
            get { return this._ImageData; }
            set { this._ImageData = value; }
        }
    }
}
