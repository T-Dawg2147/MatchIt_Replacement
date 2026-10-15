using MatchIt.MatchReplacement.Core.Matching.ContactDedup;
using MatchIt.MatchReplacement.Core.Matching.SiteDedup;
using MatchIt.MatchReplacement.Core.Matching.Suppression;

namespace MatchIt.MatchReplacement.Core.Diagnostics;

public static class DiagnosticRowFactory
{
    public static SuppressionInternalDuplicateDiagnosticRow From(SuppressionInternalDuplicateMatch match)
    {
        return new SuppressionInternalDuplicateDiagnosticRow
        {
            LeftOther1 = match.Left.Other1,
            LeftCompany = match.Left.Company,
            LeftPostcode = match.Left.Postcode,
            RightOther1 = match.Right.Other1,
            RightCompany = match.Right.Company,
            RightPostcode = match.Right.Postcode,
            Score = match.Score,
            Reason = match.Reason
        };
    }

    public static ContactInternalDuplicateDiagnosticRow From(ContactDedupMatch match)
    {
        return new ContactInternalDuplicateDiagnosticRow
        {
            LeftCid = match.Left.Cid,
            LeftAccNo = match.Left.AccNo,
            LeftForenames = match.Left.Forenames,
            LeftSurname = match.Left.Surname,
            LeftCompany = match.Left.Company,
            LeftPostcode = match.Left.Postcode,
            RightCid = match.Right.Cid,
            RightAccNo = match.Right.AccNo,
            RightForenames = match.Right.Forenames,
            RightSurname = match.Right.Surname,
            RightCompany = match.Right.Company,
            RightPostcode = match.Right.Postcode,
            Score = match.Score,
            Reason = match.Reason
        };
    }

    public static SuppressionOverlapDiagnosticRow From(SuppressionOverlapMatch match)
    {
        return new SuppressionOverlapDiagnosticRow
        {
            ContactCid = match.Contact.Cid,
            ContactAccNo = match.Contact.AccNo,
            ContactCompany = match.Contact.Company,
            ContactPostcode = match.Contact.Postcode,
            SuppressionOther1 = match.Suppression.Other1,
            SuppressionCompany = match.Suppression.Company,
            SuppressionPostcode = match.Suppression.Postcode,
            Score = match.Score,
            Reason = match.Reason
        };
    }

    public static SiteDedupDiagnosticRow From(SiteDedupMatch match)
    {
        return new SiteDedupDiagnosticRow
        {
            LeftCid = match.Left.Cid,
            LeftAccNo = match.Left.AccNo,
            LeftCompany = match.Left.Company,
            LeftPostcode = match.Left.Postcode,
            RightCid = match.Right.Cid,
            RightAccNo = match.Right.AccNo,
            RightCompany = match.Right.Company,
            RightPostcode = match.Right.Postcode,
            Score = match.Score,
            Reason = match.Reason
        };
    }
}