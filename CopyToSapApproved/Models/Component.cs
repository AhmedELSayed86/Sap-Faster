using CommunityToolkit.Mvvm.ComponentModel;

namespace CopyToSapApproved.Models
{
    /// <summary>
    /// قطع غيار
    /// </summary>
    public class Component : ObservableObject
    { 
        private int id;
        private string name;
        private string stor;
        private string status;
        private short quantity;
        private string oPAC;
        //public ICommand ChangeIDCommand { get; }
        //public ICommand ChangeNameCommand { get; }
        //public ICommand ChangeStorCommand { get; }
        //public ICommand ChangeStatusCommand { get; }
        //public ICommand ChangeQuantityCommand { get; }
        //public ICommand ChangeOPACCommand { get; }
        public int ID
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string Stor
        {
            get => stor;
            set => SetProperty(ref stor, value);
        }

        public string Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        public short Quantity
        {
            get => quantity;
            set => SetProperty(ref quantity, value);
        }

        /// <summary>
        /// 0010 كود
        /// </summary>
        public string OPAC
        {
            get => oPAC;
            set => SetProperty(ref oPAC, value);
        }
    }
}
