using System.IO;

namespace GenerateAst
{
    class CustomStreamWriter
    {
        private readonly StreamWriter _streamWriter;
        private int _currentIndent = 0;

        public CustomStreamWriter(string path)
        {
            _streamWriter = new StreamWriter(path);
        }

        public CustomStreamWriter IncrementIndent()
        {
            _currentIndent++;
            return this;
        }

        public CustomStreamWriter DecrementIndent()
        {
            _currentIndent--;
            return this;
        }

        public CustomStreamWriter WriteLine(string text)
        {
            WriteIndent();

            _streamWriter.WriteLine(text);

            return this;
        }

        public void Flush()
        {
            _streamWriter.Flush();
        }

        private void WriteIndent()
        {
            var i = 1;

            while (i <= _currentIndent)
            {
                _streamWriter.Write("    ");
                i++;
            }
        }
    }
}
