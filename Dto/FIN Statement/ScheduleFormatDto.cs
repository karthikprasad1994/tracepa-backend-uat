namespace TracePca.Dto.FIN_Statement
{
    public class ScheduleFormatDto
    {

        //GetScheduleHeading
        public class ScheduleHeadingDto
        {
            public int HeadingId { get; set; }
            public string HeadingName { get; set; }
        }

        //GetScheduleSubHeading
        public class ScheduleSubHeadingDto
        {
            public int SubheadingID { get; set; }
            public string SubheadingName { get; set; }
        }

        //GetScheduleItem
        public class ScheduleItemDto
        {
            public int ItemsId { get; set; }
            public string ItemsName { get; set; }
        }

        //GetScheduleSubItem
        public class ScheduleSubItemDto
        {
            public int SubitemsId { get; set; }
            public string SubitemsName { get; set; }
        }

        //ScheduleTemplate
        public class ScheduleFormatTemplateDto
        {
            public int AST_ID { get; set; }
            public int? HeadingId { get; set; }
            public string HeadingName { get; set; }
            public int? SubheadingID { get; set; }
            public string SubheadingName { get; set; }
            public int? ItemId { get; set; }
            public string ItemName { get; set; }
            public int? SubitemId { get; set; }
            public string SubitemName { get; set; }
            public int AST_AccHeadId { get; set; }
            public string AccHeadName { get; set; }
        }

        //DeleteScheduleTemplate(Grid)
        public class DeleteScheduleTemplateDto
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }
        }

        //SaveScheduleHeading
        public class SaveScheduleHeadingDto
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

        //SaveScheduleSubHeading
        public class SaveScheduleSubHeadingDto
        {
            public int ASSH_ID { get; set; }
            public string ASSH_Name { get; set; }
            public int ASSH_HeadingID { get; set; }
            public string ASSH_DELFLG { get; set; }
            public int ASSH_CRBY { get; set; }
            public string ASSH_STATUS { get; set; }
            public int ASSH_UPDATEDBY { get; set; }
            public string ASSH_IPAddress { get; set; }
            public int ASSH_CompId { get; set; }
            public int ASSH_YEARId { get; set; }
            public string ASSH_Notes { get; set; }
            public string ASSH_scheduletype { get; set; }
            public string ASSH_Orgtype { get; set; }

            // --- Schedule Template ---
            public int AST_ID { get; set; }
            public string AST_Name { get; set; }
            public int AST_HeadingID { get; set; }     // FK to Heading
            public int AST_SubHeadingID { get; set; }  // FK to SubHeading
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

        //SaveScheduleItem
        public class SaveScheduleItemDto
        {
            // Item Fields
            public int ASI_ID { get; set; }
            public string? ASI_Name { get; set; }
            public int ASI_HeadingID { get; set; }
            public int ASI_SubHeadingID { get; set; }
            public string? ASI_DELFLG { get; set; }
            public int ASI_CRBY { get; set; }
            public string? ASI_STATUS { get; set; }
            public string? ASI_IPAddress { get; set; }
            public int ASI_CompId { get; set; }
            public int ASI_YEARId { get; set; }
            public int ASI_scheduletype { get; set; }
            public int ASI_Orgtype { get; set; }

            // Template Fields
            public int AST_ID { get; set; }
            public string? AST_Name { get; set; }
            public int AST_HeadingID { get; set; }  // Can also use ASSH_ID here
            public int AST_SubHeadingID { get; set; }
            public int AST_ItemID { get; set; }
            public int AST_SubItemID { get; set; }
            public int AST_AccHeadId { get; set; }
            public string? AST_DELFLG { get; set; }
            public int AST_CRBY { get; set; }
            public string? AST_STATUS { get; set; }
            public int AST_UPDATEDBY { get; set; }
            public string? AST_IPAddress { get; set; }
            public int AST_CompId { get; set; }
            public int AST_YEARId { get; set; }
            public int AST_Schedule_type { get; set; }
            public int AST_Companytype { get; set; }
            public int AST_Company_limit { get; set; }
        }

        //SaveScheduleSubItem
        public class SaveScheduleSubItemDto
        {
            public int ASSI_ID { get; set; }
            public string? ASSI_Name { get; set; }
            public int ASSI_HeadingID { get; set; }
            public int ASSI_SubHeadingID { get; set; }
            public int ASSI_ItemsID { get; set; }
            public string? ASSI_DELFLG { get; set; }
            public int ASSI_CRBY { get; set; }
            public string? ASSI_STATUS { get; set; }
            public int ASSI_UPDATEDBY { get; set; }
            public string? ASSI_IPAddress { get; set; }
            public int ASSI_CompId { get; set; }
            public int ASSI_YEARId { get; set; }
            public string? ASSI_ScheduleType { get; set; }
            public string? ASSI_OrgType { get; set; }

            // Template properties
            public int AST_ID { get; set; }
            public string? AST_Name { get; set; }
            public int AST_HeadingID { get; set; }
            public int AST_SubHeadingID { get; set; }
            public int AST_ItemID { get; set; }
            public int AST_SubItemID { get; set; }
            public int AST_AccHeadId { get; set; }
            public string? AST_DELFLG { get; set; }
            public int AST_CRBY { get; set; }
            public string? AST_STATUS { get; set; }
            public int AST_UPDATEDBY { get; set; }
            public string? AST_IPAddress { get; set; }
            public int AST_CompId { get; set; }
            public int AST_YEARId { get; set; }
            public int AST_Schedule_type { get; set; }
            public int AST_Companytype { get; set; }
            public int AST_Company_limit { get; set; }
        }

        //SaveOrUpdateScheduleHeadingAlias
        public class ScheduleHeadingAliasDto
        {
            public int AGA_ID { get; set; }
            public string AGA_Description { get; set; }
            public int AGA_GLID { get; set; }
            public string AGA_GLDESC { get; set; }
            public int AGA_GrpLevel { get; set; }
            public int AGA_scheduletype { get; set; }
            public int AGA_Orgtype { get; set; }
            public int AGA_Compid { get; set; }
            public string AGA_Status { get; set; }
            public int AGA_Createdby { get; set; }
            public string AGA_IPAddress { get; set; }
        }

        //GetScheduleTemplateCount
        public class ScheduleTemplateCountDto
        {
            public int TemplateCount { get; set; }
        }

        //SaveScheduleTemplate(P and L)
        public class ScheduleTemplatePandLDto
        {
            // === Schedule Heading ===
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

            // === Schedule SubHeading ===
            public int ASSH_ID { get; set; }
            public string ASSH_Name { get; set; }
            public int ASSH_HeadingID { get; set; }
            public string ASSH_DELFLG { get; set; }
            public int ASSH_CRBY { get; set; }
            public string ASSH_STATUS { get; set; }
            public int ASSH_UPDATEDBY { get; set; }
            public string ASSH_IPAddress { get; set; }
            public int ASSH_CompId { get; set; }
            public int ASSH_YEARId { get; set; }
            public int ASSH_Notes { get; set; }
            public int ASSH_scheduletype { get; set; }
            public int ASSH_Orgtype { get; set; }

            // === Schedule Item ===
            public int ASI_ID { get; set; }
            public string? ASI_Name { get; set; }
            public int ASI_HeadingID { get; set; }
            public int ASI_SubHeadingID { get; set; }
            public string? ASI_DELFLG { get; set; }
            public int ASI_CRBY { get; set; }
            public string? ASI_STATUS { get; set; }
            public string? ASI_IPAddress { get; set; }
            public int ASI_CompId { get; set; }
            public int ASI_YEARId { get; set; }
            public int ASI_scheduletype { get; set; }
            public int ASI_Orgtype { get; set; }

            // === Schedule SubItem ===
            public int ASSI_ID { get; set; }
            public string? ASSI_Name { get; set; }
            public int ASSI_HeadingID { get; set; }
            public int ASSI_SubHeadingID { get; set; }
            public int ASSI_ItemsID { get; set; }
            public string? ASSI_DELFLG { get; set; }
            public int ASSI_CRBY { get; set; }
            public string? ASSI_STATUS { get; set; }
            public int ASSI_UPDATEDBY { get; set; }
            public string? ASSI_IPAddress { get; set; }
            public int ASSI_CompId { get; set; }
            public int ASSI_YEARId { get; set; }
            public int ASSI_ScheduleType { get; set; }
            public int ASSI_OrgType { get; set; }

            // === Schedule Template ===
            public int AST_ID { get; set; }
            public string? AST_Name { get; set; }
            public int AST_HeadingID { get; set; }
            public int AST_SubHeadingID { get; set; }
            public int AST_ItemID { get; set; }
            public int AST_SubItemID { get; set; }
            public int AST_AccHeadId { get; set; }
            public string? AST_DELFLG { get; set; }
            public int AST_CRBY { get; set; }
            public string? AST_STATUS { get; set; }
            public int AST_UPDATEDBY { get; set; }
            public string? AST_IPAddress { get; set; }
            public int AST_CompId { get; set; }
            public int AST_YEARId { get; set; }
            public int AST_Schedule_type { get; set; }
            public int AST_Companytype { get; set; }
            public int AST_Company_limit { get; set; }

            // === Extra / Derived Property ===
            public string AST_AccountHeadName { get; set; }
        }

        //SaveScheduleTemplate
        public class ScheduleTemplate
        {
            // === Schedule Heading ===
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

            // === Schedule SubHeading ===
            public int ASSH_ID { get; set; }
            public string ASSH_Name { get; set; }
            public int ASSH_HeadingID { get; set; }
            public string ASSH_DELFLG { get; set; }
            public int ASSH_CRBY { get; set; }
            public string ASSH_STATUS { get; set; }
            public int ASSH_UPDATEDBY { get; set; }
            public string ASSH_IPAddress { get; set; }
            public int ASSH_CompId { get; set; }
            public int ASSH_YEARId { get; set; }
            public int ASSH_Notes { get; set; }
            public int ASSH_scheduletype { get; set; }
            public int ASSH_Orgtype { get; set; }

            // === Schedule Item ===
            public int ASI_ID { get; set; }
            public string? ASI_Name { get; set; }
            public int ASI_HeadingID { get; set; }
            public int ASI_SubHeadingID { get; set; }
            public string? ASI_DELFLG { get; set; }
            public int ASI_CRBY { get; set; }
            public string? ASI_STATUS { get; set; }
            public string? ASI_IPAddress { get; set; }
            public int ASI_CompId { get; set; }
            public int ASI_YEARId { get; set; }
            public int ASI_scheduletype { get; set; }
            public int ASI_Orgtype { get; set; }

            // === Schedule SubItem ===
            public int ASSI_ID { get; set; }
            public string? ASSI_Name { get; set; }
            public int ASSI_HeadingID { get; set; }
            public int ASSI_SubHeadingID { get; set; }
            public int ASSI_ItemsID { get; set; }
            public string? ASSI_DELFLG { get; set; }
            public int ASSI_CRBY { get; set; }
            public string? ASSI_STATUS { get; set; }
            public int ASSI_UPDATEDBY { get; set; }
            public string? ASSI_IPAddress { get; set; }
            public int ASSI_CompId { get; set; }
            public int ASSI_YEARId { get; set; }
            public int ASSI_ScheduleType { get; set; }
            public int ASSI_OrgType { get; set; }

            // === Schedule Template ===
            public int AST_ID { get; set; }
            public string? AST_Name { get; set; }
            public int AST_HeadingID { get; set; }
            public int AST_SubHeadingID { get; set; }
            public int AST_ItemID { get; set; }
            public int AST_SubItemID { get; set; }
            public int AST_AccHeadId { get; set; }
            public string? AST_DELFLG { get; set; }
            public int AST_CRBY { get; set; }
            public string? AST_STATUS { get; set; }
            public int AST_UPDATEDBY { get; set; }
            public string? AST_IPAddress { get; set; }
            public int AST_CompId { get; set; }
            public int AST_YEARId { get; set; }
            public int AST_Schedule_type { get; set; }
            public int AST_Companytype { get; set; }
            public int AST_Company_limit { get; set; }

            // === Extra / Derived Property ===
            public string AST_AccountHeadName { get; set; }
        }
    }
}
