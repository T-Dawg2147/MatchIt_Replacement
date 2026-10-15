using MatchIt.MatchReplacement.Core.Models;

namespace MatchIt.MatchReplacement.Core.IO;

public sealed class ContactExportProjection
{
    public string? cid { get; init; }
    public string? accno { get; init; }
    public string? office { get; init; }
    public string? prefix { get; init; }
    public string? forenames { get; init; }
    public string? surname { get; init; }
    public string? job_title { get; init; }
    public string? dept { get; init; }
    public string? company { get; init; }
    public string? address1 { get; init; }
    public string? address2 { get; init; }
    public string? address3 { get; init; }
    public string? address4 { get; init; }
    public string? address5 { get; init; }
    public string? postcode { get; init; }
    public string? email { get; init; }
    public string? ccreated { get; init; }
    public string? lasttypecr { get; init; }
    public string? lastdatecr { get; init; }
    public string? crecency { get; init; }
    public string? bdm { get; init; }
    public string? hierarchy { get; init; }
    public string? rolluploy { get; init; }
    public string? rollupacc { get; init; }
    public string? acttype { get; init; }
    public string? accsupp { get; init; }
    public string? rem { get; init; }

    public static ContactExportProjection From(ContactRecord record)
    {
        return new ContactExportProjection
        {
            cid = record.Cid,
            accno = record.AccNo,
            office = record.Office,
            prefix = record.Prefix,
            forenames = record.Forenames,
            surname = record.Surname,
            job_title = record.JobTitle,
            dept = record.Dept,
            company = record.Company,
            address1 = record.Address1,
            address2 = record.Address2,
            address3 = record.Address3,
            address4 = record.Address4,
            address5 = record.Address5,
            postcode = record.Postcode,
            email = record.Email,
            ccreated = record.CCreated,
            lasttypecr = record.LastTypeCr,
            lastdatecr = record.LastDateCr,
            crecency = record.CRecency,
            bdm = record.Bdm,
            hierarchy = record.Hierarchy,
            rolluploy = record.RollupLoy,
            rollupacc = record.RollupAcc,
            acttype = record.ActType,
            accsupp = record.AccSupp,
            rem = record.Rem
        };
    }
}