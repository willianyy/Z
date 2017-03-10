using System;
using System.Collections.Generic; 
using System.Linq;  
using System.Text;
using Easy4net.CustomAttributes;

namespace Entity  
{  
	 [Table(Name = "AGC_SUBJECTINFO")] 
	 public class Agcsubjectinfo
	 { 
		[Column(Name = "ICON")]
		public string Icon{ get; set; } 

		[Column(Name = "SORT")]
		public Decimal? Sort{ get; set; } 

		[Id(Name = "ID", Strategy = GenerationType.GUID)]
		public string Id{ get; set; } 

		[Column(Name = "NAME")]
		public string Name{ get; set; } 

		[Column(Name = "DESCRIPTION")]
		public string Description{ get; set; } 

		[Column(Name = "CODE")]
		public string Code{ get; set; } 

		[Column(Name = "TYPE")]
		public string Type{ get; set; } 

		[Column(Name = "STATUS")]
		public Decimal? Status{ get; set; } 

		[Column(Name = "CREATEUSER")]
		public string Createuser{ get; set; } 

		[Column(Name = "CREATEDATE")]
		public DateTime? Createdate{ get; set; } 

	 } 
}    

