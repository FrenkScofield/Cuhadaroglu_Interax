using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;


public enum SpecType : int
{
    [Description("tanım")]
    tanim = 1,
    [Description("check")]
    check = 2,
    [Description("list")]
    list = 3,
    [Description("text")]
    text = 4,
    [Description("editor")]
    editor = 5,

}
public enum TemplateType : int
{
    [Description("None")]
    None = 0,
    [Description("Normal Html")]
    NormalHtml = 1,
    [Description("Sayfa")]
    Sayfa = 2,
    [Description("Full Sayfa")]
    FullSayfa = 4,
    [Description("Dikey Sayfa")]
    DikeySayfa = 5,
    [Description("Yatay Sayfa")]
    YataySayfa = 6,
}




public enum EventType : int
{
    [Description("Counter")]
    Counter = 1,
    [Description("tip2")]
    tip2 = 2,

}




