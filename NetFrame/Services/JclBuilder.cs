using System.Collections.Generic;
using System.Text;

namespace NetFrame.Services
{
    public class JclBuilder
    {
        private readonly List<string> _lines = new List<string>();

        public JclBuilder AddJobCard(string jobName, string accountInfo, string programmerName, string jobClass, string msgClass, string msgLevel = "1,1")
        {
            _lines.Add($"//{jobName} JOB {accountInfo},'{programmerName}',CLASS={jobClass},MSGCLASS={msgClass},MSGLEVEL=({msgLevel})");
            return this;
        }

        public JclBuilder AddExecStep(string stepName, string programName, string? parm = null)
        {
            string line = $"//{stepName} EXEC PGM={programName}";
            if (!string.IsNullOrEmpty(parm))
            {
                line += $",PARM='{parm}'";
            }
            _lines.Add(line);
            return this;
        }

        public JclBuilder AddDdStatement(string ddName, string dsn, string disp, string? space = null, string? unit = null, string? dcb = null)
        {
            string line = $"//{ddName} DD DSN={dsn},DISP={disp}";
            if (!string.IsNullOrEmpty(space)) line += $",SPACE={space}";
            if (!string.IsNullOrEmpty(unit)) line += $",UNIT={unit}";
            if (!string.IsNullOrEmpty(dcb)) line += $",DCB=({dcb})";
            _lines.Add(line);
            return this;
        }

        public JclBuilder AddDdinContent(string ddName, List<string> dataLines)
        {
            _lines.Add($"//{ddName} DD *");
            foreach (var dl in dataLines)
            {
                _lines.Add(dl);
            }
            _lines.Add("/*");
            return this;
        }

        public string Build()
        {
            var sb = new StringBuilder();
            foreach (var line in _lines)
            {
                string trimmedLine = line.Length > 80 ? line.Substring(0, 80) : line;
                sb.Append(trimmedLine).Append("\n");
            }
            return sb.ToString();
        }
    }
}
