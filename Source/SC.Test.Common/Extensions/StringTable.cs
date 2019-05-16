using SC.Base.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SC.Test.Common.Extensions
{
    /*
     * This class is used for generating an ascii formatted string table.
     * The dataformat of the table is a List consisting of arrays.
     * Each array represents one row.
     * */
    public class StringTable
    {
        #region ascii chars
        private const char LineHorizontal = '─';
        private const char LineVertical = '│';

        private const char Padding = ' ';
        private const string ColumnHeaderPaddingPre = " [";
        private const string ColumnHeaderPaddingPost = "] ";
        private const string SectionHeaderPadding = "Δ ";
        private const string SubSectionHeaderPadding = "➝ ";
        
        private const char ConnectionTopLeft = '┌';
        private const char ConnectionTop = '┬';
        private const char ConnectionTopRight = '┐';

        private const char ConnectionLeft = '├';
        private const char Connection = '┼';
        private const char ConnectionRight = '┤';

        private const char ConnectionBottomLeft = '└';
        private const char ConnectionBottom = '┴';
        private const char ConnectionBottomRight = '┘';
        #endregion

        public StringTable()
        {
            Table = new List<object[]>();
        }

		private List<object[]> Table { get; }

        private object[] AddPadding(object[] row, bool isHeader = false, bool isSectionHeader = false)
        {
            for (int i = 0; i < row.Length; i++)
            {
                string content = GetString(row[i]);
                if (!string.IsNullOrEmpty(content))
                {
                    if (isSectionHeader && isHeader)
                        row[i] = $"{SectionHeaderPadding}{content}{Padding}";
                    else if (isSectionHeader)
                        row[i] = $"{SubSectionHeaderPadding}{content}{Padding}";
                    else if (isHeader)
                        row[i] = $"{ColumnHeaderPaddingPre}{content}{ColumnHeaderPaddingPost}";
                    else
                        row[i] = $"{Padding}{content}{Padding}";
                }
                else
                    row[i] = string.Empty;

            }
            return row;
        }

		private string GetString(object obj)
		{
			if (obj == null)
				return string.Empty;
			var command = obj as RelayCommand;
			if (command != null)
				return GetRelayCommandString(command);
			
			return (obj as double?)?.ToString("0.#########") ?? obj.ToString();
		}

		private string GetRelayCommandString(RelayCommand command)
		{
			return command.CanExecute(null).ToString();
		}

	    public void AddRow(object[] row)
        {
            row = AddPadding(row);
            Table.Add(row);
        }

        public void AddTitleRow(object[] row)
        {
            row = AddPadding(row, true);
            Table.Add(row);
        }

        public string GetFormattedString(int inherit = 0)
        {
            string tableString = FormatStringTable();
            string newTableString = tableString;

            if(inherit > 0)
            {
                newTableString = string.Empty;

                string inheritString = "";
                for(int i=0; i<inherit; i++)
                    inheritString += "\t";

                foreach (string line in tableString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                    newTableString += inheritString + line + Environment.NewLine;
            }
            
            newTableString = newTableString.TrimEnd('\r', '\n');

            return newTableString;
        }

        private int[] GetMaxCellWidths()
        {
            int maximumCells = 0;
            foreach (Array row in Table)
            {
                if (row.Length > maximumCells)
                    maximumCells = row.Length;
            }

            int[] maximumCellWidths = new int[maximumCells];
            for (int i = 0; i < maximumCellWidths.Length; i++)
                maximumCellWidths[i] = 0;

            foreach (Array row in Table)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    if (row.GetValue(i).ToString().Length > maximumCellWidths[i])
                        maximumCellWidths[i] = row.GetValue(i).ToString().Length;
                }
            }

            return maximumCellWidths;
        }

        private string FormatStringTable()
        {
            StringBuilder formattedTable = new StringBuilder();
            Array nextRow = Table.FirstOrDefault();
            Array previousRow = Table.FirstOrDefault();

            if (Table == null || nextRow == null)
                return string.Empty;

            int[] maximumCellWidths = GetMaxCellWidths();
            for (int i = 0; i < nextRow.Length; i++)
            {
                if (i == 0 && i == nextRow.Length - 1)
                    formattedTable.AppendLine(
                        $"{ConnectionTopLeft}{string.Empty.PadRight(maximumCellWidths[i], LineHorizontal)}{ConnectionTopRight}");
                else if (i == 0)
                    formattedTable.Append(
                        $"{ConnectionTopLeft}{string.Empty.PadRight(maximumCellWidths[i], LineHorizontal)}");
                else if (i == nextRow.Length - 1)
                    formattedTable.AppendLine(
                        $"{ConnectionTop}{string.Empty.PadRight(maximumCellWidths[i], LineHorizontal)}{ConnectionTopRight}");
                else
                    formattedTable.Append(
                        $"{ConnectionTop}{string.Empty.PadRight(maximumCellWidths[i], LineHorizontal)}");
            }

            int rowIndex = 0;
            int lastRowIndex = Table.Count - 1;
            foreach (Array row in Table)
            {
                int cellIndex = 0;
                int lastCellIndex = row.Length - 1;
                foreach (object thisCell in row)
                {
                    string thisValue = thisCell.ToString().PadRight(maximumCellWidths[cellIndex], Padding);

                    if (cellIndex == lastCellIndex)
                        formattedTable.AppendLine($"{LineVertical}{thisValue}{LineVertical}");
                    else
                        formattedTable.Append($"{LineVertical}{thisValue}");

                    cellIndex++;
                }

                previousRow = row;

                if (rowIndex != lastRowIndex)
                {
                    nextRow = Table[rowIndex + 1];

                    int maximumCells = Math.Max(previousRow.Length, nextRow.Length);
                    for (int i = 0; i < maximumCells; i++)
                    {
                        if (i == 0 && i == maximumCells - 1)
                            formattedTable.AppendLine(
                                $"{ConnectionLeft}{string.Empty.PadRight(maximumCellWidths[i], LineHorizontal)}{ConnectionRight}");
                        else if (i == 0)
                            formattedTable.Append(
                                $"{ConnectionLeft}{string.Empty.PadRight(maximumCellWidths[i], LineHorizontal)}");
                        else if (i == maximumCells - 1)
                        {
                            if (i > previousRow.Length)
                                formattedTable.AppendLine(
                                    $"{ConnectionTop}{string.Empty.PadRight(maximumCellWidths[i], LineHorizontal)}{ConnectionTopRight}");
                            else if (i > nextRow.Length)
                                formattedTable.AppendLine(
                                    $"{ConnectionBottom}{string.Empty.PadRight(maximumCellWidths[i], LineHorizontal)}{ConnectionBottomRight}");
                            else if (i > previousRow.Length - 1)
                                formattedTable.AppendLine(
                                    $"{Connection}{string.Empty.PadRight(maximumCellWidths[i], LineHorizontal)}{ConnectionTopRight}");
                            else if (i > nextRow.Length - 1)
                                formattedTable.AppendLine(
                                    $"{Connection}{string.Empty.PadRight(maximumCellWidths[i], LineHorizontal)}{ConnectionBottomRight}");
                            else
                                formattedTable.AppendLine(
                                    $"{Connection}{string.Empty.PadRight(maximumCellWidths[i], LineHorizontal)}{ConnectionRight}");
                        }
                        else
                        {
                            if (i > previousRow.Length)
                                formattedTable.Append(
                                    $"{ConnectionTop}{string.Empty.PadRight(maximumCellWidths[i], LineHorizontal)}");
                            else if (i > nextRow.Length)
                                formattedTable.Append(
                                    $"{ConnectionBottom}{string.Empty.PadRight(maximumCellWidths[i], LineHorizontal)}");
                            else
                                formattedTable.Append(
                                    $"{Connection}{string.Empty.PadRight(maximumCellWidths[i], LineHorizontal)}");
                        }
                    }
                }

                rowIndex++;
            }

			if (previousRow != null)
				for (int i = 0; i < previousRow.Length; i++)
				{
					if (i == 0 && i == previousRow.Length - 1)
						formattedTable.AppendLine(
							$"{ConnectionBottomLeft}{string.Empty.PadRight(maximumCellWidths[i], LineHorizontal)}{ConnectionBottomRight}");
					else if (i == 0)
						formattedTable.Append(
							$"{ConnectionBottomLeft}{string.Empty.PadRight(maximumCellWidths[i], LineHorizontal)}");
					else if (i == previousRow.Length - 1)
						formattedTable.AppendLine(
							$"{ConnectionBottom}{string.Empty.PadRight(maximumCellWidths[i], LineHorizontal)}{ConnectionBottomRight}");
					else
						formattedTable.Append(
							$"{ConnectionBottom}{string.Empty.PadRight(maximumCellWidths[i], LineHorizontal)}");
				}

			return formattedTable.ToString();
        }

    }
}