using MatchIt.MatchReplacement.Host;

try
{
    var options = RunOptions.FromArgs(args);
    var runner = new HostRunner();
    await runner.RunAsync(options);
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine("Fatal error:");
    Console.Error.WriteLine(ex);
    return 1;
}