using System;
using System.IO;

namespace NotScuffed.Strings
{
    public class CodeWriter : IDisposable
    {
        private readonly string _oneTab;
        private readonly TextWriter _writer;
        private int _indent;

        public CodeWriter(TextWriter writer, string oneTab = "    ")
        {
            _writer = writer;
            _oneTab = oneTab;
        }

        public int GetCurrentIndent()
        {
            return _indent;
        }

        public void BeginCase(string value)
        {
            WriteIndentedLine($"case {value}:");
            Indent();
        }

        public void EndCase()
        {
            WriteIndentedLine("break;");
            Unindent();
        }

        public void BeginBlock()
        {
            WriteIndentedLine("{");
            Indent();
        }

        public void EndBlock()
        {
            Unindent();
            WriteIndentedLine("}");
        }

        public void Indent()
        {
            _indent++;
        }

        public void Unindent()
        {
            _indent--;

            if (_indent < 0)
                throw new InvalidOperationException("Negative indent");
        }

        public void WriteLine() => _writer.WriteLine();
        public void AppendLine() => _writer.WriteLine();
        public void WriteLine(string line) => _writer.WriteLine(line);
        public void AppendLine(string line) => _writer.WriteLine(line);
        public void WriteLine<T>(T line) => _writer.WriteLine(line);
        public void AppendLine<T>(T line) => _writer.WriteLine(line);

        public void WriteIndentedLine()
        {
            WriteIndent();
            _writer.WriteLine();
        }

        public void WriteIndentedLine(string line)
        {
            WriteIndent();
            _writer.WriteLine(line);
        }
        
        public void WriteIndentedLine<T>(T line)
        {
            WriteIndent();
            _writer.WriteLine(line);
        }

        public void Write(string text)
        {
            _writer.Write(text);
        }

        public void Append(string text)
        {
            _writer.Write(text);
        }

        public void Write<T>(T o)
        {
            _writer.Write(o);
        }

        public void Append<T>(T o)
        {
            _writer.Write(o);
        }

        public void WriteIndent()
        {
            for (var i = 0; i < _indent; i++)
                _writer.Write(_oneTab);
        }

        public void Dispose()
        {
            _writer?.Dispose();
        }
    }
}