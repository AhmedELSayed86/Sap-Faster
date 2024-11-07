using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyToSapApproved.Models;

public class TasksModels : ObservableObject
{
    //private int id;
    private int no;
    private string taskText;
    private string responsible;
    private string listName;
    private string completedBy;

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

    public int No
    {
        get => no;
        set => SetProperty(ref no , value);
    }

    public string TaskText
    {
        get => taskText;
        set => SetProperty(ref taskText , value);
    }

    public string Responsible
    {
        get => responsible;
        set => SetProperty(ref responsible , value);
    }

    public string ListName
    {
        get => listName;
        set => SetProperty(ref listName , value);
    }

    public string CompletedBy
    {
        get => completedBy;
        set => SetProperty(ref completedBy , value);
    }
}