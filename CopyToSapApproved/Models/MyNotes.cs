using CommunityToolkit.Mvvm.ComponentModel;
using DocumentFormat.OpenXml.Drawing;
using System;
using System.ComponentModel.DataAnnotations;

namespace CopyToSapApproved.Models;

internal class MyNotes : ObservableObject
{
    [Key]
    private int id;
    private string title;
    private string myNote;
    private DateTime createdAt;
    private DateTime alertTime;
    private bool alerted;
    
    public int ID
    {
        get => id;
        set => SetProperty(ref id , value);
    }

    public string Title
    {
        get => title;
        set => SetProperty(ref title , value);
    }

    public string MyNote
    {
        get => myNote;
        set => SetProperty(ref myNote , value);
    }

    public DateTime CreatedAt
    {
        get => createdAt;
        set => SetProperty(ref createdAt , value);
    }

    public DateTime AlertTime
    {
        get => alertTime;
        set => SetProperty(ref alertTime , value);
    }

    public bool Alerted
    {
        get => alerted;
        set => SetProperty(ref alerted , value);
    }
}
