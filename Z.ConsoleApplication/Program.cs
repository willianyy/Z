using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easy4net.Common;
using Easy4net.CustomAttributes;
using Easy4net.DBUtility;
using Entity;

namespace Z.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Biz biz = new Biz();
            biz.ImportSubjectInfo();
            biz.ImportBaseDepartmentInfo();
            biz.GetIllAndDepartment();
            Console.ReadKey();
        }
    }
}
