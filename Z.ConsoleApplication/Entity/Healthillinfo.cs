using System;
using System.Collections.Generic; 
using System.Linq;  
using System.Text;
using Easy4net.CustomAttributes;

namespace Entity  
{  
	 [Table(Name = "HEALTH_ILLINFO")] 
	 public class Healthillinfo
	 { 
		[Id(Name = "ID", Strategy = GenerationType.GUID)]
		public string Id{ get; set; } 

		[Column(Name = "NAME")]
		public string Name{ get; set; } 

		[Column(Name = "INPUTCODE")]
		public string Inputcode{ get; set; } 

		[Column(Name = "ILLTYPEID")]
		public string Illtypeid{ get; set; } 

		[Column(Name = "ILLLEVEL")]
		public string Illlevel{ get; set; } 

		[Column(Name = "INTRODUCE")]
		public string Introduce{ get; set; } 

		[Column(Name = "OUTBREAK")]
		public string Outbreak{ get; set; } 

		[Column(Name = "CLINICAL")]
		public string Clinical{ get; set; } 

		[Column(Name = "ADVIE")]
		public string Advie{ get; set; } 

		[Column(Name = "PREVISE")]
		public string Previse{ get; set; } 

		[Column(Name = "ISMAN")]
		public Decimal? Isman{ get; set; } 

		[Column(Name = "ISWOMAN")]
		public Decimal? Iswoman{ get; set; } 

		[Column(Name = "ISCHILD")]
		public Decimal? Ischild{ get; set; } 

		[Column(Name = "CREATEUSER")]
		public string Createuser{ get; set; } 

		[Column(Name = "CREATEDATE")]
		public DateTime? Createdate{ get; set; } 

		[Column(Name = "UPDATEUSER")]
		public string Updateuser{ get; set; } 

		[Column(Name = "UPDATEDATE")]
		public DateTime? Updatedate{ get; set; } 

		[Column(Name = "STATUS")]
		public Decimal? Status{ get; set; } 
	 } 
}    

