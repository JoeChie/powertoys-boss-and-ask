using System;
namespace Wox.Plugin {
    // Test-only contract shim. Never included in production packages.
    public interface IPlugin { void Init(PluginInitContext context); System.Collections.Generic.List<Result> Query(Query query); }
    public class PluginInitContext {}
    public class Query {public string Search;public string RawQuery;public string ActionKeyword;}
    public class Result {public string Title,SubTitle,QueryTextDisplay,IcoPath;public int Score;public Func<object,bool> Action;}
}
