namespace TracePca.Dto.FIN_Statement
{
    public class SaveScheduleFormatHeadingDto
    {
            public int ASH_ID { get; set; }
            public string ASH_Name { get; set; }
            public string ASH_DELFLG { get; set; }
            public int ASH_CRBY { get; set; }
            public string ASH_STATUS { get; set; }
            public int ASH_UPDATEDBY { get; set; }
            public string ASH_IPAddress { get; set; }
            public int ASH_CompId { get; set; }
            public int ASH_YEARId { get; set; }
            public int Ash_scheduletype { get; set; }
            public int Ash_Orgtype { get; set; }
            public int ASH_Notes { get; set; }

            //ScheduleTemplateDto
            public int AST_ID { get; set; }
            public string AST_Name { get; set; }
            public int AST_HeadingID { get; set; } // This is usually filled by the caller or the method
            public int AST_SubHeadingID { get; set; }
            public int AST_ItemID { get; set; }
            public int AST_SubItemID { get; set; }
            public int AST_AccHeadId { get; set; }
            public string AST_DELFLG { get; set; }
            public int AST_CRBY { get; set; }
            public string AST_STATUS { get; set; }
            public int AST_UPDATEDBY { get; set; }
            public string AST_IPAddress { get; set; }
            public int AST_CompId { get; set; }
            public int AST_YEARId { get; set; }
            public int AST_Schedule_type { get; set; }
            public int AST_Companytype { get; set; }
            public int AST_Company_limit { get; set; }
        }
    }

