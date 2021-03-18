using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


public partial class ProjectProduct : BaseModel
{

    public int? ProductId { get; set; }
    public int? ProjectId { get; set; }

    public virtual ContentPage Product { get; set; }
    public virtual ContentPage Project { get; set; }
}

