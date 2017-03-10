//=====================================================================================
// All Rights Reserved , Copyright © Learun 2013
//=====================================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Xml;
using System.IO;

namespace Z.Utilities
{
    public class XMLHelper
    {
        public XMLHelper()
        {
        }

        #region 对象转换
        /// <summary>
        /// Hashtable
        /// </summary>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        public static Hashtable XMLToHashtable(string xmlData)
        {
            DataTable dt = XMLToDataTable(xmlData);
            return DataHelper.DataTableToHashtable(dt);
        }
        /// <summary>
        /// DataTable
        /// </summary>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        public static DataTable XMLToDataTable(string xmlData)
        {
            if (!String.IsNullOrEmpty(xmlData))
            {
                DataSet ds = new DataSet();
                ds.ReadXml(new System.IO.StringReader(xmlData));
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];
            }
            return null;
        }
        /// <summary>
        /// DataSet
        /// </summary>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        public static DataSet XMLToDataSet(string xmlData)
        {
            if (!String.IsNullOrEmpty(xmlData))
            {
                DataSet ds = new DataSet();
                ds.ReadXml(new System.IO.StringReader(xmlData));
                return ds;
            }
            return null;
        }

        /**/
        /// <summary>
        /// 将Xml内容字符串转换成DataSet对象
        /// </summary>
        /// <param >Xml内容字符串</param>
        /// <returns>DataSet对象</returns>
        public static DataSet XmlTitleToDataSet(string xmlStr)
        {
            if (!string.IsNullOrEmpty(xmlStr))
            {
                xmlStr = "<?xml version='1.0' encoding='GB2312'?><hbitem>" + xmlStr + "</hbitem>";
                StringReader StrStream = null;
                XmlTextReader Xmlrdr = null;
                try
                {
                    DataSet ds = new DataSet();
                    //读取字符串中的信息
                    StrStream = new StringReader(xmlStr);
                    //获取StrStream中的数据
                    Xmlrdr = new XmlTextReader(StrStream);
                    //ds获取Xmlrdr中的数据                
                    ds.ReadXml(Xmlrdr);
                    return ds;
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    //释放资源
                    if (Xmlrdr != null)
                    {
                        Xmlrdr.Close();
                        StrStream.Close();
                        StrStream.Dispose();
                    }
                }
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 增、删、改操作
        /// <summary>
        /// 追加节点
        /// </summary>
        /// <param name="filePath">XML文档绝对路径</param>
        /// <param name="xPath">范例: @"Skill/First/SkillItem"</param>
        /// <param name="xmlNode">XmlNode节点</param>
        /// <returns></returns>
        public static bool AppendChild(string filePath, string xPath, XmlNode xmlNode)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                XmlNode xn = doc.SelectSingleNode(xPath);
                XmlNode n = doc.ImportNode(xmlNode, true);
                xn.AppendChild(n);
                doc.Save(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 从XML文档中读取节点追加到另一个XML文档中
        /// </summary>
        /// <param name="filePath">需要读取的XML文档绝对路径</param>
        /// <param name="xPath">范例: @"Skill/First/SkillItem"</param>
        /// <param name="toFilePath">被追加节点的XML文档绝对路径</param>
        /// <param name="toXPath">范例: @"Skill/First/SkillItem"</param>
        /// <returns></returns>
        public static bool AppendChild(string filePath, string xPath, string toFilePath, string toXPath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(toFilePath);
                XmlNode xn = doc.SelectSingleNode(toXPath);

                XmlNodeList xnList = ReadNodes(filePath, xPath);
                if (xnList != null)
                {
                    foreach (XmlElement xe in xnList)
                    {
                        XmlNode n = doc.ImportNode(xe, true);
                        xn.AppendChild(n);
                    }
                    doc.Save(toFilePath);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 修改节点的InnerText的值
        /// </summary>
        /// <param name="filePath">XML文件绝对路径</param>
        /// <param name="xPath">范例: @"Skill/First/SkillItem"</param>
        /// <param name="value">节点的值</param>
        /// <returns></returns>
        public static bool UpdateNodeInnerText(string filePath, string xPath, string value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                XmlNode xn = doc.SelectSingleNode(xPath);
                XmlElement xe = (XmlElement)xn;
                xe.InnerText = value;
                doc.Save(filePath);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 读取XML文档
        /// </summary>
        /// <param name="filePath">XML文件绝对路径</param>
        /// <returns></returns>
        public static XmlDocument LoadXmlDoc(string filePath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                return doc;
            }
            catch
            {
                return null;
            }
        }
        #endregion 增、删、改操作

        #region 扩展方法
        /// <summary>
        /// 读取XML的所有子节点
        /// </summary>
        /// <param name="filePath">XML文件绝对路径</param>
        /// <param name="xPath">范例: @"Skill/First/SkillItem"</param>
        /// <returns></returns>
        public static XmlNodeList ReadNodes(string filePath, string xPath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                XmlNode xn = doc.SelectSingleNode(xPath);
                XmlNodeList xnList = xn.ChildNodes;  //得到该节点的子节点
                return xnList;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时返回该属性值，否则返回串联值</param>
        /// <returns>string</returns>
        /**************************************************
         * 使用示列:
         * XmlHelper.Read(path, "/Node", "")
         * XmlHelper.Read(path, "/Node/Element[@Attribute='Name']", "Attribute")
         ************************************************/
        public static string Read(string path, string node, string attribute)
        {
            string value = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                value = (attribute.Equals("") ? xn.InnerText : xn.Attributes[attribute].Value);
            }
            catch { }
            return value;
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时返回该属性值，否则返回串联值</param>
        /// <returns>string</returns>
        /**************************************************
         * 使用示列:
         * XmlHelper.Read(path, "/Node", "")
         * XmlHelper.Read(path, "/Node/Element[@Attribute='Name']", "Attribute")
         ************************************************/
        public static string ReadXml(XmlDocument xml, string node, string attribute)
        {
            string value = "";
            try
            {
                //XmlDocument doc = new XmlDocument();
                //doc.Load(path);
                XmlNode xn = xml.SelectSingleNode(node);
                value = (attribute.Equals("") ? xn.InnerText : xn.Attributes[attribute].Value);
            }
            catch { }
            return value;
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="element">元素名，非空时插入新元素，否则在该元素中插入属性</param>
        /// <param name="attribute">属性名，非空时插入该元素属性值，否则插入元素值</param>
        /// <param name="value">值</param>
        /**************************************************
         * 使用示列:
         * XmlHelper.Insert(path, "/Node", "Element", "", "Value")
         * XmlHelper.Insert(path, "/Node", "Element", "Attribute", "Value")
         * XmlHelper.Insert(path, "/Node", "", "Attribute", "Value")
         ************************************************/
        public static void Insert(string path, string node, string element, string attribute, string value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                if (element.Equals(""))
                {
                    if (!attribute.Equals(""))
                    {
                        XmlElement xe = (XmlElement)xn;
                        xe.SetAttribute(attribute, value);
                    }
                }
                else
                {
                    XmlElement xe = doc.CreateElement(element);
                    if (attribute.Equals(""))
                        xe.InnerText = value;
                    else
                        xe.SetAttribute(attribute, value);
                    xn.AppendChild(xe);
                }
                doc.Save(path);
            }
            catch { }
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时修改该节点属性值，否则修改节点值</param>
        /// <param name="value">值</param>
        /**************************************************
         * 使用示列:
         * XmlHelper.Insert(path, "/Node", "", "Value")
         * XmlHelper.Insert(path, "/Node", "Attribute", "Value")
         ************************************************/
        public static void Update(string path, string node, string attribute, string value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                if (attribute.Equals(""))
                    xe.InnerText = value;
                else
                    xe.SetAttribute(attribute, value);
                doc.Save(path);
            }
            catch { }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时删除该节点属性值，否则删除节点值</param>
        /**************************************************
         * 使用示列:
         * XmlHelper.Delete(path, "/Node", "")
         * XmlHelper.Delete(path, "/Node", "Attribute")
         ************************************************/
        public static void Delete(string path, string node, string attribute)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                if (attribute.Equals(""))
                    xn.ParentNode.RemoveChild(xn);
                else
                    xe.RemoveAttribute(attribute);
                doc.Save(path);
            }
            catch { }
        }
        #endregion 扩展方法
    }
}
