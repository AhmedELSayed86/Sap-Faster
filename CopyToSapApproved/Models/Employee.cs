#nullable enable

using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CopyToSapApproved.Models;

/// <summary>
/// نموذج يمثل موظف
/// </summary>
public class Employee : ObservableObject
{
    [Key]
    private int code;
    private string? name;
    private string? job;
    private string? branch;
    private string? workLocation;
    private string? department;
    private string? vendor;

    public int Code
    {
        get => code;
        set => SetProperty(ref code , value);
    }

    public string? Name
    {
        get => name;
        set => SetProperty(ref name , value);
    }

    public string? Job
    {
        get => job;
        set => SetProperty(ref job , value);
    }

    public string? Branch
    {
        get => branch;
        set => SetProperty(ref branch , value);
    }

    public string? WorkLocation
    {
        get => workLocation;
        set => SetProperty(ref workLocation , value);
    }

    public string? Department
    {
        get => department;
        set => SetProperty(ref department , value);
    }

    public string? Vendor
    {
        get => vendor;
        set => SetProperty(ref vendor , value);
    }
}
