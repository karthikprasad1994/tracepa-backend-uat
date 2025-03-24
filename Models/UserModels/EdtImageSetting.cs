using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TracePca.Models.UserModels;

[Keyless]
[Table("EDT_Image_Settings")]
public partial class EdtImageSetting
{
    [Column("Img_Id")]
    public int ImgId { get; set; }

    [Column("Img_Form")]
    [StringLength(1)]
    [Unicode(false)]
    public string? ImgForm { get; set; }

    [Column("Img_ImgId")]
    public int? ImgImgId { get; set; }

    [Column("Img_Rotate")]
    public int? ImgRotate { get; set; }

    [Column("Img_Bright")]
    public int? ImgBright { get; set; }

    [Column("Img_Contrast")]
    public int? ImgContrast { get; set; }

    [Column("Img_Gamma")]
    public int? ImgGamma { get; set; }

    [Column("Img_RotateAny")]
    public int? ImgRotateAny { get; set; }
}
