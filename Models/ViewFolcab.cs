using System;
using System.Collections.Generic;

namespace TracePca.Models;

public partial class ViewFolcab
{
    public int? FolFolid { get; set; }

    public int? FolCabinet { get; set; }

    public string? FolName { get; set; }

    public string? FolNote { get; set; }

    public int? FolCreatedBy { get; set; }

    public DateTime? FolCreatedOn { get; set; }

    public string? FolStatus { get; set; }

    public string? FolDelflag { get; set; }

    public int? CbnId { get; set; }

    public string? CbnName { get; set; }

    public int? CbnParent { get; set; }

    public string? CbnNote { get; set; }

    public int? CbnUserid { get; set; }

    public int? CbnDepartment { get; set; }

    public DateTime? CbnCreatedOn { get; set; }

    public string? CbnDelFlag { get; set; }

    public int? CbnSubCabCount { get; set; }

    public int? CbnFolderCount { get; set; }

    public string? UsrFullName { get; set; }
}
