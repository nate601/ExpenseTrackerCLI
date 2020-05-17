using System;
using System.IO;
using System.Linq;
using System.Text;

namespace expenseTrackerCli.Reporting
{
    public class Report
    {
        public readonly string reportName;
        private readonly StringBuilder stringBuilder;

        public Report(string _reportName)
        {
            reportName = _reportName;
            stringBuilder = new StringBuilder();
        }

        public void SaveReport()
        {
            FileStream fileStream = File.OpenWrite($"{reportName}.md");
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.Write(stringBuilder.ToString());
            streamWriter.Flush();
            streamWriter.Close();
        }

        public void AddLine(string text)
        {
            _ = stringBuilder.AppendLine(text);
        }

        public void Add(string text)
        {
            _ = stringBuilder.Append(text);
        }

        public void AddHeader(string headerName)
        {
            _ = stringBuilder.Append("# ").AppendLine(headerName);
        }

        public void AddTable(string[] headers, string[] values)
        {
            int numberOfColumns = headers.Length;
            int numberOfRows = values.Length / numberOfColumns;
            Add("|");
            headers.ToList().ForEach(x => Add($"{x}|"));
            AddNewLine();
            Add("|");
            headers.ToList().ForEach(_ => Add("---|"));
            AddNewLine();
            for (int i = 0; i < numberOfRows; i++)
            {
                string[] currentRow = values.Skip(i * numberOfColumns).Take(numberOfColumns).ToArray();
                Add("|");
                foreach(string x in currentRow)
                {
                    Add($"{x}|");
                }
                AddNewLine();
            }
        }

        public void AddBlank()
        {
            _ = stringBuilder.AppendLine();
        }

        public void AddNewLine()
        {
            Add("\n");
        }

        public void DebugPrintToConsole()
        {
            Console.Write(stringBuilder.ToString());
        }
    }
}
