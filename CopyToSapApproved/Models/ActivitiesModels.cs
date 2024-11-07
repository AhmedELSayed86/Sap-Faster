using CommunityToolkit.Mvvm.ComponentModel;

namespace CopyToSapApproved.Models;

public class ActivitiesModels : ObservableObject
{
    //private int id;
    private int no;
    private string activityText;
    private string quantityFactor;
    private string startDate;
    private string endDate;

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

    public string ActivityText
    {
        get => activityText;
        set => SetProperty(ref activityText , value);
    }

    public string QuantityFactor
    {
        get => quantityFactor;
        set => SetProperty(ref quantityFactor , value);
    }

    public string StartDate
    {
        get => startDate;
        set => SetProperty(ref startDate , value);
    }

    public string EndDate
    {
        get => endDate;
        set => SetProperty(ref endDate , value);
    }
}
