using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyToSapApproved.Models;

public class ModelsModels : ObservableObject
{ 
    private string mModels;
    private string part;
    private string descriptionAR;
      
    public string MModels
    {
        get => mModels;
        set => SetProperty(ref mModels , value);
    }

    public string Part
    {
        get => part;
        set => SetProperty(ref part , value);
    }

    public string DescriptionAR
    {
        get => descriptionAR;
        set => SetProperty(ref descriptionAR , value);
    }
}