using MatchIt.MatchReplacement.Core.Engine;
using MatchIt.MatchReplacement.Core.Normalization;

namespace MatchIt.MatchReplacement.Core.Stages;

public sealed class NormalizeRecordsStage : IProcessStage
{
    public string Name => "Normalize Records";

    public Task ExecuteAsync(ProcessRunContext context, CancellationToken cancellationToken = default)
    {
        var textNormalizer = new TextNormalizer();
        var postcodeNormalizer = new PostcodeNormalizer(textNormalizer);
        var companyNormalizer = new CompanyNormalizer(textNormalizer);
        var addressNormalizer = new AddressNormalizer(textNormalizer);
        var phoneticEncoder = new PhoneticEncoder(textNormalizer);

        var keyBuilder = new KeyBuilder(
            textNormalizer,
            postcodeNormalizer,
            companyNormalizer,
            addressNormalizer,
            phoneticEncoder);

        Console.WriteLine("Normalizing suppression records...");
        foreach (var suppression in context.Suppressions)
        {
            keyBuilder.Enrich(suppression);
        }

        Console.WriteLine("Normalizing contact records...");
        foreach (var contact in context.Contacts)
        {
            keyBuilder.Enrich(contact);
        }

        return Task.CompletedTask;
    }
}