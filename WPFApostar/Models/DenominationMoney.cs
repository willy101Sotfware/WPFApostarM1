﻿using System.ComponentModel;

namespace WPFApostar.Models
{
    public class DenominationMoney : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        private decimal _Denominacion;

        private decimal _Quantity;

        private decimal _Total;

        private string _Code;

        #endregion

        #region Properties

        public decimal Denominacion
        {
            get { return _Denominacion; }
            set
            {
                if (_Denominacion != value)
                {
                    _Denominacion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Denominacion)));
                }
            }
        }

        public decimal Quantity
        {
            get { return _Quantity; }
            set
            {
                if (_Quantity != value)
                {
                    _Quantity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Quantity)));
                }
            }
        }

        public decimal Total
        {
            get { return _Total; }
            set
            {
                if (_Total != value)
                {
                    _Total = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Total)));
                }
            }
        }

        public string Code
        {
            get { return _Code; }
            set
            {
                if (_Code != value)
                {
                    _Code = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Code)));
                }
            }
        }
        #endregion
    }
}