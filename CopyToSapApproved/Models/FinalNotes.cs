using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CopyToSapApproved.Models;

public class FinalNotes : ObservableObject
{
    [Key]
    private int id;
    private int sapNo;
    private string miniNote;
    private string notes;
    private string sAPStatus;
    private string isFinished;
    private string explains;

    public int ID
    {
        get => id;
        set => SetProperty(ref id , value);
    }

    public int SapNo
    {
        get => sapNo;
        set => SetProperty(ref sapNo , value);
    }

    public string MiniNote
    {
        get => miniNote;
        set => SetProperty(ref miniNote , value);
    }

    public string Notes
    {
        get => notes;
        set => SetProperty(ref notes , value);
    }

    public string SAPStatus
    {
        get => sAPStatus;
        set => SetProperty(ref sAPStatus , value);
    }

    public string IsFinished
    {
        get => isFinished;
        set => SetProperty(ref isFinished , value);
    } 
    
    public string Explains
    {
        get => explains;
        set => SetProperty(ref explains , value);
    }
}