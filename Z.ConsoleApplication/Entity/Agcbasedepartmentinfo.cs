using System;
using System.Collections.Generic; 
using System.Linq;  
using System.Text;
using Easy4net.CustomAttributes;

namespace Entity  
{  
	 [Table(Name = "AGC_BASEDEPARTMENTINFO")] 
	 public class Agcbasedepartmentinfo
	 { 
		[Column(Name = "STATUS")]
		public Decimal? Status{ get; set; } 

		[Column(Name = "CREATEUSER")]
		public string Createuser{ get; set; } 

		[Column(Name = "CREATEDATE")]
		public DateTime? Createdate{ get; set; } 

		[Column(Name = "UPDATEUSER")]
		public string Updateuser{ get; set; } 

		[Column(Name = "UPDATEDATE")]
		public DateTime? Updatedate{ get; set; } 

		[Id(Name = "ID", Strategy = GenerationType.GUID)]
		public string Id{ get; set; } 

		[Column(Name = "FULLNAME")]
		public string Fullname{ get; set; } 

		[Column(Name = "SHORTNAME")]
		public string Shortname{ get; set; } 

		[Column(Name = "SUBJECTID")]
		public string Subjectid{ get; set; } 

		[Column(Name = "PROPERTY")]
		public Decimal? Property{ get; set; } 

		[Column(Name = "LOGO")]
		public string Logo{ get; set; } 

		[Column(Name = "SORT")]
		public Decimal? Sort{ get; set; } 

	 } 
}    

