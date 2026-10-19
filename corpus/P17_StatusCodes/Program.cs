using System.Text;

namespace Corpus;

public static class Program
{
    static string Reason(int code)
    {
        switch (code)
        {
            case 100: return "Continue";
            case 101: return "Switching";
            case 200: return "OK";
            case 201: return "Created";
            case 202: return "Accepted";
            case 204: return "No Content";
            case 301: return "Moved";
            case 302: return "Found";
            case 304: return "Not Modified";
            case 400: return "Bad Request";
            case 401: return "Unauthorized";
            case 403: return "Forbidden";
            case 404: return "Not Found";
            case 418: return "Teapot";
            case 429: return "Too Many";
            case 500: return "Server Error";
            case 502: return "Bad Gateway";
            case 503: return "Unavailable";
            case 1000: return "Custom";
            case 65535: return "Max";
            default: return "Unknown";
        }
    }

    static int Severity(int code)
    {
        switch (code / 100)
        {
            case 1: return 0;
            case 2: return 1;
            case 3: return 2;
            case 4: return 5;
            case 5: return 9;
            default: return -1;
        }
    }

    static bool Retryable(int code)
    {
        switch (code)
        {
            case 408:
            case 429:
            case 500:
            case 502:
            case 503:
            case 504:
                return true;
            default:
                return false;
        }
    }

    static char Letter(int digit)
    {
        switch (digit)
        {
            case 0: return 'z';
            case 1: return 'o';
            case 2: return 't';
            case 3: return 'h';
            case 4: return 'f';
            case 5: return 'v';
            case 6: return 's';
            case 7: return 'e';
            case 8: return 'g';
            case 9: return 'n';
            default: return '?';
        }
    }

    public static string Run(string input)
    {
        var sb = new StringBuilder();
        foreach (string tok in input.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            int code = int.Parse(tok);
            sb.Append(code).Append('=').Append(Reason(code));
            sb.Append('/').Append(Severity(code)).Append(Retryable(code) ? "R" : "-");
            sb.Append(Letter(code % 10)).Append(' ');
        }
        return sb.ToString().TrimEnd();
    }
}
