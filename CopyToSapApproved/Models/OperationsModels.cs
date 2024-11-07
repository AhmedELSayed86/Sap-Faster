using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyToSapApproved.Models;

public class OperationsModels : ObservableObject
{
    //private int id;
    private int    opac                ;
    private string controlKy                ; 
    private string operationShortText               ; 
    private string recipient                ; 
    private string unloadingPoint           ; 

    //public ICommand ChangeIDCommand { get; }
    //public ICommand ChangeNameCommand { get; }
    //public ICommand ChangeStorCommand { get; }
    //public ICommand ChangeStatusCommand { get; }
    //public ICommand ChangeQuantityCommand { get; }
    //public ICommand ChangeOPACCommand { get; }
    //public int ID
    //{
    //    get => id;
    //    set => SetProperty(ref id , value);
    //}

    public int Opac
    {
        get => opac;
        set => SetProperty(ref opac , value);
    }

    public string ControlKy
    {
        get => controlKy;
        set => SetProperty(ref controlKy , value);
    }

    public string OperationShortText
    {
        get => operationShortText;
        set => SetProperty(ref operationShortText , value);
    }

    public string Recipient
    {
        get => recipient;
        set => SetProperty(ref recipient , value);
    }

    public string UnloadingPoint
    {
        get => unloadingPoint;
        set => SetProperty(ref unloadingPoint , value);
    }
}