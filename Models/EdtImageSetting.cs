using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class EdtImageSetting
{
    public int ImgId { get; set; }

    public string? ImgForm { get; set; }

    public int? ImgImgId { get; set; }

    public int? ImgRotate { get; set; }

    public int? ImgBright { get; set; }

    public int? ImgContrast { get; set; }

    public int? ImgGamma { get; set; }

    public int? ImgRotateAny { get; set; }
}
