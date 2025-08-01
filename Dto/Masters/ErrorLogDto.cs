using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Text.Json.Serialization;
using TracePca.Dto.Audit;

namespace TracePca.Dto.Masters
{
 
    public class ErrorLogDto
    {
       
     
        public string CM_AccessCode { get; set; }
        public string sad_Config_Key { get; set; }
        public int sad_compid { get; set; }

    }
     
}
