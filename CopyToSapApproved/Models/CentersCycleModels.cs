using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyToSapApproved.Models;

public class CentersCycleModels : ObservableObject
{
    private int id;
    private string shortText;   
    private string quantity;
   
    //public ICommand ChangeIDCommand { get; }
    //public ICommand ChangeNameCommand { get; }
    //public ICommand ChangeStorCommand { get; }
    //public ICommand ChangeStatusCommand { get; }
    //public ICommand ChangeQuantityCommand { get; }
    //public ICommand ChangeOPACCommand { get; }
  
    public int ID
    {
        get => id;
        set => SetProperty(ref id , value);
    }

    public string ShortText
    {
        get => shortText;
        set => SetProperty(ref shortText , value);
    } 
}