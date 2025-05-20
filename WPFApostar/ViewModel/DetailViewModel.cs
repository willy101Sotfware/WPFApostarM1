using WPFApostar.Classes;
using WPFApostar.Models;
using System.Windows.Data;
using System.ComponentModel;
using System.Reflection;

namespace WPFApostar.ViewModel
{
    public class DetailViewModel
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public string Title { get; set; }
        public string TasaCambio { get; set; }
        public string ImgIngresas { get; set; }
        public string ImgRetiras { get; set; }

        private List<MockupsModel> _DocumentList;

        public List<MockupsModel> DocumentList
        {
            get { return _DocumentList; }
            set
            {
                _DocumentList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DocumentList)));
            }
        }

        private CollectionViewSource _DocumentEntries;

        public CollectionViewSource DocumentEntries
        {
            get
            {
                return _DocumentEntries;
            }
            set
            {
                _DocumentEntries = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DocumentEntries)));
            }
        }

        public void LoadListDocuments(List<MockupsModel> ResponseTypeDocuments)
        {
            try
            {
                if (ResponseTypeDocuments != null && ResponseTypeDocuments.Count > 0)
                {
                    DocumentList.Clear();
                    DocumentList = ResponseTypeDocuments;
                    DocumentEntries.Source = DocumentList;

                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }

    }



}