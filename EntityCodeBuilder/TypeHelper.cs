﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsDemo
{
    public class TypeHelper
    {
        public static string GetType(string type, bool isNull)
        {
            string newType = "string";
            type = type.ToLower();
            switch (type)
            {
                case "varchar":
                case "varchar2":
                case "nvarchar":
                case "char":
                    newType = "string";
                    break;
                case "int":
                case "integer":
                case "bit":
                case "smallint":
                    newType = "int";
                    if (isNull) newType = newType + "?";
                    break;
                case "long":
                case "bigint":
                    newType = "long";
                    if (isNull) newType = newType + "?";
                    break;
                case "date":
                case "datetime":
                case "datetime2":
                case "datetimeoffset":
                    newType = "DateTime";
                    if (isNull) newType = newType + "?";
                    break;
                case "number":
                case "decimal":
                case "money":
                case "numeric":
                    newType = "Decimal";
                    if (isNull) newType = newType + "?";
                    break;
                case "double":
                    newType = "double";
                    if (isNull) newType = newType + "?";
                    break;
                case "float":
                    newType = "float";
                    if (isNull) newType = newType + "?";
                    break;
            }

            return newType;
        }
    }
}
