using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easy4net.Common;
using Easy4net.DBUtility;
using Entity;
using Z.ConsoleApplication.Entity;

namespace Z.ConsoleApplication
{
    public class Biz
    {
        public static DBHelper dbHelper = new DBHelper();
        DataTable dt = Z.Utilities.Base.Document.Excel.NPOIHelper.ImportExceltoDt(@"D:\\疾病库科室分类(原始).xlsx", 0, 0, false);

        List<BaseDepartmentSheet> _excelList = new List<BaseDepartmentSheet>();

        public Biz()
        {
            GetExcelInfo();
        }

        private void GetExcelInfo()
        {
            foreach (DataRow dr in dt.Rows)
            {
                BaseDepartmentSheet er = new BaseDepartmentSheet();
                er.DepartmentType = dr[0].ToString();
                er.BaseDepratmentName = dr[1].ToString();
                er.DisDepartmentName = dr[2].ToString();
                _excelList.Add(er);
            }
        }

        #region 学科分类
        /// <summary>
        /// 导入学科分类
        /// </summary>
        public void ImportSubjectInfo()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string result = "";
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    dictionary.Add(dr[0].ToString(), Guid.NewGuid().ToString());
                }
                catch (Exception)
                {
                    continue;
                }
            }

            List<Agcsubjectinfo> listSubjectInfo = new List<Agcsubjectinfo>();
            var en = dbHelper.FindAll<Agcsubjectinfo>();
            int count = 0;
            foreach (KeyValuePair<string, string> keyValuePair in dictionary)
            {
                string id = "10000000-0000-0000-0001-00000000000" + (count + 1).ToString();
                result += "key:" + keyValuePair.Key + "\n";
                result += "value:" + keyValuePair.Value + "\r\n";

                if (en.Any(x => x.Name == keyValuePair.Key))
                {
                    continue;
                }
                listSubjectInfo.Add(new Agcsubjectinfo()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = keyValuePair.Key,
                    Createuser = "90000000-0000-0000-0001-000000000000",
                    Sort = 99,
                    Status = 0,
                    Code = Z.Utilities.PinyinHelper.PinyinString(keyValuePair.Key),
                    Createdate = DateTime.Now,
                    Description = keyValuePair.Key
                });
                count++;
            }
            var rc = dbHelper.Insert<Agcsubjectinfo>(listSubjectInfo);
            Console.WriteLine("添加学科分类完毕");
        }
        #endregion

        #region 标准科室
        /// <summary>
        /// 导入标准科室
        /// </summary>
        public void ImportBaseDepartmentInfo()
        {
            List<Agcbasedepartmentinfo> listBaseDepartmentInfo = new List<Agcbasedepartmentinfo>();
            var e = dbHelper.FindAll<Agcsubjectinfo>();
            var er = dbHelper.FindAll<Agcbasedepartmentinfo>();
            foreach (DataRow dr in dt.Rows)
            {
                if (listBaseDepartmentInfo.Any(x => x.Fullname == dr[1].ToString()))
                {
                    continue;
                }
                if (er.Any(x => x.Fullname == dr[1].ToString()))
                {
                    continue;
                }
                var a = e.FirstOrDefault(x => x.Name == dr[0].ToString());
                if (a == null || string.IsNullOrEmpty(a.Id))
                {
                    continue;
                }
                string subjectId = a.Id;
                listBaseDepartmentInfo.Add(new Agcbasedepartmentinfo()
                {
                    Id = Guid.NewGuid().ToString(),
                    Createdate = DateTime.Now,
                    Createuser = "90000000-0000-0000-0001-000000000000",
                    Fullname = dr[1].ToString(),
                    Shortname = dr[1].ToString(),
                    Sort = 99,
                    Status = 0,
                    Subjectid = subjectId,
                    Updatedate = DateTime.Now,
                    Updateuser = "90000000-0000-0000-0001-000000000000"
                });
            }
            var rc = dbHelper.Insert<Agcbasedepartmentinfo>(listBaseDepartmentInfo);
            Console.WriteLine("添加标准科室完毕");
        }
        #endregion

        #region 疾病与科室

        public void GetIllAndDepartment()
        {
            var illlist = dbHelper.FindAll<Ill>();
            List<Agcbasedepartmentinfo> listBaseDepartmentInfo = dbHelper.FindAll<Agcbasedepartmentinfo>();
            List<Healthillinfo> listHealthillinfos = dbHelper.FindAll<Healthillinfo>();
            int count = 0;
            string result = "";
            int failCount = 0;
            listHealthillinfos.ForEach(t =>
            {
                var disEr = illlist.FirstOrDefault(x => x.Tit == t.Name);
                if (disEr != null)
                {
                    var disDept = _excelList.FirstOrDefault(x => x.DisDepartmentName == disEr.Dept);
                    if (disDept != null)
                    {
                        var baseDept = listBaseDepartmentInfo.FirstOrDefault(x => x.Fullname == disDept.BaseDepratmentName);
                        if (baseDept != null)
                        {
                            count++;
                            Console.WriteLine("=============================" + t.Name + "===================================");
                            Console.WriteLine("学科分类：" + disDept.DepartmentType);
                            Console.WriteLine("原始疾病名称：" + disEr.Tit);
                            Console.WriteLine("入库疾病名称：" + t.Name);
                            Console.WriteLine("原始科室名称：" + disEr.Dept);
                            Console.WriteLine("标准科室名称：" + baseDept.Fullname);
                            Console.WriteLine("入库疾病ID：" + t.Id);
                            Console.WriteLine("标准科室ID：" + baseDept.Id);
                            //编写导入疾病与科室逻辑
                        }
                        else
                        {
                            result += "Excel中" + disDept.BaseDepratmentName + "在数据库中不存在\r\n";
                            failCount++;
                        }
                    }
                    else
                    {
                        result += "原始疾病科室：" + disEr.Dept + "与Excel中的疾病科室无法匹配\r\n";
                        failCount++;
                    }
                }
                else
                {
                    result += "疾病：" + t.Name + "与原始疾病无法匹配\r\n";
                    failCount++;
                }
            });
            Console.WriteLine("可被归类的疾病数量为：" + count);
            Console.WriteLine("未被归类疾病数量为：" + failCount);
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine(result);
        }
        #endregion

        public static void OracleMethod()
        {
            var rc = dbHelper.Save(new Agcsubjectinfo()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "orm测试科室"
            });
            var result = dbHelper.Find<Agcsubjectinfo>(new DbCondition("select * from AGC_BASEDEPARTMENTINFO"));
        }
    }
}
