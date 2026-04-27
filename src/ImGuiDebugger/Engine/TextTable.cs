//namespace Divine.Plugin.Engine;

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;

//internal sealed class TextTable
//{
//    public static readonly char[] FRAME_UNICODE = { '│', '─', '┌', '├', '└', '┬', '┼', '┴', '┐', '┤', '┘' };
//    public static readonly char[] FRAME_COMPAT = { '|', '-', '+', '+', '+', '+', '+', '+', '+', '+', '+' };

//    private string title;
//    private readonly ColDef[] columns;
//    private readonly string paddingLeft;
//    private readonly string paddingRight;
//    private readonly bool framed;
//    private readonly char[] frame;
//    private readonly int? minWidth;
//    private readonly SortedDictionary<int, SortedDictionary<int, object>> data = new();

//    private TextTable(string title, List<ColDef> columns, int paddingLeft, int paddingRight, bool framed, char[] frame, int? minWidth)
//    {
//        this.title = title;
//        this.minWidth = minWidth;
//        this.columns = columns.ToArray();
//        this.paddingLeft = repeat(paddingLeft, ' ');
//        this.paddingRight = repeat(paddingRight, ' ');
//        this.framed = framed;
//        this.frame = frame;
//    }

//    public void setTitle(string title)
//    {
//        this.title = title;
//    }

//    public void clear()
//    {
//        data.Clear();
//    }

//    public void setData(int row, int column, object value)
//    {
//        if (column >= columns.Length)
//        {
//            throw new IndexOutOfRangeException("column must be smaller than " + columns.Length);
//        }

//        if (!data.TryGetValue(row, out var rowMap))
//        {
//            rowMap = new();
//            data[row] = rowMap;
//        }
//        rowMap[column] = value;
//    }

//    public object? getData(int row, int column)
//    {
//        if (column >= columns.Length)
//        {
//            throw new IndexOutOfRangeException("column must be smaller than " + columns.Length);
//        }

//        if (!data.TryGetValue(row, out var rowMap) || !rowMap.TryGetValue(row, out var rowMap2))
//        {
//            return null;
//        }

//        return rowMap2;
//    }

//    public int getColumnCount()
//    {
//        return columns.Length;
//    }

//    public int getRowCount()
//    {
//        return data.Count == 0 ? 0 : data.Keys.Last() + 1;
//    }

//    public void print(TextWriter writer)
//    {
//        int[] widths = calculateColumnWidths();
//        string f = calcFormatString(widths);
//        if (title != null)
//        {
//            if (framed)
//            {
//                writer.WriteLine(calcTitleSeparatorString(widths, 0));
//            }
//            writer.Write(calcTitleFormatString(widths), title);
//            writer.WriteLine();
//            if (framed)
//            {
//                writer.WriteLine(calcTitleSeparatorString(widths, 1));
//            }
//        }
//        else
//        {
//            if (framed)
//            {
//                writer.WriteLine(calcSeparatorString(widths, 0));
//            }
//        }
//        writer.Write(f, getHeaders());
//        writer.WriteLine();
//        if (framed)
//        {
//            writer.WriteLine(calcSeparatorString(widths, 1));
//        }

//        for (int r = 0; r < getRowCount(); r++)
//        {
//            writer.Write(f, getObjects(r));
//            writer.WriteLine();
//            if (framed)
//            {
//                writer.WriteLine(calcSeparatorString(widths, r + 1 == getRowCount() ? 2 : 1));
//            }
//        }
//    }

//    public void print(Stream stream)
//    {
//        print(new StreamWriter(stream));
//        stream.Flush();
//    }

//    public string toString()
//    {
//        StringWriter w = new StringWriter();
//        print(w);
//        return w.ToString();
//    }

//    public string repeat(int n, char ch)
//    {
//        return new string(ch, n);
//    }

//    private string calcFormatString(int[] widths)
//    {
//        StringBuilder buf = new StringBuilder();
//        if (framed)
//        {
//            buf.Append(frame[0]);
//        }
//        for (int i = 0; i < columns.Length; i++)
//        {
//            buf.Append(paddingLeft);
//            buf.Append('%');
//            if (columns[i].alignment == Alignment.LEFT)
//            {
//                buf.Append('-');
//            }
//            buf.Append(widths[i]);
//            buf.Append(columns[i].conversion);
//            buf.Append(paddingRight);
//            if (framed)
//            {
//                buf.Append(frame[0]);
//            }
//        }
//        return buf.ToString();
//    }

//    private String calcTitleFormatString(int[] widths)
//    {
//        StringBuilder buf = new StringBuilder();
//        if (framed)
//        {
//            buf.Append(frame[0]);
//        }
//        buf.Append(paddingLeft);
//        buf.Append('%');
//        buf.Append('-');
//        buf.Append(widths[widths.Length - 1] - (framed ? 2 : 0) - paddingLeft.Length - paddingRight.Length);
//        buf.Append('s');
//        buf.Append(paddingRight);
//        if (framed)
//        {
//            buf.Append(frame[0]);
//        }
//        return buf.ToString();
//    }

//    private String calcSeparatorString(int[] widths, int pos)
//    {
//        StringBuilder buf = new StringBuilder();
//        buf.Append(frame[2 + pos]);
//        for (int i = 0; i < columns.Length; i++)
//        {
//            buf.Append(repeat(paddingLeft.Length + widths[i] + paddingRight.Length, frame[1]));
//            buf.Append(frame[(i + 1 == columns.Length ? 8 : 5) + pos]);
//        }
//        return buf.ToString();
//    }

//    private string calcTitleSeparatorString(int[] widths, int pos)
//    {
//        StringBuilder buf = new StringBuilder();
//        buf.Append(frame[2 + pos]);
//        for (int i = 0; i < columns.Length; i++)
//        {
//            buf.Append(repeat(paddingLeft.Length + widths[i] + paddingRight.Length, frame[1]));
//            if (i + 1 == columns.Length)
//            {
//                buf.Append(frame[8 + pos]);
//            }
//            else
//            {
//                buf.Append(frame[pos == 0 ? 1 : 5]);
//            }
//        }
//        return buf.ToString();
//    }

//    private int[] calculateColumnWidths()
//    {
//        int[] widths = new int[columns.Length + 1];
//        for (int i = 0; i < columns.Length; i++)
//        {
//            widths[i] = columns[i].header.Length;
//        }

//        foreach (var rowMap in data.Values)
//        {
//            foreach (var entry in rowMap)
//            {
//                if (entry.Value != null)
//                {
//                    widths[entry.Key] = Math.Max(widths[entry.Key], convertToString(entry.Key, entry.Value).Length);
//                }
//            }
//        }
//        int complete = framed ? 1 : 0;
//        for (int i = 0; i < columns.Length; i++)
//        {
//            complete += widths[i] + paddingLeft.Length + paddingRight.Length + (framed ? 1 : 0);
//        }

//        int usedMin = Math.Max(minWidth ?? 0, (title != null ? title.Length : 0) + paddingLeft.Length + paddingRight.Length + (framed ? 2 : 0));
//        if (usedMin > complete)
//        {
//            float p = ((float)(usedMin - complete)) / columns.Length;
//            float c = 0.01f;
//            for (int i = 0; i < columns.Length; i++)
//            {
//                c += p;
//                int x = (int)Math.Floor(c);
//                widths[i] += x;
//                c -= x;
//            }
//            complete = usedMin;
//        }
//        widths[columns.Length] = complete;
//        return widths;
//    }

//    private string convertToString(int i, object value)
//    {
//        return string.Format("{0}", value);
//    }

//    private object[] getHeaders()
//    {
//        var result = new object[columns.Length];
//        for (int i = 0; i < columns.Length; i++)
//        {
//            result[i] = columns[i].header;
//        }
//        return result;
//    }

//    private object[] getObjects(int row)
//    {
//        object[] result = new Object[columns.Length];
//        for (int i = 0; i < columns.Length; i++)
//        {
//            object? v = getData(row, i);
//            result[i] = v == null ? "-" : v;
//        }
//        return result;
//    }

//    public enum Alignment
//    {
//        LEFT,
//        RIGHT
//    }

//    private class ColDef
//    {
//        public string header;
//        public Alignment alignment;
//        public string conversion = "s";
//        public ColDef(string header, Alignment alignment)
//        {
//            this.header = header;
//            this.alignment = alignment;
//        }
//    }

//    public class Builder
//    {
//        private string title;
//        private List<ColDef> columns = new();
//        private int paddingLeft = 1;
//        private int paddingRight = 1;
//        private bool framed = true;
//        private char[] frame = FRAME_UNICODE;
//        private int? minWidth = null;
//        public Builder setPadding(int paddingLeft, int paddingRight)
//        {
//            this.paddingLeft = paddingLeft;
//            this.paddingRight = paddingRight;
//            return this;
//        }
//        public Builder setTitle(string title)
//        {
//            this.title = title;
//            return this;
//        }
//        public Builder setFramed(bool framed)
//        {
//            this.framed = framed;
//            return this;
//        }
//        public Builder setFrame(char[] frame)
//        {
//            this.frame = frame;
//            return this;
//        }
//        public Builder setMinWidth(int? minWidth)
//        {
//            this.minWidth = minWidth;
//            return this;
//        }
//        public Builder addColumn()
//        {
//            return addColumn("");
//        }
//        public Builder addColumn(string header)
//        {
//            return addColumn(header, Alignment.LEFT);
//        }
//        public Builder addColumn(string header, Alignment alignment)
//        {
//            columns.Add(new ColDef(header, alignment));
//            return this;
//        }
//        public TextTable build()
//        {
//            return new TextTable(title, columns, paddingLeft, paddingRight, framed, frame, minWidth);
//        }
//    }
//}