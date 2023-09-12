using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace StarkovTestApp
{
    class Parser
    {
        public List<List<string>> GetTableData(IParser parser, string file)
        {
            var rowList = GetRowList(file);
            return parser.GetTableData(rowList);
        }
        private List<string> GetRowList(string file)
        {
            var substring = file.Split('\n');
            List<string> rowList = substring.ToList();
            //for(var i = 0; i < rowList.Count; i++)
            //{
            //    if (rowList[i].FirstOrDefault() == '\t')
            //    {
            //        Console.WriteLine("Ошибка! Не удалось добавить запись: \"" + rowList[i] + "\"");
            //        rowList.Remove(rowList[i]);
            //        i--;
            //    }
            //}
            return rowList;
        }
    }
    interface IParser
    {
        List<List<string>> GetTableData(List<string> rowList);
    }
    abstract class Purifier
    {
        public string ClearName(string name)
        {
            while (name.IndexOf("  ") != -1) name = name.Replace("  ", " ");
            name = name.ToLower().Trim();
            var splitedName = name.Split(' ');
            for (var i = 0; i < splitedName.Length; i++)
            {
                splitedName[i] = char.ToUpper(splitedName[i][0]) + splitedName[i].Substring(1);
            }
            name = String.Join(' ', splitedName);
            return name;
        }

        public string ClearGeneral(string text)
        {
            if (text.Length < 1)
                return text;
            text = text.ToLower().Trim();
            while (text.IndexOf("  ") != -1) text = text.Replace("  ", " ");
            text = char.ToUpper(text[0]) + text.Substring(1);
            return text;
        }
        public string ClearPhone(string text)
        {
            text = text.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "").Trim();
            text = 7 + text.Substring(1);
            return text;
        }
    }
    class DepartmentParser : Purifier, IParser
    {
        public List<List<string>> GetTableData(List<string> rowList)
        {
            List<List<string>> result = new List<List<string>>();
            foreach(var row in rowList)
            {
                var splitedRow = row.Split('\t');
                var isFullInfo = true;
                if (splitedRow.Length != 4)
                {
                    continue;
                };
                //Название
                if (splitedRow[0] == "")               
                    isFullInfo = false;
                splitedRow[0] = ClearGeneral(splitedRow[0]);                
                //Род. подразделение
                if (splitedRow[1] != "")
                {
                    splitedRow[1] = ClearGeneral(splitedRow[1]);                    
                }
                //Руководитель
                if (splitedRow[2] == "")
                    isFullInfo = false;
                splitedRow[2] = ClearName(splitedRow[2]);
                //Телефон
                if (splitedRow[3] == "")               
                    isFullInfo = false;
                splitedRow[3] = ClearPhone(splitedRow[3]);
                if (!isFullInfo)
                {
                    Console.WriteLine("Ошибка! Не удалось добавить запись: \"" + String.Join(row, " ") + "\"");
                    continue;
                }
                result.Add(splitedRow.ToList<string>());
            }
            if (!result.Any())
            {
                return null;
            }
            return result;
        }
    }
    class EmployeeParser : Purifier, IParser
    {

        public List<List<string>> GetTableData(List<string> rowList)
        {
            List<List<string>> result = new List<List<string>>();
            foreach (var row in rowList)
            {
                var splitedRow = row.Split('\t');
                var isFullInfo = true;
                if (splitedRow.Length != 5)
                {
                    //return null;
                    continue;
                }
                //Подразделение
                if (splitedRow[0] == "")
                    isFullInfo = false;
                splitedRow[0] = ClearGeneral(splitedRow[0]);               
                //Имя
                if (splitedRow[1] == "")
                    isFullInfo = false;
                splitedRow[1] = ClearName(splitedRow[1]);              

                //Логин
                if (splitedRow[2] == "")
                    isFullInfo = false;
                splitedRow[2] = splitedRow[2].Replace(" ", "");

                //Пароль
                if (splitedRow[3] == "")
                    isFullInfo = false;
                splitedRow[3] = splitedRow[3].Replace(" ", "");

                //Должность
                if (splitedRow[4] == "")
                    isFullInfo = false;                
                splitedRow[4] = ClearGeneral(splitedRow[4]);
                if (!isFullInfo)
                {
                    Console.WriteLine("Ошибка! Не удалось добавить запись: \"" + String.Join(row, " ") + "\"");
                    continue;
                }
                result.Add(splitedRow.ToList<string>());
            }
            if (!result.Any())
            {
                return null;
            }
            return result;
        }
    }
    class JobTitleParser : Purifier, IParser
    {
        public List<List<string>> GetTableData(List<string> rowList)
        {
            List<List<string>> result = new List<List<string>>();
            for (var i = 0; i < rowList.Count; i++)
            {
                var row = rowList[i];
                var splitedRow = row.Split('\t');
                if(row.Contains('\t'))
                {
                    return null;
                }
                if (row == "")
                {
                    Console.WriteLine("Ошибка! Не удалось добавить запись: \"" + String.Join(row, " ") + "\"");
                    continue;
                }
                row = ClearGeneral(row);
                result.Add(new List<string>() { row });
            }
            return result;
        }
    }
}
