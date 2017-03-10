using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Easy4net.CustomAttributes;

namespace Entity
{
    [Table(Name = "ILL")]
    public class Ill
    {
        [Id(Name = "KEY_NO", Strategy = 0)]
        public string Keyno { get; set; }

        [Column(Name = "XML")]
        public string Xml { get; set; }

        [Column(Name = "TIT")]
        public string Tit { get; set; }

        [Column(Name = "DEPT")]
        public string Dept { get; set; }

        [Column(Name = "CVOCABLE")]
        public string Cvocable { get; set; }

        [Column(Name = "SYMLINK")]
        public string Symlink { get; set; }

        [Column(Name = "NOSYMLINK")]
        public string Nosymlink { get; set; }

        [Column(Name = "ISMAN")]
        public string Isman { get; set; }

        [Column(Name = "ISWOMAN")]
        public string Iswoman { get; set; }

        [Column(Name = "ISCHILD")]
        public string Ischild { get; set; }

        [Column(Name = "XML2")]
        public string Xml2 { get; set; }

        [Column(Name = "XML3")]
        public string Xml3 { get; set; }

    }
}

