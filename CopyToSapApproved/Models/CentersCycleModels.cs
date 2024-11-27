using CommunityToolkit.Mvvm.ComponentModel;

namespace CopyToSapApproved.Models;

public class CentersCycleModels : ObservableObject
{
    private int id;
    private string shortText;
   
    //public ICommand ChangeIDCommand { get; }
  
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